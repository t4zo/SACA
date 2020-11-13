using SACA.Models.Requests;
using SACA.Models.Responses;
using System.Threading.Tasks;

namespace SACA.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> SignInAsync(string email, string password, bool remember = false);
        Task<UserResponse> CreateAsync(SignUpRequest signUpDto);
    }
}
