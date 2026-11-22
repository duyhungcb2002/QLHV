/* ============================================================================
   QLHV - Hệ Thống Quản Lý Học Viên
   File: database/005_operations_hardening.sql
   Mục tiêu:
     1. Bổ sung audit cho hủy phiếu thu (NgayHuy, NguoiHuyTaiKhoanId)
     2. Tạo index tối ưu hóa cho atomic concurrency checks (Khóa đăng ký, chống xung đột lịch học, tính công nợ)
     3. An toàn, idempotent, chạy được trên cả QLHV và QLHV_Test trong transaction
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

BEGIN TRY
    -- 1. Bổ sung cột audit hủy phiếu thu vào PhieuThu nếu chưa có
    IF NOT EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(N'dbo.PhieuThu') AND name = N'NgayHuy'
    )
    BEGIN
        ALTER TABLE dbo.PhieuThu ADD NgayHuy DATETIME2(7) NULL;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(N'dbo.PhieuThu') AND name = N'NguoiHuyTaiKhoanId'
    )
    BEGIN
        ALTER TABLE dbo.PhieuThu ADD NguoiHuyTaiKhoanId INT NULL;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = N'FK_PhieuThu_NguoiHuy' AND parent_object_id = OBJECT_ID(N'dbo.PhieuThu')
    )
    BEGIN
        ALTER TABLE dbo.PhieuThu ADD CONSTRAINT FK_PhieuThu_NguoiHuy FOREIGN KEY (NguoiHuyTaiKhoanId) REFERENCES dbo.TaiKhoan(Id);
    END;

    -- 2. Đảm bảo Index phục vụ kiểm tra xung đột lịch học
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LichHoc_Conflict_Phong' AND object_id = OBJECT_ID(N'dbo.LichHoc'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_LichHoc_Conflict_Phong
        ON dbo.LichHoc (PhongHocId, NgayHoc, TrangThai)
        INCLUDE (GioBatDau, GioKetThuc, LopHocId, MonHocId, GiangVienId);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LichHoc_Conflict_GiangVien' AND object_id = OBJECT_ID(N'dbo.LichHoc'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_LichHoc_Conflict_GiangVien
        ON dbo.LichHoc (GiangVienId, NgayHoc, TrangThai)
        INCLUDE (GioBatDau, GioKetThuc, LopHocId, MonHocId, PhongHocId);
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LichHoc_Conflict_LopHoc' AND object_id = OBJECT_ID(N'dbo.LichHoc'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_LichHoc_Conflict_LopHoc
        ON dbo.LichHoc (LopHocId, NgayHoc, TrangThai)
        INCLUDE (GioBatDau, GioKetThuc, MonHocId, GiangVienId, PhongHocId);
    END;

    -- 3. Đảm bảo Index phục vụ tính công nợ và khóa phiếu thu theo đăng ký
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PhieuThu_DangKy_TrangThai' AND object_id = OBJECT_ID(N'dbo.PhieuThu'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_PhieuThu_DangKy_TrangThai
        ON dbo.PhieuThu (DangKyLopHocId, TrangThai)
        INCLUDE (SoTien, NgayThu, MaPhieuThu);
    END;

    COMMIT TRANSACTION;
    PRINT N'Migration 005_operations_hardening.sql đã thực thi thành công.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    RAISERROR(@ErrorMessage, 16, 1);
END CATCH;
