using DigitalResourcesStore.EntityFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Services
{
    public interface IDepositService
    {
        string GenerateDepositCode();
        DepositHistory GetDepositHistoryById(int id);
        void SaveDeposit(DepositHistory deposit);
        void UpdateDepositHistory(int id, bool isSuccess);
        List<DepositHistory> GetAllDeposits();
    }

    public class DepositService : IDepositService
    {
        private readonly DigitalResourcesStoreDbContext _context;

        public DepositService(DigitalResourcesStoreDbContext context)
        {
            _context = context;
        }

        public string GenerateDepositCode()
        {
            return GenerateCodeDeposit();
        }

        public DepositHistory GetDepositHistoryById(int id)
        {
            return _context.DepositHistories.FirstOrDefault(x => x.Id == id);
        }

        public void SaveDeposit(DepositHistory deposit)
        {
            _context.DepositHistories.Add(deposit);
            _context.SaveChanges();
        }

        public void UpdateDepositHistory(int id, bool isSuccess)
        {
            var deposit = GetDepositHistoryById(id);
            if (deposit != null)
            {
                deposit.IsSuccess = isSuccess;
                _context.DepositHistories.Update(deposit);
                _context.SaveChanges();
            }
        }

        public List<DepositHistory> GetAllDeposits()
        {
            return _context.DepositHistories.ToList();
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
