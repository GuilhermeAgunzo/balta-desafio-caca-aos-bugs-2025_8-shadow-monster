using Flunt.Notifications;
using Flunt.Validations;

namespace BugStore.Domain.Entities;

public class Order : Notifiable<Notification>
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; set; } = null!;

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public List<OrderLine> Lines { get; private set; } = [];

    public Order()
    {

    }

    private Order(Customer customer)
    {
        var utcNow = DateTime.UtcNow;

        Id = Guid.NewGuid();
        CustomerId = customer.Id;
        Customer = customer;
        CreatedAt = utcNow;
        UpdatedAt = utcNow;

        Validate();
    }

    public static (Order? order, IReadOnlyCollection<Notification> errors) Create(Customer customer)
    {
        var order = new Order(customer);
        return (
            order: order.IsValid ? order : null,
            errors: order.Notifications);
    }

    public void AddLine(Product product, int quantity)
    {
        var (line, errors) = OrderLine.Create(orderId: Id, product: product, quantity: quantity);

        if (line is not null)
        {
            Lines.Add(line);
            UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            AddNotifications(errors);
        }
    }

    private void Validate()
    {
        Clear();
        AddNotifications(new Contract<Order>()
            .Requires()
            .IsNotNull(Customer, "Order.Customer", "Customer is required")
            .IsGreaterThan(CreatedAt, DateTime.MinValue, "Order.CreatedAt", "CreatedAt must be valid")
            .IsGreaterOrEqualsThan(UpdatedAt, CreatedAt, "Order.UpdatedAt", "UpdatedAt must be after CreatedAt"));
    }

    public bool CanFinalize()
    {
        Validate();

        if (Lines.Count == 0)
            AddNotification(key: "Order.Lines", message: "Order Lines must have at least 1 item");

        return IsValid;
    }
}