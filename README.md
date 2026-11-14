# Hệ Thống Quản Lý Học Viên (QLHV)

> **Môn học:** Lập trình Windows (C# Windows Forms)  
> **Thành viên phụ trách Core / Kiến trúc:** Duy Hưng  
> **Phiên bản:** 1.1.0 (Tích hợp SQL Server ADO.NET & Hỗ trợ chế độ Fake Data Demo)

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
- **Hệ thống Kiểm thử tự động (Unit & Integration Tests)**: 103/103 bài test MSTest (100% Passed) bao phủ toàn diện Auth, Phân quyền UI, Bảo vệ ma trận quyền & phiên đăng nhập (`SEC-01`), Hạ tầng `SqlConnectionFactory` (`SQL-01A`), 7 SQL Repositories ADO.NET (`SQL-01B1`), Cấu hình đa chế độ & Composition Root WinForms, Khởi động an toàn, Lọc log bảo mật toàn diện (`SQL-01B2`).
- **Cập nhật giai đoạn `SQL-01B1 + SQL-01B2` (Kết nối WinForms với SQL Server thật)**:
  - **7 SQL Repositories ADO.NET (`SQL-01B1`)**: Đã triển khai đầy đủ `SqlTaiKhoanRepository`, `SqlHocVienRepository`, `SqlGiangVienRepository`, `SqlMonHocRepository`, `SqlKhoaHocRepository`, `SqlKhoaHocMonHocRepository`, `SqlPhongHocRepository` trên project `QLHV.Infrastructure.SqlServer`.
  - **Kết nối WinForms và Cấu hình đa chế độ (`SQL-01B2`)**:
    - Hỗ trợ chế độ dữ liệu kép qua cấu hình: `SqlServer` (mặc định cho ứng dụng) và `Fake` (chế độ demo/offline khi không có SQL Server).
    - Thứ tự ưu tiên cấu hình: Biến môi trường (`QLHV_DATA_PROVIDER`, `QLHV_CONNECTION_STRING`) > `appsettings.Local.json` > `appsettings.json`.
    - Composition root đơn giản, không phụ thuộc framework DI bên ngoài, phù hợp cho sinh viên học tập.
    - Cơ chế Startup Check: Kiểm tra kết nối SQL Server trước khi mở form đăng nhập. Nếu lỗi, hiển thị thông báo thân thiện (đã che thông tin nhạy cảm), ghi log an toàn và dừng khởi động, tuyệt đối không tự ý fallback ngầm về Fake.
    - Khóa bảo vệ database: Nghiêm cấm chạy ứng dụng chính với database kiểm thử `QLHV_Test`. Toàn bộ test tự động có thao tác ghi chỉ chạy trên `QLHV_Test`, tuyệt đối không thay đổi database chính `QLHV`.
    - Cập nhật giao diện: Badge hiển thị môi trường bắt buộc trên `FrmMain` (`Môi trường: SQL Server` hoặc `Môi trường: Fake Data`), cập nhật nhãn tài khoản mẫu trên `FrmLogin`.
    - Trạng thái nghiệm thu: Phần triển khai (implementation) và toàn bộ kiểm thử tự động (automated tests) đã hoàn thành đạt 100%. Kiểm thử giao diện thực tế (manual WinForms UI UAT) đang chờ người dùng trực tiếp mở ứng dụng và xác nhận trên môi trường desktop.

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
├── database/                     # Script CSDL SQL Server (001_schema.sql, 002_seed.sql, README.md)
├── src/
│   ├── QLHV.Contracts/           # Enums, DTOs, Filters, OperationResult, ErrorCodes
│   ├── QLHV.Core/                # Abstractions, Repositories/Services Interfaces, Security, Logging, Validation
│   ├── QLHV.Infrastructure.SqlServer/ # SqlConnectionFactory, SqlServerOptions & 7 SQL Repositories ADO.NET
│   ├── QLHV.Features.Catalog/    # In-memory fake repositories & Services triển khai quy tắc nghiệp vụ
│   └── QLHV.WinForms/            # Giao diện Forms, UserControls, Composition Root (Program.cs), appsettings.json
├── tests/
│   └── QLHV.Tests/               # Bộ kiểm thử đơn vị & tích hợp MSTest (103 tests passed)
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
Kết quả chạy: **103/103 tests PASSED (100%)** (gồm Auth, WinForms UI Smoke, Catalog Validation & CRUD, Service Authorization, Session & Matrix immutability, SqlConnectionFactory, 10 SQL Integration Tests trên database `QLHV_Test`, AppConfig/Provider wiring, và FileAppLogger Sanitization).

> [!IMPORTANT]
> **Yêu cầu đối với 10 bài SQL Integration Tests:**
> - Cần có Microsoft SQL Server Express (hoặc LocalDB) đang chạy.
> - Cần chuẩn bị trước cơ sở dữ liệu kiểm thử riêng biệt **`QLHV_Test`** (đã chạy `database/001_schema.sql` và `database/002_seed.sql`). Tuyệt đối không chạy integration tests trên database ứng dụng `QLHV`.
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
| **`admin`** | `Admin@123` | **ADMIN** | Toàn bộ quyền: Xem & Quản lý Catalog, Đào tạo, Học phí, Điểm, Báo cáo, Quản lý người dùng. |
| **`daotao`** | `DaoTao@123` | **DAOTAO** | Xem & Quản lý Catalog, Xem & Quản lý phân hệ Đào tạo. |
| **`thungan`** | `ThuNgan@123` | **THUNGAN** | Xem danh mục (chỉ đọc), Quản lý phân hệ Học phí. |
| **`giangvien`** | `GiangVien@123` | **GIANGVIEN** | Xem danh mục (chỉ đọc), Nhập điểm số học viên. |
| **`quanly`** | `QuanLy@123` | **QUANLY** | Xem danh mục (chỉ đọc), Xem báo cáo & thống kê. |
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
   - Bộ 10 bài test `SqlServerIntegrationTests` chạy trên database chuyên biệt `QLHV_Test`, hoàn toàn cách ly với database ứng dụng `QLHV`.

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
| **DH-06** | Ổn định service dùng chung và tích hợp module của thành viên khác | **PENDING INTEGRATION** | Đang chờ module Training từ Bá Hiệp. |
| **DH-07** | Kiểm tra quyền ở service, chuẩn hóa lỗi/logging, lọc dữ liệu nhạy cảm và tích hợp SQL Server | **DONE** | Hoàn thành 100% phân quyền Service, Logging an toàn lọc nhạy cảm, 7 SQL Repositories ADO.NET và 103 bài test tự động. |
| **DH-08** | Đóng gói, cấu hình đa chế độ (SQL/Fake), release và kiểm tra máy sạch | **PARTIAL** | Đã hoàn thiện cấu hình SQL/Fake, Startup check, chờ module hoàn thiện để đóng gói bản phát hành chính thức. |

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

### Cần Chu Đức cung cấp:
- [x] Schema CSDL chính thức (`database/001_schema.sql` - đã hoàn thành ở `SQL-01A`).
- [x] Quy ước đặt tên bảng và cột (đồng nhất với DTO trong `QLHV.Contracts.Dtos` - đã xong).
- [x] Script dữ liệu mẫu SQL (`database/002_seed.sql` - đã hoàn thành ở `SQL-01A`).
- [x] File chuỗi kết nối mẫu không chứa mật khẩu thực (`appsettings.sample.json` - đã xong).
- [x] Triển khai 7 ADO.NET Repositories thực tế theo các interface trong `QLHV.Core.Repositories` (`SQL-01B1`).
- [x] Kiểm thử tích hợp SQL trên CSDL thật (SQL runtime integration tests - `SQL-01B1`).
- [x] Nối WinForms với SQL Server và cấu hình đa chế độ (`SQL-01B2`).

### Cần Bá Hiệp cung cấp:
- [ ] Các DTOs và Contracts cho module Training (Lớp học, Phiếu thu, Điểm số).
- [ ] Triển khai các Services nghiệp vụ đào tạo kế thừa mẫu `OperationResult<T>`.
- [ ] Các Windows Forms cho phân hệ Lớp học, Điểm số và Học phí để tích hợp vào menu `FrmMain`.
