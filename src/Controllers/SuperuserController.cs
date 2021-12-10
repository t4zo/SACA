using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Constants;
using SACA.Entities.Identity;
using SACA.Entities.Responses;
using SACA.Interfaces;
using SACA.Repositories.Interfaces;

namespace SACA.Controllers
{
    [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
    public class SuperuserController : BaseApiController
    {
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUnityOfWork _uow;
        private readonly IUserService _userService;

        public SuperuserController(
            IUserService userService,
            IS3Service s3Service,
            IMapper mapper,
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

        [HttpGet("users")]
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

        [HttpGet("user")]
        public async Task<ActionResult<UserResponse>> Get([FromHeader] int userId)
        {
            var user = await _userRepository.GetUserAsync(userId);
            if (user is null)
            {
                return BadRequest();
            }

            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.Roles = await _userService.GetRolesAsync(user);

            return userResponse;
        }

        [HttpDelete("user")]
        public async Task<ActionResult<UserResponse>> Remove([FromHeader] int userId)
        {
            var user = await _userRepository.GetUserAsync(userId);
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