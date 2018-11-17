using DevToolsConnector;
using DevToolsMessage;
using Fleck;
using NLog;
using System;

namespace DevToolsConsole
{
    class Program
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var server = new DevToolServer();
            server.Init(new Handler());
            LOGGER.Debug("Enter to stop console.");
            Console.ReadLine();
            server.Close();
        }
    }

    class Handler : IDevRequestHandler
    {
        public void HandleRequest(DevSocket pSocket, DevRequest pRequest)
        {
            // DO NOTHING
        }
    }
}
