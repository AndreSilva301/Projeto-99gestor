using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class LineItem
    {
        public int Id { get; set; }

        [Required]
        public int QuoteId { get; set; }

        [ForeignKey(nameof(QuoteId))]
        public Quote Quote { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total { get; set; }
    }
}
