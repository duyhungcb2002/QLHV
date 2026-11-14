# Ghi Chú Tích Hợp Kỹ Thuật (Integration Notes)

Tài liệu này hướng dẫn chi tiết cách các thành viên khác tích hợp phần việc của mình vào kiến trúc chuẩn do Duy Hưng xây dựng.

---

## 1. Dành cho Chu Đức (Database & ADO.NET Repositories)

### 1.1. Hiện trạng kiến trúc (Sau giai đoạn `SQL-01B1 + SQL-01B2`)
- **`SEC-01` đã hoàn thành**: Bảo vệ tính bất biến của ma trận quyền và toàn vẹn phiên đăng nhập.
- **`SQL-01A` đã hoàn thành**: Hạ tầng `SqlConnectionFactory`, schema `001_schema.sql`, seed `002_seed.sql`.
- **`SQL-01B1` đã hoàn thành**:
  - Triển khai đầy đủ 7 SQL Repositories ADO.NET (`SqlTaiKhoanRepository`, `SqlHocVienRepository`, `SqlGiangVienRepository`, `SqlMonHocRepository`, `SqlKhoaHocRepository`, `SqlKhoaHocMonHocRepository`, `SqlPhongHocRepository`).
  - Đồng bộ validation độ dài tối đa tại Service với schema SQL.
  - Bộ kiểm thử tích hợp (SQL Integration Tests) 10/10 passed trên database `QLHV_Test`.
- **`SQL-01B2` (Kết nối WinForms & Cấu hình an toàn)**:
  - **SQL-01B2 implementation**: **DONE** (Nối toàn diện WinForms với SQL Server thật `QLHV`, `AppConfigLoader` đa nguồn, `StartupConnectionTester` chặn `QLHV_Test`, badge môi trường bắt buộc, làm sạch log toàn diện).
  - **Automated tests**: **DONE** (103/103 tests passed, gồm 10 bài test SQL Server integration độc lập trên `QLHV_Test`, các test cấu hình đa chế độ và kiểm tra lọc log bảo mật).
  - **Manual WinForms UAT**: **PENDING USER VERIFICATION** (Chờ người dùng mở ứng dụng trên môi trường desktop thực tế để xác nhận trải nghiệm người dùng).

### 1.2. Hướng dẫn cấu hình kết nối SQL Server
- File cấu hình chính: `src/QLHV.WinForms/appsettings.json`.
- File ghi đè cục bộ (không commit): `src/QLHV.WinForms/appsettings.Local.json`.
- Cấu hình mẫu:
  ```json
  {
    "DataProvider": "SqlServer",
    "ConnectionStrings": {
      "QLHV": "Server=.\\SQLEXPRESS;Database=QLHV;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;Connect Timeout=5"
    }
  }
  ```
- Biến môi trường (nếu muốn ghi đè qua CI/CD):
  - `QLHV_DATA_PROVIDER`: `SqlServer` hoặc `Fake`.
  - `QLHV_CONNECTION_STRING`: Chuỗi kết nối tới SQL Server.

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
| **DH-06** | Ổn định service dùng chung và tích hợp module thành viên khác | Nhóm | **PENDING INTEGRATION** (Chờ module của Bá Hiệp) |
| **DH-07** | Kiểm tra quyền ở service, chuẩn hóa lỗi/logging, lọc dữ liệu nhạy cảm và tích hợp SQL Server | Duy Hưng | **DONE** |
| **DH-08** | Đóng gói, cấu hình đa chế độ (SQL/Fake), release và kiểm thử máy sạch | Duy Hưng | **PARTIAL** (Đã hoàn thiện cấu hình SQL/Fake, chờ module hoàn thiện để đóng gói) |
