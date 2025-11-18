namespace ManiaDeLimpeza.Application.Dtos
{
    public class QuoteFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? ClientName { get; set; }      
        public string? ClientPhone { get; set; }

        public DateTime? CreatedAtStart { get; set; }
        public DateTime? CreatedAtEnd { get; set; }

        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = false;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
