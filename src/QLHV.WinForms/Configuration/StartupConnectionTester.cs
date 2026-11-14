using Microsoft.Data.SqlClient;

namespace QLHV.WinForms.Configuration;

public sealed class ConnectionTestResult
{
    public bool IsSuccess { get; init; }
    public string SanitizedInfo { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }

    public static ConnectionTestResult Success(string sanitizedInfo) => new()
    {
        IsSuccess = true,
        SanitizedInfo = sanitizedInfo
    };

    public static ConnectionTestResult Failure(string sanitizedInfo, string errorMessage) => new()
    {
        IsSuccess = false,
        SanitizedInfo = sanitizedInfo,
        ErrorMessage = errorMessage
    };
}

/// <summary>
/// Kiểm tra kết nối SQL Server trước khi mở màn hình đăng nhập
/// Đảm bảo không kết nối nhầm database và không tự động fallback về Fake
/// </summary>
public static class StartupConnectionTester
{
    public static ConnectionTestResult TestConnection(string? connectionString)
    {
        var sanitized = AppConfigLoader.SanitizeConnectionString(connectionString);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return ConnectionTestResult.Failure(sanitized, "Chuỗi kết nối SQL Server rỗng.");
        }

        try
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            // Kiểm tra bảo vệ không được trỏ vào QLHV_Test
            if (string.Equals(builder.InitialCatalog, AppConfigLoader.TestDatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                return ConnectionTestResult.Failure(sanitized,
                    $"Không được phép chạy ứng dụng với database kiểm thử '{AppConfigLoader.TestDatabaseName}'.");
            }

            if (!string.Equals(builder.InitialCatalog, AppConfigLoader.ExpectedDatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                return ConnectionTestResult.Failure(sanitized,
                    $"Database phải là '{AppConfigLoader.ExpectedDatabaseName}', không phải '{builder.InitialCatalog}'.");
            }

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT TOP (1) 1 FROM dbo.TaiKhoan;", connection);
            command.CommandTimeout = 5;
            command.ExecuteScalar();

            return ConnectionTestResult.Success(sanitized);
        }
        catch (SqlException ex)
        {
            return ConnectionTestResult.Failure(sanitized, $"Lỗi SQL Server ({ex.Number}): {ex.Message}");
        }
        catch (Exception ex)
        {
            return ConnectionTestResult.Failure(sanitized, $"Lỗi kết nối: {ex.Message}");
        }
    }
}

