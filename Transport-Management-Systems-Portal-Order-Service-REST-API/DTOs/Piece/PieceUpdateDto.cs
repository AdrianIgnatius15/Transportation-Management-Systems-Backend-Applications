using System.ComponentModel.DataAnnotations;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Piece
{
    public record PieceUpdateDto
    {
        [DataType(DataType.Text)]
        public string? Description { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Height { get; set; }

        public decimal? Width { get; set; }

        public decimal? Length { get; set; }
    }
}