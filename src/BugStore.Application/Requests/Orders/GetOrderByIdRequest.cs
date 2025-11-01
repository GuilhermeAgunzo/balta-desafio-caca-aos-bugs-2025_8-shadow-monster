namespace BugStore.Application.Requests.Orders;

public class GetOrderByIdRequest : Request
{
    public Guid OrderId { get; set; }
}