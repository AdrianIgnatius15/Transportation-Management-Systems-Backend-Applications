using Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Enums;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.DTOs.Document
{
    public record DocumentUpdateDto
    {
        public DocumentType? Type { get; set; }

        public string? FileName { get; set; }

        public string? StorageUrl { get; set; }
    }
}
