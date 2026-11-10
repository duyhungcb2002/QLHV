using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlPhongHocRepository : IPhongHocRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlPhongHocRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<PhongHocDto>> GetAllAsync(PhongHocFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT Id, MaPhong, TenPhong, SucChua, TrangThai
            FROM dbo.PhongHoc
            WHERE 1=1
            """);

        await using var command = new SqlCommand();
        command.Connection = connection;

        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
            {
                var escaped = SqlRepositoryHelper.EscapeLike(filter.TuKhoa);
                sqlBuilder.Append("""
                     AND (MaPhong LIKE @TuKhoa ESCAPE N'~'
                       OR TenPhong LIKE @TuKhoa ESCAPE N'~'
                       OR TenPhong COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~')
                    """);
                command.Parameters.Add(new SqlParameter("@TuKhoa", SqlDbType.NVarChar) { Value = $"%{escaped}%" });
            }

            if (filter.TrangThai.HasValue)
            {
                sqlBuilder.Append(" AND TrangThai = @TrangThai");
                command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)filter.TrangThai.Value });
            }
        }

        sqlBuilder.Append(" ORDER BY Id;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<PhongHocDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapPhongHoc(reader));
        }

        return list;
    }

    public async Task<PhongHocDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaPhong, TenPhong, SucChua, TrangThai
            FROM dbo.PhongHoc
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapPhongHoc(reader);
        }

        return null;
    }

    public async Task<PhongHocDto?> GetByMaPhongAsync(string maPhong)
    {
        if (string.IsNullOrWhiteSpace(maPhong)) return null;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaPhong, TenPhong, SucChua, TrangThai
            FROM dbo.PhongHoc
            WHERE MaPhong = @MaPhong;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.NVarChar, 20) { Value = maPhong.Trim() });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapPhongHoc(reader);
        }

        return null;
    }

    public async Task<PhongHocDto> AddAsync(PhongHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            INSERT INTO dbo.PhongHoc (MaPhong, TenPhong, SucChua, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@MaPhong, @TenPhong, @SucChua, @TrangThai);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.NVarChar, 20) { Value = dto.MaPhong.Trim() });
        command.Parameters.Add(new SqlParameter("@TenPhong", SqlDbType.NVarChar, 100) { Value = dto.TenPhong.Trim() });
        command.Parameters.Add(new SqlParameter("@SucChua", SqlDbType.Int) { Value = dto.SucChua });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        var newId = await command.ExecuteScalarAsync();
        dto.Id = Convert.ToInt32(newId);
        return dto;
    }

    public async Task<bool> UpdateAsync(PhongHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.PhongHoc
            SET MaPhong = @MaPhong,
                TenPhong = @TenPhong,
                SucChua = @SucChua,
                TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaPhong", SqlDbType.NVarChar, 20) { Value = dto.MaPhong.Trim() });
        command.Parameters.Add(new SqlParameter("@TenPhong", SqlDbType.NVarChar, 100) { Value = dto.TenPhong.Trim() });
        command.Parameters.Add(new SqlParameter("@SucChua", SqlDbType.Int) { Value = dto.SucChua });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.PhongHoc
            SET TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)trangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private static PhongHocDto MapPhongHoc(SqlDataReader reader)
    {
        return new PhongHocDto
        {
            Id = reader.GetInt32(0),
            MaPhong = reader.GetString(1),
            TenPhong = reader.GetString(2),
            SucChua = reader.GetInt32(3),
            TrangThai = (TrangThaiHoatDong)reader.GetInt32(4)
        };
    }
}
