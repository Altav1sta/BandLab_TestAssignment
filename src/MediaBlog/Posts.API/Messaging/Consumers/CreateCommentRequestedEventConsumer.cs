using Common.Messaging;
using Common.Messaging.Events;
using Posts.API.Services.Interfaces;
using RabbitMQ.Client;

namespace Posts.API.Messaging.Consumers
{
    public class CreateCommentRequestedEventConsumer(IServiceProvider services, IConnection connection, ILogger<CreateCommentRequestedEventConsumer> logger)
        : RabbitMQConsumer<CreateCommentRequestedEvent>(connection, logger, MessagingConsts.Queues.CreateCommentRequestedQueue)
    {
        protected override async Task ProcessEventAsync(CreateCommentRequestedEvent model)
        {
            using var scope = services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICommentsService>();
            await service.CreateCommentAsync(model);
        }
    }
}
