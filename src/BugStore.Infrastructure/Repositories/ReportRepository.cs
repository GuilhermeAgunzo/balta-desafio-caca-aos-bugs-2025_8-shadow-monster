using BugStore.Application.Abstractions.Repositories;
using BugStore.Domain.Reports;
using BugStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Repositories;

public class ReportRepository(AppDbContext db) : IReportRepository
{
    public async Task<List<CustomerOrderSummary>> GetCustomerOrderSummaryReportAsync(CancellationToken cancellationToken = default)
    {
        var report = await db.CustomerOrderSummary
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return report;
    }

    public async Task<List<RevenueByPeriod>> GetRevenueByPeriodReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var report = await db.RevenueByPeriod.FromSqlInterpolated($@"
            SELECT
                strftime('%Y', o.CreatedAt) AS Year,
                strftime('%m', o.CreatedAt) AS Month,
                COUNT(o.Id) AS TotalOrders,
                SUM(ol.Total) AS TotalRevenue
            FROM Orders o
            JOIN OrderLines ol ON o.Id = ol.OrderId
            WHERE DATE(o.CreatedAt) BETWEEN DATE({startDate}) AND DATE({endDate})
            GROUP BY Year, Month
            ORDER BY Year, Month
        ").ToListAsync(cancellationToken);

        return report;
    }
}
