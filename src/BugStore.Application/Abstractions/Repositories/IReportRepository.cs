using BugStore.Domain.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugStore.Application.Abstractions.Repositories;

public interface IReportRepository
{
    Task<List<CustomerOrderSummary>> GetCustomerOrderSummaryReportAsync(CancellationToken cancellationToken = default);
    Task<List<RevenueByPeriod>> GetRevenueByPeriodReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
