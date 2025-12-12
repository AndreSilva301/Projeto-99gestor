using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Application.Services;
public class DashboardService : IDashboardService, IScopedDependency
{
    private readonly ApplicationDbContext _db;

    public DashboardService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(int companyId)
    {
        var now = DateTime.UtcNow;
        var firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
        var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);
        var lastDayLastMonth = firstDayThisMonth.AddDays(-1);

        // Customers
        var totalCustomers = await _db.Customers
            .Where(c => c.CompanyId == companyId)
            .CountAsync();

        var customersThisMonth = await _db.Customers
            .Where(c => c.CompanyId == companyId && c.CreatedDate >= firstDayThisMonth)
            .CountAsync();

        var customersLastMonth = await _db.Customers
            .Where(c => c.CompanyId == companyId &&
                        c.CreatedDate >= firstDayLastMonth &&
                        c.CreatedDate <= lastDayLastMonth)
            .CountAsync();

        // Quotes
        var totalQuotes = await _db.Quotes
            .Where(q => q.CompanyId == companyId)
            .CountAsync();

        var quotesThisMonth = await _db.Quotes
            .Where(q => q.CompanyId == companyId && q.CreatedAt >= firstDayThisMonth)
            .CountAsync();

        var quotesLastMonth = await _db.Quotes
            .Where(q => q.CompanyId == companyId &&
                        q.CreatedAt >= firstDayLastMonth &&
                        q.CreatedAt <= lastDayLastMonth)
            .CountAsync();

        // Revenue (sum of TotalPrice)
        var revenueThisMonth = await _db.Quotes
            .Where(q => q.CompanyId == companyId && q.CreatedAt >= firstDayThisMonth)
            .SumAsync(q => (decimal?)q.TotalPrice) ?? 0;

        var revenueLastMonth = await _db.Quotes
            .Where(q => q.CompanyId == companyId &&
                        q.CreatedAt >= firstDayLastMonth &&
                        q.CreatedAt <= lastDayLastMonth)
            .SumAsync(q => (decimal?)q.TotalPrice) ?? 0;

        // Employees
        var totalEmployees = await _db.Users
            .Where(e => e.CompanyId == companyId)
            .CountAsync();

        return new DashboardStatsDto
        {
            TotalCustomers = totalCustomers,
            CustomersThisMonth = customersThisMonth,
            CustomersLastMonth = customersLastMonth,
            TotalQuotes = totalQuotes,
            QuotesThisMonth = quotesThisMonth,
            QuotesLastMonth = quotesLastMonth,
            RevenueThisMonth = revenueThisMonth,
            RevenueLastMonth = revenueLastMonth,
            TotalEmployees = totalEmployees
        };
    }
}
