namespace BugStore.Application.Requests.Products;

public class GetProductsRequest : PagedRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}