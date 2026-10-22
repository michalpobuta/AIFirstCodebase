namespace Shop.Shared;

public enum ErrorKind
{
    Invalid,
    NotFound,
    Conflict,
}

public sealed record Error(ErrorKind Kind, string Message)
{
    public static Error Invalid(string message) => new(ErrorKind.Invalid, message);
    public static Error NotFound(string message) => new(ErrorKind.NotFound, message);
    public static Error Conflict(string message) => new(ErrorKind.Conflict, message);
}

/// <summary>Outcome of a use case that returns nothing: success, or an <see cref="Shared.Error"/>.</summary>
public readonly record struct Result(Error? Error)
{
    public static Result Success => default;

    public bool IsSuccess => Error is null;

    public static implicit operator Result(Error error) => new(error);
}

/// <summary>Outcome of a use case that returns a value: the value, or an <see cref="Shared.Error"/>.</summary>
public readonly record struct Result<T>(T? Value, Error? Error)
{
    public bool IsSuccess => Error is null;

    public static implicit operator Result<T>(T value) => new(value, null);
    public static implicit operator Result<T>(Error error) => new(default, error);
}
