namespace BugStore.Application.Requests.Customers;

public class DeleteCustomerRequest : Request
{
    public Guid CustomerId { get; set; }
}