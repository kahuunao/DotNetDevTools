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
            var serializer = new NewtonsoftSerializer();
            var server = new DevToolServer(new DevSocketFactory(serializer));
            server.RegisterListener(EnumDevMessageType.GET_FILE_CONFIG, OnFileConfigRequested);
            server.Bound();
            LOGGER.Debug("Enter to stop console.");
            Console.ReadLine();
            server.UnRegisterListener(EnumDevMessageType.GET_FILE_CONFIG, OnFileConfigRequested);
            server.Close();
        }

        private static async void OnFileConfigRequested(IDevSocket pSocket, DevMessage pMessage)
        {
            await pSocket.RespondAt(pMessage, new DevResponse { Toto = "Not found" });
        }
    }
}
