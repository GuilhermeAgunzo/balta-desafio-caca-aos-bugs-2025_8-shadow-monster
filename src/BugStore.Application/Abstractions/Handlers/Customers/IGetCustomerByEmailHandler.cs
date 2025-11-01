using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Customers;

public interface IGetCustomerByEmailHandler
{
    Task<Response<Customer>> HandleAsync(GetCustomerByEmailRequest req, CancellationToken cancellationToken = default);
}
