using Microsoft.IdentityModel.Tokens;
using SACA.Configurations;
using SACA.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SACA.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenConfiguration _tokenConfigurations;

        public TokenService(TokenConfiguration tokenConfigurations)
        {
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
                SigningCredentials = _tokenConfigurations.SigningCredentials
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
