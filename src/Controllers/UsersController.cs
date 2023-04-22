using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Constants;
using SACA.Entities.Responses;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Repositories.Interfaces;

namespace SACA.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IS3Service _s3Service;
        private readonly MapperlyMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUnityOfWork _uow;
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService,
            IS3Service s3Service,
            MapperlyMapper mapper,
            IUserRepository userRepository,
            IUnityOfWork unityOfWork
        )
        {
            _userService = userService;
            _s3Service = s3Service;
            _mapper = mapper;
            _userRepository = userRepository;
            _uow = unityOfWork;
        }
        
        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var usersResponse = await _userRepository.GetUsersAsync();

            foreach (var userResponse in usersResponse)
            {
                var user = _mapper.MapToApplicationUser(userResponse);
                userResponse.Roles = await _userService.GetRolesAsync(user);
            }

            return usersResponse;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> Get(int id)
        {
            try
            {
                var applicationUser = await _userRepository.GetUserAsync(id);
                if (applicationUser is null)
                {
                    return BadRequest();
                }
                
                var userResponse = _mapper.MapToUserResponse(applicationUser);
                userResponse.Roles = await _userService.GetRolesAsync(applicationUser);

                return userResponse;
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(new ProblemDetails { Title = nameof(BadRequest), Detail = argumentException.Message });
            }
        }
    }
}