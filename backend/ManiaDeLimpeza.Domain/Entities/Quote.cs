using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;
        public int CompanyId { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public List<QuoteItem> QuoteItems { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(500)]
        public string PaymentConditions { get; set; } = string.Empty;

        public decimal? CashDiscount { get; set; }

        public bool IsForCompany(int companyId)
        {
            return CompanyId == companyId;
        }

        public void RecalculateTotals()
        {
            if (QuoteItems == null)
                QuoteItems = new List<QuoteItem>();

            int order = 1;
            foreach (var item in QuoteItems)
            {
                item.TotalPrice = item.GetCalculatedTotalPrice();
            }

            TotalPrice = GetCalculatedTotalPrice();
        }

        public decimal GetCalculatedTotalPrice()
        {
            return Math.Round(QuoteItems.Sum(i => i.TotalPrice), 2);
        }

        public void EnsureQuoteItemsOrder()
        {
            if (QuoteItems == null)
                return;
            int order = 1;
            foreach (var item in QuoteItems)
            {
                item.Order = order++;
            }
        }

    }
}
    