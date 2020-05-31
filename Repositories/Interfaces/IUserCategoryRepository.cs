using SACA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Repositories.Interfaces
{
    public interface IUserCategoryRepository
    {
        Task CreateAsync(UserCategory userCategory);
        Task CreateForAllAsync(IEnumerable<User> users, int categoryId);
        Task<bool> ExistsAsync(UserCategory userCategory);
    }
}
