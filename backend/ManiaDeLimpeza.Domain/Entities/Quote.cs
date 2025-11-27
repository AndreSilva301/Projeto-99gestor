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

        public void RecalculateTotals()
        {
            if (QuoteItems == null)
                QuoteItems = new List<QuoteItem>();

            int order = 1;
            foreach (var item in QuoteItems)
            {
                item.TotalPrice = Math.Round((item.Quantity ?? 0m) * (item.UnitPrice ?? 0m), 2);
                item.Order = order++;
            }

            TotalPrice = Math.Round(QuoteItems.Sum(i => i.TotalPrice), 2);

            decimal finalPrice = TotalPrice;

            if (!string.IsNullOrWhiteSpace(PaymentConditions) &&
                (PaymentConditions.Contains("à vista", StringComparison.OrdinalIgnoreCase) ||
                 PaymentConditions.Contains("àvista", StringComparison.OrdinalIgnoreCase)))
            {
                if (CashDiscount.HasValue)
                    finalPrice -= CashDiscount.Value;
            }

            TotalPrice = Math.Round(QuoteItems.Sum(i => i.TotalPrice), 2);
        }

        public decimal GetFinalPrice()
        {
            decimal finalPrice = TotalPrice;

            bool isCashPayment =
                !string.IsNullOrWhiteSpace(PaymentConditions) &&
                (PaymentConditions.Contains("à vista", StringComparison.OrdinalIgnoreCase) ||
                 PaymentConditions.Contains("àvista", StringComparison.OrdinalIgnoreCase));

            if (isCashPayment && CashDiscount.HasValue)
                finalPrice -= CashDiscount.Value;

            return Math.Round(Math.Max(0m, finalPrice), 2);
        }
    }
}
    