using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models
{
    public class CatalogItemDto
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? PictureFileName { get; set; }

        public int AvailableStock { get; set; }
    }
}
