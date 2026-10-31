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
/// Đảm bảo an toàn không gây crash ứng dụng nếu lỗi I/O
/// </summary>
public class FileAppLogger : IAppLogger
{
    private readonly string _logDirectory;
    private readonly object _lock = new();

    public FileAppLogger(string? customLogDir = null)
    {
        _logDirectory = customLogDir ?? Path.Combine(AppContext.BaseDirectory, "logs");
    }

    public void LogInfo(string message)
    {
        WriteLog("INFO", message, null);
    }

    public void LogWarning(string message)
    {
        WriteLog("WARN", message, null);
    }

    public void LogError(string message, Exception? exception = null)
    {
        WriteLog("ERROR", message, exception);
    }

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

            var sanitizedMessage = SanitizeMessage(message);

            var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {sanitizedMessage}";
            if (exception != null)
            {
                logLine += Environment.NewLine + $"Exception: {exception.GetType().FullName}: {exception.Message}"
                         + Environment.NewLine + $"StackTrace: {exception.StackTrace}";
            }
            logLine += Environment.NewLine;

            lock (_lock)
            {
                File.AppendAllText(filePath, logLine);
            }
        }
        catch (Exception ex)
        {
            // Không được crash ứng dụng nếu ghi log thất bại
            System.Diagnostics.Debug.WriteLine($"[FileAppLogger Failure] {ex.Message}");
        }
    }

    /// <summary>
    /// Loại bỏ nguy cơ log mật khẩu hoặc chuỗi nhạy cảm
    /// </summary>
    private static string SanitizeMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return string.Empty;
        
        // Thay thế các mẫu mật khẩu nếu có
        var result = System.Text.RegularExpressions.Regex.Replace(
            message,
            @"(password|matkhau|pwd)\s*[:=]\s*[^\s,;]+",
            "$1: [REDACTED]",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Thay thế connection string nếu có
        result = System.Text.RegularExpressions.Regex.Replace(
            result,
            @"(Password|User ID|Initial Catalog)\s*=\s*[^;]+;",
            "$1=[REDACTED];",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return result;
    }
}
