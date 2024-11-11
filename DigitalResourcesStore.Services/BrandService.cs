using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalResourcesStore.Models.BrandDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using QuizApp.Models;
using static System.Net.Mime.MediaTypeNames;
using DigitalResourcesStore.Models.ProductDtos;

namespace DigitalResourcesStore.Services
{
    public interface IBrandService
    {
        Task<BrandDtos> GetById(int id);
        Task<PagedResponse<BrandDtos>> Get(QueryBrandDto query);
        Task<bool> Create(CreatedBrandDtos request);
        Task<bool> Update(int id, UpdateBrandDtos request);
        Task<bool> Delete(int id);
    }
     public class BrandService : IBrandService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;

        public BrandService(IConfiguration config, DigitalResourcesStoreDbContext db)
        {
            _config = config;
            _db = db;
        }
        public async Task<BrandDtos> GetById(int id)
        {
            var brand = await _db.Brands.FindAsync(id);
            if (brand == null)
            {
                throw new ArgumentException("Thương hiệu không hợp lệ");
            }

            return new BrandDtos
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
                CreatedBy = brand.CreatedBy,
                UpdatedAt = brand.UpdatedAt,
                UpdatedBy = brand.UpdatedBy,
                DeletedAt = brand.DeletedAt,
                IsDelete = brand.IsDelete,
                DeletedBy = brand.DeletedBy,
            };
        }
        public async Task<PagedResponse<BrandDtos>> Get(QueryBrandDto query)
        {
            var brandsQuery = _db.Brands.AsQueryable();

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                string lowerKeyword = query.Keyword.ToLower();
                brandsQuery = brandsQuery.Where(brand =>
                    brand.Name.ToLower().Contains(lowerKeyword));
            }

            // Đếm tổng số thương hiệu thỏa mãn điều kiện tìm kiếm
            var totalCount = await brandsQuery.CountAsync();

            // Phân trang
            var pagedBrands = await brandsQuery
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // Chuyển đổi sang BrandDtos
            var brandDtos = pagedBrands.Select(brand => new BrandDtos
            {
                Id = brand.Id,
                Name = brand.Name,
                CreatedAt = brand.CreatedAt,
                CreatedBy = brand.CreatedBy,
                UpdatedAt = brand.UpdatedAt,
                UpdatedBy = brand.UpdatedBy,
                DeletedAt = brand.DeletedAt,
                IsDelete = brand.IsDelete,
                DeletedBy = brand.DeletedBy,
            }).ToList();

            // Trả về PagedResponse
            return new PagedResponse<BrandDtos>(brandDtos, query.PageIndex, query.PageSize, totalCount);
        }

        public async Task<bool> Create(CreatedBrandDtos viewModel)
        {
            bool isCheck = await _db.Brands
                .AnyAsync(b => b.Name.ToLower() == viewModel.Name.ToLower() && !(b.IsDelete ?? false));

            if (isCheck)
            {
                throw new ArgumentException("Tên thương hiệu đã tồn tại.");
            }

            var brand = new Brand
            {
                Name = viewModel.Name,
                CreatedAt = DateTime.Now,
                CreatedBy = "admin"
            };

            await _db.Brands.AddAsync(brand);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(int id, UpdateBrandDtos viewModel)
        {
            var brand = await _db.Brands.FindAsync(id);
            if (brand == null)
            {
                throw new ArgumentException("Thương hiệu không hợp lệ");
            }

            brand.Name = viewModel.Name;
            brand.UpdatedAt = DateTime.Now;
            brand.UpdatedBy = "admin"; // Có thể thay đổi người dùng cập nhật nếu cần

            _db.Brands.Update(brand);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var brand = await _db.Brands.FindAsync(id);
            if (brand == null)
            {
                throw new ArgumentException("Thương hiệu không hợp lệ");
            }

            brand.IsDelete = true;
            brand.DeletedAt = DateTime.Now;
            brand.DeletedBy = "admin";

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
