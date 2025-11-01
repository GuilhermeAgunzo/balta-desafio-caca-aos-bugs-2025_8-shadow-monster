using BugStore.Application.Requests.Products;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Repositories;

namespace BugStore.Infrastructure.Tests.UnitTests;

public class ProductUnitTest
{
    private readonly List<Product> _db = [];
    private readonly FakeProductRepository _repository;

    public ProductUnitTest()
    {
        _repository = new FakeProductRepository(_db);
    }

    [Fact]
    public async Task AddAsync_ShouldAddValidProduct()
    {
        var (product, _) = Product.Create("Mouse", "Wireless mouse", 99.90m);

        var result = await _repository.AddAsync(product!, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Contains(product, _db);
    }

    [Fact]
    public async Task AddAsync_ShouldFail_WhenProductIsInvalid()
    {
        var (product, _) = Product.Create("", "", -1);

        var result = await _repository.AddAsync(product!, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.DoesNotContain(product, _db);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
    {
        var (product, _) = Product.Create("Keyboard", "Mechanical keyboard", 199.90m);
        _db.Add(product!);

        var result = await _repository.GetByIdAsync(product!.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(product.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnProduct_WhenExists()
    {
        var (product, _) = Product.Create("Monitor", "4K monitor", 999.90m);
        _db.Add(product!);

        var result = await _repository.GetBySlugAsync(product!.Slug, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(product.Slug, result.Data!.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetBySlugAsync("non-existent-slug", TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 10; i++)
        {
            var (product, _) = Product.Create($"Product {i}", "Test", 10m * i);
            _db.Add(product!);
        }

        var request = new GetProductsRequest
        {
            PageNumber = 2,
            PageSize = 3
        };

        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByTitle()
    {
        // Arrange
        var (product1, _) = Product.Create("Gaming Mouse", "Wireless gaming mouse", 99.90m);
        var (product2, _) = Product.Create("Gaming Keyboard", "Mechanical gaming keyboard", 199.90m);
        var (product3, _) = Product.Create("Office Mouse", "Wireless office mouse", 49.90m);
        var (product4, _) = Product.Create("Wireless Headset", "Bluetooth headset", 149.90m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);
        _db.Add(product4!);

        var request = new GetProductsRequest
        {
            Title = "Gaming",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Contains("Gaming", p.Title));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByDescription()
    {
        // Arrange
        var (product1, _) = Product.Create("Mouse A", "Wireless technology", 99.90m);
        var (product2, _) = Product.Create("Mouse B", "Wired connection", 79.90m);
        var (product3, _) = Product.Create("Keyboard", "Wireless technology", 149.90m);
        var (product4, _) = Product.Create("Monitor", "LED display", 499.90m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);
        _db.Add(product4!);

        var request = new GetProductsRequest
        {
            Description = "Wireless",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Contains("Wireless", p.Description));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterBySlug()
    {
        // Arrange
        var (product1, _) = Product.Create("Gaming Mouse", "Description 1", 99.90m);
        var (product2, _) = Product.Create("Gaming Keyboard", "Description 2", 199.90m);
        var (product3, _) = Product.Create("Office Chair", "Description 3", 299.90m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);

        var request = new GetProductsRequest
        {
            Slug = "gaming",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Contains("gaming", p.Slug));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByMinPrice()
    {
        // Arrange
        var (product1, _) = Product.Create("Product A", "Description A", 50.00m);
        var (product2, _) = Product.Create("Product B", "Description B", 100.00m);
        var (product3, _) = Product.Create("Product C", "Description C", 150.00m);
        var (product4, _) = Product.Create("Product D", "Description D", 200.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);
        _db.Add(product4!);

        var request = new GetProductsRequest
        {
            MinPrice = 100.00m,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, p => Assert.True(p.Price >= 100.00m));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByMaxPrice()
    {
        // Arrange
        var (product1, _) = Product.Create("Product A", "Description A", 50.00m);
        var (product2, _) = Product.Create("Product B", "Description B", 100.00m);
        var (product3, _) = Product.Create("Product C", "Description C", 150.00m);
        var (product4, _) = Product.Create("Product D", "Description D", 200.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);
        _db.Add(product4!);

        var request = new GetProductsRequest
        {
            MaxPrice = 120.00m,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.True(p.Price <= 120.00m));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByPriceRange()
    {
        // Arrange
        var (product1, _) = Product.Create("Product A", "Description A", 50.00m);
        var (product2, _) = Product.Create("Product B", "Description B", 100.00m);
        var (product3, _) = Product.Create("Product C", "Description C", 150.00m);
        var (product4, _) = Product.Create("Product D", "Description D", 200.00m);
        var (product5, _) = Product.Create("Product E", "Description E", 250.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);
        _db.Add(product4!);
        _db.Add(product5!);

        var request = new GetProductsRequest
        {
            MinPrice = 100.00m,
            MaxPrice = 200.00m,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, p => Assert.True(p.Price >= 100.00m && p.Price <= 200.00m));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByMultipleFields()
    {
        // Arrange
        var (product1, _) = Product.Create("Gaming Mouse Pro", "Wireless gaming peripheral", 99.90m);
        var (product2, _) = Product.Create("Gaming Keyboard", "Mechanical gaming keyboard", 199.90m);
        var (product3, _) = Product.Create("Gaming Mouse Basic", "Wired gaming mouse", 49.90m);
        var (product4, _) = Product.Create("Office Mouse", "Wireless office mouse", 89.90m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);
        _db.Add(product4!);

        var request = new GetProductsRequest
        {
            Title = "Gaming Mouse",
            Description = "Wireless",
            MinPrice = 80.00m,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        var product = result.Items.First();
        Assert.Contains("Gaming Mouse", product.Title);
        Assert.Contains("Wireless", product.Description);
        Assert.True(product.Price >= 80.00m);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEmpty_WhenNoMatchesFound()
    {
        // Arrange
        var (product1, _) = Product.Create("Mouse", "Wireless mouse", 99.90m);
        var (product2, _) = Product.Create("Keyboard", "Mechanical keyboard", 199.90m);

        _db.Add(product1!);
        _db.Add(product2!);

        var request = new GetProductsRequest
        {
            Title = "NonExistentProduct",
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
        var (product1, _) = Product.Create("Product A", "Description A", 50.00m);
        var (product2, _) = Product.Create("Product B", "Description B", 100.00m);

        _db.Add(product1!);
        _db.Add(product2!);

        var request = new GetProductsRequest
        {
            Title = null,
            Description = null,
            Slug = null,
            MinPrice = null,
            MaxPrice = null,
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
        var (product1, _) = Product.Create("Product A", "Description A", 50.00m);
        var (product2, _) = Product.Create("Product B", "Description B", 100.00m);

        _db.Add(product1!);
        _db.Add(product2!);

        var request = new GetProductsRequest
        {
            Title = "",
            Description = "   ",
            Slug = "",
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
        for (int i = 1; i <= 20; i++)
        {
            var (product, _) = Product.Create($"Gaming Product {i}", "Gaming description", 50.00m + i);
            _db.Add(product!);
        }

        var request = new GetProductsRequest
        {
            Title = "Gaming",
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
    public async Task GetPagedAsync_ShouldFilterByPartialTitle()
    {
        // Arrange
        var (product1, _) = Product.Create("Mechanical Keyboard", "Description 1", 199.90m);
        var (product2, _) = Product.Create("Mechanic Tools", "Description 2", 99.90m);
        var (product3, _) = Product.Create("Wireless Mouse", "Description 3", 79.90m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);

        var request = new GetProductsRequest
        {
            Title = "Mech",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, p => Assert.Contains("Mech", p.Title));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByPartialDescription()
    {
        // Arrange
        var (product1, _) = Product.Create("Product A", "High quality wireless device", 99.90m);
        var (product2, _) = Product.Create("Product B", "Premium quality wired device", 89.90m);
        var (product3, _) = Product.Create("Product C", "Standard device", 69.90m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);

        var request = new GetProductsRequest
        {
            Description = "quality",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, p => Assert.Contains("quality", p.Description));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnOrderedByTitle()
    {
        // Arrange
        var (product1, _) = Product.Create("Zebra Product", "Description Z", 50.00m);
        var (product2, _) = Product.Create("Alpha Product", "Description A", 100.00m);
        var (product3, _) = Product.Create("Mike Product", "Description M", 150.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);

        var request = new GetProductsRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        var products = result.Items.ToList();
        Assert.Equal("Alpha Product", products[0].Title);
        Assert.Equal("Mike Product", products[1].Title);
        Assert.Equal("Zebra Product", products[2].Title);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByExactPriceMatch()
    {
        // Arrange
        var (product1, _) = Product.Create("Product A", "Description A", 99.99m);
        var (product2, _) = Product.Create("Product B", "Description B", 100.00m);
        var (product3, _) = Product.Create("Product C", "Description C", 100.01m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);

        var request = new GetProductsRequest
        {
            MinPrice = 100.00m,
            MaxPrice = 100.00m,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Items);
        Assert.Equal(100.00m, result.Items.First().Price);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByCombinedSlugAndPriceRange()
    {
        // Arrange
        var (product1, _) = Product.Create("Gaming Mouse Pro", "Description 1", 150.00m);
        var (product2, _) = Product.Create("Gaming Keyboard", "Description 2", 200.00m);
        var (product3, _) = Product.Create("Gaming Monitor", "Description 3", 500.00m);
        var (product4, _) = Product.Create("Office Chair", "Description 4", 180.00m);

        _db.Add(product1!);
        _db.Add(product2!);
        _db.Add(product3!);
        _db.Add(product4!);

        var request = new GetProductsRequest
        {
            Slug = "gaming",
            MinPrice = 100.00m,
            MaxPrice = 250.00m,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, p =>
        {
            Assert.Contains("gaming", p.Slug);
            Assert.True(p.Price >= 100.00m && p.Price <= 250.00m);
        });
    }
}