namespace BugStore.Application.Requests.Customers;

public class GetCustomersRequest : PagedRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}