using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class QuoteItemDto : IBasicDto
{
    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public decimal? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public Dictionary<string, string> CustomFields { get; set; } = new();

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("A descrição é obrigatória.");
        else if (Description.Length > 200)
            errors.Add("A descrição não pode ter mais de 200 caracteres.");

        if (Quantity.HasValue && Quantity.Value < 0)
            errors.Add("A quantidade não pode ser menor que zero.");

        if (UnitPrice.HasValue && UnitPrice.Value < 0)
            errors.Add("O preço unitário não pode ser menor que zero.");

        foreach (var kvp in CustomFields)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key))
                errors.Add("As chaves do campo customizado não podem ser vazias.");
            else if (kvp.Key.Length > 50)
                errors.Add("As chaves do campo customizado não podem ter mais de 50 caracteres.");

            if (kvp.Value != null && kvp.Value.Length > 200)
                errors.Add("Os valores do campo customizado não podem ter mais de 200 caracteres.");
        }

        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}