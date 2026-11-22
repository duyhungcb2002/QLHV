using Microsoft.Data.SqlClient;
using QLHV.Core.Logging;

namespace QLHV.WinForms.Configuration;

public sealed class ConnectionTestResult
{
    public bool IsSuccess { get; init; }
    public string SanitizedInfo { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }
    public string? UserFriendlyMessage { get; init; }

    public static ConnectionTestResult Success(string sanitizedInfo) => new()
    {
        IsSuccess = true,
        SanitizedInfo = sanitizedInfo
    };

    public static ConnectionTestResult Failure(string sanitizedInfo, string errorMessage, string? userFriendlyMessage = null) => new()
    {
        IsSuccess = false,
        SanitizedInfo = sanitizedInfo,
        ErrorMessage = errorMessage,
        UserFriendlyMessage = userFriendlyMessage ?? errorMessage
    };
}

/// <summary>
/// Kiểm tra kết nối SQL Server trước khi mở màn hình đăng nhập
/// Đảm bảo không kết nối nhầm database và kiểm tra toàn bộ bảng schema cần thiết
/// </summary>
public static class StartupConnectionTester
{
    private static readonly string[] RequiredTables = new[]
    {
        "dbo.TaiKhoan", "dbo.GiangVien", "dbo.HocVien", "dbo.MonHoc", "dbo.KhoaHoc", "dbo.KhoaHocMonHoc", "dbo.PhongHoc",
        "dbo.LopHoc", "dbo.DangKyLopHoc", "dbo.LichHoc", "dbo.KetQuaHocTap", "dbo.PhieuThu"
    };

    public static ConnectionTestResult TestConnection(string? connectionString)
    {
        var sanitized = AppConfigLoader.SanitizeConnectionString(connectionString);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return ConnectionTestResult.Failure(sanitized, "Chuỗi kết nối SQL Server rỗng.");
        }

        try
        {
            var builder = new SqlConnectionStringBuilder(connectionString);

            // Kiểm tra bảo vệ không được trỏ vào QLHV_Test
            if (string.Equals(builder.InitialCatalog, AppConfigLoader.TestDatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                return ConnectionTestResult.Failure(sanitized,
                    $"Không được phép chạy ứng dụng với database kiểm thử '{AppConfigLoader.TestDatabaseName}'.");
            }

            if (!string.Equals(builder.InitialCatalog, AppConfigLoader.ExpectedDatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                return ConnectionTestResult.Failure(sanitized,
                    $"Database phải là '{AppConfigLoader.ExpectedDatabaseName}', không phải '{builder.InitialCatalog}'.");
            }

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // Kiểm tra toàn bộ schema bắt buộc
            foreach (var table in RequiredTables)
            {
                using var command = new SqlCommand($"SELECT TOP (1) 1 FROM {table};", connection);
                command.CommandTimeout = 5;
                command.ExecuteScalar();
            }

            // Kiểm tra cột audit của 005_operations_hardening.sql trên PhieuThu
            using (var auditCmd = new SqlCommand("SELECT TOP (1) NgayHuy, NguoiHuyTaiKhoanId FROM dbo.PhieuThu;", connection))
            {
                auditCmd.CommandTimeout = 5;
                auditCmd.ExecuteScalar();
            }

            // Kiểm tra khóa ngoại FK_PhieuThu_NguoiHuy
            using (var fkCmd = new SqlCommand("SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_PhieuThu_NguoiHuy' AND parent_object_id = OBJECT_ID('dbo.PhieuThu');", connection))
            {
                fkCmd.CommandTimeout = 5;
                var fkExists = fkCmd.ExecuteScalar();
                if (fkExists == null)
                {
                    return ConnectionTestResult.Failure(sanitized,
                        "Thiếu khóa ngoại FK_PhieuThu_NguoiHuy trong cơ sở dữ liệu. Vui lòng chạy script database/005_operations_hardening.sql.");
                }
            }

            // Kiểm tra bắt buộc migration 006 (Hồ sơ học viên & Giảng viên chủ nhiệm lớp học)
            var migration006Result = CheckMigration006(connection, sanitized);
            if (!migration006Result.IsSuccess)
            {
                return migration006Result;
            }

            return ConnectionTestResult.Success(sanitized);
        }
        catch (SqlException ex)
        {
            var safeExMsg = FileAppLogger.Sanitize(ex.Message);
            if (ex.Number == 208 || ex.Number == 207) // Invalid object name or Invalid column name
            {
                if (safeExMsg.Contains("GiangVienChuNhiemId", StringComparison.OrdinalIgnoreCase) ||
                    safeExMsg.Contains("SoDinhDanh", StringComparison.OrdinalIgnoreCase) ||
                    safeExMsg.Contains("NgayCapGiayTo", StringComparison.OrdinalIgnoreCase) ||
                    safeExMsg.Contains("NoiCapGiayTo", StringComparison.OrdinalIgnoreCase) ||
                    safeExMsg.Contains("NhanVienPhuTrach", StringComparison.OrdinalIgnoreCase))
                {
                    return ConnectionTestResult.Failure(sanitized,
                        $"Thiếu cột của migration 006 trong cơ sở dữ liệu ({ex.Number}): {safeExMsg}. Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.",
                        "Cơ sở dữ liệu chưa được cập nhật migration 006. Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.");
                }

                return ConnectionTestResult.Failure(sanitized,
                    $"Thiếu bảng hoặc cột trong cơ sở dữ liệu ({ex.Number}): {safeExMsg}",
                    "Cơ sở dữ liệu chưa được cập nhật đầy đủ bảng/cột cần thiết. Vui lòng chạy các script migration trong thư mục database/.");
            }
            return ConnectionTestResult.Failure(sanitized,
                $"Lỗi SQL Server ({ex.Number}): {safeExMsg}",
                "Không thể kết nối đến máy chủ SQL Server. Vui lòng kiểm tra dịch vụ SQL Server và chuỗi kết nối.");
        }
        catch (Exception ex)
        {
            var safeExMsg = FileAppLogger.Sanitize(ex.Message);
            return ConnectionTestResult.Failure(sanitized,
                $"Lỗi kết nối: {safeExMsg}",
                "Đã xảy ra lỗi khi kiểm tra kết nối cơ sở dữ liệu. Vui lòng kiểm tra lại cấu hình hệ thống.");
        }
    }

    /// <summary>
    /// Kiểm tra toàn diện các thành phần của migration 006 trên kết nối thật
    /// </summary>
    public static ConnectionTestResult CheckMigration006(SqlConnection connection, string sanitizedInfo)
    {
        // 1. Kiểm tra 11 cột hồ sơ mở rộng của dbo.HocVien
        const string checkHocVienColsSql = """
            SELECT TOP (1)
                SoDinhDanh, NgayCapGiayTo, NoiCapGiayTo,
                HoTenNguoiGiamHo, DienThoaiNguoiGiamHo, QuanHeNguoiGiamHo,
                NhanVienPhuTrachTaiKhoanId, NguonTuyenSinh, NgheNghiep,
                DonViCongTac, GhiChuDacBiet
            FROM dbo.HocVien;
            """;
        using (var cmd = new SqlCommand(checkHocVienColsSql, connection))
        {
            cmd.CommandTimeout = 5;
            cmd.ExecuteScalar();
        }

        // 2. Kiểm tra cột GiangVienChuNhiemId trong dbo.LopHoc
        const string checkLopHocColSql = "SELECT TOP (1) GiangVienChuNhiemId FROM dbo.LopHoc;";
        using (var cmd = new SqlCommand(checkLopHocColSql, connection))
        {
            cmd.CommandTimeout = 5;
            cmd.ExecuteScalar();
        }

        // 3. Kiểm tra Unique Filtered Index trên HocVien(SoDinhDanh) (unique, filtered, đúng cột SoDinhDanh là key column)
        const string checkIndexSql = """
            SELECT 1
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            WHERE i.name = 'UX_HocVien_SoDinhDanh'
              AND i.object_id = OBJECT_ID('dbo.HocVien')
              AND i.is_unique = 1
              AND i.has_filter = 1
              AND c.name = 'SoDinhDanh'
              AND ic.key_ordinal > 0
              AND ic.is_included_column = 0
              AND i.filter_definition LIKE '%SoDinhDanh%IS NOT NULL%';
            """;
        using (var cmd = new SqlCommand(checkIndexSql, connection))
        {
            cmd.CommandTimeout = 5;
            if (cmd.ExecuteScalar() == null)
            {
                return ConnectionTestResult.Failure(sanitizedInfo,
                    "Chỉ mục UX_HocVien_SoDinhDanh không hợp lệ hoặc thiếu (phải là unique, filtered trên cột SoDinhDanh). Vui lòng kiểm tra script database/006_student_profile_and_class_advisor.sql.",
                    "Cơ sở dữ liệu chưa được cập nhật migration 006 (chỉ mục UX_HocVien_SoDinhDanh không đạt chuẩn unique filtered trên SoDinhDanh). Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.");
            }
        }

        // 4. Kiểm tra Foreign Key FK_HocVien_NhanVienPhuTrach (đúng bảng/cột nguồn-đích, không disabled, không untrusted)
        const string checkFkStaffSql = """
            SELECT 1
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.columns pc ON fkc.parent_object_id = pc.object_id AND fkc.parent_column_id = pc.column_id
            INNER JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
            WHERE fk.name = 'FK_HocVien_NhanVienPhuTrach'
              AND fk.parent_object_id = OBJECT_ID('dbo.HocVien')
              AND pc.name = 'NhanVienPhuTrachTaiKhoanId'
              AND fk.referenced_object_id = OBJECT_ID('dbo.TaiKhoan')
              AND rc.name = 'Id'
              AND fk.is_disabled = 0
              AND fk.is_not_trusted = 0;
            """;
        using (var cmd = new SqlCommand(checkFkStaffSql, connection))
        {
            cmd.CommandTimeout = 5;
            if (cmd.ExecuteScalar() == null)
            {
                return ConnectionTestResult.Failure(sanitizedInfo,
                    "Khóa ngoại FK_HocVien_NhanVienPhuTrach không hợp lệ (phải liên kết HocVien.NhanVienPhuTrachTaiKhoanId -> TaiKhoan.Id, enabled và trusted). Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.",
                    "Cơ sở dữ liệu chưa được cập nhật migration 006 (FK_HocVien_NhanVienPhuTrach không hợp lệ hoặc bị vô hiệu hóa/untrusted). Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.");
            }
        }

        // 5. Kiểm tra Foreign Key FK_LopHoc_GiangVienChuNhiem (đúng bảng/cột nguồn-đích, không disabled, không untrusted)
        const string checkFkAdvisorSql = """
            SELECT 1
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.columns pc ON fkc.parent_object_id = pc.object_id AND fkc.parent_column_id = pc.column_id
            INNER JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
            WHERE fk.name = 'FK_LopHoc_GiangVienChuNhiem'
              AND fk.parent_object_id = OBJECT_ID('dbo.LopHoc')
              AND pc.name = 'GiangVienChuNhiemId'
              AND fk.referenced_object_id = OBJECT_ID('dbo.GiangVien')
              AND rc.name = 'Id'
              AND fk.is_disabled = 0
              AND fk.is_not_trusted = 0;
            """;
        using (var cmd = new SqlCommand(checkFkAdvisorSql, connection))
        {
            cmd.CommandTimeout = 5;
            if (cmd.ExecuteScalar() == null)
            {
                return ConnectionTestResult.Failure(sanitizedInfo,
                    "Khóa ngoại FK_LopHoc_GiangVienChuNhiem không hợp lệ (phải liên kết LopHoc.GiangVienChuNhiemId -> GiangVien.Id, enabled và trusted). Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.",
                    "Cơ sở dữ liệu chưa được cập nhật migration 006 (FK_LopHoc_GiangVienChuNhiem không hợp lệ hoặc bị vô hiệu hóa/untrusted). Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.");
            }
        }

        return ConnectionTestResult.Success(sanitizedInfo);
    }

    /// <summary>
    /// Helper kiểm tra logic phân định migration 006 phục vụ Unit Testing
    /// </summary>
    public static ConnectionTestResult ValidateMigration006Schema(
        bool hasHocVienColumns,
        bool hasGiangVienChuNhiemColumn,
        bool hasFilteredIndex,
        bool hasFkNhanVienPhuTrach,
        bool hasFkGiangVienChuNhiem,
        string sanitizedInfo)
    {
        if (!hasHocVienColumns || !hasGiangVienChuNhiemColumn || !hasFilteredIndex ||
            !hasFkNhanVienPhuTrach || !hasFkGiangVienChuNhiem)
        {
            return ConnectionTestResult.Failure(sanitizedInfo,
                "Cơ sở dữ liệu thiếu cấu trúc migration 006 (Hồ sơ học viên & Giảng viên chủ nhiệm). Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.",
                "Cơ sở dữ liệu chưa được cập nhật migration 006. Vui lòng chạy script database/006_student_profile_and_class_advisor.sql.");
        }
        return ConnectionTestResult.Success(sanitizedInfo);
    }
}
