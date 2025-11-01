namespace BugStore.Application.Requests.Customers;

public class GetCustomerByIdRequest : Request
{
    public Guid CustomerId { get; set; }
}