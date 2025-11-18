using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class CreateQuoteDto : IBasicDto
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(500)]
    public string PaymentConditions { get; set; } = string.Empty;

    public decimal? CashDiscount { get; set; }

    public List<QuoteItemDto> Items { get; set; } = new();

    public Dictionary<string, string> CustomFields { get; set; } = new();

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (CustomerId <= 0)
            errors.Add("Cliente é obrigatório.");

        if (UserId <= 0)
            errors.Add("Usuário é obrigatório.");

        if (TotalPrice < 0)
            errors.Add("O valor total deve ser maior que zero.");

        if (!string.IsNullOrWhiteSpace(PaymentConditions) && PaymentConditions.Length > 500)
            errors.Add("As condições de pagamento não podem ter mais de 500 caracteres.");

        if (CustomFields != null && CustomFields.Count > 0)
        {
            foreach (var kvp in CustomFields)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key))
                    errors.Add("Chaves do campo customizado não podem ser vazias.");
                else if (kvp.Key.Length > 50)
                    errors.Add("Chaves do campo customizado não podem ter mais de 50 caracteres.");

                if (kvp.Value != null && kvp.Value.Length > 200)
                    errors.Add("Valores do campo customizado não podem ter mais de 200 caracteres.");
            }
        }

        if (Items == null || Items.Count == 0)
            errors.Add("A cotação deve conter pelo menos um item.");

        foreach (var item in Items)
        {
            var itemErrors = item.Validate();
            if (itemErrors.Count > 0)
                errors.AddRange(itemErrors);
        }

        return errors;
    }

    public bool IsValid() => Validate().Count == 0;
}