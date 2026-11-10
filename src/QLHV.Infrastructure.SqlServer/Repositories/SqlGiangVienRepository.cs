using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlGiangVienRepository : IGiangVienRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlGiangVienRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<GiangVienDto>> GetAllAsync(GiangVienFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT Id, MaGiangVien, HoTen, ChuyenMon, DienThoai, Email, TrangThai
            FROM dbo.GiangVien
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
                     AND (MaGiangVien LIKE @TuKhoa ESCAPE N'~'
                       OR HoTen LIKE @TuKhoa ESCAPE N'~'
                       OR HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~'
                       OR ChuyenMon LIKE @TuKhoa ESCAPE N'~'
                       OR ChuyenMon COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~'
                       OR DienThoai LIKE @TuKhoa ESCAPE N'~'
                       OR Email LIKE @TuKhoa ESCAPE N'~')
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
        var list = new List<GiangVienDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapGiangVien(reader));
        }

        return list;
    }

    public async Task<GiangVienDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaGiangVien, HoTen, ChuyenMon, DienThoai, Email, TrangThai
            FROM dbo.GiangVien
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapGiangVien(reader);
        }

        return null;
    }

    public async Task<GiangVienDto?> GetByMaGiangVienAsync(string maGiangVien)
    {
        if (string.IsNullOrWhiteSpace(maGiangVien)) return null;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaGiangVien, HoTen, ChuyenMon, DienThoai, Email, TrangThai
            FROM dbo.GiangVien
            WHERE MaGiangVien = @MaGiangVien;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaGiangVien", SqlDbType.NVarChar, 20) { Value = maGiangVien.Trim() });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapGiangVien(reader);
        }

        return null;
    }

    public async Task<GiangVienDto> AddAsync(GiangVienDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            INSERT INTO dbo.GiangVien (MaGiangVien, HoTen, ChuyenMon, DienThoai, Email, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@MaGiangVien, @HoTen, @ChuyenMon, @DienThoai, @Email, @TrangThai);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaGiangVien", SqlDbType.NVarChar, 20) { Value = dto.MaGiangVien.Trim() });
        command.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = dto.HoTen.Trim() });
        command.Parameters.Add(new SqlParameter("@ChuyenMon", SqlDbType.NVarChar, 200) { Value = SqlRepositoryHelper.ToDbValue(dto.ChuyenMon?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DienThoai", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.DienThoai?.Trim()) });
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = SqlRepositoryHelper.ToDbValue(dto.Email?.Trim()) });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        var newId = await command.ExecuteScalarAsync();
        dto.Id = Convert.ToInt32(newId);
        return dto;
    }

    public async Task<bool> UpdateAsync(GiangVienDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.GiangVien
            SET MaGiangVien = @MaGiangVien,
                HoTen = @HoTen,
                ChuyenMon = @ChuyenMon,
                DienThoai = @DienThoai,
                Email = @Email,
                TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaGiangVien", SqlDbType.NVarChar, 20) { Value = dto.MaGiangVien.Trim() });
        command.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = dto.HoTen.Trim() });
        command.Parameters.Add(new SqlParameter("@ChuyenMon", SqlDbType.NVarChar, 200) { Value = SqlRepositoryHelper.ToDbValue(dto.ChuyenMon?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DienThoai", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.DienThoai?.Trim()) });
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = SqlRepositoryHelper.ToDbValue(dto.Email?.Trim()) });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.GiangVien
            SET TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)trangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private static GiangVienDto MapGiangVien(SqlDataReader reader)
    {
        return new GiangVienDto
        {
            Id = reader.GetInt32(0),
            MaGiangVien = reader.GetString(1),
            HoTen = reader.GetString(2),
            ChuyenMon = reader.IsDBNull(3) ? null : reader.GetString(3),
            DienThoai = reader.IsDBNull(4) ? null : reader.GetString(4),
            Email = reader.IsDBNull(5) ? null : reader.GetString(5),
            TrangThai = (TrangThaiHoatDong)reader.GetInt32(6)
        };
    }
}
