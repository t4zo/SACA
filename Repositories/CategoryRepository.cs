using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Models;
using SACA.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        async Task<IEnumerable<Category>> ICategoryRepository.GetAllAsync(int userId)
        {
            var categories = await _context.Categories.Include(c => c.Images)
               .Where(c => c.UserCategories.Any(uc => uc.UserId == userId))
               .AsNoTracking()
               .ToListAsync();

            foreach (var category in categories)
            {
                IList<Image> images = new List<Image>();

                foreach (var image in category.Images)
                {
                    if (image.UserId == null || image.UserId == userId)
                    {
                        images.Add(image);
                    }
                }

                category.Images = images;
            }

            return categories;
        }

        async Task<IEnumerable<Category>> ICategoryRepository.GetAllAsync()
        {
            return await _context.Categories.Include(c => c.Images).Where(c => c.Images.Any(c => c.CategoryId != 1)).AsNoTracking().ToListAsync();
        }

        async Task<Category> ICategoryRepository.GetAsync(int userId, int categoryId)
        {
            var category = await _context.Categories.Include(c => c.Images)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserCategories.Any(uc => uc.UserId == userId && uc.CategoryId == categoryId));

            IList<Image> images = new List<Image>();

            foreach (var image in category.Images)
            {
                if (image.UserId == null || image.UserId == userId)
                {
                    images.Add(image);
                }
            }

            category.Images = images;

            return category;
        }

        async Task ICategoryRepository.CreateAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }
    }
}
