using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SACA.Configurations;
using SACA.Models;
using SACA.Models.Dto;
using SACA.Services.Interfaces;
using SACA.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SACA.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UserService(
            IConfiguration configuration, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IMapper mapper
            )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        async Task<UserDto> IUserService.AuthenticateAsync(string username, string password, bool remember)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: remember, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);
                var roles = await _userManager.GetRolesAsync(user);

                var appConfiguration = _configuration.GetSection("AppConfiguration").Get<AppConfiguration>();

                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["AppConfiguration:Token:SecurityKey"]);

                var claimsIdentity = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(Constants.RememberUser, remember.ToString().ToLower())
                    });

                claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claimsIdentity,
                    Issuer = appConfiguration.Token.Issuer,
                    Audience = appConfiguration.Token.Audience,
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                var userDto = _mapper.Map<UserDto>(user);

                userDto.Roles = roles;
                userDto.Token = token;

                return userDto.WithoutPassword();
            }

            return null;
        }

        async Task<UserDto> IUserService.Create(SignUpDto signUpDto)
        {
            if(signUpDto.Password != signUpDto.ConfirmPassword)
            {
                return null;
            }

            var user = new User
            {
                UserName = signUpDto.UserName,
                Email = signUpDto.Email,
            };

            var result = await _userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();

                throw new InvalidOperationException(error.Description);
            }

            foreach (var role in signUpDto.Roles)
            {
                if(Constants.AllRoles.Contains(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            await _userManager.AddToRoleAsync(user, Constants.Usuario);

            return _mapper.Map<UserDto>(user);
        }

        async Task<UserDto> IUserService.FindByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);
            return userDto;
        }

        async Task<IEnumerable<UserDto>> IUserService.GetUsersInRoleAsync(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);

            foreach (var userDto in usersDto)
            {
                var user = users.FirstOrDefault(u => u.Id == userDto.Id);
                userDto.Roles = await _userManager.GetRolesAsync(user);
            }

            return usersDto.AsEnumerable();
        }

        async Task<bool> IUserService.IsInRole(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        async Task IUserService.Remove(User user)
        {
            await _userManager.DeleteAsync(user);
        }
    }
}
