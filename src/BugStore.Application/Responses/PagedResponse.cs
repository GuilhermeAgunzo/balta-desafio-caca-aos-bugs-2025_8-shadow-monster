using System.Text.Json.Serialization;

namespace BugStore.Application.Responses;

public class PagedResponse<TData> : Response<TData>
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    [JsonConstructor]
    public PagedResponse(TData? data, int totalCount, int currentPage, int pageSize) : base(data)
    {
        Data = data;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }

    public PagedResponse(TData? data, int statusCode = 200, string[]? messages = null) : base(data, statusCode, messages)
    {

    }
}
