using Newtonsoft.Json;

namespace DevToolsConnector.Impl
{
    public class NewtonsoftSerializer : IDevMessageSerializer
    {
        public T DeserializeObject<T>(string pData)
        {
            var request = JsonConvert.DeserializeObject<T>(pData);
            return request;
        }

        public string SerializeObject(object pData)
        {
            var request = JsonConvert.SerializeObject(pData);
            return request;
        }
    }
}
