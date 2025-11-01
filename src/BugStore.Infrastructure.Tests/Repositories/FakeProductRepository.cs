using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Common;
using BugStore.Application.Requests.Products;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Extensions;

namespace BugStore.Infrastructure.Tests.Repositories;

public class FakeProductRepository(List<Product> db) : IProductRepository
{
    public async Task<Result<Product>> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!product.IsValid)
                return Result<Product>.Fail("INVALID_ENTITY: Product is not valid");

            db.Add(product);

            return Result<Product>.Ok(product);
        }
        catch (Exception ex)
        {
            return Result<Product>.Fail($"GENERIC: Unexpected error while adding product {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = db.FirstOrDefault(c => c.Id == id);

            if (product is null)
                return Result<bool>.Fail("NOT_FOUND:Product not found");

            db.Remove(product);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"GENERIC: Unexpected error while deleting product - {ex.Message}");
        }
    }

    public async Task<Result<Product>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = db.FirstOrDefault(p => p.Id == id);

            return product is not null
                ? Result<Product>.Ok(product)
                : Result<Product>.Fail("NOT_FOUND: Product not found");
        }
        catch (Exception ex)
        {
            return Result<Product>.Fail($"GENERIC: Unexpected error while retrieving product - {ex.Message}");
        }
    }

    public async Task<Result<Product>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = db.FirstOrDefault(p => p.Slug == slug);

            return product is not null
                ? Result<Product>.Ok(product)
                : Result<Product>.Fail("NOT_FOUND: Product not found");
        }
        catch (Exception ex)
        {
            return Result<Product>.Fail($"GENERIC: Unexpected error while retrieving product by slug - {ex.Message}");
        }
    }

    public async Task<PagedResult<Product>> GetPagedAsync(GetProductsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = db
                .AsQueryable()
                .FilterBy(request)
                .OrderBy(p => p.Title);

            var totalCount = query.Count();

            var items = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return PagedResult<Product>.Ok(items, totalCount, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return PagedResult<Product>.Fail($"GENERIC: Unexpected error while paging products - {ex.Message}");
        }
    }

    public async Task<Result<Product>> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!product.IsValid)
                return Result<Product>.Fail("INVALID_ENTITY: Product is not valid");

            var existing = db.FirstOrDefault(c => c.Id == product.Id);
            if (existing is null)
                return Result<Product>.Fail("NOT_FOUND: Product not found");

            db.Remove(existing);
            db.Add(product);

            return Result<Product>.Ok(product);
        }
        catch (Exception ex)
        {
            return Result<Product>.Fail($"GENERIC: Unexpected error while updating product - {ex.Message}");
        }
    }
}
