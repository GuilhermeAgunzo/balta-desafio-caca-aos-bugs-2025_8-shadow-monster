using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Customers;

public interface ICreateCustomerHandler
{
    Task<Response<Customer>> HandleAsync(CreateCustomerRequest req, CancellationToken cancellationToken = default);
}
