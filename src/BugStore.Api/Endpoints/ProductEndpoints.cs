using BugStore.Application.Abstractions.Handlers.Products;
using BugStore.Application.Requests.Products;
using Microsoft.AspNetCore.Mvc;

namespace BugStore.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
  .WithTags("Products");

        group.MapPost("", async (
  [FromBody] CreateProductRequest request,
            [FromServices] ICreateProductHandler handler,
CancellationToken cancellationToken) =>
{
            var response = await handler.HandleAsync(request, cancellationToken);
     return response.IsSuccess
      ? Results.Created($"/api/products/{response.Data?.Id}", response)
     : Results.BadRequest(response);
   })
      .WithName("CreateProduct");

  group.MapGet("", async (
   [AsParameters] GetProductsRequest request,
   [FromServices] IGetProductsHandler handler,
  CancellationToken cancellationToken) =>
{
  var response = await handler.HandleAsync(request, cancellationToken);
return response.IsSuccess
        ? Results.Ok(response)
      : Results.BadRequest(response);
 })
    .WithName("GetProducts");

     group.MapGet("{id:guid}", async (
   Guid id,
      [FromServices] IGetProductByIdHandler handler,
   CancellationToken cancellationToken) =>
   {
   var request = new GetProductByIdRequest { ProductId = id };
          var response = await handler.HandleAsync(request, cancellationToken);
  return response.IsSuccess
  ? Results.Ok(response)
      : Results.NotFound(response);
        })
     .WithName("GetProductById");

        group.MapPut("{id:guid}", async (
       Guid id,
            [FromBody] UpdateProductRequest request,
  [FromServices] IUpdateProductHandler handler,
     CancellationToken cancellationToken) =>
        {
      request.ProductId = id;
 var response = await handler.HandleAsync(request, cancellationToken);
            return response.IsSuccess
    ? Results.Ok(response)
    : Results.BadRequest(response);
        })
.WithName("UpdateProduct");

        group.MapDelete("{id:guid}", async (
 Guid id,
            [FromServices] IDeleteProductHandler handler,
  CancellationToken cancellationToken) =>
 {
       var request = new DeleteProductRequest { ProductId = id };
      var response = await handler.HandleAsync(request, cancellationToken);
   return response.IsSuccess
            ? Results.NoContent()
: Results.BadRequest(response);
   })
   .WithName("DeleteProduct");
  }
}
