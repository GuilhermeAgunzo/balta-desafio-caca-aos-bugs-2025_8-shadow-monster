using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Common;
using BugStore.Application.Requests.Orders;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Repositories;

public class OrderRepository(AppDbContext db) : IOrderRepository
{
    public async Task<Result<Order>> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!order.IsValid)
                return Result<Order>.Fail("INVALID_ENTITY: Order is not valid");

            order.Customer = null; // To avoid EF trying to insert/update the Customer entity
            foreach (var line in order.Lines)
            {
                line.Product = null; // To avoid EF trying to insert/update the Product entity
            }

            await db.Orders.AddAsync(order, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

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
            var order = await db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Lines)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

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
            var query = db.Orders
                .AsNoTracking()
                .Include(o => o.Lines)
                .FilterBy(request)
                .OrderByDescending(o => o.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

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
            var query = db.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Lines)
                .AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return PagedResult<Order>.Ok(items, totalCount, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            return PagedResult<Order>.Fail($"GENERIC: Unexpected error while paging orders: {ex.Message}");
        }
    }
}
