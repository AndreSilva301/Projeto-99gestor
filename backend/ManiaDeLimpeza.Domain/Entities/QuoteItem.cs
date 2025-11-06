using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class QuoteItem
    {
        public int Id { get; set; }

        [Required]
        public int QuoteId { get; set; }
        [ForeignKey(nameof(QuoteId))]

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public string ExtraFields { get; set; } = string.Empty;
    }
}
