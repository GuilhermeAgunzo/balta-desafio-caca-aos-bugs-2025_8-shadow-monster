using BugStore.Application.Abstractions.Handlers.Products;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Products;

public class GetProductByIdHandler(IProductRepository productRepository) : IGetProductByIdHandler
{
    public async Task<Response<Product>> HandleAsync(GetProductByIdRequest req, CancellationToken cancellationToken = default)
    {
        var result = await productRepository.GetByIdAsync(req.ProductId, cancellationToken);

        if (result.Data is null)
            return new Response<Product>(null, 404, ["Product not found"]);

        return new Response<Product>(result.Data);
    }
}
