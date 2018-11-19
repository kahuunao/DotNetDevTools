using DevToolsMessage;

namespace DevToolsConnector.Common
{
    public interface IDevMessageSerializer
    {
        IDevMessage DeserializeObject(string pMessage);
        string SerializeObject(IDevMessage pData);
    }
}
