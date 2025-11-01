using BugStore.Application.Handlers.Customers;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Tests.Repositories;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.UnitTests.Handlers.Customers;

public class GetCustomersHandlerTest
{
    private readonly List<Customer> _db = [];
    private readonly FakeCustomerRepository _repository;
    private readonly GetCustomersHandler _handler;

    public GetCustomersHandlerTest()
    {
        _repository = new FakeCustomerRepository(_db);
        _handler = new GetCustomersHandler(_repository);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenDatabaseIsEmpty()
    {
        // Arrange
        var request = new GetCustomersRequest
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
    public async Task HandleAsync_ShouldReturnAllCustomers_WhenPageSizeIsGreaterThanCount()
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

        var request = new GetCustomersRequest
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
            var customer = Customer.Create(
     $"Customer {i:D2}",
    $"customer{i}@email.com",
        new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var request = new GetCustomersRequest
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
            var customer = Customer.Create(
          $"Customer {i:D2}",
               $"customer{i}@email.com",
                new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var request = new GetCustomersRequest
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
        var customer = Customer.Create(
            "Single Customer",
 "single@email.com",
            new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);

        var request = new GetCustomersRequest
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
            var customer = Customer.Create(
       $"Customer {i}",
               $"customer{i}@email.com",
                       new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var request = new GetCustomersRequest
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
    public async Task HandleAsync_ShouldReturnSingleCustomer_WhenOnlyOneExists()
    {
        // Arrange
        var customer = Customer.Create(
  "John Doe",
            "john.doe@email.com",
            new DateTime(1990, 1, 1),
            "123456789").customer!;
        _db.Add(customer);

        var request = new GetCustomersRequest
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

        var returnedCustomer = response.Data.First();
        Assert.Equal(customer.Id, returnedCustomer.Id);
        Assert.Equal("John Doe", returnedCustomer.Name);
        Assert.Equal("john.doe@email.com", returnedCustomer.Email);
        Assert.Equal("123456789", returnedCustomer.Phone);
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateTotalPagesCorrectly_WhenTotalCountIsDivisibleByPageSize()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var customer = Customer.Create(
 $"Customer {i}",
            $"customer{i}@email.com",
                new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var request = new GetCustomersRequest
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
            var customer = Customer.Create(
                  $"Customer {i}",
          $"customer{i}@email.com",
        new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var request = new GetCustomersRequest
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
    public async Task HandleAsync_ShouldReturnCustomersWithVariousPhoneValues()
    {
        // Arrange
        var customerWithPhone = Customer.Create(
             "Has Phone",
                "hasphone@email.com",
         new DateTime(1990, 1, 1),
             "111222333").customer!;
        var customerWithoutPhone = Customer.Create(
  "No Phone",
       "nophone@email.com",
    new DateTime(1991, 2, 2),
         null).customer!;

        _db.Add(customerWithPhone);
        _db.Add(customerWithoutPhone);

        var request = new GetCustomersRequest
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

        var customers = response.Data.ToList();
        Assert.Contains(customers, c => c.Phone == "111222333");
        Assert.Contains(customers, c => c.Phone == null);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCustomersOrderedByName()
    {
        // Arrange
        var customer1 = Customer.Create(
          "Zoe Customer",
                 "zoe@email.com",
        new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
    "Alice Customer",
"alice@email.com",
         new DateTime(1991, 2, 2)).customer!;
        var customer3 = Customer.Create(
    "Mike Customer",
              "mike@email.com",
              new DateTime(1992, 3, 3)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomersRequest
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

        var customers = response.Data.ToList();
        Assert.Equal("Alice Customer", customers[0].Name);
        Assert.Equal("Mike Customer", customers[1].Name);
        Assert.Equal("Zoe Customer", customers[2].Name);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotModifyDatabase_WhenRetrievingCustomers()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            var customer = Customer.Create(
              $"Customer {i}",
                $"customer{i}@email.com",
              new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var initialCount = _db.Count;
        var initialCustomers = _db.ToList();

        var request = new GetCustomersRequest
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
        Assert.All(initialCustomers, c => Assert.Contains(c, _db));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectPageData_WhenPageSizeIsOne()
    {
        // Arrange
        var customer1 = Customer.Create(
                 "Customer A",
                 "a@email.com",
          new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
        "Customer B",
            "b@email.com",
            new DateTime(1991, 2, 2)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);

        var request = new GetCustomersRequest
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
            var customer = Customer.Create(
       $"Customer {i:D2}",
                  $"customer{i}@email.com",
    new DateTime(1990, 1, 1)).customer!;
            _db.Add(customer);
        }

        var pageSize = 5;
        var allCustomers = new List<Customer>();

        // Act - Get all pages
        for (int page = 1; page <= 3; page++)
        {
            var request = new GetCustomersRequest
            {
                PageNumber = page,
                PageSize = pageSize
            };

            var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);
            allCustomers.AddRange(response.Data!);
        }

        // Assert
        Assert.Equal(15, allCustomers.Count);
        Assert.Equal(_db.Count, allCustomers.Count);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCorrectStatusCode_OnSuccess()
    {
        // Arrange
        var customer = Customer.Create(
                 "Test Customer",
                 "test@email.com",
         new DateTime(1990, 1, 1)).customer!;
        _db.Add(customer);

        var request = new GetCustomersRequest
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
            var customer = Customer.Create(
    $"Customer {i}",
          $"customer{i}@email.com",
            new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var request = new GetCustomersRequest
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
            var customer = Customer.Create(
                      $"Customer {i}",
                      $"customer{i}@email.com",
                        new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var request = new GetCustomersRequest
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

    #region Filter Tests

    [Fact]
    public async Task HandleAsync_ShouldFilterByName_WhenNameFilterIsProvided()
    {
        // Arrange
        var customer1 = Customer.Create(
            "John Doe",
          "john@email.com",
     new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
            "Jane Smith",
            "jane@email.com",
        new DateTime(1991, 2, 2)).customer!;
        var customer3 = Customer.Create(
     "John Williams",
  "williams@email.com",
 new DateTime(1992, 3, 3)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Name = "John"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.Equal(2, response.TotalCount);
        Assert.All(response.Data, c => Assert.Contains("John", c.Name));
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByEmail_WhenEmailFilterIsProvided()
    {
        // Arrange
        var customer1 = Customer.Create(
            "Customer One",
     "john.doe@gmail.com",
 new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
    "Customer Two",
     "jane.smith@yahoo.com",
         new DateTime(1991, 2, 2)).customer!;
        var customer3 = Customer.Create(
            "Customer Three",
    "john.williams@gmail.com",
            new DateTime(1992, 3, 3)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Email = "gmail"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.Equal(2, response.TotalCount);
        Assert.All(response.Data, c => Assert.Contains("gmail", c.Email));
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByPhone_WhenPhoneFilterIsProvided()
    {
        // Arrange
        var customer1 = Customer.Create(
            "Customer One",
            "customer1@email.com",
            new DateTime(1990, 1, 1),
            "11999998888").customer!;
        var customer2 = Customer.Create(
            "Customer Two",
            "customer2@email.com",
            new DateTime(1991, 2, 2),
            "21888887777").customer!;
        var customer3 = Customer.Create(
            "Customer Three",
            "customer3@email.com",
            new DateTime(1992, 3, 3),
            "11777776666").customer!;

        var customer4 = Customer.Create(
            "Customer Four",
            "customer4@email.com",
            new DateTime(1993, 4, 4),
            null).customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);
        _db.Add(customer4);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Phone = "119"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data!);
        Assert.Equal(1, response.TotalCount);
        Assert.All(response.Data!, c => Assert.Contains("119", c.Phone));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmpty_WhenPhoneFilterMatchesNoCustomers()
    {
        // Arrange
        var customer1 = Customer.Create(
            "Customer One",
            "customer1@email.com",
            new DateTime(1990, 1, 1),
            "11999998888").customer!;
        var customer2 = Customer.Create(
         "Customer Two",
          "customer2@email.com",
                 new DateTime(1991, 2, 2),
          null).customer!;

        _db.Add(customer1);
        _db.Add(customer2);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Phone = "999567"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Empty(response.Data);
        Assert.Equal(0, response.TotalCount);
    }

    [Fact]
    public async Task HandleAsync_ShouldApplyMultipleFilters_WhenNameAndEmailProvided()
    {
        // Arrange
        var customer1 = Customer.Create(
            "John Doe",
"john.doe@gmail.com",
            new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
            "John Smith",
        "john.smith@yahoo.com",
            new DateTime(1991, 2, 2)).customer!;
        var customer3 = Customer.Create(
            "Jane Doe",
   "jane.doe@gmail.com",
  new DateTime(1992, 3, 3)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Name = "John",
            Email = "gmail"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data);
        Assert.Equal(1, response.TotalCount);
        Assert.Equal("John Doe", response.Data.First().Name);
        Assert.Contains("gmail", response.Data.First().Email);
    }

    [Fact]
    public async Task HandleAsync_ShouldApplyAllFilters_WhenNameEmailAndPhoneProvided()
    {
        // Arrange
        var customer1 = Customer.Create(
            "John Doe",
   "john.doe@gmail.com",
   new DateTime(1990, 1, 1),
        "11999998888").customer!;
        var customer2 = Customer.Create(
            "John Smith",
            "john.smith@gmail.com",
                new DateTime(1991, 2, 2),
      "21888887777").customer!;
        var customer3 = Customer.Create(
            "Jane Doe",
            "jane.doe@gmail.com",
  new DateTime(1992, 3, 3),
    "11777776666").customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Name = "John",
            Email = "gmail",
            Phone = "119"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data);
        Assert.Equal(1, response.TotalCount);
        Assert.Equal("John Doe", response.Data.First().Name);
        Assert.Equal("john.doe@gmail.com", response.Data.First().Email);
        Assert.Equal("11999998888", response.Data.First().Phone);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmpty_WhenFiltersMatchNoCustomers()
    {
        // Arrange
        var customer1 = Customer.Create(
            "John Doe",
     "john@email.com",
new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
     "Jane Smith",
            "jane@email.com",
          new DateTime(1991, 2, 2)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Name = "NonExistent"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Empty(response.Data);
        Assert.Equal(0, response.TotalCount);
        Assert.Equal(0, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldApplyPaginationWithFilters()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var customer = Customer.Create(
   $"John Customer {i:D2}",
 $"john{i}@email.com",
       new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var customer11 = Customer.Create(
            "Jane Smith",
         "jane@email.com",
new DateTime(1991, 2, 2)).customer!;
        _db.Add(customer11);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 5,
            Name = "John"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Data.Count());
        Assert.Equal(10, response.TotalCount);
        Assert.Equal(1, response.CurrentPage);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(2, response.TotalPages);
        Assert.All(response.Data, c => Assert.Contains("John", c.Name));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSecondPageWithFilters()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var customer = Customer.Create(
             $"John Customer {i:D2}",
              $"john{i}@email.com",
               new DateTime(1990, 1, i)).customer!;
            _db.Add(customer);
        }

        var request = new GetCustomersRequest
        {
            PageNumber = 2,
            PageSize = 5,
            Name = "John"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(5, response.Data.Count());
        Assert.Equal(10, response.TotalCount);
        Assert.Equal(2, response.CurrentPage);
        Assert.Equal(5, response.PageSize);
        Assert.Equal(2, response.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_ShouldIgnoreNullFilters()
    {
        // Arrange
        var customer1 = Customer.Create(
     "John Doe",
      "john@email.com",
      new DateTime(1990, 1, 1),
            "11999998888").customer!;
        var customer2 = Customer.Create(
            "Jane Smith",
            "jane@email.com",
    new DateTime(1991, 2, 2),
            "21888887777").customer!;

        _db.Add(customer1);
        _db.Add(customer2);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Name = null,
            Email = null,
            Phone = null
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.Equal(2, response.TotalCount);
    }

    [Fact]
    public async Task HandleAsync_ShouldIgnoreEmptyStringFilters()
    {
        // Arrange
        var customer1 = Customer.Create(
          "John Doe",
  "john@email.com",
        new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
               "Jane Smith",
      "jane@email.com",
         new DateTime(1991, 2, 2)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Name = "",
            Email = "",
            Phone = ""
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.Equal(2, response.TotalCount);
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByPartialEmail()
    {
        // Arrange
        var customer1 = Customer.Create(
         "Customer One",
"test@gmail.com",
  new DateTime(1990, 1, 1)).customer!;
        var customer2 = Customer.Create(
            "Customer Two",
    "user@hotmail.com",
      new DateTime(1991, 2, 2)).customer!;
        var customer3 = Customer.Create(
        "Customer Three",
                  "admin@gmail.com",
                  new DateTime(1992, 3, 3)).customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Email = "test"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Single(response.Data);
        Assert.Equal("test@gmail.com", response.Data.First().Email);
    }

    [Fact]
    public async Task HandleAsync_ShouldExcludeCustomersWithNullPhone_WhenPhoneFilterIsApplied()
    {
        // Arrange
        var customer1 = Customer.Create(
      "Customer One",
              "customer1@email.com",
     new DateTime(1990, 1, 1),
              "11999998888").customer!;
        var customer2 = Customer.Create(
 "Customer Two",
            "customer2@email.com",
  new DateTime(1991, 2, 2),
      null).customer!;
        var customer3 = Customer.Create(
       "Customer Three",
              "customer3@email.com",
              new DateTime(1992, 3, 3),
     "11988887777").customer!;

        _db.Add(customer1);
        _db.Add(customer2);
        _db.Add(customer3);

        var request = new GetCustomersRequest
        {
            PageNumber = 1,
            PageSize = 10,
            Phone = "119"
        };

        // Act
        var response = await _handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Data.Count());
        Assert.All(response.Data, c => Assert.NotNull(c.Phone));
    }

    #endregion
}
