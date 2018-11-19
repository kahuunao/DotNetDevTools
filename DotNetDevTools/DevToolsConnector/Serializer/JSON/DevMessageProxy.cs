using DevToolsMessage;

namespace DevToolsConnector.Serializer.JSON
{
    public class DevMessageProxy
    {
        public IDevMessage Message { get; set; }

        public DevMessageProxy(IDevMessage pMessage)
        {
            Message = pMessage;
        }
    }
}
