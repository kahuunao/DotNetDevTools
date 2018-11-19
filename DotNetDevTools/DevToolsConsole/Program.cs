using DevToolsConnector.Common;
using DevToolsConnector.Inspected;
using DevToolsConnector.Serializer.JSON;

using DevToolsMessage;

using DevToolsTestMessage.Request;
using DevToolsTestMessage.Response;

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
            server.RegisterListener<DevIdentificationRequest>(IdentificationRequestHandler);
            server.RegisterListener<DevStartSendLogsRequest>(OnSetLogConfig);

            LOGGER.Debug("Enter to stop console.");
            Console.ReadLine();

            server.UnRegisterListener<DevIdentificationRequest>(IdentificationRequestHandler);
            server.UnRegisterListener<DevStartSendLogsRequest>(OnSetLogConfig);
            server.Close();
        }

        private static void IdentificationRequestHandler(IDevSocket pSocket, IDevMessage pMessage)
        {
            if (pMessage is IDevRequest request)
            {
                pSocket.RespondAt(request, new DevIdentificationResponse
                {
                    AppName = AppDomain.CurrentDomain.FriendlyName
                });
            }
        }

        private static async void OnSetLogConfig(IDevSocket pSocket, IDevMessage pMessage)
        {
            if (pMessage is IDevRequest request)
            {
                await pSocket.RespondAt(request);
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
