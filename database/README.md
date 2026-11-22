# Hướng Dẫn Cơ Sở Dữ Liệu SQL Server (QLHV)

> **Trạng thái kiểm chứng hiện tại:**
> Toàn bộ 7 script trong thư mục này (`001_schema.sql`, `002_seed.sql`, `003_operations_schema.sql`, `004_operations_seed.sql`, `005_operations_hardening.sql`, `006_student_profile_and_class_advisor.sql`, `007_unicode_data_repair.sql`) đã được thực thi và kiểm chứng runtime thành công trên Microsoft SQL Server Express (`.\SQLEXPRESS`).
> - Database chính của ứng dụng: **`QLHV`** (phục vụ người dùng và ứng dụng WinForms).
> - Database kiểm thử tích hợp: **`QLHV_Test`** (phục vụ bộ test tự động `SqlServerIntegrationTests`).
> - Ứng dụng WinForms tuyệt đối không kết nối vào database kiểm thử `QLHV_Test`.

---

## 1. Yêu cầu môi trường SQL Server

- **Hệ quản trị CSDL:** Microsoft SQL Server 2019 / 2022 trở lên (Developer, Express) hoặc SQL Server Express LocalDB.
- **Công cụ thực thi:** SQL Server Management Studio (SSMS), Azure Data Studio hoặc công cụ dòng lệnh `sqlcmd`.
- **Bảng mã / Collation khuyến nghị:** `Vietnamese_100_CI_AI` (hỗ trợ tiếng Việt đầy đủ, không phân biệt hoa/thường, không phân biệt dấu khi tìm kiếm). Tất cả các script CSDL được mã hóa theo chuẩn UTF-8 with BOM (`utf-8-bom`).

---

## 2. Cách tạo Database Demo

Kết nối vào SQL Server local bằng quyền tạo database và chạy lệnh sau trên SSMS hoặc `sqlcmd`:

```sql
IF DB_ID(N'QLHV') IS NULL
BEGIN
    CREATE DATABASE QLHV COLLATE Vietnamese_100_CI_AI;
END;
GO
```

---

## 3. Thứ tự chạy Script

Luôn chạy các script theo đúng thứ tự sau trên database `QLHV` (và tương tự cho `QLHV_Test`):

1. **`001_schema.sql`** — Khởi tạo 7 bảng danh mục & xác thực (`GiangVien`, `TaiKhoan`, `HocVien`, `MonHoc`, `KhoaHoc`, `KhoaHocMonHoc`, `PhongHoc`), kèm theo khóa chính, khóa ngoại, unique constraint, check constraint và index.
2. **`002_seed.sql`** — Nạp dữ liệu mẫu idempotent cho 6 danh mục và 6 tài khoản demo: 5 vai trò hoạt động và 1 tài khoản bị khóa (`ADMIN`, `DAOTAO`, `THUNGAN`, `GIANGVIEN`, `QUANLY`, `bikhoa`). Mật khẩu được lưu dưới dạng băm PBKDF2-SHA256 tương thích với `Pbkdf2PasswordHasher`.
3. **`003_operations_schema.sql`** — Khởi tạo 5 bảng nghiệp vụ vận hành (`LopHoc`, `DangKyLopHoc`, `LichHoc`, `KetQuaHocTap`, `PhieuThu`), ràng buộc toàn vẹn, index hiệu năng cao.
4. **`004_operations_seed.sql`** — Nạp dữ liệu mẫu hoạt động đào tạo, các lớp học đang mở, lịch học giảng dạy, phân công giảng viên, điểm thi và các giao dịch đóng học phí.
5. **`005_operations_hardening.sql`** — Bổ sung cột kiểm toán hủy phiếu thu (`NgayHuy`, `NguoiHuyTaiKhoanId`), ràng buộc khóa ngoại tới `TaiKhoan`, cùng các index tăng tốc kiểm tra tranh chấp lịch học và đồng thời.
6. **`006_student_profile_and_class_advisor.sql`** — Mở rộng hồ sơ học viên trong `dbo.HocVien` (11 trường: `SoDinhDanh`, `NgayCapGiayTo`, `NoiCapGiayTo`, `HoTenNguoiGiamHo`, `DienThoaiNguoiGiamHo`, `QuanHeNguoiGiamHo`, `NhanVienPhuTrachTaiKhoanId`, `NguonTuyenSinh`, `NgheNghiep`, `DonViCongTac`, `GhiChuDacBiet`), filtered unique index trên `SoDinhDanh`, và bổ sung `GiangVienChuNhiemId` cho `dbo.LopHoc` kèm backfill tự động từ lịch dạy sớm nhất.
7. **`007_unicode_data_repair.sql`** — Sửa chữa triệt để lỗi mã hóa tiếng Việt (mojibake) trên toàn bộ dữ liệu nghiệp vụ `LopHoc`, `DangKyLopHoc`, `LichHoc`, `KetQuaHocTap`, `PhieuThu`. Script được mã hóa UTF-8 with BOM (`utf-8-bom`), sử dụng điều kiện kết hợp mã nghiệp vụ cụ thể và collation nhị phân chính xác (`COLLATE Latin1_General_BIN`), loại bỏ hoàn toàn các pattern wildcard rộng để không bao giờ ghi đè dữ liệu tiếng Việt hợp lệ và đảm bảo tính lũy thừa (idempotency).

Ví dụ chạy bằng `sqlcmd` với Windows Authentication:

```powershell
sqlcmd -S .\SQLEXPRESS -d QLHV -E -C -i database\001_schema.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV -E -C -i database\002_seed.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV -E -C -i database\003_operations_schema.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV -E -C -i database\004_operations_seed.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV -E -C -i database\005_operations_hardening.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV -E -C -i database\006_student_profile_and_class_advisor.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV -E -C -i database\007_unicode_data_repair.sql -b
```

---

## 4. Cách kiểm tra bảng và dữ liệu sau khi chạy

Chạy truy vấn kiểm tra nhanh trên database `QLHV`:

```sql
USE QLHV;
GO

-- 1. Kiểm tra danh sách 12 bảng đã tạo
SELECT name AS TableName
FROM sys.tables
WHERE name IN (
    N'GiangVien', N'TaiKhoan', N'HocVien', N'MonHoc', N'KhoaHoc', N'KhoaHocMonHoc', N'PhongHoc',
    N'LopHoc', N'DangKyLopHoc', N'LichHoc', N'KetQuaHocTap', N'PhieuThu'
)
ORDER BY name;

-- 2. Kiểm tra số lượng bản ghi seed trong từng bảng
SELECT N'GiangVien' AS Bang, COUNT(*) AS SoBanGhi FROM dbo.GiangVien
UNION ALL
SELECT N'TaiKhoan', COUNT(*) FROM dbo.TaiKhoan
UNION ALL
SELECT N'HocVien', COUNT(*) FROM dbo.HocVien
UNION ALL
SELECT N'MonHoc', COUNT(*) FROM dbo.MonHoc
UNION ALL
SELECT N'KhoaHoc', COUNT(*) FROM dbo.KhoaHoc
UNION ALL
SELECT N'KhoaHocMonHoc', COUNT(*) FROM dbo.KhoaHocMonHoc
UNION ALL
SELECT N'PhongHoc', COUNT(*) FROM dbo.PhongHoc
UNION ALL
SELECT N'LopHoc', COUNT(*) FROM dbo.LopHoc
UNION ALL
SELECT N'DangKyLopHoc', COUNT(*) FROM dbo.DangKyLopHoc
UNION ALL
SELECT N'LichHoc', COUNT(*) FROM dbo.LichHoc
UNION ALL
SELECT N'KetQuaHocTap', COUNT(*) FROM dbo.KetQuaHocTap
UNION ALL
SELECT N'PhieuThu', COUNT(*) FROM dbo.PhieuThu;

-- 3. Kiểm tra danh sách 6 tài khoản theo vai trò
SELECT Id, TenDangNhap, VaiTro, GiangVienId, TrangThai
FROM dbo.TaiKhoan
ORDER BY Id;
```

---

## 5. Cách sử dụng Windows Authentication

Ưu tiên sử dụng **Windows Authentication** (`Trusted_Connection=True` hoặc `Integrated Security=True`) để không phải lưu trữ tài khoản và mật khẩu SQL Server trong file cấu hình:

- Với SQL Server Express (khuyến nghị trên máy phát triển):
  ```text
  Server=.\SQLEXPRESS;Database=QLHV;Trusted_Connection=True;TrustServerCertificate=True
  ```
- Với SQL Server mặc định trên máy cục bộ:
  ```text
  Server=.;Database=QLHV;Trusted_Connection=True;TrustServerCertificate=True
  ```
- Với SQL Server LocalDB:
  ```text
  Server=(localdb)\MSSQLLocalDB;Database=QLHV;Trusted_Connection=True;TrustServerCertificate=True
  ```

---

## 6. Phân biệt Database ứng dụng và Database kiểm thử tích hợp

Hệ thống phân định rạch ròi giữa database chạy ứng dụng chính và database kiểm thử tích hợp:

- **`QLHV`**: Database chạy ứng dụng Windows Forms và phục vụ người dùng.
- **`QLHV_Test`**: Database chạy bộ kiểm thử tích hợp tự động (`SqlServerIntegrationTests`).

```sql
IF DB_ID(N'QLHV_Test') IS NULL
BEGIN
    CREATE DATABASE QLHV_Test COLLATE Vietnamese_100_CI_AI;
END;
GO
```

Sau đó chạy toàn bộ 7 script vào `QLHV_Test`:

```powershell
sqlcmd -S .\SQLEXPRESS -d QLHV_Test -E -C -i database\001_schema.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV_Test -E -C -i database\002_seed.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV_Test -E -C -i database\003_operations_schema.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV_Test -E -C -i database\004_operations_seed.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV_Test -E -C -i database\005_operations_hardening.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV_Test -E -C -i database\006_student_profile_and_class_advisor.sql -b
sqlcmd -S .\SQLEXPRESS -d QLHV_Test -E -C -i database\007_unicode_data_repair.sql -b
```

> [!NOTE]
> **Lưu ý kiến trúc về `dbo.LopHoc.GiangVienChuNhiemId`:**
> Tại tầng CSDL SQL Server, cột `GiangVienChuNhiemId` được thiết kế kiểu `INT NULL` (cho phép NULL) nhằm đảm bảo tính tương thích ngược với các bản ghi lớp học cũ trước khi chạy migration và tạo thuận lợi cho việc backfill dữ liệu. Tuy nhiên, tại tầng nghiệp vụ (`LopHocService`), hệ thống áp dụng quy tắc nghiệp vụ nghiêm ngặt bắt buộc mọi thao tác tạo mới hoặc cập nhật lớp học đều phải có Giảng viên chủ nhiệm đang hoạt động (`TrangThai = DangHoatDong`), từ chối lưu và trả về mã lỗi nếu thiếu hoặc giảng viên không hợp lệ.

---

## 7. Quy tắc an toàn và cảnh báo bắt buộc

- **Phân tách môi trường kiểm thử:** Bộ test tự động chỉ thao tác trên `QLHV_Test`. Ứng dụng WinForms tuyệt đối không kết nối vào `QLHV_Test`.
- **Cảnh báo dữ liệu mẫu (Seed):** File `002_seed.sql` **chỉ dành riêng cho mục đích demo và kiểm thử nội bộ**. Tuyệt đối không chạy script dữ liệu mẫu này lên database sản xuất (production). Khi đưa vào triển khai thật, bắt buộc phải xóa bỏ hoặc thay đổi mật khẩu toàn bộ các tài khoản demo (`admin`, `daotao`, `thungan`, `giangvien`, `quanly`, `bikhoa`).
- **Cấu hình chứng chỉ kết nối:** Thuộc tính `TrustServerCertificate=True` trong chuỗi kết nối mẫu chỉ dành riêng cho môi trường phát triển / LocalDB cục bộ, tuyệt đối không áp dụng trên môi trường production nếu không có chứng chỉ TLS/SSL hợp lệ.
- **Bảo mật chuỗi kết nối:** Tuyệt đối không đưa chuỗi kết nối thật (connection string chứa server nội bộ, mật khẩu SQL Server thật) lên Git. Sử dụng file `appsettings.Local.json` (đã được cấu hình trong `.gitignore`) cho các thiết lập cục bộ.
