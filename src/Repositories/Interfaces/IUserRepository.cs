using SACA.Entities;
using SACA.Entities.Identity;
using SACA.Entities.Responses;

namespace SACA.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserResponse>> GetUsersAsync();
        Task<ApplicationUser> GetUserAsync(int id);
        Task<ApplicationUser> GetUserCategoryAsync(int userId);
    }
}
