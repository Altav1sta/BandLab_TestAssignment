using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Posts.API.Messaging
{
    public abstract class RabbitMQConsumer<T>(IConnection connection, ILogger logger, string queue) : BackgroundService
    {
        private readonly IConnection _connection = connection;
        private readonly ILogger _logger = logger;
        private readonly string _queue = queue;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = _connection.CreateModel();

            channel.QueueDeclare(_queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventModel = JsonSerializer.Deserialize<T>(message);

                _logger.LogInformation("Received message: {message}", message);

                await ProcessEventAsync(eventModel!);

                channel.BasicAck(args.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(_queue, autoAck: false, consumer);

            return Task.CompletedTask;
        }

        protected abstract Task ProcessEventAsync(T eventModel);
    }
}
