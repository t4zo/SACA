﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ImageMagick;
using Microsoft.Extensions.Configuration;
using SACA.Models;
using SACA.Models.Dto;
using SACA.Services.Interfaces;
using SACA.Utilities;
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

            _cloudinaryEnvironmentFolder = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Constants.Development ? Constants.SACA_Development : Constants.SACA;
        }

        async Task<(string FullyQualifiedPublicId, string PublicId)> IImageService.UploadToCloudinaryAsync(ImageDto model, int? userId)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription($@"data:image/png;base64,{model.Base64}"),
                PublicId = userId.HasValue ? $"{_cloudinaryEnvironmentFolder}/{Constants.users}/{userId}/{Guid.NewGuid()}" : $"{_cloudinaryEnvironmentFolder}/_defaults/{Guid.NewGuid()}",
                Async = "true",
                Overwrite = true
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return (result.FullyQualifiedPublicId, result.PublicId);
        }

        async Task<bool> IImageService.RemoveImageFromCloudinaryAsync(Image model, User user)
        {
            var result = await _cloudinary.DeleteResourcesAsync(model.Url);

            var status = result.Deleted.First().Value;

            return status != "not_found";
        }

        async Task IImageService.RemoveFolderFromCloudinaryAsync(int userId)
        {
            var userFolder = $"{_cloudinaryEnvironmentFolder}/{Constants.users}/{userId}";

            await _cloudinary.DeleteResourcesByPrefixAsync(userFolder);
            await _cloudinary.DeleteFolderAsync(userFolder);
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
