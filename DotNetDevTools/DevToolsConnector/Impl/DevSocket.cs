using DevToolsMessage;

using NLog;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DevToolsConnector.Impl
{
    public class DevSocket : IDevSocket
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public event EventHandler<DevMessageReceivedEventArg> OnMessageReceived;
        public event EventHandler OnConnectionChanged;

        public TcpClient Socket { get; private set; }
        public IDevMessageSerializer Serializer { get; private set; }

        private bool _isConnected;
        /// <summary>
        /// Indique si la connexion est établie
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnConnectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private NetworkStream _stream;

        public DevSocket(IDevMessageSerializer pSerializer)
        {
            Serializer = pSerializer;
        }

        /// <summary>
        /// Ouvre une connexion vers un serveur distant
        /// </summary>
        /// <param name="pRemote"></param>
        /// <returns></returns>
        public async Task<bool> Connect(Uri pRemote)
        {
            // Fermeture des précédents connexions
            Close();

            try
            {
                var newSocket = new TcpClient();
                LOGGER.Debug("Connexion vers le serveur {0}", pRemote);
                await newSocket.ConnectAsync(pRemote.Host, pRemote.Port);
                LOGGER.Debug("Connected: {0}", newSocket.Connected);
                if (newSocket.Connected)
                {
                    SetSocket(newSocket);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Erreur durant l'ouverture de la socket vers l'uri {0}", pRemote);
            }
            return Socket != null && Socket.Connected;
        }

        /// <summary>
        /// Utilise une socket ouverte depuis l'exterieur
        /// </summary>
        /// <param name="socket"></param>
        public void UseConnectedSocket(TcpClient socket)
        {
            // Fermeture des précédents connexions
            Close();

            LOGGER.Debug("Utilisation de la conexion provenant de {0}", socket?.Client?.RemoteEndPoint);
            SetSocket(socket);
        }

        public void Close()
        {
            IsConnected = Socket != null && Socket.Connected;

            try
            {
                if (_stream != null)
                {
                    LOGGER.Debug("Fermeture du flux de données");
                    _stream.Close();
                    _stream.Dispose();
                }
            }
            catch (ObjectDisposedException)
            {
                // Object déjà détruit. Cela nous va aussi.
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Une erreur est survenue durant la fermeture du flux réseau");
            }
            finally
            {
                _stream = null;
            }

            try
            {
                if (Socket != null)
                {
                    LOGGER.Debug("Fermeture de la connexion avec {0}", Socket?.Client?.RemoteEndPoint);
                    Socket.Close();
                    Socket.Dispose();
                }
            }
            catch (ObjectDisposedException)
            {
                // Object déjà détruit. Cela nous va aussi.
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Une erreur est survenue durant la fermeture de la socket");
            }
            finally
            {
                Socket = null;
            }
        }

        private void SetSocket(TcpClient pSocket)
        {
            IsConnected = pSocket != null && pSocket.Connected;
            if (IsConnected)
            {
                Socket = pSocket;
                _stream = Socket.GetStream();
                Listen().RunSafe();
                AutoPing().RunSafe();
            }
        }

        public async Task Send(DevMessage pRequest)
        {
            if (pRequest == null)
            {
                return;
            }

            if (Socket == null || !Socket.Connected)
            {
                IsConnected = false;
                LOGGER.Warn("Impossible d'envoyer de message. Socket non connecté");
                return;
            }

            var data = Serializer.SerializeObject(pRequest);
            if (data != null)
            {
                byte[] rawData = Encoding.ASCII.GetBytes(data);
                byte[] size = BitConverter.GetBytes(rawData.Length);

                try
                {
                    await _stream.WriteAsync(size, 0, size.Length);
                    await _stream.WriteAsync(rawData, 0, rawData.Length);
                    await _stream.FlushAsync();
                    LOGGER.Debug("Message de {0} bytes envoyé vers {1}. Message: {2}", rawData.Length, Socket.Client.RemoteEndPoint, data);
                }
                catch (Exception e)
                {
                    if (e.InnerException is SocketException socketEx)
                    {
                        LOGGER.Error(socketEx, "Une erreur réseau est survenue durant l'envoi des données");
                    }
                    else
                    {
                        LOGGER.Error(e, "Une erreur est survenu durant l'envoi de données");
                    }
                    Close(); // On arrêt cette socket par précaution
                }
            }
        }

        public async Task RespondAt(DevMessage pRequest, DevResponse pResponse)
        {
            if (pRequest != null && pRequest.Id != Guid.Empty)
            {
                pRequest.Response = pResponse;
                await Send(pRequest);
            }
        }

        /// <summary>
        /// Ecoute de la socket
        /// </summary>
        /// <returns></returns>
        private async Task Listen()
        {
            byte[] datas = new byte[Socket.ReceiveBufferSize];
            try
            {
                LOGGER.Debug("Prêt à réceptionner les données depuis {0}", Socket?.Client?.RemoteEndPoint);
                while (Socket != null && Socket.Connected)
                {
                    await Read(datas);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is SocketException socketEx)
                {
                    LOGGER.Error(socketEx, "Une erreur réseau est survenue");
                }
                else
                {
                    LOGGER.Error(e, "Une erreur est survenue durant la lecture des données reçues");
                }
                Close(); // On arrêt cette socket par précaution
            }
        }

        /// <summary>
        /// Lecture du prochain message
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private async Task Read(byte[] buffer)
        {
            int? size = null;
            string responseData = string.Empty;

            // Récupération de la taille du prochain packet
            size = await ReadNextMessageLength();
            // Lecture du message
            if (size != null && size > 0)
            {
                responseData = await ReadMessage(buffer, size.Value);
            }
            // Traitement du message
            if (responseData != null)
            {
                OnMessage(responseData);
            }
        }

        /// <summary>
        /// Lecture de la taille du prochain message
        /// </summary>
        /// <returns></returns>
        private async Task<int?> ReadNextMessageLength()
        {
            int? size = null;
            byte[] sizeDatas = new byte[4];
            int byteRead = await _stream.ReadAsync(sizeDatas, 0, sizeDatas.Length);
            if (byteRead > 0)
            {
                size = BitConverter.ToInt32(sizeDatas, 0);
            }

            LOGGER.Debug("Taille du prochain message: {0}", size);
            return size;
        }

        /// <summary>
        /// Lecture du message
        /// </summary>
        /// <param name="pBuffer">Buffer de lecture</param>
        /// <param name="pSize">Taille totale du message à lire</param>
        /// <returns>Message reçu</returns>
        private async Task<string> ReadMessage(byte[] pBuffer, int pSize)
        {
            if (pSize <= 0 || pBuffer == null)
            {
                return string.Empty;
            }

            string responseData = string.Empty;
            int byteRead = 0;
            int byteRemaining = pSize;

            while (byteRemaining > 0)
            {
                byteRead = await _stream.ReadAsync(pBuffer, 0, pBuffer.Length);
                if (byteRead > 0)
                {
                    responseData += Encoding.ASCII.GetString(pBuffer, 0, byteRead);
                }
                byteRemaining -= byteRead;
                LOGGER.Debug("Lecture de {0} bytes, Reste {1}", byteRead, byteRemaining);
            }

            LOGGER.Debug("Message reçue: {0}", responseData);
            return responseData;
        }

        private void OnMessage(string pMessage)
        {
            try
            {
                DevMessage messageRead = Serializer.DeserializeObject<DevMessage>(pMessage);
                OnMessageReceived?.Invoke(this, new DevMessageReceivedEventArg(messageRead));
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Message invalide: {0}", pMessage);
            }
        }

        /// <summary>
        /// Ping régulièrement la connexion pour vérifier l'état de celle-ci
        /// </summary>
        /// <returns></returns>
        private async Task AutoPing()
        {
            while (Socket != null && Socket.Connected)
            {
                try
                {
                    await _stream.WriteAsync(new byte[0], 0, 0);
                    await _stream.FlushAsync();
                    await Task.Delay(500);
                }
                catch (Exception e)
                {
                    if (e.InnerException is SocketException socketEx)
                    {
                        LOGGER.Error(socketEx, "Une erreur réseau est survenue");
                    }
                    IsConnected = Socket != null && Socket.Connected;
                }
            }
        }
    }
}
