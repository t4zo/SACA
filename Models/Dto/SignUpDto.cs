using System.Collections.Generic;

namespace SACA.Models.Dto
{
    public class SignUpDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
