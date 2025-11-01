namespace BugStore.Application.Requests.Products;

public class GetProductByIdRequest : Request
{
    public Guid ProductId { get; set; }
}