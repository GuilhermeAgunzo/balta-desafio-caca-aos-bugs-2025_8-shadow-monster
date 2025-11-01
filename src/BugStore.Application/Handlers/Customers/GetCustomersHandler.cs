using BugStore.Application.Abstractions.Handlers.Customers;
using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Requests.Customers;
using BugStore.Application.Responses;
using BugStore.Domain.Entities;

namespace BugStore.Application.Handlers.Customers;

public class GetCustomersHandler(ICustomerRepository customerRepository) : IGetCustomersHandler
{
    public async Task<PagedResponse<IEnumerable<Customer>>> HandleAsync(GetCustomersRequest req, CancellationToken cancellationToken = default)
    {
        var result = await customerRepository.GetPagedAsync(req, cancellationToken);

        return result.Success
            ? new PagedResponse<IEnumerable<Customer>>(data: result.Items, totalCount: result.TotalCount, currentPage: req.PageNumber, pageSize: req.PageSize)
            : new PagedResponse<IEnumerable<Customer>>(null, 400, [result.ErrorMessage ?? "Unspecified error."]);
    }
}
