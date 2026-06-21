using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Models;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Service.Interface;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Services.ConfigurationModel;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Services
{
    public class DocumentStorageService : IDocumentStorageService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IOptions<DocumentStorageSettings> _settings;
        private readonly ILogger _logger;

        public DocumentStorageService(IAmazonS3 amazonS3, IOptions<DocumentStorageSettings> settings, ILogger<DocumentStorageService> logger)
        {
            _amazonS3 = amazonS3;
            _settings = settings;
            _logger = logger;
        }

        public async Task DeleteDocumentAsync(string objectKey)
        {
            await _amazonS3.DeleteObjectAsync(_settings.Value.BucketName, objectKey);
            _logger.LogInformation("Deleted document {Key}", objectKey);
        }

        public async Task<Stream> DownloadDocumentAsync(string objectKey)
        {
            var response = await _amazonS3.GetObjectAsync(_settings.Value.BucketName, objectKey);
            return response.ResponseStream;
        }

        public async Task<string> GetPresignedUrlAsync(string objectKey, int expiryMinutes = 60)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _settings.Value.BucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Verb = HttpVerb.GET
            };

            return _amazonS3.GetPreSignedURL(request);
        }

        public async Task<IEnumerable<DocumentMetadata>> ListOrderDocumentsAsync(string orderId)
        {
            var prefix   = $"orders/{orderId}/documents/";
            var response = await _amazonS3.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = _settings.Value.BucketName,
                Prefix = prefix
            });

            return response.S3Objects.Select(obj =>
            {
                var fileName = obj.Key.Split('/').Last();
                return new DocumentMetadata(
                    ObjectKey:    obj.Key,
                    FileName:     fileName,
                    SizeBytes:    obj.Size.GetValueOrDefault(),
                    LastModified: obj.LastModified.GetValueOrDefault(),
                    ExpiresAt:    obj.LastModified.GetValueOrDefault().AddDays(_settings.Value.RetentionDays)
                );
            });
        }

        public async Task<string> UploadDocumentAsync(Stream filestream, string fileName, string contentType, string orderId)
        {
            string objectKey = $"orders/{orderId}/documents/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{fileName}";
            DateTime expiresAt = DateTime.UtcNow.AddDays(_settings.Value.RetentionDays);

            var request = new PutObjectRequest
            {
                BucketName  = _settings.Value.BucketName,
                Key         = objectKey,
                InputStream = filestream,
                ContentType = contentType,
                // Tag with expiry for lifecycle policy
                TagSet =
                [
                    new() { Key = "OrderId",   Value = orderId },
                    new() { Key = "ExpiresAt", Value = expiresAt.ToString("O") },
                    new() { Key = "RetentionDays", Value = _settings.Value.RetentionDays.ToString() }
                ]
            };

            await _amazonS3.PutObjectAsync(request);
            _logger.LogInformation("Uploaded {FileName} for order {OrderId} → {Key}", fileName, orderId, objectKey);
            return objectKey;
        }
    }
}