namespace ManiaDeLimpeza.Application.Dtos
{
    public class PagedResult<T>
    {
        public int TotalCount { get; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = new();

        public int TotalPages => (int)Math.Ceiling(TotalItems / (decimal)PageSize);

        public PagedResult()
        {
            Items = new List<T>();
        }

        public PagedResult(int totalCount, IEnumerable<T> items, int page, int pageSize)
        {
            TotalCount = totalCount;
            Items = items.ToList();
            Page = page;
            PageSize = pageSize;
        }
    }
}
