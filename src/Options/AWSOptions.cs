namespace SACA.Options
{
    public class AWSOptions
    {
        public string AwsAccessKeyId { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public S3Options S3 { get; set; }
    }
}
