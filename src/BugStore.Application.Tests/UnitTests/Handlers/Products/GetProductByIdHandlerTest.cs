using BugStore.Application.Handlers.Products;
using BugStore.Application.Requests.Products;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Products;

public class GetProductByIdHandlerTest
{
    private readonly List<Product> _db = [];
    private readonly FakeProductRepository _repository;
    private readonly GetProductByIdHandler _handler;

  public GetProductByIdHandlerTest()
    {
        _repository = new FakeProductRepository(_db);
        _handler = new GetProductByIdHandler(_repository);
    }

  [Fact]
  public async Task HandleAsync_ShouldReturnProduct_WhenIdExists()
  {
    // Arrange
        var (product, _) = Product.Create(
            "Test Product",
    "Test Description",
            99.99m);
        _db.Add(product!);

        var request = new GetProductByIdRequest
        {
          ProductId = product!.Id
      };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
 Assert.NotNull(response.Data);
        Assert.Equal(product.Id, response.Data.Id);
        Assert.Equal("Test Product", response.Data.Title);
        Assert.Equal("Test Description", response.Data.Description);
  Assert.Equal(99.99m, response.Data.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
     // Arrange
        var nonExistentId = Guid.NewGuid();
      var request = new GetProductByIdRequest
        {
     ProductId = nonExistentId
 };

        // Act
    var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
        Assert.Null(response.Data);
  Assert.NotNull(response.Messages);
    Assert.Contains("Product not found", response.Messages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectProduct_WhenMultipleProductsExist()
    {
    // Arrange
     var (product1, _) = Product.Create("Product One", "Description 1", 10.00m);
        var (product2, _) = Product.Create("Product Two", "Description 2", 20.00m);
        var (product3, _) = Product.Create("Product Three", "Description 3", 30.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);

var request = new GetProductByIdRequest
        {
        ProductId = product2!.Id
      };

        // Act
  var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(product2.Id, response.Data.Id);
        Assert.Equal("Product Two", response.Data.Title);
     Assert.Equal("Description 2", response.Data.Description);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenProductIdIsEmpty()
    {
 // Arrange
        var (product, _) = Product.Create("Test Product", "Test Description", 50.00m);
  _db.Add(product!);

      var request = new GetProductByIdRequest
  {
       ProductId = Guid.Empty
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
    public async Task HandleAsync_ShouldReturnProductWithSlug_WhenProductExists()
    {
   // Arrange
        var (product, _) = Product.Create(
     "Amazing Product",
     "Amazing Description",
     150.00m);
        _db.Add(product!);

   var request = new GetProductByIdRequest
    {
         ProductId = product!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

   // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
  Assert.NotNull(response.Data);
        Assert.Equal("amazing-product", response.Data.Slug);
    }

    [Fact]
public async Task HandleAsync_ShouldNotModifyDatabase_WhenRetrievingProduct()
{
        // Arrange
        var (product, _) = Product.Create("Read Only Test", "Description", 100.00m);
_db.Add(product!);
        var initialCount = _db.Count;

        var request = new GetProductByIdRequest
        {
            ProductId = product!.Id
        };

        // Act
  var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(initialCount, _db.Count);
        Assert.Contains(product, _db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDatabaseIsEmpty()
  {
        // Arrange
        var request = new GetProductByIdRequest
        {
            ProductId = Guid.NewGuid()
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

    [Fact]
    public async Task HandleAsync_ShouldReturnUniqueProduct_ByUniqueId()
    {
    // Arrange
      var (product1, _) = Product.Create("First Product", "Description 1", 10.00m);
        var (product2, _) = Product.Create("Second Product", "Description 2", 20.00m);

        _db.Add(product1!);
     _db.Add(product2!);

        var request = new GetProductByIdRequest
        {
 ProductId = product1!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
      Assert.NotNull(response);
        Assert.True(response.IsSuccess);
      Assert.NotNull(response.Data);
        Assert.Equal(product1.Id, response.Data.Id);
        Assert.NotEqual(product2!.Id, response.Data.Id);
        Assert.Equal("First Product", response.Data.Title);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSameProduct_WhenCalledMultipleTimes()
    {
        // Arrange
        var (product, _) = Product.Create("Consistent Product", "Description", 75.00m);
 _db.Add(product!);

        var request = new GetProductByIdRequest
  {
            ProductId = product!.Id
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
        Assert.Equal(response1.Data.Title, response2.Data.Title);
        Assert.Equal(response1.Data.Price, response2.Data.Price);
    }

 [Fact]
    public async Task HandleAsync_ShouldReturnProduct_WithCorrectStatusCode()
    {
        // Arrange
        var (product, _) = Product.Create("Status Test", "Description", 50.00m);
        _db.Add(product!);

        var request = new GetProductByIdRequest
    {
       ProductId = product!.Id
    };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnProductWithAllProperties()
    {
        // Arrange
        var (product, _) = Product.Create(
   "Complete Product",
         "Complete Description",
 123.45m);
        _db.Add(product!);

 var request = new GetProductByIdRequest
        {
            ProductId = product!.Id
        };

        // Act
    var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(product.Id, response.Data.Id);
   Assert.Equal("Complete Product", response.Data.Title);
        Assert.Equal("Complete Description", response.Data.Description);
        Assert.Equal(123.45m, response.Data.Price);
     Assert.Equal("complete-product", response.Data.Slug);
    }

  [Fact]
    public async Task HandleAsync_ShouldReturnProduct_WithDecimalPrice()
    {
        // Arrange
        var (product, _) = Product.Create("Decimal Price Product", "Description", 99.99m);
      _db.Add(product!);

    var request = new GetProductByIdRequest
        {
            ProductId = product!.Id
        };

        // Act
    var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(99.99m, response.Data.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnProduct_WithGeneratedSlug()
    {
      // Arrange
        var (product, _) = Product.Create("My New Product", "Description", 45.00m);
      _db.Add(product!);

var request = new GetProductByIdRequest
        {
            ProductId = product!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
  Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Slug);
     Assert.Equal("my-new-product", response.Data.Slug);
    }
}
