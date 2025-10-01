using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos
{
    public class QuoteFilterDto
    {
        public string? ClientName { get; set; }
        public string? ClientPhone { get; set; }
        public DateTime? CreatedAtStart { get; set; }
        public DateTime? CreatedAtEnd { get; set; }

        public string? SortBy { get; set; } = "CreatedAt"; // or "ClientName"
        public bool SortDescending { get; set; } = false;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
