using Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Piece;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Shipment
{
    public record ShipmentCreateDto
    {
        public List<PieceCreateDto> Pieces { get; set; } = new List<PieceCreateDto>();
    }
}