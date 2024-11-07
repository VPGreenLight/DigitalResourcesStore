﻿global using DigitalResourcesStore.EntityFramework;
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

        }
    }
}
