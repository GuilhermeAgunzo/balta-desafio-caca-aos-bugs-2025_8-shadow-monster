namespace BugStore.Application.Requests.Orders;

public class GetOrdersByCustomerRequest : PagedRequest
{
    public Guid CustomerId { get; set; }
}
