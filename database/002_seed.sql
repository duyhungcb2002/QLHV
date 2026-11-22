/* ============================================================================
   QLHV - Hệ Thống Quản Lý Học Viên
   File: database/002_seed.sql
   Giai đoạn: SQL-01A (Dữ liệu mẫu idempotent cho Đăng nhập và 6 Danh mục)
   Đặc điểm:
     - Chạy lại nhiều lần không sinh bản ghi trùng (Idempotent).
     - Mật khẩu trong bảng TaiKhoan được băm bằng PBKDF2-SHA256 (100.000 vòng lặp,
       16-byte salt, 32-byte key, định dạng "{Base64(Salt)}:{Base64(Hash)}")
       tương thích 100% với Pbkdf2PasswordHasher trong QLHV.Core.Security.
     - Không lưu mật khẩu thô trong cột MatKhauHash.
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

-- ============================================================================
-- 1. Seed Giảng viên (GiangVien)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.GiangVien WHERE MaGiangVien = N'GV001')
BEGIN
    INSERT INTO dbo.GiangVien (MaGiangVien, HoTen, ChuyenMon, DienThoai, Email, TrangThai)
    VALUES (N'GV001', N'TS. Phạm Văn Dũng', N'Lập trình C#, WinForms & .NET', N'0971112233', N'dung.pham@example.com', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.GiangVien WHERE MaGiangVien = N'GV002')
BEGIN
    INSERT INTO dbo.GiangVien (MaGiangVien, HoTen, ChuyenMon, DienThoai, Email, TrangThai)
    VALUES (N'GV002', N'ThS. Đỗ Thị Hương', N'Cơ sở dữ liệu & SQL Server', N'0972223344', N'huong.do@example.com', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.GiangVien WHERE MaGiangVien = N'GV003')
BEGIN
    INSERT INTO dbo.GiangVien (MaGiangVien, HoTen, ChuyenMon, DienThoai, Email, TrangThai)
    VALUES (N'GV003', N'ThS. Ngô Quang Hải', N'Lập trình Web & Kiến trúc phần mềm', N'0973334455', N'hai.ngo@example.com', 1);
END;

-- ============================================================================
-- 2. Seed Học viên (HocVien)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.HocVien WHERE MaHocVien = N'HV001')
BEGIN
    INSERT INTO dbo.HocVien (MaHocVien, HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, NgayDangKy, TrangThai)
    VALUES (N'HV001', N'Nguyễn Văn An', '2003-05-12', N'Nam', N'0981234567', N'an.nguyen@example.com', N'Hà Nội', '2026-01-15', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.HocVien WHERE MaHocVien = N'HV002')
BEGIN
    INSERT INTO dbo.HocVien (MaHocVien, HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, NgayDangKy, TrangThai)
    VALUES (N'HV002', N'Trần Thị Bình', '2004-08-20', N'Nữ', N'0912345678', N'binh.tran@example.com', N'Đà Nẵng', '2026-02-10', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.HocVien WHERE MaHocVien = N'HV003')
BEGIN
    INSERT INTO dbo.HocVien (MaHocVien, HoTen, NgaySinh, GioiTinh, DienThoai, Email, DiaChi, NgayDangKy, TrangThai)
    VALUES (N'HV003', N'Lê Hoàng Cường', '2002-11-05', N'Nam', N'0903456789', N'cuong.le@example.com', N'TP. Hồ Chí Minh', '2026-03-01', 1);
END;

-- ============================================================================
-- 3. Seed Môn học (MonHoc)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = N'MH01')
BEGIN
    INSERT INTO dbo.MonHoc (MaMonHoc, TenMonHoc, SoBuoiMacDinh, MoTa, TrangThai)
    VALUES (N'MH01', N'Lập trình C# cơ bản', 15, N'Nhập môn cú pháp C#, OOP và các cấu trúc dữ liệu cơ bản', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = N'MH02')
BEGIN
    INSERT INTO dbo.MonHoc (MaMonHoc, TenMonHoc, SoBuoiMacDinh, MoTa, TrangThai)
    VALUES (N'MH02', N'Lập trình Windows Forms', 20, N'Thiết kế giao diện Windows Forms, controls, validation, ADO.NET', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = N'MH03')
BEGIN
    INSERT INTO dbo.MonHoc (MaMonHoc, TenMonHoc, SoBuoiMacDinh, MoTa, TrangThai)
    VALUES (N'MH03', N'Quản trị & Lập trình SQL Server', 18, N'Thiết kế CSDL quan hệ, T-SQL, Stored Procedures, Triggers, Views', 1);
END;

-- ============================================================================
-- 4. Seed Khóa học (KhoaHoc)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.KhoaHoc WHERE MaKhoaHoc = N'KH01')
BEGIN
    INSERT INTO dbo.KhoaHoc (MaKhoaHoc, TenKhoaHoc, HocPhiNiemYet, MoTa, TrangThai)
    VALUES (N'KH01', N'Chuyên viên Lập trình .NET Toàn diện', 8500000.00, N'Khóa đào tạo chuyên sâu từ C# cơ bản, WinForms đến SQL Server', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.KhoaHoc WHERE MaKhoaHoc = N'KH02')
BEGIN
    INSERT INTO dbo.KhoaHoc (MaKhoaHoc, TenKhoaHoc, HocPhiNiemYet, MoTa, TrangThai)
    VALUES (N'KH02', N'Lập trình C# WinForms Cơ bản & Nâng cao', 5500000.00, N'Thành thạo xây dựng ứng dụng máy tính trên nền tảng Windows', 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.KhoaHoc WHERE MaKhoaHoc = N'KH03')
BEGIN
    INSERT INTO dbo.KhoaHoc (MaKhoaHoc, TenKhoaHoc, HocPhiNiemYet, MoTa, TrangThai)
    VALUES (N'KH03', N'Quản trị CSDL & T-SQL Doanh nghiệp', 4500000.00, N'Thiết kế, tối ưu hóa và quản trị cơ sở dữ liệu SQL Server', 1);
END;

-- ============================================================================
-- 5. Seed Quan hệ Khóa học - Môn học (KhoaHocMonHoc)
-- ============================================================================
DECLARE @Kh01Id INT = (SELECT Id FROM dbo.KhoaHoc WHERE MaKhoaHoc = N'KH01');
DECLARE @Kh02Id INT = (SELECT Id FROM dbo.KhoaHoc WHERE MaKhoaHoc = N'KH02');
DECLARE @Mh01Id INT = (SELECT Id FROM dbo.MonHoc WHERE MaMonHoc = N'MH01');
DECLARE @Mh02Id INT = (SELECT Id FROM dbo.MonHoc WHERE MaMonHoc = N'MH02');
DECLARE @Mh03Id INT = (SELECT Id FROM dbo.MonHoc WHERE MaMonHoc = N'MH03');

IF @Kh01Id IS NOT NULL AND @Mh01Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.KhoaHocMonHoc WHERE KhoaHocId = @Kh01Id AND MonHocId = @Mh01Id)
BEGIN
    INSERT INTO dbo.KhoaHocMonHoc (KhoaHocId, MonHocId, ThuTu, SoBuoiDuKien)
    VALUES (@Kh01Id, @Mh01Id, 1, 15);
END;

IF @Kh01Id IS NOT NULL AND @Mh02Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.KhoaHocMonHoc WHERE KhoaHocId = @Kh01Id AND MonHocId = @Mh02Id)
BEGIN
    INSERT INTO dbo.KhoaHocMonHoc (KhoaHocId, MonHocId, ThuTu, SoBuoiDuKien)
    VALUES (@Kh01Id, @Mh02Id, 2, 20);
END;

IF @Kh01Id IS NOT NULL AND @Mh03Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.KhoaHocMonHoc WHERE KhoaHocId = @Kh01Id AND MonHocId = @Mh03Id)
BEGIN
    INSERT INTO dbo.KhoaHocMonHoc (KhoaHocId, MonHocId, ThuTu, SoBuoiDuKien)
    VALUES (@Kh01Id, @Mh03Id, 3, 18);
END;

IF @Kh02Id IS NOT NULL AND @Mh01Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.KhoaHocMonHoc WHERE KhoaHocId = @Kh02Id AND MonHocId = @Mh01Id)
BEGIN
    INSERT INTO dbo.KhoaHocMonHoc (KhoaHocId, MonHocId, ThuTu, SoBuoiDuKien)
    VALUES (@Kh02Id, @Mh01Id, 1, 15);
END;

IF @Kh02Id IS NOT NULL AND @Mh02Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.KhoaHocMonHoc WHERE KhoaHocId = @Kh02Id AND MonHocId = @Mh02Id)
BEGIN
    INSERT INTO dbo.KhoaHocMonHoc (KhoaHocId, MonHocId, ThuTu, SoBuoiDuKien)
    VALUES (@Kh02Id, @Mh02Id, 2, 20);
END;

-- ============================================================================
-- 6. Seed Phòng học (PhongHoc)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.PhongHoc WHERE MaPhong = N'P101')
BEGIN
    INSERT INTO dbo.PhongHoc (MaPhong, TenPhong, SucChua, TrangThai)
    VALUES (N'P101', N'Phòng Lab Máy tính 01', 35, 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.PhongHoc WHERE MaPhong = N'P102')
BEGIN
    INSERT INTO dbo.PhongHoc (MaPhong, TenPhong, SucChua, TrangThai)
    VALUES (N'P102', N'Phòng Lab Máy tính 02', 40, 1);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.PhongHoc WHERE MaPhong = N'P201')
BEGIN
    INSERT INTO dbo.PhongHoc (MaPhong, TenPhong, SucChua, TrangThai)
    VALUES (N'P201', N'Phòng Lý thuyết Hội thảo', 60, 1);
END;

-- ============================================================================
-- 7. Seed Tài khoản người dùng (TaiKhoan) - 5 vai trò hệ thống + 1 tài khoản bị khóa
-- ============================================================================
DECLARE @Gv001Id INT = (SELECT Id FROM dbo.GiangVien WHERE MaGiangVien = N'GV001');

-- Vai trò 1: ADMIN (tài khoản: admin)
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = N'admin')
BEGIN
    INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, GiangVienId, TrangThai, LanDangNhapCuoi)
    VALUES (
        N'admin',
        N'ge5Z3vXgtKRJyZ0FjGC8KQ==:FrqyNElSVuecnx4DFz3RLBUMY0kX+A+y+EAeiJLv/yo=',
        N'ADMIN',
        NULL,
        1,
        NULL
    );
END;

-- Vai trò 2: DAOTAO (tài khoản: daotao)
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = N'daotao')
BEGIN
    INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, GiangVienId, TrangThai, LanDangNhapCuoi)
    VALUES (
        N'daotao',
        N'gNmofhEyEN21eKdn5NyDgQ==:ZAsupeJoh/Nm4fYnDBEeAZPhMfLwx94MRfv0l5SX/9M=',
        N'DAOTAO',
        NULL,
        1,
        NULL
    );
END;

-- Vai trò 3: THUNGAN (tài khoản: thungan)
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = N'thungan')
BEGIN
    INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, GiangVienId, TrangThai, LanDangNhapCuoi)
    VALUES (
        N'thungan',
        N'pu+8MNiVRKTRL+LWeX/H+w==:6/gaRm4lYR1nny64oDlZ1S6s8CUEroTLyasWTn3dqzc=',
        N'THUNGAN',
        NULL,
        1,
        NULL
    );
END;

-- Vai trò 4: GIANGVIEN (tài khoản: giangvien, liên kết với GV001)
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = N'giangvien')
BEGIN
    INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, GiangVienId, TrangThai, LanDangNhapCuoi)
    VALUES (
        N'giangvien',
        N'E3g3Eogs4e0v+6bBwdqmYg==:Qp30mXvB0kt/qdE+y9JCuMqTOW9hWy4joaguE4NcFPc=',
        N'GIANGVIEN',
        @Gv001Id,
        1,
        NULL
    );
END;

-- Vai trò 5: QUANLY (tài khoản: quanly)
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = N'quanly')
BEGIN
    INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, GiangVienId, TrangThai, LanDangNhapCuoi)
    VALUES (
        N'quanly',
        N'i2SMTRqcuaj6GShNoEBhYQ==:nAgGZ30CoGOE4Q1l9ymocs3bzD69daY8DtgeRKeHpCk=',
        N'QUANLY',
        NULL,
        1,
        NULL
    );
END;

-- Tài khoản kiểm thử bị khóa: bikhoa (Vai trò: DAOTAO, Trạng thái: 0 - BiKhoa)
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = N'bikhoa')
BEGIN
    INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, GiangVienId, TrangThai, LanDangNhapCuoi)
    VALUES (
        N'bikhoa',
        N'v73u9UsWLV6abGBofTpaVg==:2MvbpK6pvGoBBBqy/hgV2PMz4lcyAuYf5RHVF4AxISE=',
        N'DAOTAO',
        NULL,
        0,
        NULL
    );
END;

COMMIT TRANSACTION;
GO
