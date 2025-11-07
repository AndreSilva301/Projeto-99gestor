using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        
        public Customer Customer { get; set; } = null!;

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

    }
}
    