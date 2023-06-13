using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Constants;
using SACA.Entities;
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
        public async Task<ActionResult<ImageResponse>> Create(
            [FromForm] IFormFile file,
            [FromForm] int categoryId,
            [FromForm] int resizeWidth,
            [FromForm] int resizeHeight,
            [FromForm] bool compress
        )
        {
            var userId = User.GetId();
            if (!userId.HasValue)
            {
                return NoContent();
            }

            var applicationUser = await _userRepository.GetUserCategoryAsync(userId.Value);

            if (resizeWidth > 0 && resizeHeight > 0)
            {
                await _imageService.Resize(file, resizeWidth, resizeHeight);
            }

            if (compress)
            {
                // await _imageService.Compress(file);
            }
            
            var name = file.FileName.Contains('.') ? file.FileName[..file.FileName[..^1].LastIndexOf(".", StringComparison.Ordinal)] : file.FileName;
            
            var image = new Image
            {
                UserId = userId.Value,
                CategoryId = categoryId,
                Name = name,
                Url = await _s3Service.UploadUserFileAsync(file, applicationUser.Id.ToString())
            };

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
        public async Task<ActionResult<ImageResponse>> Update(
            int id,
            [FromForm] IFormFile file,
            [FromForm] int resizeWidth,
            [FromForm] int resizeHeight,
            [FromForm] bool compress
        )
        {
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

            if (resizeWidth > 0 && resizeHeight > 0)
            {
                await _imageService.Resize(file, resizeWidth, resizeHeight);
            }

            if (compress)
            {
                // await _imageService.Compress(file);
            }

            image.Name = file.FileName.Contains('.') ? file.FileName[..file.FileName[..^1].LastIndexOf(".", StringComparison.Ordinal)] : file.FileName;

            await _s3Service.RemoveFileAsync(originalImage.Url);
            image.Url = await _s3Service.UploadUserFileAsync(file, applicationUser.Id.ToString());

            await _uow.SaveChangesAsync();

            return Ok(_mapper.MapToImageResponse(image));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ImageResponse>> Remove(int id)
        {
            var userId = User.GetId();
            if (!userId.HasValue)
            {
                return NoContent();
            }

            var image = await _imageRepository.GetAsync(userId.Value, id);
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
        public async Task<ActionResult<ImageResponse>> SuperuserCreateImage(
            [FromForm] IFormFile file,
            [FromForm] int categoryId,
            [FromForm] int resizeWidth,
            [FromForm] int resizeHeight,
            [FromForm] bool compress
        )
        {
            if (resizeWidth > 0 && resizeHeight > 0)
            {
                await _imageService.Resize(file, resizeWidth, resizeHeight);
            }

            var name = file.FileName.Contains('.') ? file.FileName[..file.FileName[..^1].LastIndexOf(".", StringComparison.Ordinal)] : file.FileName;

            var image = new Image
            {
                CategoryId = categoryId,
                Name = name,
                Url = await _s3Service.UploadCommonFileAsync(file)
            };

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