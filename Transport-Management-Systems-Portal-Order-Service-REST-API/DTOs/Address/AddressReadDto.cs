namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address
{
    public record AddressReadDto
    {
        public Guid Id { get; set; }

        public string Line1 { get; set; } = default!;

        public string? Line2 { get; set; }

        public string City { get; set; } = default!;

        public string State { get; set; } = default!;

        public string PostalCode { get; set; } = default!;

        public string Country { get; set; } = default!;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
