using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests;

public class ProductTest
{
    [Fact]
    public void Should_Create_Valid_Product()
    {
        var (product, _) = Product.Create("Product 1", "Product 1 description", 15);

        Assert.NotEqual(Guid.Empty, product!.Id);
    }

    [Fact]
    public void Should_Not_Create_Invalid_Product()
    {
        var (product, _) = Product.Create("", "", 15);

        Assert.Null(product);
    }

    [Fact]
    public void Should_Return_Errors_On_Invalid_Product()
    {
        var (_, errors) = Product.Create("", "", 15);

        Assert.True(errors.Count > 0);
    }

    [Fact]
    public void Should_Generate_Slug()
    {
        var (product, _) = Product.Create("Product 1", "Product 1 description", 15);

        Assert.Equal("product-1", product!.Slug);
    }
}