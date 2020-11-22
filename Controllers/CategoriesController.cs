using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Models;
using SACA.Models.Identity;

namespace SACA.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
                return await _context.Categories
                    .Include(
                        x => x.Images.Where(x => x.CategoryId.ToString() != "EE0230EE-BFE5-4B4C-86F6-A5C54D0E2BE7"))
                    .AsNoTracking()
                    .ToListAsync();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var categories = await _context.Categories
                .Include(x => x.Images)
                .Include(x => x.ApplicationUsers.Where(au => au.Id == userId))
                .AsNoTracking()
                .ToListAsync();

            foreach (var category in categories)
            {
                category.ApplicationUsers = null;

                var images = new List<Image>();

                foreach (var image in category.Images)
                {
                    if (string.IsNullOrEmpty(image.UserId.ToString()) && image.UserId != userId) continue;

                    image.User = null;
                    image.Category = null;
                    images.Add(image);
                }

                category.Images = images;
            }


            return categories;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var category = await _context.Categories
                .Include(x => x.Images)
                .Include(x => x.ApplicationUsers.Where(au => au.Id == userId))
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            category.ApplicationUsers = null;

            var images = new List<Image>();

            foreach (var image in category.Images)
            {
                if (string.IsNullOrEmpty(image.UserId.ToString()) && image.UserId != userId) continue;

                image.User = null;
                image.Category = null;
                images.Add(image);
            }

            category.Images = images;

            return category;
        }
    }
}