using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Entities;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    public class ImagesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public ImagesController(ApplicationDbContext context, IImageService imageService, IMapper mapper)
        {
            _context = context;
            _imageService = imageService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ImageResponse>> Get()
        {
            var userId = User.GetId();

            var imageResponse = await _context.Images
                .AsNoTracking()
                .Include(x => x.User.Id == userId)
                .Where(x => x.UserId == userId)
                .ProjectTo<ImageResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (imageResponse is null)
            {
                return NotFound();
            }

            return Ok(imageResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImageResponse>> Get(int id)
        {
            var userId = User.GetId();

            var imageResponse = await _context.Images
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.Id == id && x.UserId == userId)
                .ProjectTo<ImageResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (imageResponse is null)
            {
                return NotFound();
            }

            return Ok(imageResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ImageResponse>> Create(ImageRequest imageRequest)
        {
            var image = _mapper.Map<Image>(imageRequest);

            image.UserId = User.GetId();

            var user = await _context.Users
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == image.UserId);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            magickImage.Resize(110, 150);

            imageRequest.Base64 = magickImage.ToBase64();

            (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest, user.Id);

            await _context.Images.AddAsync(image);

            var category = await _context.Categories
                .Include(x => x.ApplicationUsers)
                .FirstOrDefaultAsync(x => x.Id == image.CategoryId);

            var userCategoryExists = user.Categories.Any(x => x.Id == category.Id);
            if (!userCategoryExists)
            {
                category.ApplicationUsers.Add(user);
            }

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ImageResponse>> Update(int id, ImageRequest imageRequest)
        {
            if (id != imageRequest.Id)
            {
                return NotFound("The image was not found");
                // throw new ImageNotFoundException("The image was not found");
            }
            var originalImage = await _context.Images.FirstOrDefaultAsync(c => c.Id == id);
            
            if (originalImage is null)
            {
                return NotFound("The image was not found");
                // throw new ImageNotFoundException("The image was not found");
            }
            
            var userId = User.GetId();
            var user = await _context.Users.FindAsync(userId);

            await _imageService.RemoveImageFromCloudinaryAsync(originalImage);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            magickImage.Resize(110, 150);

            imageRequest.Base64 = magickImage.ToBase64();

            var image = _mapper.Map<Image>(originalImage);
            image.Name = imageRequest.Name;

            (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest, user.Id);

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ImageResponse>> Remove(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image is null)
            {
                return NotFound();
            }

            await _imageService.RemoveImageFromCloudinaryAsync(image);

            _context.Remove(image);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }

        [HttpPost("superuser")]
        public async Task<ActionResult<ImageResponse>> CreateAdmin(ImageRequest imageRequest)
        {
            var image = _mapper.Map<Image>(imageRequest);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            imageRequest.Base64 = _imageService.Resize(magickImage, 110, 150).ToBase64();

            (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest);

            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }

        [HttpDelete("superuser/{id}")]
        public async Task<ActionResult<ImageResponse>> RemoveAdmin(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image is null)
            {
                return NotFound();
            }

            await _imageService.RemoveImageFromCloudinaryAsync(image);

            _context.Remove(image);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<ImageResponse>(image));
        }
    }
}