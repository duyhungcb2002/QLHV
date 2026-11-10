using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace QLHV.Infrastructure.SqlServer.Repositories;

internal static class SqlRepositoryHelper
{
    public static string EscapeLike(string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return string.Empty;
        return keyword.Trim()
            .Replace("~", "~~")
            .Replace("%", "~%")
            .Replace("_", "~_")
            .Replace("[", "~[");
    }

    public static object ToDbValue(object? value) => value ?? DBNull.Value;

    public static SqlParameter CreateParam(string name, SqlDbType type, object? value, int size = 0)
    {
        var param = size > 0
            ? new SqlParameter(name, type, size) { Value = ToDbValue(value) }
            : new SqlParameter(name, type) { Value = ToDbValue(value) };
        return param;
    }

    public static async Task<SqlConnection> OpenConnectionAsync(QLHV.Core.Database.IConnectionFactory factory)
    {
        var dbConnection = factory.CreateConnection();
        if (dbConnection is not SqlConnection sqlConnection)
        {
            throw new InvalidOperationException($"IConnectionFactory tạo ra connection kiểu '{dbConnection.GetType().FullName}' thay vì Microsoft.Data.SqlClient.SqlConnection.");
        }
        await sqlConnection.OpenAsync();
        return sqlConnection;
    }
}

