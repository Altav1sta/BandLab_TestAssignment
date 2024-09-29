using Common.Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Common.Messaging
{
    public static class Extensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration config, bool withProducer = false)
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

            if (withProducer)
            {
                services.AddScoped<IMessageProducer, RabbitMQProducer>();
            }

            return services;
        }
    }
}
