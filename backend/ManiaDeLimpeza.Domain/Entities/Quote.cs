using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class Quote // Orçamento
    {
        public int Id { get; set; }

        [Required]
        public int CostumerId { get; set; }
        [ForeignKey(nameof(CostumerId))]
        public Customer Customer { get; set; } = null!;

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public List<QuoteItem> QuoteItems { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal TotalPrice { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentConditions { get; set; } = string.Empty;

        public decimal? CashDiscount { get; set; } // nullable to represent “no discount”

    }
}
