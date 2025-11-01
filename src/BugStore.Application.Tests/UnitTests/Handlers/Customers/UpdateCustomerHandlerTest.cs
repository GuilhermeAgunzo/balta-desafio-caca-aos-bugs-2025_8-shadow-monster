using BugStore.Application.Handlers.Customers;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Customers;

public class UpdateCustomerHandlerTest
{
    private readonly List<Customer> _db = [];
    private readonly FakeCustomerRepository _repository;
    private readonly UpdateCustomerHandler _handler;

    public UpdateCustomerHandlerTest()
    {
        _repository = new FakeCustomerRepository(_db);
        _handler = new UpdateCustomerHandler(_repository);
    }

    [Fact]
  public async Task HandleAsync_ShouldUpdateCustomer_WhenValidRequest()
    {
  // Arrange
        var existingCustomer = Customer.Create(
  "John Doe",
            "john.doe@email.com",
          new DateTime(1990, 1, 1),
   "123456789").customer!;
        _db.Add(existingCustomer);

     var request = new UpdateCustomerRequest
    {
    CustomerId = existingCustomer.Id,
            Name = "John Updated Doe",
  Email = "john.updated@email.com",
            BirthDate = new DateTime(1990, 1, 1),
            Phone = "987654321"
   };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
      Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
    Assert.Equal(existingCustomer.Id, response.Data.Id);
        Assert.Equal("John Updated Doe", response.Data.Name);
        Assert.Equal("john.updated@email.com", response.Data.Email);
        Assert.Equal("987654321", response.Data.Phone);
        Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateCustomer_WhenPhoneIsNull()
    {
        // Arrange
        var existingCustomer = Customer.Create(
  "Jane Smith",
 "jane.smith@email.com",
    new DateTime(1995, 5, 15),
    "111222333").customer!;
  _db.Add(existingCustomer);

        var request = new UpdateCustomerRequest
        {
   CustomerId = existingCustomer.Id,
  Name = "Jane Updated Smith",
     Email = "jane.updated@email.com",
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
        Assert.Equal(existingCustomer.Id, response.Data.Id);
        Assert.Equal("Jane Updated Smith", response.Data.Name);
        Assert.Null(response.Data.Phone);
  Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
     // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateCustomerRequest
        {
            CustomerId = nonExistentId,
            Name = "Non Existent",
    Email = "nonexistent@email.com",
         BirthDate = new DateTime(1990, 1, 1)
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
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var existingCustomer = Customer.Create(
            "Test User",
    "test@email.com",
          new DateTime(1990, 1, 1)).customer!;
        _db.Add(existingCustomer);

        var request = new UpdateCustomerRequest
        {
   CustomerId = existingCustomer.Id,
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
        Assert.Single(_db); // Original customer should still exist
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBadRequest_WhenEmailIsEmpty()
{
        // Arrange
        var existingCustomer = Customer.Create(
    "Test User",
        "test@email.com",
     new DateTime(1990, 1, 1)).customer!;
   _db.Add(existingCustomer);

    var request = new UpdateCustomerRequest
   {
            CustomerId = existingCustomer.Id,
   Name = "Test User",
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
        Assert.Single(_db); // Original customer should still exist
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateCustomer_WhenBirthDateIsMinValue()
    {
      // Arrange
  // Note: Customer validation uses IsGreaterOrEqualsThan with DateTime.MinValue,
     // which means DateTime.MinValue is actually a valid value
 var existingCustomer = Customer.Create(
 "Test User",
    "test@email.com",
       new DateTime(1990, 1, 1)).customer!;
        _db.Add(existingCustomer);

        var request = new UpdateCustomerRequest
        {
CustomerId = existingCustomer.Id,
       Name = "Test User Updated",
     Email = "test.updated@email.com",
            BirthDate = DateTime.MinValue
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
  Assert.NotNull(response.Data);
        Assert.Equal(existingCustomer.Id, response.Data.Id);
        Assert.Equal(DateTime.MinValue, response.Data.BirthDate);
   Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
    {
    // Arrange
        var existingCustomer = Customer.Create(
         "Test User",
            "test@email.com",
         new DateTime(1990, 1, 1)).customer!;
      _db.Add(existingCustomer);

        var request = new UpdateCustomerRequest
        {
            CustomerId = existingCustomer.Id,
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
        Assert.Single(_db); // Original customer should still exist
    }

    [Fact]
    public async Task HandleAsync_ShouldPreserveCustomerId_WhenUpdating()
    {
        // Arrange
        var existingCustomer = Customer.Create(
      "Original Name",
 "original@email.com",
            new DateTime(1990, 1, 1)).customer!;
        var originalId = existingCustomer.Id;
        _db.Add(existingCustomer);

        var request = new UpdateCustomerRequest
  {
   CustomerId = originalId,
     Name = "Updated Name",
 Email = "updated@email.com",
        BirthDate = new DateTime(1991, 2, 2)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
     Assert.NotNull(response.Data);
        Assert.Equal(originalId, response.Data.Id);
    Assert.Single(_db);
Assert.Equal(originalId, _db.First().Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateAllFields_WhenAllFieldsChanged()
    {
        // Arrange
        var existingCustomer = Customer.Create(
            "Old Name",
     "old@email.com",
         new DateTime(1980, 1, 1),
      "111111111").customer!;
        _db.Add(existingCustomer);

        var request = new UpdateCustomerRequest
        {
   CustomerId = existingCustomer.Id,
        Name = "New Name",
     Email = "new@email.com",
            BirthDate = new DateTime(2000, 12, 31),
       Phone = "999999999"
        };

   // Act
var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
 Assert.NotNull(response);
        Assert.True(response.IsSuccess);
   Assert.NotNull(response.Data);
 Assert.Equal(existingCustomer.Id, response.Data.Id);
    Assert.Equal("New Name", response.Data.Name);
        Assert.Equal("new@email.com", response.Data.Email);
     Assert.Equal(new DateTime(2000, 12, 31), response.Data.BirthDate);
   Assert.Equal("999999999", response.Data.Phone);
        Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotAffectOtherCustomers_WhenUpdating()
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
      _db.Add(customer1);
        _db.Add(customer2);

     var request = new UpdateCustomerRequest
        {
     CustomerId = customer1.Id,
     Name = "Customer One Updated",
        Email = "customer1.updated@email.com",
        BirthDate = new DateTime(1990, 1, 1)
        };

 // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, _db.Count);
   var unchangedCustomer = _db.First(c => c.Id == customer2.Id);
     Assert.Equal("Customer Two", unchangedCustomer.Name);
        Assert.Equal("customer2@email.com", unchangedCustomer.Email);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdatePhoneToNull_WhenPhoneWasPreviouslySet()
    {
        // Arrange
 var existingCustomer = Customer.Create(
  "Test User",
     "test@email.com",
        new DateTime(1990, 1, 1),
            "123456789").customer!;
        _db.Add(existingCustomer);

      var request = new UpdateCustomerRequest
        {
  CustomerId = existingCustomer.Id,
            Name = "Test User",
      Email = "test@email.com",
          BirthDate = new DateTime(1990, 1, 1),
 Phone = null
        };

        // Act
    var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

 // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Null(response.Data.Phone);
        Assert.Single(_db);
    }
}
