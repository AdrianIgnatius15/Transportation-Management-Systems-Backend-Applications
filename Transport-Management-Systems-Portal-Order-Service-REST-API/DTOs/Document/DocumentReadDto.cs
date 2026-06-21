using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Document
{
    public record DocumentReadDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public DocumentType Type { get; set; }

        public string FileName { get; set; } = default!;

        public string StorageUrl { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
    }
}
