namespace BugStore.Domain.Reports;

public class CustomerOrderSummary
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public long TotalOrders { get; set; }
    public long QtProducts { get; set; }
    public decimal TotalSpent { get; set; }
}
