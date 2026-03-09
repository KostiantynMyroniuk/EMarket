using Identity.API.Infrastructure;
using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Extensions
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

            builder.Services.AddAuthorization();

            builder.Services.AddIdentityApiEndpoints<User>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
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
