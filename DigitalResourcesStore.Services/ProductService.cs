﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalResourcesStore.Models.ProductDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using QuizApp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace DigitalResourcesStore.Services
{
    public interface IProductService
    {
        Task<ProductDtos> GetById(int id);
        Task<PagedResponse<ProductDtos>> Get(QueryProductDto query);
        Task<bool> Create(CreatedProductDtos request);
        Task<bool> Update(int id, UpdateProductDtos request);
        Task<bool> Delete(int id);
        //Task<bool> ChangePassword(int userId, ChangePasswordDto changePasswordDto);
    }
    public class ProductService : IProductService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;

        public ProductService(IConfiguration config, DigitalResourcesStoreDbContext db)
        {
            _config = config;
            _db = db;
        }

        public async Task<ProductDtos> GetById(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstAsync(c => c.Id == id);
            if (product == null)
            {
                throw new ArgumentException("Sản phẩm không hợp lệ");
            };

            return new ProductDtos
            {
                //Id = product.Id,
                Name = product.Name,
                Image = product.Image,
                Expiry = DateTime.UtcNow,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Brand = product.Brand.Name,
                Category = product.Category.Name,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                UpdatedAt = product.UpdatedAt,
                UpdatedBy = product.UpdatedBy,
                DeletedAt = product.DeletedAt,
                IsDelete = product.IsDelete,
                DeletedBy = product.DeletedBy,

            };
        }
        public async Task<PagedResponse<ProductDtos>> Get(QueryProductDto query)
        {
            var productsQuery = _db.Products.AsQueryable();

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                string lowerKeyword = query.Keyword.ToLower();
                productsQuery = productsQuery.Where(product =>
                    product.Name.ToLower().Contains(lowerKeyword) ||
                    product.Description.ToLower().Contains(lowerKeyword));
            }

            // Đếm tổng số sản phẩm thỏa mãn điều kiện tìm kiếm
            var totalCount = await productsQuery.CountAsync();

            // Phân trang
            var pagedProducts = await productsQuery
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // Chuyển đổi sang ProductDto
            var productDtos = pagedProducts.Select(product => new ProductDtos
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Image = product.Image,
                Expiry = DateTime.UtcNow,
                Brand = product.Brand.Name,
                Category = product.Category.Name,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                UpdatedAt = product.UpdatedAt,
                UpdatedBy = product.UpdatedBy,
                DeletedAt = product.DeletedAt,
                IsDelete = product.IsDelete,
                DeletedBy = product.DeletedBy,

            }).ToList();

            // Trả về PagedResponse
            return new PagedResponse<ProductDtos>(productDtos, query.PageIndex, query.PageSize, totalCount);
        }
        public async Task<bool> Create(CreatedProductDtos viewModel)
        {
            var brand = await _db.Brands.FindAsync(viewModel.BrandId);
            if (brand == null)
            {
                throw new ArgumentException("Brand không hợp lệ");
            }

            // Tìm Category dựa trên CategoryId từ viewModel
            var category = await _db.Categories.FindAsync(viewModel.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Category không hợp lệ");
            }

            var Product = new Product
            {
                //Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                Quantity = viewModel.Quantity,
                Image = viewModel.Image,
                Expiry = DateTime.UtcNow,
                Brand = brand,
                Category = category,
                CreatedAt = DateTime.Now, 
                CreatedBy = "admin",
            };

            await _db.Products.AddAsync(Product);
            return await _db.SaveChangesAsync() > 0;
        }
        public async Task<bool> Update(int id, UpdateProductDtos viewModel)
        {
            var product = await _db.Products
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new ArgumentException("Sản phẩm không hợp lệ");
            };

            // Tìm Brand dựa trên BrandId từ viewModel
            if (viewModel.BrandId.HasValue)
            {
                var brand = await _db.Brands.FindAsync(viewModel.BrandId.Value);
                if (brand == null)
                {
                    throw new ArgumentException("Brand không hợp lệ");
                }
                product.Brand = brand;
            }

            // Tìm Category dựa trên CategoryId từ viewModel
            if (viewModel.CategoryId.HasValue)
            {
                var category = await _db.Categories.FindAsync(viewModel.CategoryId.Value);
                if (category == null)
                {
                    throw new ArgumentException("Category không hợp lệ");
                }
                product.Category = category;
            }
            product.Name = viewModel.Name ;
            product.Description = viewModel.Description ;
            product.Price = viewModel.Price ;
            product.Quantity = viewModel.Quantity;
            product.Image = viewModel.Image ;
            product.Expiry = viewModel.Expiry ;
            product.UpdatedAt = DateTime.Now;
            product.UpdatedBy = "admin"; // Có thể thay đổi người dùng cập nhật nếu cần

            _db.Products.Update(product);
            return await _db.SaveChangesAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                throw new ArgumentException("Sản phẩm không hợp lệ");
            };
            product.IsDelete = true;
            product.DeletedAt = DateTime.Now;
            product.DeletedBy = "admin";
            //_db.Products.Remove(user);
            return await _db.SaveChangesAsync() > 0;
        }

    }
}