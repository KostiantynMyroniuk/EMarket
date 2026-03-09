using Cart.API.Consumers;
using Cart.API.Services;
using MassTransit;
using StackExchange.Redis;

namespace Cart.API.Extensions
{
    public static class Extensions
    {
        public static void AddAppConfiguration(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));

            builder.Services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://catalog.api:8080");
            });

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<ProductPriceChangedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration["RabbitMQ:Host"]);

                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddScoped<ChangePriceService>();
        }
    }
}
