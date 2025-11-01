using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Common;
using BugStore.Application.Requests.Customers;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public async Task<Result<Customer>> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!customer.IsValid)
                return Result<Customer>.Fail("INVALID_ENTITY: Customer is not valid");

            await db.Customers.AddAsync(customer, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return Result<Customer>.Ok(customer);
        }
        catch
        {
            // Generic catch for simplicity; in production code, consider logging the exception
            return Result<Customer>.Fail("Failed to add customer.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (customer is null)
                return Result<bool>.Fail("NOT_FOUND: Customer not found");

            db.Customers.Remove(customer);
            await db.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true);
        }
        catch
        {
            // Generic catch for simplicity; in production code, consider logging the exception
            return Result<bool>.Fail("GENERIC: Failed to delete customer.");
        }
    }

    public async Task<Result<Customer>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

            if (customer is null)
                return Result<Customer>.Fail("NOT_FOUND: Customer not found");

            return Result<Customer>.Ok(customer);
        }
        catch
        {
            // Generic catch for simplicity; in production code, consider logging the exception
            return Result<Customer>.Fail("GENERIC: Failed to get customer");
        }
    }

    public async Task<Result<Customer>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (customer is null)
                return Result<Customer>.Fail("NOT_FOUND: Customer not found");

            return Result<Customer>.Ok(customer);
        }
        catch
        {
            // Generic catch for simplicity; in production code, consider logging the exception
            return Result<Customer>.Fail("GENERIC: Failed to get customer");
        }
    }

    public async Task<PagedResult<Customer>> GetPagedAsync(GetCustomersRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = db.Customers
                .AsNoTracking()
                .FilterBy(request);

            var customers = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var count = await query.CountAsync(cancellationToken);

            return PagedResult<Customer>.Ok(
                items: customers,
                totalCount: count,
                currentPage: request.PageNumber,
                pageSize: request.PageSize);
        }
        catch
        {
            // Generic catch for simplicity; in production code, consider logging the exception
            return PagedResult<Customer>.Fail("GENERIC: Failed to get customer");
        }
    }

    public async Task<Result<Customer>> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!customer.IsValid)
                return Result<Customer>.Fail("INVALID_ENTITY: Customer is not valid");

            var existing = await db.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id, cancellationToken);

            if (existing is null)
                return Result<Customer>.Fail("NOT_FOUND: Customer not found");

            db.Entry(existing).CurrentValues.SetValues(customer);
            await db.SaveChangesAsync(cancellationToken);

            return Result<Customer>.Ok(customer);
        }
        catch
        {
            // Generic catch for simplicity; in production code, consider logging the exception
            return Result<Customer>.Fail("GENERIC: Failed to delete customer.");
        }
    }
}
