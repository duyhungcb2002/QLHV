using System.Data;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlTaiKhoanRepository : ITaiKhoanRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlTaiKhoanRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<TaiKhoanDto?> GetByTenDangNhapAsync(string tenDangNhap)
    {
        if (string.IsNullOrWhiteSpace(tenDangNhap)) return null;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, TenDangNhap, VaiTro, GiangVienId, TrangThai, LanDangNhapCuoi
            FROM dbo.TaiKhoan
            WHERE TenDangNhap = @TenDangNhap;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@TenDangNhap", SqlDbType.NVarChar, 50) { Value = tenDangNhap.Trim() });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapTaiKhoan(reader);
        }

        return null;
    }

    public async Task<string?> GetPasswordHashAsync(string tenDangNhap)
    {
        if (string.IsNullOrWhiteSpace(tenDangNhap)) return null;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT MatKhauHash
            FROM dbo.TaiKhoan
            WHERE TenDangNhap = @TenDangNhap;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@TenDangNhap", SqlDbType.NVarChar, 50) { Value = tenDangNhap.Trim() });

        var result = await command.ExecuteScalarAsync();
        return result != null && result != DBNull.Value ? (string)result : null;
    }

    public async Task<bool> UpdateLanDangNhapCuoiAsync(int id, DateTime thoiGian)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.TaiKhoan
            SET LanDangNhapCuoi = @LanDangNhapCuoi
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@LanDangNhapCuoi", SqlDbType.DateTime2) { Value = thoiGian });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<IReadOnlyList<TaiKhoanDto>> GetAllAsync()
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, TenDangNhap, VaiTro, GiangVienId, TrangThai, LanDangNhapCuoi
            FROM dbo.TaiKhoan
            ORDER BY Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var list = new List<TaiKhoanDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapTaiKhoan(reader));
        }

        return list;
    }

    private static TaiKhoanDto MapTaiKhoan(SqlDataReader reader)
    {
        var vaiTroStr = reader.GetString(2);
        var vaiTro = Enum.TryParse<VaiTro>(vaiTroStr, out var parsedRole) ? parsedRole : VaiTro.ADMIN;

        return new TaiKhoanDto
        {
            Id = reader.GetInt32(0),
            TenDangNhap = reader.GetString(1),
            VaiTro = vaiTro,
            GiangVienId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
            TrangThai = (TrangThaiTaiKhoan)reader.GetInt32(4),
            LanDangNhapCuoi = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
        };
    }
}

