using Identity.API.Infrastructure;
using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Extensions
{
    public static class Extensions
    {
        public static void AddAppServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            });

            builder.Services.AddAuthorization();

            builder.Services.AddIdentityApiEndpoints<User>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }
    }
}
