using BugStore.Application.Abstractions.Handlers.Reports;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Responses;
using BugStore.Domain.Reports;

namespace BugStore.Application.Handlers.Reports;

public class CustomerOrderSummaryReportHandler(IReportRepository reportRepository) : ICustomerOrderSummaryReportHandler
{
    public async Task<Response<List<CustomerOrderSummary>>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var result = await reportRepository.GetCustomerOrderSummaryReportAsync(cancellationToken);
        return new Response<List<CustomerOrderSummary>>(result);
    }
}
