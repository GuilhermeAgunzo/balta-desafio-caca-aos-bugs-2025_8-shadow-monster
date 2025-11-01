using BugStore.Application.Abstractions.Handlers.Products;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Products;

public class UpdateProductHandler(IProductRepository productRepository) : IUpdateProductHandler
{
    public async Task<Response<Product>> HandleAsync(UpdateProductRequest req, CancellationToken cancellationToken = default)
    {
        var existingCustomerResult = await productRepository.GetByIdAsync(req.ProductId, cancellationToken);

        if (existingCustomerResult.Data is null)
            return new Response<Product>(null, 404, ["Product not found"]);

        var (product, errors) = Product.Create(
            title: req.Title,
            description: req.Description,
            price: req.Price);

        if (product is null)
        {
            return new Response<Product>(null, 400, [.. errors.Select(e => e.Message)]);
        }

        typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, req.ProductId);

        var result = await productRepository.UpdateAsync(product, cancellationToken);

        return new Response<Product>(result.Data);
    }
}
