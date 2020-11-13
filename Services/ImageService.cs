﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SACA.Interfaces;
using SACA.Models;
using SACA.Models.Requests;
using SACA.Models.Identity;
using SACA.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _cloudinaryEnvironmentFolder;

        public ImageService(IOptionsSnapshot<CloudinaryOptions> cloudinaryOptions, IWebHostEnvironment env)
        {
            _cloudinaryEnvironmentFolder = env.IsDevelopment() ? $"{nameof(SACA)}_{env.EnvironmentName}" : nameof(SACA);
            _cloudinary = new Cloudinary(cloudinaryOptions.Value.ApiEnvironmentVariable);
        }

        public async Task<(string FullyQualifiedPublicId, string PublicId)> UploadToCloudinaryAsync(ImageRequest model, int? userId)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription($@"data:image/png;base64,{model.Base64}"),
                PublicId = userId.HasValue ? $"{_cloudinaryEnvironmentFolder}/users/{userId}/{Guid.NewGuid()}" : $"{_cloudinaryEnvironmentFolder}/_defaults/{Guid.NewGuid()}",
                Async = true.ToString(),
                Overwrite = true
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return (result.FullyQualifiedPublicId, result.PublicId);
        }

        public async Task<bool> RemoveImageFromCloudinaryAsync(Image model, ApplicationUser user)
        {
            var result = await _cloudinary.DeleteResourcesAsync(model.Url);

            var status = result.Deleted.First().Value;

            return status != "not_found";
        }

        public async Task RemoveFolderFromCloudinaryAsync(int userId)
        {
            var userFolder = $"{_cloudinaryEnvironmentFolder}/users/{userId}";

            await _cloudinary.DeleteResourcesByPrefixAsync(userFolder);
            await _cloudinary.DeleteFolderAsync(userFolder);
        }

        public MagickImage Resize(MagickImage image, int width, int height)
        {
            MagickGeometry size = new MagickGeometry(width, height)
            {
                IgnoreAspectRatio = true
            };

            image.Resize(size);

            return image;
        }
    }
}
