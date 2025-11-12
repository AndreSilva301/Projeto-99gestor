using ManiaDeLimpeza.Domain;

namespace ManiaDeLimpeza.Application.Dtos;
public class QuoteResponseDto : QuoteDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int UserId { get; set; } 
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public decimal TotalPrice { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentConditions { get; set; } = string.Empty;
    public decimal? CashDiscount { get; set; }
    public decimal FinalPrice { get; set; }
    public List<QuoteItemResponseDto> Items { get; set; } = new();
}
