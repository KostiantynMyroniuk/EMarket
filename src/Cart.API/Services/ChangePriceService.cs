using Cart.API.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Cart.API.Services
{
    public class ChangePriceService
    {
        private IConnectionMultiplexer _redis;

        public ChangePriceService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task ChangePrice(int productId, decimal newPrice)
        {
            var db = _redis.GetDatabase();
            var setKey = $"product:{productId}:carts";
            var cartKeys = await db.SetMembersAsync(setKey);

            var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            await Parallel.ForEachAsync(cartKeys, options, async (cartKey, token) =>
            {
                var cartData = await db.StringGetAsync(cartKey.ToString());

                if (cartData.IsNullOrEmpty)
                {
                    await db.SetRemoveAsync(setKey, cartKey);
                }

                var cart = JsonSerializer.Deserialize<CustomerCart>(cartData);

                var items = cart.Items.Where(i => i.ProductId == productId).ToList();

                if (items.Any())
                {
                    items.ForEach(x => x.UnitPrice = newPrice);
                    await db.StringSetAsync(cartKey.ToString(), JsonSerializer.Serialize(cart), TimeSpan.FromMinutes(5));
                }

            });

        }
    }
}
