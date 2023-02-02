namespace Lasertag.Api;

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

    public T? Output { get; }

    public bool Success { get; }

    public string? Message { get; }
}