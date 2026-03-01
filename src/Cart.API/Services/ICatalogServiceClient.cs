using Cart.API.Models;

namespace Cart.API.Services
{
    public interface ICatalogServiceClient
    {
        Task<CatalogItemDto?> GetCatalogItemAsync(int id);
    }
}
