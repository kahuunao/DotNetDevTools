using System;

namespace DevToolsMessage
{
    public class DevRequest
    {
        public Guid Id { get; set; }
        public EnumDevRequestType RequestType { get; set; }
        public DevResponse Response { get; set; }
    }
}
