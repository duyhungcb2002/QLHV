/* ============================================================================
   QLHV - Hệ Thống Quản Lý Học Viên
   File: database/004_operations_seed.sql
   Giai đoạn: MEGA MVP (Dữ liệu mẫu idempotent cho Nghiệp vụ vận hành)
   Các bảng:
     - LopHoc (2 lớp học)
     - DangKyLopHoc (3 học viên được xếp lớp với snapshot học phí)
     - LichHoc (4 buổi học mẫu)
     - KetQuaHocTap (điểm tổng kết mẫu)
     - PhieuThu (phiếu thu học phí mẫu)
   Quy tắc:
     - Idempotent: chạy nhiều lần an toàn, không sinh bản ghi trùng.
     - Khóa ngoại resolve bằng mã tự nhiên (Business Code), KHÔNG hardcode identity.
     - Học phí phải thu lấy snapshot từ KhoaHoc.HocPhiNiemYet.
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

-- ============================================================================
-- 1. Resolve Foreign Key IDs từ danh mục đã seed (002_seed.sql)
-- ============================================================================
DECLARE @Kh01Id INT = (SELECT Id FROM dbo.KhoaHoc WHERE MaKhoaHoc = N'KH01');
DECLARE @Kh02Id INT = (SELECT Id FROM dbo.KhoaHoc WHERE MaKhoaHoc = N'KH02');

DECLARE @Gv01Id INT = (SELECT Id FROM dbo.GiangVien WHERE MaGiangVien = N'GV001');
DECLARE @Gv02Id INT = (SELECT Id FROM dbo.GiangVien WHERE MaGiangVien = N'GV002');

DECLARE @Mh01Id INT = (SELECT Id FROM dbo.MonHoc WHERE MaMonHoc = N'MH01');
DECLARE @Mh02Id INT = (SELECT Id FROM dbo.MonHoc WHERE MaMonHoc = N'MH02');

DECLARE @P101Id INT = (SELECT Id FROM dbo.PhongHoc WHERE MaPhong = N'P101');
DECLARE @P102Id INT = (SELECT Id FROM dbo.PhongHoc WHERE MaPhong = N'P102');

DECLARE @Hv01Id INT = (SELECT Id FROM dbo.HocVien WHERE MaHocVien = N'HV001');
DECLARE @Hv02Id INT = (SELECT Id FROM dbo.HocVien WHERE MaHocVien = N'HV002');
DECLARE @Hv03Id INT = (SELECT Id FROM dbo.HocVien WHERE MaHocVien = N'HV003');

DECLARE @NguoiThuId INT = (SELECT Id FROM dbo.TaiKhoan WHERE TenDangNhap = N'thungan');
IF @NguoiThuId IS NULL
    SET @NguoiThuId = (SELECT TOP 1 Id FROM dbo.TaiKhoan WHERE VaiTro = N'THUNGAN' OR VaiTro = N'ADMIN');

-- ============================================================================
-- 2. Seed Lớp học (LopHoc)
-- ============================================================================
IF @Kh01Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.LopHoc WHERE MaLopHoc = N'LH_NET_01')
BEGIN
    INSERT INTO dbo.LopHoc (MaLopHoc, TenLopHoc, KhoaHocId, NgayBatDau, NgayKetThuc, SiSoToiDa, TrangThai, GhiChu)
    VALUES (N'LH_NET_01', N'Lớp Chuyên viên .NET K01', @Kh01Id, '2026-03-01', '2026-07-01', 30, 1, N'Lớp học ban ngày thứ 2-4-6');
END;

IF @Kh02Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.LopHoc WHERE MaLopHoc = N'LH_WF_02')
BEGIN
    INSERT INTO dbo.LopHoc (MaLopHoc, TenLopHoc, KhoaHocId, NgayBatDau, NgayKetThuc, SiSoToiDa, TrangThai, GhiChu)
    VALUES (N'LH_WF_02', N'Lớp WinForms Nâng Cao K02', @Kh02Id, '2026-03-15', '2026-06-15', 25, 1, N'Lớp học buổi tối thứ 3-5-7');
END;

DECLARE @Lop01Id INT = (SELECT Id FROM dbo.LopHoc WHERE MaLopHoc = N'LH_NET_01');
DECLARE @Lop02Id INT = (SELECT Id FROM dbo.LopHoc WHERE MaLopHoc = N'LH_WF_02');

-- ============================================================================
-- 3. Seed Xếp học viên vào lớp (DangKyLopHoc)
-- ============================================================================
-- HV001 vào LH_NET_01
IF @Lop01Id IS NOT NULL AND @Hv01Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DangKyLopHoc WHERE LopHocId = @Lop01Id AND HocVienId = @Hv01Id)
BEGIN
    DECLARE @HocPhi01 DECIMAL(18, 2) = (SELECT HocPhiNiemYet FROM dbo.KhoaHoc WHERE Id = @Kh01Id);
    INSERT INTO dbo.DangKyLopHoc (LopHocId, HocVienId, NgayDangKy, HocPhiPhaiThu, TrangThai, GhiChu)
    VALUES (@Lop01Id, @Hv01Id, '2026-02-15', @HocPhi01, 1, N'Đăng ký khóa .NET K01');
END;

-- HV002 vào LH_NET_01
IF @Lop01Id IS NOT NULL AND @Hv02Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DangKyLopHoc WHERE LopHocId = @Lop01Id AND HocVienId = @Hv02Id)
BEGIN
    DECLARE @HocPhi01_2 DECIMAL(18, 2) = (SELECT HocPhiNiemYet FROM dbo.KhoaHoc WHERE Id = @Kh01Id);
    INSERT INTO dbo.DangKyLopHoc (LopHocId, HocVienId, NgayDangKy, HocPhiPhaiThu, TrangThai, GhiChu)
    VALUES (@Lop01Id, @Hv02Id, '2026-02-18', @HocPhi01_2, 1, N'Đăng ký khóa .NET K01');
END;

-- HV003 vào LH_WF_02
IF @Lop02Id IS NOT NULL AND @Hv03Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DangKyLopHoc WHERE LopHocId = @Lop02Id AND HocVienId = @Hv03Id)
BEGIN
    DECLARE @HocPhi02 DECIMAL(18, 2) = (SELECT HocPhiNiemYet FROM dbo.KhoaHoc WHERE Id = @Kh02Id);
    INSERT INTO dbo.DangKyLopHoc (LopHocId, HocVienId, NgayDangKy, HocPhiPhaiThu, TrangThai, GhiChu)
    VALUES (@Lop02Id, @Hv03Id, '2026-02-20', @HocPhi02, 1, N'Đăng ký khóa WinForms K02');
END;

-- ============================================================================
-- 4. Seed Lịch học (LichHoc)
-- ============================================================================
-- Buổi 1 của LH_NET_01 (Môn MH01, GV001, P101)
IF @Lop01Id IS NOT NULL AND @Mh01Id IS NOT NULL AND @P101Id IS NOT NULL AND @Gv01Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.LichHoc WHERE LopHocId = @Lop01Id AND NgayHoc = '2026-03-02' AND GioBatDau = '08:00:00')
BEGIN
    INSERT INTO dbo.LichHoc (LopHocId, MonHocId, GiangVienId, PhongHocId, NgayHoc, GioBatDau, GioKetThuc, NoiDung, TrangThai)
    VALUES (@Lop01Id, @Mh01Id, @Gv01Id, @P101Id, '2026-03-02', '08:00:00', '11:00:00', N'C# Cơ bản - Bài 1: Cú pháp và Kiểu dữ liệu', 1);
END;

-- Buổi 2 của LH_NET_01 (Môn MH01, GV001, P101)
IF @Lop01Id IS NOT NULL AND @Mh01Id IS NOT NULL AND @P101Id IS NOT NULL AND @Gv01Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.LichHoc WHERE LopHocId = @Lop01Id AND NgayHoc = '2026-03-04' AND GioBatDau = '08:00:00')
BEGIN
    INSERT INTO dbo.LichHoc (LopHocId, MonHocId, GiangVienId, PhongHocId, NgayHoc, GioBatDau, GioKetThuc, NoiDung, TrangThai)
    VALUES (@Lop01Id, @Mh01Id, @Gv01Id, @P101Id, '2026-03-04', '08:00:00', '11:00:00', N'C# Cơ bản - Bài 2: Lập trình hướng đối tượng OOP', 1);
END;

-- Buổi 1 của LH_WF_02 (Môn MH02, GV002, P102)
IF @Lop02Id IS NOT NULL AND @Mh02Id IS NOT NULL AND @P102Id IS NOT NULL AND @Gv02Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.LichHoc WHERE LopHocId = @Lop02Id AND NgayHoc = '2026-03-17' AND GioBatDau = '18:00:00')
BEGIN
    INSERT INTO dbo.LichHoc (LopHocId, MonHocId, GiangVienId, PhongHocId, NgayHoc, GioBatDau, GioKetThuc, NoiDung, TrangThai)
    VALUES (@Lop02Id, @Mh02Id, @Gv02Id, @P102Id, '2026-03-17', '18:00:00', '21:00:00', N'WinForms - Bài 1: Form Layout & Controls cơ bản', 1);
END;

-- Buổi 2 của LH_WF_02 (Môn MH02, GV002, P102)
IF @Lop02Id IS NOT NULL AND @Mh02Id IS NOT NULL AND @P102Id IS NOT NULL AND @Gv02Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.LichHoc WHERE LopHocId = @Lop02Id AND NgayHoc = '2026-03-19' AND GioBatDau = '18:00:00')
BEGIN
    INSERT INTO dbo.LichHoc (LopHocId, MonHocId, GiangVienId, PhongHocId, NgayHoc, GioBatDau, GioKetThuc, NoiDung, TrangThai)
    VALUES (@Lop02Id, @Mh02Id, @Gv02Id, @P102Id, '2026-03-19', '18:00:00', '21:00:00', N'WinForms - Bài 2: Data Binding & DataGridView', 1);
END;

-- ============================================================================
-- 5. Seed Kết quả học tập (KetQuaHocTap)
-- ============================================================================
DECLARE @DangKy01Id INT = (SELECT Id FROM dbo.DangKyLopHoc WHERE LopHocId = @Lop01Id AND HocVienId = @Hv01Id);
DECLARE @DangKy02Id INT = (SELECT Id FROM dbo.DangKyLopHoc WHERE LopHocId = @Lop01Id AND HocVienId = @Hv02Id);
DECLARE @DangKy03Id INT = (SELECT Id FROM dbo.DangKyLopHoc WHERE LopHocId = @Lop02Id AND HocVienId = @Hv03Id);

-- HV001 - MH01 trong LH_NET_01: 8.5 điểm (Đạt)
IF @DangKy01Id IS NOT NULL AND @Mh01Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.KetQuaHocTap WHERE DangKyLopHocId = @DangKy01Id AND MonHocId = @Mh01Id)
BEGIN
    INSERT INTO dbo.KetQuaHocTap (DangKyLopHocId, MonHocId, DiemTongKet, NgayCapNhat, NguoiCapNhatTaiKhoanId, GhiChu)
    VALUES (@DangKy01Id, @Mh01Id, 8.50, SYSDATETIME(), @NguoiThuId, N'Hoàn thành xuất sắc bài tập lớn');
END;

-- HV002 - MH01 trong LH_NET_01: 4.5 điểm (Chưa đạt)
IF @DangKy02Id IS NOT NULL AND @Mh01Id IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.KetQuaHocTap WHERE DangKyLopHocId = @DangKy02Id AND MonHocId = @Mh01Id)
BEGIN
    INSERT INTO dbo.KetQuaHocTap (DangKyLopHocId, MonHocId, DiemTongKet, NgayCapNhat, NguoiCapNhatTaiKhoanId, GhiChu)
    VALUES (@DangKy02Id, @Mh01Id, 4.50, SYSDATETIME(), @NguoiThuId, N'Cần thi lại phần OOP');
END;

-- ============================================================================
-- 6. Seed Phiếu thu học phí (PhieuThu)
-- ============================================================================
-- HV001 đóng đợt 1: 5.000.000 VNĐ
IF @DangKy01Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.PhieuThu WHERE MaPhieuThu = N'PT-20260215-001')
BEGIN
    INSERT INTO dbo.PhieuThu (MaPhieuThu, DangKyLopHocId, NgayThu, SoTien, HinhThuc, NguoiThuTaiKhoanId, TrangThai, GhiChu)
    VALUES (N'PT-20260215-001', @DangKy01Id, '2026-02-15 09:30:00', 5000000.00, N'CHUYEN_KHOAN', @NguoiThuId, 1, N'Nộp đợt 1 chuyển khoản qua Vietcombank');
END;

-- HV001 đóng đợt 2: 3.500.000 VNĐ (đủ 8.500.000)
IF @DangKy01Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.PhieuThu WHERE MaPhieuThu = N'PT-20260301-002')
BEGIN
    INSERT INTO dbo.PhieuThu (MaPhieuThu, DangKyLopHocId, NgayThu, SoTien, HinhThuc, NguoiThuTaiKhoanId, TrangThai, GhiChu)
    VALUES (N'PT-20260301-002', @DangKy01Id, '2026-03-01 14:00:00', 3500000.00, N'TIEN_MAT', @NguoiThuId, 1, N'Nộp đợt 2 đủ học phí bằng tiền mặt');
END;

-- HV003 đóng toàn bộ: 5.500.000 VNĐ
IF @DangKy03Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.PhieuThu WHERE MaPhieuThu = N'PT-20260220-003')
BEGIN
    INSERT INTO dbo.PhieuThu (MaPhieuThu, DangKyLopHocId, NgayThu, SoTien, HinhThuc, NguoiThuTaiKhoanId, TrangThai, GhiChu)
    VALUES (N'PT-20260220-003', @DangKy03Id, '2026-02-20 10:15:00', 5500000.00, N'CHUYEN_KHOAN', @NguoiThuId, 1, N'Thanh toán toàn bộ học phí khóa WinForms');
END;

COMMIT TRANSACTION;
