using DevToolsConnector.Common;

using DevToolsTestMessage;
using DevToolsTestMessage.Request;

using NLog;
using NLog.Common;
using NLog.Targets;

using System.Collections.Generic;

namespace DevToolsConsole
{
    [Target("DevTool")]
    public class DevToolTarget : TargetWithLayout
    {
        public IDevSocket Socket { get; set; }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            if (Socket == null)
            {
                return;
            }

            var log = ParseLog(logEvent);
            var logs = new List<Log>
            {
                log
            };
            SendMessage(logs);
        }

        protected override void Write(IList<AsyncLogEventInfo> logEvents)
        {
            if (Socket == null)
            {
                return;
            }

            var logs = new List<AsyncLogEventInfo>(logEvents).ConvertAll((d) => ParseLog(d));
            SendMessage(logs);
        }

        private Log ParseLog(AsyncLogEventInfo pLogEvent)
        {
            var nlogData = pLogEvent.LogEvent;

            //string logMessage = this.Layout.Render(nlogData);
            return new Log
            {
                Date = nlogData.TimeStamp,
                Level = ParseLevel(nlogData.Level),
                Exception = nlogData.Exception,
                LoggerName = nlogData.LoggerName,
                Message = nlogData.FormattedMessage
            };
        }

        private EnumLogLevel ParseLevel(LogLevel pLogLevel)
        {
            return EnumLogLevel.DEBUG; // TODO
        }

        private void SendMessage(List<Log> pData)
        {
            var s = Socket;
            s?.Send(new DevLogsRequest
            {
                Logs = pData
            });
        }
    }
}
