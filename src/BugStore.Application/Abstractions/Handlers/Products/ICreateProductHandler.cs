using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Products;

public interface ICreateProductHandler
{
    Task<Response<Product>> HandleAsync(CreateProductRequest req, CancellationToken cancellationToken = default);
}
