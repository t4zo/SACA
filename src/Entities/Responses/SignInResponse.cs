namespace SACA.Entities.Responses
{
    public class SignInResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserResponse User { get; set; }
    }
}