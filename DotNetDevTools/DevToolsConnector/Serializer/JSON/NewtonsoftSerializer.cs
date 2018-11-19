using DevToolsMessage;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DevToolsConnector.Serializer.JSON
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
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        public IDevMessage DeserializeObject(string pData)
        {
            DevMessageProxy request = JsonConvert.DeserializeObject<DevMessageProxy>(pData, _setting);
            return request?.Message;
        }

        public string SerializeObject(IDevMessage pData)
        {
            var request = JsonConvert.SerializeObject(new DevMessageProxy(pData), _setting);
            return request;
        }
    }
}
