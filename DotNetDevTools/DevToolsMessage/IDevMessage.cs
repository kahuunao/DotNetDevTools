using System;

namespace DevToolsMessage
{
    public interface IDevMessage
    {
        Guid Id { get; set; }
        string Type { get; set; }
    }
}
