namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Client
{
    public record ClientUpdateDto
    {
        public string? Name { get; set; }

        public string? ContactEmail { get; set; }

        public string? ContactPhone { get; set; }
    }
}
