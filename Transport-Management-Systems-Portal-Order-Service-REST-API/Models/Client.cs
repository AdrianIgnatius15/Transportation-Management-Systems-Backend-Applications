using System.ComponentModel.DataAnnotations;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models
{
    public class Client
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ContactEmail { get; set; } = default!;

        [Required]
        public string ContactPhone { get; set; } = default!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}