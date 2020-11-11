using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SACA.Constants;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Models.Identity;
using SACA.Models.Requests;
using SACA.Models.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

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

        public async Task<UserResponse> AuthenticateAsync(string username, string password, bool remember)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: remember, lockoutOnFailure: false);

            var user = await _userManager.FindByNameAsync(username);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var claimsIdentity = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(AuthorizationConstants.Remember, remember.ToString().ToLower())
                });

            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var token = _tokenService.GenerateJWTToken(claimsIdentity);

            AddUserClaims(claimsIdentity, userClaims, roles);

            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.Roles = roles;
            userResponse.Token = token;

            return userResponse;
        }

        private static void AddUserClaims(ClaimsIdentity claimsIdentity, IList<Claim> userClaims, IList<string> roles)
        {
            foreach (var role in roles)
            {
                if (role.Equals(AuthorizationConstants.Roles.Superuser))
                {
                    foreach (var userClaim in userClaims)
                    {
                        claimsIdentity.AddClaim(new Claim(userClaim.Type, userClaim.Value));
                    }
                }

                if (role.Equals(AuthorizationConstants.Roles.User))
                {
                    foreach (var userClaim in userClaims)
                    {
                        claimsIdentity.AddClaim(new Claim(userClaim.Type, userClaim.Value));
                    }
                }
            }
        }

        public async Task<UserResponse> CreateAsync(SignUpRequest signUpRequest)
        {
            var user = new ApplicationUser
            {
                UserName = signUpRequest.UserName,
                Email = signUpRequest.Email,
            };

            await _userManager.CreateAsync(user, signUpRequest.Password);

            foreach (var role in signUpRequest.Roles)
            {
                if (typeof(Roles).GetAllPublicConstantValues<string>().Contains(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            await _userManager.AddToRoleAsync(user, AuthorizationConstants.Roles.User);

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<IEnumerable<UserResponse>> GetUsersInRoleAsync(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            var usersResponse = _mapper.Map<IEnumerable<UserResponse>>(users);

            foreach (var userResponse in usersResponse)
            {
                var user = users.FirstOrDefault(u => u.Id == userResponse.Id);
                userResponse.Roles = await _userManager.GetRolesAsync(user);
            }

            return usersResponse;
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task RemoveAsync(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }
    }
}
