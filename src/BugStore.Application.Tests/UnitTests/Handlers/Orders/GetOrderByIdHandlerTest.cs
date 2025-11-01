using BugStore.Application.Handlers.Orders;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Orders;

public class GetOrderByIdHandlerTest
{
    private readonly List<Order> _orderDb = [];
  private readonly List<Customer> _customerDb = [];
    private readonly List<Product> _productDb = [];
    private readonly FakeOrderRepository _orderRepository;
    private readonly FakeCustomerRepository _customerRepository;
    private readonly FakeProductRepository _productRepository;
    private readonly GetOrderByIdHandler _handler;

    public GetOrderByIdHandlerTest()
    {
     _orderRepository = new FakeOrderRepository(_orderDb);
   _customerRepository = new FakeCustomerRepository(_customerDb);
        _productRepository = new FakeProductRepository(_productDb);
   _handler = new GetOrderByIdHandler(_orderRepository);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WhenIdExists()
    {
   // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

 var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

        var (order, _) = Order.Create(customer!);
     order!.AddLine(product!, 2);
  _orderDb.Add(order);

   var request = new GetOrderByIdRequest
     {
  OrderId = order.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
   Assert.NotNull(response.Data);
        Assert.Equal(order.Id, response.Data.Id);
   Assert.Equal(customer.Id, response.Data.CustomerId);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenIdDoesNotExist()
    {
        // Arrange
  var nonExistentId = Guid.NewGuid();
        var request = new GetOrderByIdRequest
{
       OrderId = nonExistentId
  };

        // Act
      var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

  // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
     Assert.Equal(400, response.StatusCode);
   Assert.Null(response.Data);
     Assert.NotNull(response.Messages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectOrder_WhenMultipleOrdersExist()
    {
   // Arrange
        var (customer1, _) = Customer.Create("Customer 1", "customer1@email.com", new DateTime(1990, 1, 1));
   var (customer2, _) = Customer.Create("Customer 2", "customer2@email.com", new DateTime(1991, 2, 2));
      _customerDb.Add(customer1!);
        _customerDb.Add(customer2!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

        var (order1, _) = Order.Create(customer1!);
        order1!.AddLine(product!, 1);
        var (order2, _) = Order.Create(customer2!);
        order2!.AddLine(product!, 2);
var (order3, _) = Order.Create(customer1!);
        order3!.AddLine(product!, 3);

   _orderDb.Add(order1);
        _orderDb.Add(order2);
      _orderDb.Add(order3);

   var request = new GetOrderByIdRequest
     {
OrderId = order2.Id
        };

// Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

      // Assert
Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
   Assert.Equal(order2.Id, response.Data.Id);
        Assert.Equal(customer2.Id, response.Data.CustomerId);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenOrderIdIsEmpty()
    {
        // Arrange
   var request = new GetOrderByIdRequest
        {
     OrderId = Guid.Empty
};

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
Assert.False(response.IsSuccess);
   Assert.Equal(400, response.StatusCode);
     Assert.Null(response.Data);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WithAllLines()
    {
        // Arrange
     var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

   var (product1, _) = Product.Create("Product 1", "Description 1", 10.00m);
     var (product2, _) = Product.Create("Product 2", "Description 2", 20.00m);
        _productDb.Add(product1!);
        _productDb.Add(product2!);

        var (order, _) = Order.Create(customer!);
   order!.AddLine(product1!, 2);
  order.AddLine(product2!, 3);
    _orderDb.Add(order);

    var request = new GetOrderByIdRequest
        {
       OrderId = order.Id
        };

    // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
    Assert.NotNull(response);
     Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
  Assert.Equal(2, response.Data.Lines.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotModifyDatabase_WhenRetrievingOrder()
    {
     // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
 _customerDb.Add(customer!);

   var (product, _) = Product.Create("Product 1", "Description", 50.00m);
 _productDb.Add(product!);

        var (order, _) = Order.Create(customer!);
        order!.AddLine(product!, 1);
        _orderDb.Add(order);
     var initialCount = _orderDb.Count;

   var request = new GetOrderByIdRequest
        {
            OrderId = order.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
        Assert.True(response.IsSuccess);
  Assert.Equal(initialCount, _orderDb.Count);
        Assert.Contains(order, _orderDb);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenDatabaseIsEmpty()
{
// Arrange
     var request = new GetOrderByIdRequest
        {
         OrderId = Guid.NewGuid()
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
      Assert.Equal(400, response.StatusCode);
   Assert.Null(response.Data);
  Assert.Empty(_orderDb);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSameOrder_WhenCalledMultipleTimes()
    {
     // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
     _productDb.Add(product!);

var (order, _) = Order.Create(customer!);
   order!.AddLine(product!, 1);
        _orderDb.Add(order);

 var request = new GetOrderByIdRequest
{
    OrderId = order.Id
   };

        // Act
        var response1 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
  var response2 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
 Assert.NotNull(response1);
        Assert.NotNull(response2);
  Assert.True(response1.IsSuccess);
        Assert.True(response2.IsSuccess);
        Assert.Equal(response1.Data!.Id, response2.Data!.Id);
 Assert.Equal(response1.Data.CustomerId, response2.Data.CustomerId);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WithCorrectStatusCode()
    {
        // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
   _productDb.Add(product!);

        var (order, _) = Order.Create(customer!);
     order!.AddLine(product!, 1);
   _orderDb.Add(order);

     var request = new GetOrderByIdRequest
  {
   OrderId = order.Id
     };

        // Act
var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.True(response.IsSuccess);
  }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WithCreatedAtDate()
    {
      // Arrange
   var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
   _productDb.Add(product!);

     var (order, _) = Order.Create(customer!);
        order!.AddLine(product!, 1);
   _orderDb.Add(order);

      var request = new GetOrderByIdRequest
  {
       OrderId = order.Id
        };

     // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
   Assert.NotNull(response.Data);
   Assert.NotEqual(DateTime.MinValue, response.Data.CreatedAt);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WithUpdatedAtDate()
    {
 // Arrange
      var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

     var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

     var (order, _) = Order.Create(customer!);
        order!.AddLine(product!, 1);
   _orderDb.Add(order);

      var request = new GetOrderByIdRequest
        {
       OrderId = order.Id
  };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.NotNull(response);
     Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.NotEqual(DateTime.MinValue, response.Data.UpdatedAt);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WithCustomerReference()
    {
        // Arrange
     var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

   var (product, _) = Product.Create("Product 1", "Description", 50.00m);
      _productDb.Add(product!);

        var (order, _) = Order.Create(customer!);
   order!.AddLine(product!, 1);
      _orderDb.Add(order);

        var request = new GetOrderByIdRequest
{
      OrderId = order.Id
      };

  // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
    Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
     Assert.Equal(customer.Id, response.Data.CustomerId);
     Assert.NotNull(response.Data.Customer);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrder_WithLineItemDetails()
  {
        // Arrange
var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 25.00m);
        _productDb.Add(product!);

   var (order, _) = Order.Create(customer!);
        order!.AddLine(product!, 4);
_orderDb.Add(order);

        var request = new GetOrderByIdRequest
        {
    OrderId = order.Id
};

     // Act
  var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
  Assert.NotNull(response.Data);
    Assert.Single(response.Data.Lines);
 var line = response.Data.Lines[0];
        Assert.Equal(4, line.Quantity);
     Assert.Equal(100.00m, line.Total);
    }
}
