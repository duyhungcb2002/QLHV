using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlKetQuaHocTapRepository : IKetQuaHocTapRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlKetQuaHocTapRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<KetQuaHocTapDto>> GetAllAsync(KetQuaHocTapFilter? filter = null)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        var sqlBuilder = new StringBuilder("""
            SELECT kq.Id, kq.DangKyLopHocId,
                   dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien,
                   kq.MonHocId, mh.MaMonHoc, mh.TenMonHoc,
                   kq.DiemTongKet, kq.NguoiCapNhatTaiKhoanId, tk.TenDangNhap AS NguoiCapNhatTen,
                   kq.NgayCapNhat, kq.GhiChu
            FROM dbo.KetQuaHocTap kq
            INNER JOIN dbo.DangKyLopHoc dk ON kq.DangKyLopHocId = dk.Id
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            INNER JOIN dbo.MonHoc mh ON kq.MonHocId = mh.Id
            LEFT JOIN dbo.TaiKhoan tk ON kq.NguoiCapNhatTaiKhoanId = tk.Id
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

            if (filter.MonHocId.HasValue)
            {
                sqlBuilder.Append(" AND kq.MonHocId = @MonHocId");
                command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = filter.MonHocId.Value });
            }

            if (filter.HocVienId.HasValue)
            {
                sqlBuilder.Append(" AND dk.HocVienId = @HocVienId");
                command.Parameters.Add(new SqlParameter("@HocVienId", SqlDbType.Int) { Value = filter.HocVienId.Value });
            }
        }

        sqlBuilder.Append(" ORDER BY hv.MaHocVien;");
        command.CommandText = sqlBuilder.ToString();

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<KetQuaHocTapDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapFromReader(reader));
        }

        return list;
    }

    public async Task<KetQuaHocTapDto?> GetByIdAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT kq.Id, kq.DangKyLopHocId,
                   dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien,
                   kq.MonHocId, mh.MaMonHoc, mh.TenMonHoc,
                   kq.DiemTongKet, kq.NguoiCapNhatTaiKhoanId, tk.TenDangNhap AS NguoiCapNhatTen,
                   kq.NgayCapNhat, kq.GhiChu
            FROM dbo.KetQuaHocTap kq
            INNER JOIN dbo.DangKyLopHoc dk ON kq.DangKyLopHocId = dk.Id
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            INNER JOIN dbo.MonHoc mh ON kq.MonHocId = mh.Id
            LEFT JOIN dbo.TaiKhoan tk ON kq.NguoiCapNhatTaiKhoanId = tk.Id
            WHERE kq.Id = @Id;
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

    public async Task<KetQuaHocTapDto?> GetByDangKyAndMonAsync(int dangKyLopHocId, int monHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            SELECT kq.Id, kq.DangKyLopHocId,
                   dk.LopHocId, l.MaLopHoc, l.TenLopHoc,
                   l.KhoaHocId, kh.TenKhoaHoc,
                   dk.HocVienId, hv.MaHocVien, hv.HoTen AS HoTenHocVien,
                   kq.MonHocId, mh.MaMonHoc, mh.TenMonHoc,
                   kq.DiemTongKet, kq.NguoiCapNhatTaiKhoanId, tk.TenDangNhap AS NguoiCapNhatTen,
                   kq.NgayCapNhat, kq.GhiChu
            FROM dbo.KetQuaHocTap kq
            INNER JOIN dbo.DangKyLopHoc dk ON kq.DangKyLopHocId = dk.Id
            INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
            INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
            INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
            INNER JOIN dbo.MonHoc mh ON kq.MonHocId = mh.Id
            LEFT JOIN dbo.TaiKhoan tk ON kq.NguoiCapNhatTaiKhoanId = tk.Id
            WHERE kq.DangKyLopHocId = @DangKyLopHocId AND kq.MonHocId = @MonHocId;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dangKyLopHocId });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = monHocId });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapFromReader(reader);
        }

        return null;
    }

    public async Task<KetQuaHocTapDto> UpsertAsync(KetQuaHocTapDto dto)
    {
        if (!dto.DiemTongKet.HasValue) return dto;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);

        const string sql = """
            MERGE dbo.KetQuaHocTap AS target
            USING (SELECT @DangKyLopHocId AS DangKyLopHocId, @MonHocId AS MonHocId) AS source
            ON (target.DangKyLopHocId = source.DangKyLopHocId AND target.MonHocId = source.MonHocId)
            WHEN MATCHED THEN
                UPDATE SET DiemTongKet = @DiemTongKet,
                           NguoiCapNhatTaiKhoanId = @NguoiCapNhatTaiKhoanId,
                           NgayCapNhat = SYSDATETIME(),
                           GhiChu = @GhiChu
            WHEN NOT MATCHED THEN
                INSERT (DangKyLopHocId, MonHocId, DiemTongKet, NguoiCapNhatTaiKhoanId, NgayCapNhat, GhiChu)
                VALUES (@DangKyLopHocId, @MonHocId, @DiemTongKet, @NguoiCapNhatTaiKhoanId, SYSDATETIME(), @GhiChu)
            OUTPUT INSERTED.Id, INSERTED.NgayCapNhat;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dto.DangKyLopHocId });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
        command.Parameters.Add(new SqlParameter("@DiemTongKet", SqlDbType.Decimal) { Precision = 4, Scale = 2, Value = dto.DiemTongKet.Value });
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@NguoiCapNhatTaiKhoanId", SqlDbType.Int, dto.NguoiCapNhatTaiKhoanId));
        command.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, dto.GhiChu, 250));

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            dto.Id = reader.GetInt32(0);
            dto.NgayCapNhat = reader.GetDateTime(1);
        }

        if (dto.KhoaHocId == 0 || string.IsNullOrWhiteSpace(dto.TenKhoaHoc))
        {
            await reader.CloseAsync();
            const string lookupSql = """
                SELECT l.KhoaHocId, kh.TenKhoaHoc
                FROM dbo.DangKyLopHoc dk
                INNER JOIN dbo.LopHoc l ON dk.LopHocId = l.Id
                INNER JOIN dbo.KhoaHoc kh ON l.KhoaHocId = kh.Id
                WHERE dk.Id = @DangKyId;
                """;
            await using var lkCmd = new SqlCommand(lookupSql, connection);
            lkCmd.Parameters.Add(new SqlParameter("@DangKyId", SqlDbType.Int) { Value = dto.DangKyLopHocId });
            await using var lkReader = await lkCmd.ExecuteReaderAsync();
            if (await lkReader.ReadAsync())
            {
                dto.KhoaHocId = lkReader.GetInt32(0);
                dto.TenKhoaHoc = lkReader.GetString(1);
            }
        }

        return dto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = "DELETE FROM dbo.KetQuaHocTap WHERE Id = @Id;";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteByDangKyAndMonAsync(int dangKyLopHocId, int monHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = "DELETE FROM dbo.KetQuaHocTap WHERE DangKyLopHocId = @DangKyLopHocId AND MonHocId = @MonHocId;";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dangKyLopHocId });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = monHocId });
        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<int> SaveGradesBatchAtomicAsync(IEnumerable<KetQuaHocTapDto> toUpsert, IEnumerable<int> dangKyIdsToDelete, int monHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            int affected = 0;

            var delList = dangKyIdsToDelete.ToList();
            if (delList.Count > 0)
            {
                const string delSql = "DELETE FROM dbo.KetQuaHocTap WHERE MonHocId = @MonHocId AND DangKyLopHocId = @DangKyLopHocId;";
                foreach (var dkId in delList)
                {
                    await using var delCmd = new SqlCommand(delSql, connection, transaction);
                    delCmd.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = monHocId });
                    delCmd.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = dkId });
                    affected += await delCmd.ExecuteNonQueryAsync();
                }
            }

            var upsertList = toUpsert.Where(x => x.DiemTongKet.HasValue).ToList();
            if (upsertList.Count > 0)
            {
                const string mergeSql = """
                    MERGE dbo.KetQuaHocTap AS target
                    USING (SELECT @DangKyLopHocId AS DangKyLopHocId, @MonHocId AS MonHocId) AS source
                    ON (target.DangKyLopHocId = source.DangKyLopHocId AND target.MonHocId = source.MonHocId)
                    WHEN MATCHED THEN
                        UPDATE SET DiemTongKet = @DiemTongKet,
                                   NguoiCapNhatTaiKhoanId = @NguoiCapNhatTaiKhoanId,
                                   NgayCapNhat = SYSDATETIME(),
                                   GhiChu = @GhiChu
                    WHEN NOT MATCHED THEN
                        INSERT (DangKyLopHocId, MonHocId, DiemTongKet, NguoiCapNhatTaiKhoanId, NgayCapNhat, GhiChu)
                        VALUES (@DangKyLopHocId, @MonHocId, @DiemTongKet, @NguoiCapNhatTaiKhoanId, SYSDATETIME(), @GhiChu);
                    """;

                foreach (var item in upsertList)
                {
                    await using var cmd = new SqlCommand(mergeSql, connection, transaction);
                    cmd.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = item.DangKyLopHocId });
                    cmd.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = item.MonHocId });
                    cmd.Parameters.Add(new SqlParameter("@DiemTongKet", SqlDbType.Decimal) { Precision = 4, Scale = 2, Value = item.DiemTongKet!.Value });
                    cmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@NguoiCapNhatTaiKhoanId", SqlDbType.Int, item.NguoiCapNhatTaiKhoanId));
                    cmd.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, item.GhiChu, 250));

                    await cmd.ExecuteNonQueryAsync();
                    affected++;
                }
            }

            await transaction.CommitAsync();
            return affected;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> UpsertBatchAsync(IEnumerable<KetQuaHocTapDto> list)
    {
        var validList = list.Where(x => x.DiemTongKet.HasValue).ToList();
        if (validList.Count == 0) return 0;

        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            const string sql = """
                MERGE dbo.KetQuaHocTap AS target
                USING (SELECT @DangKyLopHocId AS DangKyLopHocId, @MonHocId AS MonHocId) AS source
                ON (target.DangKyLopHocId = source.DangKyLopHocId AND target.MonHocId = source.MonHocId)
                WHEN MATCHED THEN
                    UPDATE SET DiemTongKet = @DiemTongKet,
                               NguoiCapNhatTaiKhoanId = @NguoiCapNhatTaiKhoanId,
                               NgayCapNhat = SYSDATETIME(),
                               GhiChu = @GhiChu
                WHEN NOT MATCHED THEN
                    INSERT (DangKyLopHocId, MonHocId, DiemTongKet, NguoiCapNhatTaiKhoanId, NgayCapNhat, GhiChu)
                    VALUES (@DangKyLopHocId, @MonHocId, @DiemTongKet, @NguoiCapNhatTaiKhoanId, SYSDATETIME(), @GhiChu);
                """;

            int count = 0;
            foreach (var item in validList)
            {
                await using var command = new SqlCommand(sql, connection, transaction);
                command.Parameters.Add(new SqlParameter("@DangKyLopHocId", SqlDbType.Int) { Value = item.DangKyLopHocId });
                command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = item.MonHocId });
                command.Parameters.Add(new SqlParameter("@DiemTongKet", SqlDbType.Decimal) { Precision = 4, Scale = 2, Value = item.DiemTongKet!.Value });
                command.Parameters.Add(SqlRepositoryHelper.CreateParam("@NguoiCapNhatTaiKhoanId", SqlDbType.Int, item.NguoiCapNhatTaiKhoanId));
                command.Parameters.Add(SqlRepositoryHelper.CreateParam("@GhiChu", SqlDbType.NVarChar, item.GhiChu, 250));

                await command.ExecuteNonQueryAsync();
                count++;
            }

            await transaction.CommitAsync();
            return count;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static KetQuaHocTapDto MapFromReader(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        DangKyLopHocId = reader.GetInt32(reader.GetOrdinal("DangKyLopHocId")),
        LopHocId = reader.GetInt32(reader.GetOrdinal("LopHocId")),
        MaLopHoc = reader.GetString(reader.GetOrdinal("MaLopHoc")),
        TenLopHoc = reader.GetString(reader.GetOrdinal("TenLopHoc")),
        KhoaHocId = reader.GetInt32(reader.GetOrdinal("KhoaHocId")),
        TenKhoaHoc = reader.GetString(reader.GetOrdinal("TenKhoaHoc")),
        HocVienId = reader.GetInt32(reader.GetOrdinal("HocVienId")),
        MaHocVien = reader.GetString(reader.GetOrdinal("MaHocVien")),
        HoTenHocVien = reader.GetString(reader.GetOrdinal("HoTenHocVien")),
        MonHocId = reader.GetInt32(reader.GetOrdinal("MonHocId")),
        MaMonHoc = reader.GetString(reader.GetOrdinal("MaMonHoc")),
        TenMonHoc = reader.GetString(reader.GetOrdinal("TenMonHoc")),
        DiemTongKet = reader.GetDecimal(reader.GetOrdinal("DiemTongKet")),
        NguoiCapNhatTaiKhoanId = reader.IsDBNull(reader.GetOrdinal("NguoiCapNhatTaiKhoanId")) ? null : reader.GetInt32(reader.GetOrdinal("NguoiCapNhatTaiKhoanId")),
        NguoiCapNhatTen = reader.IsDBNull(reader.GetOrdinal("NguoiCapNhatTen")) ? null : reader.GetString(reader.GetOrdinal("NguoiCapNhatTen")),
        NgayCapNhat = reader.GetDateTime(reader.GetOrdinal("NgayCapNhat")),
        GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? null : reader.GetString(reader.GetOrdinal("GhiChu"))
    };
}
