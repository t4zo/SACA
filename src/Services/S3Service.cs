using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Options;

namespace SACA.Services
{
    public class S3Service : IS3Service
    {
        private readonly ILogger _logger;
        private readonly RegionEndpoint _bucketRegion;
        private readonly IAmazonS3 _s3Client;
        private readonly AWSOptions _awsOptions;
        private readonly bool _isProduction;

        public S3Service(ILogger<S3Service> logger, IOptionsSnapshot<AWSOptions> awsOptions)
        {
            _logger = logger;

            if (awsOptions.Value.S3.BucketRegion == "us-east-1")
            {
                _bucketRegion = RegionEndpoint.USEast1;
            };

            _awsOptions = awsOptions.Value;
            _s3Client = new AmazonS3Client(new BasicAWSCredentials(_awsOptions.AwsAccessKeyId, _awsOptions.AwsSecretAccessKey), _bucketRegion);
            _isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        }

        public async Task<string> UploadSharedFileAsync(string base64Url)
        {
            try
            {
                var normalizedKey = $"{(_isProduction ? string.Empty : "dev/")}shared/{Guid.NewGuid()}";

                var stream = GetBase64Stream(base64Url);

                // Multipart Upload
                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(stream, _awsOptions.S3.BucketName, normalizedKey);

                return $"https://{_awsOptions.S3.BucketName}.s3.amazonaws.com/{normalizedKey}";

            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }

            return string.Empty;
        }

        public async Task<string> UploadSharedFileAsync(string base64Url, string keyName)
        {
            try
            {
                var normalizedKey = $"{(_isProduction ? string.Empty : "dev/")}shared/{keyName.Replace(" ", "-").ToLower().RemoveAccent()}";

                var stream = GetBase64Stream(base64Url);

                var response = await _s3Client.PutObjectAsync(new PutObjectRequest
                {
                    InputStream = stream,
                    Key = normalizedKey,
                    BucketName = _awsOptions.S3.BucketName,
                    ContentType = "image/jpeg",
                });

                return $"https://{_awsOptions.S3.BucketName}.s3.amazonaws.com/{normalizedKey}";

            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }

            return string.Empty;
        }

        public async Task<string> UploadUserFileAsync(string base64Url, string keyName)
        {
            try
            {
                var normalizedKey = $"{(_isProduction ? string.Empty : "dev/")}users/{keyName}/{Guid.NewGuid()}";

                using var stream = GetBase64Stream(base64Url);

                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(stream, _awsOptions.S3.BucketName, normalizedKey);

                return $"https://{_awsOptions.S3.BucketName}.s3.amazonaws.com/{normalizedKey}";

            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }

            return string.Empty;

            //using var s3Client = new AmazonS3Client(_awsOptions.S3.AwsAccessKeyId, _awsOptions.S3.AwsSecretAccessKey, _bucketRegion);

            //var uploadRequest = new TransferUtilityUploadRequest
            //{
            //    InputStream = stream,
            //    Key = $"{keyName}/{Guid.NewGuid()}",
            //    BucketName = _awsOptions.S3.BucketName,
            //    CannedACL = S3CannedACL.PublicRead
            //};

            //var fileTransferUtility = new TransferUtility(s3Client);
            //await fileTransferUtility.UploadAsync(uploadRequest);
        }

        public async Task RemoveFileAsync(string keyName)
        {
            try
            {
                var normalizedKey = keyName.Split(".s3.amazonaws.com/")[1];
                await _s3Client.DeleteObjectAsync(_awsOptions.S3.BucketName, normalizedKey);
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
        }

        public async Task RemoveFolderAsync(string keyName)
        {
            try
            {
                var normalizedKey = $"users/{keyName}/";
                var folderFiles = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = _awsOptions.S3.BucketName,
                    Prefix = normalizedKey,
                });

                foreach (var file in folderFiles.S3Objects)
                {
                    await _s3Client.DeleteObjectAsync(file.BucketName, file.Key);
                }

            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
        }

        private static MemoryStream GetBase64Stream(string base64Url)
        {
            var stream = new MemoryStream();
            var bytes = Convert.FromBase64String(base64Url);
            stream.Write(bytes);
            return stream;
        }
    }
}
