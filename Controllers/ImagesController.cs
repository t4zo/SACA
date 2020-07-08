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
using SACA.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Controllers
{
    [Authorize(Permissions.Images.View)]
    public class ImagesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IUnityOfWork _uow;

        public ImagesController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IImageService imageService,
            IMapper mapper,
            IUnityOfWork uow
            )
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _mapper = mapper;
            _uow = uow;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ImageResponse>>> Get(int id)
        {
            var userId = _userManager.GetUserId(User);

            var image = await _context.Images.Include(x => x.User)
                .Where(x => x.Id == id)
                .Where(x => x.UserId == int.Parse(userId))
                .FirstOrDefaultAsync();

            if (image is null)
            {
                return Forbid();
            }

            var imageResponse = _mapper.Map<ImageResponse>(image);

            return Ok(imageResponse);
        }

        [Authorize(Permissions.Images.Create)]
        [HttpPost]
        public async Task<ActionResult<ImageResponse>> Create(ImageRequest imageRequest)
        {
            var userId = int.Parse(_userManager.GetUserId(User));

            var image = _mapper.Map<Image>(imageRequest);
            image.UserId = userId;

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            imageRequest.Base64 = _imageService.Resize(magickImage, 110, 150).ToBase64();

            try
            {
                (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            await _context.Images.AddAsync(image);

            var userCategory = new UserCategory { UserId = userId, CategoryId = image.CategoryId };
            var userCategoryExists = await _context.UserCategories.AnyAsync(uc => uc.CategoryId == userCategory.CategoryId && uc.UserId == userCategory.UserId);
            if (!userCategoryExists)
            {
                await _context.UserCategories.AddAsync(userCategory);
            }

            await _uow.CommitAsync();

            var imageResponse = _mapper.Map<ImageResponse>(image);
            return Ok(imageResponse);
        }

        [Authorize(Permissions.Images.Update)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ImageResponse>> Update(int id, ImageRequest imageRequest)
        {
            var originalImage = await _context.Images.FirstOrDefaultAsync(c => c.Id == id);
            if (originalImage is null || (originalImage.Id != imageRequest.Id))
            {
                return Forbid();
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(id.ToString());
            Image image;

            try
            {
                await _imageService.RemoveImageFromCloudinaryAsync(originalImage, user);

                using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
                imageRequest.Base64 = _imageService.Resize(magickImage, 110, 150).ToBase64();

                image = _mapper.Map<Image>(originalImage);
                image.Name = imageRequest.Name;

                (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest, int.Parse(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            _context.Entry(image).State = EntityState.Modified;
            await _uow.CommitAsync();

            var imageResponse = _mapper.Map<ImageResponse>(image);
            return Ok(imageResponse);
        }

        [Authorize(Permissions.Images.Delete)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ImageResponse>> Remove(int id)
        {
            var image = await _context.Images.FirstOrDefaultAsync(c => c.Id == id);
            if (image is null)
            {
                return BadRequest("Imagem inválida");
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(id.ToString());

            try
            {
                await _imageService.RemoveImageFromCloudinaryAsync(image, user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            _context.Remove(image);
            await _uow.CommitAsync();

            var imageResponse = _mapper.Map<ImageResponse>(image);
            return Ok(imageResponse);
        }

        [Authorize(Permissions.Images.Create)]
        [HttpPost("superuser")]
        [Authorize(Roles = Roles.Superuser)]
        public async Task<ActionResult> CreateAdmin(ImageRequest imageRequest)
        {
            var image = _mapper.Map<Image>(imageRequest);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            imageRequest.Base64 = _imageService.Resize(magickImage, 110, 150).ToBase64();

            try
            {
                (image.FullyQualifiedPublicUrl, image.Url) = await _imageService.UploadToCloudinaryAsync(imageRequest, userId: null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            await _context.Images.AddAsync(image);
            await _uow.CommitAsync();

            return Ok();
        }

        [Authorize(Permissions.Images.Delete)]
        [Authorize(Roles = Roles.Superuser)]
        [HttpDelete("superuser/{id}")]
        public async Task<ActionResult<ImageResponse>> RemoveAdmin(int id)
        {
            var image = await _context.Images.FirstOrDefaultAsync(c => c.Id == id);
            if (image is null)
            {
                return BadRequest("Imagem inválida");
            }

            try
            {
                await _imageService.RemoveImageFromCloudinaryAsync(image, user: null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            _context.Remove(image);
            await _uow.CommitAsync();

            var imageResponse = _mapper.Map<ImageResponse>(image);
            return Ok(imageResponse);
        }
    }
}
