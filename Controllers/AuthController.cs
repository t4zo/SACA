using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Constants;
using SACA.Data;
using SACA.Interfaces;
using SACA.Models;
using SACA.Models.Requests;
using SACA.Models.Responses;
using SACA.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUnityOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public AuthController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IUnityOfWork uow,
            IMapper mapper,
            IUserService userService,
            IImageService imageService
            )
        {
            _context = context;
            _userManager = userManager;
            _uow = uow;
            _mapper = mapper;
            _userService = userService;
            _imageService = imageService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<UserResponse>> Authenticate(AuthenticationRequest authenticationRequest)
        {
            var userResponse = await _userService.AuthenticateAsync(authenticationRequest.Email, authenticationRequest.Password, authenticationRequest.Remember);
            if (userResponse is null)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "first", new[] { "Email e/ou senha inválido(s)" } }
                };
                return ValidationProblem(new ValidationProblemDetails(errors));
            }

            return Ok(new AuthenticationResponse { Success = true, Message = "Usuário logado!", User = userResponse });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Create(SignUpRequest signUpRequest)
        {
            UserResponse user;

            try
            {
                user = await _userService.CreateAsync(signUpRequest);
            }
            catch (InvalidOperationException ex)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "first", new[] { ex.Message } } 
                };
                return ValidationProblem(new ValidationProblemDetails(errors));
            }

            var categories = await _context.Categories
                .Include(x => x.Images)
                .Where(x => x.Images.Any(x => x.CategoryId != 1))
                .ToListAsync();

            foreach (var category in categories)
            {
                await _context.UserCategories.AddAsync(new UserCategory { UserId = user.Id, CategoryId = category.Id });
            }

            await _uow.CommitAsync();

            //return RedirectToAction(nameof(Authenticate), new AuthenticationRequest { Email = signUpRequest.Email, Password = signUpRequest.Password });
            var userResponse = await _userService.AuthenticateAsync(signUpRequest.Email, signUpRequest.Password);
            return Ok(new AuthenticationResponse { Success = true, Message = "Usuário cadastrado!", User = userResponse });
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersResponses = _mapper.Map<IEnumerable<UserResponse>>(users);

            foreach (var userResponse in usersResponses)
            {
                var user = users.FirstOrDefault(x => x.Id == userResponse.Id);
                userResponse.Roles = await _userManager.GetRolesAsync(user);
            }

            return Ok(usersResponses);
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.Roles = await _userManager.GetRolesAsync(user);

            return Ok(userResponse);
        }

        [Authorize(Roles = AuthorizationConstants.Roles.Superuser)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Remove(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null) return BadRequest("Usuário inválido");

            await _imageService.RemoveFolderFromCloudinaryAsync(user.Id);
            await _userService.RemoveAsync(user);

            await _uow.CommitAsync();

            return Ok(user);
        }
    }
}
