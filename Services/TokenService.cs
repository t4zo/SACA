using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SACA.Configurations;
using SACA.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SACA.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenConfiguration _tokenConfigurations;

        public TokenService(IConfiguration configuration, TokenConfiguration tokenConfigurations)
        {
            _configuration = configuration;
            _tokenConfigurations = tokenConfigurations;
        }

        string ITokenService.GenerateJWTToken(ClaimsIdentity claimsIdentity)
        {
            DateTime createdAt = DateTime.Now;
            DateTime expiresAt = DateTime.Now + TimeSpan.FromHours(_tokenConfigurations.Expiration);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                Subject = claimsIdentity,
                NotBefore = createdAt,
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["AppConfiguration:Token:Key"])), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
