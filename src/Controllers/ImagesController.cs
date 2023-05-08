using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Constants;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Repositories.Interfaces;

namespace SACA.Controllers
{
    public class ImagesController : BaseApiController
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
            if (!userId.HasValue)
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
            if (!userId.HasValue)
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
            var userId = User.GetId();
            if (!userId.HasValue)
            {
                return NoContent();
            }
            
            var image = _mapper.MapToImage(imageRequest);
            var applicationUser = await _userRepository.GetUserCategoryAsync(userId.Value);
            image.UserId = userId.Value;
            
            var resizedImage = _imageService.Resize(new MagickImage(Convert.FromBase64String(imageRequest.Base64))).ToBase64();
            image.Url = await _s3Service.UploadUserFileAsync(resizedImage, applicationUser.Id.ToString());

            var category = await _categoryRepository.GetCategoryAsync(image.CategoryId);
            if (applicationUser.Categories.All(x => x.Id != category.Id))
            {
                category.ApplicationUsers.Add(applicationUser);
            }
            
            await _imageRepository.AddAsync(image);
            await _uow.SaveChangesAsync();

            return Ok(_mapper.MapToImageResponse(image));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ImageResponse>> Update(int id, ImageRequest imageRequest)
        {
            if (id != imageRequest.Id)
            {
                return NotFound("Image was not found");
            }

            var userId = User.GetId();
            if (!userId.HasValue)
            {
                return NoContent();
            }
            
            var originalImage = await _imageRepository.GetAsync(id);
            if (originalImage is null)
            {
                return NotFound("Image was not found");
            }

            var applicationUser = await _userRepository.GetUserAsync(userId.Value);
            var image = _mapper.MapToImage(originalImage);
            var resizedImage = _imageService.Resize(new MagickImage(Convert.FromBase64String(imageRequest.Base64))).ToBase64();

            await _s3Service.RemoveFileAsync(originalImage.Url);

            image.Name = imageRequest.Name;
            image.Url = await _s3Service.UploadUserFileAsync(resizedImage, applicationUser.Id.ToString());

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

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpPost("superuser")]
        public async Task<ActionResult<ImageResponse>> SuperuserCreateImage(ImageRequest imageRequest)
        {
            var image = _mapper.MapToImage(imageRequest);

            var resizedImage = _imageService.Resize(new MagickImage(Convert.FromBase64String(imageRequest.Base64))).ToBase64();
            image.Url = await _s3Service.UploadSharedFileAsync(resizedImage);

            await _imageRepository.AddAsync(image);
            await _uow.SaveChangesAsync();

            return Ok(_mapper.MapToImageResponse(image));
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpDelete("superuser/{id}")]
        public async Task<ActionResult<ImageResponse>> SuperuserRemoveImage(int id)
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