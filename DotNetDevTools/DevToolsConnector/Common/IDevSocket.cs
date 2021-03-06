﻿using DevToolsMessage;

using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DevToolsConnector.Common
{
    public interface IDevSocket
    {
        event EventHandler OnConnectionChanged;
        /// <summary>
        /// Réception d'un message <see cref="IDevMessage"/>
        /// </summary>
        event EventHandler<DevMessageReceivedEventArg> OnMessageReceived;
        /// <summary>
        /// La connexion est-elle établie
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// Utilise une socket déjà créée
        /// </summary>
        /// <param name="socket"></param>
        void UseConnectedSocket(TcpClient socket);
        /// <summary>
        /// Ouvre une connexion
        /// </summary>
        /// <param name="pRemote"></param>
        /// <returns></returns>
        Task<bool> Connect(Uri pRemote);
        /// <summary>
        /// Déconnection et libère toutes les ressources associées
        /// </summary>
        void Close();
        /// <summary>
        /// Envoie le message <see cref="IDevMessage"/>
        /// </summary>
        /// <param name="pMessage"></param>
        /// <returns></returns>
        Task Send(IDevMessage pMessage);
        /// <summary>
        /// Envoie la réponse à la requête
        /// </summary>
        /// <param name="pRequest">Requête reçue</param>
        /// <param name="pResponse">Réponse à celle-ci</param>
        /// <param name="pIsHandle">Indique si la la requête a été interprété</param>
        /// <returns></returns>
        Task RespondAt(IDevRequest pRequest, IDevResponse pResponse = null, bool pIsHandle = true);
    }
}