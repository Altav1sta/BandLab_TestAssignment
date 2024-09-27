using Posts.API.Messaging.Events;
using RabbitMQ.Client;

namespace Posts.API.Messaging.Consumers
{
    public class DeleteCommentRequestedEventConsumer(IConnection connection, ILogger<DeleteCommentRequestedEventConsumer> logger)
        : RabbitMQConsumer<DeleteCommentRequestedEvent>(connection, logger, Consts.Queues.DeleteCommentRequestedQueue)
    {
        protected override Task ProcessEventAsync(DeleteCommentRequestedEvent eventModel)
        {
            return Task.Delay(500);
        }
    }
}
