using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;

namespace BugStore.Application.Abstractions.Handlers.Customers;

public interface IDeleteCustomerHandler
{
    Task<Response<bool>> HandleAsync(DeleteCustomerRequest req, CancellationToken cancellationToken = default);
}
