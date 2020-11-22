using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OneOf;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Models.Identity;
using SACA.Models.Requests;
using SACA.Models.Responses;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMapper mapper
        )
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<UserResponse> CreateAsync(SignUpRequest signUpRequest)
        {
            var user = new ApplicationUser
            {
                UserName = signUpRequest.UserName,
                Email = signUpRequest.Email
            };

            var result = await _userManager.CreateAsync(user, signUpRequest.Password);

            if (!result.Succeeded) throw new ArgumentException(result.Errors.First().Description);

            foreach (var role in signUpRequest.Roles)
                if (typeof(Roles).GetAllPublicConstantValues<string>().Contains(role))
                    await _userManager.AddToRoleAsync(user, role);

            await _userManager.AddToRoleAsync(user, Roles.User);

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<OneOf<UserResponse, ArgumentException>> SignInAsync(string username, string password,
            bool remember)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, remember, false);

            if (!result.Succeeded) return new ArgumentException("Usuário e/ou senha inválido(s)");

            var user = await _userManager.FindByNameAsync(username);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(Remember, remember.ToString().ToLower())
            });

            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = _tokenService.GenerateJWTToken(claimsIdentity);

            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.Roles = roles;
            userResponse.Token = token;

            return userResponse;
        }
    }
}