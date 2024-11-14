using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models.DepositDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/deposits")]  // Change the route to be more API-centric
public class DepositsController : ControllerBase
{
    private readonly IVnPayService _vnPayService;
    private readonly IDepositService _depositService;
    private readonly IUserService _userService;

    public DepositsController(IVnPayService vnPayService, IDepositService depositService, IUserService userService)
    {
        _vnPayService = vnPayService;
        _depositService = depositService;
        _userService = userService;
    }

    [HttpPost("recharge")]  // Post endpoint for recharge
    public async Task<IActionResult> VnPay([FromBody] DepositDto deposit)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid data.");
        }

        int? userId = HttpContext.Session.GetInt32("User");
        if (!userId.HasValue)
        {
            return Unauthorized(new { Message = "User session not found." });
        }

        string id = _depositService.GenerateDepositCode();
        var vnPayModel = new VnPaymentRequestModel
        {
            Amount = deposit.amount,
            CreatedDate = DateTime.Now,
            Description = deposit.description ?? "",
            Id = id,
        };

        var depositHistory = new DepositHistory
        {
            Money = deposit.amount / 1000,
            Description = deposit.description ?? "",
            CreatedAt = DateTime.Now,
            UserId = userId.Value,  // Use userId.Value since userId is nullable
            IsSuccess = false
        };

        _depositService.SaveDeposit(depositHistory);

        var dhId = depositHistory.Id;
        HttpContext.Session.SetInt32("DHID", dhId);

        var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
        return Ok(new { PaymentUrl = paymentUrl });
    }

    [HttpGet("callback")]  // Callback endpoint to handle VNPay response
    public async Task<IActionResult> PaymentCallBack()
    {
        // Lấy các tham số từ query string của URL callback
        var response = _vnPayService.PaymentExecute(Request.Query);

        int? dhId = HttpContext.Session.GetInt32("DHID");
        if (!dhId.HasValue)
        {
            return BadRequest(new { Message = "Deposit history ID not found in session." });
        }

        var depositHistory = _depositService.GetDepositHistoryById(dhId.Value);

        // Kiểm tra nếu kết quả trả về từ VNPay hợp lệ và chữ ký xác thực
        if (response != null && response.VnPayReponseCode == "00" && _vnPayService.ValidateSignature(Request.Query["vnp_SecureHash"], Request.Query))
        {
            // Lấy thông tin người dùng từ session
            int? userId = HttpContext.Session.GetInt32("User");
            if (!userId.HasValue)
            {
                return Unauthorized(new { Message = "User session not found." });
            }

            var user = await _userService.GetById(userId.Value);
            if (user != null)
            {
                // Cập nhật số dư người dùng sau khi thanh toán thành công
                await _userService.UpdateUserBalance(userId.Value, response.Amount);

                // Cập nhật trạng thái lịch sử nạp tiền thành công
                depositHistory.IsSuccess = true;
                _depositService.UpdateDepositHistory(dhId.Value, true);

                // Cập nhật số tiền người dùng trong session
                HttpContext.Session.SetString("UserMoney", user.Money?.ToString("0.000") ?? "0.000");

                // Trả về kết quả thành công
                return Ok(new { Message = "Thanh toán VNPay thành công" });
            }
        }

        // Nếu không hợp lệ, trả về thông báo lỗi
        return BadRequest(new { Message = $"Lỗi thanh toán VNPay: {response?.VnPayReponseCode}" });
    }


}
