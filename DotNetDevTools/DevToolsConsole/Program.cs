using DevToolsConnector.Common;
using DevToolsConnector.Inspected;

using DevToolsMessage;
using DevToolsMessage.Response;

using NLog;
using NLog.Config;
using NLog.Targets;

using System;
using System.Threading.Tasks;

namespace DevToolsConsole
{
    class Program
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        private static DevToolTarget _targetOuput = new DevToolTarget();
        private static bool _isAdded;

        static void Main(string[] args)
        {
            //Target.Register<DevToolTarget>("DevTool");
            NullTarget blackhole = new NullTarget();
            LoggingConfiguration conf = LogManager.Configuration;

            // FIXME Evite un bouclage des logs internes avec leurs envoies. Boucle infini ...
            conf.AddRuleForAllLevels(blackhole, "DevToolsConnector.*", true);
            conf.AddRuleForAllLevels(blackhole, "DevToolsMessage.*", true);
            conf.AddTarget("DevTool", _targetOuput);
            conf.AddRuleForAllLevels(_targetOuput);
            LogManager.Configuration = conf;

            var serializer = new NewtonsoftSerializer();
            var server = new DevToolServer(new DevSocketFactory(serializer));
            server.Bound();
            server.RegisterListener(EnumDevMessageType.IDENTIFICATION, IdentificationRequestHandler);
            server.RegisterListener(EnumDevMessageType.GET_LOG_CONFIG, OnGetLogConfig);
            server.RegisterListener(EnumDevMessageType.SET_LOG_CONFIG, OnSetLogConfig);

            LOGGER.Debug("Enter to stop console.");
            Console.ReadLine();

            server.UnRegisterListener(EnumDevMessageType.IDENTIFICATION, IdentificationRequestHandler);
            server.UnRegisterListener(EnumDevMessageType.GET_LOG_CONFIG, OnGetLogConfig);
            server.UnRegisterListener(EnumDevMessageType.SET_LOG_CONFIG, OnSetLogConfig);
            server.Close();
        }

        private static void IdentificationRequestHandler(IDevSocket pSocket, DevMessage pMessage)
        {
            if (pMessage.IsRequest())
            {
                pSocket.RespondAt(pMessage, new DevResponse
                {
                    Identification = new DevIdentificationResponse
                    {
                        AppName = AppDomain.CurrentDomain.FriendlyName
                    }
                });
            }
        }

        private static async void OnGetLogConfig(IDevSocket pSocket, DevMessage pMessage)
        {
            if (pMessage.IsRequest())
            {
                await pSocket.RespondAt(pMessage, new DevResponse());
            }
        }

        private static async void OnSetLogConfig(IDevSocket pSocket, DevMessage pMessage)
        {
            if (pMessage.IsRequest())
            {
                await pSocket.RespondAt(pMessage, new DevResponse());
                _targetOuput.Socket = pSocket;
                CreateRandomLogs().RunSafe();
            }
        }

        private static async Task CreateRandomLogs()
        {
            Random rand = new Random();
            int i = 0;
            while (true)
            {
                await Task.Delay(500);
                LOGGER.Error("Coucou, ceci est le log n° {0}", i);
                i++;
            }
        }
    }
}
