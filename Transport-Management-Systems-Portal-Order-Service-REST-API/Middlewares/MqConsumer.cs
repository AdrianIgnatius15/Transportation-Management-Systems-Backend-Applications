using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Messages;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares
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

                for (int i = 0; i < bindingsArrayTopology.Count; i++)
                {
                    string exchangeName = _configuration[$"RabbitMqTopology:Bindings:{i}:ExchangeName"] ?? "";
                    string queueName = _configuration[$"RabbitMqTopology:Bindings:{i}:QueueName"] ?? "";
                    string routingKeyPattern = _configuration[$"RabbitMqTopology:Bindings:{i}:RoutingKeyPattern"] ?? "";

                    if (string.IsNullOrEmpty(exchangeName) || string.IsNullOrEmpty(queueName))
                    {
                        _logger.LogWarning($"[AMQP CONFIG SKIP] Missing properties on index payload mapping sequence block: {i}");
                        continue;
                    }

                    _logger.LogInformation($"[AMQP INITIALIZED] [Index {i}] -> Exchange: {exchangeName}, Queue: {queueName}, Routing Key: {routingKeyPattern}");

                    // Declare topology infrastructure directly over the message broker channels
                    await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
                    await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
                    await _channel.QueueBindAsync(queueName, exchangeName, routingKeyPattern, cancellationToken: stoppingToken);

                    var consumer = new AsyncEventingBasicConsumer(_channel);

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
                                _logger.LogInformation($"[AMQP EX-ROUTE] [{exchangeName}] Received context identifier: {eventType}");

                                // Forward deserialization targeting into our shared polymorphic handler switcher matrix
                                await RouteEventToSpecificHandlerAsync(eventType, rawJsonMessage);
                            }

                            // Explicitly acknowledge processing completion back onto the broker pipeline
                            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"[AMQP PROCESSING FAIL] Failure handling payload inside exchange context: {exchangeName}");

                            // Negative acknowledgement: Rolls back message onto the Docker queue safely
                            await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                        }
                    };

                    string tag = await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
                    _activeConsumerTags.Add(tag);
                }

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