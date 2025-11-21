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

        public void CalcularPrecoTotal()
        {
            TotalPrice = Math.Round(Quantity!.Value * UnitPrice!.Value, 2);
        }

        public void AplicarRegrasDePreco()
        {
            bool hasQty = Quantity.HasValue && Quantity.Value > 0;
            bool hasUnitPrice = UnitPrice.HasValue && UnitPrice.Value > 0;
            bool hasTotalPrice = TotalPrice != 0;

            if (hasQty && hasUnitPrice)
            {
                CalcularPrecoTotal(); 
                return;
            }

            if (hasQty && !hasUnitPrice)
                throw new DomainException("Preço unitário é obrigatório quando a quantidade é informada.");

            if (hasUnitPrice && !hasQty)
                throw new DomainException("Quantidade é obrigatória quando o preço unitário é informado.");

            if (!hasQty && !hasUnitPrice)
            {
                if (!hasTotalPrice)
                    throw new DomainException("É necessário informar total, ou quantidade + preço unitário.");
            }
        }
    }
}
