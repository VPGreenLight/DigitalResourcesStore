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
        var captchaCode = _captchaService.GenerateCaptchaCode();
        var captchaImage = _captchaService.GenerateCaptchaImage(captchaCode);
        return File(captchaImage, "image/png");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!_captchaService.ValidateCaptcha(model.Captcha))
        {
            return BadRequest("Invalid captcha");
        }

        var result = await _authService.LoginAsync(model);
        if (result == null)
        {
            return Unauthorized("Invalid credentials");
        }

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var result = await _authService.RegisterAsync(model);
        if (result == null)
        {
            return BadRequest("Registration failed");
        }

        return Ok(result);
    }
}