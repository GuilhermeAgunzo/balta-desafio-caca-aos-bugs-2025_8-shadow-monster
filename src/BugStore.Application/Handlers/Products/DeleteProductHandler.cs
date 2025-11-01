using BugStore.Application.Abstractions.Handlers.Products;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Products;
using BugStore.Application.Responses;

namespace BugStore.Application.Handlers.Products;

public class DeleteProductHandler(IProductRepository productRepository) : IDeleteProductHandler
{
    public async Task<Response<bool>> HandleAsync(DeleteProductRequest req, CancellationToken cancellationToken = default)
    {
        var productResult = await productRepository.GetByIdAsync(req.ProductId, cancellationToken);

        if (productResult.Data is null)
        {
            return new Response<bool>(false, 404, ["Product not found."]);
        }

        var result = await productRepository.DeleteAsync(productResult.Data.Id, cancellationToken);

        return result.Success
            ? new Response<bool>(true, 200, ["Product deleted successfully."])
            : new Response<bool>(false, 400, [result.ErrorMessage ?? "An error occurred while deleting the product."]);
    }
}
