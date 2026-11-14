using System.Text;
using System.Text.RegularExpressions;

namespace QLHV.Core.Logging;

/// <summary>
/// Giao diện ghi log chuẩn cho toàn bộ hệ thống
/// </summary>
public interface IAppLogger
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? exception = null);
}

/// <summary>
/// Triển khai ghi log ra file trong AppContext.BaseDirectory\logs
/// Đảm bảo an toàn không gây crash ứng dụng nếu lỗi I/O và tự động che giấu thông tin nhạy cảm
/// </summary>
public class FileAppLogger : IAppLogger
{
    private readonly string _logDirectory;
    private readonly object _lock = new();

    public FileAppLogger(string? customLogDir = null)
    {
        _logDirectory = customLogDir ?? Path.Combine(AppContext.BaseDirectory, "logs");
    }

    public void LogInfo(string message) => WriteLog("INFO", message, null);

    public void LogWarning(string message) => WriteLog("WARN", message, null);

    public void LogError(string message, Exception? exception = null) => WriteLog("ERROR", message, exception);

    private void WriteLog(string level, string message, Exception? exception)
    {
        try
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            var fileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
            var filePath = Path.Combine(_logDirectory, fileName);

            var sanitizedMessage = Sanitize(message);

            var sb = new StringBuilder();
            sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {sanitizedMessage}");
            sb.AppendLine();

            if (exception != null)
            {
                AppendSanitizedException(sb, exception);
            }

            lock (_lock)
            {
                File.AppendAllText(filePath, sb.ToString());
            }
        }
        catch (Exception ex)
        {
            // Không được crash ứng dụng nếu ghi log thất bại và làm sạch thông điệp lỗi trước khi xuất Debug
            System.Diagnostics.Debug.WriteLine($"[FileAppLogger Failure] {Sanitize(ex.Message)}");
        }
    }

    private static void AppendSanitizedException(StringBuilder sb, Exception exception)
    {
        var current = exception;
        int depth = 0;
        while (current != null && depth < 10)
        {
            var prefix = depth == 0 ? "Exception" : "Inner Exception";
            sb.AppendLine($"{prefix}: {current.GetType().FullName}: {Sanitize(current.Message)}");
            if (!string.IsNullOrWhiteSpace(current.StackTrace))
            {
                sb.AppendLine($"StackTrace: {Sanitize(current.StackTrace)}");
            }
            current = current.InnerException;
            depth++;
        }
    }

    /// <summary>
    /// Làm sạch thông tin nhạy cảm: Password, Pwd, User ID, UID, Connection String...
    /// Ưu tiên nhận diện giá trị quoted (dấu nháy kép hoặc đơn, xử lý escape/doubled quote) trước unquoted.
    /// Giữ lại thông tin hữu ích như tên Database trong log thông thường nếu không nằm trong Connection String bị che toàn bộ.
    /// </summary>
    public static string Sanitize(string? text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        // 1. Che Connection String khi xuất hiện dạng khóa: "Connection String=...", "Connection String: ..."
        // Ưu tiên giá trị trong dấu nháy kép/đơn trước, sau đó mới đến unquoted tới hết dòng
        var result = Regex.Replace(
            text,
            @"(?<key>connection\s*string)\s*[:=]\s*(?:""(?:[^""\\]|\\.|"""")*""|'(?:[^'\\]|\\.|'')*'|[^\r\n]+)",
            "${key}=[REDACTED]",
            RegexOptions.IgnoreCase);

        // 2. Che Password, Pwd, MatKhau: ưu tiên quoted có khoảng trắng/chấm phẩy trước unquoted
        result = Regex.Replace(
            result,
            @"(?<key>password|pwd|matkhau)\s*[:=]\s*(?:""(?:[^""\\]|\\.|"""")*""|'(?:[^'\\]|\\.|'')*'|[^;\r\n\s]+)",
            "${key}=[REDACTED]",
            RegexOptions.IgnoreCase);

        // 3. Che User ID, UID: ưu tiên quoted có khoảng trắng trước unquoted
        result = Regex.Replace(
            result,
            @"(?<key>user\s*id|uid)\s*[:=]\s*(?:""(?:[^""\\]|\\.|"""")*""|'(?:[^'\\]|\\.|'')*'|[^;\r\n\s]+)",
            "${key}=[REDACTED]",
            RegexOptions.IgnoreCase);

        return result;
    }
}
