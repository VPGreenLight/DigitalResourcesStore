using Microsoft.Extensions.Configuration;
using DigitalResourcesStore.Models.UserFavoriteDtos;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using Microsoft.AspNetCore.Http;

namespace DigitalResourcesStore.Services
{
    public interface IUserFavoriteService
    {
        Task<bool> AddToFavorite(HttpContext httpContext, int productId);
        Task<bool> RemoveFromFavorite(HttpContext httpContext, int productId);
        Task<List<UserFavoriteResponse>> GetFavoritesByUserId(HttpContext httpContext);
    }
    public class UserFavoriteService : IUserFavoriteService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;
        private readonly IAuthService _authService;
        public UserFavoriteService(DigitalResourcesStoreDbContext db, IConfiguration config, IAuthService authService)
        {
            _db = db;
            _config = config;
            _authService = authService;
        }
        public async Task<bool> AddToFavorite(HttpContext httpContext, int productId)
        {
            int userId = int.Parse(GetUserIdFromToken(httpContext.Request));

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

        public async Task<bool> RemoveFromFavorite(HttpContext httpContext, int productId)
        {
            int userId = int.Parse(GetUserIdFromToken(httpContext.Request));

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

        public async Task<List<UserFavoriteResponse>> GetFavoritesByUserId(HttpContext httpContext)
        {
            try
            {
                int userId = int.Parse(GetUserIdFromToken(httpContext.Request));

                // Truy vấn sản phẩm yêu thích
                var favoriteProducts = await _db.UserFavorites
                    .Include(f => f.Product)
                    .ThenInclude(p => p.Category)
                    .Where(f => f.UserId == userId)
                    .Select(f => new UserFavoriteResponse
                    {
                        UserId = f.UserId,
                        ProductID = f.Product.Id,
                        ProductName = f.Product.Name,
                        Category = f.Product.Category != null ? f.Product.Category.Name : "Không xác định",
                        Price = f.Product.Price,
                        Image = f.Product.Image
                    })
                    .ToListAsync();

                return favoriteProducts;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException("Lỗi xác thực: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi lấy danh sách sản phẩm yêu thích: " + ex.Message);
            }
        }

        private string GetUserIdFromToken(HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].ToString();
            var token = authHeader?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token không tồn tại.");
            }

            var userId = _authService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Token không hợp lệ hoặc hết hạn.");
            }

            return userId;
        }
    }
}