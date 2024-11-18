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
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<IProductDetailService, ProductDetailService>();
            service.AddScoped<ICategoryService, CategoryService>();
            service.AddScoped<IBrandService, BrandService>();
            service.AddScoped<ICaptchaService, CaptchaService>();
            service.AddScoped<ICartService, CartService>();
            //service.AddScoped<IVIPService, VIPService>();
            service.AddScoped<IVnPayService, VnPayService>();
            service.AddScoped<IDepositService, DepositService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IDashboardService, DashboardService>();
            service.AddScoped<IUserFavoriteService, UserFavoriteService>();
            service.AddScoped<ISupportService, SupportService>();
            service.AddScoped<IVoucherService, VoucherService>();
        }
    }
}
