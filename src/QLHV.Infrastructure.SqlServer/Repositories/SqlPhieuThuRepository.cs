using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlPhieuThuRepository : IPhieuThuRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlPhieuThuRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<PhieuThuDto>> GetAllAsync(PhieuThuFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT pt.Id, pt.MaPhieuThu, pt.DangKyLopHocId,
                   dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien,
                   pt.NgayThu, pt.SoTien, pt.HinhThuc,
                   pt.NguoiThuTaiKhoanId, tk.TenDangNhap AS NguoiThuTen,
                   pt.GhiChu, pt.TrangThai, pt.LyDoHuy,
                   pt.NgayHuy, pt.NguoiHuyTaiKhoanId,
                   tkhuy.TenDangNhap AS NguoiHuyTen
            FROM dbo.PhieuThu pt
            INNER JOIN dbo.DangKyLopHoc dk ON pt.DangKyLopHocId = dk.Id
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            INNER JOIN dbo.TaiKhoan tk ON pt.NguoiThuTaiKhoanId = tk.Id
            LEFT JOIN dbo.TaiKhoan tkhuy ON pt.NguoiHuyTaiKhoanId = tkhuy.Id
            WHERE 1=1
            """);

        await using var command = new SqlCommand();
        command.Connection = connection;

        if (filter != null)
        {
            if (filter.LopHocId.HasValue)
            {
                sqlBuilder.Append(" AND dk.LopHocId = @LopHocId");
                command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = filter.LopHocId.Value });
            }

            if (filter.DangKyLopHocId.HasValue)
            {
                sqlBuilder.Append(" AND pt.DangKyLopHocId = @DangKyLopHocId");
                command.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = filter.DangKyLopHocId.Value });
            }

            if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
            {
                var escaped = SqlRepositoryHelper.EscapeLike(filter.TuKhoa);
                sqlBuilder.Append("""
                     AND (pt.MaPhieuThu LIKE @TuKhoa ESCAPE N'~'
                       OR hv.MaHocVien LIKE @TuKhoa ESCAPE N'~'
                       OR hv.HoTen LIKE @TuKhoa ESCAPE N'~'
                       OR hv.HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~')
                    """);
                command.Parameters.Add(new SqlParameter("@TuKhoa", SqlDbType.NVarChar) { Value = $"%{escaped}%" });
            }

            if (filter.TuNgay.HasValue)
            {
                sqlBuilder.Append(" AND pt.NgayThu >= @TuNgay");
                command.Parameters.Add(new SqlParameter("@TuNgay", SqlDbType.DateTime2) { Value = filter.TuNgay.Value.Date });
            }

            if (filter.DenNgay.HasValue)
            {
                sqlBuilder.Append(" AND pt.NgayThu < @DenNgay");
                command.Parameters.Add(new SqlParameter("@DenNgay", SqlDbType.DateTime2) { Value = filter.DenNgay.Value.Date.AddDays(1) });
            }

            if (filter.TrangThai.HasValue)
            {
                sqlBuilder.Append(" AND pt.TrangThai = @TrangThai");
                command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)filter.TrangThai.Value });
            }
        }

        sqlBuilder.Append(" ORDER BY pt.Id DESC;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<PhieuThuDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapFromReader(reader));
        }

        return list;
    }

    public async Task<PhieuThuDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT pt.Id, pt.MaPhieuThu, pt.DangKyLopHocId,
                   dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien,
                   pt.NgayThu, pt.SoTien, pt.HinhThuc,
                   pt.NguoiThuTaiKhoanId, tk.TenDangNhap AS NguoiThuTen,
                   pt.GhiChu, pt.TrangThai, pt.LyDoHuy,
                   pt.NgayHuy, pt.NguoiHuyTaiKhoanId,
                   tkhuy.TenDangNhap AS NguoiHuyTen
            FROM dbo.PhieuThu pt
            INNER JOIN dbo.DangKyLopHoc dk ON pt.DangKyLopHocId = dk.Id
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            INNER JOIN dbo.TaiKhoan tk ON pt.NguoiThuTaiKhoanId = tk.Id
            LEFT JOIN dbo.TaiKhoan tkhuy ON pt.NguoiHuyTaiKhoanId = tkhuy.Id
            WHERE pt.Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }

        return null;
    }

    public async Task<PhieuThuDto?> GetByMaPhieuThuAsync(string maPhieuThu)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT pt.Id, pt.MaPhieuThu, pt.DangKyLopHocId,
                   dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien,
                   pt.NgayThu, pt.SoTien, pt.HinhThuc,
                   pt.NguoiThuTaiKhoanId, tk.TenDangNhap AS NguoiThuTen,
                   pt.GhiChu, pt.TrangThai, pt.LyDoHuy,
                   pt.NgayHuy, pt.NguoiHuyTaiKhoanId,
                   tkhuy.TenDangNhap AS NguoiHuyTen
            FROM dbo.PhieuThu pt
            INNER JOIN dbo.DangKyLopHoc dk ON pt.DangKyLopHocId = dk.Id
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            INNER JOIN dbo.TaiKhoan tk ON pt.NguoiThuTaiKhoanId = tk.Id
            LEFT JOIN dbo.TaiKhoan tkhuy ON pt.NguoiHuyTaiKhoanId = tkhuy.Id
            WHERE pt.MaPhieuThu = @MaPhieuThu;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaPhieuThu", SqlDbType.NVarChar, 30) { Value = maPhieuThu.Trim() });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }

        return null;
    }

    public async Task<PhieuThuDto> AddAsync(PhieuThuDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        if (dto.KhoaHocId == 0 || string.IsNullOrWhiteSpace(dto.TenKhoaHoc))
        {
            const string lookupSql = """
                SELECT l.KhoaHocId, kh.TenKhoaHoc
                FROM dbo.DangKyLopHoc dk
                INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
                INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
                WHERE dk.Id = @DangKyLopHocId;
                """;
            await using var lookupCmd = new SqlCommand(lookupSql, connection);
            lookupCmd.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dto.DangKyLopHocId });
            await using var lookupReader = await lookupCmd.ExecuteReaderAsync();
            if (await lookupReader.ReadAsync())
            {
                dto.KhoaHocId = lookupReader.GetInt32(0);
                dto.TenKhoaHoc = lookupReader.GetString(1);
            }
        }

        const string sql = """
            INSERT INTO dbo.PhieuThu (MaPhieuThu, DangKyLopHocId, NgayThu, SoTien, HinhThuc, NguoiThuTaiKhoanId, GhiChu, TrangThai)
            OUTPUT INSERTED.Id, INSERTED.NgayThu
            VALUES (@MaPhieuThu, @DangKyLopHocId, SYSDATETIME(), @SoTien, @HinhThuc, @NguoiThuTaiKhoanId, @GhiChu, 1);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaPhieuThu", SqlDbType.NVarChar, 30) { Value = dto.MaPhieuThu.Trim() });
        command.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dto.DangKyLopHocId });
        command.Parameters.Add(new SqlParameter("@SoTien", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = dto.SoTien });
        command.Parameters.Add(new SqlParameter("@HinhThuc", SqlDbType.NVarChar, 20) { Value = dto.HinhThuc.ToString() });
        command.Parameters.Add(new SqlParameter("@NguoiThuTaiKhoanId", SqlDbType.Int) { Value = dto.NguoiThuTaiKhoanId });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, dto.GhiChu, 250));

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            dto.Id = reader.GetInt32(0);
            dto.NgayThu = reader.GetDateTime(1);
        }

        return dto;
    }

    public async Task<bool> CancelAsync(int id, string lyDoHuy, int? nguoiHuyTaiKhoanId = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            UPDATE dbo.PhieuThu
            SET TrangThai = 0,
                LyDoHuy = @LyDoHuy,
                NgayHuy = SYSDATETIME(),
                NguoiHuyTaiKhoanId = @NguoiHuyTaiKhoanId
            WHERE Id = @Id AND TrangThai = 1;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@LyDoHuy", SqlDbType.NVarChar, lyDoHuy, 250));
        command.Parameters.Add(new SqlParameter("@NguoiHuyTaiKhoanId", SqlDbType.Int) { Value = (object?)nguoiHuyTaiKhoanId ?? DBNull.Value });

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<decimal> GetTotalPaidByDangKyIdAsync(int dangKyLopHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = "SELECT ISNULL(SUM(SoTien), 0.00) FROM dbo.PhieuThu WHERE DangKyLopHocId = @DangKyLopHocId AND TrangThai = 1;";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dangKyLopHocId });

        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToDecimal(result) : 0.00m;
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage, PhieuThuDto? Data)> ThuHocPhiAtomicAsync(
        int dangKyLopHocId, decimal soTien, HinhThucThanhToan hinhThuc, int nguoiThuTaiKhoanId, string? ghiChu)
    {
        if (soTien <= 0m)
            return (false, ErrorCodes.VALIDATION_ERROR, "Số tiền thu phải lớn hơn 0.", null);

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // 1. Khóa và lấy thông tin bản ghi đăng ký lớp học
            const string dkSql = """
                SELECT dk.Id, dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                       l.KhoaHocId, kh.TenKhoaHoc,
                       dk.HocVienId, hv.MaHocVien, hv.HoTen,
                       dk.HocPhiPhaiThu, dk.TrangThai
                FROM dbo.DangKyLopHoc dk WITH (UPDLOCK, HOLDLOCK)
                INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
                INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
                INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
                WHERE dk.Id = @DangKyLopHocId;
                """;

            int lopHocId = 0;
            string maLopHoc = string.Empty;
            string tenLopHoc = string.Empty;
            int khoaHocId = 0;
            string tenKhoaHoc = string.Empty;
            int hocVienId = 0;
            string maHocVien = string.Empty;
            string hoTenHocVien = string.Empty;
            decimal hocPhiPhaiThu = 0m;
            int trangThaiDk = 0;

            await using (var cmd = new SqlCommand(dkSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dangKyLopHocId });
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.NOT_FOUND, "Không tìm thấy thông tin đăng ký lớp học.", null);
                }

                lopHocId = reader.GetInt32(1);
                maLopHoc = reader.GetString(2);
                tenLopHoc = reader.GetString(3);
                khoaHocId = reader.GetInt32(4);
                tenKhoaHoc = reader.GetString(5);
                hocVienId = reader.GetInt32(6);
                maHocVien = reader.GetString(7);
                hoTenHocVien = reader.GetString(8);
                hocPhiPhaiThu = reader.GetDecimal(9);
                trangThaiDk = reader.GetInt32(10);
            }

            if (trangThaiDk != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể thu học phí vì đăng ký này đã bị hủy hoặc ngừng hoạt động.", null);
            }

            // 2. Tính tổng tiền đã thu hiện tại với khóa UPDLOCK/HOLDLOCK
            const string sumSql = """
                SELECT ISNULL(SUM(SoTien), 0.00)
                FROM dbo.PhieuThu WITH (UPDLOCK, HOLDLOCK)
                WHERE DangKyLopHocId = @DangKyLopHocId AND TrangThai = 1;
                """;

            decimal daThu = 0m;
            await using (var cmd = new SqlCommand(sumSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dangKyLopHocId });
                var res = await cmd.ExecuteScalarAsync();
                daThu = res != null ? Convert.ToDecimal(res) : 0m;
            }

            var conNo = Math.Max(0m, hocPhiPhaiThu - daThu);
            if (conNo <= 0m)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Học viên này đã nộp đủ học phí, không còn công nợ.", null);
            }

            if (soTien > conNo)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Số tiền thu ({soTien:N0} đ) vượt quá số tiền còn nợ ({conNo:N0} đ).", null);
            }

            // 3. Sinh mã phiếu thu duy nhất
            var maPhieuThu = $"PT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

            const string insertSql = """
                INSERT INTO dbo.PhieuThu (MaPhieuThu, DangKyLopHocId, NgayThu, SoTien, HinhThuc, NguoiThuTaiKhoanId, GhiChu, TrangThai)
                OUTPUT INSERTED.Id, INSERTED.NgayThu
                VALUES (@MaPhieuThu, @DangKyLopHocId, SYSDATETIME(), @SoTien, @HinhThuc, @NguoiThuTaiKhoanId, @GhiChu, 1);
                """;

            int newId = 0;
            DateTime ngayThu = DateTime.Now;

            await using (var cmd = new SqlCommand(insertSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@MaPhieuThu", SqlDbType.NVarChar, 30) { Value = maPhieuThu });
                cmd.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dangKyLopHocId });
                cmd.Parameters.Add(new SqlParameter("@SoTien", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = soTien });
                cmd.Parameters.Add(new SqlParameter("@HinhThuc", SqlDbType.NVarChar, 20) { Value = hinhThuc.ToString() });
                cmd.Parameters.Add(new SqlParameter("@NguoiThuTaiKhoanId", SqlDbType.Int) { Value = nguoiThuTaiKhoanId });
                cmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, ghiChu, 250));

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    newId = reader.GetInt32(0);
                    ngayThu = reader.GetDateTime(1);
                }
            }

            // 4. Lấy tên người thu
            const string tkSql = "SELECT TenDangNhap FROM dbo.TaiKhoan WHERE Id = @Id;";
            string nguoiThuTen = string.Empty;
            await using (var cmd = new SqlCommand(tkSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = nguoiThuTaiKhoanId });
                var res = await cmd.ExecuteScalarAsync();
                nguoiThuTen = res?.ToString() ?? string.Empty;
            }

            await transaction.CommitAsync();

            var dto = new PhieuThuDto
            {
                Id = newId,
                MaPhieuThu = maPhieuThu,
                DangKyLopHocId = dangKyLopHocId,
                LopHocId = lopHocId,
                MaLopHoc = maLopHoc,
                TenLopHoc = tenLopHoc,
                KhoaHocId = khoaHocId,
                TenKhoaHoc = tenKhoaHoc,
                HocVienId = hocVienId,
                MaHocVien = maHocVien,
                HoTenHocVien = hoTenHocVien,
                NgayThu = ngayThu,
                SoTien = soTien,
                HinhThuc = hinhThuc,
                NguoiThuTaiKhoanId = nguoiThuTaiKhoanId,
                NguoiThuTen = nguoiThuTen,
                GhiChu = ghiChu,
                TrangThai = TrangThaiPhieuThu.HieuLuc
            };

            return (true, null, null, dto);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, ErrorCodes.UNEXPECTED_ERROR, "Không thể lập phiếu thu học phí do lỗi hệ thống.", null);
        }
    }

    private static PhieuThuDto MapFromReader(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        MaPhieuThu = reader.GetString(reader.GetOrdinal("MaPhieuThu")),
        DangKyLopHocId = reader.GetInt32(reader.GetOrdinal("DangKyLopHocId")),
        LopHocId = reader.GetInt32(reader.GetOrdinal("LopHocId")),
        MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
        TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
        KhoaHocId = reader.GetInt32(reader.GetOrdinal("KhoaHocId")),
        TenKhoaHoc = reader.GetString(reader.GetOrdinal("TenKhoaHoc")),
        HocVienId = reader.GetInt32(reader.GetOrdinal("HocVienId")),
        MaHocVien = reader.GetString(reader.GetOrdinal("MaHocVien")),
        HoTenHocVien = reader.GetString(reader.GetOrdinal("HoTenHocVien")),
        NgayThu = reader.GetDateTime(reader.GetOrdinal("NgayThu")),
        SoTien = reader.GetDecimal(reader.GetOrdinal("SoTien")),
        HinhThuc = Enum.TryParse<HinhThucThanhToan>(reader.GetString(reader.GetOrdinal("HinhThuc")), out var ht) ? ht : HinhThucThanhToan.TIEN_MAT,
        NguoiThuTaiKhoanId = reader.GetInt32(reader.GetOrdinal("NguoiThuTaiKhoanId")),
        NguoiThuTen = reader.GetString(reader.GetOrdinal("NguoiThuTen")),
        GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu")),
        TrangThai = (TrangThaiPhieuThu)reader.GetInt32(reader.GetOrdinal("TrangThai")),
        LyDoHuy = reader.IsDBNull(reader.GetOrdinal("LyDoHuy")) ? null : reader.GetString(reader.GetOrdinal("LyDoHuy")),
        NgayHuy = reader.IsDBNull(reader.GetOrdinal("NgayHuy")) ? null : reader.GetDateTime(reader.GetOrdinal("NgayHuy")),
        NguoiHuyTaiKhoanId = reader.IsDBNull(reader.GetOrdinal("NguoiHuyTaiKhoanId")) ? null : reader.GetInt32(reader.GetOrdinal("NguoiHuyTaiKhoanId")),
        NguoiHuyTen = reader.IsDBNull(reader.GetOrdinal("NguoiHuyTen")) ? null : reader.GetString(reader.GetOrdinal("NguoiHuyTen"))
    };
}
