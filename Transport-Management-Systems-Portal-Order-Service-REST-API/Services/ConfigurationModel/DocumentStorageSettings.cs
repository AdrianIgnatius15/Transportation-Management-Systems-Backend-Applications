namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Services.ConfigurationModel
{
    public class DocumentStorageSettings
    {
        public string ServiceUrl { get; set; } = string.Empty;

        public string AccessKey { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public string BucketName { get; set; } = string.Empty;

        public int RetentionDays { get; set; } = 90;
    }
}