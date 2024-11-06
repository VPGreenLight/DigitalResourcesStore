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
        //Task<LoginResponseViewModel> RegisterAsync(RegisterViewModel registerViewModel);
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
            var user = await _db.Users.Include(u => u.Role)
                                      .FirstOrDefaultAsync(u => u.UserName == model.userName && u.Password == model.Password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            string role = user.RoleId == 1 ? "Admin" : "User";
            var token = GenerateJwtToken(model.userName, role);

            return new LoginResponseViewModel
            {
                UserInformation = "User information as JSON",
                Token = token,
                Expires = DateTime.UtcNow.AddHours(1)
            };
        }
        //public async Task<LoginResponseViewModel> RegisterAsync(RegisterViewModel model)
        //{
        //    bool isRegistered = RegisterUser(model);
        //    if (!isRegistered)
        //    {
        //        throw new UnauthorizedAccessException("Invalid credentials");
        //    }
        //    var token = GenerateJwtToken(model.userName, "User");
        //    return new LoginResponseViewModel
        //    {
        //        UserInformation = "User information as JSON",
        //        Token = token,
        //        Expires = DateTime.UtcNow.AddHours(1)
        //    };
        //}

        //private bool RegisterUser(RegisterViewModel model)
        //{
        //    return true;
        //}
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