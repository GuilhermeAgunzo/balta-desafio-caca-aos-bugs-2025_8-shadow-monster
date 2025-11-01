using BugStore.Application.Abstractions.Handlers.Products;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Products;

public class GetProductsHandler(IProductRepository productRepository) : IGetProductsHandler
{
    public async Task<PagedResponse<IEnumerable<Product>>> HandleAsync(GetProductsRequest req, CancellationToken cancellationToken = default)
    {
        var result = await productRepository.GetPagedAsync(req, cancellationToken);

        return result.Success
            ? new PagedResponse<IEnumerable<Product>>(result.Items, currentPage: req.PageNumber, pageSize: req.PageSize, totalCount: result.TotalCount)
            : new PagedResponse<IEnumerable<Product>>(null, 400, [result.ErrorMessage ?? "Unspecified error."]);
    }
}
