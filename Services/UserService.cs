using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SACA.Constants;
using SACA.Interfaces;
using SACA.Models;
using SACA.Models.Requests;
using SACA.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SACA.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UserService(
            ITokenService tokenService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper
            )
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        // TODO
        async Task<UserResponse> IUserService.AuthenticateAsync(string username, string password, bool remember)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: remember, lockoutOnFailure: false);

            if (result.Succeeded)
            {
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

            return null;
        }

        private void AddUserClaims(ClaimsIdentity claimsIdentity, IList<Claim> userClaims, IList<string> roles)
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

        async Task<UserResponse> IUserService.CreateAsync(SignUpRequest signUpRequest)
        {
            var user = new User
            {
                UserName = signUpRequest.UserName,
                Email = signUpRequest.Email,
            };

            var result = await _userManager.CreateAsync(user, signUpRequest.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(result.Errors.FirstOrDefault().Description);
            }

            foreach (var role in signUpRequest.Roles)
            {
                if (AuthorizationConstants.Roles.All.Contains(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            await _userManager.AddToRoleAsync(user, AuthorizationConstants.Roles.User);

            return _mapper.Map<UserResponse>(user);
        }

        async Task<IEnumerable<UserResponse>> IUserService.GetUsersInRoleAsync(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            var usersDto = _mapper.Map<IEnumerable<UserResponse>>(users);

            foreach (var userDto in usersDto)
            {
                var user = users.FirstOrDefault(u => u.Id == userDto.Id);
                userDto.Roles = await _userManager.GetRolesAsync(user);
            }

            return usersDto.AsEnumerable();
        }

        async Task<bool> IUserService.IsInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        async Task IUserService.RemoveAsync(User user)
        {
            await _userManager.DeleteAsync(user);
        }
    }
}
