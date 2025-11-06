using ManiaDeLimpeza.Domain.Interfaces;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateQuoteItemDto : IBasicDto
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

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

        return errors;
    }
    public bool IsValid() => Validate().Count == 0;
}

