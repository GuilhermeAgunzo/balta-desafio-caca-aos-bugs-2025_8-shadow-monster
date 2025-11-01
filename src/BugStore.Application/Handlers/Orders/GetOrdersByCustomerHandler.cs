using BugStore.Application.Abstractions.Handlers.Orders;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Orders;

public class GetOrdersByCustomerHandler(IOrderRepository orderRepository) : IGetOrdersByCustomerHandler
{
    public async Task<Response<IEnumerable<Order>>> HandleAsync(GetOrdersByCustomerRequest req, CancellationToken cancellationToken = default)
    {
        var searchResult = await orderRepository.GetPagedByCustomerAsync(req.CustomerId, req.PageNumber, req.PageSize, cancellationToken);

        return searchResult.Success
            ? new Response<IEnumerable<Order>>(searchResult.Items)
            : new Response<IEnumerable<Order>>(null, 400, [searchResult.ErrorMessage ?? "An error occurred while retrieving orders."]);
    }
}
