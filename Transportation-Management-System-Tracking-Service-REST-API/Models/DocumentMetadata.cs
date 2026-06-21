namespace Transportation_Management_System_Tracking_Service_REST_API.Models
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