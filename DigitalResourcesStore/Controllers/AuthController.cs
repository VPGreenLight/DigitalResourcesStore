using DigitalResourcesStore.Models.AuthDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalResourcesStore.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await _authService.LoginAsync(model);
            if (result == null)
                return Unauthorized("Invalid credentials");

            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var result = await _authService.RegisterAsync(model);
            if (result == null)
                return BadRequest("Registration failed");

            return Ok(result);
        }
    }
}
