# Hệ Thống Quản Lý Học Viên (QLHV)

> **Môn học:** Lập trình Windows (C# Windows Forms)  
> **Thành viên phụ trách Core / Kiến trúc:** Duy Hưng  
> **Phiên bản:** 1.0.0-preview (Cơ sở kiến trúc & In-Memory Fake Data)

---

## 1. Giới thiệu hệ thống

**Hệ thống Quản lý học viên (QLHV)** là phần mềm máy tính (Desktop App) phục vụ công tác quản lý tuyển sinh, quản lý danh mục đào tạo, phân công giảng dạy, theo dõi lớp học, quản lý điểm và học phí cho các trung tâm đào tạo / cơ sở giáo dục.

Dự án được xây dựng với kiến trúc hướng dịch vụ nhiều tầng (Layered Architecture), tuân thủ nguyên tắc trách nhiệm duy nhất (Single Responsibility Principle), đảm bảo dễ bảo trì, dễ mở rộng và rất trực quan cho sinh viên học tập lập trình Windows Forms với C#.

---

## 2. Phạm vi đã hoàn thành trong giai đoạn hiện tại (Duy Hưng)

- **Cấu trúc Solution chuẩn mực**: Tách biệt hoàn toàn Contracts, Core, Features và Presentation (WinForms).
- **Hệ thống Contracts dùng chung**: DTOs, Enums, Filters, mã lỗi chuẩn hóa (`OperationResult`, `ErrorCodes`).
- **Xác thực & Phân quyền 2 lớp (Two-Layer Authorization)**:
  - Hashing mật khẩu bằng PBKDF2 (`Rfc2898DeriveBytes.Pbkdf2`) an toàn.
  - Ma trận phân quyền theo vai trò (`IAuthorizationService`).
  - Quản lý phiên đăng nhập người dùng (`ICurrentUserService`).
  - Phân quyền chặt chẽ 2 lớp: kiểm tra ở tầng Presentation (ẩn/khóa menu, nút bấm trong WinForms) và kiểm tra bắt buộc tại tầng Service (`AuthorizationGuard` kiểm tra `CatalogView` và `CatalogManage`), không gọi repository khi thiếu quyền.
- **Giao diện Windows Forms hoàn chỉnh**:
  - `FrmLogin`: Đăng nhập, kiểm tra tài khoản, phân biệt mật khẩu sai và tài khoản bị khóa, phím tắt Enter.
  - `FrmMain`: Màn hình chính hiện đại, điều hướng mượt mà, menu tự động ẩn/khóa theo quyền, chống mở lặp cửa sổ.
  - `FrmHocVien` & `FrmHocVienEdit`: Quản lý danh mục học viên (Tìm kiếm không dấu/có dấu, Thêm, Sửa, Ngừng hoạt động).
  - `FrmGiangVien` & `FrmGiangVienEdit`: Quản lý danh mục giảng viên.
  - `FrmMonHoc` & `FrmMonHocEdit`: Quản lý danh mục môn học.
  - `FrmKhoaHoc` & `FrmKhoaHocEdit`: Quản lý danh mục khóa học & học phí niêm yết.
  - `FrmKhoaHocMonHoc` & `FrmKhoaHocMonHocEdit`: Quản lý danh sách môn học thuộc từng khóa đào tạo.
  - `FrmPhongHoc` & `FrmPhongHocEdit`: Quản lý danh mục phòng học & sức chứa.
  - `FrmBaoCaoMau`: Mẫu giao diện báo cáo chuẩn (dùng lại được) sẵn sàng cho Chu Đức.
- **Logging an toàn**: `FileAppLogger` tự động ghi log vào thư mục `logs/`, lọc mật khẩu và connection string, không gây crash ứng dụng.
- **Hệ thống Kiểm thử tự động (Unit Tests)**: 60/60 bài test MSTest bao phủ toàn diện Auth, Phân quyền UI, Bảo vệ bất biến ma trận quyền & phiên đăng nhập (`SEC-01`), Hạ tầng `SqlConnectionFactory` (`SQL-01A`), CRUD, Validation nghiệp vụ và Kiểm thử phân quyền độc lập tại tầng Service cho toàn bộ 6 Service Catalog.
- **Cập nhật giai đoạn `SEC-01 + SQL-01A`**:
  - **`SEC-01` (Bảo vệ ma trận quyền và phiên đăng nhập)**: `AuthorizationService.GetPermissions()` trả về bản sao độc lập (defensive copy), ngăn sửa đổi ma trận quyền nội bộ từ bên ngoài; `CurrentUserService` tạo bản sao phòng vệ khi lưu (`SetCurrentUser`) và khi đọc (`CurrentUser`), ngăn leo thang đặc quyền qua việc sửa DTO từ bên ngoài.
  - **`SQL-01A` (Hạ tầng kết nối, Schema & Seed SQL Server)**: Đã khởi tạo project `src/QLHV.Infrastructure.SqlServer` với `SqlConnectionFactory` và `SqlServerOptions`, tạo script `database/001_schema.sql` (khóa collation `Vietnamese_100_CI_AI`), `database/002_seed.sql` (kèm tài khoản test bị khóa `bikhoa`), `database/README.md` và cấu hình mẫu `src/QLHV.WinForms/appsettings.sample.json`.
  - **Trạng thái thực thi hiện tại**:
    - Ứng dụng hiện vẫn chạy bằng **Fake Repository** (In-Memory).
    - **Chưa có SQL Repository** (`SqlTaiKhoanRepository`, `SqlHocVienRepository`, ...).
    - `Program.cs` **chưa được nối sang SQL**.
    - SQL persistence **chưa hoàn thành** và **SQL runtime chưa được xác minh** do môi trường hiện tại chưa cài đặt SQL Server.
    - Bước tiếp theo là giai đoạn **`SQL-01B`** (triển khai các SQL Repository và kết nối cấu hình).

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
│   ├── QLHV.Infrastructure.SqlServer/ # SqlConnectionFactory & SqlServerOptions (giai đoạn SQL-01A)
│   ├── QLHV.Features.Catalog/    # In-memory fake repositories & Services triển khai quy tắc nghiệp vụ
│   └── QLHV.WinForms/            # Giao diện Forms, UserControls, Composition Root (Program.cs), appsettings.sample.json
├── tests/
│   └── QLHV.Tests/               # Bộ kiểm thử đơn vị MSTest cho Auth, Authorization, Catalog CRUD & Validation
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
        │     QLHV.Core     │ ────────┐
        └─────────▲─────────┘         │
                  │                   │
        ┌─────────┴─────────┐         │
        │QLHV.Features.Cat..│         │
        └─────────▲─────────┘         │
                  │                   │
        ┌─────────┴───────────────────┴─────────┐
        │              QLHV.WinForms            │
        └───────────────────────────────────────┘
```

- Không có vòng phụ thuộc (No circular dependencies).
- WinForms **không** chứa bất kỳ câu lệnh SQL hoặc kết nối CSDL trực tiếp nào.
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

## 9. Cách Chạy Bộ Kiểm thử (Unit Tests)

### Sử dụng Visual Studio:
- Mở `Test Explorer` (`Test` -> `Test Explorer`), nhấn nút `Run All Tests`.

### Sử dụng dòng lệnh:
```powershell
cd D:\HMNU\Laptrinhwindow\QLHV
dotnet test tests\QLHV.Tests\QLHV.Tests.csproj
```
Kết quả chạy: **60/60 tests PASSED (100%)** (gồm Auth, WinForms UI Smoke, Catalog Validation & CRUD, Service Authorization, Session & Matrix immutability, và SqlConnectionFactory).

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

## 11. Cơ chế dữ liệu In-Memory (Fake Data)

- Trong giai đoạn hiện tại, toàn bộ dữ liệu (tài khoản, học viên, giảng viên, môn học, khóa học, phòng học) được lưu trữ trong bộ nhớ RAM (`FakeHocVienRepository`, `FakeGiangVienRepository`, ...).
- **Đặc điểm:**
  - Dữ liệu có sẵn tối thiểu 3 bản ghi mẫu thực tế cho mỗi danh mục ngay khi khởi động.
  - Các thao tác Thêm, Sửa, Ngừng hoạt động có hiệu lực ngay lập tức trong phiên chạy.
  - **Dữ liệu sẽ được hoàn nguyên về trạng thái mẫu ban đầu khi tắt ứng dụng**.

---

## 12. Hướng dẫn thay thế In-Memory Repository bằng ADO.NET SQL Server (Kế hoạch giai đoạn SQL-01B)

Hạ tầng nền tảng SQL Server giai đoạn `SQL-01A` đã hoàn tất:
- Project hạ tầng `src/QLHV.Infrastructure.SqlServer` đã được tạo và kiểm thử đơn vị.
- Class `SqlConnectionFactory : IConnectionFactory` và `SqlServerOptions` đã tồn tại.
- Database scripts (`database/001_schema.sql` khóa collation `Vietnamese_100_CI_AI`, `database/002_seed.sql` chứa tài khoản hash PBKDF2) và file cấu hình mẫu `src/QLHV.WinForms/appsettings.sample.json` đã tồn tại.

Các hạng mục **chưa triển khai** (thuộc giai đoạn `SQL-01B`):
1. **Triển khai 7 SQL Repositories** kế thừa interface trong `QLHV.Core.Repositories`:
   - `SqlHocVienRepository : IHocVienRepository`
   - `SqlGiangVienRepository : IGiangVienRepository`
   - `SqlMonHocRepository : IMonHocRepository`
   - `SqlKhoaHocRepository : IKhoaHocRepository`
   - `SqlKhoaHocMonHocRepository : IKhoaHocMonHocRepository`
   - `SqlPhongHocRepository : IPhongHocRepository`
   - `SqlTaiKhoanRepository : ITaiKhoanRepository`
2. **Cơ chế đọc cấu hình và Composition Root (`Program.cs`)**:
   - Hiện tại `Program.cs` **vẫn khởi tạo 100% Fake Repository** và chưa được nối sang SQL Server.
   - Việc chuyển đổi và wiring thực tế giữa `Fake` và `SqlServer` sẽ được thực hiện tại `SQL-01B`.
3. **Kiểm thử tích hợp SQL (Integration Tests)**:
   - Sẽ kiểm chứng trên môi trường SQL Server thật tại giai đoạn `SQL-01B`. Không khẳng định các Form và Service chắc chắn hoạt động ổn định với SQL Server khi chưa có kết quả kiểm thử tích hợp runtime thực tế.

---

## 13. Prerequisite cho giai đoạn SQL-01B

Trước khi nối SQL Repository vào ứng dụng:
- **Validation độ dài tại tầng Service**: Các giới hạn độ dài ký tự trong schema (ví dụ: `MaHocVien NVARCHAR(20)`, `HoTen NVARCHAR(100)`, `Email NVARCHAR(150)`, `DiaChi NVARCHAR(250)`, `TenMonHoc NVARCHAR(150)`, `MoTa NVARCHAR(500)`, `ChuyenMon NVARCHAR(200)`) phải được phản ánh thành các quy tắc kiểm tra tính hợp lệ (validation) tương ứng tại các Service Catalog.
- Cần bổ sung unit test kiểm tra các giá trị vượt quá độ dài quy định cho các trường mã, tên, email, địa chỉ, mô tả và chuyên môn.
- Sau khi hoàn thành kiểm thử validation mới kết nối SQL Repository vào Composition Root.

---

## 14. Bảng trạng thái công việc (DH-01 đến DH-08)

| Mã CV | Nội dung công việc | Trạng thái | Lý do / Ghi chú |
|---|---|---|---|
| **DH-01** | Tạo solution, kiến trúc module, README, convention, form mẫu | **DONE** | Hoàn thành đầy đủ cấu trúc solution, file convention, .gitignore, README. |
| **DH-02** | Shared Contracts & Core, DTO, Interface, OperationResult, ConnectionFactory | **DONE** | Đầy đủ DTOs, Enums, Filters, Result pattern, IConnectionFactory abstraction. |
| **DH-03** | Đăng nhập, phân quyền ma trận, Form chính và điều hướng | **DONE** | FrmLogin, FrmMain, phân quyền tự động ẩn/khóa menu theo quyền người dùng. |
| **DH-04** | CRUD Học viên và Giảng viên | **DONE** | Đầy đủ form danh sách, form nhập liệu, validation, ngừng hoạt động, in-memory repository. |
| **DH-05** | CRUD Môn học, Khóa học, Quan hệ Khóa-Môn, Phòng học | **DONE** | Đầy đủ form danh mục, dialog gán môn học vào khóa, kiểm tra ràng buộc. |
| **DH-06** | Ổn định service dùng chung và tích hợp module của thành viên khác | **PENDING INTEGRATION** | Đang chờ module Training từ Bá Hiệp và module Reporting/CSDL từ Chu Đức. |
| **DH-07** | Kiểm tra quyền ở service, chuẩn hóa lỗi/logging và tạo RC1 | **PARTIAL** | Đã hoàn thành 100% phân quyền tại Service (`AuthorizationGuard`) cho toàn bộ 6 Service Catalog, mã lỗi `ErrorCodes.FORBIDDEN` và Logging (`FileAppLogger`). Phần RC1 và kiểm thử tích hợp SQL thật đang chờ CSDL chính thức. |
| **DH-08** | Đóng gói, cấu hình mẫu, release và kiểm tra máy sạch | **PENDING INTEGRATION** | Chờ CSDL thật và các module hoàn thiện để đóng gói bản phát hành chính thức. |

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
- [ ] Triển khai 7 ADO.NET Repositories thực tế theo các interface trong `QLHV.Core.Repositories` (Giai đoạn `SQL-01B`).
- [ ] Kiểm thử tích hợp SQL trên CSDL thật (SQL runtime integration tests - Giai đoạn `SQL-01B`).

### Cần Bá Hiệp cung cấp:
- [ ] Các DTOs và Contracts cho module Training (Lớp học, Phiếu thu, Điểm số).
- [ ] Triển khai các Services nghiệp vụ đào tạo kế thừa mẫu `OperationResult<T>`.
- [ ] Các Windows Forms cho phân hệ Lớp học, Điểm số và Học phí để tích hợp vào menu `FrmMain`.
