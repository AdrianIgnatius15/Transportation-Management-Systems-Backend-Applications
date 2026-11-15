using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Transportation_Management_Systems_Portal_Order_Service_REST_API.Middlewares.Interfaces;
using Transportation_Management_Systems_Portal_Order_Service_REST_API.Models.Config;

namespace Transportation_Management_Systems_Portal_Order_Service_REST_API.Middlewares
{
    public class MqProducer : IMQProducer
    {
        private readonly IRabbitMQConnection _mQConnection;
        private readonly IConfiguration _configuration;

        public MqProducer(
            IRabbitMQConnection mQConnection,
            IConfiguration configuration
        )
        {
            _mQConnection = mQConnection;
            _configuration = configuration;
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

        public async Task SendOrderCreatedEventMessage<OrderCreatedEvent>(OrderCreatedEvent orderCreatedEvent)
        {
            var connection = await _mQConnection.GetConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            var configMqList = _configuration.GetSection("RabbitMqTopology:Bindings").Get<List<RabbitMQConfig>>();

            var configMq = configMqList?.FirstOrDefault(config => config.Id == "order-shipment");
            await channel.QueueDeclareAsync(queue: configMq?.QueueName ?? "tms", durable: true, exclusive: false, autoDelete: false);

            var messageToSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderCreatedEvent));
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: configMq?.RoutingKeyPattern.Replace("#", "created") ?? "",
                mandatory: true,
                body: messageToSend
            );
        }
    }
}