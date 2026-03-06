using Azure.Core;
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
            app.MapGet("/{userId}", GetCart).WithName("GetMyCart");
            app.MapPost("/", AddCartItem).WithName("AddItemToCart");

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

        public record AddCartItemRequest(string UserId, int CatalogItemId, int Quantity);

        public static async Task<Results<Ok, NotFound>> AddCartItem(
            AddCartItemRequest request,
            ICatalogServiceClient catalogClient,
            IConnectionMultiplexer redis)
        {
            var db = redis.GetDatabase();

            var catalogItem = await catalogClient.GetCatalogItemAsync(request.CatalogItemId);

            if (catalogItem == null)
            {
                return TypedResults.NotFound();
            }

            var cartData = await db.StringGetAsync($"cart:{request.UserId}");

            var cart = string.IsNullOrEmpty(cartData) 
                ? new CustomerCart { CustomerId = request.UserId } 
                : JsonSerializer.Deserialize<CustomerCart>(cartData);

            var item = cart.Items.FirstOrDefault(c => c.ProductId == request.CatalogItemId);

            if (item is not null)
            {
                item.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = request.CatalogItemId,
                    ProductName = catalogItem.Name,
                    Quantity = request.Quantity,
                    UnitPrice = catalogItem.Price,
                });
            }

            await db.StringSetAsync($"cart:{request.UserId}", JsonSerializer.Serialize(cart), TimeSpan.FromMinutes(2));

            return TypedResults.Ok();
        }
    }
}
