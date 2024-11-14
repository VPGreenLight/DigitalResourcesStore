using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models.UserFavoriteDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;

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
        [HttpPost("add")]
        public async Task<IActionResult> AddToFavorite(int productId)
        {
            //var userId = HttpContext.Session.GetInt32("User");
            //if (userId == null)
            //{
            //    return Json(new { success = false, message = "Bạn phải đăng nhập để tiếp tục." });
            //}

            //var success = await _userFavoriteService.AddToFavorite(userId.Value, productId);
            //return Json(new { success, message = success ? "Sản phẩm đã được thêm vào yêu thích." : "Thêm sản phẩm thất bại." });
            var success = await _userFavoriteService.AddToFavorite(1, productId);
            return Json(new { success, message = success ? "Sản phẩm đã được thêm vào yêu thích." : "Thêm sản phẩm thất bại." });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromFavorite(int productId)
        {
            //var userId = HttpContext.Session.GetInt32("User");
            //if (userId == null)
            //{
            //    return Json(new { success = false, message = "Bạn phải đăng nhập để tiếp tục." });
            //}

            var success = await _userFavoriteService.RemoveFromFavorite(1, productId);
            return Json(new { success, message = success ? "Xoá sản phẩm thành công." : "Không tìm thấy sản phẩm." });
        }

        [HttpGet("list")]
        public async Task<IActionResult> FavoriteProducts()
        {
            //var userId = HttpContext.Session.GetInt32("User");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //var favoriteProducts = await _userFavoriteService.GetFavoritesByUserId(userId.Value);
            //return Ok(favoriteProducts);
            var favoriteProducts = await _userFavoriteService.GetFavoritesByUserId(1);
            return Ok(favoriteProducts);
        }
    }
}
