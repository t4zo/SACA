namespace SACA.Models.Dto
{
    public class ResponseSignInUserDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserDto User { get; set; }
    }
}
