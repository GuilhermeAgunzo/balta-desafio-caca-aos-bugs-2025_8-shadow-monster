using BugStore.Application.Requests.Orders;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Repositories;

namespace BugStore.Infrastructure.Tests.UnitTests;

public class OrderUnitTest
{
    private readonly List<Order> _db = [];
    private readonly OrderFakeRepository _repository;
    private readonly Customer _customer;

    public OrderUnitTest()
    {
        _customer = Customer.Create("John Doe", "john.doe@email.com", new DateTime(1990, 1, 1)).customer!;
        _repository = new OrderFakeRepository(_db);
    }

    [Fact]
    public async Task AddAsync_ShouldAddValidOrder()
    {
        var order = Order.Create(_customer).order!;

        var result = await _repository.AddAsync(order, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Contains(order, _db);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenExists()
    {
        var order = Order.Create(_customer).order!;
        _db.Add(order);

        var result = await _repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(order.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetPagedByCustomerAsync_ShouldReturnCorrectPage()
    {
        for (int i = 0; i < 10; i++)
        {
            var order = Order.Create(_customer).order!;
            typeof(Order).GetProperty("CreatedAt")!.SetValue(order, DateTime.UtcNow.AddMinutes(-i));
            _db.Add(order);
        }

        var result = await _repository.GetPagedByCustomerAsync(_customer.Id, pageNumber: 2, pageSize: 3, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task GetPagedByCustomerAsync_ShouldReturnEmpty_WhenCustomerHasNoOrders()
    {
        var result = await _repository.GetPagedByCustomerAsync(Guid.NewGuid(), pageNumber: 1, pageSize: 5, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByCustomerName()
    {
        // Arrange
        var customer1 = Customer.Create("Alice Johnson", "alice@email.com", new DateTime(1990, 1, 1)).customer!;
  var customer2 = Customer.Create("Bob Smith", "bob@email.com", new DateTime(1991, 2, 2)).customer!;
        var customer3 = Customer.Create("Alice Brown", "alice.brown@email.com", new DateTime(1992, 3, 3)).customer!;

        var order1 = Order.Create(customer1).order!;
     var order2 = Order.Create(customer2).order!;
        var order3 = Order.Create(customer3).order!;

        _db.Add(order1);
        _db.Add(order2);
     _db.Add(order3);

   var request = new GetOrdersRequest
        {
CustomerName = "Alice",
            PageNumber = 1,
   PageSize = 10
  };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, o => Assert.Contains("Alice", o.Customer.Name));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByCustomerEmail()
    {
        // Arrange
  var customer1 = Customer.Create("Customer 1", "user@gmail.com", new DateTime(1990, 1, 1)).customer!;
     var customer2 = Customer.Create("Customer 2", "user@yahoo.com", new DateTime(1991, 2, 2)).customer!;
     var customer3 = Customer.Create("Customer 3", "admin@gmail.com", new DateTime(1992, 3, 3)).customer!;

        var order1 = Order.Create(customer1).order!;
 var order2 = Order.Create(customer2).order!;
        var order3 = Order.Create(customer3).order!;

        _db.Add(order1);
        _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
        {
CustomerEmail = "gmail",
    PageNumber = 1,
       PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
    Assert.Equal(2, result.TotalCount);
 Assert.All(result.Items, o => Assert.Contains("gmail", o.Customer.Email));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByCustomerPhone()
    {
        // Arrange
     var customer1 = Customer.Create("Customer 1", "c1@email.com", new DateTime(1990, 1, 1), "11999999999").customer!;
        var customer2 = Customer.Create("Customer 2", "c2@email.com", new DateTime(1991, 2, 2), "21888888888").customer!;
        var customer3 = Customer.Create("Customer 3", "c3@email.com", new DateTime(1992, 3, 3), "11777777777").customer!;

        var order1 = Order.Create(customer1).order!;
     var order2 = Order.Create(customer2).order!;
        var order3 = Order.Create(customer3).order!;

  _db.Add(order1);
   _db.Add(order2);
     _db.Add(order3);

var request = new GetOrdersRequest
        {
  CustomerPhone = "11",
      PageNumber = 1,
  PageSize = 10
    };

  // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.True(result.Success);
     Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
     Assert.All(result.Items, o => Assert.Contains("11", o.Customer.Phone ?? ""));
    }

  [Fact]
    public async Task GetPagedAsync_ShouldFilterByProductTitle()
    {
    // Arrange
     var product1 = Product.Create("Gaming Mouse", "Wireless mouse", 99.90m).product!;
        var product2 = Product.Create("Gaming Keyboard", "Mechanical keyboard", 199.90m).product!;
  var product3 = Product.Create("Office Chair", "Ergonomic chair", 299.90m).product!;

var order1 = Order.Create(_customer).order!;
        order1.AddLine(product1, 1);

 var order2 = Order.Create(_customer).order!;
        order2.AddLine(product2, 1);

        var order3 = Order.Create(_customer).order!;
        order3.AddLine(product3, 1);

        _db.Add(order1);
        _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
 {
        ProductTitle = "Gaming",
          PageNumber = 1,
    PageSize = 10
        };

 // Act
var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
 Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
 Assert.Equal(2, result.TotalCount);
    Assert.All(result.Items, o => 
            Assert.True(o.Lines.Any(l => l.Product.Title.Contains("Gaming"))));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByProductDescription()
    {
     // Arrange
var product1 = Product.Create("Mouse", "Wireless technology", 99.90m).product!;
        var product2 = Product.Create("Keyboard", "Wired connection", 199.90m).product!;
    var product3 = Product.Create("Headset", "Wireless technology", 149.90m).product!;

      var order1 = Order.Create(_customer).order!;
        order1.AddLine(product1, 1);

        var order2 = Order.Create(_customer).order!;
   order2.AddLine(product2, 1);

      var order3 = Order.Create(_customer).order!;
        order3.AddLine(product3, 1);

        _db.Add(order1);
        _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
        {
            ProductDescription = "Wireless",
         PageNumber = 1,
            PageSize = 10
        };

        // Act
 var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

      // Assert
        Assert.True(result.Success);
   Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, o => 
        Assert.True(o.Lines.Any(l => l.Product.Description.Contains("Wireless"))));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByProductSlug()
    {
        // Arrange
        var product1 = Product.Create("Gaming Mouse", "Description 1", 99.90m).product!;
 var product2 = Product.Create("Gaming Keyboard", "Description 2", 199.90m).product!;
        var product3 = Product.Create("Office Desk", "Description 3", 299.90m).product!;

    var order1 = Order.Create(_customer).order!;
        order1.AddLine(product1, 1);

        var order2 = Order.Create(_customer).order!;
        order2.AddLine(product2, 1);

    var order3 = Order.Create(_customer).order!;
        order3.AddLine(product3, 1);

        _db.Add(order1);
     _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
        {
 ProductSlug = "gaming",
      PageNumber = 1,
      PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

  // Assert
      Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, o => 
    Assert.True(o.Lines.Any(l => l.Product.Slug.Contains("gaming"))));
    }

    [Fact]
public async Task GetPagedAsync_ShouldFilterByProductPriceStart()
    {
        // Arrange
        var product1 = Product.Create("Product A", "Description", 50.00m).product!;
  var product2 = Product.Create("Product B", "Description", 100.00m).product!;
    var product3 = Product.Create("Product C", "Description", 150.00m).product!;

        var order1 = Order.Create(_customer).order!;
        order1.AddLine(product1, 1);

        var order2 = Order.Create(_customer).order!;
   order2.AddLine(product2, 1);

    var order3 = Order.Create(_customer).order!;
   order3.AddLine(product3, 1);

        _db.Add(order1);
        _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
        {
        ProductPriceStart = 100.00m,
            PageNumber = 1,
   PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, o => 
            Assert.True(o.Lines.Any(l => l.Product.Price >= 100.00m)));
    }

    [Fact]
 public async Task GetPagedAsync_ShouldFilterByProductPriceEnd()
    {
   // Arrange
     var product1 = Product.Create("Product A", "Description", 50.00m).product!;
        var product2 = Product.Create("Product B", "Description", 100.00m).product!;
   var product3 = Product.Create("Product C", "Description", 150.00m).product!;

   var order1 = Order.Create(_customer).order!;
        order1.AddLine(product1, 1);

     var order2 = Order.Create(_customer).order!;
        order2.AddLine(product2, 1);

        var order3 = Order.Create(_customer).order!;
        order3.AddLine(product3, 1);

        _db.Add(order1);
        _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
        {
      ProductPriceEnd = 100.00m,
   PageNumber = 1,
        PageSize = 10
        };

   // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, o => 
            Assert.True(o.Lines.Any(l => l.Product.Price <= 100.00m)));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByProductPriceRange()
    {
    // Arrange
        var product1 = Product.Create("Product A", "Description", 50.00m).product!;
   var product2 = Product.Create("Product B", "Description", 100.00m).product!;
  var product3 = Product.Create("Product C", "Description", 150.00m).product!;
     var product4 = Product.Create("Product D", "Description", 200.00m).product!;

   var order1 = Order.Create(_customer).order!;
     order1.AddLine(product1, 1);

     var order2 = Order.Create(_customer).order!;
    order2.AddLine(product2, 1);

var order3 = Order.Create(_customer).order!;
        order3.AddLine(product3, 1);

        var order4 = Order.Create(_customer).order!;
      order4.AddLine(product4, 1);

        _db.Add(order1);
      _db.Add(order2);
        _db.Add(order3);
 _db.Add(order4);

        var request = new GetOrdersRequest
        {
  ProductPriceStart = 100.00m,
   ProductPriceEnd = 150.00m,
        PageNumber = 1,
            PageSize = 10
 };

        // Act
  var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

     // Assert
     Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
      Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, o => 
            Assert.True(o.Lines.Any(l => l.Product.Price >= 100.00m && l.Product.Price <= 150.00m)));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByCreatedAtStart()
    {
        // Arrange
 var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var order1 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order1, baseDate.AddDays(-5));

   var order2 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order2, baseDate);

        var order3 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order3, baseDate.AddDays(5));

        _db.Add(order1);
        _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
        {
       CreatedAtStart = baseDate,
         PageNumber = 1,
     PageSize = 10
     };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

    // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
      Assert.All(result.Items, o => Assert.True(o.CreatedAt >= baseDate));
    }

  [Fact]
    public async Task GetPagedAsync_ShouldFilterByCreatedAtEnd()
    {
    // Arrange
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var order1 = Order.Create(_customer).order!;
 typeof(Order).GetProperty("CreatedAt")!.SetValue(order1, baseDate.AddDays(-5));

        var order2 = Order.Create(_customer).order!;
      typeof(Order).GetProperty("CreatedAt")!.SetValue(order2, baseDate);

     var order3 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order3, baseDate.AddDays(5));

        _db.Add(order1);
        _db.Add(order2);
    _db.Add(order3);

        var request = new GetOrdersRequest
     {
            CreatedAtEnd = baseDate,
  PageNumber = 1,
     PageSize = 10
        };

        // Act
   var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
     Assert.All(result.Items, o => Assert.True(o.CreatedAt <= baseDate));
}

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByCreatedAtRange()
    {
    // Arrange
     var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
   var endDate = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc);

        var order1 = Order.Create(_customer).order!;
  typeof(Order).GetProperty("CreatedAt")!.SetValue(order1, startDate.AddDays(-5));

  var order2 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order2, startDate.AddDays(3));

     var order3 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order3, endDate.AddDays(5));

        _db.Add(order1);
      _db.Add(order2);
        _db.Add(order3);

   var request = new GetOrdersRequest
        {
      CreatedAtStart = startDate,
    CreatedAtEnd = endDate,
  PageNumber = 1,
      PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.True(result.Success);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.All(result.Items, o => 
 Assert.True(o.CreatedAt >= startDate && o.CreatedAt <= endDate));
    }

  [Fact]
    public async Task GetPagedAsync_ShouldFilterByUpdatedAtStart()
    {
        // Arrange
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var order1 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("UpdatedAt")!.SetValue(order1, baseDate.AddDays(-5));

        var order2 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("UpdatedAt")!.SetValue(order2, baseDate);

 var order3 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("UpdatedAt")!.SetValue(order3, baseDate.AddDays(5));

   _db.Add(order1);
        _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
        {
          UpdatedAtStart = baseDate,
            PageNumber = 1,
       PageSize = 10
 };

        // Act
    var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, o => Assert.True(o.UpdatedAt >= baseDate));
    }

 [Fact]
    public async Task GetPagedAsync_ShouldFilterByUpdatedAtEnd()
    {
     // Arrange
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

  var order1 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("UpdatedAt")!.SetValue(order1, baseDate.AddDays(-5));

        var order2 = Order.Create(_customer).order!;
 typeof(Order).GetProperty("UpdatedAt")!.SetValue(order2, baseDate);

        var order3 = Order.Create(_customer).order!;
   typeof(Order).GetProperty("UpdatedAt")!.SetValue(order3, baseDate.AddDays(5));

        _db.Add(order1);
    _db.Add(order2);
  _db.Add(order3);

  var request = new GetOrdersRequest
        {
      UpdatedAtEnd = baseDate,
            PageNumber = 1,
            PageSize = 10
 };

     // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
     Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, o => Assert.True(o.UpdatedAt <= baseDate));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByMultipleFields()
    {
        // Arrange
    var customer1 = Customer.Create("Alice Johnson", "alice@gmail.com", new DateTime(1990, 1, 1), "11999999999").customer!;
  var customer2 = Customer.Create("Bob Smith", "bob@yahoo.com", new DateTime(1991, 2, 2), "21888888888").customer!;

     var product1 = Product.Create("Gaming Mouse", "Wireless gaming", 99.90m).product!;
        var product2 = Product.Create("Office Chair", "Ergonomic design", 299.90m).product!;

        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var order1 = Order.Create(customer1).order!;
        order1.AddLine(product1, 1);
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order1, baseDate);

        var order2 = Order.Create(customer2).order!;
        order2.AddLine(product2, 1);
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order2, baseDate.AddDays(5));

        var order3 = Order.Create(customer1).order!;
        order3.AddLine(product2, 1);
     typeof(Order).GetProperty("CreatedAt")!.SetValue(order3, baseDate.AddDays(-5));

   _db.Add(order1);
     _db.Add(order2);
        _db.Add(order3);

 var request = new GetOrdersRequest
        {
      CustomerName = "Alice",
     CustomerEmail = "gmail",
            ProductTitle = "Gaming",
            CreatedAtStart = baseDate.AddDays(-1),
            PageNumber = 1,
    PageSize = 10
   };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.True(result.Success);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        var order = result.Items.First();
        Assert.Contains("Alice", order.Customer.Name);
  Assert.Contains("gmail", order.Customer.Email);
        Assert.True(order.Lines.Any(l => l.Product.Title.Contains("Gaming")));
  Assert.True(order.CreatedAt >= baseDate.AddDays(-1));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEmpty_WhenNoMatchesFound()
    {
        // Arrange
        var order = Order.Create(_customer).order!;
        _db.Add(order);

        var request = new GetOrdersRequest
     {
        CustomerName = "NonExistentCustomer",
       PageNumber = 1,
    PageSize = 10
     };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

  // Assert
        Assert.True(result.Success);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldIgnoreNullFilters()
    {
        // Arrange
  var order1 = Order.Create(_customer).order!;
        var order2 = Order.Create(_customer).order!;

      _db.Add(order1);
        _db.Add(order2);

      var request = new GetOrdersRequest
     {
    CustomerName = null,
    CustomerEmail = null,
            CustomerPhone = null,
            ProductTitle = null,
      ProductDescription = null,
         ProductSlug = null,
            ProductPriceStart = null,
      ProductPriceEnd = null,
            CreatedAtStart = null,
       CreatedAtEnd = null,
  UpdatedAtStart = null,
      UpdatedAtEnd = null,
         PageNumber = 1,
   PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
    Assert.True(result.Success);
   Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldIgnoreEmptyStringFilters()
    {
        // Arrange
        var order1 = Order.Create(_customer).order!;
   var order2 = Order.Create(_customer).order!;

   _db.Add(order1);
      _db.Add(order2);

        var request = new GetOrdersRequest
   {
  CustomerName = "",
            CustomerEmail = "   ",
  ProductTitle = "",
         PageNumber = 1,
       PageSize = 10
        };

 // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
public async Task GetPagedAsync_ShouldApplyPaginationWithFilters()
    {
        // Arrange
        for (int i = 0; i < 20; i++)
  {
var order = Order.Create(_customer).order!;
    typeof(Order).GetProperty("CreatedAt")!.SetValue(order, DateTime.UtcNow.AddMinutes(-i));
   _db.Add(order);
        }

        var request = new GetOrdersRequest
 {
            CustomerName = "John",
            PageNumber = 2,
            PageSize = 5
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(5, result.Items.Count());
      Assert.Equal(20, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
        Assert.Equal(2, result.CurrentPage);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnOrderedByCreatedAtDescending()
    {
        // Arrange
   var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var order1 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order1, baseDate);

        var order2 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order2, baseDate.AddDays(5));

      var order3 = Order.Create(_customer).order!;
        typeof(Order).GetProperty("CreatedAt")!.SetValue(order3, baseDate.AddDays(10));

    _db.Add(order1);
        _db.Add(order2);
        _db.Add(order3);

        var request = new GetOrdersRequest
      {
  PageNumber = 1,
            PageSize = 10
   };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

    // Assert
        Assert.True(result.Success);
    Assert.Equal(3, result.Items.Count());
        var orders = result.Items.ToList();
        Assert.True(orders[0].CreatedAt >= orders[1].CreatedAt);
  Assert.True(orders[1].CreatedAt >= orders[2].CreatedAt);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterOrderWithMultipleProducts()
  {
        // Arrange
        var product1 = Product.Create("Gaming Mouse", "Description 1", 99.90m).product!;
        var product2 = Product.Create("Office Chair", "Description 2", 299.90m).product!;
        var product3 = Product.Create("Gaming Keyboard", "Description 3", 199.90m).product!;

 var order1 = Order.Create(_customer).order!;
    order1.AddLine(product1, 1);
        order1.AddLine(product2, 1);

        var order2 = Order.Create(_customer).order!;
      order2.AddLine(product3, 1);

        _db.Add(order1);
        _db.Add(order2);

        var request = new GetOrdersRequest
        {
            ProductTitle = "Gaming",
      PageNumber = 1,
PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, o => 
            Assert.True(o.Lines.Any(l => l.Product.Title.Contains("Gaming"))));
    }
}
