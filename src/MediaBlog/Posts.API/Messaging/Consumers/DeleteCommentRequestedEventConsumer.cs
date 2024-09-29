using Posts.API.Messaging.Events;
using Posts.API.Services.Interfaces;
using RabbitMQ.Client;

namespace Posts.API.Messaging.Consumers
{
    public class DeleteCommentRequestedEventConsumer(IServiceProvider services, IConnection connection, ILogger<DeleteCommentRequestedEventConsumer> logger)
        : RabbitMQConsumer<DeleteCommentRequestedEvent>(connection, logger, Consts.Queues.DeleteCommentRequestedQueue)
    {
        protected override async Task ProcessEventAsync(DeleteCommentRequestedEvent model)
        {
            using var scope = services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICommentsService>();
            await service.DeleteCommentAsync(model);
        }
    }
}
