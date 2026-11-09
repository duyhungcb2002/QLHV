using System.Data.Common;
using Microsoft.Data.SqlClient;
using QLHV.Core.Database;

namespace QLHV.Infrastructure.SqlServer;

/// <summary>
/// Cấp phát kết nối ADO.NET SqlConnection cho SQL Server.
/// Chỉ khởi tạo đối tượng kết nối ở trạng thái chưa mở (Closed);
/// các Repository chịu trách nhiệm OpenAsync() và Dispose().
/// </summary>
public sealed class SqlConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "Cấu hình chuỗi kết nối SQL Server (ConnectionString) không hợp lệ: không được để null, rỗng hoặc chỉ chứa khoảng trắng.",
                nameof(connectionString));
        }

        _connectionString = connectionString.Trim();
    }

    public SqlConnectionFactory(SqlServerOptions options)
        : this(options?.ConnectionString ?? throw new ArgumentNullException(nameof(options), "Cấu hình SqlServerOptions không được phép null."))
    {
    }

    public DbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
