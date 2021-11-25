using SACA.Entities;

namespace SACA.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetCategoryQuery();
        Task<List<Category>> GetUserCategoriesAsync(IQueryable<Category> baseQuery, int userId);
        Task<Category> GetUserCategoryAsync(int userId, int categoryId);
        Task<List<Category>> GetCommonCategoriesAsync();
        Task<Category> GetCategoryUserAsync(int categoryId);
    }
}
