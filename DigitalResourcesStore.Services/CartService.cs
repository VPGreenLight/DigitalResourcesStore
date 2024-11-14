using DigitalResourcesStore.EntityFramework.Models;
using DigitalResourcesStore.Models.CartsDtos;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Services
{
    public interface ICartService
    {
        List<CartItemDto> GetCartItems(HttpContext httpContext);
        Task AddToCart(HttpContext httpContext, int productId, int quantity);
        void UpdateCartQuantity(HttpContext httpContext, int productId, int quantity);
        decimal GetCartTotal(HttpContext httpContext);
        Task PopulateCartItemDetails(CartItemDto cartItem);
    }
    public class CartService : ICartService
    {
        private readonly DigitalResourcesStoreDbContext _dbContext;

        public CartService(DigitalResourcesStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CartItemDto> GetCartItems(HttpContext httpContext) =>
            SessionHelper.GetObjectFromJson<List<CartItemDto>>(httpContext.Session, "cart");

        public async Task AddToCart(HttpContext httpContext, int productId, int quantity)
        {
            var cart = GetCartItems(httpContext) ?? new List<CartItemDto>();

            var cartItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItemDto { ProductId = productId, Quantity = quantity };
                await PopulateCartItemDetails(cartItem);
                cart.Add(cartItem);
            }

            SessionHelper.SetObjectAsJson(httpContext.Session, "cart", cart);
        }

        public void UpdateCartQuantity(HttpContext httpContext, int productId, int quantity)
        {
            var cart = GetCartItems(httpContext);
            if (cart != null)
            {
                var item = cart.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    SessionHelper.SetObjectAsJson(httpContext.Session, "cart", cart);
                }
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
