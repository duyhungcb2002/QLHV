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
    private const string SelectColumns = """
        hv.Id, hv.MaHocVien, hv.HoTen, hv.NgaySinh, hv.GioiTinh, hv.DienThoai, hv.Email, hv.DiaChi, hv.NgayDangKy, hv.TrangThai,
        hv.SoDinhDanh, hv.NgayCapGiayTo, hv.NoiCapGiayTo, hv.HoTenNguoiGiamHo, hv.DienThoaiNguoiGiamHo, hv.QuanHeNguoiGiamHo,
        hv.NhanVienPhuTrachTaiKhoanId, hv.NguonTuyenSinh, hv.NgheNghiep, hv.DonViCongTac, hv.GhiChuDacBiet,
        tk.TenDangNhap AS NhanVienTenDangNhap, tk.VaiTro AS NhanVienVaiTro
        """;

    private readonly IConnectionFactory _connectionFactory;

    public SqlHocVienRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<HocVienDto>> GetAllAsync(HocVienFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder($"""
            SELECT {SelectColumns}
            FROM dbo.HocVien hv
            LEFT JOIN dbo.TaiKhoan tk ON hv.NhanVienPhuTrachTaiKhoanId = tk.Id
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
                     AND (hv.MaHocVien LIKE @TuKhoa ESCAPE N'~'
                       OR hv.HoTen LIKE @TuKhoa ESCAPE N'~'
                       OR hv.HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~'
                       OR hv.DienThoai LIKE @TuKhoa ESCAPE N'~'
                       OR hv.Email LIKE @TuKhoa ESCAPE N'~'
                       OR hv.SoDinhDanh LIKE @TuKhoa ESCAPE N'~'
                       OR hv.HoTenNguoiGiamHo LIKE @TuKhoa ESCAPE N'~'
                       OR hv.DienThoaiNguoiGiamHo LIKE @TuKhoa ESCAPE N'~'
                       OR hv.NguonTuyenSinh LIKE @TuKhoa ESCAPE N'~')
                    """);
                command.Parameters.Add(new SqlParameter("@TuKhoa", SqlDbType.NVarChar) { Value = $"%{escaped}%" });
            }

            if (filter.TrangThai.HasValue)
            {
                sqlBuilder.Append(" AND hv.TrangThai = @TrangThai");
                command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)filter.TrangThai.Value });
            }

            if (!string.IsNullOrWhiteSpace(filter.SoDinhDanh))
            {
                sqlBuilder.Append(" AND hv.SoDinhDanh = @SoDinhDanh");
                command.Parameters.Add(new SqlParameter("@SoDinhDanh", SqlDbType.NVarChar, 20) { Value = filter.SoDinhDanh.Trim() });
            }

            if (!string.IsNullOrWhiteSpace(filter.NguoiGiamHo))
            {
                var escaped = SqlRepositoryHelper.EscapeLike(filter.NguoiGiamHo);
                sqlBuilder.Append(" AND hv.HoTenNguoiGiamHo LIKE @NguoiGiamHo ESCAPE N'~'");
                command.Parameters.Add(new SqlParameter("@NguoiGiamHo", SqlDbType.NVarChar) { Value = $"%{escaped}%" });
            }

            if (!string.IsNullOrWhiteSpace(filter.DienThoaiNguoiGiamHo))
            {
                sqlBuilder.Append(" AND hv.DienThoaiNguoiGiamHo = @DienThoaiNguoiGiamHo");
                command.Parameters.Add(new SqlParameter("@DienThoaiNguoiGiamHo", SqlDbType.NVarChar, 20) { Value = filter.DienThoaiNguoiGiamHo.Trim() });
            }

            if (!string.IsNullOrWhiteSpace(filter.NguonTuyenSinh))
            {
                sqlBuilder.Append(" AND hv.NguonTuyenSinh = @NguonTuyenSinh");
                command.Parameters.Add(new SqlParameter("@NguonTuyenSinh", SqlDbType.NVarChar, 50) { Value = filter.NguonTuyenSinh.Trim() });
            }

            if (filter.NhanVienPhuTrachTaiKhoanId.HasValue)
            {
                sqlBuilder.Append(" AND hv.NhanVienPhuTrachTaiKhoanId = @NhanVienPhuTrachTaiKhoanId");
                command.Parameters.Add(new SqlParameter("@NhanVienPhuTrachTaiKhoanId", SqlDbType.Int) { Value = filter.NhanVienPhuTrachTaiKhoanId.Value });
            }
        }

        sqlBuilder.Append(" ORDER BY hv.Id;");
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
        var sql = $"""
            SELECT {SelectColumns}
            FROM dbo.HocVien hv
            LEFT JOIN dbo.TaiKhoan tk ON hv.NhanVienPhuTrachTaiKhoanId = tk.Id
            WHERE hv.Id = @Id;
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
        var sql = $"""
            SELECT {SelectColumns}
            FROM dbo.HocVien hv
            LEFT JOIN dbo.TaiKhoan tk ON hv.NhanVienPhuTrachTaiKhoanId = tk.Id
            WHERE hv.MaHocVien = @MaHocVien;
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

    public async Task<HocVienDto?> GetBySoDinhDanhAsync(string soDinhDanh)
    {
        if (string.IsNullOrWhiteSpace(soDinhDanh)) return null;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        var sql = $"""
            SELECT {SelectColumns}
            FROM dbo.HocVien hv
            LEFT JOIN dbo.TaiKhoan tk ON hv.NhanVienPhuTrachTaiKhoanId = tk.Id
            WHERE hv.SoDinhDanh = @SoDinhDanh;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@SoDinhDanh", SqlDbType.NVarChar, 20) { Value = soDinhDanh.Trim() });

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
            INSERT INTO dbo.HocVien (
                MaHocVien, HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, NgayDangKy, TrangThai,
                SoDinhDanh, NgayCapGiayTo, NoiCapGiayTo, HoTenNguoiGiamHo, DienThoaiNguoiGiamHo, QuanHeNguoiGiamHo,
                NhanVienPhuTrachTaiKhoanId, NguonTuyenSinh, NgheNghiep, DonViCongTac, GhiChuDacBiet
            )
            OUTPUT INSERTED.Id
            VALUES (
                @MaHocVien, @HoTen, @NgaySinh, @GioiTinh, @DienThoai, @Email, @DiaChi, @NgayDangKy, @TrangThai,
                @SoDinhDanh, @NgayCapGiayTo, @NoiCapGiayTo, @HoTenNguoiGiamHo, @DienThoaiNguoiGiamHo, @QuanHeNguoiGiamHo,
                @NhanVienPhuTrachTaiKhoanId, @NguonTuyenSinh, @NgheNghiep, @DonViCongTac, @GhiChuDacBiet
            );
            """;

        await using var command = new SqlCommand(sql, connection);
        AddParameters(command, dto);

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
                TrangThai = @TrangThai,
                SoDinhDanh = @SoDinhDanh,
                NgayCapGiayTo = @NgayCapGiayTo,
                NoiCapGiayTo = @NoiCapGiayTo,
                HoTenNguoiGiamHo = @HoTenNguoiGiamHo,
                DienThoaiNguoiGiamHo = @DienThoaiNguoiGiamHo,
                QuanHeNguoiGiamHo = @QuanHeNguoiGiamHo,
                NhanVienPhuTrachTaiKhoanId = @NhanVienPhuTrachTaiKhoanId,
                NguonTuyenSinh = @NguonTuyenSinh,
                NgheNghiep = @NgheNghiep,
                DonViCongTac = @DonViCongTac,
                GhiChuDacBiet = @GhiChuDacBiet
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        AddParameters(command, dto);
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

    private static void AddParameters(SqlCommand command, HocVienDto dto)
    {
        command.Parameters.Add(new SqlParameter("@MaHocVien", SqlDbType.NVarChar, 20) { Value = dto.MaHocVien.Trim() });
        command.Parameters.Add(new SqlParameter("@HoTen", SqlDbType.NVarChar, 100) { Value = dto.HoTen.Trim() });
        command.Parameters.Add(new SqlParameter("@NgaySinh", SqlDbType.Date) { Value = dto.NgaySinh.HasValue ? dto.NgaySinh.Value.Date : DBNull.Value });
        command.Parameters.Add(new SqlParameter("@GioiTinh", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.GioiTinh?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DienThoai", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.DienThoai?.Trim()) });
        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = SqlRepositoryHelper.ToDbValue(dto.Email?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DiaChi", SqlDbType.NVarChar, 250) { Value = SqlRepositoryHelper.ToDbValue(dto.DiaChi?.Trim()) });
        command.Parameters.Add(new SqlParameter("@NgayDangKy", SqlDbType.Date) { Value = dto.NgayDangKy.Date });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        command.Parameters.Add(new SqlParameter("@SoDinhDanh", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.SoDinhDanh?.Trim()) });
        command.Parameters.Add(new SqlParameter("@NgayCapGiayTo", SqlDbType.Date) { Value = dto.NgayCapGiayTo.HasValue ? dto.NgayCapGiayTo.Value.Date : DBNull.Value });
        command.Parameters.Add(new SqlParameter("@NoiCapGiayTo", SqlDbType.NVarChar, 150) { Value = SqlRepositoryHelper.ToDbValue(dto.NoiCapGiayTo?.Trim()) });
        command.Parameters.Add(new SqlParameter("@HoTenNguoiGiamHo", SqlDbType.NVarChar, 100) { Value = SqlRepositoryHelper.ToDbValue(dto.HoTenNguoiGiamHo?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DienThoaiNguoiGiamHo", SqlDbType.NVarChar, 20) { Value = SqlRepositoryHelper.ToDbValue(dto.DienThoaiNguoiGiamHo?.Trim()) });
        command.Parameters.Add(new SqlParameter("@QuanHeNguoiGiamHo", SqlDbType.NVarChar, 50) { Value = SqlRepositoryHelper.ToDbValue(dto.QuanHeNguoiGiamHo?.Trim()) });
        command.Parameters.Add(new SqlParameter("@NhanVienPhuTrachTaiKhoanId", SqlDbType.Int) { Value = (object?)dto.NhanVienPhuTrachTaiKhoanId ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@NguonTuyenSinh", SqlDbType.NVarChar, 50) { Value = SqlRepositoryHelper.ToDbValue(dto.NguonTuyenSinh?.Trim()) });
        command.Parameters.Add(new SqlParameter("@NgheNghiep", SqlDbType.NVarChar, 100) { Value = SqlRepositoryHelper.ToDbValue(dto.NgheNghiep?.Trim()) });
        command.Parameters.Add(new SqlParameter("@DonViCongTac", SqlDbType.NVarChar, 200) { Value = SqlRepositoryHelper.ToDbValue(dto.DonViCongTac?.Trim()) });
        command.Parameters.Add(new SqlParameter("@GhiChuDacBiet", SqlDbType.NVarChar, 1000) { Value = SqlRepositoryHelper.ToDbValue(dto.GhiChuDacBiet?.Trim()) });
    }

    private static HocVienDto MapHocVien(SqlDataReader reader)
    {
        var dto = new HocVienDto
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
            TrangThai = (TrangThaiHoatDong)reader.GetInt32(9),

            SoDinhDanh = reader.IsDBNull(10) ? null : reader.GetString(10),
            NgayCapGiayTo = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
            NoiCapGiayTo = reader.IsDBNull(12) ? null : reader.GetString(12),
            HoTenNguoiGiamHo = reader.IsDBNull(13) ? null : reader.GetString(13),
            DienThoaiNguoiGiamHo = reader.IsDBNull(14) ? null : reader.GetString(14),
            QuanHeNguoiGiamHo = reader.IsDBNull(15) ? null : reader.GetString(15),
            NhanVienPhuTrachTaiKhoanId = reader.IsDBNull(16) ? null : reader.GetInt32(16),
            NguonTuyenSinh = reader.IsDBNull(17) ? null : reader.GetString(17),
            NgheNghiep = reader.IsDBNull(18) ? null : reader.GetString(18),
            DonViCongTac = reader.IsDBNull(19) ? null : reader.GetString(19),
            GhiChuDacBiet = reader.IsDBNull(20) ? null : reader.GetString(20)
        };

        if (!reader.IsDBNull(21) && !reader.IsDBNull(22))
        {
            var tenDangNhap = reader.GetString(21);
            var vaiTro = reader.GetString(22);
            dto.TenNhanVienPhuTrach = $"{tenDangNhap} ({vaiTro})";
        }

        return dto;
    }
}
