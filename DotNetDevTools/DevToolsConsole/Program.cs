using DevToolsConnector;
using DevToolsConnector.Impl;
using DevToolsMessage;
using DevToolsMessage.Request;
using NLog;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevToolsConsole
{
    class Program
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var serializer = new NewtonsoftSerializer();
            var server = new DevToolServer(new DevSocketFactory(serializer));
            server.RegisterListener(EnumDevMessageType.GET_LOG_CONFIG, OnGetLogConfig);
            server.RegisterListener(EnumDevMessageType.SET_LOG_CONFIG, OnSetLogConfig);
            server.Bound();
            LOGGER.Debug("Enter to stop console.");
            Console.ReadLine();
            server.UnRegisterListener(EnumDevMessageType.GET_LOG_CONFIG, OnGetLogConfig);
            server.UnRegisterListener(EnumDevMessageType.SET_LOG_CONFIG, OnSetLogConfig);
            server.Close();
        }

        private static async void OnGetLogConfig(IDevSocket pSocket, DevMessage pMessage)
        {
            await pSocket.RespondAt(pMessage, new DevResponse());   
        }

        private static async void OnSetLogConfig(IDevSocket pSocket, DevMessage pMessage)
        {
            await pSocket.RespondAt(pMessage, new DevResponse());
            SendLog(pSocket).RunSafe();
        }

        private static async Task SendLog(IDevSocket pSocket)
        {
            while (pSocket != null && pSocket.IsConnected)
            {
                await pSocket.Send(new DevMessage
                {
                    RequestType = EnumDevMessageType.LOG_LINE,
                    Request = new DevRequest
                    {
                        LogLine = new List<DevLogLine>
                        {
                            new DevLogLine
                            {
                                Date = DateTime.Now,
                                Level = EnumLogLevel.INFO,
                                LoggerName = "Toto",
                                Message = "oups"
                            }
                        }
                    }
                });
                await Task.Delay(500);
            }
        }
    }
}
