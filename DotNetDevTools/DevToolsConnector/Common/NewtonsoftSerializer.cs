using DevToolsMessage;
using Newtonsoft.Json;

namespace DevToolsConnector.Common
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

        public IDevMessage DeserializeObject(string pData)
        {
            // FIXME ne marchera pas !!
            var request = JsonConvert.DeserializeObject(pData, _setting);
            return request as IDevMessage;
        }

        public string SerializeObject(IDevMessage pData)
        {
            var request = JsonConvert.SerializeObject(pData, _setting);
            return request;
        }
    }
}
