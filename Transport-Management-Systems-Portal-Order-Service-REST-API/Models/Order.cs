using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models
{
    public class Order
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Client")]
        public Guid ClientId { get; set; } = Guid.NewGuid();

        public Client Client { get; set; } = default!;

        [Required]
        public string OrderNumber { get; set; } = default!;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Created;

        [Required]
        public string Priority { get; set; } = string.Empty;

        [Required]
        [ForeignKey("ShipmentAddressId")]
        public Guid ShipmentAddressId { get; set; } = Guid.NewGuid();

        public Address ShipmentAddress { get; set; } = default!;

        [Required]
        [ForeignKey("DeliveryAddressId")]
        public Guid DeliveryAddressId { get; set; } = Guid.NewGuid();

        public Address DeliveryAddress { get; set; } = default!;

        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }
}