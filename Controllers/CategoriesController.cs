using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Models;
using SACA.Models.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return await _context.Categories
                    .Include(x => x.Images.Where(x => x.CategoryId != 1))
                    .AsNoTracking()
                    .ToListAsync();
            }

            var userId = int.Parse(_userManager.GetUserId(User));

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
                    if (image.UserId is null || image.UserId == userId)
                    {
                        image.User = null;
                        image.Category = null;
                        images.Add(image);
                    }
                }

                category.Images = images;
            }


            return categories;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var userId = int.Parse(_userManager.GetUserId(User));

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
                if (image.UserId is null || image.UserId == userId)
                {
                    image.User = null;
                    image.Category = null;
                    images.Add(image);
                }
            }

            category.Images = images;

            return category;
        }
    }
}
