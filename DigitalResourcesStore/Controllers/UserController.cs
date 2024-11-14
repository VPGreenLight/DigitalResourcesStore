using DigitalResourcesStore.Models.UserDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;

namespace DigitalResourcesStore.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
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
    }
}
