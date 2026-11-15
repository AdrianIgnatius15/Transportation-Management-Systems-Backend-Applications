namespace Transportation_Management_Systems_Portal_Order_Service_REST_API.Models.Messages
{
    public record OrderCreatedEvent
    {
        public Guid ClientId { get; set; } = Guid.Empty;

        public string OrderNumber { get; set; } = default!;

        public Guid Id { get; set; } = Guid.Empty;
    }
}