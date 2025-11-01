using BugStore.Application.Requests.Customers;
using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Repositories;

namespace BugStore.Infrastructure.Tests.UnitTests;

public class CustomerUnitTest
{
    private readonly List<Customer> _db = [];
    private readonly FakeCustomerRepository _repository;

    public CustomerUnitTest()
    {
        _repository = new FakeCustomerRepository(_db);
    }

    [Fact]
    public async Task AddAsync_ShouldAddValidCustomer()
    {
        var (customer, _) = Customer.Create("John Doe", "john.doe@email.com", new DateTime(1990, 1, 1));

        var result = await _repository.AddAsync(customer!, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Contains(customer, _db);
    }

    [Fact]
    public async Task AddAsync_ShouldFail_WhenCustomerIsInvalid()
    {
        var (customer, _) = Customer.Create("", "", new DateTime(1990, 1, 1));

        var result = await _repository.AddAsync(customer, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.DoesNotContain(customer, _db);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenExists()
    {
        var (customer, _) = Customer.Create("Ana", "ana@email.com", new DateTime(1990, 1, 1));
        _db.Add(customer!);

        var result = await _repository.GetByIdAsync(customer!.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(customer.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCustomer_WhenExists()
    {
        var (customer, _) = Customer.Create("Bruno", "bruno@email.com", new DateTime(1990, 1, 1));
        _db.Add(customer!);

        var result = await _repository.GetByEmailAsync("bruno@email.com", TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal("Bruno", result.Data!.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetByEmailAsync("notfound@email.com", TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 10; i++)
        {
            var (customer, _) = Customer.Create($"Customer {i}", $"c{i}@mail.com", new DateTime(1990, 1, 1));
            _db.Add(customer!);
        }

        var request = new GetCustomersRequest
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
    public async Task UpdateAsync_ShouldReplaceCustomer_WhenValid()
    {
        var (original, _) = Customer.Create("Lucas", "lucas@email.com", new DateTime(1990, 1, 1));
        _db.Add(original!);

        var updated = Customer.Create("Lucas Silva", "lucas@email.com", new DateTime(1990, 1, 1)).customer!;
        typeof(Customer).GetProperty("Id")!.SetValue(updated, original!.Id);

        var result = await _repository.UpdateAsync(updated, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.DoesNotContain(original, _db);
        Assert.Contains(updated, _db);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenCustomerIsInvalid()
    {
        var (original, _) = Customer.Create("Paula", "paula@email.com", new DateTime(1990, 1, 1));
        _db.Add(original!);

        var invalid = Customer.Create("", "", new DateTime(1990, 1, 1)).customer!;

        var result = await _repository.UpdateAsync(invalid, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Contains(original, _db);
        Assert.DoesNotContain(invalid, _db);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCustomer_WhenExists()
    {
        var (customer, _) = Customer.Create("Rafael", "rafael@email.com", new DateTime(1990, 1, 1));
        _db.Add(customer!);

        var result = await _repository.DeleteAsync(customer!.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.DoesNotContain(customer, _db);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.DeleteAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByName()
    {
        // Arrange
        var (customer1, _) = Customer.Create("John Doe", "john@email.com", new DateTime(1990, 1, 1));
        var (customer2, _) = Customer.Create("Jane Smith", "jane@email.com", new DateTime(1991, 2, 2));
        var (customer3, _) = Customer.Create("John Wilson", "wilson@email.com", new DateTime(1992, 3, 3));
        var (customer4, _) = Customer.Create("Mary Johnson", "mary@email.com", new DateTime(1993, 4, 4));

        _db.Add(customer1!);
        _db.Add(customer2!);
        _db.Add(customer3!);
        _db.Add(customer4!);

        var request = new GetCustomersRequest
        {
            Name = "John",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, c => Assert.Contains("John", c.Name));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByEmail()
    {
        // Arrange
        var (customer1, _) = Customer.Create("Alice Brown", "alice@gmail.com", new DateTime(1990, 1, 1));
        var (customer2, _) = Customer.Create("Bob Green", "bob@yahoo.com", new DateTime(1991, 2, 2));
        var (customer3, _) = Customer.Create("Charlie White", "charlie@gmail.com", new DateTime(1992, 3, 3));
        var (customer4, _) = Customer.Create("Diana Black", "diana@hotmail.com", new DateTime(1993, 4, 4));

        _db.Add(customer1!);
        _db.Add(customer2!);
        _db.Add(customer3!);
        _db.Add(customer4!);

        var request = new GetCustomersRequest
        {
            Email = "gmail",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, c => Assert.Contains("gmail", c.Email));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByPhone()
    {
        // Arrange
        var (customer1, _) = Customer.Create("Customer 1", "c1@email.com", new DateTime(1990, 1, 1), "11999999999");
        var (customer2, _) = Customer.Create("Customer 2", "c2@email.com", new DateTime(1991, 2, 2), "21888888888");
        var (customer3, _) = Customer.Create("Customer 3", "c3@email.com", new DateTime(1992, 3, 3), "11777777777");
        var (customer4, _) = Customer.Create("Customer 4", "c4@email.com", new DateTime(1993, 4, 4), null);

        _db.Add(customer1!);
        _db.Add(customer2!);
        _db.Add(customer3!);
        _db.Add(customer4!);

        var request = new GetCustomersRequest
        {
            Phone = "11",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, c => Assert.Contains("11", c.Phone ?? ""));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByMultipleFields()
    {
        // Arrange
        var (customer1, _) = Customer.Create("John Silva", "john.silva@gmail.com", new DateTime(1990, 1, 1), "11999999999");
        var (customer2, _) = Customer.Create("John Doe", "john.doe@yahoo.com", new DateTime(1991, 2, 2), "21888888888");
        var (customer3, _) = Customer.Create("Jane Silva", "jane.silva@gmail.com", new DateTime(1992, 3, 3), "11777777777");
        var (customer4, _) = Customer.Create("John Santos", "john.santos@gmail.com", new DateTime(1993, 4, 4), "31666666666");

        _db.Add(customer1!);
        _db.Add(customer2!);
        _db.Add(customer3!);
        _db.Add(customer4!);

        var request = new GetCustomersRequest
        {
            Name = "John",
            Email = "gmail",
            Phone = "11",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        var customer = result.Items.First();
        Assert.Equal("John Silva", customer.Name);
        Assert.Contains("gmail", customer.Email);
        Assert.Contains("11", customer.Phone ?? "");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEmpty_WhenNoMatchesFound()
    {
        // Arrange
        var (customer1, _) = Customer.Create("Alice Brown", "alice@email.com", new DateTime(1990, 1, 1));
        var (customer2, _) = Customer.Create("Bob Green", "bob@email.com", new DateTime(1991, 2, 2));

        _db.Add(customer1!);
        _db.Add(customer2!);

        var request = new GetCustomersRequest
        {
            Name = "NonExistent",
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
        var (customer1, _) = Customer.Create("Customer 1", "c1@email.com", new DateTime(1990, 1, 1));
        var (customer2, _) = Customer.Create("Customer 2", "c2@email.com", new DateTime(1991, 2, 2));

        _db.Add(customer1!);
        _db.Add(customer2!);

        var request = new GetCustomersRequest
        {
            Name = null,
            Email = null,
            Phone = null,
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
        var (customer1, _) = Customer.Create("Customer 1", "c1@email.com", new DateTime(1990, 1, 1));
        var (customer2, _) = Customer.Create("Customer 2", "c2@email.com", new DateTime(1991, 2, 2));

        _db.Add(customer1!);
        _db.Add(customer2!);

        var request = new GetCustomersRequest
        {
            Name = "",
            Email = "   ",
            Phone = "",
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
            var (customer, _) = Customer.Create($"John Customer {i}", $"john{i}@email.com", new DateTime(1990, 1, 1));
            _db.Add(customer!);
        }

        var request = new GetCustomersRequest
        {
            Name = "John",
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
    public async Task GetPagedAsync_ShouldFilterByPartialName()
    {
        // Arrange
        var (customer1, _) = Customer.Create("Michael Jackson", "michael@email.com", new DateTime(1990, 1, 1));
        var (customer2, _) = Customer.Create("Michelle Obama", "michelle@email.com", new DateTime(1991, 2, 2));
        var (customer3, _) = Customer.Create("Peter Parker", "peter@email.com", new DateTime(1992, 3, 3));

        _db.Add(customer1!);
        _db.Add(customer2!);
        _db.Add(customer3!);

        var request = new GetCustomersRequest
        {
            Name = "Mich",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, c => Assert.Contains("Mich", c.Name));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldFilterByPartialEmail()
    {
        // Arrange
        var (customer1, _) = Customer.Create("Customer 1", "test123@domain.com", new DateTime(1990, 1, 1));
        var (customer2, _) = Customer.Create("Customer 2", "test456@domain.com", new DateTime(1991, 2, 2));
        var (customer3, _) = Customer.Create("Customer 3", "user789@other.com", new DateTime(1992, 3, 3));

        _db.Add(customer1!);
        _db.Add(customer2!);
        _db.Add(customer3!);

        var request = new GetCustomersRequest
        {
            Email = "domain",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Items.Count());
        Assert.All(result.Items, c => Assert.Contains("domain", c.Email));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldExcludeCustomersWithNullPhone_WhenFilteringByPhone()
    {
        // Arrange
        var (customer1, _) = Customer.Create("Customer 1", "c1@email.com", new DateTime(1990, 1, 1), "1234567890");
        var (customer2, _) = Customer.Create("Customer 2", "c2@email.com", new DateTime(1991, 2, 2), null);
        var (customer3, _) = Customer.Create("Customer 3", "c3@email.com", new DateTime(1992, 3, 3), "9876543210");

        _db.Add(customer1!);
        _db.Add(customer2!);
        _db.Add(customer3!);

        var request = new GetCustomersRequest
        {
            Phone = "123",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Items);
        Assert.Equal("Customer 1", result.Items.First().Name);
        Assert.NotNull(result.Items.First().Phone);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnOrderedByName()
    {
        // Arrange
        var (customer1, _) = Customer.Create("Zara Wilson", "zara@email.com", new DateTime(1990, 1, 1));
        var (customer2, _) = Customer.Create("Alice Brown", "alice@email.com", new DateTime(1991, 2, 2));
        var (customer3, _) = Customer.Create("Michael Davis", "michael@email.com", new DateTime(1992, 3, 3));

        _db.Add(customer1!);
        _db.Add(customer2!);
        _db.Add(customer3!);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.GetPagedAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        var customers = result.Items.ToList();
        Assert.Equal("Alice Brown", customers[0].Name);
        Assert.Equal("Michael Davis", customers[1].Name);
        Assert.Equal("Zara Wilson", customers[2].Name);
    }
}
