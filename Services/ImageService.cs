using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ImageMagick;
using Microsoft.Extensions.Configuration;
using SACA.Models;
using SACA.Models.Dto;
using SACA.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _cloudinaryEnvironmentFolder;

        public ImageService(IConfiguration configuration)
        {
            _cloudinary = new Cloudinary(
                new Account
                {
                    Cloud = configuration["CloudinaryConfiguration:Cloud"],
                    ApiKey = configuration["CloudinaryConfiguration:ApiKey"],
                    ApiSecret = configuration["CloudinaryConfiguration:ApiSecret"]
                }
            );

            _cloudinaryEnvironmentFolder = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? "SACA_Development" : "SACA";
        }

        async Task<string> IImageService.UploadToCloudinaryAsync(ImageDto model, int? userId)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription($@"data:image/png;base64,{model.Base64}"),
                PublicId = userId.HasValue ? $"{_cloudinaryEnvironmentFolder}/users/{userId}/{model.Name}" : $"{_cloudinaryEnvironmentFolder}/_defaults/{model.Name}",
                Async = "true",
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.FullyQualifiedPublicId;
        }

        async Task<bool> IImageService.RemoveImageFromCloudinaryAsync(Image model, User user)
        {
            string url;

            if (user == null)
            {
                url = $"{_cloudinaryEnvironmentFolder}/_defaults/{model.Name}";
            }
            else
            {
                url = $"{_cloudinaryEnvironmentFolder}/users/{user.Id}/{model.Name}";
            }

            var result = await _cloudinary.DeleteResourcesAsync(url);
            var status = result.Deleted.ToArray().First().Value;
            return status != "not_found";
        }

        async Task IImageService.RemoveFolderFromCloudinaryAsync(int userId)
        {
            await _cloudinary.DeleteResourcesByPrefixAsync($"{_cloudinaryEnvironmentFolder}/users/{userId}");
            await _cloudinary.DeleteFolderAsync($"{_cloudinaryEnvironmentFolder}/users/{userId}");
        }

        MagickImage IImageService.Resize(MagickImage image, int width, int height)
        {
            MagickGeometry size = new MagickGeometry(width, height);

            size.IgnoreAspectRatio = true;

            image.Resize(size);

            return image;
        }
    }
}
