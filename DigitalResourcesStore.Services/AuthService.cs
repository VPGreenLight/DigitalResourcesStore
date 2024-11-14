using Microsoft.AspNetCore.Http;  // Make sure this is included
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalResourcesStore.Models.AuthDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using System.Security.Cryptography;
using Newtonsoft.Json;
using DigitalResourcesStore.Models.UserDtos;

namespace DigitalResourcesStore.Services
{
    public interface IAuthService
    {
        Task<LoginResponseViewModel> LoginAsync(LoginViewModel loginViewModel);
        Task<LoginResponseViewModel> RegisterAsync(RegisterViewModel registerViewModel);
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor; // Inject IHttpContextAccessor

        // Inject IHttpContextAccessor along with other services
        public AuthService(DigitalResourcesStoreDbContext db, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _config = config;
            _httpContextAccessor = httpContextAccessor;  // Initialize the IHttpContextAccessor
        }

        public async Task<LoginResponseViewModel> LoginAsync(LoginViewModel model)
        {
            var hashedPassword = GetMD5(model.Password);
            var user = await _db.Users.Include(u => u.Role)
                                          .FirstOrDefaultAsync(u => u.UserName == model.userName && u.Password == hashedPassword);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            string role = user.RoleId == 1 ? "Admin" : "User";

            // Use IHttpContextAccessor to access the session
            _httpContextAccessor.HttpContext.Session.SetInt32("User", user.Id);

            var token = GenerateJwtToken(model.userName, role);

            var userLoginDto = new UserLoginDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                RoleName = user.Role.Name,
                Money = user.Money.HasValue ? (decimal)user.Money.Value : 0,
                VipRank = user.VipRank
            };

            return new LoginResponseViewModel
            {
                UserInformation = userLoginDto,
                Token = token,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };
        }

        public async Task<LoginResponseViewModel> RegisterAsync(RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                throw new ArgumentException("Mật khẩu và xác nhận mật khẩu không trùng khớp.");
            }

            bool isRegistered = await _db.Users.AnyAsync(u => u.UserName == model.UserName || u.Email == model.Email);
            if (isRegistered)
            {
                throw new UnauthorizedAccessException("Người dùng đã tồn tại với tên đăng nhập hoặc email này.");
            }

            var token = GenerateJwtToken(model.UserName, "User");
            var hashedPassword = GetMD5(model.Password);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = hashedPassword,
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                IsActive = true,
                IsDelete = false,
                Money = 0,
                RoleId = 2
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            var userLoginDto = new UserLoginDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                RoleName = "User",
                Money = user.Money ?? 0,
                VipRank = user.VipRank
            };

            return new LoginResponseViewModel
            {
                UserInformation = userLoginDto,
                Token = token,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };
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

        private string GenerateJwtToken(string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
