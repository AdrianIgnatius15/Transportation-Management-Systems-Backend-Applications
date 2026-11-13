namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models.Messages
{
    public class DocumentDeletedEvent
    {
        public Guid OrderId { get; set; }
        public string FileKey { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public int FileSize { get; set; }
    }
}