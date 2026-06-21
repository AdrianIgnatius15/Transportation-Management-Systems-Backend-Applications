using System.ComponentModel.DataAnnotations;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Address
{
    public record AddressCreateDto
    {
        [Required(ErrorMessage = "Line 1/Street Address is required")]
        public string Line1 { get; set; } = default!;

        public string? Line2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = default!;

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; } = default!;

        [Required(ErrorMessage = "Postal Code is required")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = default!;

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = default!;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}
