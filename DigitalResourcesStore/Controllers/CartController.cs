using DigitalResourcesStore.Models.CartsDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using DigitalResourcesStore.Helper;


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

        [HttpPost("buy")]
        public async Task<IActionResult> Buy(int productId, int quantity = 1)
        {
            var result = await _cartService.Buy(HttpContext, productId, quantity);
            return result;
        }
        [HttpGet("cart-details")]
        public async Task<IActionResult> GetCartDetails()
        {
            var userId = HttpContext.Session.GetInt32("User");
            if (userId == null)
            {
                return Unauthorized(new { message = "User not logged in." });
            }

            var cart = await _cartService.GetCartDetails(userId.Value);

            if (cart == null || !cart.Any())
            {
                return NotFound(new { message = "No items found in the cart." });
            }

            var total = cart.Sum(item => item.Price * item.Quantity);
            var uniqueProductCount = cart.Select(item => item.ProductId).Distinct().Count();

            return Ok(new
            {
                CartItems = cart,
                TotalItems = uniqueProductCount,
                TotalPrice = total
            });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove(int productId)
        {
            var result = await _cartService.RemoveFromCart(HttpContext, productId);
            return result;
        }

        [HttpPost("increase-quantity")]
        public async Task<IActionResult> IncreaseQuantity(int productId)
        {
            var result = await _cartService.IncreaseQuantity(HttpContext, productId);
            return result;
        }

        [HttpPost("decrease-quantity")]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            var result = await _cartService.DecreaseQuantity(HttpContext, productId);
            return result;
        }

        [HttpPost("apply-voucher")]
        public async Task<IActionResult> ApplyVoucher(string code)
        {
            var result = await _cartService.ApplyVoucher(HttpContext, code);
            return result;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut()
        {
            var result = await _cartService.CheckOutAsync(HttpContext);
            return result;
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
