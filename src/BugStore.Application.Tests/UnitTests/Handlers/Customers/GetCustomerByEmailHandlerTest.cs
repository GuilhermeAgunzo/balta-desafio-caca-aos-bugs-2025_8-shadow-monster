using BugStore.Application.Handlers.Customers;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Customers;

public class GetCustomerByEmailHandlerTest
{
    private readonly List<Customer> _db = [];
    private readonly FakeCustomerRepository _repository;
    private readonly GetCustomerByEmailHandler _handler;

    public GetCustomerByEmailHandlerTest()
    {
        _repository = new FakeCustomerRepository(_db);
     _handler = new GetCustomerByEmailHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomer_WhenEmailExists()
    {
        // Arrange
        var customer = Customer.Create(
            "John Doe",
            "john.doe@email.com",
            new DateTime(1990, 1, 1),
"123456789").customer!;
        _db.Add(customer);

    var request = new GetCustomerByEmailRequest
  {
        Email = "john.doe@email.com"
    };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
    Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(customer.Id, response.Data.Id);
      Assert.Equal("John Doe", response.Data.Name);
     Assert.Equal("john.doe@email.com", response.Data.Email);
        Assert.Equal("123456789", response.Data.Phone);
        Assert.Equal(new DateTime(1990, 1, 1), response.Data.BirthDate);
}

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenEmailDoesNotExist()
    {
// Arrange
        var request = new GetCustomerByEmailRequest
        {
   Email = "nonexistent@email.com"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

  // Assert
        Assert.NotNull(response);
   Assert.False(response.IsSuccess);
 Assert.Equal(404, response.StatusCode);
    Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains("Customer not found", response.Messages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectCustomer_WhenMultipleCustomersExist()
    {
        // Arrange
    var customer1 = Customer.Create(
            "Customer One",
          "customer1@email.com",
            new DateTime(1990, 1, 1)).customer!;
     var customer2 = Customer.Create(
         "Customer Two",
      "customer2@email.com",
            new DateTime(1991, 2, 2)).customer!;
     var customer3 = Customer.Create(
            "Customer Three",
            "customer3@email.com",
   new DateTime(1992, 3, 3)).customer!;
        
        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomerByEmailRequest
        {
    Email = "customer2@email.com"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
 Assert.NotNull(response.Data);
        Assert.Equal(customer2.Id, response.Data.Id);
      Assert.Equal("Customer Two", response.Data.Name);
        Assert.Equal("customer2@email.com", response.Data.Email);
    }

    [Fact]
  public async Task HandleAsync_ShouldReturnCustomerWithNullPhone_WhenPhoneIsNull()
    {
        // Arrange
        var customer = Customer.Create(
            "No Phone Customer",
            "nophone@email.com",
            new DateTime(1990, 1, 1),
 null).customer!;
        _db.Add(customer);

    var request = new GetCustomerByEmailRequest
        {
            Email = "nophone@email.com"
     };

      // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal("No Phone Customer", response.Data.Name);
        Assert.Null(response.Data.Phone);
    }

    [Fact]
    public async Task HandleAsync_ShouldBeCaseSensitive_WhenSearchingByEmail()
    {
        // Arrange
        var customer = Customer.Create(
            "Test User",
 "test@email.com",
  new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);

  var request = new GetCustomerByEmailRequest
      {
            Email = "TEST@EMAIL.COM" // Different case
      };

        // Act
  var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  // Note: This test behavior depends on the repository implementation
        // If email comparison is case-insensitive, this test should be adjusted
    Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenEmailIsEmpty()
  {
     // Arrange
    var customer = Customer.Create(
            "Test User",
         "test@email.com",
            new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);

     var request = new GetCustomerByEmailRequest
        {
Email = ""
        };

  // Act
     var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
  Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomerWithMinBirthDate_WhenBirthDateIsMinValue()
    {
        // Arrange
     var customer = Customer.Create(
     "Min Date Customer",
            "mindate@email.com",
  DateTime.MinValue).customer!;
        _db.Add(customer);

        var request = new GetCustomerByEmailRequest
        {
      Email = "mindate@email.com"
    };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.NotNull(response);
   Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(DateTime.MinValue, response.Data.BirthDate);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFirstCustomer_WhenSearchedByExactEmail()
    {
        // Arrange
      var email = "specific@email.com";
        var customer = Customer.Create(
         "Specific Customer",
      email,
        new DateTime(1990, 1, 1),
            "111222333").customer!;
        _db.Add(customer);

        var request = new GetCustomerByEmailRequest
        {
    Email = email
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
 Assert.NotNull(response.Data);
        Assert.Equal(customer.Id, response.Data.Id);
        Assert.Equal(email, response.Data.Email);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotModifyDatabase_WhenRetrievingCustomer()
    {
    // Arrange
        var customer = Customer.Create(
"Read Only Test",
          "readonly@email.com",
     new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);
        var initialCount = _db.Count;

      var request = new GetCustomerByEmailRequest
        {
     Email = "readonly@email.com"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.NotNull(response);
        Assert.True(response.IsSuccess);
     Assert.Equal(initialCount, _db.Count);
  Assert.Contains(customer, _db);
  }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDatabaseIsEmpty()
    {
        // Arrange
        var request = new GetCustomerByEmailRequest
        {
            Email = "any@email.com"
  };

      // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
        Assert.Null(response.Data);
        Assert.Empty(_db);
    }
}
