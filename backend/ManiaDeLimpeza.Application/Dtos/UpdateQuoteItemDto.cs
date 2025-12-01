using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateQuoteItemDto : QuoteItemDto
{
    public int Id { get; set; }

    public int QuoteId { get; set; }

    public int Order { get; set; }

    public new virtual List<string> Validate()
    {
        var errors = base.Validate();

        if (Id <= 0)
            errors.Add("O identificador do item (Id) deve ser maior que zero.");

        if (QuoteId <= 0)
            errors.Add("O identificador do orçamento (QuoteId) deve ser maior que zero.");

        return errors;
    }

}