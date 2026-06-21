using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models
{
    public class Piece
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [DataType(DataType.Text)]
        public string Description { get; set; } = string.Empty;
        
        public decimal Weight { get; set; }

        public decimal Height { get; set; }

        public decimal Width { get; set; }

        public decimal Length { get; set; }

        [ForeignKey("ShipmentId")]
        public Guid ShipmentId { get; set; } = Guid.Empty;

        public Shipment Shipment { get; set; } = default!;
    }
}