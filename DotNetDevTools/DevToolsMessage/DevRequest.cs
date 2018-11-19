using DevToolsMessage.Request;

using System.Collections.Generic;

namespace DevToolsMessage
{
    public class DevRequest : DevMessage, IDevRequest
    {
        public DevIdentificationRequest Identification { get; set; }
        public List<DevLogLine> LogLine { get; set; }
    }
}
