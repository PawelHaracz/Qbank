namespace Qbank.Core.Orchestrations.Impl
{
    public interface ICallSerializer
    {
        T Deserialize<T>(byte[] payload);
        byte[] Serialize<T>(T result);
    }
}