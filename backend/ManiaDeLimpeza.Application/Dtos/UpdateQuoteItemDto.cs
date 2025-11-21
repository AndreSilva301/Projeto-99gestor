using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateQuoteItemDto : IBasicDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int QuoteId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public int Order { get; set; }

    public Dictionary<string, string> CustomFields { get; set; } = new();

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("O identificador do item (Id) deve ser maior que zero.");

        if (QuoteId <= 0)
            errors.Add("O identificador do orçamento (QuoteId) deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("A descrição é obrigatória.");
        else if (Description.Length > 200)
            errors.Add("A descrição não pode ter mais de 200 caracteres.");

        var hasQtyPrice = Quantity.HasValue && UnitPrice.HasValue;
        var hasManualTotal = TotalPrice.HasValue && TotalPrice.Value > 0;

        if (!hasQtyPrice && !hasManualTotal)
        {
            errors.Add("Informe (Quantidade e Preço Unitário) ou apenas o Preço Total.");
        }

        if (Quantity.HasValue && Quantity <= 0)
            errors.Add("A quantidade deve ser maior que zero.");

        if (UnitPrice.HasValue && UnitPrice <= 0)
            errors.Add("O preço unitário deve ser maior que zero.");

        if (TotalPrice.HasValue && TotalPrice <= 0)
            errors.Add("O preço total deve ser maior que zero.");

        foreach (var kvp in CustomFields)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key))
                errors.Add("Chaves do campo customizado não podem ser vazias.");
            else if (kvp.Key.Length > 50)
                errors.Add("Chaves do campo customizado não podem ter mais de 50 caracteres.");

            if (kvp.Value != null && kvp.Value.Length > 200)
                errors.Add("Valores do campo customizado não podem ter mais de 200 caracteres.");
        }

        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}