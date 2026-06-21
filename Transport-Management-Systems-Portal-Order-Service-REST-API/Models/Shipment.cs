using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models
{
    public class Shipment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("OrderId")]
        public Guid OrderId { get; set; } = Guid.Empty;

        public Order Order { get; set; } = default!;

        public ICollection<Piece> Pieces { get; set; } = new List<Piece>();
    }
}