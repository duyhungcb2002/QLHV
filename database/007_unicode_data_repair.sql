/* ============================================================================
   QLHV - Hệ Thống Quản Lý Học Viên
   File: database/007_unicode_data_repair.sql
   Mục tiêu:
     Sửa chữa dữ liệu tiếng Việt bị mojibake trong các bảng vận hành (LopHoc, DangKyLopHoc, LichHoc, KetQuaHocTap, PhieuThu)
     theo đúng mã nghiệp vụ tự nhiên (Business Code) và khớp chính xác các chuỗi hỏng đã biết.
   Đặc điểm:
     - Idempotent: chạy nhiều lần an toàn
     - So sánh nhị phân chính xác (COLLATE Latin1_General_BIN IN (...))
     - Tuyệt đối không dùng marker rộng (LIKE '%Ã%', LIKE '%Â%') tránh ghi đè chuỗi hợp lệ
     - SET XACT_ABORT ON, giao dịch nguyên tử (TRANSACTION)
     - Lưu trữ UTF-8 with BOM (EF BB BF)
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

-- 1. Bảng dbo.LopHoc
-- 1.1 LH_NET_01 - TenLopHoc
UPDATE dbo.LopHoc
SET TenLopHoc = N'Lớp Chuyên viên .NET K01'
WHERE MaLopHoc = N'LH_NET_01'
  AND TenLopHoc COLLATE Latin1_General_BIN IN (
      N'L?p ChuyÃªn viÃªn .NET K01',
      N'Lá»›p ChuyÃªn viÃªn .NET K01',
      N'L?p Chuyên viên .NET K01'
  );

-- 1.2 LH_NET_01 - GhiChu
UPDATE dbo.LopHoc
SET GhiChu = N'Lớp học ban ngày thứ 2-4-6'
WHERE MaLopHoc = N'LH_NET_01'
  AND GhiChu COLLATE Latin1_General_BIN IN (
      N'L?p h?c ban ngÃ y th? 2-4-6',
      N'Lá»›p há»c ban ngÃ y thá»© 2-4-6',
      N'L?p h?c ban ngày th? 2-4-6'
  );

-- 1.3 LH_WF_02 - TenLopHoc
UPDATE dbo.LopHoc
SET TenLopHoc = N'Lớp WinForms Nâng Cao K02'
WHERE MaLopHoc = N'LH_WF_02'
  AND TenLopHoc COLLATE Latin1_General_BIN IN (
      N'L?p WinForms NÃ¢ng Cao K02',
      N'Lá»›p WinForms NÃ¢ng Cao K02',
      N'L?p WinForms Nâng Cao K02'
  );

-- 1.4 LH_WF_02 - GhiChu
UPDATE dbo.LopHoc
SET GhiChu = N'Lớp học buổi tối thứ 3-5-7'
WHERE MaLopHoc = N'LH_WF_02'
  AND GhiChu COLLATE Latin1_General_BIN IN (
      N'L?p h?c bu?i t?i th? 3-5-7',
      N'Lá»›p há»c buá»•i tá»‘i thá»© 3-5-7',
      N'L?p h?c bu?i tối th? 3-5-7'
  );

-- 2. Bảng dbo.DangKyLopHoc
-- 2.1 LH_NET_01 & HV001 - GhiChu
UPDATE dk
SET dk.GhiChu = N'Đăng ký khóa .NET K01'
FROM dbo.DangKyLopHoc dk
INNER JOIN dbo.LopHoc lh ON dk.LopHocId = lh.Id
INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
WHERE lh.MaLopHoc = N'LH_NET_01' AND hv.MaHocVien = N'HV001'
  AND dk.GhiChu COLLATE Latin1_General_BIN IN (
      N'?Äng kÃ½ khÃ³a .NET K01',
      N'ÄÄƒng kÃ½ khÃ³a .NET K01',
      N'?ăng ký khóa .NET K01'
  );

-- 2.2 LH_NET_01 & HV002 - GhiChu
UPDATE dk
SET dk.GhiChu = N'Đăng ký khóa .NET K01'
FROM dbo.DangKyLopHoc dk
INNER JOIN dbo.LopHoc lh ON dk.LopHocId = lh.Id
INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
WHERE lh.MaLopHoc = N'LH_NET_01' AND hv.MaHocVien = N'HV002'
  AND dk.GhiChu COLLATE Latin1_General_BIN IN (
      N'?Äng kÃ½ khÃ³a .NET K01',
      N'ÄÄƒng kÃ½ khÃ³a .NET K01',
      N'?ăng ký khóa .NET K01'
  );

-- 2.3 LH_WF_02 & HV003 - GhiChu
UPDATE dk
SET dk.GhiChu = N'Đăng ký khóa WinForms K02'
FROM dbo.DangKyLopHoc dk
INNER JOIN dbo.LopHoc lh ON dk.LopHocId = lh.Id
INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
WHERE lh.MaLopHoc = N'LH_WF_02' AND hv.MaHocVien = N'HV003'
  AND dk.GhiChu COLLATE Latin1_General_BIN IN (
      N'?Äng kÃ½ khÃ³a WinForms K02',
      N'ÄÄƒng kÃ½ khÃ³a WinForms K02',
      N'?ăng ký khóa WinForms K02'
  );

-- 3. Bảng dbo.LichHoc
-- 3.1 LH_NET_01, MH01, 2026-03-02 08:00 - NoiDung
UPDATE lh
SET lh.NoiDung = N'C# Cơ bản - Bài 1: Cú pháp và Kiểu dữ liệu'
FROM dbo.LichHoc lh
INNER JOIN dbo.LopHoc lop ON lh.LopHocId = lop.Id
INNER JOIN dbo.MonHoc mh ON lh.MonHocId = mh.Id
WHERE lop.MaLopHoc = N'LH_NET_01' AND mh.MaMonHoc = N'MH01' AND lh.NgayHoc = '2026-03-02' AND lh.GioBatDau = '08:00:00'
  AND lh.NoiDung COLLATE Latin1_General_BIN IN (
      N'C# CÆ¡ b?n - BÃ i 1: CÃº phÃ¡p vÃ  Ki?u d? li?u',
      N'C# CÆ¡ báº£n - BÃ i 1: CÃº phÃ¡p vÃ  Kiá»ƒu dá»¯ liá»‡u',
      N'C# Cơ b?n - Bài 1: Cú pháp và Ki?u d? li?u'
  );

-- 3.2 LH_NET_01, MH01, 2026-03-04 08:00 - NoiDung
UPDATE lh
SET lh.NoiDung = N'C# Cơ bản - Bài 2: Lập trình hướng đối tượng OOP'
FROM dbo.LichHoc lh
INNER JOIN dbo.LopHoc lop ON lh.LopHocId = lop.Id
INNER JOIN dbo.MonHoc mh ON lh.MonHocId = mh.Id
WHERE lop.MaLopHoc = N'LH_NET_01' AND mh.MaMonHoc = N'MH01' AND lh.NgayHoc = '2026-03-04' AND lh.GioBatDau = '08:00:00'
  AND lh.NoiDung COLLATE Latin1_General_BIN IN (
      N'C# CÆ¡ b?n - BÃ i 2: L?p trÃ¬nh hÆ°?ng d?i tÆ°?ng OOP',
      N'C# CÆ¡ báº£n - BÃ i 2: Láº­p trÃ¬nh hÆ°á»›ng Ä‘á»‘i tÆ°á»£ng OOP',
      N'C# Cơ b?n - Bài 2: L?p trình h?ớng đối t?ợng OOP'
  );

-- 3.3 LH_WF_02, MH02, 2026-03-17 18:00 - NoiDung
UPDATE lh
SET lh.NoiDung = N'WinForms - Bài 1: Form Layout & Controls cơ bản'
FROM dbo.LichHoc lh
INNER JOIN dbo.LopHoc lop ON lh.LopHocId = lop.Id
INNER JOIN dbo.MonHoc mh ON lh.MonHocId = mh.Id
WHERE lop.MaLopHoc = N'LH_WF_02' AND mh.MaMonHoc = N'MH02' AND lh.NgayHoc = '2026-03-17' AND lh.GioBatDau = '18:00:00'
  AND lh.NoiDung COLLATE Latin1_General_BIN IN (
      N'WinForms - BÃ i 1: Form Layout & Controls cÆ¡ b?n',
      N'WinForms - BÃ i 1: Form Layout & Controls cÆ¡ báº£n',
      N'WinForms - Bài 1: Form Layout & Controls cơ b?n'
  );

-- 3.4 LH_WF_02, MH02, 2026-03-19 18:00 - NoiDung
UPDATE lh
SET lh.NoiDung = N'WinForms - Bài 2: Data Binding & DataGridView'
FROM dbo.LichHoc lh
INNER JOIN dbo.LopHoc lop ON lh.LopHocId = lop.Id
INNER JOIN dbo.MonHoc mh ON lh.MonHocId = mh.Id
WHERE lop.MaLopHoc = N'LH_WF_02' AND mh.MaMonHoc = N'MH02' AND lh.NgayHoc = '2026-03-19' AND lh.GioBatDau = '18:00:00'
  AND lh.NoiDung COLLATE Latin1_General_BIN IN (
      N'WinForms - BÃ i 2: Data Binding & DataGridView',
      N'WinForms - Bài 2: Data Binding & DataGridView'
  );

-- 4. Bảng dbo.KetQuaHocTap
-- 4.1 LH_NET_01 & HV001 & MH01 - GhiChu
UPDATE kq
SET kq.GhiChu = N'Hoàn thành xuất sắc bài tập lớn'
FROM dbo.KetQuaHocTap kq
INNER JOIN dbo.DangKyLopHoc dk ON kq.DangKyLopHocId = dk.Id
INNER JOIN dbo.LopHoc lop ON dk.LopHocId = lop.Id
INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
INNER JOIN dbo.MonHoc mh ON kq.MonHocId = mh.Id
WHERE lop.MaLopHoc = N'LH_NET_01' AND hv.MaHocVien = N'HV001' AND mh.MaMonHoc = N'MH01'
  AND kq.GhiChu COLLATE Latin1_General_BIN IN (
      N'HoÃ n thÃ nh xu?t s?c bÃ i t?p l?n',
      N'HoÃ n thÃ nh xuáº¥t sáº¯c bÃ i táº­p lá»›n',
      N'Hoàn thành xu?t s?c bài t?p lớn'
  );

-- 4.2 LH_NET_01 & HV002 & MH01 - GhiChu
UPDATE kq
SET kq.GhiChu = N'Cần thi lại phần OOP'
FROM dbo.KetQuaHocTap kq
INNER JOIN dbo.DangKyLopHoc dk ON kq.DangKyLopHocId = dk.Id
INNER JOIN dbo.LopHoc lop ON dk.LopHocId = lop.Id
INNER JOIN dbo.HocVien hv ON dk.HocVienId = hv.Id
INNER JOIN dbo.MonHoc mh ON kq.MonHocId = mh.Id
WHERE lop.MaLopHoc = N'LH_NET_01' AND hv.MaHocVien = N'HV002' AND mh.MaMonHoc = N'MH01'
  AND kq.GhiChu COLLATE Latin1_General_BIN IN (
      N'C?n thi l?i ph?n OOP',
      N'Cáº§n thi láº¡i pháº§n OOP'
  );

-- 5. Bảng dbo.PhieuThu
-- 5.1 PT-20260215-001 - GhiChu
UPDATE dbo.PhieuThu
SET GhiChu = N'Nộp đợt 1 chuyển khoản qua Vietcombank'
WHERE MaPhieuThu = N'PT-20260215-001'
  AND GhiChu COLLATE Latin1_General_BIN IN (
      N'N?p d?t 1 chuy?n kho?n qua Vietcombank',
      N'Ná»™p Ä‘á»£t 1 chuyá»ƒn khoáº£n qua Vietcombank',
      N'Nộp đ?t 1 chuyển khoản qua Vietcombank'
  );

-- 5.2 PT-20260301-002 - GhiChu
UPDATE dbo.PhieuThu
SET GhiChu = N'Nộp đợt 2 đủ học phí bằng tiền mặt'
WHERE MaPhieuThu = N'PT-20260301-002'
  AND GhiChu COLLATE Latin1_General_BIN IN (
      N'N?p d?t 2 d? h?c phÃ­ b?ng ti?n m?t',
      N'Ná»™p Ä‘á»£t 2 Ä‘á»§ há»c phÃ­ báº±ng tiá»n máº·t',
      N'Nộp đ?t 2 đủ học phí bằng tiền m?t'
  );

-- 5.3 PT-20260220-003 - GhiChu
UPDATE dbo.PhieuThu
SET GhiChu = N'Thanh toán toàn bộ học phí khóa WinForms'
WHERE MaPhieuThu = N'PT-20260220-003'
  AND GhiChu COLLATE Latin1_General_BIN IN (
      N'Thanh toÃ¡n toÃ n b? h?c phÃ­ khÃ³a WinForms',
      N'Thanh toÃ¡n toÃ n bá»™ há»c phÃ­ khÃ³a WinForms',
      N'Thanh toán toàn b? học phí khóa WinForms'
  );

COMMIT TRANSACTION;
