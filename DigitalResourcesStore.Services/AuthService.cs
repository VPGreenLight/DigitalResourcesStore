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
        string GetUserIdFromToken(string token);
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
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName && u.Password == hashedPassword);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Sai thông tin người dùng");
            }

            string role = user.RoleId == 1 ? "Admin" : "User";
            var token = GenerateJwtToken(user.Id);

            return new LoginResponseViewModel
            {
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

            var userRegistered = await _db.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName || u.Email == model.Email);
            if (userRegistered != null)
            {
                throw new UnauthorizedAccessException("Người dùng đã tồn tại với tên đăng nhập hoặc email này.");
            }

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

            var userId = user.Id;
            var token = GenerateJwtToken(userId);

            return new LoginResponseViewModel
            {
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

        private string GenerateJwtToken(int id)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = Convert.ToInt32(_config["Jwt:ExpiryInMinutes"]);

            // Claims không cần chứa "exp"
            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier, id.ToString())
             };

            // Tạo token với expires được cấu hình chính xác
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes), // Thời gian hết hạn
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ClaimsPrincipal DecodeJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]);
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Console.WriteLine("Token payload:");
            foreach (var kvp in jwtToken.Payload)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
            // Cấu hình các tham số xác thực token
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],

                ValidateLifetime = true, // Kiểm tra thời hạn token
                ClockSkew = TimeSpan.FromMinutes(5), // Cho phép lệch thời gian nhỏ

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]))
            };

            try
            {
                // Xác thực và giải mã token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi token không hợp lệ
                Console.WriteLine($"Token không hợp lệ: {ex.Message}");
                Console.WriteLine($"Chi tiết lỗi: {ex.StackTrace}");
                return null;
            }
        }
        public string GetUserIdFromToken(string token)
        {
            var principal = DecodeJwtToken(token);
            if (principal == null) return null;

            // Lấy claim Id của người dùng từ token (nếu có)
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            return userIdClaim?.Value;
        }
    }
}
