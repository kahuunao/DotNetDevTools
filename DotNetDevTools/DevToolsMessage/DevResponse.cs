namespace DevToolsMessage
{
    public abstract class DevResponse : DevMessage, IDevResponse
    {
        public bool IsHandled { get; set; }
    }
}
