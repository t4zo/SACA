using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SACA.Options
{
    public class TokenOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecurityKey { get; set; }
        public int Expiration { get; set; }
        public SymmetricSecurityKey Key => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));
        public SigningCredentials SigningCredentials => new SigningCredentials(Key, SecurityAlgorithms.HmacSha256Signature);
    }
}
