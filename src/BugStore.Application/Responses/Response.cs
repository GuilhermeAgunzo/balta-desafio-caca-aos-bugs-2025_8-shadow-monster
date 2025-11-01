using System.Text.Json.Serialization;

namespace BugStore.Application.Responses;

public class Response<TData>
{
    private readonly int _statusCode;

    public TData? Data { get; set; }
    public string[]? Messages { get; set; }

    public int StatusCode => _statusCode;
    [JsonIgnore]
    public bool IsSuccess => _statusCode >= 200 && _statusCode < 300;

    [JsonConstructor]
    public Response()
    {
        _statusCode = 200;
    }

    public Response(TData? data, int code = 200, string[]? messages = null)
    {
        _statusCode = code;
        Data = data;
        Messages = messages ?? [];
    }
}
