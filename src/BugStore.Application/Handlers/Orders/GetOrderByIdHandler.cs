using BugStore.Application.Abstractions.Handlers.Orders;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Orders;

public class GetOrderByIdHandler(IOrderRepository orderRepository) : IGetOrderByIdHandler
{
    public async Task<Response<Order>> HandleAsync(GetOrderByIdRequest req, CancellationToken cancellationToken)
    {
        var searchResult = await orderRepository.GetByIdAsync(req.OrderId, cancellationToken);

        return searchResult.Success
            ? new Response<Order>(searchResult.Data!)
            : new Response<Order>(null, 400, [searchResult.ErrorMessage!]);
    }
}
