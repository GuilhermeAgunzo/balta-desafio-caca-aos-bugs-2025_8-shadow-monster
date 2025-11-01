using BugStore.Application.Handlers.Customers;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Customers;

public class DeleteCustomerHandlerTest
{
    private readonly List<Customer> _db = [];
    private readonly FakeCustomerRepository _repository;
    private readonly DeleteCustomerHandler _handler;

    public DeleteCustomerHandlerTest()
    {
        _repository = new FakeCustomerRepository(_db);
  _handler = new DeleteCustomerHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteCustomer_WhenCustomerExists()
    {
        // Arrange
        var customer = Customer.Create(
      "John Doe",
  "john.doe@email.com",
            new DateTime(1990, 1, 1),
            "123456789").customer!;
        _db.Add(customer);

        var request = new DeleteCustomerRequest
        {
            CustomerId = customer.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

   // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.True(response.Data);
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
  var request = new DeleteCustomerRequest
   {
            CustomerId = nonExistentId
        };

      // Act
  var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

      // Assert
     Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
        Assert.False(response.Data);
 Assert.NotNull(response.Messages);
  Assert.Contains("Customer not found", response.Messages);
  Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteOnlySpecifiedCustomer_WhenMultipleCustomersExist()
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

        var request = new DeleteCustomerRequest
     {
            CustomerId = customer2.Id
   };

     // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.True(response.Data);
     Assert.Equal(2, _db.Count);
   Assert.Contains(customer1, _db);
   Assert.DoesNotContain(customer2, _db);
        Assert.Contains(customer3, _db);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteCustomerWithAllFields_WhenCustomerHasCompleteData()
  {
  // Arrange
        var customer = Customer.Create(
         "Jane Smith",
            "jane.smith@email.com",
            new DateTime(1995, 5, 15),
     "987654321").customer!;
        _db.Add(customer);

      var request = new DeleteCustomerRequest
        {
  CustomerId = customer.Id
        };

        // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

      // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.True(response.Data);
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteCustomerWithNullPhone_WhenPhoneIsOptional()
    {
     // Arrange
        var customer = Customer.Create(
        "No Phone Customer",
     "nophone@email.com",
            new DateTime(1990, 1, 1),
       null).customer!;
  _db.Add(customer);

        var request = new DeleteCustomerRequest
        {
          CustomerId = customer.Id
        };

  // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

// Assert
     Assert.NotNull(response);
        Assert.True(response.IsSuccess);
      Assert.True(response.Data);
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDeletingAlreadyDeletedCustomer()
    {
        // Arrange
     var customer = Customer.Create(
        "Test User",
 "test@email.com",
  new DateTime(1990, 1, 1)).customer!;
     var customerId = customer.Id;
        _db.Add(customer);

        var request = new DeleteCustomerRequest
  {
    CustomerId = customerId
        };

   // First deletion
 await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Act - Second deletion attempt
     var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.NotNull(response);
        Assert.False(response.IsSuccess);
  Assert.Equal(404, response.StatusCode);
        Assert.False(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains("Customer not found", response.Messages);
     Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenCustomerIdIsEmpty()
    {
      // Arrange
  var request = new DeleteCustomerRequest
 {
CustomerId = Guid.Empty
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
        Assert.False(response.Data);
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteCustomer_AndReduceDatabaseSize()
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
        var initialCount = _db.Count;

        var request = new DeleteCustomerRequest
        {
      CustomerId = customer1.Id
        };

      // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
   Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(initialCount - 1, _db.Count);
        Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenDeletionIsSuccessful()
    {
        // Arrange
        var customer = Customer.Create(
     "Success Test",
    "success@email.com",
            new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);

  var request = new DeleteCustomerRequest
      {
            CustomerId = customer.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.True(response.Data);
        Assert.Equal(200, response.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalse_WhenDeletionFails()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new DeleteCustomerRequest
        {
            CustomerId = nonExistentId
     };

     // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
      Assert.NotNull(response.Data);
        Assert.False(response.Data);
        Assert.Equal(404, response.StatusCode);
    }
}
