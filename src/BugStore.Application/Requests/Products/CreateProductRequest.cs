namespace BugStore.Application.Requests.Products;

public class CreateProductRequest : Request
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}