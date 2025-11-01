namespace BugStore.Application.Requests.Customers;

public class UpdateCustomerRequest : Request
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime BirthDate { get; set; }
}