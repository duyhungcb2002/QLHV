using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlMonHocRepository : IMonHocRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlMonHocRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<MonHocDto>> GetAllAsync(MonHocFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT Id, MaMonHoc, TenMonHoc, SoBuoiMacDinh, MoTa, TrangThai
            FROM dbo.MonHoc
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
                     AND (MaMonHoc LIKE @TuKhoa ESCAPE N'~'
                       OR TenMonHoc LIKE @TuKhoa ESCAPE N'~'
                       OR TenMonHoc COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~'
                       OR MoTa LIKE @TuKhoa ESCAPE N'~'
                       OR MoTa COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~')
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
        var list = new List<MonHocDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapMonHoc(reader));
        }

        return list;
    }

    public async Task<MonHocDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaMonHoc, TenMonHoc, SoBuoiMacDinh, MoTa, TrangThai
            FROM dbo.MonHoc
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapMonHoc(reader);
        }

        return null;
    }

    public async Task<MonHocDto?> GetByMaMonHocAsync(string maMonHoc)
    {
        if (string.IsNullOrWhiteSpace(maMonHoc)) return null;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaMonHoc, TenMonHoc, SoBuoiMacDinh, MoTa, TrangThai
            FROM dbo.MonHoc
            WHERE MaMonHoc = @MaMonHoc;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaMonHoc", SqlDbType.NVarChar, 20) { Value = maMonHoc.Trim() });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapMonHoc(reader);
        }

        return null;
    }

    public async Task<MonHocDto> AddAsync(MonHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            INSERT INTO dbo.MonHoc (MaMonHoc, TenMonHoc, SoBuoiMacDinh, MoTa, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@MaMonHoc, @TenMonHoc, @SoBuoiMacDinh, @MoTa, @TrangThai);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaMonHoc", SqlDbType.NVarChar, 20) { Value = dto.MaMonHoc.Trim() });
        command.Parameters.Add(new SqlParameter("@TenMonHoc", SqlDbType.NVarChar, 150) { Value = dto.TenMonHoc.Trim() });
        command.Parameters.Add(new SqlParameter("@SoBuoiMacDinh", SqlDbType.Int) { Value = dto.SoBuoiMacDinh });
        command.Parameters.Add(new SqlParameter("@MoTa", SqlDbType.NVarChar, 500) { Value = SqlRepositoryHelper.ToDbValue(dto.MoTa?.Trim()) });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        var newId = await command.ExecuteScalarAsync();
        dto.Id = Convert.ToInt32(newId);
        return dto;
    }

    public async Task<bool> UpdateAsync(MonHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.MonHoc
            SET MaMonHoc = @MaMonHoc,
                TenMonHoc = @TenMonHoc,
                SoBuoiMacDinh = @SoBuoiMacDinh,
                MoTa = @MoTa,
                TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaMonHoc", SqlDbType.NVarChar, 20) { Value = dto.MaMonHoc.Trim() });
        command.Parameters.Add(new SqlParameter("@TenMonHoc", SqlDbType.NVarChar, 150) { Value = dto.TenMonHoc.Trim() });
        command.Parameters.Add(new SqlParameter("@SoBuoiMacDinh", SqlDbType.Int) { Value = dto.SoBuoiMacDinh });
        command.Parameters.Add(new SqlParameter("@MoTa", SqlDbType.NVarChar, 500) { Value = SqlRepositoryHelper.ToDbValue(dto.MoTa?.Trim()) });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.MonHoc
            SET TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)trangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private static MonHocDto MapMonHoc(SqlDataReader reader)
    {
        return new MonHocDto
        {
            Id = reader.GetInt32(0),
            MaMonHoc = reader.GetString(1),
            TenMonHoc = reader.GetString(2),
            SoBuoiMacDinh = reader.GetInt32(3),
            MoTa = reader.IsDBNull(4) ? null : reader.GetString(4),
            TrangThai = (TrangThaiHoatDong)reader.GetInt32(5)
        };
    }
}
