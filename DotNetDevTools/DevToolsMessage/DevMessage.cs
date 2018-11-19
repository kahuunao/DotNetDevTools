using System;

namespace DevToolsMessage
{
    public abstract class DevMessage : IDevMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
