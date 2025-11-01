using BugStore.Application.Common;
using BugStore.Application.Requests.Orders;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Order>> GetPagedByCustomerAsync(Guid customerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<Order>> GetPagedAsync(GetOrdersRequest request, CancellationToken cancellationToken = default);
    Task<Result<Order>> AddAsync(Order order, CancellationToken cancellationToken = default);
}
