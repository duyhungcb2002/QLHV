namespace QLHV.WinForms.Configuration;

/// <summary>
/// Cấu hình khởi động của ứng dụng WinForms
/// </summary>
public sealed class AppConfig
{
    public DataProviderMode DataProvider { get; init; } = DataProviderMode.SqlServer;

    public string? ConnectionString { get; init; }

    /// <summary>
    /// Nhãn hiển thị môi trường trên giao diện chính
    /// </summary>
    public string DisplayEnvironment => DataProvider == DataProviderMode.SqlServer ? "SQL Server" : "Fake Data";
}
