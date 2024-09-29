namespace Posts.API.Messaging.Interfaces
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message, string queue);
    }
}
