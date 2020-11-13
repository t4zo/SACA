using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Constants;
using SACA.Data;
using SACA.Interfaces;
using SACA.Models.Identity;
using SACA.Models.Requests;
using SACA.Models.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public AuthController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IUserService userService, IImageService imageService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _userService = userService;
            _imageService = imageService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<SignInResponse>> Authenticate(SignInRequest authenticationRequest)
        {
            var userResponse = await _userService.SignInAsync(authenticationRequest.Email, authenticationRequest.Password, authenticationRequest.Remember);

            return new SignInResponse { Success = true, Message = "Usuário logado!", User = userResponse };
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<SignInResponse>> Create(SignUpRequest signUpRequest)
        {
            var userCreated = await _userService.CreateAsync(signUpRequest);
            var user = await _userManager.FindByIdAsync(userCreated.Id.ToString());

            user.Categories = await _context.Categories
                .Include(x => x.Images)
                .AsNoTracking()
                .Where(x => x.Images.Any(x => x.CategoryId != 1))
                .ToListAsync();

            await _context.SaveChangesAsync();

            var userResponse = await _userService.SignInAsync(signUpRequest.Email, signUpRequest.Password);
            return new SignInResponse { Success = true, Message = "Usuário cadastrado!", User = userResponse };
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersResponses = _mapper.Map<List<UserResponse>>(users);

            foreach (var userResponse in usersResponses)
            {
                var user = users.FirstOrDefault(x => x.Id == userResponse.Id);
                userResponse.Roles = await _userManager.GetRolesAsync(user);
            }

            return usersResponses;
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.Roles = await _userManager.GetRolesAsync(user);

            return userResponse;
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApplicationUser>> Remove(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null) return BadRequest("Usuário inválido");

            await _imageService.RemoveFolderFromCloudinaryAsync(user.Id);
            await _userManager.DeleteAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}
