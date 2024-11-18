using DigitalResourcesStore.Models.SupportDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using DigitalResourcesStore.Models;

namespace DigitalResourcesStore.Controllers
{
    [Route("support")]
    [ApiController]
    public class SupportController : Controller
    {
        private readonly ISupportService _supportService;

        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }
        [HttpGet("contact-requests")]
        public async Task<IActionResult> GetAllContactRequests()
        {
            var contactRequests = await _supportService.GetAllContactRequests();
            if (contactRequests == null || contactRequests.Count == 0)
            {
                return NotFound("Không tìm thấy yêu cầu liên hệ nào.");
            }
            return Ok(contactRequests);
        }

        // POST: api/support/contact-requests
        [HttpPost("contact-requests")]
        public async Task<IActionResult> CreateContactRequest([FromBody] ContactRequestDtos contactRequestDto)
        {
            var userId = HttpContext.Session.GetInt32("User");
            if (userId == null)
            {
                return Unauthorized("User không đăng nhập.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu yêu cầu liên hệ không hợp lệ.");
            }

            var createdContactRequest = await _supportService.CreateContactRequest(contactRequestDto, userId.Value);
            return CreatedAtAction(nameof(GetAllContactRequests), new { userId = userId.Value }, new
            {
                Message = "Tạo yêu cầu liên hệ thành công.",
                Data = createdContactRequest
            });
        }

        // GET: api/support/support-messages
        [HttpGet("support-messages")]
        public async Task<IActionResult> GetAllSupportMessages()
        {
            var supportMessages = await _supportService.GetAllSupportMessages();
            if (supportMessages == null || supportMessages.Count == 0)
            {
                return NotFound("Không tìm thấy tin nhắn hỗ trợ nào.");
            }
            return Ok(supportMessages);
        }

        // POST: api/support/support-messages
        [HttpPost("support-messages")]
        public async Task<IActionResult> CreateSupportMessage([FromBody] MessageSupportDtos messageSupportDto)
        {
            var userId = HttpContext.Session.GetInt32("User");
            if (userId == null)
            {
                return Unauthorized("User không đăng nhập.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu tin nhắn hỗ trợ không hợp lệ.");
            }

            var createdSupportMessage = await _supportService.CreateSupportMessage(messageSupportDto, userId.Value);
            return CreatedAtAction(nameof(GetAllSupportMessages), new { userId = userId.Value }, new
            {
                Message = "Tin nhắn hỗ trợ đã được tạo thành công.",
                Data = createdSupportMessage
            });
        }
        [HttpPost("admin-replies")]
        public async Task<IActionResult> CreateAdminReply([FromBody] AdminReplyDtos adminReplyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu phản hồi không hợp lệ.");
            }

            var createdAdminReply = await _supportService.CreateAdminReply(adminReplyDto);

            // Trả về phản hồi thành công mà không cần ID
            return Ok(createdAdminReply); // Hoặc return Ok(adminReplyDto) nếu chỉ muốn trả về DTO mà không cần sự thay đổi
        }

    }
}
