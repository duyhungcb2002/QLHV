using System.Data.Common;

namespace QLHV.Core.Database;

/// <summary>
/// Abstraction cấp phát kết nối CSDL ADO.NET
/// Thành viên Chu Đức sẽ triển khai class SqlConnectionFactory kết nối SQL Server thật khi chuyển từ in-memory sang CSDL.
/// </summary>
public interface IConnectionFactory
{
    DbConnection CreateConnection();
}

/// <summary>
/// Placeholder implementation trước khi Chu Đức hoàn thiện kết nối SQL Server
/// </summary>
public class PendingConnectionFactory : IConnectionFactory
{
    public DbConnection CreateConnection()
    {
        throw new NotImplementedException("Kết nối CSDL thật đang chờ Chu Đức cung cấp implementation ADO.NET SQL Server (DH-06/DH-08).");
    }
}
