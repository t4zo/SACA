using System.Security.Claims;

namespace SACA.Interfaces
{
    public interface ITokenService
    {
        string GenerateJWTToken(ClaimsIdentity claimsIdentity);
    }
}
