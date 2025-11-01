namespace BugStore.Domain.Reports;

public class RevenueByPeriod
{
    public string Year { get; set; } = string.Empty;
    public string Month { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}
