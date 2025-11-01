using System.Text.Json.Serialization;

namespace BugStore.Application.Common;

public class PagedResult<TItem>
{
    public IEnumerable<TItem> Items { get; init; } = [];
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    [JsonConstructor]
    public PagedResult()
    {

    }

    private PagedResult(IEnumerable<TItem> items, int totalCount, int currentPage, int pageSize)
    {
        Success = true;
        Items = items;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }

    private PagedResult(string errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
    }

    public static PagedResult<TItem> Ok(IEnumerable<TItem> items, int totalCount, int currentPage, int pageSize)
        => new(items, totalCount, currentPage, pageSize);

    public static PagedResult<TItem> Fail(string? errorMessage = null)
        => new(errorMessage);
}
