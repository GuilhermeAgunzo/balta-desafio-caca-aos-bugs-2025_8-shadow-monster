using BugStore.Application.Handlers.Orders;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Orders;

public class GetOrdersByCustomerHandlerTest
{
  private readonly List<Order> _orderDb = [];
    private readonly List<Customer> _customerDb = [];
    private readonly List<Product> _productDb = [];
    private readonly FakeOrderRepository _orderRepository;
    private readonly FakeCustomerRepository _customerRepository;
 private readonly FakeProductRepository _productRepository;
    private readonly GetOrdersByCustomerHandler _handler;

public GetOrdersByCustomerHandlerTest()
    {
    _orderRepository = new FakeOrderRepository(_orderDb);
        _customerRepository = new FakeCustomerRepository(_customerDb);
        _productRepository = new FakeProductRepository(_productDb);
      _handler = new GetOrdersByCustomerHandler(_orderRepository);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenCustomerHasNoOrders()
    {
   // Arrange
     var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var request = new GetOrdersByCustomerRequest
      {
    CustomerId = customer!.Id,
        PageNumber = 1,
         PageSize = 10
  };

     // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

  // Assert
        Assert.NotNull(response);
  Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
   Assert.NotNull(response.Data);
Assert.Empty(response.Data);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrders_ForSpecificCustomer()
    {
        // Arrange
     var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

   var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

        var (order1, _) = Order.Create(customer!);
 order1!.AddLine(product!, 1);
   var (order2, _) = Order.Create(customer!);
   order2!.AddLine(product!, 2);

      _orderDb.Add(order1);
        _orderDb.Add(order2);

    var request = new GetOrdersByCustomerRequest
        {
       CustomerId = customer.Id,
  PageNumber = 1,
PageSize = 10
        };

   // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
   Assert.True(response.IsSuccess);
  Assert.Equal(200, response.StatusCode);
   Assert.NotNull(response.Data);
    Assert.Equal(2, response.Data.Count());
    }

 [Fact]
    public async Task HandleAsync_ShouldReturnOnlyOrdersForSpecifiedCustomer()
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
        order2!.AddLine(product!, 1);
   var (order3, _) = Order.Create(customer1!);
        order3!.AddLine(product!, 1);

        _orderDb.Add(order1);
        _orderDb.Add(order2);
  _orderDb.Add(order3);

   var request = new GetOrdersByCustomerRequest
    {
       CustomerId = customer1.Id,
       PageNumber = 1,
    PageSize = 10
        };

 // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

   // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
   Assert.Equal(2, response.Data.Count());
     Assert.All(response.Data, o => Assert.Equal(customer1.Id, o.CustomerId));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrdersOrderedByCreatedAtDescending()
  {
        // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

   var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

     // Create orders with slight delays to ensure different timestamps
   var (order1, _) = Order.Create(customer!);
        order1!.AddLine(product!, 1);
     _orderDb.Add(order1);

  await Task.Delay(10);

   var (order2, _) = Order.Create(customer!);
        order2!.AddLine(product!, 1);
  _orderDb.Add(order2);

        await Task.Delay(10);

        var (order3, _) = Order.Create(customer!);
        order3!.AddLine(product!, 1);
        _orderDb.Add(order3);

      var request = new GetOrdersByCustomerRequest
      {
CustomerId = customer.Id,
            PageNumber = 1,
       PageSize = 10
        };

  // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
  Assert.True(response.IsSuccess);
   var orders = response.Data!.ToList();
   Assert.Equal(3, orders.Count);
   // Most recent order should be first
   Assert.True(orders[0].CreatedAt >= orders[1].CreatedAt);
        Assert.True(orders[1].CreatedAt >= orders[2].CreatedAt);
    }

  [Fact]
 public async Task HandleAsync_ShouldReturnFirstPage_WhenPageNumberIsOne()
    {
        // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

    var (product, _) = Product.Create("Product 1", "Description", 50.00m);
 _productDb.Add(product!);

        for (int i = 0; i < 10; i++)
        {
   var (order, _) = Order.Create(customer!);
    order!.AddLine(product!, 1);
            _orderDb.Add(order);
        }

        var request = new GetOrdersByCustomerRequest
      {
     CustomerId = customer.Id,
    PageNumber = 1,
      PageSize = 5
     };

   // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
   Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(5, response.Data.Count());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSecondPage_WhenPageNumberIsTwo()
    {
   // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

      var (product, _) = Product.Create("Product 1", "Description", 50.00m);
   _productDb.Add(product!);

        for (int i = 0; i < 10; i++)
        {
       var (order, _) = Order.Create(customer!);
         order!.AddLine(product!, 1);
        _orderDb.Add(order);
   }

        var request = new GetOrdersByCustomerRequest
        {
            CustomerId = customer.Id,
       PageNumber = 2,
   PageSize = 5
  };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

   // Assert
   Assert.NotNull(response);
 Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(5, response.Data.Count());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenPageNumberExceedsAvailablePages()
    {
      // Arrange
   var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

     var (product, _) = Product.Create("Product 1", "Description", 50.00m);
  _productDb.Add(product!);

    var (order, _) = Order.Create(customer!);
        order!.AddLine(product!, 1);
        _orderDb.Add(order);

   var request = new GetOrdersByCustomerRequest
   {
            CustomerId = customer.Id,
     PageNumber = 5,
       PageSize = 10
        };

        // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
   Assert.Empty(response.Data);
    }

  [Fact]
    public async Task HandleAsync_ShouldReturnPartialPage_WhenLastPageIsNotFull()
    {
        // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

   for (int i = 0; i < 7; i++)
   {
       var (order, _) = Order.Create(customer!);
   order!.AddLine(product!, 1);
       _orderDb.Add(order);
  }

        var request = new GetOrdersByCustomerRequest
 {
     CustomerId = customer.Id,
     PageNumber = 2,
   PageSize = 5
     };

        // Act
var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

   // Assert
   Assert.NotNull(response);
   Assert.True(response.IsSuccess);
   Assert.NotNull(response.Data);
   Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectStatusCode_OnSuccess()
    {
        // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

   var request = new GetOrdersByCustomerRequest
        {
   CustomerId = customer.Id,
       PageNumber = 1,
       PageSize = 10
        };

      // Act
      var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

  // Assert
        Assert.NotNull(response);
   Assert.Equal(200, response.StatusCode);
        Assert.True(response.IsSuccess);
    }

    [Fact]
 public async Task HandleAsync_ShouldNotModifyDatabase_WhenRetrievingOrders()
    {
        // Arrange
     var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

 var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

        for (int i = 0; i < 5; i++)
        {
 var (order, _) = Order.Create(customer!);
    order!.AddLine(product!, 1);
      _orderDb.Add(order);
     }

        var initialCount = _orderDb.Count;

        var request = new GetOrdersByCustomerRequest
        {
       CustomerId = customer.Id,
    PageNumber = 1,
        PageSize = 10
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
        Assert.True(response.IsSuccess);
   Assert.Equal(initialCount, _orderDb.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

   var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

        for (int i = 0; i < 3; i++)
  {
       var (order, _) = Order.Create(customer!);
            order!.AddLine(product!, 1);
            _orderDb.Add(order);
   }

 var request = new GetOrdersByCustomerRequest
   {
   CustomerId = customer.Id,
            PageNumber = 1,
     PageSize = 10
    };

        // Act
        var response1 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
        var response2 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

   // Assert
   Assert.NotNull(response1);
   Assert.NotNull(response2);
        Assert.True(response1.IsSuccess);
   Assert.True(response2.IsSuccess);
     Assert.Equal(response1.Data!.Count(), response2.Data!.Count());
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleLargePageSize()
    {
  // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

   for (int i = 0; i < 5; i++)
        {
            var (order, _) = Order.Create(customer!);
            order!.AddLine(product!, 1);
   _orderDb.Add(order);
        }

        var request = new GetOrdersByCustomerRequest
        {
       CustomerId = customer.Id,
       PageNumber = 1,
PageSize = 1000
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
   Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Data!.Count());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrders_WithAllLineItems()
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

   var request = new GetOrdersByCustomerRequest
{
    CustomerId = customer.Id,
       PageNumber = 1,
  PageSize = 10
        };

     // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

    // Assert
        Assert.NotNull(response);
 Assert.True(response.IsSuccess);
    Assert.Single(response.Data!);
        var returnedOrder = response.Data.First();
   Assert.Equal(2, returnedOrder.Lines.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenCustomerIdDoesNotExist()
    {
        // Arrange
     var nonExistentCustomerId = Guid.NewGuid();
        var request = new GetOrdersByCustomerRequest
     {
            CustomerId = nonExistentCustomerId,
            PageNumber = 1,
       PageSize = 10
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

    // Assert
        Assert.NotNull(response);
   Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
    }
}
