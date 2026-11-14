namespace QLHV.WinForms.Configuration;

/// <summary>
/// Chế độ cung cấp dữ liệu cho ứng dụng QLHV
/// </summary>
public enum DataProviderMode
{
    /// <summary>
    /// Dữ liệu mẫu giả lập trong bộ nhớ (In-Memory Fake)
    /// </summary>
    Fake,

    /// <summary>
    /// Cơ sở dữ liệu Microsoft SQL Server thật qua ADO.NET
    /// </summary>
    SqlServer
}

