namespace QLHV.Contracts.Results;

/// <summary>
/// Các mã lỗi chuẩn hóa toàn hệ thống
/// </summary>
public static class ErrorCodes
{
    public const string REQUIRED = "REQUIRED";
    public const string INVALID_FORMAT = "INVALID_FORMAT";
    public const string DUPLICATE_CODE = "DUPLICATE_CODE";
    public const string NOT_FOUND = "NOT_FOUND";
    public const string FORBIDDEN = "FORBIDDEN";
    public const string INVALID_RANGE = "INVALID_RANGE";
    public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
    public const string ACCOUNT_LOCKED = "ACCOUNT_LOCKED";
    public const string UNEXPECTED_ERROR = "UNEXPECTED_ERROR";
}

/// <summary>
/// Chi tiết lỗi thao tác
/// </summary>
public class OperationError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public OperationError() { }

    public OperationError(string code, string message)
    {
        Code = code;
        Message = message;
    }
}

/// <summary>
/// Kết quả thao tác không trả về dữ liệu
/// </summary>
public class OperationResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public OperationError? Error { get; set; }

    public static OperationResult Success(string message = "Thao tác thành công.")
    {
        return new OperationResult
        {
            IsSuccess = true,
            Message = message,
            Error = null
        };
    }

    public static OperationResult Failed(string code, string message)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Message = message,
            Error = new OperationError(code, message)
        };
    }

    public static OperationResult Failed(OperationError error)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Message = error.Message,
            Error = error
        };
    }
}

/// <summary>
/// Kết quả thao tác có trả về dữ liệu kiểu T
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
public class OperationResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public OperationError? Error { get; set; }
    public T? Data { get; set; }

    public static OperationResult<T> Success(T data, string message = "Thao tác thành công.")
    {
        return new OperationResult<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Error = null
        };
    }

    public static OperationResult<T> Failed(string code, string message, T? defaultData = default)
    {
        return new OperationResult<T>
        {
            IsSuccess = false,
            Message = message,
            Data = defaultData,
            Error = new OperationError(code, message)
        };
    }

    public static OperationResult<T> Failed(OperationError error, T? defaultData = default)
    {
        return new OperationResult<T>
        {
            IsSuccess = false,
            Message = error.Message,
            Data = defaultData,
            Error = error
        };
    }
}
