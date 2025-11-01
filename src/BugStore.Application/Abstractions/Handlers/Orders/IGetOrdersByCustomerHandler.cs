using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Orders;

public interface IGetOrdersByCustomerHandler
{
    Task<Response<IEnumerable<Order>>> HandleAsync(GetOrdersByCustomerRequest req, CancellationToken cancellationToken = default);
}
