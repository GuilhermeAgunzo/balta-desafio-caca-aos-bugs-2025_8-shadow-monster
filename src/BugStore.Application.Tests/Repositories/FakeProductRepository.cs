using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Common;
using BugStore.Application.Requests.Products;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.Repositories;

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
            var query = db.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Title))
                query = query.Where(p => p.Title.Contains(request.Title, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(request.Description))
                query = query.Where(p => p.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(request.Slug))
                query = query.Where(p => p.Slug.Equals(request.Slug, StringComparison.OrdinalIgnoreCase));
            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            var totalCount = query.Count();

            var items = query
                .OrderBy(p => p.Title)
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
