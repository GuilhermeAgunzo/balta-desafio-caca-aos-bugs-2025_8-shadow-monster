using BugStore.Application.Handlers.Orders;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Orders;

public class CreateOrderHandlerTest
{
    private readonly List<Order> _orderDb = [];
    private readonly List<Customer> _customerDb = [];
 private readonly List<Product> _productDb = [];
    private readonly FakeOrderRepository _orderRepository;
    private readonly FakeCustomerRepository _customerRepository;
    private readonly FakeProductRepository _productRepository;
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTest()
    {
        _orderRepository = new FakeOrderRepository(_orderDb);
        _customerRepository = new FakeCustomerRepository(_customerDb);
  _productRepository = new FakeProductRepository(_productDb);
 _handler = new CreateOrderHandler(_orderRepository, _customerRepository, _productRepository);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateOrder_WhenRequestIsValid()
    {
    // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

var (product1, _) = Product.Create("Product 1", "Description 1", 50.00m);
    var (product2, _) = Product.Create("Product 2", "Description 2", 100.00m);
      _productDb.Add(product1!);
 _productDb.Add(product2!);

        var request = new CreateOrderRequest
  {
CustomerId = customer!.Id,
      OrderLines =
            [
         new OrderLineDTO { ProductId = product1!.Id, Quantity = 2 },
      new OrderLineDTO { ProductId = product2!.Id, Quantity = 1 }
       ]
        };

  // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
     Assert.NotNull(response);
    Assert.True(response.IsSuccess);
Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(customer.Id, response.Data.CustomerId);
     Assert.Equal(2, response.Data.Lines.Count);
        Assert.Single(_orderDb);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenCustomerDoesNotExist()
    {
        // Arrange
      var nonExistentCustomerId = Guid.NewGuid();
    var request = new CreateOrderRequest
        {
    CustomerId = nonExistentCustomerId,
       OrderLines = []
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
    public async Task HandleAsync_ShouldReturnError_WhenProductDoesNotExist()
 {
// Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var nonExistentProductId = Guid.NewGuid();
        var request = new CreateOrderRequest
   {
            CustomerId = customer!.Id,
   OrderLines =
            [
  new OrderLineDTO { ProductId = nonExistentProductId, Quantity = 1 }
            ]
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
  public async Task HandleAsync_ShouldReturnError_WhenOrderLinesAreEmpty()
    {
        // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

        var request = new CreateOrderRequest
        {
     CustomerId = customer!.Id,
       OrderLines = []
    };

    // Act
 var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
     Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains(response.Messages, m => m.Contains("Order Lines"));
        Assert.Empty(_orderDb);
    }

    [Fact]
public async Task HandleAsync_ShouldReturnError_WhenQuantityIsZero()
    {
// Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
     _customerDb.Add(customer!);

     var (product, _) = Product.Create("Product 1", "Description", 50.00m);
  _productDb.Add(product!);

      var request = new CreateOrderRequest
        {
  CustomerId = customer!.Id,
  OrderLines =
       [
                new OrderLineDTO { ProductId = product!.Id, Quantity = 0 }
       ]
   };

        // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

  // Assert
   Assert.NotNull(response);
        Assert.False(response.IsSuccess);
  Assert.Equal(400, response.StatusCode);
        Assert.Null(response.Data);
     Assert.NotNull(response.Messages);
    Assert.Empty(_orderDb);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenQuantityIsNegative()
    {
        // Arrange
  var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
 _customerDb.Add(customer!);

   var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

        var request = new CreateOrderRequest
        {
    CustomerId = customer!.Id,
   OrderLines =
 [
    new OrderLineDTO { ProductId = product!.Id, Quantity = -1 }
       ]
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
    public async Task HandleAsync_ShouldCreateOrder_WithSingleProduct()
    {
   // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

 var request = new CreateOrderRequest
        {
            CustomerId = customer!.Id,
        OrderLines =
        [
  new OrderLineDTO { ProductId = product!.Id, Quantity = 3 }
   ]
        };

        // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
      Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
 Assert.Single(response.Data.Lines);
        Assert.Equal(3, response.Data.Lines[0].Quantity);
        Assert.Equal(150.00m, response.Data.Lines[0].Total);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateOrder_WithMultipleProducts()
    {
 // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
     _customerDb.Add(customer!);

 var (product1, _) = Product.Create("Product 1", "Description 1", 10.00m);
      var (product2, _) = Product.Create("Product 2", "Description 2", 20.00m);
        var (product3, _) = Product.Create("Product 3", "Description 3", 30.00m);
 _productDb.Add(product1!);
        _productDb.Add(product2!);
        _productDb.Add(product3!);

        var request = new CreateOrderRequest
        {
 CustomerId = customer!.Id,
  OrderLines =
            [
         new OrderLineDTO { ProductId = product1!.Id, Quantity = 1 },
     new OrderLineDTO { ProductId = product2!.Id, Quantity = 2 },
        new OrderLineDTO { ProductId = product3!.Id, Quantity = 3 }
    ]
   };

     // Act
 var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Lines.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateLineTotal_Correctly()
    {
   // Arrange
   var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 25.50m);
        _productDb.Add(product!);

   var request = new CreateOrderRequest
        {
CustomerId = customer!.Id,
     OrderLines =
        [
         new OrderLineDTO { ProductId = product!.Id, Quantity = 4 }
   ]
};

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
 Assert.Equal(102.00m, response.Data.Lines[0].Total);
}

    [Fact]
public async Task HandleAsync_ShouldGenerateUniqueOrderId()
    {
        // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

 var request = new CreateOrderRequest
     {
  CustomerId = customer!.Id,
            OrderLines =
     [
     new OrderLineDTO { ProductId = product!.Id, Quantity = 1 }
    ]
        };

        // Act
var response1 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
      var response2 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

    // Assert
Assert.True(response1.IsSuccess);
        Assert.True(response2.IsSuccess);
   Assert.NotEqual(response1.Data!.Id, response2.Data!.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldPersistOrder_InDatabase()
    {
 // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
  _productDb.Add(product!);

     var request = new CreateOrderRequest
        {
       CustomerId = customer!.Id,
   OrderLines =
          [
       new OrderLineDTO { ProductId = product!.Id, Quantity = 2 }
            ]
  };

  // Act
     var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
      Assert.Single(_orderDb);
        var savedOrder = _orderDb.First();
        Assert.Equal(response.Data!.Id, savedOrder.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldSetCreatedAt_WhenOrderIsCreated()
    {
     // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
    _productDb.Add(product!);

        var request = new CreateOrderRequest
        {
        CustomerId = customer!.Id,
    OrderLines =
        [
       new OrderLineDTO { ProductId = product!.Id, Quantity = 1 }
   ]
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
    public async Task HandleAsync_ShouldReturnCorrectStatusCode_OnSuccess()
    {
     // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
   _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
        _productDb.Add(product!);

        var request = new CreateOrderRequest
        {
       CustomerId = customer!.Id,
        OrderLines =
  [
     new OrderLineDTO { ProductId = product!.Id, Quantity = 1 }
     ]
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
  Assert.Equal(200, response.StatusCode);
        Assert.True(response.IsSuccess);
  }

    [Fact]
    public async Task HandleAsync_ShouldCreateOrder_WithLargeQuantity()
    {
   // Arrange
        var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
  _customerDb.Add(customer!);

      var (product, _) = Product.Create("Product 1", "Description", 10.00m);
        _productDb.Add(product!);

        var request = new CreateOrderRequest
        {
       CustomerId = customer!.Id,
OrderLines =
  [
       new OrderLineDTO { ProductId = product!.Id, Quantity = 100 }
      ]
   };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
        Assert.NotNull(response);
     Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(1000.00m, response.Data.Lines[0].Total);
    }

  [Fact]
    public async Task HandleAsync_ShouldHandleMultipleOrdersForSameCustomer()
{
        // Arrange
    var (customer, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
 _customerDb.Add(customer!);

        var (product, _) = Product.Create("Product 1", "Description", 50.00m);
 _productDb.Add(product!);

     var request = new CreateOrderRequest
        {
         CustomerId = customer!.Id,
            OrderLines =
            [
      new OrderLineDTO { ProductId = product!.Id, Quantity = 1 }
     ]
      };

        // Act
        var response1 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
   var response2 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response1.IsSuccess);
        Assert.True(response2.IsSuccess);
        Assert.Equal(2, _orderDb.Count);
        Assert.All(_orderDb, o => Assert.Equal(customer.Id, o.CustomerId));
    }
}
