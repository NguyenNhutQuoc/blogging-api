namespace BloggingSystem.Infrastructure.Settings
{
    public class S3Settings
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
        public bool UseAccelerateEndpoint { get; set; }
        public string Endpoint { get; set; }
        public bool UseHttp { get; set; }
    }
}