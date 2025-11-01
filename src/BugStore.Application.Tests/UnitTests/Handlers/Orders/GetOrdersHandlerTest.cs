using BugStore.Application.Handlers.Orders;
using BugStore.Application.Requests.Orders;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Orders;

public class GetOrdersHandlerTest
{
    private readonly List<Order> _db = [];
    private readonly FakeOrderRepository _repository;
    private readonly GetOrdersHandler _handler;

    public GetOrdersHandlerTest()
    {
        _repository = new FakeOrderRepository(_db);
        _handler = new GetOrdersHandler(_repository);
    }

    #region Basic Pagination Tests

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenDatabaseIsEmpty()
    {
        // Arrange
        var request = new GetOrdersRequest
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllOrders_WhenPageSizeIsGreaterThanCount()
    {
        // Arrange
        var customer1 = CreateCustomer("Customer One", "customer1@email.com");
        var customer2 = CreateCustomer("Customer Two", "customer2@email.com");
        var customer3 = CreateCustomer("Customer Three", "customer3@email.com");

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);
        var order3 = CreateOrder(customer3);

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFirstPage_WhenPageNumberIsOne()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var customer = CreateCustomer($"Customer {i:D2}", $"customer{i}@email.com");
            var order = CreateOrder(customer);
            _db.Add(order);
        }

        var request = new GetOrdersRequest
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSecondPage_WhenPageNumberIsTwo()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var customer = CreateCustomer($"Customer {i:D2}", $"customer{i}@email.com");
            var order = CreateOrder(customer);
            _db.Add(order);
        }

        var request = new GetOrdersRequest
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenPageNumberExceedsTotalPages()
    {
        // Arrange
        var customer = CreateCustomer("Single Customer", "single@email.com");
        var order = CreateOrder(customer);
        _db.Add(order);

        var request = new GetOrdersRequest
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPartialPage_WhenLastPageIsNotFull()
    {
        // Arrange
        for (int i = 1; i <= 7; i++)
        {
            var customer = CreateCustomer($"Customer {i}", $"customer{i}@email.com");
            var order = CreateOrder(customer);
            _db.Add(order);
        }

        var request = new GetOrdersRequest
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnOrdersOrderedByCreatedAtDescending()
    {
        // Arrange
        var customer = CreateCustomer("Test Customer", "test@email.com");

        var order1 = CreateOrder(customer, DateTime.UtcNow.AddDays(-3));
        var order2 = CreateOrder(customer, DateTime.UtcNow.AddDays(-1));
        var order3 = CreateOrder(customer, DateTime.UtcNow.AddDays(-2));

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
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

        var orders = response.Data.ToList();
        Assert.True(orders[0].CreatedAt >= orders[1].CreatedAt);
        Assert.True(orders[1].CreatedAt >= orders[2].CreatedAt);
    }

    #endregion

    #region Customer Filter Tests

    [Fact]
    public async Task HandleAsync_ShouldFilterByCustomerName_WhenCustomerNameProvided()
    {
        // Arrange
        var customer1 = CreateCustomer("John Doe", "john@email.com");
        var customer2 = CreateCustomer("Jane Smith", "jane@email.com");
        var customer3 = CreateCustomer("John Williams", "williams@email.com");

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);
        var order3 = CreateOrder(customer3);

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerName = "John"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.All(response.Data, o => Assert.Contains("John", o.Customer.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByCustomerEmail_WhenCustomerEmailProvided()
    {
        // Arrange
        var customer1 = CreateCustomer("Customer One", "john.doe@gmail.com");
        var customer2 = CreateCustomer("Customer Two", "jane.smith@yahoo.com");
        var customer3 = CreateCustomer("Customer Three", "john.williams@gmail.com");

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);
        var order3 = CreateOrder(customer3);

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerEmail = "gmail"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.All(response.Data, o => Assert.Contains("gmail", o.Customer.Email, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByCustomerPhone_WhenCustomerPhoneProvided()
    {
        // Arrange
        var customer1 = CreateCustomer("Customer One", "customer1@email.com", "11999998888");
        var customer2 = CreateCustomer("Customer Two", "customer2@email.com", "21888887777");
        var customer3 = CreateCustomer("Customer Three", "customer3@email.com", "11777776666");

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);
        var order3 = CreateOrder(customer3);

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerPhone = "119"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data!);
        Assert.All(response.Data!, o => Assert.Contains("119", o.Customer.Phone));
    }

    [Fact]
    public async Task HandleAsync_ShouldApplyMultipleCustomerFilters()
    {
        // Arrange
        var customer1 = CreateCustomer("John Doe", "john.doe@gmail.com", "11999998888");
        var customer2 = CreateCustomer("John Smith", "john.smith@yahoo.com", "21888887777");
        var customer3 = CreateCustomer("Jane Doe", "jane.doe@gmail.com", "11777776666");

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);
        var order3 = CreateOrder(customer3);

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerName = "John",
            CustomerEmail = "gmail"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data);
        Assert.Equal("John Doe", response.Data.First().Customer.Name);
    }

    #endregion

    #region Date Filter Tests

    [Fact]
    public async Task HandleAsync_ShouldFilterByCreatedAtStart()
    {
        // Arrange
        var customer = CreateCustomer("Test Customer", "test@email.com");

        var order1 = CreateOrder(customer, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var order2 = CreateOrder(customer, new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        var order3 = CreateOrder(customer, new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CreatedAtStart = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.All(response.Data, o => Assert.True(o.CreatedAt >= new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByCreatedAtEnd()
    {
        // Arrange
        var customer = CreateCustomer("Test Customer", "test@email.com");

        var order1 = CreateOrder(customer, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var order2 = CreateOrder(customer, new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        var order3 = CreateOrder(customer, new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CreatedAtEnd = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.All(response.Data, o => Assert.True(o.CreatedAt <= new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByCreatedAtRange()
    {
        // Arrange
        var customer = CreateCustomer("Test Customer", "test@email.com");

        var order1 = CreateOrder(customer, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var order2 = CreateOrder(customer, new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        var order3 = CreateOrder(customer, new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc));
        var order4 = CreateOrder(customer, new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc));

        _db.AddRange([order1, order2, order3, order4]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CreatedAtStart = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedAtEnd = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.All(response.Data, o =>
        {
            Assert.True(o.CreatedAt >= new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));
            Assert.True(o.CreatedAt <= new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc));
        });
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByUpdatedAtStart()
    {
        // Arrange
        var customer = CreateCustomer("Test Customer", "test@email.com");

        var order1 = CreateOrder(customer, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var order2 = CreateOrder(customer, new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        var order3 = CreateOrder(customer, new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            UpdatedAtStart = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.All(response.Data, o => Assert.True(o.UpdatedAt >= new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByUpdatedAtEnd()
    {
        // Arrange
        var customer = CreateCustomer("Test Customer", "test@email.com");

        var order1 = CreateOrder(customer, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var order2 = CreateOrder(customer, new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        var order3 = CreateOrder(customer, new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            UpdatedAtEnd = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.All(response.Data, o => Assert.True(o.UpdatedAt <= new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc)));
    }

    #endregion

    #region Combined Filter Tests

    [Fact]
    public async Task HandleAsync_ShouldApplyCombinedFilters()
    {
        // Arrange
        var customer1 = CreateCustomer("John Doe", "john@gmail.com");
        var customer2 = CreateCustomer("Jane Smith", "jane@yahoo.com");
        var customer3 = CreateCustomer("John Williams", "john@yahoo.com");

        var order1 = CreateOrder(customer1, new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc));
        var order2 = CreateOrder(customer2, new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc));
        var order3 = CreateOrder(customer3, new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc));

        _db.AddRange([order1, order2, order3]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerName = "John",
            CustomerEmail = "gmail",
            CreatedAtStart = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedAtEnd = new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data);
        Assert.Equal("John Doe", response.Data.First().Customer.Name);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmpty_WhenFiltersMatchNoOrders()
    {
        // Arrange
        var customer = CreateCustomer("John Doe", "john@email.com");
        var order = CreateOrder(customer);
        _db.Add(order);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerName = "NonExistent"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Empty(response.Data);
    }

    [Fact]
    public async Task HandleAsync_ShouldIgnoreNullFilters()
    {
        // Arrange
        var customer1 = CreateCustomer("Customer One", "customer1@email.com");
        var customer2 = CreateCustomer("Customer Two", "customer2@email.com");

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);

        _db.AddRange([order1, order2]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerName = null,
            CustomerEmail = null,
            CustomerPhone = null,
            CreatedAtStart = null,
            CreatedAtEnd = null,
            UpdatedAtStart = null,
            UpdatedAtEnd = null
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task HandleAsync_ShouldIgnoreEmptyStringFilters()
    {
        // Arrange
        var customer1 = CreateCustomer("Customer One", "customer1@email.com");
        var customer2 = CreateCustomer("Customer Two", "customer2@email.com");

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);

        _db.AddRange([order1, order2]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerName = "",
            CustomerEmail = "",
            CustomerPhone = ""
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
    }

    #endregion

    #region Pagination with Filters Tests

    [Fact]
    public async Task HandleAsync_ShouldApplyPaginationWithFilters()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var customer = CreateCustomer($"John Customer {i:D2}", $"john{i}@email.com");
            var order = CreateOrder(customer);
            _db.Add(order);
        }

        var otherCustomer = CreateCustomer("Jane Smith", "jane@email.com");
        var otherOrder = CreateOrder(otherCustomer);
        _db.Add(otherOrder);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 5,
            CustomerName = "John"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Data.Count());
        Assert.All(response.Data, o => Assert.Contains("John", o.Customer.Name));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSecondPageWithFilters()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var customer = CreateCustomer($"John Customer {i:D2}", $"john{i}@email.com");
            var order = CreateOrder(customer);
            _db.Add(order);
        }

        var request = new GetOrdersRequest
        {
            PageNumber = 2,
            PageSize = 5,
            CustomerName = "John"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Data.Count());
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task HandleAsync_ShouldReturnConsistentResults_WhenCalledMultipleTimes()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            var customer = CreateCustomer($"Customer {i}", $"customer{i}@email.com");
            var order = CreateOrder(customer);
            _db.Add(order);
        }

        var request = new GetOrdersRequest
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
        Assert.Equal(response1.Data.Count(), response2.Data.Count());
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleLargePageSize()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            var customer = CreateCustomer($"Customer {i}", $"customer{i}@email.com");
            var order = CreateOrder(customer);
            _db.Add(order);
        }

        var request = new GetOrdersRequest
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
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectStatusCode_OnSuccess()
    {
        // Arrange
        var customer = CreateCustomer("Test Customer", "test@email.com");
        var order = CreateOrder(customer);
        _db.Add(order);

        var request = new GetOrdersRequest
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
    public async Task HandleAsync_ShouldNotModifyDatabase_WhenRetrievingOrders()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            var customer = CreateCustomer($"Customer {i}", $"customer{i}@email.com");
            var order = CreateOrder(customer);
            _db.Add(order);
        }

        var initialCount = _db.Count;
        var initialOrders = _db.ToList();

        var request = new GetOrdersRequest
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
        Assert.All(initialOrders, o => Assert.Contains(o, _db));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSingleOrder_WhenOnlyOneExists()
    {
        // Arrange
        var customer = CreateCustomer("John Doe", "john.doe@email.com", "123456789");
        var order = CreateOrder(customer);
        _db.Add(order);

        var request = new GetOrdersRequest
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

        var returnedOrder = response.Data.First();
        Assert.Equal(order.Id, returnedOrder.Id);
        Assert.Equal(customer.Id, returnedOrder.CustomerId);
        Assert.Equal("John Doe", returnedOrder.Customer.Name);
    }

    [Fact]
    public async Task HandleAsync_ShouldHandleCustomersWithNullPhone()
    {
        // Arrange
        var customer1 = CreateCustomer("Customer One", "customer1@email.com", "11999998888");
        var customer2 = CreateCustomer("Customer Two", "customer2@email.com", null);

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);

        _db.AddRange([order1, order2]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task HandleAsync_ShouldBeCaseInsensitive_WhenFilteringByCustomerName()
    {
        // Arrange
        var customer1 = CreateCustomer("JOHN DOE", "john@email.com");
        var customer2 = CreateCustomer("john smith", "smith@email.com");

        var order1 = CreateOrder(customer1);
        var order2 = CreateOrder(customer2);

        _db.AddRange([order1, order2]);

        var request = new GetOrdersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            CustomerName = "john"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
    }

    #endregion

    #region Helper Methods

    private static Customer CreateCustomer(string name, string email, string? phone = null)
    {
        var (customer, _) = Customer.Create(name, email, new DateTime(1990, 1, 1), phone);
        return customer!;
    }

    private static Order CreateOrder(Customer customer, DateTime? createdAt = null)
    {
        var (order, _) = Order.Create(customer);

        if (createdAt.HasValue && order != null)
        {
            // Use reflection to set the CreatedAt and UpdatedAt for testing purposes
            var createdAtProperty = typeof(Order).GetProperty("CreatedAt");
            var updatedAtProperty = typeof(Order).GetProperty("UpdatedAt");

            createdAtProperty?.SetValue(order, createdAt.Value);
            updatedAtProperty?.SetValue(order, createdAt.Value);
        }

        return order!;
    }

    #endregion
}
