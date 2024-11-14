using DigitalResourcesStore.Models.CartsDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DigitalResourcesStore.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(IUserService userService, ICartService cartService, IOrderService orderService)
        {
            _userService = userService;
            _cartService = cartService;
            _orderService = orderService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("User");
            if (userId == null)
            {
                return Unauthorized("User not logged in.");
            }

            await _cartService.AddToCart(HttpContext, productId, quantity);

            // Get updated cart details
            var cartItems = _cartService.GetCartItems(HttpContext);
            var totalQuantity = cartItems.Sum(item => item.Quantity);
            var totalPrice = _cartService.GetCartTotal(HttpContext);

                   return Ok(new
            {
                Message = "Product added to cart successfully.",
                CartSummary = new
                {
                    TotalItems = totalQuantity,
                    TotalPrice = totalPrice,
                    Items = cartItems
                }
            });
        }


        [HttpPost("process-order")]
        public async Task<IActionResult> ProcessOrder()
        {
            var userId = HttpContext.Session.GetInt32("User");
            var cart = _cartService.GetCartItems(HttpContext);
            var total = _cartService.GetCartTotal(HttpContext);

            if (userId != null && cart != null)
            {
                await _orderService.ProcessOrderAsync(userId.Value, cart, total);
                return RedirectToAction("PaymentSuccess");
            }

            return RedirectToAction("PaymentFail");
        }
    }
}
