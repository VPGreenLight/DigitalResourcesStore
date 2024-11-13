using DigitalResourcesStore.Models.DashBoardDtos;
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
    public interface IDashboardService
    {
        Task<DashBoardDtos> GetDashboardDataAsync();
    }
    public class DashboardService : IDashboardService
    {
        private readonly IConfiguration _config;
        private readonly DigitalResourcesStoreDbContext _db;

        public DashboardService(IConfiguration config, DigitalResourcesStoreDbContext db)
        {
            _config = config;
            _db = db;
        }
        public async Task<DashBoardDtos> GetDashboardDataAsync()
        {
            var viewModel = new DashBoardDtos
            {
                ProductCount = await _db.Products.CountAsync(),
                UserCount = await _db.Users.CountAsync(),
                FeedbackCount = await _db.Feedbacks.CountAsync(),
                OrderCount = await _db.OrderHistories.CountAsync(),
                TotalRevenue = await _db.OrderHistories.SumAsync(o => o.TotalPrice),
                QuantitySold = (await _db.OrderHistories.SumAsync(o => o.Quantity)) ?? 0
            };  

            viewModel.PercentGrowth = CalculatePercentageRevenueGrowth();
            LoadMonthlyRevenues(viewModel);
            viewModel.BestSellingProducts = await LoadBestSellingProductsAsync();

            return viewModel;
        }
        private async Task<List<BestSellingProductDtos>> LoadBestSellingProductsAsync()
        {
            var bestSellingProducts = await _db.OrderHistories
                .GroupBy(o => o.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(o => o.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(6) // Top 6 best-selling products
                .ToListAsync();

            return bestSellingProducts
                .Select(item => new BestSellingProductDtos
                {
                    ProductId = item.ProductId,
                    ProductName = _db.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name,
                    TotalQuantitySold = item.TotalQuantity
                })
                .ToList();
        }
        private decimal CalculatePercentageRevenueGrowth()
        {
            try
            {
                var previousMonthRevenue = _db.OrderHistories
                    .Where(o => o.Date.HasValue && o.Date.Value.Year == DateTime.Now.Year && o.Date.Value.Month == DateTime.Now.Month - 1)
                    .Sum(o => o.TotalPrice);

                if (previousMonthRevenue == 0)
                    return 1000;

                var currentMonthRevenue = _db.OrderHistories
                    .Where(o => o.Date.HasValue && o.Date.Value.Year == DateTime.Now.Year && o.Date.Value.Month == DateTime.Now.Month)
                    .Sum(o => o.TotalPrice);

                return (currentMonthRevenue - previousMonthRevenue) / previousMonthRevenue * 100;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CalculatePercentageRevenueGrowth: {ex.Message}");
                return 0;
            }
        }

        private void LoadMonthlyRevenues(DashBoardDtos viewModel)
        {
            try
            {
                var monthlyData = _db.OrderHistories
                    .Where(o => o.Date.HasValue && o.Date.Value.Year == DateTime.Now.Year)
                    .GroupBy(o => o.Date.Value.Month)
                    .Select(g => new { Month = g.Key, Total = g.Sum(o => o.TotalPrice) })
                    .ToList();

                foreach (var data in monthlyData)
                {
                    viewModel.MonthlyRevenues[data.Month - 1] = data.Total;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadMonthlyRevenues: {ex.Message}");
            }
        }

    }
}
