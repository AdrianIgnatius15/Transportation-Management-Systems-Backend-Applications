using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Transportation_Management_System_Tracking_Service_REST_API.Middlewares.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.Models.Config;

namespace Transportation_Management_System_Tracking_Service_REST_API.Middlewares
{
    public class MqProducer : IMQProducer
    {
        private readonly IRabbitMQConnection _mQConnection;
        private readonly IConfiguration _configuration;

        public MqProducer(IRabbitMQConnection mQConnection, IConfiguration configuration)
        {
            _mQConnection = mQConnection;
            _configuration = configuration;
        }

        public async Task SendMessage<T>(T message)
        {
            var connection = await _mQConnection.GetConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var rabbitMQTopology = _configuration.GetSection("RabbitMqTopology:Bindings").Get<List<QueueConfiguration>>() ?? [];
            var trackingQueueConfiguration = rabbitMQTopology.FirstOrDefault(queueConfig => queueConfig.Id == "shipment-tracking");

            await channel.QueueDeclareAsync(queue: "tms-queue", durable: true, exclusive: true, autoDelete: false);

            var messageToSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            try
            {
                await channel.BasicPublishAsync(
                    exchange: "order.event",
                    routingKey: "order.event.something",
                    mandatory: true,
                    body: messageToSend
                );

                Console.WriteLine("Message successfully send to queue");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message failed to send to queue with the exchange due to {ex.Message}");
            }
        }

        public async Task SendTrackingOrderShipmentUpdates<TrackingEventPublishedEvent>(TrackingEventPublishedEvent trackingEvent)
        {
            var connection = await _mQConnection.GetConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var rabbitMQTopology = _configuration.GetSection("RabbitMqTopology:Bindings").Get<List<QueueConfiguration>>() ?? [];
            var trackingQueueConfiguration = rabbitMQTopology.FirstOrDefault(queueConfig => queueConfig.Id == "shipment-tracking");

            await channel.QueueDeclareAsync(queue: trackingQueueConfiguration?.QueueName ?? "tms-queue", durable: true, exclusive: true, autoDelete: false);

            var messageToSend = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(trackingEvent));
            try
            {
                await channel.BasicPublishAsync(
                    exchange: trackingQueueConfiguration?.ExchangeName ?? "shipment.tracking.exchange",
                    routingKey: trackingQueueConfiguration?.RoutingKeyPattern ?? "tms.tracking.updates",
                    mandatory: true,
                    body: messageToSend
                );

                Console.WriteLine("Message successfully send to queue");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message failed to send to queue with the exchange due to {ex.Message}");
            }
        }
    }
}