using System.Collections.Generic;

namespace SACA.Models.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string Token { get; set; }
    }
}
