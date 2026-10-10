public enum ResultStatus { Invalid = 0, Success, NotFound, };

public readonly record struct Result<T>
{
    public ResultStatus Status { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(ResultStatus status, T? value, string? error = null)
    {
        (Status, Value, Error) = (status, value, error);
    }

    public static Result<T> Ok(T value) => new(ResultStatus.Success, value);
    public static Result<T> NotFound(string error) => new(ResultStatus.NotFound, default, error);
    public static Result<T> Invalid(string error) => new(ResultStatus.Invalid, default, error);
}

public readonly record struct Result(ResultStatus Status, string? Error = null)
{
    public static Result Ok() => new(ResultStatus.Success, null);
    public static Result NotFound(string error) => new(ResultStatus.NotFound, error);
    public static Result Invalid(string error) => new(ResultStatus.Invalid, error);
}