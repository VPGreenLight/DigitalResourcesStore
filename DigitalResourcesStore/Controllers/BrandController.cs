using DigitalResourcesStore.Models.BrandDtos;
using DigitalResourcesStore.Models.ProductDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;

namespace DigitalResourcesStore.Controllers
{
    [Route("brand")]
    [ApiController]
    public class BrandController : Controller
    {

        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDtos>> GetById(int id)
        {
            var brand = await _brandService.GetById(id);
            if (brand == null) return NotFound();
            return Ok(brand);
        }

        [HttpGet]
        public async Task<PagedResponse<BrandDtos>> Get([FromQuery] QueryBrandDto query)
        {
            return await _brandService.Get(query);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] CreatedBrandDtos request)
        {
            var result = await _brandService.Create(request);
            if (!result) return BadRequest("Failed to create brand.");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdateBrandDtos request)
        {
            var result = await _brandService.Update(id, request);
            if (!result) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _brandService.Delete(id);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
