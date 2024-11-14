using DigitalResourcesStore.Models.SupportDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;

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
        public async Task<IActionResult> GetUserContactRequests([FromQuery] int userId)
        {
            var contactRequests = await _supportService.GetUserContactRequests(userId);
            if (contactRequests == null || contactRequests.Count == 0)
            {
                return NotFound("Không tìm thấy yêu cầu liên hệ nào cho người dùng này.");
            }
            return Ok(contactRequests);
        }

        // POST: api/support/contact-requests
        [HttpPost("contact-requests")]
        public async Task<IActionResult> CreateContactRequest([FromBody] ContactRequestDtos contactRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu yêu cầu liên hệ không hợp lệ.");
            }

            var createdContactRequest = await _supportService.CreateContactRequest(contactRequestDto);
            return CreatedAtAction(nameof(GetUserContactRequests), new { userId = createdContactRequest.UserId }, createdContactRequest);
        }

        // GET: api/support/support-messages
        [HttpGet("support-messages")]
        public async Task<IActionResult> GetUserSupportMessages([FromQuery] int userId)
        {
            var supportMessages = await _supportService.GetUserSupportMessages(userId);
            if (supportMessages == null || supportMessages.Count == 0)
            {
                return NotFound("Không tìm thấy tin nhắn hỗ trợ nào cho người dùng này.");
            }
            return Ok(supportMessages);
        }

        // POST: api/support/support-messages
        [HttpPost("support-messages")]
        public async Task<IActionResult> CreateSupportMessage([FromBody] MessageSupportDtos messageSupportDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dữ liệu tin nhắn hỗ trợ không hợp lệ.");
            }

            var createdSupportMessage = await _supportService.CreateSupportMessage(messageSupportDto);
            return CreatedAtAction(nameof(GetUserSupportMessages), new { userId = createdSupportMessage.UserId }, createdSupportMessage);
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
