using Posts.API.Messaging.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Posts.API.Messaging
{
    public class RabbitMQProducer(IConnection rabbitConnection) : IMessageProducer
    {
        public void SendMessage<T>(T message, string queue)
        {
            using var channel = rabbitConnection.CreateModel();

            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: null, body: body);
        }
    }
}
