using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Orders;

public interface IGetOrderByIdHandler
{
    Task<Response<Order>> HandleAsync(GetOrderByIdRequest req, CancellationToken cancellationToken);
}
