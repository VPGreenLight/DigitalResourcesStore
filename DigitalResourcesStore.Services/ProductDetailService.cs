using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DigitalResourcesStore.Models.ProductDetailDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models;
using static System.Net.Mime.MediaTypeNames;
using DigitalResourcesStore.Models.ProductDtos;

namespace DigitalResourcesStore.Services
{
    public interface IProductDetailService
    {
        Task<ProductDetailDtos> GetById(int id);
        Task<PagedResponse<ProductDetailDtos>> Get(QueryProductDetailDto query);
        Task<bool> Create(CreateProductDetailDtos request);
        Task<bool> Update(int id, UpdateProductDetailDtos request);
        Task<bool> Delete(int id);
    }
    class ProductDetailService : IProductDetailService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;

        public ProductDetailService(IConfiguration config, DigitalResourcesStoreDbContext db)
        {
            _config = config;
            _db = db;
        }
        public async Task<ProductDetailDtos> GetById(int id)
        {
            var productdetail = await _db.ProductDetails
                .FirstAsync(c => c.Id == id);
            if (productdetail == null)
            {
                throw new ArgumentException("Sản phẩm không hợp lệ");
            };

            return new ProductDetailDtos
            {
                Id = productdetail.Id,
                Serial = productdetail.Serial,
                Code = productdetail.Code,
                ProductId = productdetail.ProductId,
                IsDelete = productdetail.IsDelete,
            };
        }
        public async Task<PagedResponse<ProductDetailDtos>> Get(QueryProductDetailDto query)
        {
            var productsdetailQuery = _db.ProductDetails.AsQueryable();

            // Tìm kiếm theo từ khóa nếu có
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                string lowerKeyword = query.Keyword.ToLower();
                productsdetailQuery = productsdetailQuery.Where(p =>
                    p.Serial.ToLower().Contains(lowerKeyword) ||
                    p.Code.ToLower().Contains(lowerKeyword));
            }

            // Đếm tổng số sản phẩm thỏa mãn điều kiện tìm kiếm
            var totalCount = await productsdetailQuery.CountAsync();

            // Phân trang
            var pagedProductDetail = await productsdetailQuery
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // Chuyển đổi sang ProductDto
            var productdetailDtos = pagedProductDetail.Select(productdetail => new ProductDetailDtos
            {
                Id = productdetail.Id,
                Serial = productdetail.Serial,
                Code = productdetail.Code,
                IsDelete = productdetail.IsDelete,

            }).ToList();

            // Trả về PagedResponse
            return new PagedResponse<ProductDetailDtos>(productdetailDtos, query.PageIndex, query.PageSize, totalCount);
        }
        public async Task<bool> Create(CreateProductDetailDtos viewModel)
        {
            //var product = await _db.Products.FindAsync(viewModel.ProductId);
            //if (product == null)
            //{
            //    throw new ArgumentException("Sản phẩm không hợp lệ");
            //}
            var existingProductDetail = await _db.ProductDetails
        .FirstOrDefaultAsync(pd => pd.Serial == viewModel.Serial || pd.Code == viewModel.Code);

            if (existingProductDetail != null)
            {
                throw new ArgumentException("Serial hoặc Code đã tồn tại.");
            }
            var productDetail = new ProductDetail
            {
                Serial = viewModel.Serial,
                Code = viewModel.Code,
                ProductId = viewModel.ProductId,
                IsDelete = false,
            };

            await _db.ProductDetails.AddAsync(productDetail);
            return await _db.SaveChangesAsync() > 0;
        }
        public async Task<bool> Update(int id, UpdateProductDetailDtos viewModel)
        {
            var productDetail = await _db.ProductDetails
        .Include(pd => pd.Product) // Bao gồm thông tin về Product nếu cần
        .FirstOrDefaultAsync(pd => pd.Id == id);

            if (productDetail == null)
            {
                throw new ArgumentException("Chi tiết sản phẩm không hợp lệ");
            }

            // Kiểm tra nếu ProductId có thay đổi và cập nhật thông tin Product nếu cần
            if (viewModel.ProductId.HasValue && productDetail.ProductId != viewModel.ProductId.Value)
            {
                var product = await _db.Products.FindAsync(viewModel.ProductId.Value);
                if (product == null)
                {
                    throw new ArgumentException("Sản phẩm liên kết không hợp lệ");
                }
                productDetail.ProductId = viewModel.ProductId.Value;
                productDetail.Product = product; // Cập nhật thông tin về Product nếu cần
            }

            productDetail.Serial = viewModel.Serial;
            productDetail.Code = viewModel.Code;
            productDetail.IsDelete = viewModel.IsDelete;

            _db.ProductDetails.Update(productDetail);
            return await _db.SaveChangesAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            var productDetail = await _db.ProductDetails.FindAsync(id);
            if (productDetail == null)
            {
                throw new ArgumentException("Chi tiết sản phẩm không hợp lệ");
            }
            productDetail.IsDelete = true;

            //_db.Products.Remove(productDetail);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
