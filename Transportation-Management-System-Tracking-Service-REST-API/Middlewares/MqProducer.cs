using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Transportation_Management_System_Tracking_Service_REST_API.Middlewares.Interfaces;

namespace Transportation_Management_System_Tracking_Service_REST_API.Middlewares
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
            await channel.QueueDeclareAsync(
                queue: "tracking.events", 
                durable: true, 
                exclusive: false, 
                autoDelete: false
            );

            var messageToSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "tracking.events",
                mandatory: true,
                body: messageToSend
            );
        }
    }
}