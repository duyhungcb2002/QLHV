using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlLichHocRepository : ILichHocRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlLichHocRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<LichHocDto>> GetAllAsync(LichHocFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT lh.Id, lh.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   lh.MonHocId, mh.MaMonHoc, mh.TenMonHoc,
                   lh.GiangVienId, gv.MaGiangVien, gv.HoTen AS HoTenGiangVien,
                   lh.PhongHocId, ph.MaPhong, ph.TenPhong,
                   lh.NgayHoc, lh.GioBatDau, lh.GioKetThuc, lh.NoiDung, lh.TrangThai
            FROM dbo.LichHoc lh
            INNER JOIN dbo.LopHoc l ON lh.LopHocId = l.Id
            INNER JOIN dbo.MonHoc mh ON lh.MonHocId = mh.Id
            INNER JOIN dbo.GiangVien gv ON lh.GiangVienId = gv.Id
            INNER JOIN dbo.PhongHoc ph ON lh.PhongHocId = ph.Id
            WHERE 1=1
            """);

        await using var command = new SqlCommand();
        command.Connection = connection;

        if (filter != null)
        {
            if (filter.LopHocId.HasValue)
            {
                sqlBuilder.Append(" AND lh.LopHocId = @LopHocId");
                command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = filter.LopHocId.Value });
            }

            if (filter.MonHocId.HasValue)
            {
                sqlBuilder.Append(" AND lh.MonHocId = @MonHocId");
                command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = filter.MonHocId.Value });
            }

            if (filter.GiangVienId.HasValue)
            {
                sqlBuilder.Append(" AND lh.GiangVienId = @GiangVienId");
                command.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = filter.GiangVienId.Value });
            }

            if (filter.PhongHocId.HasValue)
            {
                sqlBuilder.Append(" AND lh.PhongHocId = @PhongHocId");
                command.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = filter.PhongHocId.Value });
            }

            if (filter.TuNgay.HasValue)
            {
                sqlBuilder.Append(" AND lh.NgayHoc >= @TuNgay");
                command.Parameters.Add(new SqlParameter("@TuNgay", SqlDbType.Date) { Value = filter.TuNgay.Value.Date });
            }

            if (filter.DenNgay.HasValue)
            {
                sqlBuilder.Append(" AND lh.NgayHoc <= @DenNgay");
                command.Parameters.Add(new SqlParameter("@DenNgay", SqlDbType.Date) { Value = filter.DenNgay.Value.Date });
            }

            if (filter.TrangThai.HasValue)
            {
                sqlBuilder.Append(" AND lh.TrangThai = @TrangThai");
                command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)filter.TrangThai.Value });
            }
        }

        sqlBuilder.Append(" ORDER BY lh.NgayHoc ASC, lh.GioBatDau ASC;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<LichHocDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapFromReader(reader));
        }

        return list;
    }

    public async Task<LichHocDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT lh.Id, lh.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   lh.MonHocId, mh.MaMonHoc, mh.TenMonHoc,
                   lh.GiangVienId, gv.MaGiangVien, gv.HoTen AS HoTenGiangVien,
                   lh.PhongHocId, ph.MaPhong, ph.TenPhong,
                   lh.NgayHoc, lh.GioBatDau, lh.GioKetThuc, lh.NoiDung, lh.TrangThai
            FROM dbo.LichHoc lh
            INNER JOIN dbo.LopHoc l ON lh.LopHocId = l.Id
            INNER JOIN dbo.MonHoc mh ON lh.MonHocId = mh.Id
            INNER JOIN dbo.GiangVien gv ON lh.GiangVienId = gv.Id
            INNER JOIN dbo.PhongHoc ph ON lh.PhongHocId = ph.Id
            WHERE lh.Id = @Id;
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

    public async Task<LichHocDto> AddAsync(LichHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            INSERT INTO dbo.LichHoc (LopHocId, MonHocId, GiangVienId, PhongHocId, NgayHoc, GioBatDau, GioKetThuc, NoiDung, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@LopHocId, @MonHocId, @GiangVienId, @PhongHocId, @NgayHoc, @GioBatDau, @GioKetThuc, @NoiDung, @TrangThai);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = dto.LopHocId });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
        command.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = dto.GiangVienId });
        command.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = dto.PhongHocId });
        command.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
        command.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
        command.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@NoiDung", SqlDbType.NVarChar, dto.NoiDung, 500));
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        var res = await command.ExecuteScalarAsync();
        if (res != null)
        {
            dto.Id = Convert.ToInt32(res);
        }

        return dto;
    }

    public async Task<bool> UpdateAsync(LichHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            UPDATE dbo.LichHoc
            SET MonHocId = @MonHocId,
                GiangVienId = @GiangVienId,
                PhongHocId = @PhongHocId,
                NgayHoc = @NgayHoc,
                GioBatDau = @GioBatDau,
                GioKetThuc = @GioKetThuc,
                NoiDung = @NoiDung,
                TrangThai = @TrangThai
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
        command.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = dto.GiangVienId });
        command.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = dto.PhongHocId });
        command.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
        command.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
        command.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@NoiDung", SqlDbType.NVarChar, dto.NoiDung, 500));
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = "UPDATE dbo.LichHoc SET TrangThai = @TrangThai WHERE Id = @Id;";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)trangThai });

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Chuyển soft-cancellation: ngừng hoạt động lịch học thay vì hard-delete
        return await SetTrangThaiAsync(id, TrangThaiHoatDong.NgungHoatDong);
    }

    public async Task<bool> HasConflictPhongAsync(int phongHocId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT 1 FROM dbo.LichHoc
            WHERE PhongHocId = @PhongHocId
              AND NgayHoc = @NgayHoc
              AND TrangThai = 1
              AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau)
            """);

        if (excludeId.HasValue)
        {
            sqlBuilder.Append(" AND Id <> @ExcludeId");
        }

        await using var command = new SqlCommand(sqlBuilder.ToString(), connection);
        command.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = phongHocId });
        command.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = ngayHoc.Date });
        command.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = gioBatDau });
        command.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = gioKetThuc });
        if (excludeId.HasValue)
        {
            command.Parameters.Add(new SqlParameter("@ExcludeId", SqlDbType.Int) { Value = excludeId.Value });
        }

        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<bool> HasConflictGiangVienAsync(int giangVienId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT 1 FROM dbo.LichHoc
            WHERE GiangVienId = @GiangVienId
              AND NgayHoc = @NgayHoc
              AND TrangThai = 1
              AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau)
            """);

        if (excludeId.HasValue)
        {
            sqlBuilder.Append(" AND Id <> @ExcludeId");
        }

        await using var command = new SqlCommand(sqlBuilder.ToString(), connection);
        command.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = giangVienId });
        command.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = ngayHoc.Date });
        command.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = gioBatDau });
        command.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = gioKetThuc });
        if (excludeId.HasValue)
        {
            command.Parameters.Add(new SqlParameter("@ExcludeId", SqlDbType.Int) { Value = excludeId.Value });
        }

        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<bool> HasConflictLopHocAsync(int lopHocId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT 1 FROM dbo.LichHoc
            WHERE LopHocId = @LopHocId
              AND NgayHoc = @NgayHoc
              AND TrangThai = 1
              AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau)
            """);

        if (excludeId.HasValue)
        {
            sqlBuilder.Append(" AND Id <> @ExcludeId");
        }

        await using var command = new SqlCommand(sqlBuilder.ToString(), connection);
        command.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = lopHocId });
        command.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = ngayHoc.Date });
        command.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = gioBatDau });
        command.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = gioKetThuc });
        if (excludeId.HasValue)
        {
            command.Parameters.Add(new SqlParameter("@ExcludeId", SqlDbType.Int) { Value = excludeId.Value });
        }

        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage, LichHocDto? Data)> CreateAtomicAsync(LichHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // 1. Kiểm tra lớp học, môn học, giảng viên, phòng học có hợp lệ và đang hoạt động không (UPDLOCK)
            const string checkEntitiesSql = """
                SELECT
                    (SELECT TrangThai FROM dbo.LopHoc WITH (UPDLOCK, HOLDLOCK) WHERE Id = @LopHocId),
                    (SELECT NgayBatDau FROM dbo.LopHoc WHERE Id = @LopHocId),
                    (SELECT NgayKetThuc FROM dbo.LopHoc WHERE Id = @LopHocId),
                    (SELECT KhoaHocId FROM dbo.LopHoc WHERE Id = @LopHocId),
                    (SELECT SiSoToiDa FROM dbo.LopHoc WHERE Id = @LopHocId),
                    (SELECT TrangThai FROM dbo.MonHoc WITH (UPDLOCK, HOLDLOCK) WHERE Id = @MonHocId),
                    (SELECT TrangThai FROM dbo.GiangVien WITH (UPDLOCK, HOLDLOCK) WHERE Id = @GiangVienId),
                    (SELECT TrangThai FROM dbo.PhongHoc WITH (UPDLOCK, HOLDLOCK) WHERE Id = @PhongHocId),
                    (SELECT SucChua FROM dbo.PhongHoc WHERE Id = @PhongHocId);
                """;

            int? trangThaiLop = null;
            DateTime? ngayBatDauLop = null;
            DateTime? ngayKetThucLop = null;
            int? khoaHocId = null;
            int? siSoToiDa = null;
            int? trangThaiMon = null;
            int? trangThaiGv = null;
            int? trangThaiPhong = null;
            int? sucChuaPhong = null;

            await using (var cmd = new SqlCommand(checkEntitiesSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = dto.LopHocId });
                cmd.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
                cmd.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = dto.GiangVienId });
                cmd.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = dto.PhongHocId });

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    trangThaiLop = reader.IsDBNull(0) ? null : reader.GetInt32(0);
                    ngayBatDauLop = reader.IsDBNull(1) ? null : reader.GetDateTime(1);
                    ngayKetThucLop = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
                    khoaHocId = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                    siSoToiDa = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                    trangThaiMon = reader.IsDBNull(5) ? null : reader.GetInt32(5);
                    trangThaiGv = reader.IsDBNull(6) ? null : reader.GetInt32(6);
                    trangThaiPhong = reader.IsDBNull(7) ? null : reader.GetInt32(7);
                    sucChuaPhong = reader.IsDBNull(8) ? null : reader.GetInt32(8);
                }
            }

            if (trangThaiLop != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học không tồn tại hoặc đã ngừng hoạt động.", null);
            }

            if (trangThaiMon != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không tồn tại hoặc đã ngừng hoạt động.", null);
            }

            if (trangThaiGv != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên không tồn tại hoặc đã ngừng hoạt động.", null);
            }

            if (trangThaiPhong != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học không tồn tại hoặc đã ngừng hoạt động.", null);
            }

            // 2. Kiểm tra môn có thuộc khóa học của lớp
            const string checkKhoaMonSql = "SELECT 1 FROM dbo.KhoaHocMonHoc WHERE KhoaHocId = @KhoaHocId AND MonHocId = @MonHocId;";
            await using (var cmd = new SqlCommand(checkKhoaMonSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = khoaHocId!.Value });
                cmd.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
                var res = await cmd.ExecuteScalarAsync();
                if (res == null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của khóa học này.", null);
                }
            }

            // 3. Kiểm tra ngày học nằm trong thời gian bắt đầu - kết thúc của lớp
            if (ngayBatDauLop.HasValue && dto.NgayHoc.Date < ngayBatDauLop.Value.Date)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.VALIDATION_ERROR,
                    $"Ngày học ({dto.NgayHoc:dd/MM/yyyy}) không được trước ngày bắt đầu của lớp ({ngayBatDauLop.Value:dd/MM/yyyy}).", null);
            }

            if (ngayKetThucLop.HasValue && dto.NgayHoc.Date > ngayKetThucLop.Value.Date)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.VALIDATION_ERROR,
                    $"Ngày học ({dto.NgayHoc:dd/MM/yyyy}) không được sau ngày kết thúc của lớp ({ngayKetThucLop.Value:dd/MM/yyyy}).", null);
            }

            // 4. Kiểm tra sức chứa phòng không nhỏ hơn sĩ số tối đa của lớp
            if (sucChuaPhong.HasValue && siSoToiDa.HasValue && sucChuaPhong.Value < siSoToiDa.Value)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Sức chứa phòng học ({sucChuaPhong.Value} chỗ) nhỏ hơn sĩ số tối đa của lớp ({siSoToiDa.Value} học viên).", null);
            }

            // 5. Kiểm tra trùng phòng (khóa HOLDLOCK)
            const string conflictPhongSql = """
                SELECT 1 FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE PhongHocId = @PhongHocId AND NgayHoc = @NgayHoc AND TrangThai = 1
                  AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau);
                """;
            await using (var cmd = new SqlCommand(conflictPhongSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = dto.PhongHocId });
                cmd.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
                cmd.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
                cmd.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
                var res = await cmd.ExecuteScalarAsync();
                if (res != null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học đã có lịch xếp trong khung giờ này.", null);
                }
            }

            // 6. Kiểm tra trùng giảng viên (khóa HOLDLOCK)
            const string conflictGvSql = """
                SELECT 1 FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE GiangVienId = @GiangVienId AND NgayHoc = @NgayHoc AND TrangThai = 1
                  AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau);
                """;
            await using (var cmd = new SqlCommand(conflictGvSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = dto.GiangVienId });
                cmd.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
                cmd.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
                cmd.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
                var res = await cmd.ExecuteScalarAsync();
                if (res != null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên đã có lịch dạy trong khung giờ này.", null);
                }
            }

            // 7. Kiểm tra trùng chính lớp học (khóa HOLDLOCK)
            const string conflictLopSql = """
                SELECT 1 FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE LopHocId = @LopHocId AND NgayHoc = @NgayHoc AND TrangThai = 1
                  AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau);
                """;
            await using (var cmd = new SqlCommand(conflictLopSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = dto.LopHocId });
                cmd.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
                cmd.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
                cmd.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
                var res = await cmd.ExecuteScalarAsync();
                if (res != null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã có buổi học khác trong khung giờ này.", null);
                }
            }

            // 8. Chèn bản ghi
            const string insertSql = """
                INSERT INTO dbo.LichHoc (LopHocId, MonHocId, GiangVienId, PhongHocId, NgayHoc, GioBatDau, GioKetThuc, NoiDung, TrangThai)
                OUTPUT INSERTED.Id
                VALUES (@LopHocId, @MonHocId, @GiangVienId, @PhongHocId, @NgayHoc, @GioBatDau, @GioKetThuc, @NoiDung, 1);
                """;

            int newId = 0;
            await using (var cmd = new SqlCommand(insertSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = dto.LopHocId });
                cmd.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
                cmd.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = dto.GiangVienId });
                cmd.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = dto.PhongHocId });
                cmd.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
                cmd.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
                cmd.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
                cmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@NoiDung", SqlDbType.NVarChar, dto.NoiDung, 500));
                var res = await cmd.ExecuteScalarAsync();
                newId = res != null ? Convert.ToInt32(res) : 0;
            }

            await transaction.CommitAsync();

            dto.Id = newId;
            dto.TrangThai = TrangThaiHoatDong.DangHoatDong;
            return (true, null, null, dto);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, ErrorCodes.UNEXPECTED_ERROR, "Không thể tạo lịch học mới do lỗi hệ thống.", null);
        }
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateAtomicAsync(LichHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // 1. Kiểm tra bản ghi lịch học tồn tại và đang hoạt động
            const string checkCurrentSql = "SELECT LopHocId, TrangThai FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK) WHERE Id = @Id;";
            int lopHocId = 0;
            int trangThaiHienTai = 0;
            await using (var cmd = new SqlCommand(checkCurrentSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.NOT_FOUND, "Không tìm thấy buổi học cần sửa.");
                }
                lopHocId = reader.GetInt32(0);
                trangThaiHienTai = reader.GetInt32(1);
            }

            if (trangThaiHienTai != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể chỉnh sửa buổi học đã ngừng hoạt động/đã bị hủy.");
            }

            dto.LopHocId = lopHocId;
            dto.TrangThai = TrangThaiHoatDong.DangHoatDong;

            // 2. Kiểm tra các thực thể liên quan
            const string checkEntitiesSql = """
                SELECT
                    (SELECT TrangThai FROM dbo.LopHoc WITH (UPDLOCK, HOLDLOCK) WHERE Id = @LopHocId),
                    (SELECT NgayBatDau FROM dbo.LopHoc WHERE Id = @LopHocId),
                    (SELECT NgayKetThuc FROM dbo.LopHoc WHERE Id = @LopHocId),
                    (SELECT KhoaHocId FROM dbo.LopHoc WHERE Id = @LopHocId),
                    (SELECT SiSoToiDa FROM dbo.LopHoc WHERE Id = @LopHocId),
                    (SELECT TrangThai FROM dbo.MonHoc WITH (UPDLOCK, HOLDLOCK) WHERE Id = @MonHocId),
                    (SELECT TrangThai FROM dbo.GiangVien WITH (UPDLOCK, HOLDLOCK) WHERE Id = @GiangVienId),
                    (SELECT TrangThai FROM dbo.PhongHoc WITH (UPDLOCK, HOLDLOCK) WHERE Id = @PhongHocId),
                    (SELECT SucChua FROM dbo.PhongHoc WHERE Id = @PhongHocId);
                """;

            int? trangThaiLop = null;
            DateTime? ngayBatDauLop = null;
            DateTime? ngayKetThucLop = null;
            int? khoaHocId = null;
            int? siSoToiDa = null;
            int? trangThaiMon = null;
            int? trangThaiGv = null;
            int? trangThaiPhong = null;
            int? sucChuaPhong = null;

            await using (var cmd = new SqlCommand(checkEntitiesSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = dto.LopHocId });
                cmd.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
                cmd.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = dto.GiangVienId });
                cmd.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = dto.PhongHocId });

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    trangThaiLop = reader.IsDBNull(0) ? null : reader.GetInt32(0);
                    ngayBatDauLop = reader.IsDBNull(1) ? null : reader.GetDateTime(1);
                    ngayKetThucLop = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
                    khoaHocId = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                    siSoToiDa = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                    trangThaiMon = reader.IsDBNull(5) ? null : reader.GetInt32(5);
                    trangThaiGv = reader.IsDBNull(6) ? null : reader.GetInt32(6);
                    trangThaiPhong = reader.IsDBNull(7) ? null : reader.GetInt32(7);
                    sucChuaPhong = reader.IsDBNull(8) ? null : reader.GetInt32(8);
                }
            }

            if (trangThaiLop != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động.");
            }

            if (trangThaiMon != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không tồn tại hoặc đã ngừng hoạt động.");
            }

            if (trangThaiGv != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên không tồn tại hoặc đã ngừng hoạt động.");
            }

            if (trangThaiPhong != (int)TrangThaiHoatDong.DangHoatDong)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học không tồn tại hoặc đã ngừng hoạt động.");
            }

            // 3. Kiểm tra môn thuộc khóa học
            const string checkKhoaMonSql = "SELECT 1 FROM dbo.KhoaHocMonHoc WHERE KhoaHocId = @KhoaHocId AND MonHocId = @MonHocId;";
            await using (var cmd = new SqlCommand(checkKhoaMonSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = khoaHocId!.Value });
                cmd.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
                var res = await cmd.ExecuteScalarAsync();
                if (res == null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của khóa học này.");
                }
            }

            // 4. Kiểm tra ngày học nằm trong thời gian lớp
            if (ngayBatDauLop.HasValue && dto.NgayHoc.Date < ngayBatDauLop.Value.Date)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.VALIDATION_ERROR,
                    $"Ngày học ({dto.NgayHoc:dd/MM/yyyy}) không được trước ngày bắt đầu của lớp ({ngayBatDauLop.Value:dd/MM/yyyy}).");
            }

            if (ngayKetThucLop.HasValue && dto.NgayHoc.Date > ngayKetThucLop.Value.Date)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.VALIDATION_ERROR,
                    $"Ngày học ({dto.NgayHoc:dd/MM/yyyy}) không được sau ngày kết thúc của lớp ({ngayKetThucLop.Value:dd/MM/yyyy}).");
            }

            // 5. Kiểm tra sức chứa phòng
            if (sucChuaPhong.HasValue && siSoToiDa.HasValue && sucChuaPhong.Value < siSoToiDa.Value)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Sức chứa phòng học ({sucChuaPhong.Value} chỗ) nhỏ hơn sĩ số tối đa của lớp ({siSoToiDa.Value} học viên).");
            }

            // 6. Kiểm tra xung đột phòng (loại trừ chính id đang sửa)
            const string conflictPhongSql = """
                SELECT 1 FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE PhongHocId = @PhongHocId AND NgayHoc = @NgayHoc AND TrangThai = 1 AND Id <> @Id
                  AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau);
                """;
            await using (var cmd = new SqlCommand(conflictPhongSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = dto.PhongHocId });
                cmd.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
                cmd.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
                cmd.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                var res = await cmd.ExecuteScalarAsync();
                if (res != null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học đã có lịch xếp trong khung giờ này.");
                }
            }

            // 7. Kiểm tra xung đột giảng viên
            const string conflictGvSql = """
                SELECT 1 FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE GiangVienId = @GiangVienId AND NgayHoc = @NgayHoc AND TrangThai = 1 AND Id <> @Id
                  AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau);
                """;
            await using (var cmd = new SqlCommand(conflictGvSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = dto.GiangVienId });
                cmd.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
                cmd.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
                cmd.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                var res = await cmd.ExecuteScalarAsync();
                if (res != null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên đã có lịch dạy trong khung giờ này.");
                }
            }

            // 8. Kiểm tra xung đột lớp học
            const string conflictLopSql = """
                SELECT 1 FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE LopHocId = @LopHocId AND NgayHoc = @NgayHoc AND TrangThai = 1 AND Id <> @Id
                  AND (@GioBatDau < GioKetThuc AND @GioKetThuc > GioBatDau);
                """;
            await using (var cmd = new SqlCommand(conflictLopSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@LopHocId", SqlDbType.Int) { Value = dto.LopHocId });
                cmd.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
                cmd.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
                cmd.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                var res = await cmd.ExecuteScalarAsync();
                if (res != null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã có buổi học khác trong khung giờ này.");
                }
            }

            // 9. Cập nhật bản ghi
            const string updateSql = """
                UPDATE dbo.LichHoc
                SET MonHocId = @MonHocId,
                    GiangVienId = @GiangVienId,
                    PhongHocId = @PhongHocId,
                    NgayHoc = @NgayHoc,
                    GioBatDau = @GioBatDau,
                    GioKetThuc = @GioKetThuc,
                    NoiDung = @NoiDung,
                    TrangThai = @TrangThai
                WHERE Id = @Id AND TrangThai = 1;
                """;
            await using (var cmd = new SqlCommand(updateSql, connection, transaction))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                cmd.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
                cmd.Parameters.Add(new SqlParameter("@GiangVienId", SqlDbType.Int) { Value = dto.GiangVienId });
                cmd.Parameters.Add(new SqlParameter("@PhongHocId", SqlDbType.Int) { Value = dto.PhongHocId });
                cmd.Parameters.Add(new SqlParameter("@NgayHoc", SqlDbType.Date) { Value = dto.NgayHoc.Date });
                cmd.Parameters.Add(new SqlParameter("@GioBatDau", SqlDbType.Time) { Value = dto.GioBatDau });
                cmd.Parameters.Add(new SqlParameter("@GioKetThuc", SqlDbType.Time) { Value = dto.GioKetThuc });
                cmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@NoiDung", SqlDbType.NVarChar, dto.NoiDung, 500));
                cmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });
                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể cập nhật buổi học vì buổi học đã bị hủy hoặc không tồn tại.");
                }
            }

            await transaction.CommitAsync();
            return (true, null, null);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, ErrorCodes.UNEXPECTED_ERROR, "Không thể cập nhật lịch học do lỗi hệ thống.");
        }
    }

    private static LichHocDto MapFromReader(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        LopHocId = reader.GetInt32(reader.GetOrdinal("LopHocId")),
        MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
        TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
        MonHocId = reader.GetInt32(reader.GetOrdinal("MonHocId")),
        MaMonHoc = reader.GetString(reader.GetOrdinal("MaMonHoc")),
        TenMonHoc = reader.GetString(reader.GetOrdinal("TenMonHoc")),
        GiangVienId = reader.GetInt32(reader.GetOrdinal("GiangVienId")),
        MaGiangVien = reader.GetString(reader.GetOrdinal("MaGiangVien")),
        HoTenGiangVien = reader.GetString(reader.GetOrdinal("HoTenGiangVien")),
        PhongHocId = reader.GetInt32(reader.GetOrdinal("PhongHocId")),
        MaPhong = reader.GetString(reader.GetOrdinal("MaPhong")),
        TenPhong = reader.GetString(reader.GetOrdinal("TenPhong")),
        NgayHoc = reader.GetDateTime(reader.GetOrdinal("NgayHoc")),
        GioBatDau = reader.GetTimeSpan(reader.GetOrdinal("GioBatDau")),
        GioKetThuc = reader.GetTimeSpan(reader.GetOrdinal("GioKetThuc")),
        NoiDung = reader.IsDBNull(reader.GetOrdinal("NoiDung")) ? null : reader.GetString(reader.GetOrdinal("NoiDung")),
        TrangThai = (TrangThaiHoatDong)reader.GetInt32(reader.GetOrdinal("TrangThai"))
    };
}
