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
            server.RegisterListener("IDENTIFICATION", IdentificationRequestHandler);
            server.RegisterListener("GET_LOG_CONFIG", OnGetLogConfig);
            server.RegisterListener("SET_LOG_CONFIG", OnSetLogConfig);

            LOGGER.Debug("Enter to stop console.");
            Console.ReadLine();

            server.UnRegisterListener("IDENTIFICATION", IdentificationRequestHandler);
            server.UnRegisterListener("GET_LOG_CONFIG", OnGetLogConfig);
            server.UnRegisterListener("SET_LOG_CONFIG", OnSetLogConfig);
            server.Close();
        }

        private static void IdentificationRequestHandler(IDevSocket pSocket, IDevMessage pMessage)
        {
            if (pMessage is IDevRequest)
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

        private static async void OnGetLogConfig(IDevSocket pSocket, IDevMessage pMessage)
        {
            if (pMessage is IDevRequest)
            {
                await pSocket.RespondAt(pMessage, new DevResponse());
            }
        }

        private static async void OnSetLogConfig(IDevSocket pSocket, IDevMessage pMessage)
        {
            if (pMessage is IDevRequest)
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
