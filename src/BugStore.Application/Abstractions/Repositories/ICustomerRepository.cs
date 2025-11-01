using BugStore.Application.Common;
using BugStore.Application.Requests.Customers;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Repositories;

public interface ICustomerRepository
{
    Task<Result<Customer>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Customer>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PagedResult<Customer>> GetPagedAsync(GetCustomersRequest request, CancellationToken cancellationToken = default);

    Task<Result<Customer>> AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Result<Customer>> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
