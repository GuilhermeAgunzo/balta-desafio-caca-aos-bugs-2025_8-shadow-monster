using Flunt.Notifications;
using Flunt.Validations;

namespace BugStore.Domain.Entities;

public class OrderLine : Notifiable<Notification>
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }

    public int Quantity { get; private set; }
    public decimal Total { get; private set; }

    public Guid ProductId { get; private set; }
    public Product Product { get; set; } = null!;

    public OrderLine() { }

    private OrderLine(Guid orderId, Product product, int quantity)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Quantity = quantity;
        ProductId = product.Id;
        Product = product;
        Total = CalculateTotal(quantity: Quantity, price: product.Price);

        Validate();
    }

    public static (OrderLine? line, IReadOnlyCollection<Notification> errors) Create(Guid orderId, Product product, int quantity)
    {
        var line = new OrderLine(orderId: orderId, product: product, quantity: quantity);

        return (
            line: line.IsValid ? line : null,
            errors: line.Notifications);
    }

    private static decimal CalculateTotal(int quantity, decimal price)
        => quantity * price;

    private void Validate()
    {
        Clear();
        AddNotifications(new Contract<OrderLine>()
            .IsTrue(val: OrderId != Guid.Empty, property: "OrderLine:OrderId", message: "OrderId is required")
            .IsGreaterThan(val: Quantity, comparer: 0, key: "OrderLine.Quantity", message: "Quantity must be greater than 0")
            .IsNotNull(val: Product, key: "OrderLine.Product", message: "Product is required"));
    }
}