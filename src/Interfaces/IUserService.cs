using Microsoft.AspNetCore.Identity;
using SACA.Models.Identity;
using SACA.Models.Requests;
using SACA.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

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