using BugStore.Application.Handlers.Customers;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Customers;

public class GetCustomerByIdHandlerTest
{
    private readonly List<Customer> _db = [];
    private readonly FakeCustomerRepository _repository;
    private readonly GetCustomerByIdHandler _handler;

    public GetCustomerByIdHandlerTest()
    {
        _repository = new FakeCustomerRepository(_db);
        _handler = new GetCustomerByIdHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomer_WhenIdExists()
    {
        // Arrange
        var customer = Customer.Create(
            "John Doe",
            "john.doe@email.com",
            new DateTime(1990, 1, 1),
            "123456789").customer!;
        _db.Add(customer);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(customer.Id, response.Data.Id);
        Assert.Equal("John Doe", response.Data.Name);
        Assert.Equal("john.doe@email.com", response.Data.Email);
        Assert.Equal("123456789", response.Data.Phone);
        Assert.Equal(new DateTime(1990, 1, 1), response.Data.BirthDate);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new GetCustomerByIdRequest
        {
            CustomerId = nonExistentId
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.Equal(404, response.StatusCode);
        Assert.Null(response.Data);
        Assert.NotNull(response.Messages);
        Assert.Contains("Customer not found", response.Messages);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectCustomer_WhenMultipleCustomersExist()
    {
        // Arrange
        var customer1 = Customer.Create(
            "Customer One",
            "customer1@email.com",
            new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
            "Customer Two",
            "customer2@email.com",
            new DateTime(1991, 2, 2)).customer!;
        var customer3 = Customer.Create(
            "Customer Three",
            "customer3@email.com",
            new DateTime(1992, 3, 3)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer2.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(customer2.Id, response.Data.Id);
        Assert.Equal("Customer Two", response.Data.Name);
        Assert.Equal("customer2@email.com", response.Data.Email);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomerWithNullPhone_WhenPhoneIsNull()
    {
        // Arrange
        var customer = Customer.Create(
            "No Phone Customer",
            "nophone@email.com",
            new DateTime(1990, 1, 1),
            null).customer!;
        _db.Add(customer);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal("No Phone Customer", response.Data.Name);
        Assert.Null(response.Data.Phone);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenCustomerIdIsEmpty()
    {
        // Arrange
        var customer = Customer.Create(
            "Test User",
            "test@email.com",
            new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = Guid.Empty
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
    public async Task HandleAsync_ShouldReturnCustomerWithMinBirthDate_WhenBirthDateIsMinValue()
    {
        // Arrange
        var customer = Customer.Create(
            "Min Date Customer",
            "mindate@email.com",
            DateTime.MinValue).customer!;
        _db.Add(customer);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(DateTime.MinValue, response.Data.BirthDate);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomerWithAllFields_WhenCustomerHasCompleteData()
    {
        // Arrange
        var customer = Customer.Create(
            "Complete Customer",
            "complete@email.com",
            new DateTime(1995, 6, 15),
            "987654321").customer!;
        _db.Add(customer);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(customer.Id, response.Data.Id);
        Assert.Equal("Complete Customer", response.Data.Name);
        Assert.Equal("complete@email.com", response.Data.Email);
        Assert.Equal(new DateTime(1995, 6, 15), response.Data.BirthDate);
        Assert.Equal("987654321", response.Data.Phone);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotModifyDatabase_WhenRetrievingCustomer()
    {
        // Arrange
        var customer = Customer.Create(
            "Read Only Test",
            "readonly@email.com",
            new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);
        var initialCount = _db.Count;

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(initialCount, _db.Count);
        Assert.Contains(customer, _db);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenDatabaseIsEmpty()
    {
        // Arrange
        var request = new GetCustomerByIdRequest
        {
            CustomerId = Guid.NewGuid()
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
    public async Task HandleAsync_ShouldReturnUniqueCustomer_ByUniqueId()
    {
        // Arrange
        var customer1 = Customer.Create(
            "First Customer",
            "first@email.com",
            new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
            "Second Customer",
            "second@email.com",
            new DateTime(1991, 2, 2)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer1.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
        Assert.Equal(customer1.Id, response.Data.Id);
        Assert.NotEqual(customer2.Id, response.Data.Id);
        Assert.Equal("First Customer", response.Data.Name);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSameCustomer_WhenCalledMultipleTimes()
    {
        // Arrange
        var customer = Customer.Create(
            "Consistent Customer",
            "consistent@email.com",
            new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer.Id
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
        Assert.Equal(response1.Data.Name, response2.Data.Name);
        Assert.Equal(response1.Data.Email, response2.Data.Email);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomer_WithCorrectStatusCode()
    {
        // Arrange
        var customer = Customer.Create(
            "Status Test",
            "status@email.com",
            new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);

        var request = new GetCustomerByIdRequest
        {
            CustomerId = customer.Id
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
        Assert.True(response.IsSuccess);
        Assert.NotNull(response.Data);
    }
}
