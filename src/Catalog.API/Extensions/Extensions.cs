using Catalog.API.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Extensions
{
    public static class Extensions
    {
        public static void AddAppServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration["RabbitMQ:Host"]);
                });
            });
        }


        public static WebApplication UseMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();
            
            return app;
        }
    }
}
