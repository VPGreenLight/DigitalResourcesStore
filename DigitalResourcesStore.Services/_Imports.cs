global using DigitalResourcesStore.EntityFramework;
global using Microsoft.Extensions.DependencyInjection;

namespace DigitalResourcesStore.Services
{
    public static class _Imports
    {
        public static void AddServiceCollections(this IServiceCollection service)
        {
            // Import các service vào hệ thống theo cấu trúc đã cho
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<IProductDetailService, ProductDetailService>();
            service.AddScoped<ICategoryService, CategoryService>();
            service.AddScoped<IBrandService, BrandService>();
        }
    }
}
//hải bm có service méo đâu thế khả năng m chưa push lên hoặc sao đó nên git kh lưu
