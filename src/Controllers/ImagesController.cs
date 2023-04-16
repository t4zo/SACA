using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Repositories.Interfaces;

namespace SACA.Controllers
{
    public class ImagesController : BaseApiController, IApiMarker
    {
        private readonly IImageService _imageService;
        private readonly IS3Service _s3Service;
        private readonly MapperlyMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnityOfWork _uow;

        public ImagesController(IImageService imageService, IS3Service s3Service, MapperlyMapper mapper, ICategoryRepository categoryRepository, IImageRepository imageRepository,
            IUserRepository userRepository, IUnityOfWork uow)
        {
            _imageService = imageService;
            _s3Service = s3Service;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _imageRepository = imageRepository;
            _userRepository = userRepository;
            _uow = uow;
        }

        [HttpGet]
        public async Task<ActionResult<ImageResponse>> Get()
        {
            var userId = User.GetId();
            if (userId is null)
            {
                return NoContent();
            }

            var imageResponse = await _imageRepository.GetUserImageAsync(userId.Value);

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
            if (userId is null)
            {
                return NoContent();
            }

            var imageResponse = await _imageRepository.GetUserImageAsync(userId.Value, id);

            if (imageResponse is null)
            {
                return NotFound();
            }

            return Ok(imageResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ImageResponse>> Create(ImageRequest imageRequest)
        {
            var image = _mapper.MapToImage(imageRequest);

            image.UserId = User.GetId();
            if (image.UserId is null)
            {
                return NoContent();
            }

            var user = await _userRepository.GetUserCategoryAsync(image.UserId.Value);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            magickImage.Resize(110, 150);

            imageRequest.Base64 = magickImage.ToBase64();

            image.Url = await _s3Service.UploadUserFileAsync(imageRequest.Base64, user.Id.ToString());

            await _imageRepository.AddAsync(image);

            var category = await _categoryRepository.GetCategoryUserAsync(image.CategoryId);

            var userCategoryExists = user.Categories.Any(x => x.Id == category.Id);
            if (!userCategoryExists)
            {
                category.ApplicationUsers.Add(user);
            }

            await _uow.SaveChangesAsync();

            return Ok(_mapper.MapToImageResponse(image));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ImageResponse>> Update(int id, ImageRequest imageRequest)
        {
            if (id != imageRequest.Id)
            {
                return NotFound("The image was not found");
                // throw new ImageNotFoundException("The image was not found");
            }

            var originalImage = await _imageRepository.GetAsync(id);

            if (originalImage is null)
            {
                return NotFound("The image was not found");
                // throw new ImageNotFoundException("The image was not found");
            }

            var userId = User.GetId();
            if (userId is null)
            {
                return NoContent();
            }

            var user = await _userRepository.GetUserAsync(userId.Value);

            await _s3Service.RemoveFileAsync(originalImage.Url);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            magickImage.Resize(110, 150);

            imageRequest.Base64 = magickImage.ToBase64();

            var image = _mapper.MapToImage(originalImage);
            image.Name = imageRequest.Name;

            image.Url = await _s3Service.UploadUserFileAsync(imageRequest.Base64, user.Id.ToString());

            await _uow.SaveChangesAsync();

            return Ok(_mapper.MapToImageResponse(image));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ImageResponse>> Remove(int id)
        {
            var image = await _imageRepository.GetAsync(id);
            if (image is null)
            {
                return NotFound();
            }

            await _s3Service.RemoveFileAsync(image.Url);

            _imageRepository.Remove(image);
            await _uow.SaveChangesAsync();

            return Ok(_mapper.MapToImageResponse(image));
        }

        [HttpPost("superuser")]
        public async Task<ActionResult<ImageResponse>> CreateAdmin(ImageRequest imageRequest)
        {
            var image = _mapper.MapToImage(imageRequest);

            using var magickImage = new MagickImage(Convert.FromBase64String(imageRequest.Base64));
            imageRequest.Base64 = _imageService.Resize(magickImage, 110, 150).ToBase64();

            image.Url = await _s3Service.UploadSharedFileAsync(imageRequest.Base64);

            await _imageRepository.AddAsync(image);
            await _uow.SaveChangesAsync();

            return Ok(_mapper.MapToImageResponse(image));
        }

        [HttpDelete("superuser/{id}")]
        public async Task<ActionResult<ImageResponse>> RemoveAdmin(int id)
        {
            var image = await _imageRepository.GetAsync(id);
            if (image is null)
            {
                return NotFound();
            }

            await _s3Service.RemoveFileAsync(image.Url);

            _imageRepository.Remove(image);
            await _uow.SaveChangesAsync();

            return Ok(_mapper.MapToImageResponse(image));
        }
    }
}