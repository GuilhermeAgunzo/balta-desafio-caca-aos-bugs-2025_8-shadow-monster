namespace BugStore.Application.Requests.Products;

public class DeleteProductRequest : Request
{
    public Guid ProductId { get; set; }
}