namespace Lasertag.Api;

public static class ApiResultExtensions
{
    public static void EnsureSuccess<TEvent>(this ApiResult<TEvent> apiResult)
    {
        if (!apiResult.Success)
        {
            throw new InvalidOperationException(apiResult.Message);
        }
    }
}