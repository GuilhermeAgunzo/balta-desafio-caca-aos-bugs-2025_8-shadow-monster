using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Common;
using BugStore.Application.Requests.Products;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BugStore.Infrastructure.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<Result<Product>> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!product.IsValid)
                return Result<Product>.Fail("INVALID_ENTITY: Product is not valid");

            await db.Products.AddAsync(product, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

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
            var product = await db.Products.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (product is null)
                return Result<bool>.Fail("NOT_FOUND:Product not found");

            db.Products.Remove(product);
            await db.SaveChangesAsync(cancellationToken);

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
            var product = await db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

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
            var product = await db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);

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
            var query = db.Products
                .AsNoTracking()
                .FilterBy(request)
                .OrderBy(p => p.Title);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

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

            var existing = await db.Products.FirstOrDefaultAsync(c => c.Id == product.Id, cancellationToken);
            if (existing is null)
                return Result<Product>.Fail("NOT_FOUND: Product not found");

            db.Entry(existing).CurrentValues.SetValues(product);
            await db.SaveChangesAsync(cancellationToken);

            return Result<Product>.Ok(product);
        }
        catch (Exception ex)
        {
            return Result<Product>.Fail($"GENERIC: Unexpected error while updating product - {ex.Message}");
        }
    }
}
