using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlLopHocRepository : ILopHocRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlLopHocRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<LopHocDto>> GetAllAsync(LopHocFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT l.Id, l.MaLopHoc, l.TenLopHoc, l.KhoaHocId, k.TenKhoaHoc,
                   l.NgayBatDau, l.NgayKetThuc, l.SiSoToiDa, l.GhiChu, l.TrangThai, l.NgayTao,
                   l.GiangVienChuNhiemId, gvcn.HoTen AS HoTenGiangVienChuNhiem,
                   (SELECT COUNT(*) FROM dbo.DangKyLopHoc dk WHERE dk.LopHocId = l.Id AND dk.TrangThai = 1) AS SiSoHienTai
            FROM dbo.LopHoc l
            INNER JOIN dbo.KhoaHoc k ON l.KhoaHocId = k.Id
            LEFT JOIN dbo.GiangVien gvcn ON l.GiangVienChuNhiemId = gvcn.Id
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
                     AND (l.MaLopHoc LIKE @TuKhoa ESCAPE N'~'
                       OR l.TenLopHoc LIKE @TuKhoa ESCAPE N'~'
                       OR l.TenLopHoc COLLATE SQL_Latin1_General_CP1_CI_AI LIKE @TuKhoa COLLATE SQL_Latin1_General_CP1_CI_AI ESCAPE N'~'
                       OR k.TenKhoaHoc LIKE @TuKhoa ESCAPE N'~')
                    """);
                command.Parameters.Add(new SqlParameter("@TuKhoa", SqlDbType.NVarChar) { Value = $"%{escaped}%" });
            }

            if (filter.KhoaHocId.HasValue)
            {
                sqlBuilder.Append(" AND l.KhoaHocId = @KhoaHocId");
                command.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = filter.KhoaHocId.Value });
            }

            if (filter.TrangThai.HasValue)
            {
                sqlBuilder.Append(" AND l.TrangThai = @TrangThai");
                command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)filter.TrangThai.Value });
            }
        }

        sqlBuilder.Append(" ORDER BY l.Id DESC;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<LopHocDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapFromReader(reader));
        }

        return list;
    }

    public async Task<LopHocDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT l.Id, l.MaLopHoc, l.TenLopHoc, l.KhoaHocId, k.TenKhoaHoc,
                   l.NgayBatDau, l.NgayKetThuc, l.SiSoToiDa, l.GhiChu, l.TrangThai, l.NgayTao,
                   l.GiangVienChuNhiemId, gvcn.HoTen AS HoTenGiangVienChuNhiem,
                   (SELECT COUNT(*) FROM dbo.DangKyLopHoc dk WHERE dk.LopHocId = l.Id AND dk.TrangThai = 1) AS SiSoHienTai
            FROM dbo.LopHoc l
            INNER JOIN dbo.KhoaHoc k ON l.KhoaHocId = k.Id
            LEFT JOIN dbo.GiangVien gvcn ON l.GiangVienChuNhiemId = gvcn.Id
            WHERE l.Id = @Id;
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

    public async Task<LopHocDto?> GetByMaLopAsync(string maLop)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT l.Id, l.MaLopHoc, l.TenLopHoc, l.KhoaHocId, k.TenKhoaHoc,
                   l.NgayBatDau, l.NgayKetThuc, l.SiSoToiDa, l.GhiChu, l.TrangThai, l.NgayTao,
                   l.GiangVienChuNhiemId, gvcn.HoTen AS HoTenGiangVienChuNhiem,
                   (SELECT COUNT(*) FROM dbo.DangKyLopHoc dk WHERE dk.LopHocId = l.Id AND dk.TrangThai = 1) AS SiSoHienTai
            FROM dbo.LopHoc l
            INNER JOIN dbo.KhoaHoc k ON l.KhoaHocId = k.Id
            LEFT JOIN dbo.GiangVien gvcn ON l.GiangVienChuNhiemId = gvcn.Id
            WHERE l.MaLopHoc = @MaLopHoc;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@MaLopHoc", SqlDbType.NVarChar, 20) { Value = maLop.Trim() });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }

        return null;
    }

    public async Task<LopHocDto> AddAsync(LopHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            INSERT INTO dbo.LopHoc (MaLopHoc, TenLopHoc, KhoaHocId, NgayBatDau, NgayKetThuc, SiSoToiDa, GhiChu, TrangThai, NgayTao, GiangVienChuNhiemId)
            OUTPUT INSERTED.Id, INSERTED.NgayTao
            VALUES (@MaLopHoc, @TenLopHoc, @KhoaHocId, @NgayBatDau, @NgayKetThuc, @SiSoToiDa, @GhiChu, @TrangThai, SYSDATETIME(), @GiangVienChuNhiemId);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@MaLopHoc", SqlDbType.NVarChar, dto.MaLopHoc, 20));
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@TenLopHoc", SqlDbType.NVarChar, dto.TenLopHoc, 150));
        command.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = dto.KhoaHocId });
        command.Parameters.Add(new SqlParameter("@NgayBatDau", SqlDbType.Date) { Value = dto.NgayBatDau.Date });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@NgayKetThuc", SqlDbType.Date, dto.NgayKetThuc?.Date));
        command.Parameters.Add(new SqlParameter("@SiSoToiDa", SqlDbType.Int) { Value = dto.SiSoToiDa });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, dto.GhiChu, 500));
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });
        command.Parameters.Add(new SqlParameter("@GiangVienChuNhiemId", SqlDbType.Int) { Value = (object?)dto.GiangVienChuNhiemId ?? DBNull.Value });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            dto.Id = reader.GetInt32(0);
            dto.NgayTao = reader.GetDateTime(1);
        }

        return dto;
    }

    public async Task<bool> UpdateAsync(LopHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            UPDATE dbo.LopHoc
            SET TenLopHoc = @TenLopHoc,
                KhoaHocId = @KhoaHocId,
                NgayBatDau = @NgayBatDau,
                NgayKetThuc = @NgayKetThuc,
                SiSoToiDa = @SiSoToiDa,
                GhiChu = @GhiChu,
                TrangThai = @TrangThai,
                GiangVienChuNhiemId = @GiangVienChuNhiemId
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@TenLopHoc", SqlDbType.NVarChar, dto.TenLopHoc, 150));
        command.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = dto.KhoaHocId });
        command.Parameters.Add(new SqlParameter("@NgayBatDau", SqlDbType.Date) { Value = dto.NgayBatDau.Date });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@NgayKetThuc", SqlDbType.Date, dto.NgayKetThuc?.Date));
        command.Parameters.Add(new SqlParameter("@SiSoToiDa", SqlDbType.Int) { Value = dto.SiSoToiDa });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, dto.GhiChu, 500));
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });
        command.Parameters.Add(new SqlParameter("@GiangVienChuNhiemId", SqlDbType.Int) { Value = (object?)dto.GiangVienChuNhiemId ?? DBNull.Value });

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateAtomicAsync(LopHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // 0. Khóa bản ghi LopHoc và kiểm tra tồn tại cũng như trạng thái
            const string getExistingSql = """
                SELECT KhoaHocId, TrangThai
                FROM dbo.LopHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE Id = @Id;
                """;

            int? existingKhoaHocId = null;
            int? existingTrangThai = null;
            await using (var getCmd = new SqlCommand(getExistingSql, connection, transaction))
            {
                getCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                await using (var reader = await getCmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        existingKhoaHocId = reader.GetInt32(0);
                        existingTrangThai = reader.GetInt32(1);
                    }
                }
            }

            if (!existingKhoaHocId.HasValue || !existingTrangThai.HasValue)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.NOT_FOUND, $"Không tìm thấy lớp học ID={dto.Id}.");
            }

            // Nếu trạng thái trong DB đang là NgungHoatDong (0), từ chối cập nhật hoặc hồi phục qua UpdateAtomic
            if (existingTrangThai.Value == 0)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động, không thể chỉnh sửa thông tin.");
            }

            // Đảm bảo không thay đổi trạng thái qua update thông thường
            dto.TrangThai = (TrangThaiHoatDong)existingTrangThai.Value;

            // 1. Khóa các đăng ký hiện tại của lớp và đếm số lượng học viên đang học
            const string countSql = """
                SELECT COUNT(*)
                FROM dbo.DangKyLopHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE LopHocId = @Id AND TrangThai = 1;
                """;

            int activeCount = 0;
            await using (var countCmd = new SqlCommand(countSql, connection, transaction))
            {
                countCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                var res = await countCmd.ExecuteScalarAsync();
                activeCount = res != null ? Convert.ToInt32(res) : 0;
            }

            if (dto.SiSoToiDa < activeCount)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.VALIDATION_ERROR,
                    $"Không thể giảm sĩ số tối đa xuống {dto.SiSoToiDa} vì lớp đang có {activeCount} học viên đang học.");
            }

            // 2. Kiểm tra nếu đổi KhoaHocId thì không được có bất kỳ enrollment, schedule hoặc grade nào trong lịch sử
            if (existingKhoaHocId != dto.KhoaHocId)
            {
                const string checkHistorySql = """
                    SELECT
                        (SELECT COUNT(*) FROM dbo.DangKyLopHoc WHERE LopHocId = @Id) +
                        (SELECT COUNT(*) FROM dbo.LichHoc WHERE LopHocId = @Id) +
                        (SELECT COUNT(*) FROM dbo.KetQuaHocTap kq INNER JOIN dbo.DangKyLopHoc dk ON kq.DangKyLopHocId = dk.Id WHERE dk.LopHocId = @Id) AS TotalHistory;
                    """;

                await using (var checkCmd = new SqlCommand(checkHistorySql, connection, transaction))
                {
                    checkCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                    var total = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                    if (total > 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                            "Không thể thay đổi khóa học của lớp khi lớp đã phát sinh danh sách học viên, lịch học hoặc điểm số.");
                    }
                }
            }

            // 3. Kiểm tra ngày học của các lịch học đang hoạt động phải nằm trong [NgayBatDau, NgayKetThuc]
            const string checkScheduleDatesSql = """
                SELECT COUNT(*)
                FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE LopHocId = @Id AND TrangThai = 1
                  AND (NgayHoc < @NgayBatDau OR (@NgayKetThuc IS NOT NULL AND NgayHoc > @NgayKetThuc));
                """;

            await using (var dateCmd = new SqlCommand(checkScheduleDatesSql, connection, transaction))
            {
                dateCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                dateCmd.Parameters.Add(new SqlParameter("@NgayBatDau", SqlDbType.Date) { Value = dto.NgayBatDau.Date });
                dateCmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@NgayKetThuc", SqlDbType.Date, dto.NgayKetThuc?.Date));

                var outOfRange = Convert.ToInt32(await dateCmd.ExecuteScalarAsync());
                if (outOfRange > 0)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                        "Thời gian mới của lớp học xung đột với các lịch học đang hoạt động (có lịch học nằm ngoài khoảng thời gian này).");
                }
            }

            // 4. Kiểm tra sức chứa phòng của tất cả các lịch học đang hoạt động: dto.SiSoToiDa <= phong.SucChua
            const string checkRoomCapacitySql = """
                SELECT TOP 1 p.TenPhong, p.SucChua
                FROM dbo.LichHoc lh WITH (UPDLOCK, HOLDLOCK)
                INNER JOIN dbo.PhongHoc p ON lh.PhongHocId = p.Id
                WHERE lh.LopHocId = @Id AND lh.TrangThai = 1 AND p.SucChua < @SiSoToiDa;
                """;

            string? violatingRoomName = null;
            int violatingRoomCap = 0;
            await using (var capCmd = new SqlCommand(checkRoomCapacitySql, connection, transaction))
            {
                capCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                capCmd.Parameters.Add(new SqlParameter("@SiSoToiDa", SqlDbType.Int) { Value = dto.SiSoToiDa });
                await using (var capReader = await capCmd.ExecuteReaderAsync())
                {
                    if (await capReader.ReadAsync())
                    {
                        violatingRoomName = capReader.GetString(0);
                        violatingRoomCap = capReader.GetInt32(1);
                    }
                }
            }

            if (violatingRoomName != null)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Sĩ số tối đa ({dto.SiSoToiDa}) vượt quá sức chứa ({violatingRoomCap}) của phòng học '{violatingRoomName}' đã xếp cho lớp.");
            }

            const string updateSql = """
                UPDATE dbo.LopHoc
                SET TenLopHoc = @TenLopHoc,
                    KhoaHocId = @KhoaHocId,
                    NgayBatDau = @NgayBatDau,
                    NgayKetThuc = @NgayKetThuc,
                    SiSoToiDa = @SiSoToiDa,
                    GhiChu = @GhiChu,
                    TrangThai = @TrangThai,
                    GiangVienChuNhiemId = @GiangVienChuNhiemId
                WHERE Id = @Id;
                """;

            await using (var updateCmd = new SqlCommand(updateSql, connection, transaction))
            {
                updateCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
                updateCmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@TenLopHoc", SqlDbType.NVarChar, dto.TenLopHoc, 150));
                updateCmd.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = dto.KhoaHocId });
                updateCmd.Parameters.Add(new SqlParameter("@NgayBatDau", SqlDbType.Date) { Value = dto.NgayBatDau.Date });
                updateCmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@NgayKetThuc", SqlDbType.Date, dto.NgayKetThuc?.Date));
                updateCmd.Parameters.Add(new SqlParameter("@SiSoToiDa", SqlDbType.Int) { Value = dto.SiSoToiDa });
                updateCmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, dto.GhiChu, 500));
                updateCmd.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)dto.TrangThai });
                updateCmd.Parameters.Add(new SqlParameter("@GiangVienChuNhiemId", SqlDbType.Int) { Value = (object?)dto.GiangVienChuNhiemId ?? DBNull.Value });

                var rows = await updateCmd.ExecuteNonQueryAsync();
                if (rows == 0)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.NOT_FOUND, $"Không tìm thấy lớp học ID={dto.Id}.");
                }
            }

            await transaction.CommitAsync();
            return (true, null, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, ErrorCodes.UNEXPECTED_ERROR, $"Không thể cập nhật thông tin lớp học do lỗi hệ thống: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage)> DeactivateAtomicAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // 1. Khóa và kiểm tra trạng thái lớp học
            const string getLopSql = "SELECT TrangThai FROM dbo.LopHoc WITH (UPDLOCK, HOLDLOCK) WHERE Id = @Id;";
            int currentStatus;
            await using (var getCmd = new SqlCommand(getLopSql, connection, transaction))
            {
                getCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                var res = await getCmd.ExecuteScalarAsync();
                if (res == null)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.NOT_FOUND, $"Không tìm thấy lớp học ID={id}.");
                }
                currentStatus = Convert.ToInt32(res);
            }

            if (currentStatus == 0)
            {
                await transaction.RollbackAsync();
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ở trạng thái ngừng hoạt động trước đó.");
            }

            // 2. Kiểm tra học viên đang hoạt động (UPDLOCK, HOLDLOCK trên DangKyLopHoc)
            const string checkActiveStudentsSql = """
                SELECT COUNT(*)
                FROM dbo.DangKyLopHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE LopHocId = @Id AND TrangThai = 1;
                """;

            await using (var stuCmd = new SqlCommand(checkActiveStudentsSql, connection, transaction))
            {
                stuCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                int activeStudents = Convert.ToInt32(await stuCmd.ExecuteScalarAsync());
                if (activeStudents > 0)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                        $"Không thể ngừng hoạt động lớp học vì vẫn còn {activeStudents} học viên đang theo học. Vui lòng chuyển hoặc hủy đăng ký trước.");
                }
            }

            // 3. Kiểm tra lịch học đang hoạt động (UPDLOCK, HOLDLOCK trên LichHoc)
            const string checkActiveSchedulesSql = """
                SELECT COUNT(*)
                FROM dbo.LichHoc WITH (UPDLOCK, HOLDLOCK)
                WHERE LopHocId = @Id AND TrangThai = 1;
                """;

            await using (var schCmd = new SqlCommand(checkActiveSchedulesSql, connection, transaction))
            {
                schCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                int activeSchedules = Convert.ToInt32(await schCmd.ExecuteScalarAsync());
                if (activeSchedules > 0)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                        $"Không thể ngừng hoạt động lớp học vì vẫn còn {activeSchedules} buổi học đang hoạt động. Vui lòng hủy các buổi học trước.");
                }
            }

            // 4. Cập nhật trạng thái lớp sang ngừng hoạt động (0)
            const string updateSql = "UPDATE dbo.LopHoc SET TrangThai = 0 WHERE Id = @Id AND TrangThai = 1;";
            await using (var updCmd = new SqlCommand(updateSql, connection, transaction))
            {
                updCmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                int affected = await updCmd.ExecuteNonQueryAsync();
                if (affected == 0)
                {
                    await transaction.RollbackAsync();
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể ngừng hoạt động lớp học do trạng thái lớp đã thay đổi.");
                }
            }

            await transaction.CommitAsync();
            return (true, null, null);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, ErrorCodes.UNEXPECTED_ERROR, "Không thể ngừng hoạt động lớp học do lỗi hệ thống.");
        }
    }

    public async Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = "UPDATE dbo.LopHoc SET TrangThai = @TrangThai WHERE Id = @Id;";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
        command.Parameters.Add(new SqlParameter("@TrangThai", SqlDbType.Int) { Value = (int)trangThai });

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static LopHocDto MapFromReader(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
        TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
        KhoaHocId = reader.GetInt32(reader.GetOrdinal("KhoaHocId")),
        TenKhoaHoc = reader.GetString(reader.GetOrdinal("TenKhoaHoc")),
        NgayBatDau = reader.GetDateTime(reader.GetOrdinal("NgayBatDau")),
        NgayKetThuc = reader.IsDBNull(reader.GetOrdinal("NgayKetThuc")) ? null : reader.GetDateTime(reader.GetOrdinal("NgayKetThuc")),
        SiSoToiDa = reader.GetInt32(reader.GetOrdinal("SiSoToiDa")),
        GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu")),
        TrangThai = (TrangThaiHoatDong)reader.GetInt32(reader.GetOrdinal("TrangThai")),
        NgayTao = reader.GetDateTime(reader.GetOrdinal("NgayTao")),
        SiSoHienTai = reader.GetInt32(reader.GetOrdinal("SiSoHienTai")),
        GiangVienChuNhiemId = reader.IsDBNull(reader.GetOrdinal("GiangVienChuNhiemId")) ? null : reader.GetInt32(reader.GetOrdinal("GiangVienChuNhiemId")),
        HoTenGiangVienChuNhiem = reader.IsDBNull(reader.GetOrdinal("HoTenGiangVienChuNhiem")) ? null : reader.GetString(reader.GetOrdinal("HoTenGiangVienChuNhiem"))
    };
}
