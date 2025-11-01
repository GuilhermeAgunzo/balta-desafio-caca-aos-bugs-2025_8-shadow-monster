namespace BugStore.Application.Requests;

public abstract class PagedRequest : Request
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
