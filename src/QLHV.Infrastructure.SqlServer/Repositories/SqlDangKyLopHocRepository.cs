using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlDangKyLopHocRepository : IDangKyLopHocRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlDangKyLopHocRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<DangKyLopHocDto>> GetAllAsync(DangKyLopHocFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT dk.Id, dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien, hv.DienThoai AS DienThoaiHocVien,
                   dk.NgayDangKy, dk.HocPhiPhaiThu, dk.GhiChu, dk.TrangThai, dk.NgayTao,
                   ISNULL((SELECT SUM(pt.SoTien) FROM dbo.PhieuThu pt WHERE pt.DangKyLopHocId = dk.Id AND pt.TrangThai = 1), 0.00) AS DaThu
            FROM dbo.DangKyLopHoc dk
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
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

            if (filter.HocVienId.HasValue)
            {
                sqlBuilder.Append(" AND dk.HocVienId = @HocVienId");
                command.Parameters.Add(new SqlParameter("@HocVienId", SqlDbType.Int) { Value = filter.HocVienId.Value });
            }

            if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
            {
                var escaped = SqlRepositoryHelper.EscapeLike(filter.TuKhoa);
                sqlBuilder.Append("""
                     AND (hv.MaHocVien LIKE @TuKhoa ESCAPE N'~'
                       OR hv.HoTen LIKE @TuKhoa ESCAPE N'~'
                       OR hv.HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~'
                       OR l.MaLopHoc LIKE @TuKhoa ESCAPE N'~')
                    """);
                command.Parameters.Add(new SqlParameter("@TuKhoa", SqlDbType.NVarChar) { Value = $"%{escaped}%" });
            }

            if (filter.TrangThai.HasValue)
            {
                sqlBuilder.Append(" AND dk.TrangThai = @TrangThai");
                command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)filter.TrangThai.Value });
            }
        }

        sqlBuilder.Append(" ORDER BY dk.Id DESC;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<DangKyLopHocDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapFromReader(reader));
        }

        return list;
    }

    public async Task<DangKyLopHocDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT dk.Id, dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien, hv.DienThoai AS DienThoaiHocVien,
                   dk.NgayDangKy, dk.HocPhiPhaiThu, dk.GhiChu, dk.TrangThai, dk.NgayTao,
                   ISNULL((SELECT SUM(pt.SoTien) FROM dbo.PhieuThu pt WHERE pt.DangKyLopHocId = dk.Id AND pt.TrangThai = 1), 0.00) AS DaThu
            FROM dbo.DangKyLopHoc dk
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            WHERE dk.Id = @Id;
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

    public async Task<DangKyLopHocDto?> GetByLopAndHocVienAsync(int lopHocId, int hocVienId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT dk.Id, dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien, hv.DienThoai AS DienThoaiHocVien,
                   dk.NgayDangKy, dk.HocPhiPhaiThu, dk.GhiChu, dk.TrangThai, dk.NgayTao,
                   ISNULL((SELECT SUM(pt.SoTien) FROM dbo.PhieuThu pt WHERE pt.DangKyLopHocId = dk.Id AND pt.TrangThai = 1), 0.00) AS DaThu
            FROM dbo.DangKyLopHoc dk
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            WHERE dk.LopHocId = @LopHocId AND dk.HocVienId = @HocVienId;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });
        command.Parameters.Add(new SqlParameter("@HocVienId", SqlDbType.Int) { Value = hocVienId });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }

        return null;
    }

    public async Task<DangKyLopHocDto> AddAsync(DangKyLopHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            INSERT INTO dbo.DangKyLopHoc (LopHocId, HocVienId, NgayDangKy, HocPhiPhaiThu, GhiChu, TrangThai, NgayTao)
            OUTPUT INSERTED.Id, INSERTED.NgayTao
            VALUES (@LopHocId, @HocVienId, @NgayDangKy, @HocPhiPhaiThu, @GhiChu, @TrangThai, SYSDATETIME());
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = dto.LopHocId });
        command.Parameters.Add(new SqlParameter("@HocVienId", SqlDbType.Int) { Value = dto.HocVienId });
        command.Parameters.Add(new SqlParameter("@NgayDangKy", SqlDbType.Date) { Value = dto.NgayDangKy.Date });
        command.Parameters.Add(new SqlParameter("@HocPhiPhaiThu", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = dto.HocPhiPhaiThu });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, dto.GhiChu, 250));
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            dto.Id = reader.GetInt32(0);
            dto.NgayTao = reader.GetDateTime(1);
        }

        if (dto.KhoaHocId == 0 || string.IsNullOrWhiteSpace(dto.TenKhoaHoc))
        {
            await reader.CloseAsync();
            const string lookupSql = """
                SELECT l.KhoaHocId, kh.TenKhoaHoc
                FROM dbo.LopHoc l
                INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
                WHERE l.Id = @LopHocId;
                """;
            await using var lkCmd = new SqlCommand(lookupSql, connection);
            lkCmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = dto.LopHocId });
            await using var lkReader = await lkCmd.ExecuteReaderAsync();
            if (await lkReader.ReadAsync())
            {
                dto.KhoaHocId = lkReader.GetInt32(0);
                dto.TenKhoaHoc = lkReader.GetString(1);
            }
        }

        return dto;
    }

    public async Task<bool> UpdateAsync(DangKyLopHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            UPDATE dbo.DangKyLopHoc
            SET GhiChu = @GhiChu,
                TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, dto.GhiChu, 250));
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = "UPDATE dbo.DangKyLopHoc SET TrangThai = @TrangThai WHERE Id = @Id;";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)trangThai });

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<int> CountByLopHocIdAsync(int lopHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = "SELECT COUNT(*) FROM dbo.DangKyLopHoc WHERE LopHocId = @LopHocId AND TrangThai = 1;";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });

        var result = await command.ExecuteScalarAsync();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    public async Task<IReadOnlyList<CongNoHocPhiDto>> GetCongNoByLopHocAsync(int lopHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT dk.Id AS DangKyLopHocId, dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien, hv.DienThoai,
                   dk.HocPhiPhaiThu,
                   ISNULL((SELECT SUM(pt.SoTien) FROM dbo.PhieuThu pt WHERE pt.DangKyLopHocId = dk.Id AND pt.TrangThai = 1), 0.00) AS DaThu
            FROM dbo.DangKyLopHoc dk
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            WHERE dk.LopHocId = @LopHocId AND dk.TrangThai = 1
            ORDER BY hv.MaHocVien;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<CongNoHocPhiDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new CongNoHocPhiDto
            {
                DangKyLopHocId = reader.GetInt32(reader.GetOrdinal("DangKyLopHocId")),
                LopHocId = reader.GetInt32(reader.GetOrdinal("LopHocId")),
                MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
                TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
                KhoaHocId = reader.GetInt32(reader.GetOrdinal("KhoaHocId")),
                TenKhoaHoc = reader.GetString(reader.GetOrdinal("TenKhoaHoc")),
                HocVienId = reader.GetInt32(reader.GetOrdinal("HocVienId")),
                MaHocVien = reader.GetString(reader.GetOrdinal("MaHocVien")),
                HoTenHocVien = reader.GetString(reader.GetOrdinal("HoTenHocVien")),
                DienThoai = reader.IsDBNull(reader.GetOrdinal("DienThoai")) ? null : reader.GetString(reader.GetOrdinal("DienThoai")),
                HocPhiPhaiThu = reader.GetDecimal(reader.GetOrdinal("HocPhiPhaiThu")),
                DaThu = reader.GetDecimal(reader.GetOrdinal("DaThu"))
            });
        }

        return list;
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage, DangKyLopHocDto? Data)> EnrollAtomicAsync(
        int lopHocId, int hocVienId, decimal hocPhiPhaiThu, string? ghiChu,
        string? maHocVien = null, string? hoTenHocVien = null, string? dienThoaiHocVien = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // 1. Khóa lớp học và kiểm tra trạng thái, sĩ số tối đa
            const string checkLopSql = """
                SELECT l.SiSoToiDa, l.TrangThai, l.MaLopHoc, l.TenLopHoc, l.KhoaHocId, kh.TenKhoaHoc
                FROM dbo.LopHoc l WITH (UPDLOCK, HOLDLOCK)
                INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
                WHERE l.Id = @LopHocId;
                """;

            string maLopHoc = string.Empty;
            string tenLopHoc = string.Empty;
            int khoaHocId = 0;
            string tenKhoaHoc = string.Empty;
            int siSoToiDa = 0;
            int trangThaiLop = 0;

            await using (var cmd = new SqlCommand(checkLopSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.NOT_FOUND, "Không tìm thấy lớp học.", null);
                }
                siSoToiDa = reader.GetInt32(0);
                trangThaiLop = reader.GetInt32(1);
                maLopHoc = reader.GetString(2);
                tenLopHoc = reader.GetString(3);
                khoaHocId = reader.GetInt32(4);
                tenKhoaHoc = reader.GetString(5);
            }

            if (trangThaiLop != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động.", null);
            }

            // 2. Kiểm tra học viên còn hoạt động
            const string checkHVSql = """
                SELECT MaHocVien, HoTen, DienThoai, TrangThai
                FROM dbo.HocVien WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @HocVienId;
                """;

            string actualMaHV = string.Empty;
            string actualHoTenHV = string.Empty;
            string? actualDienThoaiHV = null;
            int trangThaiHV = 0;

            await using (var cmd = new SqlCommand(checkHVSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@HocVienId", SqlDbType.Int) { Value = hocVienId });
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.NOT_FOUND, "Không tìm thấy học viên.", null);
                }
                actualMaHV = reader.GetString(0);
                actualHoTenHV = reader.GetString(1);
                actualDienThoaiHV = reader.IsDBNull(2) ? null : reader.GetString(2);
                trangThaiHV = reader.GetInt32(3);
            }

            if (trangThaiHV != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Học viên đã ngừng hoạt động.", null);
            }

            // 3. Đếm sĩ số hiện tại của lớp với khóa HOLDLOCK
            const string countSql = """
                SELECT COUNT(*)
                FROM dbo.DangKyLopHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE LopHocId = @LopHocId AND TrangThai = 1;
                """;

            int currentCount = 0;
            await using (var cmd = new SqlCommand(countSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });
                var res = await cmd.ExecuteScalarAsync();
                currentCount = res != null ? Convert.ToInt32(res) : 0;
            }

            // 4. Kiểm tra xem học viên này đã có bản ghi đăng ký nào trong lớp chưa (cả active lẫn inactive)
            const string checkExistingSql = """
                SELECT Id, TrangThai
                FROM dbo.DangKyLopHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE LopHocId = @LopHocId AND HocVienId = @HocVienId;
                """;

            int? existingId = null;
            int? existingTrangThai = null;
            await using (var cmd = new SqlCommand(checkExistingSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });
                cmd.Parameters.Add(new SqlParameter("@HocVienId", SqlDbType.Int) { Value = hocVienId });
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    existingId = reader.GetInt32(0);
                    existingTrangThai = reader.GetInt32(1);
                }
            }

            if (existingTrangThai == (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.ALREADY_EXISTS, "Học viên này đã có trong danh sách lớp học.", null);
            }

            if (currentCount >= siSoToiDa)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Lớp học đã đạt sĩ số tối đa ({siSoToiDa} học viên). Không thể xếp thêm.", null);
            }

            int finalId = 0;
            DateTime ngayDangKy = DateTime.Today;
            DateTime ngayTao = DateTime.Now;

            if (existingId.HasValue)
            {
                // Reactivate đăng ký cũ và cập nhật học phí snapshot mới
                const string updateSql = """
                    UPDATE dbo.DangKyLopHoc
                    SET TrangThai = 1,
                        HocPhiPhaiThu = @HocPhiPhaiThu,
                        GhiChu = @GhiChu,
                        NgayDangKy = @NgayDangKy
                    WHERE Id = @Id;
                    """;
                await using var cmd = new SqlCommand(updateSql, connection, transaction);
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = existingId.Value });
                cmd.Parameters.Add(new SqlParameter("@HocPhiPhaiThu", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = hocPhiPhaiThu });
                cmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, ghiChu, 250));
                cmd.Parameters.Add(new SqlParameter("@NgayDangKy", SqlDbType.Date) { Value = ngayDangKy });
                await cmd.ExecuteNonQueryAsync();

                finalId = existingId.Value;
            }
            else
            {
                // Chèn mới
                const string insertSql = """
                    INSERT INTO dbo.DangKyLopHoc (LopHocId, HocVienId, NgayDangKy, HocPhiPhaiThu, GhiChu, TrangThai, NgayTao)
                    OUTPUT INSERTED.Id, INSERTED.NgayTao
                    VALUES (@LopHocId, @HocVienId, @NgayDangKy, @HocPhiPhaiThu, @GhiChu, 1, SYSDATETIME());
                    """;
                await using var cmd = new SqlCommand(insertSql, connection, transaction);
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });
                cmd.Parameters.Add(new SqlParameter("@HocVienId", SqlDbType.Int) { Value = hocVienId });
                cmd.Parameters.Add(new SqlParameter("@NgayDangKy", SqlDbType.Date) { Value = ngayDangKy });
                cmd.Parameters.Add(new SqlParameter("@HocPhiPhaiThu", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = hocPhiPhaiThu });
                cmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, ghiChu, 250));

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    finalId = reader.GetInt32(0);
                    ngayTao = reader.GetDateTime(1);
                }
            }

            await transaction.CommitAsync();

            var dto = new DangKyLopHocDto
            {
                Id = finalId,
                LopHocId = lopHocId,
                MaLopHoc = maLopHoc,
                TenLopHoc = tenLopHoc,
                KhoaHocId = khoaHocId,
                TenKhoaHoc = tenKhoaHoc,
                HocVienId = hocVienId,
                MaHocVien = actualMaHV,
                HoTenHocVien = actualHoTenHV,
                DienThoaiHocVien = actualDienThoaiHV,
                NgayDangKy = ngayDangKy,
                HocPhiPhaiThu = hocPhiPhaiThu,
                DaThu = 0m,
                GhiChu = ghiChu,
                TrangThai = TrangThaiHoatDong.DangHoatDong,
                NgayTao = ngayTao
            };

            return (true, null, null, dto);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, ErrorCodes.UNEXPECTED_ERROR, "Không thể xếp học viên vào lớp do lỗi hệ thống.", null);
        }
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage)> CancelEnrollmentAtomicAsync(int dangKyLopHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // 1. Khóa bản ghi đăng ký
            const string checkDkSql = """
                SELECT TrangThai
                FROM dbo.DangKyLopHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @Id;
                """;

            int currentStatus = 0;
            await using (var cmd = new SqlCommand(checkDkSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dangKyLopHocId });
                var res = await cmd.ExecuteScalarAsync();
                if (res == null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.NOT_FOUND, "Không tìm thấy thông tin đăng ký lớp.");
                }
                currentStatus = Convert.ToInt32(res);
            }

            if (currentStatus == (int)TrangThaiHoatDong.NgungHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Đăng ký này đã bị hủy trước đó.");
            }

            // 2. Kiểm tra các phiếu thu có hiệu lực với khóa HOLDLOCK
            const string checkPtSql = """
                SELECT ISNULL(SUM(SoTien), 0.00)
                FROM dbo.PhieuThu WITH (UPDLOCK, HOLDLOCK)
                WHERE DangKyLopHocId = @DangKyLopHocId AND TrangThai = 1;
                """;

            decimal totalPaid = 0m;
            await using (var cmd = new SqlCommand(checkPtSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dangKyLopHocId });
                var res = await cmd.ExecuteScalarAsync();
                totalPaid = res != null ? Convert.ToDecimal(res) : 0m;
            }

            if (totalPaid > 0m)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Không thể hủy đăng ký vì học viên đã đóng {totalPaid:N0} đ học phí. Vui lòng hủy các phiếu thu hợp lệ trước.");
            }

            // 3. Thực hiện chuyển trạng thái đăng ký sang NgungHoatDong
            const string cancelSql = "UPDATE dbo.DangKyLopHoc SET TrangThai = 0 WHERE Id = @Id;";
            await using (var cmd = new SqlCommand(cancelSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dangKyLopHocId });
                await cmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return (true, null, null);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, ErrorCodes.UNEXPECTED_ERROR, "Không thể hủy đăng ký lớp học do lỗi hệ thống.");
        }
    }

    private static DangKyLopHocDto MapFromReader(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        LopHocId = reader.GetInt32(reader.GetOrdinal("LopHocId")),
        MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
        TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
        KhoaHocId = reader.GetInt32(reader.GetOrdinal("KhoaHocId")),
        TenKhoaHoc = reader.GetString(reader.GetOrdinal("TenKhoaHoc")),
        HocVienId = reader.GetInt32(reader.GetOrdinal("HocVienId")),
        MaHocVien = reader.GetString(reader.GetOrdinal("MaHocVien")),
        HoTenHocVien = reader.GetString(reader.GetOrdinal("HoTenHocVien")),
        DienThoaiHocVien = reader.IsDBNull(reader.GetOrdinal("DienThoaiHocVien")) ? null : reader.GetString(reader.GetOrdinal("DienThoaiHocVien")),
        NgayDangKy = reader.GetDateTime(reader.GetOrdinal("NgayDangKy")),
        HocPhiPhaiThu = reader.GetDecimal(reader.GetOrdinal("HocPhiPhaiThu")),
        DaThu = reader.GetDecimal(reader.GetOrdinal("DaThu")),
        GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu")),
        TrangThai = (TrangThaiHoatDong)reader.GetInt32(reader.GetOrdinal("TrangThai")),
        NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao"))
    };
}
