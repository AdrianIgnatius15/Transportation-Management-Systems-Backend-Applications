using System.ComponentModel.DataAnnotations;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Client
{
    public record ClientCreateDto
    {
        [Required(ErrorMessage = "Name of the client is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Contact Email is invalid")]
        public string ContactEmail { get; set; } = default!;

        [Required(ErrorMessage = "Contact Phone is required")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Contact Phone is invalid")]
        public string ContactPhone { get; set; } = default!;
    }
}
