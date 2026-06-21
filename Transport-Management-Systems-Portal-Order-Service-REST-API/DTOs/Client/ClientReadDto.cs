namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Client
{
    public record ClientReadDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = default!;

        public string ContactPhone { get; set; } = default!;
    }
}
