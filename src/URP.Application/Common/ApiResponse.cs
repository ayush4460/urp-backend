namespace URP.Application.Common;

public sealed class ApiResponse<T>
{
    public bool          Success { get; init; }
    public string        Message { get; init; } = string.Empty;
    public T?            Data    { get; init; }
    public List<string>? Errors  { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}

public sealed class ApiResponse
{
    public bool          Success { get; init; }
    public string        Message { get; init; } = string.Empty;
    public List<string>? Errors  { get; init; }

    public static ApiResponse Ok(string message = "Success") =>
        new() { Success = true, Message = message };

    public static ApiResponse Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors };
}
