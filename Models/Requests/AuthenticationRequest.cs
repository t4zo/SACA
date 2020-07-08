namespace SACA.Models.Requests
{
    public class AuthenticationRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
