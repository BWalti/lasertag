using Orleans;

namespace Lasertag.Api;

[GenerateSerializer]
public record ApiResult<T>
{
    public ApiResult()
    {
    }

    public ApiResult(T output)
    {
        Output = output;
        Success = true;
    }

    public ApiResult(string message)
    {
        Message = message;
        Success = false;
    }

    public ApiResult(Exception ex)
    {
        Message = ex.InnerException == null
            ? ex.Message
            : $"{ex.Message}; InnerException {ex.InnerException.Message}";

        Success = false;
    }

    [Id(0)] public T? Output { get; }

    [Id(1)] public bool Success { get; }

    [Id(2)] public string? Message { get; }
}