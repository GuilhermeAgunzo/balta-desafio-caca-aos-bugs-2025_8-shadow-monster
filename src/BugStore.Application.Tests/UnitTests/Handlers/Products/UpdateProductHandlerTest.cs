using BugStore.Application.Handlers.Products;
using BugStore.Application.Requests.Products;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Products;

public class UpdateProductHandlerTest
{
 private readonly List<Product> _db = [];
    private readonly FakeProductRepository _repository;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTest()
    {
        _repository = new FakeProductRepository(_db);
        _handler = new UpdateProductHandler(_repository);
 }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProduct_WhenProductExistsAndRequestIsValid()
    {
        // Arrange
        var (existingProduct, _) = Product.Create(
  "Original Product",
            "Original Description",
            50.00m);
        _db.Add(existingProduct!);

var request = new UpdateProductRequest
        {
ProductId = existingProduct!.Id,
         Title = "Updated Product",
Description = "Updated Description",
            Price = 75.00m
};

     // Act
     var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

   // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
 Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(existingProduct.Id, response.Data.Id);
        Assert.Equal("Updated Product", response.Data.Title);
   Assert.Equal("Updated Description", response.Data.Description);
        Assert.Equal(75.00m, response.Data.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
    var nonExistentId = Guid.NewGuid();
   var request = new UpdateProductRequest
      {
  ProductId = nonExistentId,
     Title = "Updated Product",
  Description = "Updated Description",
 Price = 50.00m
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
  public async Task HandleAsync_ShouldReturnError_WhenTitleIsEmpty()
    {
        // Arrange
  var (existingProduct, _) = Product.Create("Original Product", "Description", 50.00m);
   _db.Add(existingProduct!);

     var request = new UpdateProductRequest
   {
            ProductId = existingProduct!.Id,
            Title = "",
   Description = "Updated Description",
   Price = 50.00m
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenDescriptionIsEmpty()
    {
 // Arrange
   var (existingProduct, _) = Product.Create("Original Product", "Description", 50.00m);
        _db.Add(existingProduct!);

     var request = new UpdateProductRequest
    {
          ProductId = existingProduct!.Id,
    Title = "Updated Product",
         Description = "",
            Price = 50.00m
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenPriceIsZero()
    {
        // Arrange
        var (existingProduct, _) = Product.Create("Original Product", "Description", 50.00m);
        _db.Add(existingProduct!);

   var request = new UpdateProductRequest
   {
ProductId = existingProduct!.Id,
     Title = "Updated Product",
      Description = "Updated Description",
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnError_WhenPriceIsNegative()
  {
        // Arrange
    var (existingProduct, _) = Product.Create("Original Product", "Description", 50.00m);
        _db.Add(existingProduct!);

      var request = new UpdateProductRequest
        {
   ProductId = existingProduct!.Id,
  Title = "Updated Product",
          Description = "Updated Description",
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
    }

    [Fact]
  public async Task HandleAsync_ShouldMaintainSameId_AfterUpdate()
    {
 // Arrange
   var (existingProduct, _) = Product.Create("Original Product", "Description", 50.00m);
     _db.Add(existingProduct!);
        var originalId = existingProduct!.Id;

    var request = new UpdateProductRequest
     {
       ProductId = originalId,
    Title = "Updated Product",
 Description = "Updated Description",
   Price = 100.00m
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
    Assert.True(response.IsSuccess);
Assert.NotNull(response.Data);
        Assert.Equal(originalId, response.Data.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateOnlyTitle_WhenOnlyTitleChanges()
    {
        // Arrange
        var (existingProduct, _) = Product.Create("Original Product", "Original Description", 50.00m);
        _db.Add(existingProduct!);

   var request = new UpdateProductRequest
        {
 ProductId = existingProduct!.Id,
   Title = "New Title",
    Description = "Original Description",
 Price = 50.00m
  };

        // Act
      var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

    // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
   Assert.NotNull(response.Data);
        Assert.Equal("New Title", response.Data.Title);
 Assert.Equal("Original Description", response.Data.Description);
        Assert.Equal(50.00m, response.Data.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateSlug_WhenTitleChanges()
    {
 // Arrange
    var (existingProduct, _) = Product.Create("Original Product", "Description", 50.00m);
    _db.Add(existingProduct!);

        var request = new UpdateProductRequest
    {
       ProductId = existingProduct!.Id,
    Title = "Completely New Title",
     Description = "Description",
            Price = 50.00m
        };

        // Act
     var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
  Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal("completely-new-title", response.Data.Slug);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotAffectOtherProducts_WhenUpdatingOne()
    {
        // Arrange
  var (product1, _) = Product.Create("Product 1", "Description 1", 10.00m);
     var (product2, _) = Product.Create("Product 2", "Description 2", 20.00m);
        var (product3, _) = Product.Create("Product 3", "Description 3", 30.00m);

   _db.Add(product1!);
   _db.Add(product2!);
   _db.Add(product3!);

        var request = new UpdateProductRequest
{
   ProductId = product2!.Id,
     Title = "Updated Product 2",
            Description = "Updated Description 2",
          Price = 25.00m
        };

     // Act
  var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
        Assert.True(response.IsSuccess);
   Assert.Equal(3, _db.Count);
   
        var unchangedProduct1 = _db.First(p => p.Id == product1!.Id);
      var unchangedProduct3 = _db.First(p => p.Id == product3!.Id);
        
        Assert.Equal("Product 1", unchangedProduct1.Title);
Assert.Equal("Product 3", unchangedProduct3.Title);
}

    [Fact]
    public async Task HandleAsync_ShouldUpdatePrice_WithDecimalValue()
    {
        // Arrange
        var (existingProduct, _) = Product.Create("Product", "Description", 50.00m);
  _db.Add(existingProduct!);

        var request = new UpdateProductRequest
   {
            ProductId = existingProduct!.Id,
   Title = "Product",
   Description = "Description",
          Price = 123.45m
        };

        // Act
 var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
     Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(123.45m, response.Data.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
    {
        // Arrange
 var (existingProduct, _) = Product.Create("Product", "Description", 50.00m);
   _db.Add(existingProduct!);

        var request = new UpdateProductRequest
        {
     ProductId = existingProduct!.Id,
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
        Assert.True(response.Messages.Length >= 3);
    }

    [Fact]
  public async Task HandleAsync_ShouldUpdateProduct_WithLongDescription()
    {
        // Arrange
        var (existingProduct, _) = Product.Create("Product", "Short description", 50.00m);
  _db.Add(existingProduct!);

     var longDescription = new string('X', 1000);
        var request = new UpdateProductRequest
        {
     ProductId = existingProduct!.Id,
   Title = "Product",
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
    public async Task HandleAsync_ShouldReturnCorrectStatusCode_OnSuccess()
    {
        // Arrange
        var (existingProduct, _) = Product.Create("Product", "Description", 50.00m);
        _db.Add(existingProduct!);

        var request = new UpdateProductRequest
        {
            ProductId = existingProduct!.Id,
         Title = "Updated Product",
    Description = "Updated Description",
 Price = 60.00m
        };

    // Act
var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

     // Assert
   Assert.NotNull(response);
    Assert.Equal(200, response.StatusCode);
        Assert.True(response.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_ShouldPersistChanges_InDatabase()
    {
        // Arrange
        var (existingProduct, _) = Product.Create("Original", "Description", 50.00m);
  _db.Add(existingProduct!);
        var productId = existingProduct!.Id;

 var request = new UpdateProductRequest
        {
  ProductId = productId,
    Title = "Persisted Update",
    Description = "Persisted Description",
     Price = 99.99m
     };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        
        var updatedProduct = _db.First(p => p.Id == productId);
        Assert.Equal("Persisted Update", updatedProduct.Title);
     Assert.Equal("Persisted Description", updatedProduct.Description);
        Assert.Equal(99.99m, updatedProduct.Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotChangeDatabaseCount_AfterUpdate()
    {
  // Arrange
        var (existingProduct, _) = Product.Create("Product", "Description", 50.00m);
      _db.Add(existingProduct!);
   var initialCount = _db.Count;

    var request = new UpdateProductRequest
   {
ProductId = existingProduct!.Id,
            Title = "Updated Product",
 Description = "Updated Description",
            Price = 75.00m
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
   Assert.NotNull(response);
        Assert.True(response.IsSuccess);
     Assert.Equal(initialCount, _db.Count);
 }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDatabaseIsEmpty()
  {
// Arrange
  var request = new UpdateProductRequest
        {
  ProductId = Guid.NewGuid(),
            Title = "Product",
Description = "Description",
            Price = 50.00m
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
    public async Task HandleAsync_ShouldUpdateProduct_MultipleTimesSuccessively()
    {
        // Arrange
        var (existingProduct, _) = Product.Create("Original", "Description", 50.00m);
        _db.Add(existingProduct!);
        var productId = existingProduct!.Id;

        // Act - First update
      var request1 = new UpdateProductRequest
    {
   ProductId = productId,
            Title = "First Update",
     Description = "Description",
    Price = 60.00m
        };
        var response1 = await _handler.HandleAsync(request1, TestContext.Current.CancellationToken);

  // Act - Second update
   var request2 = new UpdateProductRequest
{
      ProductId = productId,
      Title = "Second Update",
   Description = "Description",
            Price = 70.00m
        };
        var response2 = await _handler.HandleAsync(request2, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response1.IsSuccess);
        Assert.True(response2.IsSuccess);
        
      var finalProduct = _db.First(p => p.Id == productId);
 Assert.Equal("Second Update", finalProduct.Title);
   Assert.Equal(70.00m, finalProduct.Price);
    }
}
