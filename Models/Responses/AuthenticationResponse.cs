namespace SACA.Models.Responses
{
    public class AuthenticationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserResponse User { get; set; }
    }
}
