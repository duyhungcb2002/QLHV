using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlKhoaHocRepository : IKhoaHocRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlKhoaHocRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<KhoaHocDto>> GetAllAsync(KhoaHocFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT Id, MaKhoaHoc, TenKhoaHoc, HocPhiNiemYet, MoTa, TrangThai
            FROM dbo.KhoaHoc
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
                     AND (MaKhoaHoc LIKE @TuKhoa ESCAPE N'~'
                       OR TenKhoaHoc LIKE @TuKhoa ESCAPE N'~'
                       OR TenKhoaHoc COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~'
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
        var list = new List<KhoaHocDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapKhoaHoc(reader));
        }

        return list;
    }

    public async Task<KhoaHocDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaKhoaHoc, TenKhoaHoc, HocPhiNiemYet, MoTa, TrangThai
            FROM dbo.KhoaHoc
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapKhoaHoc(reader);
        }

        return null;
    }

    public async Task<KhoaHocDto?> GetByMaKhoaHocAsync(string maKhoaHoc)
    {
        if (string.IsNullOrWhiteSpace(maKhoaHoc)) return null;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaKhoaHoc, TenKhoaHoc, HocPhiNiemYet, MoTa, TrangThai
            FROM dbo.KhoaHoc
            WHERE MaKhoaHoc = @MaKhoaHoc;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaKhoaHoc", SqlDbType.NVarChar, 20) { Value = maKhoaHoc.Trim() });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapKhoaHoc(reader);
        }

        return null;
    }

    public async Task<KhoaHocDto> AddAsync(KhoaHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            INSERT INTO dbo.KhoaHoc (MaKhoaHoc, TenKhoaHoc, HocPhiNiemYet, MoTa, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@MaKhoaHoc, @TenKhoaHoc, @HocPhiNiemYet, @MoTa, @TrangThai);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaKhoaHoc", SqlDbType.NVarChar, 20) { Value = dto.MaKhoaHoc.Trim() });
        command.Parameters.Add(new SqlParameter("@TenKhoaHoc", SqlDbType.NVarChar, 150) { Value = dto.TenKhoaHoc.Trim() });
        command.Parameters.Add(new SqlParameter("@HocPhiNiemYet", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = dto.HocPhiNiemYet });
        command.Parameters.Add(new SqlParameter("@MoTa", SqlDbType.NVarChar, 500) { Value = SqlRepositoryHelper.ToDbValue(dto.MoTa?.Trim()) });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        var newId = await command.ExecuteScalarAsync();
        dto.Id = Convert.ToInt32(newId);
        return dto;
    }

    public async Task<bool> UpdateAsync(KhoaHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.KhoaHoc
            SET MaKhoaHoc = @MaKhoaHoc,
                TenKhoaHoc = @TenKhoaHoc,
                HocPhiNiemYet = @HocPhiNiemYet,
                MoTa = @MoTa,
                TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaKhoaHoc", SqlDbType.NVarChar, 20) { Value = dto.MaKhoaHoc.Trim() });
        command.Parameters.Add(new SqlParameter("@TenKhoaHoc", SqlDbType.NVarChar, 150) { Value = dto.TenKhoaHoc.Trim() });
        command.Parameters.Add(new SqlParameter("@HocPhiNiemYet", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = dto.HocPhiNiemYet });
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
            UPDATE dbo.KhoaHoc
            SET TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)trangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private static KhoaHocDto MapKhoaHoc(SqlDataReader reader)
    {
        return new KhoaHocDto
        {
            Id = reader.GetInt32(0),
            MaKhoaHoc = reader.GetString(1),
            TenKhoaHoc = reader.GetString(2),
            HocPhiNiemYet = reader.GetDecimal(3),
            MoTa = reader.IsDBNull(4) ? null : reader.GetString(4),
            TrangThai = (TrangThaiHoatDong)reader.GetInt32(5)
        };
    }
}
