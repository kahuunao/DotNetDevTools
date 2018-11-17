using DevToolsMessage;

using Newtonsoft.Json;

using NLog;

using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DevToolsConnector
{
    public class DevSocket
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public TcpClient Socket { get; private set; }
        public IDevRequestHandler Handler { get; private set; }

        /// <summary>
        /// Indique si la connexion est établie
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return Socket != null && Socket.Connected;
            }
        }

        private NetworkStream _stream;

        public DevSocket(IDevRequestHandler pHandler)
        {
            Handler = pHandler;
        }

        /// <summary>
        /// Ouvre une connexion vers un serveur distant
        /// </summary>
        /// <param name="pRemote"></param>
        /// <returns></returns>
        public async Task<bool> Connect(Uri pRemote)
        {
            // Fermeture des précédents connexions
            Stop();

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
            Stop();

            LOGGER.Debug("Utilisation de la conexion provenant de {0}", socket?.Client?.RemoteEndPoint);
            SetSocket(socket);
        }

        public Task Send()
        {
            return Task.CompletedTask;
        }

        public void Stop()
        {
            try
            {
                if (_stream != null)
                {
                    LOGGER.Debug("Fermeture du flux de données");
                    _stream.Close();
                    _stream.Dispose();
                }
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
            if (IsConnected)
            {
                Socket = pSocket;
                _stream = Socket.GetStream();
                Listen().RunSafe();
                AutoPing().RunSafe();
            }
        }

        /// <summary>
        /// Ecoute de la socket
        /// </summary>
        /// <returns></returns>
        private async Task Listen()
        {
            byte[] datas = new byte[Socket.ReceiveBufferSize];
            int byteRead = 0;
            string responseData = string.Empty;

            try
            {
                LOGGER.Debug("Prêt à réceptionner les données depuis {0}", Socket?.Client?.RemoteEndPoint);
                while (IsConnected)
                {
                    responseData = string.Empty;
                    byteRead = await _stream.ReadAsync(datas, 0, datas.Length);
                    if (byteRead > 0)
                    {
                        responseData = System.Text.Encoding.ASCII.GetString(datas, 0, byteRead);
                        LOGGER.Debug("Received: {0}", responseData);
                        OnMessage(responseData);
                    }
                }
            }
            catch(TaskCanceledException)
            {
                // Tâche arrêté.
                Stop();
            }
            catch(Exception e)
            {
                LOGGER.Error(e, "Une erreur est survenue durant la lecture des données reçues");
                Stop(); // On arrêt cette socket par précaution
            }
        }

        private void OnMessage(string pMessage)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<DevRequest>(pMessage);
                Handler?.HandleRequest(this, request);
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Invalide message: {0}", pMessage);
            }
        }

        private async Task AutoPing()
        {
            while (Socket != null && Socket.Connected)
            {
                try
                {
                    Socket.Client.Send(new byte[0]);
                    await Task.Delay(500);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
