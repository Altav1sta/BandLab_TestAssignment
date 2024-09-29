namespace Common.Messaging.Interfaces
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message, string queue);
    }
}
