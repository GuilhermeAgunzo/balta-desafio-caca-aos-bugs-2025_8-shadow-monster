using BugStore.Application.Abstractions.Handlers.Orders;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Orders;

public class CreateOrderHandler(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IProductRepository productRepository) : ICreateOrderHandler
{
    public async Task<Response<Order>> HandleAsync(CreateOrderRequest req, CancellationToken cancellationToken)
    {
        var customerResult = await customerRepository.GetByIdAsync(req.CustomerId, cancellationToken);

        if (!customerResult.Success)
        {
            return new Response<Order>(null, 400, [customerResult.ErrorMessage ?? "Unspecified error."]);
        }

        var (order, orderErrors) = Order.Create(customerResult.Data!);

        if (order is null)
        {
            return new Response<Order>(null, 400, [.. orderErrors.Select(e => e.Message)]);
        }

        foreach (var line in req.OrderLines)
        {
            var productResult = await productRepository.GetByIdAsync(line.ProductId, cancellationToken);

            if (productResult.Data is null)
                return new Response<Order>(null, 400, [productResult.ErrorMessage ?? "Unspecified error."]);

            order.AddLine(productResult.Data, line.Quantity);
        }

        if (!order.CanFinalize())
        {
            return new Response<Order>(null, 400, [.. order.Notifications.Select(n => n.Message)]);
        }

        var createdOrderResult = await orderRepository.AddAsync(order, cancellationToken);

        return createdOrderResult.Success
            ? new Response<Order>(createdOrderResult.Data)
            : new Response<Order>(null, 500, [createdOrderResult.ErrorMessage ?? "Unspecified error."]);
    }
}
