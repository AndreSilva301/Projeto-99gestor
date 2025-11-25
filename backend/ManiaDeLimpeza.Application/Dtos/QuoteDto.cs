using ManiaDeLimpeza.Domain;

namespace ManiaDeLimpeza.Application.Dtos
{
    public class QuoteDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentConditions { get; set; } = string.Empty;
        public decimal? CashDiscount { get; set; }
        public List<QuoteItemDto> QuoteItems { get; set; } = new();
    }
}
