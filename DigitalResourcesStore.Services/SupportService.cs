using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalResourcesStore.Models.SupportDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models;
using static System.Net.Mime.MediaTypeNames;

namespace DigitalResourcesStore.Services
{
    public interface ISupportService
    {
        Task<List<ContactRequestDtos>> GetAllContactRequests();
        Task<List<MessageSupportDtos>> GetAllSupportMessages();
        Task<ContactRequestDtos> CreateContactRequest(ContactRequestDtos contactRequestDto, int userId);
        Task<MessageSupportDtos> CreateSupportMessage(MessageSupportDtos messageSupportDto, int userId);
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
        public async Task<List<ContactRequestDtos>> GetAllContactRequests()
        {
            return await _db.ContactRequests
                .Select(cr => new ContactRequestDtos
                {
                    Id = cr.Id,
                    FullName = cr.FullName,
                    DateOfBirth = cr.DateOfBirth,
                    Cccd = cr.Cccd,
                    FacebookLink = cr.FacebookLink,
                    PhoneNumber = cr.PhoneNumber,
                    Email = cr.Email,
                    Content = cr.Content,
                    IsAccepted = cr.IsAccepted,
                    AdminReply = cr.AdminReply != null,
                }).ToListAsync();
        }

        public async Task<ContactRequestDtos> CreateContactRequest(ContactRequestDtos contactRequestDto, int userId)
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
                UserId = userId,
                IsAccepted = false,
                //AdminReply = false,
            };

            _db.ContactRequests.Add(contactRequest);
            await _db.SaveChangesAsync();

            //contactRequestDto.Id = contactRequest.Id;
            return contactRequestDto;
        }
        public async Task<List<MessageSupportDtos>> GetAllSupportMessages()
        {
            return await _db.MessageSupports
                .Select(ms => new MessageSupportDtos
                {
                    Id = ms.Id,
                    Email = ms.Email,
                    PhoneNumber = ms.PhoneNumber,
                    Subject = ms.Subject,
                    Content = ms.Content,
                    IsProcessed = ms.IsProcessed,
                    AdminReply = ms.AdminReply != null,
                    //UserName = ms.UserName,
                    //UserId = ms.UserId
                }).ToListAsync();
        }

        public async Task<MessageSupportDtos> CreateSupportMessage(MessageSupportDtos messageSupportDto, int userId)
        {
            var messageSupport = new MessageSupport
            {
                Email = messageSupportDto.Email,
                PhoneNumber = messageSupportDto.PhoneNumber,
                Subject = messageSupportDto.Subject,
                Content = messageSupportDto.Content,
                IsProcessed = false,
                UserId = userId,
                //UserName= messageSupportDto.UserName,
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
                    messageSupport.AdminReplyId = adminReply.Id;
                    messageSupport.IsProcessed = true;
                    // Cập nhật AdminReplyId trong bảng MessageSupport
                    _db.Update(messageSupport);
                    await _db.SaveChangesAsync(); // Lưu thay đổi vào MessageSupport
                }
            }

            // Trả về DTO phản hồi
            return adminReplyDto;
        }

    }
}
