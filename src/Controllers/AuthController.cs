using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Extensions;
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
        [HttpPost("signin")]
        public async Task<ActionResult<UserResponse>> SignIn(SignInRequest signInRequest)
        {
            try
            {
                return await _userService.LogInAsync(signInRequest.Email, signInRequest.Password, signInRequest.Remember);
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(new ProblemDetails { Title = nameof(BadRequest), Detail = argumentException.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<UserResponse>> Create(SignUpRequest signUpRequest)
        {
            try
            {
                var applicationUser = await _userService.SignUpAsync(signUpRequest);
                await _userService.AssignRolesAsync(applicationUser, signUpRequest.Roles);
                var userResponse = await _userService.LogInAsync(signUpRequest.Email, signUpRequest.Password);

                var user = await _userRepository.GetUserAsync(userResponse.Id);
                user.Categories = await _categoryRepository.GetCommonCategoriesAsync();

                await _uow.SaveChangesAsync();

                return userResponse;
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(new ProblemDetails { Title = nameof(BadRequest), Detail = argumentException.Message });
            }
        }

        [HttpDelete("user")]
        public async Task<ActionResult<UserResponse>> Remove()
        {
            var userId = User.GetId();
            if (!userId.HasValue)
            {
                return BadRequest();
            }

            var user = await _userRepository.GetUserAsync(userId.Value);
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