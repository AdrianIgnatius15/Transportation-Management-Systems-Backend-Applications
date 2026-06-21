using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Piece;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Shipment
{
    public record ShipmentReadDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public List<PieceReadDto> Pieces { get; set; } = new List<PieceReadDto>();
    }
}