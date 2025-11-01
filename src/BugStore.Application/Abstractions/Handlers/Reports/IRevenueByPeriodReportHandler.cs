using BugStore.Application.Requests.Reports;
using BugStore.Application.Responses;
using BugStore.Domain.Reports;

namespace BugStore.Application.Abstractions.Handlers.Reports;

public interface IRevenueByPeriodReportHandler
{
    Task<Response<List<RevenueByPeriod>>> HandleAsync(RevenueByPeriodReportRequest req, CancellationToken cancellationToken = default);
}
