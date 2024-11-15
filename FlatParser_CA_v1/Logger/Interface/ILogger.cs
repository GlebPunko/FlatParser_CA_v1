namespace FlatParser_CA_v1.Logger.Interface
{
    public interface ILogger
    {
        Task Log(Exception ex);
    }
}
