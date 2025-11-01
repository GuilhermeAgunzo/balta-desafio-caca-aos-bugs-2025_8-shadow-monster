using BugStore.Application.Handlers.Products;
using BugStore.Application.Requests.Products;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Products;

public class GetProductsHandlerTest
{
private readonly List<Product> _db = [];
    private readonly FakeProductRepository _repository;
    private readonly GetProductsHandler _handler;

    public GetProductsHandlerTest()
    {
        _repository = new FakeProductRepository(_db);
        _handler = new GetProductsHandler(_repository);
    }

    [Fact]
 public async Task HandleAsync_ShouldReturnEmptyList_WhenDatabaseIsEmpty()
    {
        // Arrange
        var request = new GetProductsRequest
        {
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
        Assert.Equal(0, response.TotalCount);
      Assert.Equal(1, response.CurrentPage);
 Assert.Equal(10, response.PageSize);
 Assert.Equal(0, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllProducts_WhenPageSizeIsGreaterThanCount()
    {
      // Arrange
        var (product1, _) = Product.Create("Product One", "Description 1", 10.00m);
        var (product2, _) = Product.Create("Product Two", "Description 2", 20.00m);
        var (product3, _) = Product.Create("Product Three", "Description 3", 30.00m);

    _db.Add(product1!);
        _db.Add(product2!);
  _db.Add(product3!);

        var request = new GetProductsRequest
        {
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
        Assert.Equal(3, response.Data.Count());
 Assert.Equal(3, response.TotalCount);
        Assert.Equal(1, response.CurrentPage);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(1, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFirstPage_WhenPageNumberIsOne()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
          var (product, _) = Product.Create(
            $"Product {i:D2}",
       $"Description {i}",
             i * 10.00m);
   _db.Add(product!);
        }

        var request = new GetProductsRequest
        {
   PageNumber = 1,
            PageSize = 5
     };

     // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
     Assert.NotNull(response.Data);
   Assert.Equal(5, response.Data.Count());
        Assert.Equal(10, response.TotalCount);
 Assert.Equal(1, response.CurrentPage);
   Assert.Equal(5, response.PageSize);
      Assert.Equal(2, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSecondPage_WhenPageNumberIsTwo()
  {
        // Arrange
      for (int i = 1; i <= 10; i++)
        {
 var (product, _) = Product.Create(
  $"Product {i:D2}",
$"Description {i}",
                i * 10.00m);
         _db.Add(product!);
        }

 var request = new GetProductsRequest
        {
   PageNumber = 2,
      PageSize = 5
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
     Assert.True(response.IsSuccess);
   Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(5, response.Data.Count());
Assert.Equal(10, response.TotalCount);
        Assert.Equal(2, response.CurrentPage);
    Assert.Equal(5, response.PageSize);
        Assert.Equal(2, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenPageNumberExceedsTotalPages()
    {
    // Arrange
        var (product, _) = Product.Create("Single Product", "Description", 50.00m);
        _db.Add(product!);

        var request = new GetProductsRequest
   {
          PageNumber = 5,
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
   Assert.Equal(1, response.TotalCount);
        Assert.Equal(5, response.CurrentPage);
    Assert.Equal(10, response.PageSize);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPartialPage_WhenLastPageIsNotFull()
    {
        // Arrange
 for (int i = 1; i <= 7; i++)
        {
     var (product, _) = Product.Create(
  $"Product {i}",
     $"Description {i}",
       i * 15.00m);
            _db.Add(product!);
        }

        var request = new GetProductsRequest
  {
            PageNumber = 2,
            PageSize = 5
        };

    // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
    Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count());
        Assert.Equal(7, response.TotalCount);
        Assert.Equal(2, response.CurrentPage);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(2, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSingleProduct_WhenOnlyOneExists()
  {
 // Arrange
        var (product, _) = Product.Create(
"Unique Product",
    "Unique Description",
 99.99m);
        _db.Add(product!);

        var request = new GetProductsRequest
{
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
        Assert.Single(response.Data);
        Assert.Equal(1, response.TotalCount);
    Assert.Equal(1, response.TotalPages);

        var returnedProduct = response.Data.First();
        Assert.Equal(product!.Id, returnedProduct.Id);
        Assert.Equal("Unique Product", returnedProduct.Title);
        Assert.Equal("Unique Description", returnedProduct.Description);
    Assert.Equal(99.99m, returnedProduct.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateTotalPagesCorrectly_WhenTotalCountIsDivisibleByPageSize()
    {
        // Arrange
      for (int i = 1; i <= 10; i++)
   {
            var (product, _) = Product.Create(
     $"Product {i}",
        $"Description {i}",
                i * 10.00m);
         _db.Add(product!);
        }

  var request = new GetProductsRequest
 {
            PageNumber = 1,
            PageSize = 5
        };

    // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(10, response.TotalCount);
        Assert.Equal(5, response.PageSize);
  Assert.Equal(2, response.TotalPages); // 10 / 5 = 2
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateTotalPagesCorrectly_WhenTotalCountIsNotDivisibleByPageSize()
    {
        // Arrange
        for (int i = 1; i <= 11; i++)
        {
            var (product, _) = Product.Create(
    $"Product {i}",
       $"Description {i}",
           i * 10.00m);
_db.Add(product!);
        }

        var request = new GetProductsRequest
        {
      PageNumber = 1,
            PageSize = 5
        };

   // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
 Assert.True(response.IsSuccess);
        Assert.Equal(11, response.TotalCount);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(3, response.TotalPages); // Ceiling(11 / 5) = 3
  }

    [Fact]
    public async Task HandleAsync_ShouldReturnProductsOrderedByTitle()
    {
        // Arrange
        var (product1, _) = Product.Create("Zulu Product", "Description", 10.00m);
var (product2, _) = Product.Create("Alpha Product", "Description", 20.00m);
        var (product3, _) = Product.Create("Mike Product", "Description", 30.00m);

        _db.Add(product1!);
        _db.Add(product2!);
  _db.Add(product3!);

        var request = new GetProductsRequest
        {
   PageNumber = 1,
            PageSize = 10
        };

  // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.NotNull(response);
      Assert.True(response.IsSuccess);
        Assert.Equal(3, response.Data.Count());

        var products = response.Data.ToList();
        Assert.Equal("Alpha Product", products[0].Title);
        Assert.Equal("Mike Product", products[1].Title);
        Assert.Equal("Zulu Product", products[2].Title);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotModifyDatabase_WhenRetrievingProducts()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
        var (product, _) = Product.Create(
  $"Product {i}",
           $"Description {i}",
i * 10.00m);
       _db.Add(product!);
      }

    var initialCount = _db.Count;
        var initialProducts = _db.ToList();

        var request = new GetProductsRequest
        {
            PageNumber = 1,
  PageSize = 10
     };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(initialCount, _db.Count);
 Assert.All(initialProducts, p => Assert.Contains(p, _db));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectPageData_WhenPageSizeIsOne()
  {
        // Arrange
 var (product1, _) = Product.Create("Product A", "Description", 10.00m);
        var (product2, _) = Product.Create("Product B", "Description", 20.00m);

        _db.Add(product1!);
  _db.Add(product2!);

        var request = new GetProductsRequest
{
  PageNumber = 2,
       PageSize = 1
   };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data);
        Assert.Equal(2, response.TotalCount);
 Assert.Equal(2, response.CurrentPage);
    Assert.Equal(1, response.PageSize);
    Assert.Equal(2, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllPagesData_WhenMultiplePagesExist()
    {
        // Arrange
        for (int i = 1; i <= 15; i++)
  {
         var (product, _) = Product.Create(
    $"Product {i:D2}",
  $"Description {i}",
          i * 10.00m);
            _db.Add(product!);
        }

        var pageSize = 5;
        var allProducts = new List<Product>();

        // Act - Get all pages
  for (int page = 1; page <= 3; page++)
        {
            var request = new GetProductsRequest
   {
 PageNumber = page,
          PageSize = pageSize
};

    var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
            allProducts.AddRange(response.Data!);
        }

        // Assert
        Assert.Equal(15, allProducts.Count);
        Assert.Equal(_db.Count, allProducts.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectStatusCode_OnSuccess()
    {
        // Arrange
        var (product, _) = Product.Create("Test Product", "Description", 100.00m);
        _db.Add(product!);

   var request = new GetProductsRequest
   {
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
    public async Task HandleAsync_ShouldReturnConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
          var (product, _) = Product.Create(
            $"Product {i}",
          $"Description {i}",
                i * 10.00m);
            _db.Add(product!);
        }

        var request = new GetProductsRequest
    {
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
        Assert.Equal(response1.TotalCount, response2.TotalCount);
        Assert.Equal(response1.Data!.Count(), response2.Data!.Count());
        Assert.Equal(response1.CurrentPage, response2.CurrentPage);
        Assert.Equal(response1.PageSize, response2.PageSize);
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleLargePageSize()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            var (product, _) = Product.Create(
    $"Product {i}",
  $"Description {i}",
             i * 10.00m);
       _db.Add(product!);
        }

        var request = new GetProductsRequest
        {
 PageNumber = 1,
      PageSize = 1000
      };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Data.Count());
        Assert.Equal(5, response.TotalCount);
   Assert.Equal(1, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnProductsWithAllProperties()
{
        // Arrange
   var (product, _) = Product.Create("Complete Product", "Complete Description", 125.50m);
        _db.Add(product!);

      var request = new GetProductsRequest
   {
    PageNumber = 1,
    PageSize = 10
        };

        // Act
   var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data);

        var returnedProduct = response.Data.First();
        Assert.NotEqual(Guid.Empty, returnedProduct.Id);
        Assert.Equal("Complete Product", returnedProduct.Title);
        Assert.Equal("Complete Description", returnedProduct.Description);
        Assert.Equal(125.50m, returnedProduct.Price);
      Assert.NotNull(returnedProduct.Slug);
    }
}
