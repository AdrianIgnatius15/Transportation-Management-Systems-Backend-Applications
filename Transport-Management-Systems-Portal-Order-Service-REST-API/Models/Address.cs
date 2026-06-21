using System.ComponentModel.DataAnnotations;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models
{
    public class Address
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Line1 { get; set; } = default!;

        public string? Line2 { get; set; }

        [Required]
        public string City { get; set; } = default!;

        [Required]
        public string State { get; set; } = default!;

        [Required]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = default!;

        [Required]
        public string Country { get; set; } = default!;

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}