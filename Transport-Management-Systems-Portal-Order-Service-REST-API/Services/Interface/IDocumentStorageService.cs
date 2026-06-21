using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Service.Interface
{
    public interface IDocumentStorageService
    {
        Task<string> UploadDocumentAsync(Stream filestream, string fileName, string contentType, string orderId);
        Task<Stream> DownloadDocumentAsync(string objectKey);
        Task<IEnumerable<DocumentMetadata>> ListOrderDocumentsAsync(string orderId);
        Task DeleteDocumentAsync(string objectKey);
        Task<string> GetPresignedUrlAsync(string objectKey, int expiryMinutes = 60);
    }
}