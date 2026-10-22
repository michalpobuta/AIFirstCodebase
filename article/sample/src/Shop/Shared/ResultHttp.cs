namespace Shop.Shared;

/// <summary>The one place where a use-case <see cref="Error"/> becomes an HTTP status code.</summary>
public static class ResultHttp
{
    public static IResult ToHttpResult(this Result result) =>
        result.Error is null ? Results.NoContent() : Problem(result.Error);

    public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess) =>
        result.Error is null ? onSuccess(result.Value!) : Problem(result.Error);

    private static IResult Problem(Error error) => Results.Problem(error.Message, statusCode: error.Kind switch
    {
        ErrorKind.Invalid => StatusCodes.Status400BadRequest,
        ErrorKind.NotFound => StatusCodes.Status404NotFound,
        ErrorKind.Conflict => StatusCodes.Status409Conflict,
    });
}
