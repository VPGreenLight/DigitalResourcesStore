using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DigitalResourcesStore.EntityFramework
{
    public class DigitalResourcesStoreDbContextFactory : IDesignTimeDbContextFactory<DigitalResourcesStoreDbContext>
    {
        public DigitalResourcesStoreDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "DigitalResourcesStore"))
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<DigitalResourcesStoreDbContext>();
            var connectionString = configuration.GetConnectionString("SqlServer");

            builder.UseSqlServer(connectionString);

            return new DigitalResourcesStoreDbContext(builder.Options);
        }
    }
}
