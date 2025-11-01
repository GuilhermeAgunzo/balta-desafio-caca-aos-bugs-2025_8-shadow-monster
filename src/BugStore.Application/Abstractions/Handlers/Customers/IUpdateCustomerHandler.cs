using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Customers;

public interface IUpdateCustomerHandler
{
    Task<Response<Customer>> HandleAsync(UpdateCustomerRequest req, CancellationToken cancellationToken = default);
}
