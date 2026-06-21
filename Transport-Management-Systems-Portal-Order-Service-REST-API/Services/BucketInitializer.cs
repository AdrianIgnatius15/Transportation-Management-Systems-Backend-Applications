using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Services.ConfigurationModel;

namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Services
{
    public class BucketInitializer : IHostedService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IOptions<DocumentStorageSettings> _settings;

        public BucketInitializer(IAmazonS3 amazonS3, IOptions<DocumentStorageSettings> settings)
        {
            _amazonS3 = amazonS3;
            _settings = settings;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string bucket = _settings.Value.BucketName;
            bool bucketExistFlag = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucket);

            if (!bucketExistFlag)
            {
                await _amazonS3.PutBucketAsync(new PutBucketRequest { BucketName = bucket }, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
         => Task.CompletedTask;
    }
}