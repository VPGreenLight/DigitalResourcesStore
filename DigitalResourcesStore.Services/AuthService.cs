using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalResourcesStore.Models.AuthDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
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
        public AuthService(DigitalResourcesStoreDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public async Task<LoginResponseViewModel> LoginAsync(LoginViewModel model)
        {
            // Placeholder for user verification logic (e.g., database lookup).
            // Assume user verification is successful for this example.
            var isValidUser = ValidateUser(model.userName, model.Password);
            if (!isValidUser)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            var token = GenerateJwtToken(model.userName, "User");
            return new LoginResponseViewModel
            {
                UserInformation = "User information as JSON",
                Token = token,
                Expires = DateTime.UtcNow.AddHours(1)
            };
        }
        public async Task<LoginResponseViewModel> RegisterAsync(RegisterViewModel model)
        {
            bool isRegistered = RegisterUser(model);
            if (!isRegistered)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            if (model.Password != model.ConfirmPassword)
            {
                throw new ArgumentException("Mật khẩu và xác nhận mật khẩu không trùng khớp.");
            }
            var token = GenerateJwtToken(model.UserName, "User");
            User user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                IsActive = true,
                IsDelete = false,
                Money = 0,
                RoleId = 2,
                //ReferralCode = GetUniqueReferralCode(context),
                //FriendReferralCode = friendReferralCode // Gán mã mời bạn bè vào thuộc tính FriendReferralCode
            };
            var users = _db.Set<User>();
            users.Add(user);
            _db.SaveChanges();
            return new LoginResponseViewModel
            {
                UserInformation = "User information as JSON",
                Token = token,
                Expires = DateTime.UtcNow.AddHours(1)
            };
        }
        private bool ValidateUser(string userName, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                return false;
            }
            return true;
        }
        private bool RegisterUser(RegisterViewModel model)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName == model.UserName && u.Email == model.Email);
            if (user == null)
            {
                return true;
            }
            return false;
        }
        private string GenerateJwtToken(string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
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
        private int GetTokenExpiryInMinutes()
        {
            return int.TryParse(_config["Jwt:ExpiryInMinutes"], out var expiry) ? expiry : 30;
        }
    }
}