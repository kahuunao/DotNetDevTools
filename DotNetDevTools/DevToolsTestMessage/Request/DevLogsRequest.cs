using DevToolsMessage;
using System;
using System.Collections.Generic;

namespace DevToolsTestMessage.Request
{
    public class DevLogsRequest : DevMessage, IDevRequest
    {
        public List<Log> Logs { get; set; }
    }
}
