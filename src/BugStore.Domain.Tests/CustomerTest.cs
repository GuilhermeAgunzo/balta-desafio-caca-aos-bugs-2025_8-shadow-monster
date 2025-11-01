using BugStore.Domain.Entities;

namespace BugStore.Domain.Tests;

public class CustomerTest
{
    [Fact]
    public void Should_Create_Valid_Customer()
    {
        var (customer, _) = Customer.Create(
            name: "John",
            email: "john.doe@email.com",
            birthDate: new DateTime(1990, 1, 1),
            phone: "5511111111111");

        Assert.NotEqual(Guid.Empty, customer!.Id);
    }

    [Fact]
    public void Should_Not_Create_Invalid_Customer()
    {
        var (customer, _) = Customer.Create(
            name: "",
            email: "john.doe@email.com",
            birthDate: new DateTime(1990, 1, 1),
            phone: "5511111111111");

        Assert.Null(customer);
    }

    [Fact]
    public void Should_Return_Errors_On_Invalid_Customer()
    {
        var (_, errors) = Customer.Create(
            name: "",
            email: "john.doe@email.com",
            birthDate: new DateTime(1990, 1, 1),
            phone: "5511111111111");

        Assert.True(errors.Count > 0);
    }
}
