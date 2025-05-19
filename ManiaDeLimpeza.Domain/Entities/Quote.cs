using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        public List<LineItem> LineItems { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedBy { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal? CashDiscount { get; set; } // nullable to represent “no discount”
    }
}
