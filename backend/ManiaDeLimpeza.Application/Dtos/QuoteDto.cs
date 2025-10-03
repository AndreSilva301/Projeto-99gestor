using ManiaDeLimpeza.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos
{
    public class QuoteDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentConditions { get; set; } = string.Empty;
        public decimal? CashDiscount { get; set; }
        public List<LineItemDto> LineItems { get; set; } = new();
    }

}
