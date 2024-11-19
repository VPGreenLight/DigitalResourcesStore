using DigitalResourcesStore.Models.UserDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using DigitalResourcesStore.Models;

namespace DigitalResourcesStore.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<PagedResponse<UserDto>> Get([FromQuery] QueryUserDto query)
        {
            return await _userService.Get(query);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create([FromBody] CreatedUserDto request)
        {
            return await _userService.Create(request);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdatedUserDto request)
        {
            return await _userService.Update(id, request);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await _userService.Delete(id);
        }

        [HttpPost("{id}/changePassword")]
        public async Task<ActionResult<bool>> ChangePassword(int id, [FromBody] ChangePasswordDto request)
        {
            return await _userService.ChangePassword(id, request);
        }

        [HttpGet("user")]
        public async Task<ActionResult<UserDto>> GetUserFromToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var token = Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            Console.WriteLine($"Token nhận được: {token}");

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            var userId = _authService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Console.WriteLine($"User ID từ token: {userId}");

            var user = await _userService.GetById(int.Parse(userId));
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("deposit-history")]
        public async Task<IActionResult> GetMyDepositHistory()
        {
            // Lấy token từ header Authorization
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var token = authHeader?.Replace("Bearer ", "");

            Console.WriteLine($"Token nhận được: {token}");

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            // Giải mã token để lấy userId
            var userId = _authService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Console.WriteLine($"User ID từ token: {userId}");

            // Lấy lịch sử giao dịch dựa trên userId
            var depositHistories = await _userService.GetDepositHistoryByUserIdAsync(int.Parse(userId));
            if (depositHistories == null || !depositHistories.Any())
            {
                return NotFound(new { Message = "No deposit history found for this user." });
            }

            return Ok(depositHistories);
        }

        [HttpGet("all-deposit-history")]
        public async Task<IActionResult> GetAllDepositHistories()
        {
            // Lấy token từ header Authorization
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var token = authHeader?.Replace("Bearer ", "");

            Console.WriteLine($"Token nhận được: {token}");

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            // Giải mã token để lấy userId
            var userId = _authService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Kiểm tra quyền hạn (ví dụ: admin)
            var user = await _userService.GetById(int.Parse(userId));
            if (user == null || user.RoleId != 1) // Replace "Role" if different property is used
            {
                return Forbid("Only admins can view all deposit histories.");
            }

            // Lấy tất cả lịch sử giao dịch
            var depositHistories = await _userService.GetAllDepositHistoriesAsync();
            if (depositHistories == null || !depositHistories.Any())
            {
                return NotFound(new { Message = "No deposit histories found." });
            }

            return Ok(depositHistories);
        }

        [HttpGet("order-history")]
        public async Task<IActionResult> GetMyOrderHistory()
        {
            // Lấy token từ header Authorization
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var token = authHeader?.Replace("Bearer ", "");

            Console.WriteLine($"Token nhận được: {token}");

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            // Giải mã token để lấy userId
            var userId = _authService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Console.WriteLine($"User ID từ token: {userId}");

            // Lấy lịch sử giao dịch dựa trên userId
            var depositHistories = await _userService.GetTransactionHistoryByUserIdAsync(int.Parse(userId));
            if (depositHistories == null || !depositHistories.Any())
            {
                return NotFound(new { Message = "No deposit history found for this user." });
            }

            return Ok(depositHistories);
        }

        [HttpGet("all-order-history")]
        public async Task<IActionResult> GetAllOrderHistories()
        {
            // Lấy token từ header Authorization
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            var token = authHeader?.Replace("Bearer ", "");

            Console.WriteLine($"Token nhận được: {token}");

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            // Giải mã token để lấy userId
            var userId = _authService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Kiểm tra quyền hạn (ví dụ: admin)
            var user = await _userService.GetById(int.Parse(userId));
            if (user == null || user.RoleId != 1) // Replace "Role" if different property is used
            {
                return Forbid("Only admins can view all deposit histories.");
            }

            // Lấy tất cả lịch sử giao dịch
            var depositHistories = await _userService.GetAllTransactionHistoriesAsync();
            if (depositHistories == null || !depositHistories.Any())
            {
                return NotFound(new { Message = "No deposit histories found." });
            }

            return Ok(depositHistories);
        }
    }
}
