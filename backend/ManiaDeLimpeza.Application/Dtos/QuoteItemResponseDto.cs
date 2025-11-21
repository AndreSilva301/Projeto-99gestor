namespace ManiaDeLimpeza.Application.Dtos;
public class QuoteItemResponseDto : QuoteItemDto
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public decimal TotalPrice { get; set; }
    public int Order { get; set; }
}
