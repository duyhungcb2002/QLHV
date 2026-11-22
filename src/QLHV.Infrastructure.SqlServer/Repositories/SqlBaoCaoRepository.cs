using System.Data;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlBaoCaoRepository : IBaoCaoRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlBaoCaoRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<BaoCaoHocVienTheoLopDto>> GetBaoCaoHocVienTheoLopAsync(int lopHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT l.MaLopHoc, l.TenLopHoc, k.TenKhoaHoc,
                   hv.MaHocVien, hv.HoTen AS HoTenHocVien, hv.NgaySinh, hv.DienThoai, hv.Email,
                   dk.NgayDangKy, dk.HocPhiPhaiThu,
                   ISNULL((SELECT SUM(pt.SoTien) FROM dbo.PhieuThu pt WHERE pt.DangKyLopHocId = dk.Id AND pt.TrangThai = 1), 0.00) AS DaThu
            FROM dbo.DangKyLopHoc dk
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc k ON l.KhoaHocId = k.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            WHERE dk.LopHocId = @LopHocId AND dk.TrangThai = 1
            ORDER BY hv.MaHocVien;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<BaoCaoHocVienTheoLopDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new BaoCaoHocVienTheoLopDto
            {
                MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
                TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
                TenKhoaHoc = reader.GetString(reader.GetOrdinal("TenKhoaHoc")),
                MaHocVien = reader.GetString(reader.GetOrdinal("MaHocVien")),
                HoTenHocVien = reader.GetString(reader.GetOrdinal("HoTenHocVien")),
                NgaySinh = reader.IsDBNull(reader.GetOrdinal("NgaySinh")) ? null : reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
                DienThoai = reader.IsDBNull(reader.GetOrdinal("DienThoai")) ? null : reader.GetString(reader.GetOrdinal("DienThoai")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                NgayDangKy = reader.GetDateTime(reader.GetOrdinal("NgayDangKy")),
                HocPhiPhaiThu = reader.GetDecimal(reader.GetOrdinal("HocPhiPhaiThu")),
                DaThu = reader.GetDecimal(reader.GetOrdinal("DaThu"))
            });
        }

        return list;
    }

    public async Task<IReadOnlyList<BaoCaoLichHocTheoLopDto>> GetBaoCaoLichHocTheoLopAsync(int lopHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT l.MaLopHoc, l.TenLopHoc,
                   mh.MaMonHoc, mh.TenMonHoc,
                   gv.HoTen AS HoTenGiangVien,
                   ph.TenPhong,
                   lh.NgayHoc, lh.GioBatDau, lh.GioKetThuc, lh.NoiDung
            FROM dbo.LichHoc lh
            INNER JOIN dbo.LopHoc l ON lh.LopHocId = l.Id
            INNER JOIN dbo.MonHoc mh ON lh.MonHocId = mh.Id
            INNER JOIN dbo.GiangVien gv ON lh.GiangVienId = gv.Id
            INNER JOIN dbo.PhongHoc ph ON lh.PhongHocId = ph.Id
            WHERE lh.LopHocId = @LopHocId AND lh.TrangThai = 1
            ORDER BY lh.NgayHoc, lh.GioBatDau;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<BaoCaoLichHocTheoLopDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new BaoCaoLichHocTheoLopDto
            {
                MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
                TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
                MaMonHoc = reader.GetString(reader.GetOrdinal("MaMonHoc")),
                TenMonHoc = reader.GetString(reader.GetOrdinal("TenMonHoc")),
                HoTenGiangVien = reader.GetString(reader.GetOrdinal("HoTenGiangVien")),
                TenPhong = reader.GetString(reader.GetOrdinal("TenPhong")),
                NgayHoc = reader.GetDateTime(reader.GetOrdinal("NgayHoc")),
                GioBatDau = reader.GetTimeSpan(reader.GetOrdinal("GioBatDau")),
                GioKetThuc = reader.GetTimeSpan(reader.GetOrdinal("GioKetThuc")),
                NoiDung = reader.IsDBNull(reader.GetOrdinal("NoiDung")) ? null : reader.GetString(reader.GetOrdinal("NoiDung"))
            });
        }

        return list;
    }

    public async Task<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>> GetBaoCaoGiangDayTheoGiangVienAsync(int giangVienId, DateTime? tuNgay = null, DateTime? denNgay = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new System.Text.StringBuilder("""
            SELECT gv.MaGiangVien, gv.HoTen AS HoTenGiangVien,
                   l.MaLopHoc, l.TenLopHoc,
                   mh.TenMonHoc,
                   lh.NgayHoc, lh.GioBatDau, lh.GioKetThuc,
                   ph.TenPhong
            FROM dbo.LichHoc lh
            INNER JOIN dbo.GiangVien gv ON lh.GiangVienId = gv.Id
            INNER JOIN dbo.LopHoc l ON lh.LopHocId = l.Id
            INNER JOIN dbo.MonHoc mh ON lh.MonHocId = mh.Id
            INNER JOIN dbo.PhongHoc ph ON lh.PhongHocId = ph.Id
            WHERE lh.GiangVienId = @GiangVienId AND lh.TrangThai = 1
            """);

        await using var command = new SqlCommand();
        command.Connection = connection;
        command.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = giangVienId });

        if (tuNgay.HasValue)
        {
            sqlBuilder.Append(" AND lh.NgayHoc >= @TuNgay");
            command.Parameters.Add(new SqlParameter("@TuNgay", SqlDbType.Date) { Value = tuNgay.Value.Date });
        }

        if (denNgay.HasValue)
        {
            sqlBuilder.Append(" AND lh.NgayHoc <= @DenNgay");
            command.Parameters.Add(new SqlParameter("@DenNgay", SqlDbType.Date) { Value = denNgay.Value.Date });
        }

        sqlBuilder.Append(" ORDER BY lh.NgayHoc, lh.GioBatDau;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<BaoCaoGiangDayTheoGiangVienDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new BaoCaoGiangDayTheoGiangVienDto
            {
                MaGiangVien = reader.GetString(reader.GetOrdinal("MaGiangVien")),
                HoTenGiangVien = reader.GetString(reader.GetOrdinal("HoTenGiangVien")),
                MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
                TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
                TenMonHoc = reader.GetString(reader.GetOrdinal("TenMonHoc")),
                NgayHoc = reader.GetDateTime(reader.GetOrdinal("NgayHoc")),
                GioBatDau = reader.GetTimeSpan(reader.GetOrdinal("GioBatDau")),
                GioKetThuc = reader.GetTimeSpan(reader.GetOrdinal("GioKetThuc")),
                TenPhong = reader.GetString(reader.GetOrdinal("TenPhong"))
            });
        }

        return list;
    }

    public async Task<IReadOnlyList<BaoCaoKetQuaHocTapDto>> GetBaoCaoKetQuaHocTapAsync(int? lopHocId, int? monHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        // Hiển thị toàn bộ học viên có đăng ký còn hoạt động, phân biệt rõ Chưa nhập điểm
        var sqlBuilder = new System.Text.StringBuilder("""
            SELECT l.MaLopHoc, l.TenLopHoc,
                   mh.MaMonHoc, mh.TenMonHoc,
                   hv.MaHocVien, hv.HoTen AS HoTenHocVien,
                   kq.DiemTongKet, kq.GhiChu
            FROM dbo.DangKyLopHoc dk
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            INNER JOIN dbo.KhoaHocMonHoc khm ON l.KhoaHocId = khm.KhoaHocId
            INNER JOIN dbo.MonHoc mh ON khm.MonHocId = mh.Id
            LEFT JOIN dbo.KetQuaHocTap kq ON dk.Id = kq.DangKyLopHocId AND mh.Id = kq.MonHocId
            WHERE dk.TrangThai = 1
            """);

        await using var command = new SqlCommand();
        command.Connection = connection;

        if (lopHocId.HasValue)
        {
            sqlBuilder.Append(" AND dk.LopHocId = @LopHocId");
            command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId.Value });
        }

        if (monHocId.HasValue)
        {
            sqlBuilder.Append(" AND mh.Id = @MonHocId");
            command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = monHocId.Value });
        }

        sqlBuilder.Append(" ORDER BY l.MaLopHoc, mh.MaMonHoc, hv.MaHocVien;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<BaoCaoKetQuaHocTapDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new BaoCaoKetQuaHocTapDto
            {
                MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
                TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
                MaMonHoc = reader.GetString(reader.GetOrdinal("MaMonHoc")),
                TenMonHoc = reader.GetString(reader.GetOrdinal("TenMonHoc")),
                MaHocVien = reader.GetString(reader.GetOrdinal("MaHocVien")),
                HoTenHocVien = reader.GetString(reader.GetOrdinal("HoTenHocVien")),
                DiemTongKet = reader.IsDBNull(reader.GetOrdinal("DiemTongKet")) ? null : reader.GetDecimal(reader.GetOrdinal("DiemTongKet")),
                GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu"))
            });
        }

        return list;
    }

    public async Task<IReadOnlyList<ThongKeTyLeMonHocDto>> GetThongKeTyLeMonHocAsync(int? lopHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        // Đếm theo toàn bộ đăng ký hoạt động của các lớp có môn học đó
        var sqlBuilder = new System.Text.StringBuilder("""
            SELECT mh.MaMonHoc, mh.TenMonHoc,
                   COUNT(dk.Id) AS TongSoHocVien,
                   COUNT(kq.DiemTongKet) AS SoLuongDaCham,
                   SUM(CASE WHEN kq.DiemTongKet IS NULL THEN 1 ELSE 0 END) AS SoLuongChuaNhap,
                   SUM(CASE WHEN kq.DiemTongKet >= 5.00 THEN 1 ELSE 0 END) AS SoLuongDat
            FROM dbo.DangKyLopHoc dk
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHocMonHoc khm ON l.KhoaHocId = khm.KhoaHocId
            INNER JOIN dbo.MonHoc mh ON khm.MonHocId = mh.Id
            LEFT JOIN dbo.KetQuaHocTap kq ON dk.Id = kq.DangKyLopHocId AND mh.Id = kq.MonHocId
            WHERE dk.TrangThai = 1
            """);

        await using var command = new SqlCommand();
        command.Connection = connection;

        if (lopHocId.HasValue)
        {
            sqlBuilder.Append(" AND dk.LopHocId = @LopHocId");
            command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId.Value });
        }

        sqlBuilder.Append(" GROUP BY mh.MaMonHoc, mh.TenMonHoc ORDER BY mh.MaMonHoc;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<ThongKeTyLeMonHocDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new ThongKeTyLeMonHocDto
            {
                MaMonHoc = reader.GetString(reader.GetOrdinal("MaMonHoc")),
                TenMonHoc = reader.GetString(reader.GetOrdinal("TenMonHoc")),
                TongSoHocVien = reader.GetInt32(reader.GetOrdinal("TongSoHocVien")),
                SoLuongDaCham = reader.GetInt32(reader.GetOrdinal("SoLuongDaCham")),
                SoLuongChuaNhap = reader.GetInt32(reader.GetOrdinal("SoLuongChuaNhap")),
                SoLuongDat = reader.GetInt32(reader.GetOrdinal("SoLuongDat"))
            });
        }

        return list;
    }

    public async Task<IReadOnlyList<BaoCaoCongNoHocPhiDto>> GetBaoCaoCongNoHocPhiAsync(int? lopHocId = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        // Bỏ đăng ký đã hủy (dk.TrangThai = 1) và bỏ phiếu thu đã hủy (pt.TrangThai = 1)
        var sqlBuilder = new System.Text.StringBuilder("""
            SELECT l.MaLopHoc, l.TenLopHoc,
                   COUNT(dk.Id) AS TongHocVien,
                   ISNULL(SUM(dk.HocPhiPhaiThu), 0.00) AS TongHocPhiPhaiThu,
                   ISNULL(SUM(ISNULL(pt.TongDaThu, 0.00)), 0.00) AS TongDaThu
            FROM dbo.LopHoc l
            LEFT JOIN dbo.DangKyLopHoc dk ON l.Id = dk.LopHocId AND dk.TrangThai = 1
            LEFT JOIN (
                SELECT DangKyLopHocId, SUM(SoTien) AS TongDaThu
                FROM dbo.PhieuThu
                WHERE TrangThai = 1
                GROUP BY DangKyLopHocId
            ) pt ON dk.Id = pt.DangKyLopHocId
            WHERE 1=1
            """);

        await using var command = new SqlCommand();
        command.Connection = connection;

        if (lopHocId.HasValue)
        {
            sqlBuilder.Append(" AND l.Id = @LopHocId");
            command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId.Value });
        }

        sqlBuilder.Append(" GROUP BY l.MaLopHoc, l.TenLopHoc ORDER BY l.MaLopHoc;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<BaoCaoCongNoHocPhiDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new BaoCaoCongNoHocPhiDto
            {
                MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
                TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
                TongHocVien = reader.GetInt32(reader.GetOrdinal("TongHocVien")),
                TongHocPhiPhaiThu = reader.GetDecimal(reader.GetOrdinal("TongHocPhiPhaiThu")),
                TongDaThu = reader.GetDecimal(reader.GetOrdinal("TongDaThu"))
            });
        }

        return list;
    }
}
