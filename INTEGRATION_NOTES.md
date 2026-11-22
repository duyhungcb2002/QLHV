# Ghi Chú Tích Hợp Kỹ Thuật (Integration Notes)

Tài liệu này ghi nhận hiện trạng tích hợp toàn diện của hệ thống QLHV sau khi hoàn thành giai đoạn **Mega MVP** và **Student Profile & Training UI 05 (`STUDENT-PROFILE-AND-TRAINING-UI-05`)**.

---

## 1. Trạng thái phân hệ Đào tạo, Hồ sơ học viên & Báo cáo

Toàn bộ các phân hệ đã được triển khai hoàn chỉnh và kết nối thông suốt:
- **`QLHV.Features.Catalog` & `HocVienService`**:
  - Mở rộng hồ sơ học viên với 11 trường thông tin mới (`SoDinhDanh`, `NgayCapGiayTo`, `NoiCapGiayTo`, `HoTenNguoiGiamHo`, `DienThoaiNguoiGiamHo`, `QuanHeNguoiGiamHo`, `NhanVienPhuTrachTaiKhoanId`, `NguonTuyenSinh`, `NgheNghiep`, `DonViCongTac`, `GhiChuDacBiet`).
  - Validation chặt chẽ tầng nghiệp vụ: Kiểm tra độ dài, số định danh CCCD/CMND đúng 9 hoặc 12 chữ số, chống trùng lặp (kết hợp filtered unique index), bộ giấy tờ đi liền nhau và ngày cấp không được ở tương lai, người giám hộ bắt buộc khi học viên dưới 18 tuổi, nhân viên phụ trách phải là tài khoản hoạt động thuộc nhóm ADMIN hoặc DAOTAO, nguồn tuyển sinh thuộc danh mục chuẩn.
- **`QLHV.Features.Training`**: Triển khai các Services nghiệp vụ đào tạo:
  - `LopHocService`: Mở lớp, cập nhật thông tin lớp, kiểm tra giới hạn sĩ số (1 - 200), bắt buộc có Giảng viên chủ nhiệm (`GiangVienChuNhiemId`) đang hoạt động.
  - `DangKyLopHocService`: Xếp học viên vào lớp, lưu vết snapshot học phí tại thời điểm ghi danh, kiểm tra sĩ số tối đa và chống ghi danh trùng.
  - `LichHocService`: Lập lịch học, tự động kiểm tra xung đột trùng phòng và trùng giảng viên theo công thức giao thoa thời gian.
  - `KetQuaHocTapService`: Quản lý điểm tổng kết môn học, xếp loại Đạt/Không đạt (ngưỡng 5.00), hỗ trợ xóa điểm (blank cell), kiểm tra phân công giảng viên.
  - `HocPhiService`: Quản lý công nợ, lập phiếu thu thanh toán từng phần/toàn phần, ngăn chặn nộp thừa, hủy phiếu thu an toàn (Soft Cancel kèm lý do).
  - `BaoCaoService`: Thống kê tổng hợp theo 5 chuyên đề: Học viên theo lớp, Lịch học theo lớp, Phân công giảng dạy theo giảng viên, Kết quả học tập & Tỷ lệ đạt môn học, Công nợ học phí theo lớp.
- **Hạ tầng SQL Server ADO.NET (`QLHV.Infrastructure.SqlServer`)**: Triển khai 13 SQL Repositories hoàn chỉnh kết hợp script migration `006_student_profile_and_class_advisor.sql` và `007_unicode_data_repair.sql`.
- **Giao diện người dùng WinForms**:
  - `FrmHocVien`: Che mờ số CCCD/CMND (`SoDinhDanhMasked`), bổ sung nút "Xem chi tiết" gọi `FrmHocVienDetail`.
  - `FrmHocVienEdit`: Thiết kế hiện đại 5 tab (Thông tin cơ bản, Giấy tờ tùy thân, Người giám hộ, Nghề nghiệp & Tuyển sinh, Ghi chú), đếm ký tự ghi chú thời gian thực.
  - `FrmHocVienDetail`: Xem đầy đủ hồ sơ học viên, xem/ẩn số định danh và lịch sử đào tạo (tra cứu qua `DangKyLopHoc`, lớp học, khóa học, học phí snapshot, công nợ).
  - `FrmDaoTao`: Chuẩn hóa 3 DataGridView (`dgvLopHoc`, `dgvDangKy`, `dgvLichHoc`), `AutoGenerateColumns = false`, tiêu đề tiếng Việt chuẩn, định dạng ngày tháng và tiền tệ chuẩn.
  - `FrmLopHocEdit`: Bổ sung chọn Giảng viên chủ nhiệm từ danh sách giảng viên đang hoạt động.
  - `FrmDiem`: Ghi nhận bảng điểm trực quan trên DataGridView.
  - `FrmHocPhi` & `FrmThuHocPhi`: Quản lý công nợ, lập phiếu và hủy phiếu thu.
  - `FrmBaoCao`: 5 Tab báo cáo tổng hợp.
  - `FrmMain`: Đã nối toàn bộ các nút điều hướng chính thức, gỡ bỏ toàn bộ placeholder.

---

## 2. Hướng dẫn cấu hình kết nối SQL Server
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
- Biến môi trường:
  - `QLHV_DATA_PROVIDER`: `SqlServer` hoặc `Fake`.
  - `QLHV_CONNECTION_STRING`: Chuỗi kết nối tới SQL Server.

---

## 3. Trạng thái các đầu việc (Status Matrix)

| Mã CV | Nội dung | Phụ trách | Trạng thái |
|---|---|---|---|
| **DH-01** | Tạo solution, kiến trúc module, README, convention | Duy Hưng | **DONE** |
| **DH-02** | Contracts/Core, DTO, Interfaces, OperationResult, ConnectionFactory, Fake repositories | Duy Hưng | **DONE** |
| **DH-03** | Đăng nhập, xác thực, phân quyền ma trận, Form chính và điều hướng | Duy Hưng | **DONE** |
| **DH-04** | CRUD Học viên, Giảng viên (Validation, UI, Fake Service) | Duy Hưng | **DONE** |
| **DH-05** | CRUD Môn học, Khóa học, Khóa-Môn, Phòng học | Duy Hưng | **DONE** |
| **DH-06** | Ổn định service dùng chung và tích hợp module thành viên khác | Nhóm | **DONE** (Đã tích hợp xong toàn bộ Training, Điểm, Học phí, Báo cáo) |
| **DH-07** | Kiểm tra quyền ở service, chuẩn hóa lỗi/logging, lọc dữ liệu nhạy cảm và tích hợp SQL Server | Duy Hưng | **DONE** |
| **DH-08** | Đóng gói, cấu hình đa chế độ (SQL/Fake), release và kiểm thử máy sạch | Duy Hưng | **DONE** (Kiểm tra publish win-x64 Release thành công) |

---

## 4. Kết quả kiểm thử và nghiệm thu tự động (Testing Metrics)

- **Tổng số bài kiểm thử:** **164/164 passed** (100% Passed, 0 Failed, 0 Skipped).
  - **Non-SQL Unit & Smoke Tests:** 138/138 passed.
  - **SQL Server Integration Tests:** 26/26 passed (thực thi trên cơ sở dữ liệu kiểm thử `QLHV_Test`).
- **Lưu ý kiến trúc `dbo.LopHoc.GiangVienChuNhiemId`:** Tại tầng CSDL SQL Server, cột `GiangVienChuNhiemId` được định nghĩa cho phép `NULL` nhằm tương thích ngược với dữ liệu cũ và hỗ trợ kịch bản migration. Tuy nhiên, tại tầng nghiệp vụ (`LopHocService`), hệ thống bắt buộc lớp học phải có Giảng viên chủ nhiệm đang hoạt động khi thêm mới hoặc cập nhật.
- **Bảo mật CCCD tầng Service:** Mặt nạ định dạng chuẩn `********1234` (giữ 4 chữ số cuối), DTO trả về tầng giao diện đã được che đối với các tài khoản không có quyền `CatalogManage` (chỉ quyền `CatalogManage` mới được xem số đầy đủ).
- **Hợp đồng dữ liệu Khóa học (Course Contract):** Hoàn thiện ánh xạ `KhoaHocId` và `TenKhoaHoc` xuyên suốt các repository ADO.NET và Fake Store (`DangKyLopHoc`, `PhieuThu`, `KetQuaHocTap`).
- **Khởi động an toàn & Kiểm tra cấu trúc migration 006:** `StartupConnectionTester` kiểm tra 11 cột hồ sơ mở rộng của `dbo.HocVien`, cột `GiangVienChuNhiemId` của `dbo.LopHoc`, filtered unique index `UX_HocVien_SoDinhDanh` và 2 foreign keys.
- **Migration 007 An toàn với UTF-8 BOM:** Sử dụng điều kiện collation nhị phân chính xác (`COLLATE Latin1_General_BIN`), loại bỏ hoàn toàn các pattern wildcard rộng để không bao giờ ghi đè dữ liệu tiếng Việt hợp lệ.
- **Biên dịch sạch sẽ:** 0 cảnh báo compiler, 0 cảnh báo MSTest analyzer (chỉ ghi nhận NU1900 của NuGet Audit do máy chạy offline).
