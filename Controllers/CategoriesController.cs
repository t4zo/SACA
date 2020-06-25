using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA.Models;
using SACA.Repositories.Interfaces;
using SACA.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Controllers
{
    [Route("saca/v2/[controller]")]
    [Authorize(Constants.All)]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllHome()
        {
            return Ok(await _categoryRepository.GetAllAsync());
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll(int userId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categories = await _categoryRepository.GetAllAsync(userId);

            return Ok(categories);
        }

        [HttpGet("{userId}/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Category>>> Get(int userId, int categoryId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var categories = await _categoryRepository.GetAsync(userId, categoryId);

            return Ok(categories);
        }
    }
}
