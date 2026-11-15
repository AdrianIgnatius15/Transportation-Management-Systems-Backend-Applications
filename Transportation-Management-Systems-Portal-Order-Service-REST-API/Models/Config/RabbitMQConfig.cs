namespace Transportation_Management_Systems_Portal_Order_Service_REST_API.Models.Config
{
    public class RabbitMQConfig
    {
        public string Id { get; set; } = string.Empty;

        public string ExchangeName { get; set; } = string.Empty;

        public string QueueName { get; set; } = string.Empty;

        public string RoutingKeyPattern { get; set; } = string.Empty;
    }
}