using BugStore.Application.Abstractions.Handlers.Customers;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Customers;

public class GetCustomerByIdHandler(ICustomerRepository customerRepository) : IGetCustomerByIdHandler
{
    public async Task<Response<Customer>> HandleAsync(GetCustomerByIdRequest req, CancellationToken cancellationToken = default)
    {
        var existingCustomerResult = await customerRepository.GetByIdAsync(req.CustomerId, cancellationToken);
        if (existingCustomerResult.Data is null)
            return new Response<Customer>(null, 404, ["Customer not found"]);

        return new Response<Customer>(existingCustomerResult.Data);
    }
}
