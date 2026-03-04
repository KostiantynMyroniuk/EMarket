namespace Cart.API.Models
{
    public class CatalogItemDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = default!;

        public decimal Price { get; set; }
    }
}
