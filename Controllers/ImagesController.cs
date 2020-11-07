using AutoMapper;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Interfaces;
using SACA.Models;
using SACA.Models.Dto;
using SACA.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    [Authorize]
    public class ImagesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public ImagesController(ApplicationDbContext context, UserManager<User> userManager, IImageService imageService, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> Get(int id)
        {
            var userId = int.Parse(_userManager.GetUserId(User));

            var image = await _context.Images
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.Id == id && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (image is null)
            {
                return Forbid();
            }

            var imageResponse = _mapper.Map<ImageResponse>(image);

            return Ok(imageResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ImageResponse>> Create(ImageRequest imageRequest)
        {
            var image = _mapper.Map<Image>(imageRequest);

            var userId = int.Parse(_userManager.GetUserId(User));
            image.UserId = userId;

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            magickImage.Resize(110, 150);

            imageRequest.Base64 = magickImage.ToBase64();

            (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest, userId);

            await _context.Images.AddAsync(image);

            var userCategory = new UserCategory { UserId = userId, CategoryId = image.CategoryId };

            var userCategoryExists = await _context.UserCategories.AnyAsync(uc => uc.CategoryId == userCategory.CategoryId && uc.UserId == userCategory.UserId);
            if (!userCategoryExists)
            {
                await _context.UserCategories.AddAsync(userCategory);
            }

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ImageResponse>> Update(int id, ImageRequest imageRequest)
        {
            var originalImage = await _context.Images.FirstOrDefaultAsync(c => c.Id == id);
            if (originalImage is null || originalImage.Id != imageRequest.Id)
            {
                return Forbid();
            }

            var user = await _userManager.GetUserAsync(User);

            await _imageService.RemoveImageFromCloudinaryAsync(originalImage, user);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            magickImage.Resize(110, 150);

            imageRequest.Base64 = magickImage.ToBase64();

            var image = _mapper.Map<Image>(originalImage);
            image.Name = imageRequest.Name;

            (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest, user.Id);


            _context.Entry(image).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ImageResponse>> Remove(int id)
        {
            var image = await _context.Images.FirstOrDefaultAsync(c => c.Id == id);
            if (image is null)
            {
                return BadRequest("Imagem inválida");
            }

            var user = await _userManager.GetUserAsync(User);

            await _imageService.RemoveImageFromCloudinaryAsync(image, user);

            _context.Remove(image);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }

        [HttpPost("superuser")]
        public async Task<ActionResult> CreateAdmin(ImageRequest imageRequest)
        {
            var image = _mapper.Map<Image>(imageRequest);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            imageRequest.Base64 = _imageService.Resize(magickImage, 110, 150).ToBase64();

            (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest, userId: null);

            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("superuser/{id}")]
        public async Task<ActionResult<ImageResponse>> RemoveAdmin(int id)
        {
            var image = await _context.Images.FirstOrDefaultAsync(c => c.Id == id);
            if (image is null)
            {
                return BadRequest("Imagem inválida");
            }

            await _imageService.RemoveImageFromCloudinaryAsync(image, user: null);

            _context.Remove(image);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }
    }
}
