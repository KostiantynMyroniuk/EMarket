namespace Catalog.API.Models
{
    public record PaginatedRequest(int PageNumber = 1, int PageSize = 10);
}
