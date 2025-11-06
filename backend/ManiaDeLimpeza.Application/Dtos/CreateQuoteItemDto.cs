using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class CreateQuoteItemDto : IBasicDto
{
    [Required]
    public int QuoteId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (QuoteId <= 0)
            errors.Add("O identificador do orçamento deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("A descrição é obrigatória.");

        if (Quantity <= 0)
            errors.Add("A quantidade deve ser maior que zero.");

        if (UnitPrice <= 0)
            errors.Add("O preço unitário deve ser maior que zero.");

        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}
