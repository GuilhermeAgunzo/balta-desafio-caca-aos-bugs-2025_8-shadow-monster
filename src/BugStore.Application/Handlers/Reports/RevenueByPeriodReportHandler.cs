using BugStore.Application.Abstractions.Handlers.Reports;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Reports;
using BugStore.Application.Responses;
using BugStore.Domain.Reports;

namespace BugStore.Application.Handlers.Reports;

public class RevenueByPeriodReportHandler(IReportRepository reportRepository) : IRevenueByPeriodReportHandler
{
    public async Task<Response<List<RevenueByPeriod>>> HandleAsync(RevenueByPeriodReportRequest req, CancellationToken cancellationToken = default)
    {
        var result = await reportRepository.GetRevenueByPeriodReportAsync(req.StartDate, req.EndDate, cancellationToken);
        return new Response<List<RevenueByPeriod>>(result);
    }
}
