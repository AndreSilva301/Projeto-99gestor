namespace ManiaDeLimpeza.Application.Dtos;
public class DashboardStatsDto
{
    public int TotalCustomers { get; set; }
    public int CustomersThisMonth { get; set; }
    public int CustomersLastMonth { get; set; }

    public int TotalQuotes { get; set; }
    public int QuotesThisMonth { get; set; }
    public int QuotesLastMonth { get; set; }

    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }

    public int TotalEmployees { get; set; }
}
