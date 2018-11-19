using DevToolsConnector.Common;

using DevToolsMessage;
using DevToolsMessage.Request;

using NLog;
using NLog.Common;
using NLog.Targets;

using System.Collections.Generic;

namespace DevToolsConnector.Inspected
{
    [Target("DevTool")]
    public class DevToolTarget : TargetWithLayout
    {
        public IDevSocket Socket { get; set; }

        public DevToolTarget()
        {
            //Name = "DevTool";
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            if (Socket == null)
            {
                return;
            }

            var log = ParseLog(logEvent);
            var logs = new List<DevLogLine>
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

        private DevLogLine ParseLog(AsyncLogEventInfo pLogEvent)
        {
            var nlogData = pLogEvent.LogEvent;

            string logMessage = this.Layout.Render(nlogData);
            return new DevLogLine
            {
                Date = nlogData.TimeStamp,
                Level = ParseLevel(nlogData.Level),
                Exception = nlogData.Exception,
                LoggerName = nlogData.LoggerName,
                Message = logMessage
            };
        }

        private EnumLogLevel ParseLevel(LogLevel pLogLevel)
        {
            return EnumLogLevel.DEBUG; // TODO
        }

        private void SendMessage(List<DevLogLine> pData)
        {
            var s = Socket;
            s?.Send(new DevRequest
            {
                Type = "LOG_LINE", // FIXME
                LogLine = pData
            });
        }
    }
}
