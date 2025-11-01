using BugStore.Application.Abstractions.Handlers.Customers;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Customers;

public class CreateCustomerHandler(ICustomerRepository customerRepository) : ICreateCustomerHandler
{
    public async Task<Response<Customer>> HandleAsync(CreateCustomerRequest req, CancellationToken cancellationToken = default)
    {
        var result = await customerRepository.GetByEmailAsync(req.Email, cancellationToken);

        if (result.Data is not null)
            return new Response<Customer>(null, 400, ["Customer already exists"]);

        var (customer, errors) = Customer.Create(
            name: req.Name,
            email: req.Email,
            birthDate: req.BirthDate,
            phone: req.Phone);

        if (customer is null)
        {
            return new Response<Customer>(
                data: null,
                code: 400,
                messages: [.. errors.Select(e => e.Message)]);
        }

        var addResult = await customerRepository.AddAsync(customer, cancellationToken);

        return new Response<Customer>(addResult.Data);
    }
}