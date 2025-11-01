using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Common;
using BugStore.Application.Requests.Customers;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.Repositories;

public class FakeCustomerRepository(List<Customer> db) : ICustomerRepository
{
    public async Task<Result<Customer>> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!customer.IsValid)
                return Result<Customer>.Fail("INVALID_ENTITY: Customer is not valid");

            db.Add(customer);

            return Result<Customer>.Ok(customer);
        }
        catch
        {
            return Result<Customer>.Fail("Failed to add customer.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = db.FirstOrDefault(c => c.Id == id);

            if (customer is null)
                return Result<bool>.Fail("NOT_FOUND: Customer not found");

            db.Remove(customer);

            return Result<bool>.Ok(true);
        }
        catch
        {
            return Result<bool>.Fail("GENERIC: Failed to delete customer.");
        }
    }

    public async Task<Result<Customer>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = db.FirstOrDefault(c => c.Email == email);

            if (customer is null)
                return Result<Customer>.Fail("NOT_FOUND: Customer not found");

            return Result<Customer>.Ok(customer);
        }
        catch
        {
            return Result<Customer>.Fail("GENERIC: Failed to get customer");
        }
    }

    public async Task<Result<Customer>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = db.FirstOrDefault(c => c.Id == id);

            if (customer is null)
                return Result<Customer>.Fail("NOT_FOUND: Customer not found");

            return Result<Customer>.Ok(customer);
        }
        catch
        {
            return Result<Customer>.Fail("GENERIC: Failed to get customer");
        }
    }

    public async Task<PagedResult<Customer>> GetPagedAsync(GetCustomersRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = db
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(c => c.Name.Contains(request.Name));
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                query = query.Where(c => c.Email.Contains(request.Email));
            }
            if (!string.IsNullOrEmpty(request.Phone))
            {
                query = query.Where(c => c.Phone != null && c.Phone.Contains(request.Phone));
            }

            var count = query.Count();

            var customers = query
                .OrderBy(p => p.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();


            return PagedResult<Customer>.Ok(
                items: customers,
                totalCount: count,
                currentPage: request.PageNumber,
                pageSize: request.PageSize);
        }
        catch
        {
            return PagedResult<Customer>.Fail("GENERIC: Failed to get customer");
        }
    }

    public async Task<Result<Customer>> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!customer.IsValid)
                return Result<Customer>.Fail("INVALID_ENTITY: Customer is not valid");

            var existing = db.FirstOrDefault(c => c.Id == customer.Id);

            if (existing is null)
                return Result<Customer>.Fail("NOT_FOUND: Customer not found");

            db.Remove(existing);
            db.Add(customer);

            return Result<Customer>.Ok(customer);
        }
        catch
        {
            return Result<Customer>.Fail("GENERIC: Failed to delete customer.");
        }
    }
}
