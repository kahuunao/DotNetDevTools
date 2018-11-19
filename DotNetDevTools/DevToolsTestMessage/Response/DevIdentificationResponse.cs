using DevToolsMessage;

namespace DevToolsTestMessage.Response
{
    public class DevIdentificationResponse : DevResponse, IDevResponse
    {
        public string AppName { get; set; }
    }
}
