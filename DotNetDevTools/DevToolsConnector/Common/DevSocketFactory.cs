namespace DevToolsConnector.Common
{
    public class DevSocketFactory : IDevSocketFactory
    {
        private readonly IDevMessageSerializer _serializer;

        public DevSocketFactory(IDevMessageSerializer pSerializer)
        {
            _serializer = pSerializer;
        }

        public IDevSocket BuildSocket()
        {
            return new DevSocket(_serializer);
        }
    }
}
