namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address
{
    public record AddressUpdateDto
    {
        public string? Line1 { get; set; }

        public string? Line2 { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
