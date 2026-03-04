using Identity.API.Extensions;
using Identity.API.Infrastructure;
using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddAppServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    context.Database.Migrate();

    app.MapOpenApi();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapIdentityApi<User>();

app.Run();

