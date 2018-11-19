using DevToolsMessage;

namespace DevToolsConnector.Serializer
{
    public interface IDevMessageSerializer
    {
        IDevMessage DeserializeObject(string pMessage);
        string SerializeObject(IDevMessage pData);
    }
}
