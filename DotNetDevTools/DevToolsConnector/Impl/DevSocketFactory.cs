namespace DevToolsConnector.Impl
{
    public class DevSocketFactory : IDevSocketFactory
    {
        private readonly IDevRequestHandler _handler;
        private readonly IDevMessageSerializer _serializer;

        public DevSocketFactory(IDevRequestHandler pHandler, IDevMessageSerializer pSerializer)
        {
            _handler = pHandler;
            _serializer = pSerializer;
        }

        public IDevSocket BuildSocket()
        {
            return new DevSocket(_handler, _serializer);
        }
    }
}
