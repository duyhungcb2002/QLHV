# Hệ Thống Quản Lý Học Viên (QLHV)

> **Môn học:** Lập trình Windows (C# Windows Forms)
> **Thành viên phụ trách Core / Kiến trúc:** Duy Hưng
> **Phiên bản:** 2.0.0 (Mega MVP - Đầy đủ các phân hệ Lớp học, Lịch học, Điểm, Học phí, Báo cáo & Hỗ trợ kép SQL Server / Fake Data)

---

## 1. Giới thiệu hệ thống

**Hệ thống Quản lý học viên (QLHV)** là phần mềm máy tính (Desktop App) phục vụ công tác quản lý tuyển sinh, quản lý danh mục đào tạo, phân công giảng dạy, theo dõi lớp học, quản lý điểm và học phí cho các trung tâm đào tạo / cơ sở giáo dục.

Dự án được xây dựng với kiến trúc hướng dịch vụ nhiều tầng (Layered Architecture), tuân thủ nguyên tắc trách nhiệm duy nhất (Single Responsibility Principle), đảm bảo dễ bảo trì, dễ mở rộng và rất trực quan cho sinh viên học tập lập trình Windows Forms với C#.

---

## 2. Phạm vi đã hoàn thành trong giai đoạn hiện tại (Duy Hưng)

- **Cấu trúc Solution chuẩn mực**: Tách biệt hoàn toàn Contracts, Core, Features, Infrastructure (SqlServer) và Presentation (WinForms).
- **Hệ thống Contracts dùng chung**: DTOs, Enums, Filters, mã lỗi chuẩn hóa (`OperationResult`, `ErrorCodes`).
- **Xác thực & Phân quyền 2 lớp (Two-Layer Authorization)**:
  - Hashing mật khẩu bằng PBKDF2 (`Rfc2898DeriveBytes.Pbkdf2`) an toàn.
  - Ma trận phân quyền theo vai trò (`IAuthorizationService`).
  - Quản lý phiên đăng nhập người dùng (`ICurrentUserService`).
  - Phân quyền chặt chẽ 2 lớp: kiểm tra ở tầng Presentation (ẩn/khóa menu, nút bấm trong WinForms) và kiểm tra bắt buộc tại tầng Service (`AuthorizationGuard` kiểm tra `CatalogView` và `CatalogManage`), không gọi repository khi thiếu quyền.
- **Giao diện Windows Forms hoàn chỉnh**:
  - `FrmLogin`: Đăng nhập, kiểm tra tài khoản, phân biệt mật khẩu sai và tài khoản bị khóa, phím tắt Enter.
  - `FrmMain`: Màn hình chính hiện đại, điều hướng mượt mà, menu tự động ẩn/khóa theo quyền, chống mở lặp cửa sổ, hiển thị rõ badge môi trường chạy (`Môi trường: SQL Server` hoặc `Môi trường: Fake Data`).
  - `FrmHocVien` & `FrmHocVienEdit`: Quản lý danh mục học viên (Tìm kiếm không dấu/có dấu, Thêm, Sửa, Ngừng hoạt động).
  - `FrmGiangVien` & `FrmGiangVienEdit`: Quản lý danh mục giảng viên.
  - `FrmMonHoc` & `FrmMonHocEdit`: Quản lý danh mục môn học.
  - `FrmKhoaHoc` & `FrmKhoaHocEdit`: Quản lý danh mục khóa học & học phí niêm yết.
  - `FrmKhoaHocMonHoc` & `FrmKhoaHocMonHocEdit`: Quản lý danh sách môn học thuộc từng khóa đào tạo.
  - `FrmPhongHoc` & `FrmPhongHocEdit`: Quản lý danh mục phòng học & sức chứa.
  - `FrmBaoCaoMau`: Mẫu giao diện báo cáo chuẩn (dùng lại được) sẵn sàng cho Chu Đức.
- **Logging an toàn & Che giấu thông tin nhạy cảm**: `FileAppLogger` tự động ghi log vào thư mục `logs/`, làm sạch toàn diện thông tin nhạy cảm (`Password`, `Pwd`, `User ID`, `UID`, `Connection String`) thành `[REDACTED]` trong cả message chính, exception, inner exceptions và stack trace.
- **Hệ thống Kiểm thử tự động (Unit & Integration Tests)**: 164/164 bài test MSTest (100% Passed, gồm 138 non-SQL + 26 SQL Integration Tests) kiểm thử tự động Auth, WinForms UI Smoke, Bảo vệ ma trận quyền & phiên đăng nhập (`SEC-01`), Hạ tầng `SqlConnectionFactory` (`SQL-01A`), 13 SQL Repositories ADO.NET (`SQL-01B1` & Mega MVP), Toàn bộ Services phân hệ Đào tạo, Xếp lớp, Điểm, Học phí và Báo cáo, Mở rộng hồ sơ học viên 11 trường & giảng viên chủ nhiệm lớp học, Cấu hình đa chế độ & Composition Root WinForms, Khởi động an toàn, Lọc log bảo mật.
- **Cập nhật giai đoạn `Mega MVP Implementation & Student Profile 05` (Hoàn thành trọn gói toàn bộ phân hệ nghiệp vụ)**:
  - **Mở rộng hồ sơ học viên & Giảng viên chủ nhiệm (`STUDENT-PROFILE-AND-TRAINING-UI-05` & `FINAL-AUDIT-05C`)**: Mở rộng 11 trường hồ sơ học viên trong `dbo.HocVien` (Số định danh, ngày cấp, nơi cấp, người giám hộ, nhân viên phụ trách, nguồn tuyển sinh, nghề nghiệp, đơn vị công tác, ghi chú đặc biệt), form sửa `FrmHocVienEdit` thiết kế 5 tab hiện đại kèm đếm ký tự ghi chú, form chi tiết `FrmHocVienDetail` xem lịch sử học tập (tra cứu qua `DangKyLopHoc`), che mờ số định danh CCCD/CMND dạng `********1234` (giữ 4 chữ số cuối) và DTO trả về tầng giao diện đã được che đối với tài khoản không có quyền `CatalogManage` (chỉ tài khoản có quyền `CatalogManage` mới được xem và chuyển đổi hiển thị số đầy đủ), và bổ sung Giảng viên chủ nhiệm cho Lớp học (`dbo.LopHoc.GiangVienChuNhiemId` - ở tầng schema CSDL cho phép null nhằm tương thích ngược với dữ liệu cũ, nhưng tại tầng Service `LopHocService` áp dụng quy tắc nghiệp vụ bắt buộc phải có giảng viên chủ nhiệm đang hoạt động khi tạo mới hoặc cập nhật).
  - **Chuẩn hóa giao diện Đào tạo (`FrmDaoTao`)**: Thiết lập tường minh các cột cho 3 DataGridView (`dgvLopHoc`, `dgvDangKy`, `dgvLichHoc`), tắt `AutoGenerateColumns = false`, tiêu đề tiếng Việt chuẩn, căn lề và định dạng ngày tháng/tiền tệ chuyên nghiệp.
  - **Sửa triệt để lỗi tiếng Việt (Mojibake)**: Áp dụng `007_unicode_data_repair.sql` chuẩn hóa an toàn dữ liệu mẫu nghiệp vụ bằng collation nhị phân (`COLLATE Latin1_General_BIN`), không dùng wildcard rộng để tránh ghi đè dữ liệu tiếng Việt hợp lệ, kèm UTF-8 with BOM (`.editorconfig`) cho tất cả các script SQL.
  - **Phân hệ Đào tạo & Lớp học (`QLHV.Features.Training`)**: Quản lý Lớp học (`FrmDaoTao` - Tab Lớp), Xếp học viên vào lớp kèm lưu vết snapshot học phí (`FrmChonHocVien`), Quản lý Lịch học kiểm tra xung đột trùng phòng và trùng giảng viên (`FrmLichHocEdit`).
  - **Phân hệ Kết quả học tập (`FrmDiem`)**: Quản lý điểm tổng kết, tự động xếp loại Đạt / Không đạt (ngưỡng 5.00), kiểm tra phân công giảng viên và ràng buộc môn học thuộc khóa.
  - **Phân hệ Học phí & Phiếu thu (`FrmHocPhi`, `FrmThuHocPhi`)**: Quản lý công nợ, thu học phí ngăn chặn thu vượt công nợ, lập phiếu thu và hủy phiếu thu an toàn có lưu lý do hủy (Soft-Cancel audit).
  - **Phân hệ Báo cáo & Thống kê (`FrmBaoCao`)**: 5 tab báo cáo chuyên sâu (Học viên theo lớp, Lịch học theo lớp, Lịch giảng dạy theo giảng viên, Báo cáo kết quả học tập & Thống kê tỷ lệ môn học, Báo cáo công nợ học phí theo lớp).
  - **Bộ tài liệu kỹ thuật hoàn chỉnh**: Đầy đủ tài liệu khảo sát yêu cầu, phân tích thiết kế, từ điển dữ liệu & sơ đồ ERD, hướng dẫn cài đặt môi trường, hướng dẫn sử dụng và bộ kịch bản kiểm thử nghiệm thu UAT (`docs/` và `qa/`).
  - **Trạng thái nghiệm thu**: 164 bài test tự động PASSED (138 non-SQL + 26 SQL Server Integration Tests). Kiểm thử thủ công UI ghi nhận trạng thái chuẩn hóa: `MANUAL_UI_UAT_NOT_VERIFIED`.
  - **Trạng thái cảnh báo**: 0 cảnh báo trình biên dịch, 0 cảnh báo MSTest analyzer (cảnh báo `NU1900` của NuGet Audit chỉ xuất hiện khi chạy máy cô lập offline không có kết nối internet ra ngoài).

---

## 3. Công nghệ sử dụng

- **Ngôn ngữ:** C# 14 / C# Latest
- **Nền tảng giao diện:** Windows Forms (WinForms)
- **Framework:** .NET 10 (`net10.0-windows` cho WinForms, `net10.0` cho thư viện)
- **Kiến trúc:** Layered Architecture, Inversion of Control qua Constructor Injection
- **Bảo mật:** PBKDF2 Password Hashing (chuẩn NIST/FIPS tích hợp trong .NET Cryptography)
- **Testing:** MSTest 4.0.2

---

## 4. Cấu trúc Solution

```text
QLHV/
├── QLHV.sln
├── database/                     # Script CSDL SQL Server (001 đến 007, README.md)
├── src/
│   ├── QLHV.Contracts/           # Enums, DTOs, Filters, OperationResult, ErrorCodes
│   ├── QLHV.Core/                # Abstractions, Repositories/Services Interfaces, Security, Logging, Validation
│   ├── QLHV.Infrastructure.SqlServer/ # SqlConnectionFactory, SqlServerOptions & 13 SQL Repositories ADO.NET
│   ├── QLHV.Features.Catalog/    # In-memory fake repositories & Services danh mục
│   ├── QLHV.Features.Training/   # Nghiệp vụ Đào tạo, Lớp học, Lịch học, Điểm, Học phí, Báo cáo (Fake & Services)
│   └── QLHV.WinForms/            # Giao diện Forms, UserControls, Composition Root (Program.cs), appsettings.json
├── tests/
│   └── QLHV.Tests/               # Bộ kiểm thử đơn vị & tích hợp MSTest (164 tests passed)
├── docs/                         # Hồ sơ phân tích, thiết kế, kiến trúc, cài đặt & hướng dẫn sử dụng
├── qa/                           # Kịch bản kiểm thử nghiệm thu UAT (MANUAL_UI_UAT_NOT_VERIFIED)
├── README.md                     # Tài liệu tổng quan và hướng dẫn chạy hệ thống
├── CONTRIBUTING.md               # Quy định làm việc nhóm và quy ước nhánh Git
├── INTEGRATION_NOTES.md          # Ghi chú kỹ thuật hướng dẫn tích hợp cho Bá Hiệp & Chu Đức
└── .gitignore                    # Bỏ qua bin/, obj/, .vs/, logs/, secrets...
```

---

## 5. Sơ đồ phụ thuộc giữa các project (Dependency Graph)

```
        ┌───────────────────┐
        │   QLHV.Contracts  │ (Không phụ thuộc project nào)
        └─────────▲─────────┘
                  │
        ┌─────────┴─────────┐
        │     QLHV.Core     │ ────────┬─────────────────────────┐
        └─────────▲─────────┘         │                         │
                  │                   │                         │
        ┌─────────┴─────────┐         │               ┌─────────┴─────────┐
        │QLHV.Features.Cat..│         │               │QLHV.Infrastructure│
        └─────────▲─────────┘         │               │     .SqlServer    │
                  │                   │               └─────────▲─────────┘
                  │                   │                         │
        ┌─────────┴───────────────────┴─────────────────────────┴─────────┐
        │                          QLHV.WinForms                          │
        └─────────────────────────────────────────────────────────────────┘
```

- Không có vòng phụ thuộc (No circular dependencies).
- Các Form chỉ gọi Service, không chứa câu lệnh SQL hoặc kết nối CSDL trực tiếp.
- Lớp Configuration/Composition trong WinForms (`StartupConnectionTester`) thực hiện kiểm tra kết nối SQL Server lúc khởi động.
- Toàn bộ giao diện chỉ giao tiếp qua Service Interfaces.

---

## 6. Cách mở Solution

1. Sử dụng **Visual Studio 2026** (hoặc Visual Studio hỗ trợ .NET 10).
2. Chọn `File` -> `Open` -> `Project/Solution...`
3. Điều hướng đến thư mục `D:\HMNU\Laptrinhwindow\QLHV` và chọn file `QLHV.sln`.

---

## 7. Cách Build Solution

### Sử dụng giao diện Visual Studio:
- Nhấn `Ctrl + Shift + B` hoặc chọn `Build` -> `Build Solution`.

### Sử dụng lệnh qua Terminal / PowerShell:
```powershell
cd D:\HMNU\Laptrinhwindow\QLHV
dotnet build QLHV.sln -c Debug
# Hoặc build Release:
dotnet build QLHV.sln -c Release
```

---

## 8. Cách Chạy ứng dụng

### Sử dụng Visual Studio:
- Đặt `QLHV.WinForms` làm Startup Project (Chuột phải vào project -> `Set as Startup Project`).
- Nhấn `F5` (Debug) hoặc `Ctrl + F5` (Start Without Debugging).

### Sử dụng dòng lệnh Terminal:
```powershell
cd D:\HMNU\Laptrinhwindow\QLHV
dotnet run --project src\QLHV.WinForms\QLHV.WinForms.csproj
```

---

## 9. Cách Chạy Bộ Kiểm thử (Unit & Integration Tests)

### Sử dụng Visual Studio:
- Mở `Test Explorer` (`Test` -> `Test Explorer`), nhấn nút `Run All Tests`.

### Sử dụng dòng lệnh:
```powershell
cd D:\HMNU\Laptrinhwindow\QLHV
dotnet test tests\QLHV.Tests\QLHV.Tests.csproj
```
Kết quả chạy: **164/164 tests PASSED (100%)** (gồm 138 non-SQL + 26 SQL Integration Tests trên database `QLHV_Test`, Auth, WinForms UI Smoke toàn bộ các Form, Catalog Validation & CRUD, Service Authorization, Session & Matrix immutability, SqlConnectionFactory, AppConfig/Provider wiring, FileAppLogger Sanitization, và toàn bộ Nghiệp vụ Đào tạo / Điểm / Học phí / Báo cáo / Hồ sơ học viên / Giảng viên chủ nhiệm).

> [!IMPORTANT]
> **Yêu cầu đối với 26 bài SQL Integration Tests:**
> - Cần có Microsoft SQL Server Express (hoặc LocalDB) đang chạy.
> - Cần chuẩn bị trước cơ sở dữ liệu kiểm thử riêng biệt **`QLHV_Test`** (đã chạy toàn bộ 7 script trong `database/`). Tuyệt đối không chạy integration tests trên database ứng dụng `QLHV`.
> - Mặc định bộ test sử dụng Data Source: `Server=.\SQLEXPRESS;Database=QLHV_Test;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;`.
> - Có thể chỉ định chuỗi kết nối kiểm thử qua biến môi trường `QLHV_TEST_CONNECTION_STRING`:
>   ```powershell
>   $env:QLHV_TEST_CONNECTION_STRING = "Server=.\SQLEXPRESS;Database=QLHV_Test;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;"
>   dotnet test --filter "TestCategory=SqlServer"
>   ```
> - Chi tiết các bước khởi tạo database test và thứ tự chạy script được hướng dẫn đầy đủ tại [database/README.md](database/README.md).

---

## 10. Danh sách Tài khoản Demo

> ⚠️ **CẢNH BÁO QUAN TRỌNG:**
> Các tài khoản và mật khẩu dưới đây **chỉ dùng cho môi trường demo / kiểm thử nội bộ**, tuyệt đối không sử dụng cho môi trường sản xuất (production). Trong mã nguồn, toàn bộ mật khẩu được băm (hash) bằng thuật toán PBKDF2 trước khi lưu vào repository.

| Tên đăng nhập | Mật khẩu demo | Vai trò | Quyền hạn đặc trưng |
|---|---|---|---|
| **`admin`** | `Admin@123` | **ADMIN** | Toàn bộ quyền: Xem & Quản lý Catalog, Đào tạo, Học phí, Điểm, Báo cáo. |
| **`daotao`** | `DaoTao@123` | **DAOTAO** | Xem & Quản lý Catalog, Xem & Quản lý phân hệ Đào tạo (Lớp học, Xếp lớp, Lịch học). Không có quyền Nhập điểm, Thu học phí, Báo cáo. |
| **`thungan`** | `ThuNgan@123` | **THUNGAN** | Xem danh mục (chỉ đọc), Quản lý phân hệ Học phí & Phiếu thu. Không có quyền Đào tạo, Điểm, Báo cáo. |
| **`giangvien`** | `GiangVien@123` | **GIANGVIEN** | Xem danh mục (chỉ đọc), Nhập điểm số học viên cho các lớp phụ trách. Không có quyền Quản lý Đào tạo hay Báo cáo. |
| **`quanly`** | `QuanLy@123` | **QUANLY** | Xem danh mục (chỉ đọc), Xem báo cáo & thống kê 5 phân hệ. Không có quyền sửa danh mục, Đào tạo, Nhập điểm hay Thu học phí. |
| **`bikhoa`** | `BiKhoa@123` | **DAOTAO** *(Bị khóa)* | Dùng để kiểm thử thông báo lỗi khi tài khoản bị khóa (`ACCOUNT_LOCKED`). |

---

## 11. Chế độ dữ liệu kép: SQL Server (Mặc định) và Fake Data (Demo/Offline)

Ứng dụng hỗ trợ hai cơ chế cung cấp dữ liệu được điều khiển qua cấu hình:

1. **Chế độ `SqlServer` (Mặc định):**
   - Sử dụng 7 SQL Repositories ADO.NET (`SqlTaiKhoanRepository`, `SqlHocVienRepository`, ...) kết nối trực tiếp đến cơ sở dữ liệu `QLHV`.
   - Dữ liệu được lưu trữ bền vững, toàn vẹn ràng buộc và có tính liên tục giữa các phiên chạy.
2. **Chế độ `Fake` (Demo / Offline):**
   - Sử dụng In-Memory Fake Repositories (`FakeTaiKhoanRepository`, `FakeHocVienRepository`, ...).
   - Phục vụ demo nhanh trên máy không cài SQL Server hoặc kiểm thử giao diện độc lập.
3. **Cơ chế chuyển đổi và bảo vệ:**
   - Cấu hình qua thuộc tính `"DataProvider"` trong `appsettings.json`, hoặc ghi đè bằng `appsettings.Local.json` hoặc biến môi trường `QLHV_DATA_PROVIDER`.
   - **Tuyệt đối không fallback ngầm:** Nếu chọn `SqlServer` mà kết nối thất bại, ứng dụng dừng khởi động và báo lỗi rõ ràng, không tự ý chuyển sang `Fake`.

---

## 12. Kiến trúc truy xuất dữ liệu SQL Server (SQL-01B1 & SQL-01B2)

Hạ tầng SQL Server được triển khai đồng bộ qua hai giai đoạn:

1. **7 SQL Repositories ADO.NET (`QLHV.Infrastructure.SqlServer`):**
   - `SqlTaiKhoanRepository : ITaiKhoanRepository`
   - `SqlHocVienRepository : IHocVienRepository`
   - `SqlGiangVienRepository : IGiangVienRepository`
   - `SqlMonHocRepository : IMonHocRepository`
   - `SqlKhoaHocRepository : IKhoaHocRepository`
   - `SqlKhoaHocMonHocRepository : IKhoaHocMonHocRepository`
   - `SqlPhongHocRepository : IPhongHocRepository`
2. **Composition Root & Factory (`Program.cs`):**
   - `RepositoryFactory.CreateRepositories(config, passwordHasher)` khởi tạo tập repository tương ứng theo cấu hình.
   - Các Service Catalog phụ thuộc thuần túy vào Repository Interfaces định nghĩa tại `QLHV.Core.Repositories`.
3. **Kiểm thử tích hợp độc lập:**
   - Bộ 26 bài test `SqlServerIntegrationTests` chạy trên database chuyên biệt `QLHV_Test`, hoàn toàn cách ly với database ứng dụng `QLHV`.

---

## 13. Khởi động an toàn & Bảo vệ thông tin nhạy cảm

1. **Kiểm tra kết nối khi khởi động (`StartupConnectionTester`):**
   - Khi chạy ở chế độ `SqlServer`, ứng dụng kiểm tra kết nối tới `QLHV` trước khi hiển thị `FrmLogin`.
   - Nếu kết nối thất bại: ghi log kỹ thuật đã làm sạch, hiển thị thông báo lỗi thân thiện tiếng Việt và thoát ứng dụng an toàn.
   - Ngăn chặn kết nối nhầm vào database kiểm thử `QLHV_Test`.
2. **Làm sạch thông tin nhạy cảm (`FileAppLogger.Sanitize`):**
   - Tự động thay thế các khóa nhạy cảm (`Password`, `Pwd`, `User ID`, `UID`, `Connection String`) thành `[REDACTED]`.
   - Áp dụng trên toàn bộ thông điệp log, exception message, inner exception và stack trace.
   - Giữ lại thông tin chẩn đoán hữu ích như tên database trong log thông thường.

---

## 14. Bảng trạng thái công việc (DH-01 đến DH-08)

| Mã CV | Nội dung công việc | Trạng thái | Lý do / Ghi chú |
|---|---|---|---|
| **DH-01** | Tạo solution, kiến trúc module, README, convention, form mẫu | **DONE** | Hoàn thành đầy đủ cấu trúc solution, file convention, .gitignore, README. |
| **DH-02** | Shared Contracts & Core, DTO, Interface, OperationResult, ConnectionFactory | **DONE** | Đầy đủ DTOs, Enums, Filters, Result pattern, IConnectionFactory abstraction. |
| **DH-03** | Đăng nhập, phân quyền ma trận, Form chính và điều hướng | **DONE** | FrmLogin, FrmMain, phân quyền tự động ẩn/khóa menu theo quyền người dùng. |
| **DH-04** | CRUD Học viên và Giảng viên | **DONE** | Đầy đủ form danh sách, form nhập liệu, validation, ngừng hoạt động, in-memory repository. |
| **DH-05** | CRUD Môn học, Khóa học, Quan hệ Khóa-Môn, Phòng học | **DONE** | Đầy đủ form danh mục, dialog gán môn học vào khóa, kiểm tra ràng buộc. |
| **DH-06** | Ổn định service dùng chung và tích hợp module của thành viên khác | **DONE** | Tích hợp toàn diện `QLHV.Features.Training`, liên kết toàn bộ phân hệ vào FrmMain. |
| **DH-07** | Kiểm tra quyền ở service, chuẩn hóa lỗi/logging, lọc dữ liệu nhạy cảm và tích hợp SQL Server | **DONE** | 13 SQL Repositories ADO.NET, 164 bài test MSTest (138 non-SQL + 26 SQL), bảo mật log nhạy cảm và RBAC 2 lớp. |
| **DH-08** | Đóng gói, cấu hình đa chế độ (SQL/Fake), release và kiểm tra máy sạch | **DONE** | Cấu hình chế độ kép hoạt động trơn tru; bản build Release độc lập sẵn sàng. |

---

## 15. Ranh giới trách nhiệm của 4 thành viên

1. **Duy Hưng (Team Lead / Core Architecture)**:
   - Kiến trúc giải pháp, Contracts dùng chung, Core Abstractions.
   - Xác thực & Phân quyền tập trung, Điều hướng Form chính.
   - Các danh mục độc lập (Học viên, Giảng viên, Môn học, Khóa học, Khóa-Môn, Phòng học).
   - Logging, chuẩn hóa lỗi, điểm tích hợp.
2. **Bá Hiệp (Training Module)**:
   - Nghiệp vụ Đào tạo: Lớp học, Xếp học viên vào lớp / Đăng ký học, Phân công giảng viên, Lịch học, Điểm thi và Phiếu thu / Học phí.
3. **Chu Đức (Database & Reporting Module)**:
   - Thiết kế CSDL SQL Server chính thức (Schema DDL, migration, seed SQL).
   - Triển khai ADO.NET repositories thay thế cho fake repositories.
   - Chức năng Báo cáo & Thống kê chính thức.
4. **Mai Chi (Documentation & Quality Assurance)**:
   - Tài liệu phân tích thiết kế, tài liệu hướng dẫn sử dụng trong `/docs`.
   - Bộ kịch bản kiểm thử nghiệm thu (UAT) trong `/qa`.

---

## 16. Đầu vào đang cần từ Bá Hiệp và Chu Đức

### Kết quả hoàn thiện toàn bộ phân hệ (Mega MVP Implementation):
- [x] Schema CSDL hoàn chỉnh (`001_schema.sql`, `002_seed.sql`, `003_operations_schema.sql`, `004_operations_seed.sql`, `005_operations_hardening.sql`, `006_student_profile_and_class_advisor.sql`, `007_unicode_data_repair.sql`).
- [x] Triển khai 13 ADO.NET Repositories thực tế theo các interface trong `QLHV.Core.Repositories`.
- [x] Kiểm thử tích hợp SQL trên CSDL thật (`SqlServerIntegrationTests`).
- [x] Nối WinForms với SQL Server và cấu hình đa chế độ (`SqlServer` và `Fake`).
- [x] Các DTOs và Contracts cho module Training (Lớp học, Lịch học, Điểm số, Học phí, Báo cáo).
- [x] Triển khai toàn bộ Services nghiệp vụ đào tạo kế thừa mẫu `OperationResult<T>`.
- [x] Xây dựng đầy đủ các Windows Forms cho phân hệ Lớp học, Điểm số, Học phí, Báo cáo tích hợp vào menu `FrmMain`.
- [x] Bộ tài liệu thiết kế, hướng dẫn cài đặt, sử dụng và kịch bản kiểm thử UAT (`docs/`, `qa/`).
