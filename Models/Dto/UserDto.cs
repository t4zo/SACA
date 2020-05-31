using System.Collections.Generic;

namespace SACA.Models.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string Token { get; set; }

        public UserDto WithoutPassword()
        {
            Password = "";
            return this;
        }
    }
}
