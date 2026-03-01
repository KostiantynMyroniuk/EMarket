using Identity.API.Extensions;
using Identity.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddAppServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapIdentityApi<User>();

app.Run();

