using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace QLHV.WinForms.Configuration;

/// <summary>
/// Nạp và kiểm tra hợp lệ cấu hình ứng dụng từ JSON và Biến môi trường
/// Thứ tự ưu tiên: Environment Variables > appsettings.Local.json > appsettings.json
/// </summary>
public static class AppConfigLoader
{
    public const string EnvDataProviderKey = "QLHV_DATA_PROVIDER";
    public const string EnvConnectionStringKey = "QLHV_CONNECTION_STRING";
    public const string ExpectedDatabaseName = "QLHV";
    public const string TestDatabaseName = "QLHV_Test";

    public static AppConfig Load(
        string? baseDirectory = null,
        Func<string, string?>? getEnvironmentVariable = null)
    {
        var dir = string.IsNullOrWhiteSpace(baseDirectory)
            ? AppContext.BaseDirectory
            : baseDirectory;

        getEnvironmentVariable ??= Environment.GetEnvironmentVariable;

        string? rawProvider = null;
        string? connectionString = null;

        // 1. Đọc từ file cấu hình cơ bản: appsettings.json
        var basePath = Path.Combine(dir, "appsettings.json");
        if (File.Exists(basePath))
        {
            ReadFromJsonFile(basePath, ref rawProvider, ref connectionString);
        }

        // 2. Đọc ghi đè từ cấu hình máy cục bộ: appsettings.Local.json (nếu có)
        var localPath = Path.Combine(dir, "appsettings.Local.json");
        if (File.Exists(localPath))
        {
            ReadFromJsonFile(localPath, ref rawProvider, ref connectionString);
        }

        // 3. Đọc ghi đè cao nhất từ Biến môi trường
        var envProvider = getEnvironmentVariable(EnvDataProviderKey);
        if (!string.IsNullOrWhiteSpace(envProvider))
        {
            rawProvider = envProvider.Trim();
        }

        var envConn = getEnvironmentVariable(EnvConnectionStringKey);
        if (!string.IsNullOrWhiteSpace(envConn))
        {
            connectionString = envConn.Trim();
        }

        // 4. Kiểm tra hợp lệ DataProvider
        if (string.IsNullOrWhiteSpace(rawProvider))
        {
            throw new InvalidOperationException(
                "Cấu hình 'DataProvider' không được để trống. Giá trị hợp lệ: 'SqlServer' hoặc 'Fake'.");
        }

        DataProviderMode providerMode;
        if (string.Equals(rawProvider, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            providerMode = DataProviderMode.SqlServer;
        }
        else if (string.Equals(rawProvider, "Fake", StringComparison.OrdinalIgnoreCase))
        {
            providerMode = DataProviderMode.Fake;
        }
        else
        {
            throw new InvalidOperationException(
                $"Giá trị DataProvider '{rawProvider}' không hợp lệ. Chỉ chấp nhận 'SqlServer' hoặc 'Fake'.");
        }

        // 5. Nếu chế độ SqlServer, kiểm tra hợp lệ chuỗi kết nối
        if (providerMode == DataProviderMode.SqlServer)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Chuỗi kết nối SQL Server (ConnectionStrings:QLHV hoặc biến môi trường QLHV_CONNECTION_STRING) không được để trống khi chạy ở chế độ SqlServer.");
            }

            SqlConnectionStringBuilder builder;
            try
            {
                builder = new SqlConnectionStringBuilder(connectionString);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Chuỗi kết nối SQL Server không đúng định dạng.", ex);
            }

            if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
            {
                throw new InvalidOperationException(
                    "Chuỗi kết nối SQL Server thiếu tên cơ sở dữ liệu (Database / Initial Catalog).");
            }

            if (string.Equals(builder.InitialCatalog, TestDatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Không được phép sử dụng database kiểm thử '{TestDatabaseName}' cho ứng dụng chính. Vui lòng cấu hình Database={ExpectedDatabaseName}.");
            }

            if (!string.Equals(builder.InitialCatalog, ExpectedDatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Database cho ứng dụng chính phải là '{ExpectedDatabaseName}', không phải '{builder.InitialCatalog}'.");
            }
        }

        return new AppConfig
        {
            DataProvider = providerMode,
            ConnectionString = connectionString
        };
    }

    /// <summary>
    /// Làm sạch chuỗi kết nối để loại bỏ mật khẩu và thông tin nhạy cảm trước khi log hoặc hiển thị
    /// </summary>
    public static string SanitizeConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return "(không có chuỗi kết nối)";
        }

        try
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(builder.DataSource) && string.IsNullOrWhiteSpace(builder.InitialCatalog))
            {
                return "(chuỗi kết nối không đúng định dạng)";
            }
            return $"Server={builder.DataSource};Database={builder.InitialCatalog}";
        }
        catch
        {
            return "(chuỗi kết nối không đúng định dạng)";
        }
    }

    private static void ReadFromJsonFile(string filePath, ref string? rawProvider, ref string? connectionString)
    {
        var jsonText = File.ReadAllText(filePath);
        using var doc = JsonDocument.Parse(jsonText);
        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        foreach (var prop in root.EnumerateObject())
        {
            if (string.Equals(prop.Name, "DataProvider", StringComparison.OrdinalIgnoreCase))
            {
                var val = prop.Value.GetString();
                if (!string.IsNullOrWhiteSpace(val))
                {
                    rawProvider = val.Trim();
                }
            }
            else if (string.Equals(prop.Name, "ConnectionStrings", StringComparison.OrdinalIgnoreCase) &&
                     prop.Value.ValueKind == JsonValueKind.Object)
            {
                foreach (var connProp in prop.Value.EnumerateObject())
                {
                    if (string.Equals(connProp.Name, "QLHV", StringComparison.OrdinalIgnoreCase))
                    {
                        var val = connProp.Value.GetString();
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            connectionString = val.Trim();
                        }
                    }
                }
            }
        }
    }
}
