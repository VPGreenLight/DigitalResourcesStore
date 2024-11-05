using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.EntityFramework
{
    public static class DbContextConfiguration
    {
        public static void AddDbConfig(this IServiceCollection services, IConfiguration configs)
        {
            services.AddDbContext<DigitalResourcesStoreDbContext>(options =>
                options.UseSqlServer(
                    configs.GetConnectionString("SqlServer"),
                    b => b.MigrationsAssembly("DigitalResourcesStore.EntityFramework")
                ));
        }
    }
}
