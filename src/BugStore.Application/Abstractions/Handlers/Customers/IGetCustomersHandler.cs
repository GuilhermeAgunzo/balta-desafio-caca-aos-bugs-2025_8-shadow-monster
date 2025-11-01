using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Customers;

public interface IGetCustomersHandler
{
    Task<PagedResponse<IEnumerable<Customer>>> HandleAsync(GetCustomersRequest req, CancellationToken cancellationToken = default);
}
