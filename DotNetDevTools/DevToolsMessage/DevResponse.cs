using DevToolsMessage.Response;

namespace DevToolsMessage
{
    public class DevResponse : DevMessage, IDevResponse
    {
        public bool IsHandled { get; set; }
        public DevIdentificationResponse Identification { get; set; }
    }
}
