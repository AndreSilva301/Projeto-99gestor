using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class QuoteItemDto : IBasicDto
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal Quantity { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }

    public virtual List<string> Validate()
    {
        var errors = new List<string>();

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
