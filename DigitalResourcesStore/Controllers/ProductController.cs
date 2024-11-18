using DigitalResourcesStore.Models.ProductDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using DigitalResourcesStore.Models;

namespace DigitalResourcesStore.Controllers
{
    [Route("products")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDtos>> GetById(int id)
        {
            var user = await _productService.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<PagedResponse<ProductDtos>> Get([FromQuery] QueryProductDto query)
        {
            return await _productService.Get(query);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] CreatedProductDtos request)
        {
            return await _productService.Create(request);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdateProductDtos request)
        {
            return await _productService.Update(id, request);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await _productService.Delete(id);
        }
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId, [FromQuery] QueryCategory query)
        {
            var products = await _productService.GetProductsByCategory(categoryId, query);
            return Ok(products);
        }

        [HttpGet("by-price-range")]
        public async Task<IActionResult> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
                var products = await _productService.GetProductsByPriceRange(minPrice, maxPrice);
                return Ok(products);
        }
    }
}
