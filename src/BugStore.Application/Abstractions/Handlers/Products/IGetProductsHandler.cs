using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Handlers.Products;

public interface IGetProductsHandler
{
    Task<PagedResponse<IEnumerable<Product>>> HandleAsync(GetProductsRequest req, CancellationToken cancellationToken = default);
}
