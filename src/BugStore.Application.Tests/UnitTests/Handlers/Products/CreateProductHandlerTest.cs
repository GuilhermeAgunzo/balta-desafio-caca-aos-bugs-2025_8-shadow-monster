using BugStore.Application.Handlers.Products;
using BugStore.Application.Requests.Products;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Products;

public class CreateProductHandlerTest
{
    private readonly List<Product> _db = [];
    private readonly FakeProductRepository _repository;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTest()
    {
_repository = new FakeProductRepository(_db);
      _handler = new CreateProductHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateProduct_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateProductRequest
        {
      Title = "Test Product",
    Description = "Test Description",
      Price = 99.99m
        };

    // Act
      var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal("Test Product", response.Data.Title);
   Assert.Equal("Test Description", response.Data.Description);
        Assert.Equal(99.99m, response.Data.Price);
        Assert.Equal("test-product", response.Data.Slug);
        Assert.Single(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenTitleIsEmpty()
    {
        // Arrange
   var request = new CreateProductRequest
        {
   Title = "",
          Description = "Test Description",
    Price = 99.99m
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
Assert.Null(response.Data);
      Assert.NotNull(response.Messages);
        Assert.Contains(response.Messages, m => m.Contains("Title"));
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenDescriptionIsEmpty()
    {
        // Arrange
        var request = new CreateProductRequest
    {
Title = "Test Product",
            Description = "",
        Price = 99.99m
        };

        // Act
     var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
   Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains(response.Messages, m => m.Contains("Description"));
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenPriceIsZero()
    {
   // Arrange
        var request = new CreateProductRequest
        {
  Title = "Test Product",
            Description = "Test Description",
            Price = 0m
        };

  // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
 Assert.Equal(400, response.StatusCode);
        Assert.Null(response.Data);
   Assert.NotNull(response.Messages);
        Assert.Contains(response.Messages, m => m.Contains("Price"));
        Assert.Empty(_db);
  }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenPriceIsNegative()
    {
        // Arrange
        var request = new CreateProductRequest
        {
  Title = "Test Product",
     Description = "Test Description",
            Price = -10m
        };

      // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
     Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
        Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains(response.Messages, m => m.Contains("Price"));
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldGenerateSlug_FromTitle()
    {
 // Arrange
        var request = new CreateProductRequest
        {
       Title = "Amazing Product Name",
        Description = "Test Description",
            Price = 49.99m
};

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal("amazing-product-name", response.Data.Slug);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateMultipleProducts_WithDifferentData()
    {
        // Arrange
        var request1 = new CreateProductRequest
        {
 Title = "Product 1",
         Description = "Description 1",
         Price = 10.00m
        };
        var request2 = new CreateProductRequest
        {
            Title = "Product 2",
            Description = "Description 2",
   Price = 20.00m
        };

        // Act
    var response1 = await _handler.HandleAsync(request1, TestContext.Current.CancellationToken);
     var response2 = await _handler.HandleAsync(request2, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response1);
    Assert.NotNull(response2);
Assert.True(response1.IsSuccess);
        Assert.True(response2.IsSuccess);
        Assert.Equal(2, _db.Count);
   Assert.NotEqual(response1.Data!.Id, response2.Data!.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldGenerateUniqueId_ForEachProduct()
    {
        // Arrange
        var request = new CreateProductRequest
  {
    Title = "Test Product",
          Description = "Test Description",
Price = 99.99m
    };

        // Act
        var response1 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
        var response2 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response1);
        Assert.NotNull(response2);
        Assert.True(response1.IsSuccess);
  Assert.True(response2.IsSuccess);
        Assert.NotEqual(response1.Data!.Id, response2.Data!.Id);
Assert.NotEqual(Guid.Empty, response1.Data.Id);
        Assert.NotEqual(Guid.Empty, response2.Data.Id);
    }

[Fact]
    public async Task HandleAsync_ShouldCreateProduct_WithDecimalPrice()
    {
        // Arrange
        var request = new CreateProductRequest
   {
      Title = "Expensive Product",
         Description = "Premium quality",
            Price = 1234.56m
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
    Assert.Equal(1234.56m, response.Data.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateProduct_WithLongDescription()
    {
        // Arrange
        var longDescription = new string('A', 500);
        var request = new CreateProductRequest
      {
            Title = "Test Product",
      Description = longDescription,
   Price = 50.00m
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
   Assert.NotNull(response.Data);
        Assert.Equal(longDescription, response.Data.Description);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
    {
      // Arrange
var request = new CreateProductRequest
        {
            Title = "",
            Description = "",
    Price = 0m
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(400, response.StatusCode);
  Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
   Assert.True(response.Messages.Length >= 3); // Multiple validation errors
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldGenerateSlug_RemovingSpecialCharacters()
    {
        // Arrange
        var request = new CreateProductRequest
        {
     Title = "Product, with. Special Characters!",
    Description = "Test Description",
   Price = 25.00m
        };

    // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
      Assert.NotNull(response);
      Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal("product-with-special-characters!", response.Data.Slug);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectStatusCode_OnSuccess()
    {
  // Arrange
        var request = new CreateProductRequest
        {
 Title = "Success Product",
Description = "Success Description",
            Price = 100.00m
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
      Assert.Equal(200, response.StatusCode);
        Assert.True(response.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_ShouldPersistProduct_InDatabase()
    {
 // Arrange
        var request = new CreateProductRequest
     {
Title = "Persist Test",
         Description = "Testing persistence",
         Price = 75.50m
      };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
   Assert.True(response.IsSuccess);
        Assert.Single(_db);
     var savedProduct = _db.First();
        Assert.Equal(response.Data!.Id, savedProduct.Id);
  Assert.Equal("Persist Test", savedProduct.Title);
    }
}
