using BugStore.Application.Common;
using BugStore.Application.Requests.Products;
using BugStore.Domain.Entities;

namespace BugStore.Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<Result<Product>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Product>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> GetPagedAsync(GetProductsRequest request, CancellationToken cancellationToken = default);

    Task<Result<Product>> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result<Product>> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
