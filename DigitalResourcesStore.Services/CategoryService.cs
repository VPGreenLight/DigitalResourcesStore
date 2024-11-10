using DigitalResourcesStore.Models.CategoryDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using QuizApp.Models;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Configuration;

namespace DigitalResourcesStore.Services
{
    public interface ICategoryService
    {
        Task<CategoryDtos> GetById(int id);
        Task<PagedResponse<CategoryDtos>> Get(QueryCategoryDto query);
        Task<bool> Create(CreatedCategoryDtos request);
        Task<bool> Update(int id, UpdateCategoryDtos request);
        Task<bool> Delete(int id);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;

        public CategoryService(IConfiguration config, DigitalResourcesStoreDbContext db)
        {
            _config = config;
            _db = db;
        }
        public async Task<CategoryDtos> GetById(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Danh mục không hợp lệ");
            }

            return new CategoryDtos
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                CreatedBy = category.CreatedBy,
                UpdatedAt = category.UpdatedAt,
                UpdatedBy = category.UpdatedBy,
                DeletedAt = category.DeletedAt,
                IsDelete = category.IsDelete,
                DeletedBy = category.DeletedBy,
            };
        }
        public async Task<PagedResponse<CategoryDtos>> Get(QueryCategoryDto query)
        {
            var categoriesQuery = _db.Categories.AsQueryable();

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                string lowerKeyword = query.Keyword.ToLower();
                categoriesQuery = categoriesQuery.Where(category =>
                    category.Name.ToLower().Contains(lowerKeyword));
            }

            // Đếm tổng số danh mục thỏa mãn điều kiện tìm kiếm
            var totalCount = await categoriesQuery.CountAsync();

            // Phân trang
            var pagedCategories = await categoriesQuery
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // Chuyển đổi sang CategoryDto
            var categoryDtos = pagedCategories.Select(category => new CategoryDtos
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                CreatedBy = category.CreatedBy,
                UpdatedAt = category.UpdatedAt,
                UpdatedBy = category.UpdatedBy,
                DeletedAt = category.DeletedAt,
                IsDelete = category.IsDelete,
                DeletedBy = category.DeletedBy,
            }).ToList();

            // Trả về PagedResponse
            return new PagedResponse<CategoryDtos>(categoryDtos, query.PageIndex, query.PageSize, totalCount);
        }

        public async Task<bool> Create(CreatedCategoryDtos viewModel)
        {
            bool ischeck = await _db.Categories
        .AnyAsync(c => c.Name.ToLower() == viewModel.Name.ToLower() && !(c.IsDelete ?? false));

            if (ischeck)
            {
                throw new ArgumentException("Tên danh mục đã tồn tại.");
            }
            var category = new Category
            {
                Name = viewModel.Name,
                CreatedAt = DateTime.Now,
                CreatedBy = "admin"
            };

            await _db.Categories.AddAsync(category);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(int id, UpdateCategoryDtos viewModel)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Danh mục không hợp lệ");
            }

            category.Name = viewModel.Name;
            category.UpdatedAt = DateTime.Now;
            category.UpdatedBy = "admin"; // Có thể thay đổi người dùng cập nhật nếu cần

            _db.Categories.Update(category);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                throw new ArgumentException("Danh mục không hợp lệ");
            }

            category.IsDelete = true;
            category.DeletedAt = DateTime.Now;
            category.DeletedBy = "admin";

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
