using DigitalResourcesStore.Models.UserDtos;
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
    public interface IUserService
    {
        Task<UserDto> GetById(int id);
        Task<PagedResponse<UserDto>> Get(QueryUserDto query);
        Task<bool> Create(CreatedUserDto request);
        Task<bool> Update(int id, UpdatedUserDto request);
        Task<bool> Delete(int id);
        Task<bool> UpdateUserBalance(int userId, decimal amount);
        Task<bool> ChangePassword(int userId, ChangePasswordDto model);
    }

    public class UserService : IUserService
    {
        private readonly DigitalResourcesStoreDbContext _context;

        public UserService(DigitalResourcesStoreDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.Phone,
                Address = user.Address,
                IsActive = user.IsActive,
                RoleId = user.RoleId, // Retrieve actual RoleId from the user entity
                Money = user.Money,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                UpdatedAt = user.UpdatedAt,
                UpdatedBy = user.UpdatedBy,
                DeletedAt = user.DeletedAt,
                IsDelete = user.IsDelete,
                DeletedBy = user.DeletedBy
            };
        }

        public async Task<PagedResponse<UserDto>> Get(QueryUserDto query)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                string lowerKeyword = query.Keyword.ToLower();
                usersQuery = usersQuery.Where(user =>
                    user.Name.ToLower().Contains(lowerKeyword) ||
                    user.Email.ToLower().Contains(lowerKeyword) ||
                    user.UserName.ToLower().Contains(lowerKeyword) ||
                    user.Phone.ToLower().Contains(lowerKeyword));
            }

            var totalCount = await usersQuery.CountAsync();

            var pagedUsers = await usersQuery
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var userDtos = pagedUsers.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.Phone,
                Address = user.Address,
                IsActive = user.IsActive,
                RoleId = user.RoleId, // Retrieve actual RoleId
                Money = user.Money,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                UpdatedAt = user.UpdatedAt,
                UpdatedBy = user.UpdatedBy,
                DeletedAt = user.DeletedAt,
                IsDelete = user.IsDelete,
                DeletedBy = user.DeletedBy
            }).ToList();

            return new PagedResponse<UserDto>(userDtos, query.PageIndex, query.PageSize, totalCount);
        }

        public async Task<bool> Create(CreatedUserDto viewModel)
        {
            var user = new User
            {
                Name = viewModel.Name,
                Email = viewModel.Email,
                UserName = viewModel.UserName,
                Password = GetMD5(viewModel.Password),
                Phone = viewModel.Phone,
                Address = viewModel.Address,
                IsActive = viewModel.IsActive,
                RoleId = 1,
                Money = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(int id, UpdatedUserDto viewModel)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Name = viewModel.Name;
            user.Email = viewModel.Email;
            user.UserName = viewModel.UserName;
            user.Phone = viewModel.Phone;
            user.Address = viewModel.Address;
            user.IsActive = viewModel.IsActive;
            user.RoleId = viewModel.RoleId;
            user.Money = viewModel.Money;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = "User";

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsDelete = true;
            user.DeletedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateUserBalance(int userId, decimal amount)
        {
            // Retrieve the user from the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                // Return false if the user does not exist
                return false;
            }

            // Update the user's balance
            user.Money += amount;

            try
            {
                // Save changes to the database
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Log exception if necessary, then return false
                return false;
            }
        }

        public async Task<bool> ChangePassword(int userId, ChangePasswordDto model)
        {
            // Kiểm tra xác nhận mật khẩu mới
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                throw new ArgumentException("Mật khẩu xác nhận không khớp.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Người dùng không tồn tại.");
            }

            // Kiểm tra mật khẩu hiện tại
            if (user.Password != GetMD5(model.CurrentPassword))
            {
                throw new ArgumentException("Mật khẩu hiện tại không đúng.");
            }

            // Cập nhật mật khẩu mới
            user.Password = GetMD5(model.NewPassword);
            user.UpdatedAt = DateTime.Now;

            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
        public static string GetMD5(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(password);
            byte[] targetData = md5.ComputeHash(fromData);
            StringBuilder byte2String = new StringBuilder();

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String.Append(targetData[i].ToString("x2"));
            }
            return byte2String.ToString();
        }

    }
}
