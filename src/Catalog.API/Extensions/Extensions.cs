using Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Extensions
{
    public static class Extensions
    {
        public static void AddAppServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            });
        }
    }
}
