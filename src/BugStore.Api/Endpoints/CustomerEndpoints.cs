using BugStore.Application.Abstractions.Handlers.Customers;
using BugStore.Application.Requests.Customers;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers")
            .WithTags("Customers");

        group.MapPost("", async (
      [FromBody] CreateCustomerRequest request,
    [FromServices] ICreateCustomerHandler handler,
   CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(request, cancellationToken);
    return response.IsSuccess
    ? Results.Created($"/api/customers/{response.Data?.Id}", response)
  : Results.BadRequest(response);
        })
        .WithName("CreateCustomer");

 group.MapGet("", async (
            [AsParameters] GetCustomersRequest request,
            [FromServices] IGetCustomersHandler handler,
        CancellationToken cancellationToken) =>
        {
var response = await handler.HandleAsync(request, cancellationToken);
    return response.IsSuccess
          ? Results.Ok(response)
                : Results.BadRequest(response);
        })
        .WithName("GetCustomers");

        group.MapGet("{id:guid}", async (
      Guid id,
            [FromServices] IGetCustomerByIdHandler handler,
    CancellationToken cancellationToken) =>
        {
       var request = new GetCustomerByIdRequest { CustomerId = id };
            var response = await handler.HandleAsync(request, cancellationToken);
     return response.IsSuccess
                ? Results.Ok(response)
       : Results.NotFound(response);
     })
        .WithName("GetCustomerById");

        group.MapGet("by-email/{email}", async (
      string email,
       [FromServices] IGetCustomerByEmailHandler handler,
       CancellationToken cancellationToken) =>
     {
     var request = new GetCustomerByEmailRequest { Email = email };
  var response = await handler.HandleAsync(request, cancellationToken);
         return response.IsSuccess
    ? Results.Ok(response)
           : Results.NotFound(response);
        })
   .WithName("GetCustomerByEmail");

        group.MapPut("{id:guid}", async (
            Guid id,
            [FromBody] UpdateCustomerRequest request,
   [FromServices] IUpdateCustomerHandler handler,
   CancellationToken cancellationToken) =>
        {
  request.CustomerId = id;
            var response = await handler.HandleAsync(request, cancellationToken);
            return response.IsSuccess
  ? Results.Ok(response)
 : Results.BadRequest(response);
        })
        .WithName("UpdateCustomer");

        group.MapDelete("{id:guid}", async (
Guid id,
        [FromServices] IDeleteCustomerHandler handler,
            CancellationToken cancellationToken) =>
    {
   var request = new DeleteCustomerRequest { CustomerId = id };
            var response = await handler.HandleAsync(request, cancellationToken);
   return response.IsSuccess
       ? Results.NoContent()
       : Results.BadRequest(response);
      })
        .WithName("DeleteCustomer");
    }
}
