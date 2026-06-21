namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Models
{
    public record DocumentMetadata
    (
        string ObjectKey,
        string FileName,
        long SizeBytes,
        DateTime LastModified,
        DateTime ExpiresAt
    );
}