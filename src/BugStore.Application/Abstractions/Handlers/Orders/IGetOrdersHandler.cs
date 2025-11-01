using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Orders;

public interface IGetOrdersHandler
{
    Task<Response<IEnumerable<Order>>> HandleAsync(GetOrdersRequest req, CancellationToken cancellationToken = default);
}
