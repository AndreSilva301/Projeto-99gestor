using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateQuoteDto : IBasicDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(500)]
    public string PaymentConditions { get; set; } = string.Empty;

    public decimal? CashDiscount { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<UpdateQuoteItemDto> Items { get; set; } = new();

    public Dictionary<string, string> CustomFields { get; set; } = new();

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (Id <= 0)
            errors.Add("Id da cotação é obrigatório.");

        if (CustomerId <= 0)
            errors.Add("Cliente é obrigatório.");

        if (TotalPrice <= 0)
            errors.Add("O valor total deve ser maior que zero.");

        if (!string.IsNullOrWhiteSpace(PaymentConditions) && PaymentConditions.Length > 500)
            errors.Add("As condições de pagamento não podem ter mais de 500 caracteres.");

        if (CustomFields != null)
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
            errors.AddRange(item.Validate());
        }

        return errors;
    }

    public bool IsValid() => Validate().Count == 0;

    public void EnsureQuoteItemsOrder()
    {
        if (Items == null)
            return;
        int order = 1;
        foreach (var item in Items)
        {
            item.Order = order++;
        }
    }
}