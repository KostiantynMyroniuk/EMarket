using Catalog.API.Infrastructure;
using Catalog.API.Models;
using Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Apis
{
    public static class CatalogApi
    {
        public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
        {
            app.MapGet("/{id:int}", GetItemById).WithName("GetItem");
            app.MapGet("/", GetAllItems).WithName("GetAllItems");
            app.MapPost("/", CreateItem).WithName("CreateItem");
            app.MapPut("/{id:int}", UpdateItem).WithName("UpdateItem");
            app.MapDelete("/{id:int}", DeleteItem).WithName("DeleteItem");

            return app;
        }

        public static async Task<Results<Ok<CatalogItem>, BadRequest<string>, NotFound>> GetItemById(
            int id,
            ApplicationDbContext context)
        {
            if (id <= 0)
            {
                return TypedResults.BadRequest("Id is not valid.");
            }

            var item = await context.CatalogItems.FindAsync(id);

            if (item is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(item);
        }

        public static async Task<Ok<PaginatedList<CatalogItem>>> GetAllItems(
            [AsParameters] PaginatedRequest request,
            ApplicationDbContext context)
        {
            var pageSize = request.PageSize;
            var pageNumber = request.PageNumber;

            var count = await context.CatalogItems.CountAsync();

            var items = await context.CatalogItems
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return TypedResults.Ok(new PaginatedList<CatalogItem>(pageNumber, pageSize, count, items));
        }
        
        public static async Task<Created> CreateItem(
            CatalogItem item,
            ApplicationDbContext context)
        {
            var catalogItem = new CatalogItem(item.Name)
            {
                Description = item.Description,
                Price = item.Price,
                PictureFileName = item.PictureFileName,
                AvailableStock = item.AvailableStock 
            };

            context.CatalogItems.Add(catalogItem);
            await context.SaveChangesAsync();

            return TypedResults.Created($"/api/catalog/{catalogItem.Id}");
        }

        public static async Task<Results<NoContent, NotFound>> UpdateItem(
            int id,
            CatalogItemDto item,
            IPublishEndpoint publishEndpoint,
            ApplicationDbContext context)
        {
            var catalogItem = await context.CatalogItems.FindAsync(id);

            if (catalogItem is null)
            {
                return TypedResults.NotFound();
            }

            if (catalogItem.Price != item.Price)
            {
                await publishEndpoint.Publish(new ProductPriceChangedIntegrationEvent(id, item.Price));
            }

            context.Entry(catalogItem!).CurrentValues.SetValues(item);

            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteItem(
            int id,
            ApplicationDbContext context)
        {
            var catalogItem = await context.CatalogItems.FindAsync(id);

            if (catalogItem is null)
            {
                return TypedResults.NotFound();
            }

            context.CatalogItems.Remove(catalogItem);
            await context.SaveChangesAsync();

            return TypedResults.NoContent();
        }
    }
}
