using System;

namespace DevToolsMessage
{
    public class DevMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public EnumDevMessageType RequestType { get; set; }
        public DevRequest Request { get; set; }
        public DevResponse Response { get; set; }

        public bool IsRequest()
        {
            return Request != null;
        }

        public bool IsResponse()
        {
            return Response != null;
        }
    }
}
