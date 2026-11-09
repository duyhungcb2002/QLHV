namespace QLHV.Infrastructure.SqlServer;

/// <summary>
/// Tùy chọn cấu hình kết nối cơ sở dữ liệu SQL Server
/// </summary>
public sealed class SqlServerOptions
{
    public const string DefaultConnectionStringName = "QLHV";

    /// <summary>
    /// Chuỗi kết nối ADO.NET đến SQL Server
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}

