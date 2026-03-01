using Cart.API.Services;
using StackExchange.Redis;

namespace Cart.API.Extensions
{
    public static class Extensions
    {
        public static void AddAppConfiguration(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(s => 
                ConnectionMultiplexer.Connect(builder.Configuration["ConnectionStrings:Redis"]));

            builder.Services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://catalog-api:8080");
            });
        }
    }
}
