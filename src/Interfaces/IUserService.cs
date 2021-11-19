using Microsoft.AspNetCore.Identity;
using SACA.Entities.Identity;
using SACA.Entities.Requests;
using SACA.Entities.Responses;

namespace SACA.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> LogInAsync(string email, string password, bool remember = false);
        Task<ApplicationUser> SignUpAsync(SignUpRequest signUpDto);
        Task<ApplicationUser> AssignRolesAsync(ApplicationUser user, ICollection<string> roles);
        Task<string> GetUserNameAsync(int userId);
        Task<IdentityResult> DeleteAsync(int userId);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<ICollection<string>> GetRolesAsync(ApplicationUser user);
    }
}