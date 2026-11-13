using Amazon.Runtime.Documents;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Messages
{
    public record DocumentCreatedEvent
    {
        public string? OrderId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public long FileSize { get; set; }
    }
}