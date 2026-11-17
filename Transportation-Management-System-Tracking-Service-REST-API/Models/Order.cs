using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Transportation_Management_System_Tracking_Service_REST_API.Models.Enums;

namespace Transportation_Management_System_Tracking_Service_REST_API.Models
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
        [ForeignKey("OriginAdressId")]
        public Guid OriginAdressId { get; set; } = Guid.NewGuid();

        public Address OriginAdress { get; set; } = default!;

        [Required]
        [ForeignKey("DestinationAddressId")]
        public Guid DestinationAddressId { get; set; } = Guid.NewGuid();

        public Address DestinationAddress { get; set; } = default!;

        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }
}