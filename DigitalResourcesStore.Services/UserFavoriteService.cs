using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalResourcesStore.Models.UserFavoriteDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace DigitalResourcesStore.Services
{
    public interface IUserFavoriteService
    {
        Task<bool> AddToFavorite(int userId, int productId);
        Task<bool> RemoveFromFavorite(int userId, int productId);
        Task<List<UserFavoriteDto>> GetFavoritesByUserId(int userId);
    }
    public class UserFavoriteService : IUserFavoriteService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;
        public UserFavoriteService(DigitalResourcesStoreDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public async Task<bool> AddToFavorite(int userId, int productId)
        {
            var favorite = new UserFavorite
            {
                UserId = userId,
                ProductId = productId,
                CreatedAt = DateTime.Now
            };

            _db.UserFavorites.Add(favorite);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFromFavorite(int userId, int productId)
        {
            var favorite = await _db.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

            if (favorite != null)
            {
                _db.UserFavorites.Remove(favorite);
                await _db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<UserFavoriteDto>> GetFavoritesByUserId(int userId)
        {
            var favorites = await _db.UserFavorites
                .Where(f => f.UserId == userId)
                .Select(f => new UserFavoriteDto
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    ProductId = f.ProductId,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();

            return favorites;
        }
    }
}
