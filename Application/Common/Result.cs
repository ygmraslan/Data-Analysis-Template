namespace DataAnalysis.Application.Common;

public sealed record Result<T>(bool Success, string Message, T? Value, string? Code = null)
{
    public static Result<T> Ok(T value, string message = "") => new(true, message, value);
    public static Result<T> Fail(string message, string? code = null) => new(false, message, default, code);
}

public sealed record Result(bool Success, string Message, string? Code = null)
{
    public static Result Ok(string message = "") => new(true, message);
    public static Result Fail(string message, string? code = null) => new(false, message, code);
}