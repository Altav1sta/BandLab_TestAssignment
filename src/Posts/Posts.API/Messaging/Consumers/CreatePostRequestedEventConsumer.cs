using Posts.API.Messaging.Events;
using Posts.API.Services.Interfaces;
using RabbitMQ.Client;

namespace Posts.API.Messaging.Consumers
{
    public class CreatePostRequestedEventConsumer(IServiceProvider services, IConnection connection, ILogger<CreatePostRequestedEventConsumer> logger) 
        : RabbitMQConsumer<CreatePostRequestedEvent>(connection, logger, Consts.Queues.CreatePostRequestedQueue)
    {
        protected override async Task ProcessEventAsync(CreatePostRequestedEvent model)
        {
            using var scope = services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IPostsService>();
            await service.CreatePostAsync(model);
        }
    }
}
