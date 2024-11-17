using DigitalResourcesStore.Models.AuthDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly CaptchaService _captchaService;

    public AuthController(IAuthService authService, CaptchaService captchaService)
    {
        _authService = authService;
        _captchaService = captchaService;
    }

    [HttpGet("captcha/refresh")]
    public IActionResult GetCaptcha()
    {
        var captchaCode = _captchaService.GenerateCaptchaCode(6);
        var captchaImage = _captchaService.GenerateCaptchaImage(captchaCode);

        // Trả về mã captcha dưới dạng JSON
        return Json(new { code = captchaCode, image = Convert.ToBase64String(captchaImage) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Captcha))
        {
            return BadRequest(new { error = "Bạn đăng nhập lỗi hoặc thiếu!" });
        }

        var result = await _authService.LoginAsync(model);
        if (result == null)
        {
            return Unauthorized("Đăng nhập lỗi. Xin hãy kiểm tra lại!");
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        var result = await _authService.RegisterAsync(model);
        if (result == null)
        {
            return BadRequest("Registration failed");
        }

        return Ok(result);
    }
}