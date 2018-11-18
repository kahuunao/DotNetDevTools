using DevToolsConnector;
using DevToolsConnector.Impl;

using DevToolsMessage;

using NLog;

using System;

namespace DevToolsConsole
{
    class Program
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var handler = new Handler();
            var serializer = new NewtonsoftSerializer();

            var server = new DevToolServer(new DevSocketFactory(handler, serializer));
            server.Init();
            LOGGER.Debug("Enter to stop console.");
            Console.ReadLine();
            server.Close();
        }
    }

    class Handler : IDevRequestHandler
    {
        public void HandleRequest(IDevSocket pSocket, DevRequest pRequest)
        {
            if (pRequest.RequestType == EnumDevRequestType.GET_FILE_CONFIG)
            {
                pRequest.Response = new DevResponse { Toto = "Merde" };
                pSocket.Send(pRequest);
            }
        }
    }
}
