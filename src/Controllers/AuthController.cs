using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Constants;
using SACA.Entities.Identity;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Interfaces;
using SACA.Repositories.Interfaces;

namespace SACA.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnityOfWork _uow;
        private readonly IUserService _userService;

        public AuthController(
            IUserService userService,
            IS3Service s3Service,
            IMapper mapper,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository,
            IUnityOfWork unityOfWork
        )
        {
            _userService = userService;
            _s3Service = s3Service;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _uow = unityOfWork;
        }

        [AllowAnonymous]
        [HttpPost("signIn")]
        public async Task<ActionResult<UserResponse>> SignIn(SignInRequest signInRequest)
        {
            UserResponse userResponse;

            try
            {
                userResponse = await _userService.LogInAsync(signInRequest.Email, signInRequest.Password, signInRequest.Remember);
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(new ProblemDetails { Title = nameof(BadRequest), Detail = argumentException.Message });
            }

            return userResponse;
        }

        [AllowAnonymous]
        [HttpPost("signUp")]
        public async Task<ActionResult<UserResponse>> Create(SignUpRequest signUpRequest)
        {
            UserResponse userResponse;

            try
            {
                var applicationUser = await _userService.SignUpAsync(signUpRequest);
                await _userService.AssignRolesAsync(applicationUser, signUpRequest.Roles);
                userResponse = await _userService.LogInAsync(signUpRequest.Email, signUpRequest.Password);
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(new ProblemDetails { Title = nameof(BadRequest), Detail = argumentException.Message });
            }

            var user = await _userRepository.GetUserAsync(userResponse.Id);
            user.Categories = await _categoryRepository.GetCommonCategoriesAsync();

            await _uow.SaveChangesAsync();

            return userResponse;
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var usersResponses = await _userRepository.GetUsersAsync();

            foreach (var userResponse in usersResponses)
            {
                var user = _mapper.Map<ApplicationUser>(userResponse);
                userResponse.Roles = await _userService.GetRolesAsync(user);
            }

            return usersResponses;
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> Get(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user is null)
            {
                return BadRequest();
            }

            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.Roles = await _userService.GetRolesAsync(user);

            return userResponse;
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserResponse>> Remove(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user is null)
            {
                return BadRequest();
            }

            await _s3Service.RemoveFolderAsync(user.Id.ToString());
            await _userService.DeleteAsync(user);

            await _uow.SaveChangesAsync();

            return _mapper.Map<UserResponse>(user);
        }
    }
}