using DigitalResourcesStore.Models.CategoryDtos;
using DigitalResourcesStore.Models.ProductDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;

namespace DigitalResourcesStore.Controllers
{
    [Route("category")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDtos>> GetById(int id)
        {
            var category = await _categoryService.GetById(id);
            if (category == null) return NotFound();
            return Ok(category);
        }
        [HttpGet]
        public async Task<PagedResponse<CategoryDtos>> Get([FromQuery] QueryCategoryDto query)
        {
            return await _categoryService.Get(query);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] CreatedCategoryDtos request)
        {
            return await _categoryService.Create(request);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdateCategoryDtos request)
        {
            return await _categoryService.Update(id, request);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await _categoryService.Delete(id);
        }
    }
}
