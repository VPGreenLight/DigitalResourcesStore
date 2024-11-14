using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalResourcesStore.Models.SupportDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using QuizApp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace DigitalResourcesStore.Services
{
    public interface ISupportService
    {
        Task<List<ContactRequestDtos>> GetUserContactRequests(int userId);
        Task<ContactRequestDtos> CreateContactRequest(ContactRequestDtos contactRequestDto);
        Task<List<MessageSupportDtos>> GetUserSupportMessages(int userId);
        Task<MessageSupportDtos> CreateSupportMessage(MessageSupportDtos messageSupportDto);
        Task<AdminReplyDtos> CreateAdminReply(AdminReplyDtos adminReplyDto);
    }

    public class SupportService : ISupportService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;

        public SupportService(IConfiguration config, DigitalResourcesStoreDbContext db)
        {
            _config = config;
            _db = db;
        }
        public async Task<List<ContactRequestDtos>> GetUserContactRequests(int userId)
        {
            return await _db.ContactRequests
                .Where(cr => cr.UserId == userId)
                .Select(cr => new ContactRequestDtos
                {
                    FullName = cr.FullName,
                    DateOfBirth = cr.DateOfBirth,
                    Cccd = cr.Cccd,
                    FacebookLink = cr.FacebookLink,
                    PhoneNumber = cr.PhoneNumber,
                    Email = cr.Email,
                    Content = cr.Content,
                    UserId = cr.UserId  
                }).ToListAsync();
        }

        public async Task<ContactRequestDtos> CreateContactRequest(ContactRequestDtos contactRequestDto)
        {
            var contactRequest = new ContactRequest
            {
                FullName = contactRequestDto.FullName,
                DateOfBirth = contactRequestDto.DateOfBirth,
                Cccd = contactRequestDto.Cccd,
                FacebookLink = contactRequestDto.FacebookLink,
                PhoneNumber = contactRequestDto.PhoneNumber,
                Email = contactRequestDto.Email,
                Content = contactRequestDto.Content,
                UserId = contactRequestDto.UserId
            };

            _db.ContactRequests.Add(contactRequest);
            await _db.SaveChangesAsync();

            //contactRequestDto.Id = contactRequest.Id;
            return contactRequestDto;
        }
        public async Task<List<MessageSupportDtos>> GetUserSupportMessages(int userId)
        {
            return await _db.MessageSupports
                .Where(ms => ms.UserId == userId)
                .Select(ms => new MessageSupportDtos
                {
                    Email = ms.Email,
                    PhoneNumber = ms.PhoneNumber,
                    Subject = ms.Subject,
                    Content = ms.Content,
                    IsProcessed = false,
                    UserId = ms.UserId
                }).ToListAsync();
        }

        public async Task<MessageSupportDtos> CreateSupportMessage(MessageSupportDtos messageSupportDto)
        {
            var messageSupport = new MessageSupport
            {
                Email = messageSupportDto.Email,
                PhoneNumber = messageSupportDto.PhoneNumber,
                Subject = messageSupportDto.Subject,
                Content = messageSupportDto.Content,
                IsProcessed = messageSupportDto.IsProcessed,
                UserId = messageSupportDto.UserId
            };

            _db.MessageSupports.Add(messageSupport);
            await _db.SaveChangesAsync();

            //messageSupportDto.Id = messageSupport.Id;
            return messageSupportDto;
        }
        public async Task<AdminReplyDtos> CreateAdminReply(AdminReplyDtos adminReplyDto)
        {
            // Tạo phản hồi mới
            var adminReply = new AdminReply
            {
                ReplyContent = adminReplyDto.ReplyContent,
                ReplyDate = DateTime.Now
            };

            _db.AdminReplies.Add(adminReply); // Thêm vào bảng AdminReplies
            await _db.SaveChangesAsync(); // Lưu vào cơ sở dữ liệu

            // Cập nhật ContactRequest nếu có
            if (adminReplyDto.ContactRequestId.HasValue)
            {
                var contactRequest = await _db.ContactRequests
                    .FirstOrDefaultAsync(cr => cr.Id == adminReplyDto.ContactRequestId.Value);

                if (contactRequest != null)
                {
                    contactRequest.AdminReplyId = adminReply.Id; // Cập nhật AdminReplyId trong bảng ContactRequest
                    _db.Update(contactRequest);
                    await _db.SaveChangesAsync(); // Lưu thay đổi vào ContactRequest
                }
            }

            // Cập nhật MessageSupport nếu có
            if (adminReplyDto.MessageSupportId.HasValue)
            {
                var messageSupport = await _db.MessageSupports
                    .FirstOrDefaultAsync(ms => ms.Id == adminReplyDto.MessageSupportId.Value);

                if (messageSupport != null)
                {
                    messageSupport.AdminReplyId = adminReply.Id; // Cập nhật AdminReplyId trong bảng MessageSupport
                    _db.Update(messageSupport);
                    await _db.SaveChangesAsync(); // Lưu thay đổi vào MessageSupport
                }
            }

            // Trả về DTO phản hồi
            return adminReplyDto;
        }

    }
}
