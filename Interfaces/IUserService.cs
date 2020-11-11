using SACA.Models.Identity;
using SACA.Models.Requests;
using SACA.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> AuthenticateAsync(string email, string password, bool remember = false);
        Task<UserResponse> CreateAsync(SignUpRequest signUpDto);
        Task<IEnumerable<UserResponse>> GetUsersInRoleAsync(string role);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
        Task RemoveAsync(ApplicationUser user);
    }
}
