using System.ComponentModel.DataAnnotations;

namespace SACA.Models.Dto
{
    public class AuthenticationDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool Remember { get; set; }
    }
}
