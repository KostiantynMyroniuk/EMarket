using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public string? Description { get; set; } = default!;

        public decimal Price { get; set; }

        public int? DiscountValue { get; set; }

        public bool HasDiscount => DiscountValue > 0;

        public decimal? DiscountPrice => Price * (100 - DiscountValue) / 100;

        public string? PictureFileName { get; set; } = default!;

        public int AvailableStock { get; set; }

    }
}
