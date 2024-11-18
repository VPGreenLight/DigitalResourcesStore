using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DigitalResourcesStore.Models;
using DigitalResourcesStore.Services;
using DigitalResourcesStore.Models.DepositDtos;

namespace DigitalResourcesStore.Controllers
{
    [ApiController]
    [Route("api/deposits")]
    public class DepositsController : ControllerBase
    {
        private readonly IDepositService _depositService;
        private readonly IAuthService _authService;
        public DepositsController(IDepositService depositService, IAuthService authService)
        {
            _depositService = depositService;
            _authService = authService;
        }

        [HttpPost("recharge")]
        public async Task<IActionResult> Recharge([FromBody] DepositDto deposit)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid input data." });

            // Lấy userId từ token (thay thế hardcode)
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

            // Gọi service xử lý logic recharge
            string paymentUrl = await _depositService.ProcessRechargeAsync(HttpContext, deposit, int.Parse(userId));

            if (string.IsNullOrEmpty(paymentUrl))
                return BadRequest(new { Message = "Failed to create payment URL." });

            return Ok(new { PaymentUrl = paymentUrl });
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallBack()
        {
            try
            {
                // Log bước bắt đầu
                Console.WriteLine("Starting PaymentCallBack");

                // Lấy token từ Authorization header
                var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
                var token = authHeader?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Token is missing");
                    return Unauthorized(new { message = "Authorization token is missing." });
                }

                // Giải mã token để lấy userId
                var userId = _authService.GetUserIdFromToken(token);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Invalid token");
                    return Unauthorized(new { message = "Invalid or expired token." });
                }

                // Lấy thông tin từ query
                var query = Request.Query;
                Console.WriteLine($"Query received: {query}");

                // Gọi service xử lý callback
                var result = await _depositService.HandlePaymentCallbackAsync(query, int.Parse(userId));

                Console.WriteLine($"Service result: {result}");
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




        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetDepositHistory(int id)
        {
            var depositHistory = await _depositService.GetDepositHistoryAsync(id);
            if (depositHistory == null)
                return NotFound(new { Message = "Deposit history not found." });

            return Ok(depositHistory);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetAllDepositHistories()
        {
            var depositHistories = await _depositService.GetAllDepositHistoriesAsync();
            return Ok(depositHistories);
        }

        [HttpPost("update-history/{id}")]
        public async Task<IActionResult> UpdateDepositHistory(int id, [FromQuery] bool isSuccess)
        {
            try
            {
                await _depositService.UpdateDepositHistoryAsync(id, isSuccess);
                return Ok(new { Message = "Deposit history updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
