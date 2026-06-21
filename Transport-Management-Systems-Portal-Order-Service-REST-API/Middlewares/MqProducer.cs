using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares.Interfaces;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares
{
    public class MqProducer : IMQProducer
    {
        private readonly IRabbitMQConnection _mQConnection;

        public MqProducer(IRabbitMQConnection mQConnection)
        {
            _mQConnection = mQConnection;
        }

        public async Task SendMessage<T>(T message)
        {
            var connection = await _mQConnection.GetConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "orders", durable: true, exclusive: false, autoDelete: false);

            var messageToSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "orders",
                mandatory: true,
                body: messageToSend
            );
        }
    }
}