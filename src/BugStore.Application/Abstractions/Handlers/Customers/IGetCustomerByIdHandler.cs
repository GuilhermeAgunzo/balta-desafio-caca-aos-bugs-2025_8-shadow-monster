using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Customers;

public interface IGetCustomerByIdHandler
{
    Task<Response<Customer>> HandleAsync(GetCustomerByIdRequest req, CancellationToken cancellationToken = default);
}
