using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalResourcesStore.Models;
using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models.DepositDtos;
using DigitalResourcesStore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace DigitalResourcesStore.Services
{
    public interface IDepositService
    {
        Task<string> ProcessRechargeAsync(HttpContext httpContext, DepositDto deposit, int userId);
        Task<string> HandlePaymentCallbackAsync(IQueryCollection query, int userId);
        Task UpdateUserBalanceAsync(int userId, decimal amount);
    }

    public class DepositService : IDepositService
    {
        private readonly DigitalResourcesStoreDbContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly IAuthService _authService;

        public DepositService(DigitalResourcesStoreDbContext context, IVnPayService vnPayService, IAuthService authService)
        {
            _context = context;
            _vnPayService = vnPayService;
            _authService = authService;
        }

        public async Task<string> ProcessRechargeAsync(HttpContext httpContext, DepositDto deposit, int userId)
        {
            string depositCode = GenerateCodeDeposit();
            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = deposit.amount,
                CreatedDate = DateTime.Now,
                Description = deposit.description ?? "",
                Id = depositCode
            };

            var depositHistory = new DepositHistory
            {
                Money = deposit.amount / 1000,
                Description = deposit.description ?? "",
                CreatedAt = DateTime.Now,
                UserId = userId,
                IsSuccess = false
            };

            _context.DepositHistories.Add(depositHistory);
            await _context.SaveChangesAsync();

            return _vnPayService.CreatePaymentUrl(httpContext, vnPayModel);
        }

        public async Task<string> HandlePaymentCallbackAsync(IQueryCollection query, int userId)
        {
            // Xác thực chữ ký số từ VNPay
            bool isValidSignature = _vnPayService.ValidateSignature(query["vnp_SecureHash"], query);
            if (!isValidSignature)
            {
                return "VNPay digital signature validation failed.";
            }

            // Kiểm tra mã phản hồi giao dịch
            if (query["vnp_ResponseCode"] != "00")
            {
                return $"Payment failed with response code {query["vnp_ResponseCode"]}.";
            }

            // Lấy thông tin từ query
            var amount = decimal.Parse(query["vnp_Amount"]) / 100; // Convert từ VNPay (đơn vị VNĐ)

            // Cập nhật số dư cho user
            await UpdateUserBalanceAsync(userId, amount);

            // Lưu trạng thái giao dịch
            var depositHistory = await _context.DepositHistories
               .Where(d => d.UserId == userId) // Lọc theo UserId
               .OrderByDescending(d => d.CreatedAt) // Lấy bản ghi mới nhất
               .FirstOrDefaultAsync();

            if (depositHistory != null)
            {
                depositHistory.IsSuccess = true;
                _context.DepositHistories.Update(depositHistory);
                await _context.SaveChangesAsync();
            }

            return "Payment successful.";
        }


        public async Task UpdateUserBalanceAsync(int userId, decimal amount)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Money += amount / 1000;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("User not found.");
            }
        }

        public static string GenerateCodeDeposit()
        {
            return "DE" + GenerateUniqueId();
        }

        public static string GenerateUniqueId()
        {
            // You can customize this function according to your requirements
            // For simplicity, this example generates a random 8-character string
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] charArray = new char[8];
            Random random = new Random();

            for (int i = 0; i < 8; i++)
            {
                charArray[i] = chars[random.Next(chars.Length)];
            }

            string uniqueId = new string(charArray);
            return uniqueId;
        }
    }
}
