using DigitalResourcesStore.Models.CartsDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;


namespace DigitalResourcesStore.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;

        public CartController(IUserService userService, ICartService cartService, IAuthService authService)
        {
            _userService = userService;
            _cartService = cartService;
            _authService = authService;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> Buy([FromBody] AddToCartDto model)
        {
            var result = await _cartService.Buy(HttpContext, model.ProductId, model.Quantity);
            return Ok(result);
        }

        [HttpGet("cart-details")]
        public async Task<IActionResult> GetCartDetails()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var token = authHeader?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Authorization token is missing." });
            }

            var userId = _authService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid or expired token." });
            }

            var cartDetails = await _cartService.GetCartDetails(int.Parse(userId));
            if (cartDetails == null || !cartDetails.Items.Any())
            {
                return NotFound(new { message = "No items found in the cart." });
            }

            return Ok(cartDetails);
        }

        [HttpPost("remove/{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var result = await _cartService.RemoveFromCart(HttpContext, productId);
            return Ok(result);
        }

        [HttpPost("increase-quantity/{productId}")]
        public async Task<IActionResult> IncreaseQuantity(int productId)
        {
            var result = await _cartService.IncreaseQuantity(HttpContext, productId);
            return Ok(result);
        }

        [HttpPost("decrease-quantity/{productId}")]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            var result = await _cartService.DecreaseQuantity(HttpContext, productId);
            return Ok(result);
        }

        [HttpPost("apply-voucher")]
        public async Task<IActionResult> ApplyVoucher(string code)
        {
            var result = await _cartService.ApplyVoucher(HttpContext, code);
            return Ok(result);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut()
        {
            var result = await _cartService.CheckOutAsync(HttpContext);
            return Ok(result);
        }

        [HttpPost("cart-payment")]
        public async Task<IActionResult> CreateCartPayment()
        {
            var result = await _cartService.ProcessCartPaymentAsync(HttpContext);
            return Ok(result);
        }

        [HttpGet("cart-payment-callback")]
        public async Task<IActionResult> CartPaymentCallback()
        {
            try
            {
                // Lấy userId từ token
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                var token = authHeader?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "Authorization token is missing." });

                var userId = _authService.GetUserIdFromToken(token);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "Invalid or expired token." });

                // Lấy thông tin query từ callback
                var query = Request.Query;

                // Gọi service xử lý callback
                var result = await _cartService.HandleCartPaymentCallbackAsync(query, int.Parse(userId), HttpContext);

                if (result.Contains("successful"))
                {
                    return Ok(new { Message = "Payment successful." });
                }
                else
                {
                    return BadRequest(new { Message = result });
                }
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Error in PaymentCallBack: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing the payment callback." });
            }
        }

    }
}