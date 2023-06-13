using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Options;
using System.Net.Mime;

namespace SACA.Services
{
    public class S3Service : IS3Service
    {
        private readonly ILogger _logger;
        private readonly RegionEndpoint _bucketRegion;
        private readonly IAmazonS3 _s3;
        private readonly AWSOptions _awsOptions;
        private readonly bool _isProduction;

        public S3Service(ILogger<S3Service> logger, IOptionsSnapshot<AWSOptions> awsOptions)
        {
            _logger = logger;

            if (awsOptions.Value.S3.BucketRegion == "us-east-1")
            {
                _bucketRegion = RegionEndpoint.USEast1;
            }

            _awsOptions = awsOptions.Value;
            _s3 = new AmazonS3Client(new BasicAWSCredentials(_awsOptions.AwsAccessKeyId, _awsOptions.AwsSecretAccessKey), _bucketRegion);
            _isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        }

        public async Task<string> UploadCommonFileAsync(IFormFile file)
        {
            try
            {
                var (name, extension, _) = file.FileName.Split(".");
                var key = NormalizedSharedKey(extension);

                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = _awsOptions.S3.BucketName,
                    Key = key,
                    ContentType = file.ContentType,
                    InputStream = file.OpenReadStream(),
                    Metadata =
                    {
                        ["x-amz-meta-originalname"] = name,
                        ["x-amz-meta-extension"] = extension,
                    }
                };

                _ = await _s3.PutObjectAsync(putObjectRequest);

                return MountUrl(key);
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

        public async Task<string> UploadUserFileAsync(IFormFile file, string keyName)
        {
            try
            {
                var (name, extension, _) = file.FileName.Split(".");
                var key = NormalizeKey(keyName, extension);

                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = _awsOptions.S3.BucketName,
                    Key = key,
                    ContentType = file.ContentType,
                    InputStream = file.OpenReadStream(),
                    Metadata =
                    {
                        ["x-amz-meta-originalname"] = name,
                        ["x-amz-meta-extension"] = extension,
                    }
                };

                _ = await _s3.PutObjectAsync(putObjectRequest);

                return MountUrl(key);
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

        public async Task RemoveFileAsync(string keyName)
        {
            try
            {
                var key = keyName.Split(".s3.amazonaws.com/").LastOrDefault();
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _awsOptions.S3.BucketName,
                    Key = key,
                };

                _ = await _s3.DeleteObjectAsync(deleteObjectRequest);
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
                var key = $"users/{keyName}/";
                var listObjectsV2Request = new ListObjectsV2Request
                {
                    BucketName = _awsOptions.S3.BucketName,
                    Prefix = key,
                };

                var folderFiles = await _s3.ListObjectsV2Async(listObjectsV2Request);

                foreach (var file in folderFiles.S3Objects)
                {
                    await _s3.DeleteObjectAsync(file.BucketName, file.Key);
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

        private string NormalizeKey(string keyName, string extension)
        {
            var normalizedKey = $"{(_isProduction ? string.Empty : "dev/")}users/{keyName}/{Guid.NewGuid()}.{extension}";
            return normalizedKey;
        }

        private string NormalizedSharedKey(string extension)
        {
            var normalizedSharedKey = $"{(_isProduction ? string.Empty : "dev/")}shared/{Guid.NewGuid()}.{extension}";
            return normalizedSharedKey;
        }

        private string MountUrl(string key)
        {
            return $"https://{_awsOptions.S3.BucketName}.s3.amazonaws.com/{key}";
        }


        public async Task<string> UploadCommonSeedFileAsync(string base64Url, string keyName, string extension)
        {
            try
            {
                var key = $"{(_isProduction ? string.Empty : "dev/")}shared/{keyName.Replace(" ", "-").ToLower().RemoveAccent()}.{extension}";
                
                using var stream = new MemoryStream();
                var bytes = Convert.FromBase64String(base64Url);
                stream.Write(bytes);
                
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = _awsOptions.S3.BucketName,
                    Key = key,
                    ContentType = MediaTypeNames.Image.Jpeg,
                    InputStream = stream,
                    Metadata =
                    {
                        ["x-amz-meta-originalname"] = keyName.RemoveAccent(),
                        ["x-amz-meta-extension"] = MediaTypeNames.Image.Jpeg.Split("/").LastOrDefault(),
                    }
                };

                _ = await _s3.PutObjectAsync(putObjectRequest);

                return MountUrl(key);
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
    }
}