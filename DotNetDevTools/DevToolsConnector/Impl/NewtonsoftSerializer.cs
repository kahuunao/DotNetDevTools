using Newtonsoft.Json;

namespace DevToolsConnector.Impl
{
    public class NewtonsoftSerializer : IDevMessageSerializer
    {
        private readonly JsonSerializerSettings _setting;

        public NewtonsoftSerializer()
        {
            _setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };
        }

        public T DeserializeObject<T>(string pData)
        {
            var request = JsonConvert.DeserializeObject<T>(pData, _setting);
            return request;
        }

        public string SerializeObject(object pData)
        {
            var request = JsonConvert.SerializeObject(pData, _setting);
            return request;
        }
    }
}
