using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SACA.Options;
using SACA.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SACA.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppOptions _appOptions;

        public TokenService(IOptionsSnapshot<AppOptions> options)
        {
            _appOptions = options.Value;
        }

        public string GenerateJWTToken(ClaimsIdentity claimsIdentity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = _appOptions.Token.Issuer,
                Audience = _appOptions.Token.Audience,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(_appOptions.Token.Expiration),
                SigningCredentials = _appOptions.Token.SigningCredentials
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}
