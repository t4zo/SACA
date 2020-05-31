using AutoMapper;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Models;
using SACA.Models.Dto;
using SACA.Repositories.Interfaces;
using SACA.Services.Interfaces;
using SACA.Transactions;
using SACA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    [Route("saca/v2/[controller]")]
    [Authorize(Constants.All)]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserCategoryRepository _userCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IUnityOfWork _uow;

        public ImagesController(
            IUserRepository userRepository, 
            IImageRepository imageRepository, 
            IUserCategoryRepository userCategoryRepository, 
            ICategoryRepository categoryRepository, 
            IImageService imageService,
            IMapper mapper,
            IUnityOfWork uow
            )
        {
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _userCategoryRepository = userCategoryRepository;
            _categoryRepository = categoryRepository;
            _imageService = imageService;
            _mapper = mapper;
            _uow = uow;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Image>>> GetAll(int userId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userRepository.GetAsync(userId);

            if (user == null) return BadRequest(ModelState);

            var categories = await _categoryRepository.GetAllAsync(userId);

            if (categories == null) return BadRequest(ModelState);

            var images = categories.SelectMany(c => c.Images);

            if (images == null) return BadRequest(ModelState);

            images.ToList().ForEach(image => image.Category = null);

            return Ok(images);
        }

        [HttpGet("{userId}/{imageId}")]
        public async Task<ActionResult<IEnumerable<Image>>> Get(int userId, int imageId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userRepository.GetAsync(userId);

            if (user == null) return BadRequest(ModelState);

            var image = await _imageRepository.GetAsync(imageId);

            image.User = null;

            if (image == null || image.UserId != user.Id) return BadRequest(ModelState);

            return Ok(image);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult<Image>> Create(int userId, ImageDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userRepository.GetAsync(userId);

            if (user == null) return BadRequest(ModelState);

            model = ResizeImage(model);

            var image = _mapper.Map<Image>(model);

            image.Url = await _imageService.UploadToCloudinaryAsync(model, userId);
            image.UserId = user.Id;

            await _imageRepository.CreateAsync(image, userId);

            var userCategory = new UserCategory { UserId = user.Id, CategoryId = image.CategoryId };

            if (!await _userCategoryRepository.ExistsAsync(userCategory))
            {
                await _userCategoryRepository.CreateAsync(userCategory);
            }

            await _uow.CommitAsync();

            image.User = null;

            return Ok(image);
        }

        [HttpPut("{userId}/{imageId}")]
        public async Task<ActionResult<Image>> Update(int userId, int imageId, ImageDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userRepository.GetAsync(userId);

            if (user == null) return BadRequest(ModelState);

            var oldImage = await _imageRepository.GetAsync(imageId);

            if (oldImage == null || (oldImage.Id != model.Id)) return BadRequest(ModelState);

            await _imageService.RemoveImageFromCloudinaryAsync(oldImage, user);

            var image = _mapper.Map<Image>(oldImage);

            model = ResizeImage(model);

            image.Name = model.Name;
            image.Url = await _imageService.UploadToCloudinaryAsync(model, user.Id);

            await _imageRepository.UpdateAsync(image);

            await _uow.CommitAsync();

            image.User = null;

            return Ok(image);
        }

        [HttpPost("admin/{adminId}")]
        [Authorize(Constants.Administrador)]
        public async Task<ActionResult<IReadOnlyCollection<Image>>> CreateAdmin(int adminId, ImageDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var admin = await _userRepository.GetAsync(adminId);

            if (admin == null) return BadRequest(ModelState);

            using MagickImage magickImage = new MagickImage(Convert.FromBase64String(model.Base64));

            model.Base64 = _imageService.Resize(magickImage, 110, 150).ToBase64();

            var image = _mapper.Map<Image>(model);

            image.Url = await _imageService.UploadToCloudinaryAsync(model, userId: null);

            await _imageRepository.CreateAsync(image, userId: null);

            await _uow.CommitAsync();

            return RedirectToAction("GetAll", new { userId = admin.Id });
        }

        [HttpDelete("{userId}/{imageId}")]
        public async Task<ActionResult<Image>> Remove(int userId, int imageId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userRepository.GetAsync(userId);

            if (user == null) return BadRequest(ModelState);

            var image = await _imageRepository.GetAsync(imageId);

            if (image == null) return BadRequest(ModelState);

            var removed = await _imageService.RemoveImageFromCloudinaryAsync(image, user);

            if (!removed) return BadRequest("Arquivo não encontrado");

            _imageRepository.Remove(image);

            await _uow.CommitAsync();

            image.User = null;

            return Ok(image);
        }

        [Authorize(Constants.Administrador)]
        [HttpDelete("{imageId}")]
        public async Task<ActionResult<Image>> Remove(int imageId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var image = await _imageRepository.GetAsync(imageId);

            if (image == null) return BadRequest(ModelState);

            var removed = await _imageService.RemoveImageFromCloudinaryAsync(image, user: null);

            if (!removed) return BadRequest("Arquivo não encontrado");

            _imageRepository.Remove(image);

            await _uow.CommitAsync();

            image.User = null;

            return Ok(image);
        }

        private ImageDto ResizeImage(ImageDto model)
        {
            using MagickImage magickImage = new MagickImage(Convert.FromBase64String(model.Base64));

            model.Base64 = _imageService.Resize(magickImage, 110, 150).ToBase64();

            return model;
        }
    }
}
