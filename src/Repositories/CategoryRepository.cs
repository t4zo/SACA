using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Entities;
using SACA.Repositories.Interfaces;

namespace SACA.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Category> GetCategoryQuery()
        {
            return _context.Categories
                .AsNoTracking()
                .Include(x => x.Images);
        }

        public async Task<Category> GetCategoryUserAsync(int categoryId)
        {
            return await _context.Categories
                .Include(x => x.ApplicationUsers)
                .FirstOrDefaultAsync(x => x.Id == categoryId);
        }

        public async Task<List<Category>> GetCommonCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(x => x.Id != 1)
                .ToListAsync();
        }

        public async Task<List<Category>> GetUserCategoriesAsync(IQueryable<Category> baseQuery, int userId)
        {
            return await baseQuery
                .Include(x => x.ApplicationUsers.Where(y => y.Id == userId))
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Images = c.Images.Where(i => i.UserId == null || i.UserId == userId).Select(i => new Image
                    {
                        Id = i.Id,
                        Name = i.Name,
                        CategoryId = i.CategoryId,
                        UserId = i.UserId,
                        Url = i.Url,
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<Category> GetUserCategoryAsync(int userId, int categoryId)
        {
            return await _context.Categories
                .AsNoTracking()
                .Include(x => x.Images)
                .Include(x => x.ApplicationUsers.Where(y => y.Id == userId))
                .Where(x => x.Id == categoryId)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Images = c.Images.Where(i => i.UserId == null || i.UserId == userId).Select(i => new Image
                    {
                        Id = i.Id,
                        Name = i.Name,
                        CategoryId = i.CategoryId,
                        UserId = i.UserId,
                        Url = i.Url,
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}
