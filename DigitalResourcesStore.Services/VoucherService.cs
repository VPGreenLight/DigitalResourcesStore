using DigitalResourcesStore.Models.VoucherDtos;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;


namespace DigitalResourcesStore.Services
{
    public interface IVoucherService
    {
        Task<List<VoucherDtos>> GetAllVouchers();
        Task<VoucherDtos?> GetVoucherById(int id);
        Task<bool> CreateVoucher(CreatedVoucherDtos dto);
        Task<bool> UpdateVoucher(int id, UpdateVoucherDtos dto);
        Task<bool> DeleteVoucher(int id, string deletedBy);
    }
    public class VoucherService : IVoucherService
    {
        private readonly DigitalResourcesStoreDbContext _context;

        public VoucherService(DigitalResourcesStoreDbContext context)
        {
            _context = context;
        }
        public async Task<List<VoucherDtos>> GetAllVouchers()
        {
            return await _context.Vouchers
                .Where(v => v.IsDelete == false)
                .Select(v => new VoucherDtos
                {
                    Id = v.Id,
                    Name = v.Name,
                    Discount = v.Discount,
                    Type = v.Type,
                    RemainUsed = v.RemainUsed,
                    IsActive = v.IsActive,
                    CreatedAt = v.CreatedAt,
                    CreatedBy = v.CreatedBy,
                    UpdatedAt = v.UpdatedAt,
                    UpdatedBy = v.UpdatedBy,
                    DeletedAt = v.DeletedAt,
                    IsDelete = v.IsDelete,
                    DeletedBy = v.DeletedBy
                }).ToListAsync();
        }
        public async Task<VoucherDtos?> GetVoucherById(int id)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Id == id && v.IsDelete==false);
            if (voucher == null) return null;

            return new VoucherDtos
            {
                Id = voucher.Id,
                Name = voucher.Name,
                Discount = voucher.Discount,
                Type = voucher.Type,
                RemainUsed = voucher.RemainUsed,
                IsActive = voucher.IsActive,
                CreatedAt = voucher.CreatedAt,
                CreatedBy = voucher.CreatedBy,
                UpdatedAt = voucher.UpdatedAt,
                UpdatedBy = voucher.UpdatedBy,
                DeletedAt = voucher.DeletedAt,
                IsDelete = voucher.IsDelete,
                DeletedBy = voucher.DeletedBy
            };
        }

        public async Task<bool> CreateVoucher(CreatedVoucherDtos dto)
        {
            var voucher = new Voucher
            {
                Name = dto.Name,
                Discount = dto.Discount,
                Type = dto.Type,
                RemainUsed = dto.RemainUsed,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now,
                CreatedBy = "admin",
                IsDelete = false
            };

            await _context.Vouchers.AddAsync(voucher);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateVoucher(int id, UpdateVoucherDtos dto)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Id == id && v.IsDelete==false);
            if (voucher == null) return false;

            voucher.Name = dto.Name;
            voucher.Discount = dto.Discount;
            voucher.Type = dto.Type;
            voucher.RemainUsed = dto.RemainUsed;
            voucher.IsActive = dto.IsActive;
            voucher.UpdatedAt = DateTime.Now;
            voucher.UpdatedBy = "admin";

            _context.Vouchers.Update(voucher);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVoucher(int id, string deletedBy)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Id == id && v.IsDelete == false);
            if (voucher == null) return false;

            voucher.IsDelete = true;
            voucher.DeletedAt = DateTime.Now;
            voucher.DeletedBy = "admin";

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
