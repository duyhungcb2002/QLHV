# Hướng Dẫn Cơ Sở Dữ Liệu SQL Server (QLHV)

> **Trạng thái kiểm chứng hiện tại:**
> Các script trong thư mục này (`001_schema.sql` và `002_seed.sql`) thuộc giai đoạn **SQL-01A**, đã được rà soát tĩnh (static review) đối chiếu với DTO và Repository Interface hiện có.
> Máy phát triển hiện tại **chưa cài đặt SQL Server / LocalDB / `sqlcmd`**, do đó các script **chưa được kiểm chứng runtime trên SQL Server thực tế**.

---

## 1. Yêu cầu môi trường SQL Server

- **Hệ quản trị CSDL:** Microsoft SQL Server 2019 / 2022 trở lên (Developer, Express) hoặc SQL Server Express LocalDB.
- **Công cụ thực thi:** SQL Server Management Studio (SSMS), Azure Data Studio hoặc công cụ dòng lệnh `sqlcmd`.
- **Bảng mã / Collation khuyến nghị:** `Vietnamese_100_CI_AI` (hỗ trợ tiếng Việt đầy đủ, không phân biệt hoa/thường, không phân biệt dấu khi tìm kiếm).

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

Luôn chạy các script theo đúng thứ tự sau trên database `QLHV`:

1. **`001_schema.sql`** — Khởi tạo 7 bảng phục vụ Đăng nhập và 6 Danh mục hiện có (`GiangVien`, `TaiKhoan`, `HocVien`, `MonHoc`, `KhoaHoc`, `KhoaHocMonHoc`, `PhongHoc`), kèm theo khóa chính, khóa ngoại, unique constraint, check constraint và index. Lưu ý: Đây là script khởi tạo database mới, không phải migration. Nếu database hoặc bảng đã tồn tại từ schema cũ thì việc chạy lại script không tự động đổi collation. Không tạo migration trong lượt này.
2. **`002_seed.sql`** — Nạp dữ liệu mẫu idempotent cho 6 danh mục và 6 tài khoản demo: 5 vai trò hoạt động và 1 tài khoản bị khóa (`ADMIN`, `DAOTAO`, `THUNGAN`, `GIANGVIEN`, `QUANLY`, `bikhoa`). Mật khẩu được lưu dưới dạng băm PBKDF2-SHA256 tương thích với `Pbkdf2PasswordHasher`.

Ví dụ chạy bằng `sqlcmd` với Windows Authentication:

```powershell
sqlcmd -S localhost -d QLHV -E -i database\001_schema.sql -b
sqlcmd -S localhost -d QLHV -E -i database\002_seed.sql -b
```

---

## 4. Cách kiểm tra bảng và dữ liệu sau khi chạy

Chạy truy vấn kiểm tra nhanh trên database `QLHV`:

```sql
USE QLHV;
GO

-- 1. Kiểm tra danh sách 7 bảng đã tạo
SELECT name AS TableName
FROM sys.tables
WHERE name IN (N'GiangVien', N'TaiKhoan', N'HocVien', N'MonHoc', N'KhoaHoc', N'KhoaHocMonHoc', N'PhongHoc')
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
SELECT N'PhongHoc', COUNT(*) FROM dbo.PhongHoc;

-- 3. Kiểm tra danh sách 6 tài khoản theo vai trò
SELECT Id, TenDangNhap, VaiTro, GiangVienId, TrangThai
FROM dbo.TaiKhoan
ORDER BY Id;
```

---

## 5. Cách sử dụng Windows Authentication

Ưu tiên sử dụng **Windows Authentication** (`Trusted_Connection=True` hoặc `Integrated Security=True`) để không phải lưu trữ tài khoản và mật khẩu SQL Server trong file cấu hình:

- Với SQL Server mặc định trên máy cục bộ:
  ```text
  Server=localhost;Database=QLHV;Trusted_Connection=True;TrustServerCertificate=True
  ```
- Với SQL Server Express:
  ```text
  Server=localhost\SQLEXPRESS;Database=QLHV;Trusted_Connection=True;TrustServerCertificate=True
  ```
- Với SQL Server LocalDB:
  ```text
  Server=(localdb)\MSSQLLocalDB;Database=QLHV;Trusted_Connection=True;TrustServerCertificate=True
  ```

---

## 6. Cách chuẩn bị Database Test riêng

Khi thực hiện kiểm thử tích hợp (Integration Test ở giai đoạn sau), luôn tách biệt database kiểm thử khỏi database demo:

```sql
IF DB_ID(N'QLHV_Test') IS NULL
BEGIN
    CREATE DATABASE QLHV_Test COLLATE Vietnamese_100_CI_AI;
END;
GO
```

Sau đó chạy `001_schema.sql` và `002_seed.sql` vào `QLHV_Test`:

```powershell
sqlcmd -S localhost -d QLHV_Test -E -i database\001_schema.sql -b
sqlcmd -S localhost -d QLHV_Test -E -i database\002_seed.sql -b
```

---

## 7. Quy tắc an toàn và cảnh báo bắt buộc

- **Cảnh báo môi trường:** Schema và dữ liệu mẫu trong thư mục này mới chỉ hoàn thành bước **static review** (rà soát cú pháp và đối chiếu DTO), **chưa được chạy trên SQL Server thật** do máy phát triển chưa cài đặt SQL Server / LocalDB.
- **Cảnh báo dữ liệu mẫu (Seed):** File `002_seed.sql` **chỉ dành riêng cho mục đích demo và kiểm thử nội bộ**. Tuyệt đối không chạy script dữ liệu mẫu này lên database sản xuất (production). Khi đưa vào triển khai thật, bắt buộc phải xóa bỏ hoặc thay đổi mật khẩu toàn bộ các tài khoản demo (`admin`, `daotao`, `thungan`, `giangvien`, `quanly`, `bikhoa`).
- **Cấu hình chứng chỉ kết nối:** Thuộc tính `TrustServerCertificate=True` trong chuỗi kết nối mẫu chỉ dành riêng cho môi trường phát triển / LocalDB cục bộ, tuyệt đối không áp dụng trên môi trường production nếu không có chứng chỉ TLS/SSL hợp lệ.
- **Bảo mật chuỗi kết nối:** Tuyệt đối không đưa chuỗi kết nối thật (connection string chứa server nội bộ, mật khẩu SQL Server thật) lên Git. Sử dụng file `appsettings.Local.json` (đã được cấu hình trong `.gitignore`) cho các thiết lập cục bộ.
