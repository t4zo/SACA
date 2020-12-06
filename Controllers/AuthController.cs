using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Constants;
using SACA.Data;
using SACA.Interfaces;
using SACA.Models.Identity;
using SACA.Models.Requests;
using SACA.Models.Responses;

namespace SACA.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public AuthController(ApplicationDbContext context, IUserService userService, IImageService imageService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _imageService = imageService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("signIn")]
        public async Task<ActionResult> SignIn(SignInRequest signInRequest)
        {
            var userOneOf = await _userService.LogInAsync(signInRequest.Email, signInRequest.Password, signInRequest.Remember);

            return userOneOf.Match<ActionResult>(
                user => Ok(new SignInResponse {Success = true, Message = "Usuário logado!", User = user}),
                argumentException => BadRequest(new ProblemDetails {Title = "Bad Request", Type = "https://httpstatuses.com/400", Detail = argumentException.Message})
            );
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Create(SignUpRequest signUpRequest)
        {
            UserResponse userResponse;
            try
            {
                userResponse = await _userService.SignInAsync(signUpRequest);
            }
            catch
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Bad Request", Type = "https://httpstatuses.com/400",
                    Detail = "Erro ao criar usuário, email e/ou senha inválido(s)"
                });
            }

            var user = await _context.Users.FindAsync(userResponse.Id);

            user.Categories = await _context.Categories
                .AsNoTracking()
                .Include(x => x.Images.Where(i => i.CategoryId.ToString() != "1"))
                .ToListAsync();

            await _context.SaveChangesAsync();

            var userOneOf = await _userService.LogInAsync(signUpRequest.Email, signUpRequest.Password);

            return userOneOf.Match<ActionResult>(
                userResult => Ok(new SignInResponse {Success = true, Message = "Usuário logado!", User = userResult}),
                argumentException => BadRequest(new ProblemDetails {Title = "Bad Request", Type = "https://httpstatuses.com/400", Detail = argumentException.Message})
            );
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var usersResponses = await _context.Users.ProjectTo<UserResponse>(_mapper.ConfigurationProvider).ToListAsync();

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
            var user = await _context.Users.FindAsync(id);
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
        public async Task<ActionResult<ApplicationUser>> Remove(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return BadRequest();
            }

            await _imageService.RemoveFolderFromCloudinaryAsync(user.Id);
            await _userService.DeleteAsync(user);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}