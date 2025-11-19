namespace ManiaDeLimpeza.Application.Dtos;
public class QuoteResponseDto : QuoteDto
{
    public string CustomerName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public decimal FinalPrice { get; set; }
    public List<QuoteItemResponseDto> Items { get; set; } = new();
}
