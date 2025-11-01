using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Products;

public interface IUpdateProductHandler
{
    Task<Response<Product>> HandleAsync(UpdateProductRequest req, CancellationToken cancellationToken = default);
}
