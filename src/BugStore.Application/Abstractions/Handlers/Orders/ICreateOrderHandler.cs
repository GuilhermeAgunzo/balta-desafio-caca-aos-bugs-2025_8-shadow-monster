using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Orders;

public interface ICreateOrderHandler
{
    Task<Response<Order>> HandleAsync(CreateOrderRequest req, CancellationToken cancellationToken);
}
