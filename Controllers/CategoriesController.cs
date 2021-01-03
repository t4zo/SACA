using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Models;

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
        [HttpGet("helloWorld")]
        public string HelloWorld()
        {
            return "Hello, World :)";
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var categories = await _context.Categories
                    .AsNoTracking()
                    .Include(x => x.Images)
                    .Include(x => x.ApplicationUsers.Where(y => y.Id == userId))
                    .Select(c => new Category
                    {
                        Id = c.Id,
                        Name = c.Name,
                        IconName = c.IconName,
                        Images = c.Images.Select(i => new Image
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

            return await _context.Categories
                .AsNoTracking()
                .Include(x => x.Images)
                .Where(x => x.Id != 1)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var category = await _context.Categories
                .AsNoTracking()
                .Include(x => x.Images)
                .Include(x => x.ApplicationUsers.Where(y => y.Id == userId))
                .Where(x => x.Id == id)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    IconName = c.IconName,
                    Images = c.Images.Select(i => new Image
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