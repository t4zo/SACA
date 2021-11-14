using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Entities;
using SACA.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var baseQuery = _context.Categories
                .AsNoTracking()
                .Include(x => x.Images);

            if (User.Identity is null || !User.Identity.IsAuthenticated)
            {
                return await baseQuery
                    .Where(x => x.Id != 1)
                    .ToListAsync();
            }

            var userId = User.GetId();

            var categories = await baseQuery
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
                        FullyQualifiedPublicUrl = i.FullyQualifiedPublicUrl
                    }).ToList()
                })
                .ToListAsync();

            return categories;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var userId = User.GetId();

            var category = await _context.Categories
                .AsNoTracking()
                .Include(x => x.Images)
                .Include(x => x.ApplicationUsers.Where(y => y.Id == userId))
                .Where(x => x.Id == id)
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
                        FullyQualifiedPublicUrl = i.FullyQualifiedPublicUrl
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return category;
        }
    }
}