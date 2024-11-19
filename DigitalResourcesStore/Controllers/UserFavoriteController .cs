using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalResourcesStore.Controllers
{
    [Route("favorites")]
    [ApiController]
    public class UserFavoriteController : Controller
    {
        private readonly IUserFavoriteService _userFavoriteService;

        public UserFavoriteController(IUserFavoriteService userFavoriteService)
        {
            _userFavoriteService = userFavoriteService;
        }

        [HttpPost("add/{productId}")]
        public async Task<IActionResult> AddToFavorite(int productId)
        {
            var success = await _userFavoriteService.AddToFavorite(HttpContext, productId);
            return Json(new { success, message = success ? "sản phẩm đã được thêm vào yêu thích." : "thêm sản phẩm thất bại." });
        }

        [HttpPost("remove/{productId}")]
        public async Task<IActionResult> RemoveFromFavorite(int productId)
        {
            var success = await _userFavoriteService.RemoveFromFavorite(HttpContext, productId);
            return Json(new { success, message = success ? "Xoá sản phẩm thành công." : "Không tìm thấy sản phẩm." });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetFavoriteProducts()
        {
            try
            {
                var favoriteProducts = await _userFavoriteService.GetFavoritesByUserId(HttpContext);
                if (favoriteProducts == null || !favoriteProducts.Any())
                {
                    return NotFound(new { message = "Danh sách yêu thích trống." });
                }
                return Ok(favoriteProducts);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}