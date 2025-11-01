using BugStore.Application.Abstractions.Handlers.Customers;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Customers;

public class UpdateCustomerHandler(ICustomerRepository customerRepository) : IUpdateCustomerHandler
{
    public async Task<Response<Customer>> HandleAsync(UpdateCustomerRequest req, CancellationToken cancellationToken = default)
    {
        var existingCustomerResult = await customerRepository.GetByIdAsync(req.CustomerId, cancellationToken);

        if (existingCustomerResult.Data is null)
            return new Response<Customer>(null, 404, ["Customer not found"]);

        var (customer, errors) = Customer.Create(
            name: req.Name,
            email: req.Email,
            birthDate: req.BirthDate,
            phone: req.Phone);

        if (customer is null)
        {
            return new Response<Customer>(null, 400, [.. errors.Select(e => e.Message)]);
        }

        typeof(Customer).GetProperty(nameof(Customer.Id))!.SetValue(customer, req.CustomerId);

        var result = await customerRepository.UpdateAsync(customer, cancellationToken);

        return new Response<Customer>(result.Data);
    }
}
