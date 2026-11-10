using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlHocVienRepository : IHocVienRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlHocVienRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<HocVienDto>> GetAllAsync(HocVienFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT Id, MaHocVien, HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, NgayDangKy, TrangThai
            FROM dbo.HocVien
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
                     AND (MaHocVien LIKE @TuKhoa ESCAPE N'~'
                       OR HoTen LIKE @TuKhoa ESCAPE N'~'
                       OR HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~'
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
        var list = new List<HocVienDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapHocVien(reader));
        }

        return list;
    }

    public async Task<HocVienDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaHocVien, HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, NgayDangKy, TrangThai
            FROM dbo.HocVien
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapHocVien(reader);
        }

        return null;
    }

    public async Task<HocVienDto?> GetByMaHocVienAsync(string maHocVien)
    {
        if (string.IsNullOrWhiteSpace(maHocVien)) return null;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT Id, MaHocVien, HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, NgayDangKy, TrangThai
            FROM dbo.HocVien
            WHERE MaHocVien = @MaHocVien;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaHocVien", SqlDbType.NVarChar, 20) { Value = maHocVien.Trim() });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapHocVien(reader);
        }

        return null;
    }

    public async Task<HocVienDto> AddAsync(HocVienDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            INSERT INTO dbo.HocVien (MaHocVien, HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, NgayDangKy, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@MaHocVien, @HoTen, @NgaySinh, @GioiTinh, @DienThoai, @Email, @DiaChi, @NgayDangKy, @TrangThai);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaHocVien", SqlDbType.NVarChar, 20) { Value = dto.MaHocVien.Trim() });
        command.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = dto.HoTen.Trim() });
        command.Parameters.Add(new SqlParameter("@NgaySinh", SqlDbType.Date) { Value = dto.NgaySinh.HasValue ? dto.NgaySinh.Value.Date : DBNull.Value });
        command.Parameters.Add(new SqlParameter("@GioiTinh", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.GioiTinh?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DienThoai", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.DienThoai?.Trim()) });
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = SqlRepositoryHelper.ToDbValue(dto.Email?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DiaChi", SqlDbType.NVarChar, 250) { Value = SqlRepositoryHelper.ToDbValue(dto.DiaChi?.Trim()) });
        command.Parameters.Add(new SqlParameter("@NgayDangKy", SqlDbType.Date) { Value = dto.NgayDangKy.Date });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        var newId = await command.ExecuteScalarAsync();
        dto.Id = Convert.ToInt32(newId);
        return dto;
    }

    public async Task<bool> UpdateAsync(HocVienDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.HocVien
            SET MaHocVien = @MaHocVien,
                HoTen = @HoTen,
                NgaySinh = @NgaySinh,
                GioiTinh = @GioiTinh,
                DienThoai = @DienThoai,
                Email = @Email,
                DiaChi = @DiaChi,
                NgayDangKy = @NgayDangKy,
                TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaHocVien", SqlDbType.NVarChar, 20) { Value = dto.MaHocVien.Trim() });
        command.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = dto.HoTen.Trim() });
        command.Parameters.Add(new SqlParameter("@NgaySinh", SqlDbType.Date) { Value = dto.NgaySinh.HasValue ? dto.NgaySinh.Value.Date : DBNull.Value });
        command.Parameters.Add(new SqlParameter("@GioiTinh", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.GioiTinh?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DienThoai", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.DienThoai?.Trim()) });
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = SqlRepositoryHelper.ToDbValue(dto.Email?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DiaChi", SqlDbType.NVarChar, 250) { Value = SqlRepositoryHelper.ToDbValue(dto.DiaChi?.Trim()) });
        command.Parameters.Add(new SqlParameter("@NgayDangKy", SqlDbType.Date) { Value = dto.NgayDangKy.Date });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.HocVien
            SET TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)trangThai });
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private static HocVienDto MapHocVien(SqlDataReader reader)
    {
        return new HocVienDto
        {
            Id = reader.GetInt32(0),
            MaHocVien = reader.GetString(1),
            HoTen = reader.GetString(2),
            NgaySinh = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
            GioiTinh = reader.IsDBNull(4) ? null : reader.GetString(4),
            DienThoai = reader.IsDBNull(5) ? null : reader.GetString(5),
            Email = reader.IsDBNull(6) ? null : reader.GetString(6),
            DiaChi = reader.IsDBNull(7) ? null : reader.GetString(7),
            NgayDangKy = reader.GetDateTime(8),
            TrangThai = (TrangThaiHoatDong)reader.GetInt32(9)
        };
    }
}
