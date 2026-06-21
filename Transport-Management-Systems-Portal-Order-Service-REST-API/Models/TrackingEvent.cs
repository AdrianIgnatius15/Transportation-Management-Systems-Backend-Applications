using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models
{
    public class TrackingEvent
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("OrderId")]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        public Order Order { get; set; } = default!;

        public TrackingEventType EventType { get; set; }

        public string? Description { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}