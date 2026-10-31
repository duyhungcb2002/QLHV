# Ghi Chú Tích Hợp Kỹ Thuật (Integration Notes)

Tài liệu này hướng dẫn chi tiết cách các thành viên khác tích hợp phần việc của mình vào kiến trúc chuẩn do Duy Hưng xây dựng.

---

## 1. Dành cho Chu Đức (Database & ADO.NET Repositories)

### 1.1. Hiện trạng kiến trúc
- Hiện tại toàn bộ dữ liệu đang chạy thông qua các **In-Memory Repositories** (`FakeHocVienRepository`, `FakeGiangVienRepository`, ...) nằm trong `QLHV.Features.Catalog.Repositories.Fake`.
- Các service trong `QLHV.Features.Catalog.Services` chỉ phụ thuộc vào Repository Interface định nghĩa tại `QLHV.Core.Repositories`:
  - `IHocVienRepository`
  - `IGiangVienRepository`
  - `IMonHocRepository`
  - `IKhoaHocRepository`
  - `IKhoaHocMonHocRepository`
  - `IPhongHocRepository`
  - `ITaiKhoanRepository`

### 1.2. Hướng dẫn thay thế bằng ADO.NET
1. **Tạo Project mới hoặc Folder**: `QLHV.Infrastructure.Data` (hoặc đặt trong `QLHV.Core.Data`).
2. **Kế thừa các Interface**: Viết các class ADO.NET thực thi các interface trên (ví dụ `SqlHocVienRepository : IHocVienRepository`).
3. **Sử dụng `IConnectionFactory`**:
   - `IConnectionFactory` đã được định nghĩa sẵn trong `QLHV.Core.Database` trả về `DbConnection`.
   - Chu Đức chỉ cần viết class `SqlConnectionFactory : IConnectionFactory` để mở `SqlConnection` với Connection String thực tế.
4. **Thay thế tại Composition Root**:
   - Trong `QLHV.WinForms\Program.cs`, thay thế các dòng khởi tạo `new FakeHocVienRepository()` bằng `new SqlHocVienRepository(connectionFactory)`.
   - Toàn bộ giao diện Forms và Business Logic Services **không cần sửa một dòng code nào**!

### 1.3. Đầu vào cần Chu Đức cung cấp
- File script tạo bảng CSDL SQL Server chính thức (Schema DDL).
- Quy ước đặt tên bảng và cột (Khuyến nghị đồng nhất với tên thuộc tính trong `QLHV.Contracts.Dtos`).
- Script dữ liệu mẫu (Seed Data SQL).
- File chuỗi kết nối mẫu không chứa mật khẩu thực tế (`appsettings.sample.json`).
- Báo cáo và thống kê: Chu Đức sẽ tạo module `QLHV.Features.Reporting` kế thừa mẫu `FrmBaoCaoMau`.

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
