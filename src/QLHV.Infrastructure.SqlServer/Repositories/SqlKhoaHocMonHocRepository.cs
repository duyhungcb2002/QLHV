using System.Data;
using Microsoft.Data.SqlClient;
using QLHV.Contracts.Dtos;
using QLHV.Core.Database;
using QLHV.Core.Repositories;

namespace QLHV.Infrastructure.SqlServer.Repositories;

public class SqlKhoaHocMonHocRepository : IKhoaHocMonHocRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public SqlKhoaHocMonHocRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<IReadOnlyList<KhoaHocMonHocDto>> GetByKhoaHocIdAsync(int khoaHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT khm.KhoaHocId, kh.MaKhoaHoc, kh.TenKhoaHoc,
                   khm.MonHocId, mh.MaMonHoc, mh.TenMonHoc,
                   khm.ThuTu, khm.SoBuoiDuKien
            FROM dbo.KhoaHocMonHoc khm
            INNER JOIN dbo.KhoaHoc kh ON khm.KhoaHocId = kh.Id
            INNER JOIN dbo.MonHoc mh ON khm.MonHocId = mh.Id
            WHERE khm.KhoaHocId = @KhoaHocId
            ORDER BY khm.ThuTu, khm.MonHocId;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = khoaHocId });

        await using var reader = await command.ExecuteReaderAsync();
        var list = new List<KhoaHocMonHocDto>();
        while (await reader.ReadAsync())
        {
            list.Add(MapKhoaHocMonHoc(reader));
        }

        return list;
    }

    public async Task<KhoaHocMonHocDto?> GetAsync(int khoaHocId, int monHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            SELECT khm.KhoaHocId, kh.MaKhoaHoc, kh.TenKhoaHoc,
                   khm.MonHocId, mh.MaMonHoc, mh.TenMonHoc,
                   khm.ThuTu, khm.SoBuoiDuKien
            FROM dbo.KhoaHocMonHoc khm
            INNER JOIN dbo.KhoaHoc kh ON khm.KhoaHocId = kh.Id
            INNER JOIN dbo.MonHoc mh ON khm.MonHocId = mh.Id
            WHERE khm.KhoaHocId = @KhoaHocId AND khm.MonHocId = @MonHocId;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = khoaHocId });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = monHocId });

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapKhoaHocMonHoc(reader);
        }

        return null;
    }

    public async Task<KhoaHocMonHocDto> AddAsync(KhoaHocMonHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            INSERT INTO dbo.KhoaHocMonHoc (KhoaHocId, MonHocId, ThuTu, SoBuoiDuKien)
            VALUES (@KhoaHocId, @MonHocId, @ThuTu, @SoBuoiDuKien);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = dto.KhoaHocId });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });
        command.Parameters.Add(new SqlParameter("@ThuTu", SqlDbType.Int) { Value = dto.ThuTu });
        command.Parameters.Add(new SqlParameter("@SoBuoiDuKien", SqlDbType.Int) { Value = dto.SoBuoiDuKien });

        await command.ExecuteNonQueryAsync();

        // Return dto with fresh joined data if needed
        var created = await GetAsync(dto.KhoaHocId, dto.MonHocId);
        return created ?? dto;
    }

    public async Task<bool> UpdateAsync(KhoaHocMonHocDto dto)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            UPDATE dbo.KhoaHocMonHoc
            SET ThuTu = @ThuTu,
                SoBuoiDuKien = @SoBuoiDuKien
            WHERE KhoaHocId = @KhoaHocId AND MonHocId = @MonHocId;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@ThuTu", SqlDbType.Int) { Value = dto.ThuTu });
        command.Parameters.Add(new SqlParameter("@SoBuoiDuKien", SqlDbType.Int) { Value = dto.SoBuoiDuKien });
        command.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = dto.KhoaHocId });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = dto.MonHocId });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int khoaHocId, int monHocId)
    {
        await using var connection = await SqlRepositoryHelper.OpenConnectionAsync(_connectionFactory);
        const string sql = """
            DELETE FROM dbo.KhoaHocMonHoc
            WHERE KhoaHocId = @KhoaHocId AND MonHocId = @MonHocId;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@KhoaHocId", SqlDbType.Int) { Value = khoaHocId });
        command.Parameters.Add(new SqlParameter("@MonHocId", SqlDbType.Int) { Value = monHocId });

        var affected = await command.ExecuteNonQueryAsync();
        return affected > 0;
    }

    private static KhoaHocMonHocDto MapKhoaHocMonHoc(SqlDataReader reader)
    {
        return new KhoaHocMonHocDto
        {
            KhoaHocId = reader.GetInt32(0),
            MaKhoaHoc = reader.GetString(1),
            TenKhoaHoc = reader.GetString(2),
            MonHocId = reader.GetInt32(3),
            MaMonHoc = reader.GetString(4),
            TenMonHoc = reader.GetString(5),
            ThuTu = reader.GetInt32(6),
            SoBuoiDuKien = reader.GetInt32(7)
        };
    }
}

