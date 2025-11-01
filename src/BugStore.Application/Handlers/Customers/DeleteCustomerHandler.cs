using BugStore.Application.Abstractions.Handlers.Customers;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;

namespace BugStore.Application.Handlers.Customers;

public class DeleteCustomerHandler(ICustomerRepository customerRepository) : IDeleteCustomerHandler
{
    public async Task<Response<bool>> HandleAsync(DeleteCustomerRequest req, CancellationToken cancellationToken = default)
    {
        var existingCustomerResult = await customerRepository.GetByIdAsync(req.CustomerId, cancellationToken);

        if (existingCustomerResult.Data is null)
            return new Response<bool>(false, 404, ["Customer not found"]);

        var deleteResult = await customerRepository.DeleteAsync(req.CustomerId, cancellationToken);

        if (!deleteResult.Success)
            return new Response<bool>(false, 500);

        return new Response<bool>(true);
    }
}
