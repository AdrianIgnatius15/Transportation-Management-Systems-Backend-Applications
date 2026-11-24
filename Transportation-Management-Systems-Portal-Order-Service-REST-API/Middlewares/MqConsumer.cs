using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Transportation_Management_Systems_Portal_Order_Service_REST_API.Middlewares.Interfaces;
using Transportation_Management_Systems_Portal_Order_Service_REST_API.Models.Config;
using Transportation_Management_Systems_Portal_Order_Service_REST_API.Models.Messages;

namespace Transportation_Management_Systems_Portal_Order_Service_REST_API.Middlewares
{
    public class MqConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IRabbitMQConnection _rabbitMQConnection;
        private readonly ILogger<MqConsumer> _logger;
        private readonly List<string> _activeConsumerTags = new List<string>();
        private IChannel? _channel;

        public MqConsumer(
            IConfiguration configuration,
            ILogger<MqConsumer> logger,
            IRabbitMQConnection rabbitMQConnection
        )
        {
            _configuration = configuration;
            _logger = logger;
            _rabbitMQConnection = rabbitMQConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[AMQP] Booting up MQ Consumer Engine");

            try
            {
                var connection = await _rabbitMQConnection.GetConnectionAsync();

                _channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

                var bindingsSection = _configuration.GetSection("RabbitMqTopology:Bindings");
                var bindingsArrayTopology = bindingsSection.GetChildren().ToList();

                if (!bindingsArrayTopology.Any())
                {
                    _logger.LogWarning("[AMQP] No message queue topology environment variables to resolve");
                    return;
                }

                // await InitializeMqConsumerListenerForShipmentDocuments(_channel, stoppingToken);
                await InitializeMqConsumerListenerForOrderEvents(_channel, stoppingToken);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("[AMQP] Background message thread processing loop cancelled via system token command.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "[AMQP FATAL] Crash event intercepted on the global background service channel engine.");
            }
        }

        private async Task InitializeMqConsumerListenerForShipmentDocuments(IChannel channel, CancellationToken stoppingToken)
        {
            var mqConfigList = _configuration.GetSection("RabbitMqTopology:Bindings").Get<List<RabbitMQConfig>>();
            var trackingMqConfig = mqConfigList?.FirstOrDefault(config => config.Id == "shipment-document");

            await channel.ExchangeDeclareAsync(trackingMqConfig?.ExchangeName ?? "shipment.documents.exchange", ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
            await channel.QueueDeclareAsync(queue: trackingMqConfig?.QueueName ?? "tms-document-service", durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await channel.QueueBindAsync(queue: trackingMqConfig?.QueueName ?? "tms-document-service", exchange: trackingMqConfig?.ExchangeName ?? "shipment.documents.exchange", routingKey: "tms.document.#", cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var rawJsonMessage = Encoding.UTF8.GetString(body);

                try
                {
                    using var jsonDoc = JsonDocument.Parse(rawJsonMessage);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("eventType", out var eventTypeProp))
                    {
                        string eventType = eventTypeProp.GetString() ?? "UNKNOWN";
                        _logger.LogInformation($"[AMQP EX-ROUTE] [{trackingMqConfig?.ExchangeName ?? ""}] Received context identifier: {eventType}");

                        // Forward deserialization targeting into our shared polymorphic handler switcher matrix
                        await RouteEventToSpecificHandlerAsync(eventType, rawJsonMessage);
                    }

                    // Explicitly acknowledge processing completion back onto the broker pipeline
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[AMQP PROCESSING FAIL] Failure handling payload inside exchange context: {trackingMqConfig?.ExchangeName ?? ""}");

                    // Negative acknowledgement: Rolls back message onto the Docker queue safely
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(queue: trackingMqConfig?.QueueName ?? "tms-document-service", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        }

        private async Task InitializeMqConsumerListenerForOrderEvents(IChannel channel, CancellationToken stoppingToken)
        {
            var mqConfigList = _configuration.GetSection("RabbitMqTopology:Bindings").Get<List<RabbitMQConfig>>();
            var orderMqConfig = mqConfigList?.FirstOrDefault(config => config.Id == "order-shipment");

            await channel.ExchangeDeclareAsync(orderMqConfig?.ExchangeName ?? "order.event.exchange", ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
            await channel.QueueDeclareAsync(queue: orderMqConfig?.QueueName ?? "tms-order-service", durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await channel.QueueBindAsync(queue: orderMqConfig?.QueueName ?? "tms-order-service", orderMqConfig?.ExchangeName ?? "order.event.exchange", orderMqConfig?.RoutingKeyPattern ?? "tms.order.#", cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var rawJsonMessage = Encoding.UTF8.GetString(body);

                try
                {
                    using var jsonDoc = JsonDocument.Parse(rawJsonMessage);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("eventType", out var eventTypeProp))
                    {
                        string eventType = eventTypeProp.GetString() ?? "UNKNOWN";
                        _logger.LogInformation($"[AMQP EX-ROUTE] [{orderMqConfig?.ExchangeName ?? ""}] Received context identifier: {eventType}");

                        // Forward deserialization targeting into our shared polymorphic handler switcher matrix
                        await RouteEventToSpecificHandlerAsync(eventType, rawJsonMessage);
                    }

                    // Explicitly acknowledge processing completion back onto the broker pipeline
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[AMQP PROCESSING FAIL] Failure handling payload inside exchange context: {orderMqConfig?.ExchangeName ?? ""}");

                    // Negative acknowledgement: Rolls back message onto the Docker queue safely
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(queue: orderMqConfig?.QueueName ?? "tms-order-service", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        }

        private async Task InitializeMqConsumerListenerForTrackingEvents(IChannel channel, CancellationToken stoppingToken)
        {
            var mqConfigList = _configuration.GetSection("RabbitMqTopology:Bindings").Get<List<RabbitMQConfig>>();
            var trackingMqConfig = mqConfigList?.FirstOrDefault(config => config.Id == "shipment-tracking");

            await channel.ExchangeDeclareAsync(trackingMqConfig?.ExchangeName ?? "", ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
            await channel.QueueDeclareAsync(queue: trackingMqConfig?.QueueName ?? "", durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await channel.QueueBindAsync(trackingMqConfig?.QueueName ?? "", trackingMqConfig?.ExchangeName ?? "", trackingMqConfig?.RoutingKeyPattern ?? "", cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var rawJsonMessage = Encoding.UTF8.GetString(body);

                try
                {
                    using var jsonDoc = JsonDocument.Parse(rawJsonMessage);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("eventType", out var eventTypeProp))
                    {
                        string eventType = eventTypeProp.GetString() ?? "UNKNOWN";
                        _logger.LogInformation($"[AMQP EX-ROUTE] [{trackingMqConfig?.ExchangeName ?? ""}] Received context identifier: {eventType}");

                        // Forward deserialization targeting into our shared polymorphic handler switcher matrix
                        await RouteEventToSpecificHandlerAsync(eventType, rawJsonMessage);
                    }

                    // Explicitly acknowledge processing completion back onto the broker pipeline
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[AMQP PROCESSING FAIL] Failure handling payload inside exchange context: {trackingMqConfig?.ExchangeName ?? ""}");

                    // Negative acknowledgement: Rolls back message onto the Docker queue safely
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(queue: "tms", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        }

        private async Task RouteEventToSpecificHandlerAsync(string eventType, string rawJson)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            switch (eventType)
            {
                case "DOCUMENT_DELETED":
                    var deleteData = JsonSerializer.Deserialize<DocumentDeletedEvent>(rawJson, options);
                    if (deleteData != null) await HandleDocumentDeletionAsync(deleteData);
                    break;

                case "DOCUMENT_CREATED":
                    var createData = JsonSerializer.Deserialize<DocumentCreatedEvent>(rawJson, options);
                    if (createData != null) await HandleDocumentCreationAsync(createData);
                    break;

                default:
                    _logger.LogWarning($"[AMQP ROUTER BYPASS] No message execution path mapped for type token: {eventType}");
                    break;
            }
        }

        private Task HandleDocumentDeletionAsync(DocumentDeletedEvent @event)
        {
            _logger.LogWarning($"[DELETION DETECTED] Processing background tasks for Order Reference: {@event.OrderId}");
            // SignalR hub execution context pushes go straight here!
            return Task.CompletedTask;
        }

        private Task HandleDocumentCreationAsync(DocumentCreatedEvent @event)
        {
            _logger.LogInformation($"[CREATION DETECTED] Processing task updates for file name: {@event.FileName}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("[AMQP CLOSING] Detaching and cancelling active message consumers gracefully...");

            if (_channel != null)
            {
                foreach (var tag in _activeConsumerTags)
                {
                    await _channel.BasicCancelAsync(tag, cancellationToken: cancellationToken);
                }
            }

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            base.Dispose();
        }
    }
}