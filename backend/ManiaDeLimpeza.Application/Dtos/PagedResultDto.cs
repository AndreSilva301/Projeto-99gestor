namespace ManiaDeLimpeza.Application.Dtos;
public class PagedResultDto<T>
{
    public int TotalCount { get; set; }
    public IEnumerable<T> Items { get; set; } = new List<T>();

    public PagedResultDto() { }

    public PagedResultDto(int totalCount, IEnumerable<T> items)
    {
        TotalCount = totalCount;
        Items = items;
    }
}
