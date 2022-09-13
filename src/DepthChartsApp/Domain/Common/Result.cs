namespace DepthChartsApp.Domain.Common;

/// <summary>
/// This class is in a very simple stage and it could grow to better handle other scenarios, for example we could handle
/// exceptions here instead of throwing them, that would improve the application performance
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string[] Errors { get; }

    protected Result()
    {
        IsSuccess = true;
        Errors = Array.Empty<string>();
    }

    protected Result(string singleError)
    {
        IsSuccess = false;
        Errors = new[] { singleError };
    }

    protected Result(string[] errors)
    {
        IsSuccess = false;
        Errors = errors;
    }

    public static Result Success() => new();
    public static Result<T> Success<T>(T value) => new(value);
    public static Result Failure(string singleError) => new(singleError);
    public static Result<T> Failure<T>(string singleError) => new(singleError);
    public static Result<T> Failure<T>(string[] errors) => new(errors);
}

public sealed class Result<T> : Result
{
    private readonly T? _value;
    public T Value => !IsSuccess || _value is null ? throw new InvalidOperationException() : _value;

    internal Result(T value)
    {
        _value = value;
    }

    internal Result(string error)
        : base(error)
    {
    }

    internal Result(string[] errors)
        : base(errors)
    {
    }
}