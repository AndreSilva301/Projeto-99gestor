using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManiaDeLimpeza.Domain.Entities
{
    public class QuoteItem
    {
        public int Id { get; set; }

        [Required]
        public int QuoteId { get; set; }
        
        public Quote Quote { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public int Order { get; set; }
       
        public Dictionary<string, string> CustomFields { get; set; } = new();
    }
}
