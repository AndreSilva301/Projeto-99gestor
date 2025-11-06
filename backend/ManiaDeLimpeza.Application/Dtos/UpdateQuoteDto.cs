namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateQuoteDto : QuoteDto
{
    public int id { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<LineItemDto> Items { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}
