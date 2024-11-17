﻿using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models.CartsDtos;
using DigitalResourcesStore.Services.Queue;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Services
{
    public interface ICartService
    {
        List<CartItemDto> GetCartItems(HttpContext httpContext);
        Task<List<CartItemDto>> GetCartDetails(int userId);

        Task<JsonResult> RemoveFromCart(HttpContext httpContext, int productId);
        Task<JsonResult> IncreaseQuantity(HttpContext httpContext, int productId);
        Task<JsonResult> DecreaseQuantity(HttpContext httpContext, int productId);
        Task<JsonResult> ApplyVoucher(HttpContext httpContext, string code);
        Task<JsonResult> CheckOutAsync(HttpContext httpContext);
        Task ProcessOrderAsync(int userId, List<CartItemDto> cart, decimal discountedTotal, CancellationToken cancellationToken);

        decimal GetCartTotal(HttpContext httpContext);
        Task PopulateCartItemDetails(CartItemDto cartItem);
        Task<JsonResult> Buy(HttpContext httpContext, int productId, int quantity);
    }
    public class CartService : ICartService
    {
        private readonly DigitalResourcesStoreDbContext _dbContext;
        private readonly ILogger<CartService> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;

        public CartService(DigitalResourcesStoreDbContext dbContext, ILogger<CartService> logger, IBackgroundTaskQueue taskQueue)
        {
            _dbContext = dbContext;
            _logger = logger;
            _taskQueue = taskQueue;
        }

        public List<CartItemDto> GetCartItems(HttpContext httpContext) =>
            SessionHelper.GetObjectFromJson<List<CartItemDto>>(httpContext.Session, "cart");

        public async Task<JsonResult> Buy(HttpContext httpContext, int productId, int quantity)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null)
            {
                return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            if (quantity > product.Quantity)
            {
                return new JsonResult(new { success = false, message = "Số lượng mua vượt quá số lượng sản phẩm còn lại." });
            }

            var userId = httpContext.Session.GetInt32("User");
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "Bạn cần phải đăng nhập để mua hàng.", loginRequired = true });
            }

            var cart = GetCartItems(httpContext) ?? new List<CartItemDto>();

            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                if (cartItem.Quantity + quantity > product.Quantity)
                {
                    return new JsonResult(new { success = false, message = "Số lượng mua vượt quá số lượng sản phẩm còn lại." });
                }
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItemDto { ProductId = productId, Quantity = quantity };
                await PopulateCartItemDetails(cartItem);
                cart.Add(cartItem);
            }

            SessionHelper.SetObjectAsJson(httpContext.Session, "cart", cart);

            var order = _dbContext.Orders.FirstOrDefault(o => o.UserId == userId && o.ProductId == productId);
            if (order != null)
            {
                order.Quantity += quantity;
            }
            else
            {
                order = new Order
                {
                    UserId = userId.Value,
                    Quantity = quantity,
                    ProductId = productId
                };
                _dbContext.Orders.Add(order);
            }

            await _dbContext.SaveChangesAsync();

            int totalUniqueProducts = cart.Select(item => item.ProductId).Distinct().Count();
            httpContext.Session.SetInt32("quantity", totalUniqueProducts);

            return new JsonResult(new { success = true, quantity = totalUniqueProducts });
        }

        public async Task<List<CartItemDto>> GetCartDetails(int userId)
        {
            var cart = await _dbContext.Orders
                .Where(o => o.UserId == userId && o.Product != null)
                .Select(o => new CartItemDto
                {
                    ProductId = o.ProductId.Value,
                    ProductName = o.Product.Name,
                    Price = o.Product.Price,
                    Image = o.Product.Image,
                    Quantity = o.Quantity.Value
                })
                .ToListAsync();

            return cart;
        }

        public async Task<JsonResult> RemoveFromCart(HttpContext httpContext, int productId)
        {
            var cart = GetCartItems(httpContext);
            if (cart == null)
                return new JsonResult(new { success = false, message = "Cart is empty." });

            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SessionHelper.SetObjectAsJson(httpContext.Session, "cart", cart);

                var userId = httpContext.Session.GetInt32("User");
                if (userId != null)
                {
                    var orderDetail = _dbContext.Orders.FirstOrDefault(od => od.UserId == userId && od.ProductId == productId);
                    if (orderDetail != null)
                    {
                        _dbContext.Orders.Remove(orderDetail);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                var total = cart.Sum(item => item.Price * item.Quantity);
                int uniqueItems = cart.Select(item => item.ProductId).Distinct().Count();
                httpContext.Session.SetInt32("quantity", uniqueItems);

                return new JsonResult(new { success = true, total = total, countItems = uniqueItems });
            }

            return new JsonResult(new { success = false, message = "Item not found in cart." });
        }

        public async Task<JsonResult> IncreaseQuantity(HttpContext httpContext, int productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null)
            {
                return new JsonResult(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            var cart = GetCartItems(httpContext);
            if (cart == null)
            {
                return new JsonResult(new { success = false, message = "Giỏ hàng trống." });
            }

            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                if (cartItem.Quantity + 1 > product.Quantity)
                {
                    return new JsonResult(new { success = false, message = "Số lượng vượt quá số sản phẩm còn lại." });
                }
                cartItem.Quantity++;

                // Cập nhật cơ sở dữ liệu
                var userId = httpContext.Session.GetInt32("User");
                if (userId != null)
                {
                    var orderDetail = _dbContext.Orders.FirstOrDefault(od => od.UserId == userId && od.ProductId == productId);
                    if (orderDetail != null)
                    {
                        orderDetail.Quantity++;
                        await _dbContext.SaveChangesAsync();
                    }
                }

                SessionHelper.SetObjectAsJson(httpContext.Session, "cart", cart);
                var total = cart.Sum(item => item.Price * item.Quantity);
                int uniqueItems = cart.Select(item => item.ProductId).Distinct().Count();
                httpContext.Session.SetInt32("quantity", uniqueItems);

                return new JsonResult(new { success = true, quantity = cartItem.Quantity, total = total, countItems = uniqueItems });
            }

            return new JsonResult(new { success = false, message = "Sản phẩm không có trong giỏ hàng." });
        }

        public async Task<JsonResult> DecreaseQuantity(HttpContext httpContext, int productId)
        {
            var cart = GetCartItems(httpContext);
            if (cart == null)
            {
                return new JsonResult(new { success = false, message = "Giỏ hàng trống." });
            }

            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;

                // Cập nhật cơ sở dữ liệu
                var userId = httpContext.Session.GetInt32("User");
                if (userId != null)
                {
                    var orderDetail = _dbContext.Orders.FirstOrDefault(od => od.UserId == userId && od.ProductId == productId);
                    if (orderDetail != null)
                    {
                        orderDetail.Quantity--;
                        await _dbContext.SaveChangesAsync();
                    }
                }

                SessionHelper.SetObjectAsJson(httpContext.Session, "cart", cart);
                var total = cart.Sum(item => item.Price * item.Quantity);
                int uniqueItems = cart.Select(item => item.ProductId).Distinct().Count();
                httpContext.Session.SetInt32("quantity", uniqueItems);

                return new JsonResult(new { success = true, quantity = cartItem.Quantity, total = total, countItems = uniqueItems });
            }

            return new JsonResult(new { success = false, message = "Không thể giảm số lượng sản phẩm." });
        }
        public async Task<JsonResult> ApplyVoucher(HttpContext httpContext, string code)
        {
            var voucher = await _dbContext.Vouchers
                .FirstOrDefaultAsync(v => v.Name == code && v.IsActive == true && v.IsDelete == false);

            if (voucher == null)
            {
                return new JsonResult(new { success = false, message = "Mã voucher không hợp lệ hoặc đã hết lượt sử dụng vui lòng kiểm tra lại." });
            }

            var cart = GetCartItems(httpContext);
            if (cart == null || !cart.Any())
            {
                return new JsonResult(new { success = false, message = "Giỏ hàng trống." });
            }

            decimal discount = voucher.Discount;
            var total = cart.Sum(item => item.Price * item.Quantity);
            decimal discountedTotal;

            if (voucher.Type == 1) // % giảm giá
            {
                discountedTotal = total - (total * discount / 100);
            }
            else if (voucher.Type == 2) // Giảm số tiền cố định
            {
                discountedTotal = total - discount;
            }
            else
            {
                return new JsonResult(new { success = false, message = "Loại voucher không hợp lệ." });
            }

            // Lưu thông tin giảm giá vào session
            httpContext.Session.SetString("DiscountedTotal", discountedTotal.ToString());
            httpContext.Session.SetString("VoucherCode", code);

            // Cập nhật thông tin voucher
            if (voucher.RemainUsed == 1)
            {
                voucher.RemainUsed = 0;
                voucher.IsActive = false;
            }
            else
            {
                voucher.RemainUsed -= 1;
            }
            await _dbContext.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                total = discountedTotal,
                discount = discount,
                type = voucher.Type,
                message = "Áp dụng mã giảm giá thành công."
            });
        }

        public async Task<JsonResult> CheckOutAsync(HttpContext httpContext)
        {
            var userId = httpContext.Session.GetInt32("User");
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "Vui lòng đăng nhập.", loginRequired = true });
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "Người dùng không tồn tại." });
            }

            var cart = SessionHelper.GetObjectFromJson<List<CartItemDto>>(httpContext.Session, "cart");
            if (cart == null || !cart.Any())
            {
                return new JsonResult(new { success = false, message = "Giỏ hàng trống." });
            }

            var userMoney = user.Money ?? 0;
            var discountedTotalStr = httpContext.Session.GetString("DiscountedTotal");
            decimal discountedTotal = string.IsNullOrEmpty(discountedTotalStr)
                ? cart.Sum(item => item.Price * item.Quantity)
                : decimal.Parse(discountedTotalStr);

            if (userMoney < discountedTotal)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Số tiền trong tài khoản của bạn không đủ vui lòng nạp thêm hoặc thanh toán qua phương thức khác.",
                });
            }

            try
            {
                // Process order
                await ProcessOrderAsync(userId.Value, cart, discountedTotal, CancellationToken.None);

                // Deduct money from user's account
                user.Money = userMoney - discountedTotal;
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                // Clear cart after successful checkout
                httpContext.Session.Remove("cart");

                return new JsonResult(new
                {
                    success = true,
                    message = "Đơn hàng của bạn đã được xử lý thành công.",
                    pointsEarned = (int)discountedTotal
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing order for user {userId}");
                return new JsonResult(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi xử lý đơn hàng của bạn. Vui lòng thử lại sau."
                });
            }
        }



        public async Task ProcessOrderAsync(int userId, List<CartItemDto> cart, decimal discountedTotal, CancellationToken token)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(token);

            try
            {
                var user = await _dbContext.Users.FindAsync(userId, token);
                if (user == null)
                {
                    throw new InvalidOperationException("Người dùng không tồn tại.");
                }

                if (user.Money < discountedTotal)
                {
                    throw new InvalidOperationException("Số tiền trong tài khoản không đủ để thanh toán.");
                }

                var orderHistories = new List<OrderHistory>();
                var orderHistoryDetails = new List<OrderHistoryDetail>();

                foreach (var item in cart)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, token);
                    if (product == null || product.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Không đủ số lượng sản phẩm {item.ProductName} trong kho.");
                    }

                    product.Quantity -= item.Quantity;
                    _dbContext.Products.Update(product);

                    // Tạo OrderHistory
                    var orderHistory = new OrderHistory
                    {
                        UserId = userId,
                        ProductId = item.ProductId,
                        Date = DateTime.UtcNow,
                        Quantity = item.Quantity,
                        TotalPrice = discountedTotal,
                        ProductName = item.ProductName
                    };
                    orderHistories.Add(orderHistory);
                }

                // Lưu OrderHistories để lấy ID
                await _dbContext.OrderHistories.AddRangeAsync(orderHistories, token);
                await _dbContext.SaveChangesAsync(token); // Lưu để tạo ID cho OrderHistories

                // Tạo OrderHistoryDetails
                foreach (var orderHistory in orderHistories)
                {
                    var productDetails = await _dbContext.ProductDetails
                        .Where(pd => pd.ProductId == orderHistory.ProductId && !pd.IsDelete)
                        .Take(cart.First(c => c.ProductId == orderHistory.ProductId).Quantity)
                        .ToListAsync(token);

                    foreach (var productDetail in productDetails)
                    {
                        orderHistoryDetails.Add(new OrderHistoryDetail
                        {
                            ProductDetailId = productDetail.Id,
                            OrderHistoryId = orderHistory.Id,
                            CategoryId = productDetail.Product.CategoryId ?? 0,
                            Serial = productDetail.Serial,
                            Code = productDetail.Code,
                            ProductName = productDetail.Product.Name
                        });

                        productDetail.IsDelete = true; // Đánh dấu là đã xóa
                    }
                }

                // Lưu OrderHistoryDetails
                await _dbContext.OrderHistoryDetails.AddRangeAsync(orderHistoryDetails, token);
                await _dbContext.SaveChangesAsync(token);

                // Cập nhật thông tin người dùng
                user.Money -= discountedTotal;
                user.MembershipPoints += (int)discountedTotal;
                _dbContext.Users.Update(user);

                await _dbContext.SaveChangesAsync(token);

                // Commit transaction
                await transaction.CommitAsync(token);

                TaskStatusStorage.SetTaskStatus(Guid.NewGuid(), "Completed", userId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(token);
                throw new Exception($"Đã xảy ra lỗi khi xử lý đơn hàng: {ex.Message}", ex);
            }
        }


        public decimal GetCartTotal(HttpContext httpContext)
        {
            var cart = GetCartItems(httpContext);
            return cart?.Sum(item => item.TotalPrice) ?? 0;
        }

        public async Task PopulateCartItemDetails(CartItemDto cartItem)
        {
            var product = await _dbContext.Products.FindAsync(cartItem.ProductId);
            if (product != null)
            {
                cartItem.ProductName = product.Name;
                cartItem.Price = product.Price;
            }
        }
    }

    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
