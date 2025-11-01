using BugStore.Application.Responses;
using BugStore.Domain.Reports;

namespace BugStore.Application.Abstractions.Handlers.Reports;

public interface ICustomerOrderSummaryReportHandler
{
    Task<Response<List<CustomerOrderSummary>>> HandleAsync(CancellationToken cancellationToken = default);
}
