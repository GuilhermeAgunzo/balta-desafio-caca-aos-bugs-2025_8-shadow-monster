namespace BugStore.Application.Requests.Products;

public class UpdateProductRequest : Request
{
    public Guid ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}