/* ============================================================================
   QLHV - Hệ Thống Quản Lý Học Viên
   File: database/006_student_profile_and_class_advisor.sql
   Mục tiêu:
     1. Mở rộng hồ sơ học viên (CCCD/CMND, Người giám hộ, Nguồn tuyển sinh, Nhân viên tư vấn)
     2. Bổ sung Giảng viên chủ nhiệm cho Lớp học và backfill từ lịch học sớm nhất
   Đặc điểm:
     - Idempotent: chạy lại an toàn mà không sinh lỗi hoặc trùng lặp
     - An toàn giao dịch: SET XACT_ABORT ON, bọc trong TRANSACTION
   ============================================================================ */

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

-- ============================================================================
-- 1. Mở rộng bảng dbo.HocVien
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'SoDinhDanh')
BEGIN
    ALTER TABLE dbo.HocVien ADD SoDinhDanh NVARCHAR(20) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'NgayCapGiayTo')
BEGIN
    ALTER TABLE dbo.HocVien ADD NgayCapGiayTo DATE NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'NoiCapGiayTo')
BEGIN
    ALTER TABLE dbo.HocVien ADD NoiCapGiayTo NVARCHAR(150) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'HoTenNguoiGiamHo')
BEGIN
    ALTER TABLE dbo.HocVien ADD HoTenNguoiGiamHo NVARCHAR(100) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'DienThoaiNguoiGiamHo')
BEGIN
    ALTER TABLE dbo.HocVien ADD DienThoaiNguoiGiamHo NVARCHAR(20) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'QuanHeNguoiGiamHo')
BEGIN
    ALTER TABLE dbo.HocVien ADD QuanHeNguoiGiamHo NVARCHAR(50) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'NhanVienPhuTrachTaiKhoanId')
BEGIN
    ALTER TABLE dbo.HocVien ADD NhanVienPhuTrachTaiKhoanId INT NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'NguonTuyenSinh')
BEGIN
    ALTER TABLE dbo.HocVien ADD NguonTuyenSinh NVARCHAR(50) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'NgheNghiep')
BEGIN
    ALTER TABLE dbo.HocVien ADD NgheNghiep NVARCHAR(100) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'DonViCongTac')
BEGIN
    ALTER TABLE dbo.HocVien ADD DonViCongTac NVARCHAR(200) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.HocVien') AND name = N'GhiChuDacBiet')
BEGIN
    ALTER TABLE dbo.HocVien ADD GhiChuDacBiet NVARCHAR(1000) NULL;
END;

-- Khóa ngoại đến dbo.TaiKhoan(Id)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_HocVien_NhanVienPhuTrach' AND parent_object_id = OBJECT_ID(N'dbo.HocVien'))
BEGIN
    ALTER TABLE dbo.HocVien
    ADD CONSTRAINT FK_HocVien_NhanVienPhuTrach
        FOREIGN KEY (NhanVienPhuTrachTaiKhoanId) REFERENCES dbo.TaiKhoan(Id);
END;

-- Filtered unique index cho SoDinhDanh
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_HocVien_SoDinhDanh' AND object_id = OBJECT_ID(N'dbo.HocVien'))
BEGIN
    EXEC(N'SET QUOTED_IDENTIFIER ON;
    CREATE UNIQUE NONCLUSTERED INDEX UX_HocVien_SoDinhDanh
        ON dbo.HocVien(SoDinhDanh)
        WHERE SoDinhDanh IS NOT NULL;');
END;

-- Index cho NhanVienPhuTrachTaiKhoanId
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HocVien_NhanVienPhuTrach' AND object_id = OBJECT_ID(N'dbo.HocVien'))
BEGIN
    EXEC(N'CREATE NONCLUSTERED INDEX IX_HocVien_NhanVienPhuTrach
        ON dbo.HocVien(NhanVienPhuTrachTaiKhoanId);');
END;

-- ============================================================================
-- 2. Bổ sung Giảng viên chủ nhiệm vào bảng dbo.LopHoc
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.LopHoc') AND name = N'GiangVienChuNhiemId')
BEGIN
    ALTER TABLE dbo.LopHoc ADD GiangVienChuNhiemId INT NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_LopHoc_GiangVienChuNhiem' AND parent_object_id = OBJECT_ID(N'dbo.LopHoc'))
BEGIN
    ALTER TABLE dbo.LopHoc
    ADD CONSTRAINT FK_LopHoc_GiangVienChuNhiem
        FOREIGN KEY (GiangVienChuNhiemId) REFERENCES dbo.GiangVien(Id);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LopHoc_GiangVienChuNhiem' AND object_id = OBJECT_ID(N'dbo.LopHoc'))
BEGIN
    EXEC(N'CREATE NONCLUSTERED INDEX IX_LopHoc_GiangVienChuNhiem
        ON dbo.LopHoc(GiangVienChuNhiemId);');
END;

-- Backfill giảng viên chủ nhiệm từ lịch học đang hoạt động sớm nhất của từng lớp
EXEC(N'UPDATE lh
SET lh.GiangVienChuNhiemId = earliest.GiangVienId
FROM dbo.LopHoc lh
CROSS APPLY (
    SELECT TOP 1 sch.GiangVienId
    FROM dbo.LichHoc sch
    WHERE sch.LopHocId = lh.Id AND sch.TrangThai = 1
    ORDER BY sch.NgayHoc ASC, sch.GioBatDau ASC, sch.Id ASC
) earliest
WHERE lh.GiangVienChuNhiemId IS NULL;');

COMMIT TRANSACTION;
