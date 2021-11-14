using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Constants;
using SACA.Data;
using SACA.Entities.Identity;
using SACA.Entities.Requests;
using SACA.Entities.Responses;
using SACA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public AuthController(ApplicationDbContext context, IUserService userService, IS3Service s3Service, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _s3Service = s3Service;
            _mapper = mapper;
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

            var user = await _context.Users.FindAsync(userResponse.Id);
            user.Categories = await _context.Categories
                .AsNoTracking()
                .Where(x => x.Id != 1)
                .ToListAsync();

            await _context.SaveChangesAsync();

            return userResponse;
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
        public async Task<ActionResult<UserResponse>> Remove(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return BadRequest();
            }

            await _s3Service.RemoveFolderAsync(user.Id.ToString());
            await _userService.DeleteAsync(user);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserResponse>(user);
        }
    }
}