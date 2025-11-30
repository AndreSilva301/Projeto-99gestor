using ManiaDeLimpeza.Domain.ExceptionErrors;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class QuoteItem
    {
        public int Id { get; set; }

        [Required]
        public int QuoteId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public decimal? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public int Order { get; set; }
       
        public Dictionary<string, string> CustomFields { get; set; } = new();

        public decimal GetCalculatedTotalPrice()
        {
            if(Quantity.HasValue && UnitPrice.HasValue)
            {
                return Math.Round(Quantity.Value * UnitPrice.Value, 2);
            }

            return TotalPrice;
        }
    }
}
