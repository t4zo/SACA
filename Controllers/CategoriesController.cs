using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Controllers
{
    [Authorize(Permissions.Categories.View)]
    public class CategoriesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CategoriesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            IEnumerable<Category> categories;

            if (!User.Identity.IsAuthenticated)
            {
                categories = await _context.Categories
                    .Include(x => x.Images)
                    .Where(x => x.Images.Any(x => x.CategoryId != 1))
                    .ToListAsync();

                return Ok(categories);
            }

            var userId = int.Parse(_userManager.GetUserId(User));

            categories = await _context.Categories
               .Include(x => x.Images)
               .Where(x => x.UserCategories.Any(uc => uc.UserId == userId))
               .ToListAsync();

            foreach (var category in categories)
            {
                IList<Image> images = new List<Image>();

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


            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var userId = int.Parse(_userManager.GetUserId(User));

            var category = await _context.Categories
               .Include(x => x.Images)
               .Where(x => x.Id == id)
               .Where(x => x.UserCategories.Any(uc => uc.UserId == userId))
               .FirstOrDefaultAsync();

            IList<Image> images = new List<Image>();

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

            return Ok(category);
        }
    }
}
