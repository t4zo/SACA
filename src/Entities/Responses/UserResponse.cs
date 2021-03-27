using System.Collections.Generic;

namespace SACA.Entities.Responses
{
    public class UserResponse : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<string> Roles { get; set; }
        public string Token { get; set; }
    }
}