using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models.CartsDtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DigitalResourcesStore.Services
{
    // IOrderService.cs
    public interface IOrderService
    {
        Task ProcessOrderAsync(int userId, List<CartItemDto> cart, decimal discountedTotal);
        List<OrderHistory> GetOrderHistory(int userId);
    }

    // OrderService.cs
    public class OrderService : IOrderService
    {
        private readonly DigitalResourcesStoreDbContext _dbContext;
        private readonly ILogger<OrderService> _logger;

        public OrderService(DigitalResourcesStoreDbContext dbContext, ILogger<OrderService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task ProcessOrderAsync(int userId, List<CartItemDto> cart, decimal discountedTotal)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var orderHistoryDetails = new List<OrderHistoryDetail>();
                foreach (var item in cart)
                {
                    var product = await _dbContext.Products.FindAsync(item.ProductId);
                    if (product == null || product.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient quantity for product {item.ProductName}");
                    }

                    product.Quantity -= item.Quantity;
                    _dbContext.Products.Update(product);

                    var orderHistory = new OrderHistory
                    {
                        UserId = userId,
                        ProductId = item.ProductId,
                        Date = DateTime.Now,
                        Quantity = item.Quantity,
                        TotalPrice = discountedTotal
                    };

                    await _dbContext.OrderHistories.AddAsync(orderHistory);
                    await _dbContext.SaveChangesAsync();

                    foreach (var productDetail in await _dbContext.ProductDetails
                                                                 .Where(pd => pd.ProductId == item.ProductId && !pd.IsDelete)
                                                                 .Take(item.Quantity)
                                                                 .ToListAsync())
                    {
                        orderHistoryDetails.Add(new OrderHistoryDetail
                        {
                            ProductDetailId = productDetail.Id,
                            OrderHistoryId = orderHistory.Id,
                            CategoryId = product.CategoryId.Value
                        });

                        productDetail.IsDelete = true;
                    }
                }

                await _dbContext.OrderHistoryDetails.AddRangeAsync(orderHistoryDetails);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Order processing failed for user {userId}");
                await transaction.RollbackAsync();
            }
        }

        public List<OrderHistory> GetOrderHistory(int userId) =>
            _dbContext.OrderHistories.Where(o => o.UserId == userId).ToList();
    }

}
