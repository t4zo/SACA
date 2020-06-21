using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Models;
using SACA.Models.Dto;
using SACA.Repositories.Interfaces;
using SACA.Services.Interfaces;
using SACA.Transactions;
using SACA.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    [Route("saca/v2/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserCategoryRepository _userCategoryRepository;
        private readonly IUnityOfWork _uow;
        private readonly IMapper _mapper;

        public AuthController(
            IUserService userService,
            IImageService imageService,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            IUserCategoryRepository userCategoryRepository,
            IUnityOfWork uow,
            IMapper mapper
            )
        {
            _userService = userService;
            _imageService = imageService;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _userCategoryRepository = userCategoryRepository;
            _uow = uow;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<UserDto>> Authenticate(AuthenticationDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userDto = await _userService.AuthenticateAsync(model.Email, model.Password, model.Remember);

            if (userDto == null) return ValidationProblem("Usuário ou Senha Inválido(s)");

            return Ok(new ResponseSignInUserDto { Success = true, Message = "User logged!", User = userDto });
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<ActionResult<UserDto>> Create(SignUpDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categories = await _categoryRepository.GetAllAsync();

            var user = await _userService.Create(model);

            foreach (var category in categories)
            {
                await _userCategoryRepository.CreateAsync(new UserCategory { UserId = user.Id, CategoryId = category.Id });
            }

            await _uow.CommitAsync();

            var userDto = await _userService.AuthenticateAsync(model.Email, model.Password, remember: false);

            return Ok(new ResponseSignInUserDto { Success = true, Message = "User Created!", User = userDto });
        }

        [Authorize(Constants.Administrador)]
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var users = await _userService.GetUsersInRoleAsync(Constants.Usuario);

            var authenticationDtoUsers = _mapper.Map<IEnumerable<UserDto>>(users);

            return Ok(authenticationDtoUsers);
        }

        [Authorize(Constants.All)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //var _id = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var user = await _userService.FindByIdAsync(id);

            return Ok(user);
        }

        [Authorize(Constants.All)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userRepository.GetAsync(id);

            if (user == null) return BadRequest(ModelState);

            await _imageService.RemoveFolderFromCloudinaryAsync(user.Id);

            await _userService.Remove(user);

            await _uow.CommitAsync();

            return Ok();
        }
    }
}
