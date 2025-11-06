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

    [Range(0.01, double.MaxValue)]
    public int Quantity { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

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

        if (Quantity <= 0)
            errors.Add("A quantidade deve ser maior que zero.");

        if (UnitPrice <= 0)
            errors.Add("O preço unitário deve ser maior que zero.");


        foreach (var field in CustomFields)
        {
            if (string.IsNullOrWhiteSpace(field.Key))
                errors.Add("Chaves do campo customizado não podem ser vazias.");

            if (string.IsNullOrWhiteSpace(field.Value))
                errors.Add("Valores do campo customizado não podem ser vazios.");

            if (field.Key.Length > 50)
                errors.Add("Chaves do campo customizado não podem ter mais de 50 caracteres.");

            if (field.Value.Length > 200)
                errors.Add("Valores do campo customizado não podem ter mais de 200 caracteres.");
        }

        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}

