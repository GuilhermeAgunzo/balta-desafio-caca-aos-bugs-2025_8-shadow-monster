using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests;

public class OrderLineTest
{
    private readonly Guid _fakeOrderId = Guid.NewGuid();

    [Fact]
    public void Should_Create_Valid_OrderLine()
    {
        var (product, _) = Product.Create(
            title: "Product 1",
            description: "Product 1 description",
            price: 15.0m);

        var (line, _) = OrderLine.Create(
            orderId: _fakeOrderId,
            product: product!,
            quantity: 2);

        Assert.NotEqual(Guid.Empty, line!.Id);
    }

    [Fact]
    public void Should_Not_Create_Invalid_OrderLine()
    {
        var (product, _) = Product.Create(
            title: "Product 1",
            description: "Product 1 description",
            price: 15.0m);

        var (_, errors) = OrderLine.Create(
            orderId: _fakeOrderId,
            product: product!,
            quantity: 0);

        Assert.True(errors.Count > 0);
    }

    [Fact]
    public void Should_Calculate_Line_Total()
    {
        var (product, _) = Product.Create(
            title: "Product 1",
            description: "Product 1 description",
            price: 15.0m);

        var (line, _) = OrderLine.Create(
            orderId: _fakeOrderId,
            product: product!,
            quantity: 2);

        Assert.Equal(30, line!.Total);
    }
}
