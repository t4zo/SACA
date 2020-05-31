using SACA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(int userId);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetAsync(int userId, int categoryId);
        Task CreateAsync(Category category);
    }
}
