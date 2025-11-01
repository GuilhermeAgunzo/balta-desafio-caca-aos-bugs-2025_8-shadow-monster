using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Common;
using BugStore.Application.Requests.Orders;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.Repositories;

public class FakeOrderRepository(List<Order> db) : IOrderRepository
{
    public async Task<Result<Order>> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!order.IsValid)
                return Result<Order>.Fail("INVALID_ENTITY: Order is not valid");

            db.Add(order);

            return Result<Order>.Ok(order);
        }
        catch (Exception ex)
        {
            return Result<Order>.Fail($"GENERIC: Unexpected error while adding order - {ex.Message}");
        }
    }

    public async Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = db.FirstOrDefault(o => o.Id == id);

            return order is not null
                     ? Result<Order>.Ok(order)
               : Result<Order>.Fail("NOT_FOUND: Order not found");
        }
        catch (Exception ex)
        {
            return Result<Order>.Fail($"GENERIC: Unexpected error while retrieving order: {ex.Message}");
        }
    }

    public async Task<PagedResult<Order>> GetPagedAsync(GetOrdersRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = db.AsQueryable();

            // Apply filters from request
            if (!string.IsNullOrWhiteSpace(request.CustomerName))
                query = query.Where(o => o.Customer.Name.Contains(request.CustomerName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
                query = query.Where(o => o.Customer.Email.Contains(request.CustomerEmail, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(request.CustomerPhone))
                query = query.Where(o => o.Customer.Phone.Contains(request.CustomerPhone, StringComparison.OrdinalIgnoreCase));
            if (request.CreatedAtStart.HasValue)
                query = query.Where(o => o.CreatedAt >= request.CreatedAtStart.Value);
            if (request.CreatedAtEnd.HasValue)
                query = query.Where(o => o.CreatedAt <= request.CreatedAtEnd.Value);
            if (request.UpdatedAtStart.HasValue)
                query = query.Where(o => o.UpdatedAt >= request.UpdatedAtStart.Value);
            if (request.UpdatedAtEnd.HasValue)
                query = query.Where(o => o.UpdatedAt <= request.UpdatedAtEnd.Value);

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return PagedResult<Order>.Ok(items, totalCount, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return PagedResult<Order>.Fail($"GENERIC: Unexpected error while paging orders: {ex.Message}");
        }
    }

    public async Task<PagedResult<Order>> GetPagedByCustomerAsync(Guid customerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var totalCount = db
            .Where(o => o.CustomerId == customerId)
            .Count();

            var items = db
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return PagedResult<Order>.Ok(items, totalCount, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            return PagedResult<Order>.Fail($"GENERIC: Unexpected error while paging orders: {ex.Message}");
        }
    }
}
