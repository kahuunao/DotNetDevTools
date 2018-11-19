namespace DevToolsMessage
{
    public interface IDevResponse : IDevMessage
    {
        bool IsHandled { get; set; }
    }
}
