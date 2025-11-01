using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests;

public class OrderTest
{
    [Fact]
    public void Should_Create_Valid_Order()
    {
        var (customer, _) = Customer.Create(
            name: "John Doe",
            email: "john.doe@email.com",
            birthDate: new DateTime(1990, 1, 1),
            phone: "5511111111111");

        var (order, _) = Order.Create(customer!);

        Assert.NotEqual(Guid.Empty, order!.Id);
    }

    [Fact]
    public void Should_Add_OrderLine()
    {
        var (customer, _) = Customer.Create(
            name: "John Doe",
            email: "john.doe@email.com",
            birthDate: new DateTime(1990, 1, 1),
            phone: "5511111111111");

        var (product1, _) = Product.Create(
            title: "Product 1",
            description: "Product 1 description",
            price: 15.0m);

        var (order, _) = Order.Create(customer!);

        order!.AddLine(product1!, 2);

        Assert.True(order!.Lines.Count > 0);
    }

    [Fact]
    public void Should_Not_Finalize_Invalid_Order()
    {
        var (customer, _) = Customer.Create(
            name: "John Doe",
            email: "john.doe@email.com",
            birthDate: new DateTime(1990, 1, 1),
            phone: "5511111111111");

        var (order, _) = Order.Create(customer!);

        Assert.False(order!.CanFinalize());
    }
}
