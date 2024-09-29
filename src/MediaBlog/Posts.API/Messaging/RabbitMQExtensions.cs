using Microsoft.Extensions.Options;
using Posts.API.Messaging.Consumers;
using Posts.API.Messaging.Interfaces;
using RabbitMQ.Client;

namespace Posts.API.Messaging
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMQSettings>(config.GetRequiredSection("RabbitMQ"));

            services.AddSingleton(x =>
            {
                var settings = x.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
                var connectionFactory = new ConnectionFactory
                {
                    HostName = settings.Hostname,
                    UserName = settings.Username,
                    Password = settings.Password
                };
                return connectionFactory.CreateConnection();
            });

            services.AddScoped<IMessageProducer, RabbitMQProducer>();

            services.AddHostedService<CreateCommentRequestedEventConsumer>();
            services.AddHostedService<CreatePostRequestedEventConsumer>();
            services.AddHostedService<DeleteCommentRequestedEventConsumer>();

            return services;
        }
    }
}
