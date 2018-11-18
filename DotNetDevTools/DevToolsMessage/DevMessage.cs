using System;

namespace DevToolsMessage
{
    public class DevMessage
    {
        public Guid Id { get; set; }
        public EnumDevMessageType RequestType { get; set; }

        public DevRequest Request { get; set; }
        public DevResponse Response { get; set; }
    }
}
