using Posts.API.Messaging.Events;
using RabbitMQ.Client;

namespace Posts.API.Messaging.Consumers
{
    public class CreateCommentRequestedEventConsumer(IConnection connection, ILogger<CreateCommentRequestedEventConsumer> logger)
        : RabbitMQConsumer<CreateCommentRequestedEvent>(connection, logger, Consts.Queues.CreateCommentRequestedQueue)
    {
        protected override Task ProcessEventAsync(CreateCommentRequestedEvent eventModel)
        {
            return Task.Delay(500);
        }
    }
}
