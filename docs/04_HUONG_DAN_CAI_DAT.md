# HƯỚNG DẪN CÀI ĐẶT VÀ TRIỂN KHAI HỆ THỐNG QLHV

Tài liệu này hướng dẫn chi tiết quy trình cài đặt môi trường, tạo cơ sở dữ liệu, cấu hình kết nối và chạy ứng dụng Quản lý học viên (**QLHV**).

---

## 1. Yêu Cầu Môi Trường và Phần Mềm

Để cài đặt và vận hành hệ thống, máy tính cần đáp ứng các điều kiện tiên quyết sau:

1. **Hệ điều hành**:
   - Windows 10/11 (64-bit) hoặc Windows Server 2019/2022.
2. **.NET SDK**:
   - .NET 10.0 SDK trở lên (kiểm tra bằng lệnh: `dotnet --version`).
3. **Cơ sở dữ liệu**:
   - Microsoft SQL Server 2019 / 2022 hoặc **Microsoft SQL Server Express** (`.\SQLEXPRESS`).
   - Dịch vụ MSSQL Server đang ở trạng thái **Running** (Dịch vụ SQL Server Browser chỉ cần thiết khi kết nối từ xa qua named instance, không bắt buộc đối với kết nối cục bộ).
   - Hỗ trợ xác thực Windows Authentication hoặc SQL Server Authentication.
4. **Công cụ quản trị/phát triển**:
   - Visual Studio 2022 (version 17.12 trở lên) hoặc VS Code / JetBrains Rider.
   - SQL Server Management Studio (SSMS) hoặc công cụ dòng lệnh `sqlcmd`.

---

## 2. Thiết Lập Cơ Sở Dữ Liệu SQL Server

Hệ thống sử dụng hai cơ sở dữ liệu:
- `QLHV`: Cơ sở dữ liệu chính phục vụ vận hành ứng dụng.
- `QLHV_Test`: Cơ sở dữ liệu cô lập dành riêng cho kiểm thử tự động (Integration Tests).

Các script được tổ chức trong thư mục `database/`:
- `001_schema.sql`: Khởi tạo các bảng danh mục cốt lõi (Học viên, Giảng viên, Khóa học, Môn học, Phòng học, Người dùng, Vai trò).
- `002_seed.sql`: Dữ liệu mẫu danh mục, tài khoản quản trị và nhân sự ban đầu.
- `003_operations_schema.sql`: Khởi tạo các bảng phân hệ đào tạo (Lớp học, Đăng ký lớp, Lịch học, Điểm số, Phiếu thu).
- `004_operations_seed.sql`: Dữ liệu mẫu hoạt động đào tạo, lớp mở, phân công lịch, ghi nhận học phí và điểm.
- `005_operations_hardening.sql`: Cột kiểm toán hủy phiếu thu (`NgayHuy`, `NguoiHuyTaiKhoanId`) và các index kiểm tra tranh chấp lịch học và đồng thời.
- `006_student_profile_and_class_advisor.sql`: Mở rộng 11 trường hồ sơ học viên trong `dbo.HocVien`, filtered unique index trên CCCD, và bổ sung Giảng viên chủ nhiệm `GiangVienChuNhiemId` cho `dbo.LopHoc`.
- `007_unicode_data_repair.sql`: Sửa chữa toàn diện lỗi mã hóa tiếng Việt (mojibake) trên các bảng dữ liệu vận hành.

### 2.1 Cài đặt bằng `sqlcmd` (Dòng lệnh PowerShell)

Mở PowerShell với quyền Administrator (hoặc user hiện tại có quyền sysadmin trên SQL Server):

```powershell
# 1. Di chuyển vào thư mục dự án
cd D:\HMNU\Laptrinhwindow\QLHV

# 2. Tạo database QLHV và QLHV_Test
sqlcmd -S ".\SQLEXPRESS" -E -C -Q "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name='QLHV') CREATE DATABASE QLHV;"
sqlcmd -S ".\SQLEXPRESS" -E -C -Q "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name='QLHV_Test') CREATE DATABASE QLHV_Test;"

# 3. Nạp Schema và Seed cho Database chính QLHV
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV -i "database\001_schema.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV -i "database\002_seed.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV -i "database\003_operations_schema.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV -i "database\004_operations_seed.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV -i "database\005_operations_hardening.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV -i "database\006_student_profile_and_class_advisor.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV -i "database\007_unicode_data_repair.sql"

# 4. Nạp Schema và Seed cho Database Test QLHV_Test
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV_Test -i "database\001_schema.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV_Test -i "database\002_seed.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV_Test -i "database\003_operations_schema.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV_Test -i "database\004_operations_seed.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV_Test -i "database\005_operations_hardening.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV_Test -i "database\006_student_profile_and_class_advisor.sql"
sqlcmd -S ".\SQLEXPRESS" -E -C -d QLHV_Test -i "database\007_unicode_data_repair.sql"
```

### 2.2 Cài đặt bằng SSMS (Giao diện trực quan)
1. Mở SSMS, kết nối tới Server Name: `.\SQLEXPRESS` (hoặc `localhost\SQLEXPRESS`).
2. Mở file `database/001_schema.sql`, chọn database đích `QLHV` và nhấn **Execute (F5)**.
3. Lần lượt mở và thực thi các file `002_seed.sql`, `003_operations_schema.sql`, `004_operations_seed.sql`, `005_operations_hardening.sql`, `006_student_profile_and_class_advisor.sql`, `007_unicode_data_repair.sql`.
4. Lặp lại tương tự cho database `QLHV_Test`.

---

## 3. Cấu Hình Ứng Dụng (`appsettings.json`)

Tệp cấu hình chính nằm tại: `src/QLHV.WinForms/appsettings.json`.

```json
{
  "DataProvider": "SqlServer",
  "ConnectionStrings": {
    "QLHV": "Server=.\\SQLEXPRESS;Database=QLHV;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;Connect Timeout=5"
  }
}
```

### Chế độ hoạt động (DataProvider):
- **`SqlServer`** (Mặc định): Kết nối tới SQL Server thật qua ADO.NET thuần. Khuyến nghị cho kiểm thử thực tế và nghiệm thu.
- **`Fake`**: Chạy bằng dữ liệu bộ nhớ (`FakeTrainingStore` và InMemory Repository). Không phụ thuộc SQL Server, thích hợp khi demo trên máy không cài đặt database engine.

> **Lưu ý bảo mật:** Hệ thống có cơ chế tự động che giấu mật khẩu (Sanitize) trong file log, không bao giờ ghi chuỗi kết nối chứa mật khẩu rõ ra file log.

---

## 4. Biên Dịch và Khởi Chạy Hệ Thống

### 4.1 Biên dịch toàn bộ Solution
```powershell
dotnet build QLHV.sln -c Debug
```

### 4.2 Chạy bộ kiểm thử tự động (Unit & Integration Tests)
```powershell
dotnet test QLHV.sln -c Debug --no-build --logger "console;verbosity=normal"
```
Kết quả mong đợi: **164 passed** (138 non-SQL + 26 SQL Server Integration tests), 0 failed, 0 skipped.

### 4.3 Khởi chạy ứng dụng WinForms
```powershell
dotnet run --project src\QLHV.WinForms\QLHV.WinForms.csproj
```

---

## 5. Tài Khoản Đăng Nhập Mặc Định

| Tài khoản | Mật khẩu | Họ và tên | Vai trò | Quyền hạn chính |
| :--- | :--- | :--- | :--- | :--- |
| `admin` | `Admin@123` | Quản Trị Viên Hệ Thống | `ADMIN` | Toàn quyền toàn bộ các phân hệ |
| `daotao` | `DaoTao@123` | Chuyên Viên Đào Tạo | `DAOTAO` | Quản lý danh mục, mở lớp, xếp học viên, lập lịch học (không nhập điểm hoặc thu tiền) |
| `thungan` | `ThuNgan@123` | Nhân Viên Thu Ngân | `THUNGAN` | Quản lý học phí, lập phiếu thu, hủy phiếu thu (không có quyền Đào tạo, Điểm, Báo cáo) |
| `giangvien` | `GiangVien@123` | ThS. Nguyễn Văn A | `GIANGVIEN` | Tra cứu lớp phụ trách, nhập điểm số học viên cho lớp mình dạy (không có quyền Đào tạo hay Báo cáo) |
| `quanly` | `QuanLy@123` | Ban Quản Lý Đào Tạo | `QUANLY` | Xem toàn bộ báo cáo thống kê, xem danh mục (không thao tác sửa đổi danh mục, Đào tạo, Điểm hay Thu học phí) |

---

## 6. Đóng Gói và Triển Khai (Release Publish)

Để phát hành phiên bản chạy độc lập trên Windows x64:
```powershell
dotnet publish src\QLHV.WinForms\QLHV.WinForms.csproj -c Release -r win-x64 --self-contained false -o .\dist\QLHV_Release
```
Thư mục `.\dist\QLHV_Release` sẽ chứa file thực thi `QLHV.WinForms.exe` cùng toàn bộ runtime dll và file cấu hình sẵn sàng đưa vào sử dụng.
