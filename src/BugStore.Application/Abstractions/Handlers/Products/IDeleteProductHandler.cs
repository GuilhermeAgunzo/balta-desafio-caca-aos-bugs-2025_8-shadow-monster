using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;

namespace BugStore.Application.Abstractions.Handlers.Products;

public interface IDeleteProductHandler
{
    Task<Response<bool>> HandleAsync(DeleteProductRequest req, CancellationToken cancellationToken = default);
}
