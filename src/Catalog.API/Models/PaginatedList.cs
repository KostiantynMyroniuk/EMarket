namespace Catalog.API.Models
{
    public class PaginatedList<T>(int pageNumber, int pageSize, int count, ICollection<T> data) where T : class
    {
        public int PageNumber { get; } = pageNumber;

        public int PageSize { get; } = pageSize;

        public int Count { get; } = count;

        public ICollection<T> Data { get; } = data;
    }
}
