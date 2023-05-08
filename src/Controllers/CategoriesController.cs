using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA.Entities;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Repositories.Interfaces;

namespace SACA.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var baseQuery = _categoryRepository.GetCategoryQuery();
            var userId = User.GetId();
            
            if (!userId.HasValue || User.Identity is null || !User.Identity.IsAuthenticated)
            {
                return await baseQuery
                    .Where(x => x.Id != 1)
                    .ToListAsync();
            }
            
            var categories = await _categoryRepository.GetUserCategoriesAsync(baseQuery, userId.Value);

            return categories;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var userId = User.GetId();

            var category = await _categoryRepository.GetUserCategoryAsync(userId.Value, id);

            return category;
        }
    }
}