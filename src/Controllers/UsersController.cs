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
        private readonly MapperlyMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService,
            MapperlyMapper mapper,
            IUserRepository userRepository
        )
        {
            _userService = userService;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        
        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var users = await _userRepository.GetUsersAsync();

            foreach (var user in users)
            {
                var applicationUser = _mapper.MapToApplicationUser(user);
                user.Roles = await _userService.GetRolesAsync(applicationUser);
            }

            return users;
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