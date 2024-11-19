using DigitalResourcesStore.Models.ProductDetailDtos;
using DigitalResourcesStore.Models.ProductDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using DigitalResourcesStore.Models;

namespace DigitalResourcesStore.Controllers
{
    [Route("productdetails")]
    [ApiController]
    public class ProductDetailController : Controller
    {
        private readonly IProductDetailService _productdetailService;

        public ProductDetailController(IProductDetailService productdetailService)
        {
            _productdetailService = productdetailService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailDtos>> GetById(int id)
        {
            var productdetail = await _productdetailService.GetById(id);
            if (productdetail == null) return NotFound();
            return Ok(productdetail);
        }
        [HttpGet]
        public async Task<PagedResponse<ProductDetailDtos>> Get([FromQuery] QueryProductDetailDto query)
        {
            return await _productdetailService.Get(query);
        }
        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] CreateProductDetailDtos request)
        {
            return await _productdetailService.Create(request);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdateProductDetailDtos request)
        {
            return await _productdetailService.Update(id, request);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await _productdetailService.Delete(id);
        }
    }
}
