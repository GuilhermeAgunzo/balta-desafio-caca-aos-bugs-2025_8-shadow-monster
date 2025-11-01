using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugStore.Application.Handlers.Customers;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Customers;

public class CreateCustomerHandlerTest
{
    private readonly List<Customer> _db = [];
    private readonly FakeCustomerRepository _repository;
    private readonly CreateCustomerHandler _handler;

    public CreateCustomerHandlerTest()
    {
        _repository = new FakeCustomerRepository(_db);
        _handler = new CreateCustomerHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateCustomer_WhenValidRequest()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "John Doe",
            Email = "john.doe@email.com",
            BirthDate = new DateTime(1990, 1, 1),
            Phone = "123456789"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal("John Doe", response.Data.Name);
        Assert.Equal("john.doe@email.com", response.Data.Email);
        Assert.Equal("123456789", response.Data.Phone);
        Assert.Equal(new DateTime(1990, 1, 1), response.Data.BirthDate);
        Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateCustomer_WhenPhoneIsNull()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "Jane Smith",
            Email = "jane.smith@email.com",
            BirthDate = new DateTime(1995, 5, 15),
            Phone = null
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal("Jane Smith", response.Data.Name);
        Assert.Null(response.Data.Phone);
        Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBadRequest_WhenCustomerAlreadyExists()
    {
        // Arrange
        var existingCustomer = Customer.Create(
            "Existing Customer",
            "existing@email.com",
            new DateTime(1985, 3, 20)).customer!;
        _db.Add(existingCustomer);

        var request = new CreateCustomerRequest
        {
            Name = "Another Customer",
            Email = "existing@email.com",
            BirthDate = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
        Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains("Customer already exists", response.Messages);
        Assert.Single(_db); // Should still have only the original customer
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "",
            Email = "test@email.com",
            BirthDate = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
        Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains(response.Messages, m => m.Contains("Name is required"));
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBadRequest_WhenEmailIsEmpty()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "John Doe",
            Email = "",
            BirthDate = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
        Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains(response.Messages, m => m.Contains("Email is required"));
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateCustomer_WhenBirthDateIsMinValue()
    {
        // Arrange
        // Note: Customer validation uses IsGreaterOrEqualsThan with DateTime.MinValue,
        // which means DateTime.MinValue is actually a valid value
        var request = new CreateCustomerRequest
        {
            Name = "John Doe",
            Email = "john@email.com",
            BirthDate = DateTime.MinValue
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "",
            Email = "",
            BirthDate = new DateTime(1990, 1, 1) // Valid birth date
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
        Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.True(response.Messages.Length >= 2); // Should have at least 2 error messages (name and email)
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldGenerateNewGuidForCustomerId()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "Test User",
            Email = "test.user@email.com",
            BirthDate = new DateTime(1992, 6, 10)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.NotEqual(Guid.Empty, response.Data.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateMultipleCustomers_WithDifferentEmails()
    {
        // Arrange
        var request1 = new CreateCustomerRequest
        {
            Name = "Customer One",
            Email = "customer1@email.com",
            BirthDate = new DateTime(1990, 1, 1)
        };

        var request2 = new CreateCustomerRequest
        {
            Name = "Customer Two",
            Email = "customer2@email.com",
            BirthDate = new DateTime(1991, 2, 2)
        };

        // Act
        var response1 = await _handler.HandleAsync(request1, TestContext.Current.CancellationToken);
        var response2 = await _handler.HandleAsync(request2, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response1.IsSuccess);
        Assert.True(response2.IsSuccess);
        Assert.Equal(2, _db.Count);
        Assert.NotEqual(response1.Data!.Id, response2.Data!.Id);
    }
}
