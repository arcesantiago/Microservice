namespace Microservice.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? Message { get; protected set; }
    public string? Error { get; protected set; }

    protected Result(bool isSuccess, string? message = null, string? error = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Error = error;
    }

    public static Result Success(string? message = null) 
        => new(true, message);

    public static Result Failure(string error) 
        => new(false, error: error);
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    protected Result(bool isSuccess, T? data = default, string? message = null, string? error = null)
        : base(isSuccess, message, error)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string? message = null) 
        => new(true, data, message);

    public static new Result<T> Failure(string error) 
        => new(false, error: error);
}
