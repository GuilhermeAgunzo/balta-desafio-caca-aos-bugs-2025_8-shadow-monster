using System.Text.Json.Serialization;

namespace BugStore.Application.Common;

public class Result<TData>
{
    public TData? Data { get; init; }
    public string? ErrorMessage { get; init; }
    public bool Success { get; init; }

    [JsonConstructor]
    public Result()
    {

    }

    private Result(TData? data)
    {
        Success = true;
        Data = data;
    }

    private Result(string errorMessage)
    {
        Success = false;
        ErrorMessage = errorMessage;
    }

    public static Result<TData> Ok(TData data)
        => new(data);

    public static Result<TData> Fail(string errorMessage)
        => new(errorMessage);
}
