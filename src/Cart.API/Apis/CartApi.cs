using Cart.API.Models;
using Cart.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using StackExchange.Redis;
using System.Text.Json;

namespace Cart.API.Apis
{
    public static class CartApi
    {
        public static IEndpointRouteBuilder MapCartApi(this IEndpointRouteBuilder app)
        {
            app.MapGet("api/cart", GetCart).WithName("GetMyCart");

            return app;
        }

        public static async Task<Ok<CustomerCart>> GetCart(
            string userId, 
            IConnectionMultiplexer redis)
        {
            var db = redis.GetDatabase();

            var cartData = await db.StringGetAsync($"cart:{userId}");

            if (string.IsNullOrEmpty(cartData))
            {
                return TypedResults.Ok(new CustomerCart { CustomerId = userId });
            }
            
            var cart = JsonSerializer.Deserialize<CustomerCart>(cartData);
            
            return TypedResults.Ok(cart);
        }
        
        public static async Task<Results<Ok, NotFound>> AddCartItem(
            string userId,
            int catalogItemId,
            int quantity,
            ICatalogServiceClient catalogClient,
            IConnectionMultiplexer redis)
        {
            var db = redis.GetDatabase();

            var catalogItem = await catalogClient.GetCatalogItemAsync(catalogItemId);

            if (catalogItem == null)
            {
                return TypedResults.NotFound();
            }

            var cartData = await db.StringGetAsync($"cart:{userId}");

            var cart = string.IsNullOrEmpty(cartData) 
                ? new CustomerCart { CustomerId = userId } 
                : JsonSerializer.Deserialize<CustomerCart>(cartData);

            var item = cart.Items.FirstOrDefault(c => c.ProductId == catalogItemId);

            if (item is not null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = catalogItemId,
                    ProductName = catalogItem.Name,
                    Quantity = quantity,
                    UnitPrice = catalogItem.Price
                });
            }

            await db.StringSetAsync($"cart:{userId}", JsonSerializer.Serialize(cart), TimeSpan.FromMinutes(2));

            return TypedResults.Ok();
        }
    }
}
