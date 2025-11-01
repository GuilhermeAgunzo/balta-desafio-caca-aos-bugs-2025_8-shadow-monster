using BugStore.Application.Abstractions.Handlers.Orders;
using BugStore.Application.Requests.Orders;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
  .WithTags("Orders");

        group.MapPost("", async (
      [FromBody] CreateOrderRequest request,
    [FromServices] ICreateOrderHandler handler,
   CancellationToken cancellationToken) =>
   {
   var response = await handler.HandleAsync(request, cancellationToken);
            return response.IsSuccess
     ? Results.Created($"/api/orders/{response.Data?.Id}", response)
: Results.BadRequest(response);
 })
        .WithName("CreateOrder");

        group.MapGet("", async (
      [AsParameters] GetOrdersRequest request,
  [FromServices] IGetOrdersHandler handler,
  CancellationToken cancellationToken) =>
{
       var response = await handler.HandleAsync(request, cancellationToken);
  return response.IsSuccess
  ? Results.Ok(response)
  : Results.BadRequest(response);
      })
     .WithName("GetOrders");

        group.MapGet("{id:guid}", async (
 Guid id,
       [FromServices] IGetOrderByIdHandler handler,
CancellationToken cancellationToken) =>
     {
            var request = new GetOrderByIdRequest { OrderId = id };
 var response = await handler.HandleAsync(request, cancellationToken);
    return response.IsSuccess
  ? Results.Ok(response)
: Results.NotFound(response);
})
        .WithName("GetOrderById");

    group.MapGet("by-customer/{customerId:guid}", async (
        Guid customerId,
     [FromServices] IGetOrdersByCustomerHandler handler,
 CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10) =>
  {
            var request = new GetOrdersByCustomerRequest 
    { 
      CustomerId = customerId,
   PageNumber = pageNumber,
      PageSize = pageSize
          };
 var response = await handler.HandleAsync(request, cancellationToken);
return response.IsSuccess
? Results.Ok(response)
         : Results.BadRequest(response);
  })
        .WithName("GetOrdersByCustomer");
}
}
