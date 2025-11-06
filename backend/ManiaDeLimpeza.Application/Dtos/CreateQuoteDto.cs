using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class CreateQuoteDto : IBasicDto
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

    public Dictionary<string, string> CustomFields { get; set; } = new();

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Descrição é obrigatória.");
        else if (Description.Length > 200)
            errors.Add("Descrição não pode ter mais de 200 caracteres.");

        if (Quantity <= 0)
            errors.Add("Quantidade deve ser maior que zero.");

        if (UnitPrice <= 0)
            errors.Add("Valor unitário deve ser maior que zero.");

        if (CustomFields != null && CustomFields.Count > 0)
        {
            foreach (var kvp in CustomFields)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key))
                    errors.Add("Chaves do campo customizado não podem ser vazias.");
                else if (kvp.Key.Length > 50)
                    errors.Add("Chaves do campo customizado não podem ter mais de 50 caracteres.");

                if (string.IsNullOrWhiteSpace(kvp.Value))
                    errors.Add("Valores do campo customizado não podem ser vazios.");
                else if (kvp.Value.Length > 200)
                    errors.Add("Valores do campo customizado não podem ter mais de 200 caracteres.");
            }
        }

        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}

