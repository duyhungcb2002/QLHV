# Ghi Chú Tích Hợp Kỹ Thuật (Integration Notes)

Tài liệu này hướng dẫn chi tiết cách các thành viên khác tích hợp phần việc của mình vào kiến trúc chuẩn do Duy Hưng xây dựng.

---

## 1. Dành cho Chu Đức (Database & ADO.NET Repositories)

### 1.1. Hiện trạng kiến trúc (Sau giai đoạn `SEC-01 + SQL-01A`)
- **`SEC-01` đã hoàn thành**: Đã bảo vệ tính bất biến của ma trận quyền (`AuthorizationService.GetPermissions()` trả bản sao độc lập) và bảo vệ toàn vẹn phiên đăng nhập (`CurrentUserService` sử dụng defensive copy khi gán và đọc `CurrentUser`).
- **`SQL-01A` đã hoàn thành**:
  - Đã tạo project `src/QLHV.Infrastructure.SqlServer` chứa `SqlConnectionFactory : IConnectionFactory` và `SqlServerOptions`.
  - Đã tạo script `database/001_schema.sql` (7 bảng hiện có), `database/002_seed.sql` (dữ liệu mẫu idempotent với mật khẩu PBKDF2) và `database/README.md`.
  - Đã tạo file cấu hình mẫu `src/QLHV.WinForms/appsettings.sample.json`.
- **Giới hạn hiện tại (Chưa sang `SQL-01B`)**:
  - Ứng dụng hiện **vẫn chạy bằng Fake Repository** (`FakeHocVienRepository`, `FakeGiangVienRepository`, ...) nằm trong `QLHV.Features.Catalog.Repositories.Fake`.
  - **Chưa có SQL Repository** (chưa triển khai `SqlTaiKhoanRepository` và 6 repository danh mục).
  - `Program.cs` **chưa được nối sang SQL**.
  - SQL persistence **chưa hoàn thành** và **SQL runtime chưa được xác minh** trên máy chưa có SQL Server.
  - Bước tiếp theo là giai đoạn **`SQL-01B`**.
- Các service trong `QLHV.Features.Catalog.Services` chỉ phụ thuộc vào Repository Interface định nghĩa tại `QLHV.Core.Repositories`:
  - `IHocVienRepository`
  - `IGiangVienRepository`
  - `IMonHocRepository`
  - `IKhoaHocRepository`
  - `IKhoaHocMonHocRepository`
  - `IPhongHocRepository`
  - `ITaiKhoanRepository`

### 1.2. Kế hoạch triển khai giai đoạn SQL-01B
Thay vì tạo mới hạ tầng, giai đoạn `SQL-01A` đã hoàn tất `QLHV.Infrastructure.SqlServer`, `SqlConnectionFactory`, schema và seed SQL. Giai đoạn `SQL-01B` sẽ tập trung vào các hạng mục sau:

1. **Bảy SQL Repositories** kế thừa các interface trong `QLHV.Core.Repositories`:
   - `SqlHocVienRepository : IHocVienRepository`
   - `SqlGiangVienRepository : IGiangVienRepository`
   - `SqlMonHocRepository : IMonHocRepository`
   - `SqlKhoaHocRepository : IKhoaHocRepository`
   - `SqlKhoaHocMonHocRepository : IKhoaHocMonHocRepository`
   - `SqlPhongHocRepository : IPhongHocRepository`
   - `SqlTaiKhoanRepository : ITaiKhoanRepository`
2. **Cơ chế đọc cấu hình (Configuration Loader)**:
   - Đọc cấu hình từ `appsettings.json` / `appsettings.Local.json` với fallback an toàn.
3. **Lựa chọn đồng bộ chế độ dữ liệu (`DataProvider`)**:
   - Chuyển đổi giữa `Fake` và `SqlServer` thông qua cấu hình, đảm bảo tính nhất quán (không fallback ngầm).
4. **Wiring tại Composition Root (`Program.cs`)**:
   - Nối `IConnectionFactory` và các SQL Repositories vào Composition Root khi chọn provider `SqlServer`.
5. **Kiểm thử tích hợp (SQL Integration Tests)**:
   - Xây dựng bộ test integration chạy trên database test chuyên biệt (`QLHV_Test`).
6. **Kiểm thử nghiệm thu (UAT) trên SQL Server thật**:
   - Kiểm chứng đầy đủ trên môi trường SQL Server thực tế trước khi phát hành.

### 1.3. Prerequisite cho giai đoạn SQL-01B
- **Đồng bộ Validation độ dài tại tầng Service**: Các giới hạn độ dài cột trong `database/001_schema.sql` (như `NVARCHAR(20)` cho mã, `NVARCHAR(100)` cho họ tên, `NVARCHAR(150)` cho email/tên môn/tên khóa, `NVARCHAR(250)` cho địa chỉ, `NVARCHAR(500)` cho mô tả) cần được kiểm tra tại các Service trước khi gọi Repository.
- Thêm unit test kiểm tra từ chối các chuỗi vượt độ dài quy định.

---

## 2. Dành cho Bá Hiệp (Module Training & Quản lý đào tạo)

### 2.1. Hiện trạng phân quyền và Menu
- Trên Form chính (`FrmMain`), các mục:
  - **Đào tạo - Lớp học**: Yêu cầu quyền `Permission.TrainingView` / `TrainingManage`.
  - **Điểm**: Yêu cầu quyền `Permission.GradeEntry`.
  - **Học phí & Thu chi**: Yêu cầu quyền `Permission.TuitionManage`.
- Các menu này hiện đang được hiển thị ở trạng thái `[Đang chờ tích hợp]` và hiển thị thông báo thân thiện khi click.

### 2.2. Hướng dẫn tích hợp Form nghiệp vụ
1. **Tạo Project**: `QLHV.Features.Training`.
2. **Tham chiếu**: Tham chiếu đến `QLHV.Contracts` và `QLHV.Core`.
3. **Tận dụng Core**:
   - Sử dụng `OperationResult<T>` cho mọi kết quả trả về của Service.
   - Sử dụng `ICurrentUserService` để biết ai đang thao tác và kiểm tra quyền.
   - Sử dụng `IAppLogger` để ghi log lỗi thống nhất.
4. **Gắn vào Menu**:
   - Trong `FrmMain`, thay thế sự kiện click của các nút placeholder bằng việc nạp Form nghiệp vụ của Bá Hiệp vào vùng `pnlContent`.

---

## 3. Trạng thái các đầu việc (Status Matrix)

| Mã CV | Nội dung | Phụ trách | Trạng thái |
|---|---|---|---|
| **DH-01** | Tạo solution, kiến trúc module, README, convention | Duy Hưng | **DONE** |
| **DH-02** | Contracts/Core, DTO, Interfaces, OperationResult, ConnectionFactory, Fake repositories | Duy Hưng | **DONE** |
| **DH-03** | Đăng nhập, xác thực, phân quyền ma trận, Form chính và điều hướng | Duy Hưng | **DONE** |
| **DH-04** | CRUD Học viên, Giảng viên (Validation, UI, Fake Service) | Duy Hưng | **DONE** |
| **DH-05** | CRUD Môn học, Khóa học, Khóa-Môn, Phòng học | Duy Hưng | **DONE** |
| **DH-06** | Ổn định service dùng chung và tích hợp module thành viên khác | Nhóm | **PENDING INTEGRATION** (Chờ module của Bá Hiệp & Chu Đức) |
| **DH-07** | Kiểm tra quyền ở service, chuẩn hóa lỗi/logging, tạo RC1 | Duy Hưng | **PARTIAL** (Đã xong Core Logging, Authorization, Error Handling; Chờ tích hợp tổng thể) |
| **DH-08** | Đóng gói, cấu hình mẫu, release và kiểm thử máy sạch | Duy Hưng | **PENDING INTEGRATION** (Chờ CSDL thật và module hoàn thiện) |
