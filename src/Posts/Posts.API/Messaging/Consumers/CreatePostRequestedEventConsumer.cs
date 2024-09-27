using Posts.API.Messaging.Events;
using RabbitMQ.Client;

namespace Posts.API.Messaging.Consumers
{
    public class CreatePostRequestedEventConsumer(IConnection connection, ILogger<CreatePostRequestedEventConsumer> logger) 
        : RabbitMQConsumer<CreatePostRequestedEvent>(connection, logger, Consts.Queues.CreatePostRequestedQueue)
    {
        protected override Task ProcessEventAsync(CreatePostRequestedEvent eventModel)
        {
            return Task.Delay(500);
        }
    }
}
