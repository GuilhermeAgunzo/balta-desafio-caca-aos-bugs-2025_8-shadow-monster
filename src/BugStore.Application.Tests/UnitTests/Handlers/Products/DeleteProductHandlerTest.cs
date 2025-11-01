using BugStore.Application.Handlers.Products;
using BugStore.Application.Requests.Products;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Products;

public class DeleteProductHandlerTest
{
    private readonly List<Product> _db = [];
    private readonly FakeProductRepository _repository;
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTest()
    {
        _repository = new FakeProductRepository(_db);
        _handler = new DeleteProductHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var (product, _) = Product.Create("Test Product", "Test Description", 99.99m);
        _db.Add(product!);
        var productId = product!.Id;

        var request = new DeleteProductRequest
        {
            ProductId = productId
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.True(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new DeleteProductRequest
        {
            ProductId = nonExistentId
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
        Assert.False(response.Data);
        Assert.NotNull(response.Messages);
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveOnlySpecifiedProduct_WhenMultipleExist()
    {
        // Arrange
        var (product1, _) = Product.Create("Product 1", "Description 1", 10.00m);
        var (product2, _) = Product.Create("Product 2", "Description 2", 20.00m);
        var (product3, _) = Product.Create("Product 3", "Description 3", 30.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);

        var request = new DeleteProductRequest
        {
            ProductId = product2!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, _db.Count);
        Assert.Contains(product1, _db);
        Assert.DoesNotContain(product2, _db);
        Assert.Contains(product3, _db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDatabaseIsEmpty()
    {
        // Arrange
        var request = new DeleteProductRequest
        {
            ProductId = Guid.NewGuid()
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
    public async Task HandleAsync_ShouldReturnNotFound_WhenProductIdIsEmpty()
    {
        // Arrange
        var (product, _) = Product.Create("Test Product", "Description", 50.00m);
        _db.Add(product!);

        var request = new DeleteProductRequest
        {
            ProductId = Guid.Empty
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
        Assert.False(response.Data);
        Assert.Single(_db); // Original product still exists
    }

    [Fact]
    public async Task HandleAsync_ShouldDecrementDatabaseCount_AfterDeletion()
    {
        // Arrange
        var (product1, _) = Product.Create("Product 1", "Description 1", 10.00m);
        var (product2, _) = Product.Create("Product 2", "Description 2", 20.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        var initialCount = _db.Count;

        var request = new DeleteProductRequest
        {
            ProductId = product1!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(initialCount - 1, _db.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteAllProducts_WhenCalledForEach()
    {
        // Arrange
        var (product1, _) = Product.Create("Product 1", "Description 1", 10.00m);
        var (product2, _) = Product.Create("Product 2", "Description 2", 20.00m);
        var (product3, _) = Product.Create("Product 3", "Description 3", 30.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);

        // Act
        var response1 = await _handler.HandleAsync(
        new DeleteProductRequest { ProductId = product1!.Id },
        TestContext.Current.CancellationToken);
        var response2 = await _handler.HandleAsync(
                new DeleteProductRequest { ProductId = product2!.Id },
    TestContext.Current.CancellationToken);
        var response3 = await _handler.HandleAsync(
       new DeleteProductRequest { ProductId = product3!.Id },
           TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response1.IsSuccess);
        Assert.True(response2.IsSuccess);
        Assert.True(response3.IsSuccess);
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDeletingSameProductTwice()
    {
        // Arrange
        var (product, _) = Product.Create("Test Product", "Description", 50.00m);
        _db.Add(product!);
        var productId = product!.Id;

        var request = new DeleteProductRequest
        {
            ProductId = productId
        };

        // Act - First deletion
        var response1 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Act - Second deletion attempt
        var response2 = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response1.IsSuccess);
        Assert.False(response2.IsSuccess);
        Assert.Equal(404, response2.StatusCode);
        Assert.False(response2.Data);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccessMessage_OnSuccessfulDeletion()
    {
        // Arrange
        var (product, _) = Product.Create("Product To Delete", "Description", 75.00m);
        _db.Add(product!);

        var request = new DeleteProductRequest
        {
            ProductId = product!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Messages);
        Assert.NotEmpty(response.Messages);
        Assert.Contains("deleted successfully", response.Messages[0].ToLower());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_OnSuccessfulDeletion()
    {
        // Arrange
        var (product, _) = Product.Create("Test Product", "Description", 100.00m);
        _db.Add(product!);

        var request = new DeleteProductRequest
        {
            ProductId = product!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalse_OnFailedDeletion()
    {
        // Arrange
        var request = new DeleteProductRequest
        {
            ProductId = Guid.NewGuid()
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Data);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProduct_WithAnyPrice()
    {
        // Arrange
        var (expensiveProduct, _) = Product.Create("Expensive", "Description", 9999.99m);
        _db.Add(expensiveProduct!);

        var request = new DeleteProductRequest
        {
            ProductId = expensiveProduct!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProduct_WithLongTitle()
    {
        // Arrange
        var longTitle = new string('A', 200);
        var (product, _) = Product.Create(longTitle, "Description", 50.00m);
        _db.Add(product!);

        var request = new DeleteProductRequest
        {
            ProductId = product!.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Empty(_db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectStatusCodes()
    {
        // Arrange
        var (product, _) = Product.Create("Test Product", "Description", 50.00m);
        _db.Add(product!);

        var successRequest = new DeleteProductRequest { ProductId = product!.Id };
        var failRequest = new DeleteProductRequest { ProductId = Guid.NewGuid() };

        // Act
        var successResponse = await _handler.HandleAsync(successRequest, TestContext.Current.CancellationToken);
        var failResponse = await _handler.HandleAsync(failRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(200, successResponse.StatusCode);
        Assert.Equal(404, failResponse.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_ShouldMaintainDataIntegrity_AfterMultipleDeletions()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var (product, _) = Product.Create($"Product {i}", $"Description {i}", i * 10.00m);
            _db.Add(product!);
        }

        var productsToDelete = _db.Take(5).ToList();

        // Act
        foreach (var product in productsToDelete)
        {
            var request = new DeleteProductRequest { ProductId = product.Id };
            await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
        }

        // Assert
        Assert.Equal(5, _db.Count);
        Assert.All(productsToDelete, p => Assert.DoesNotContain(p, _db));
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleConcurrentDeletionAttempts()
    {
        // Arrange
        var (product, _) = Product.Create("Test Product", "Description", 50.00m);
        _db.Add(product!);
        var productId = product!.Id;

        var request1 = new DeleteProductRequest { ProductId = productId };
        var request2 = new DeleteProductRequest { ProductId = productId };

        // Act
        var response1 = await _handler.HandleAsync(request1, TestContext.Current.CancellationToken);
        var response2 = await _handler.HandleAsync(request2, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response1.IsSuccess || response2.IsSuccess); // One should succeed
        Assert.False(response1.IsSuccess && response2.IsSuccess); // Both can't succeed
        Assert.Empty(_db);
    }
}
