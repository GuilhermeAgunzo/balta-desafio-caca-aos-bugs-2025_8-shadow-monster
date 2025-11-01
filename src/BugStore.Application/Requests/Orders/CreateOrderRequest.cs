namespace BugStore.Application.Requests.Orders;

public class CreateOrderRequest : Request
{
    public Guid CustomerId { get; set; }
    public IEnumerable<OrderLineDTO> OrderLines { get; set; } = [];
}

public class OrderLineDTO
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}