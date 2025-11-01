using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Products;

public interface IGetProductByIdHandler
{
    Task<Response<Product>> HandleAsync(GetProductByIdRequest req, CancellationToken cancellationToken = default);
}
