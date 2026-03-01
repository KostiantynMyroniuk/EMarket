using Cart.API.Models;

namespace Cart.API.Services
{
    public class CatalogServiceClient : ICatalogServiceClient
    {
        private HttpClient _httpClient;

        public CatalogServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CatalogItemDto?> GetCatalogItemAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/catalog/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CatalogItemDto>();
        }
    }
}
