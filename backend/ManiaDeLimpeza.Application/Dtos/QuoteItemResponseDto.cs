namespace ManiaDeLimpeza.Application.Dtos;
public class QuoteItemResponseDto
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int Order { get; set; }
    public Dictionary<string, string> CustomFields { get; set; } = new();
}
