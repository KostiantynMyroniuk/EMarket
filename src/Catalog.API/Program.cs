using Catalog.API.Apis;
using Catalog.API.Extensions;
using Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrations();
    app.MapOpenApi();
}

app.MapCatalogApi();

app.Run();
