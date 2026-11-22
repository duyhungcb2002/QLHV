# KẾ HOẠCH VÀ CHECKLIST KIỂM THỬ CHẤP NHẬN NGƯỜI DÙNG (UAT)

- Dự án: **Hệ thống Quản lý Học viên (QLHV)**
- Phiên bản: **Mega MVP v2.1.0 (Hardened & Student Profile 05)**
- Ngày cập nhật: **22/11/2026**

---

## 1. Trạng Thái Kiểm Thử Thủ Công Giao Diện (Manual UI UAT Status)

> [!WARNING]
> **TRẠNG THÁI HIỆN TẠI:** `MANUAL_UI_UAT_NOT_VERIFIED`
> Do quy trình tự động hóa môi trường không thể thực hiện tương tác chuột/bàn phím trực quan trên màn hình hiển thị của người dùng (Interactive Desktop Session), các ca kiểm thử thủ công dưới đây được thiết kế hoàn chỉnh làm kịch bản nghiệm thu chính thức cho đội ngũ kiểm thử viên / người dùng cuối thực hiện. Toàn bộ logic nghiệp vụ bên dưới đã được bảo vệ bởi 164 bài kiểm thử tự động (138 Unit/Smoke Tests & 26 SQL Server Integration Tests) và UI Smoke Tests khởi tạo Form thành công.

---

## 2. Ma Trận Truy Vết Nghiệp Vụ (Requirements Traceability Matrix)

| Yêu cầu nghiệp vụ | Ca kiểm thử UAT | Module tương ứng | Kết quả tự động hóa | Kết quả UAT thủ công |
| :--- | :--- | :--- | :--- | :--- |
| **REQ-00: Hồ sơ học viên & Giảng viên CN** | UAT-HV-01, UAT-HV-02, UAT-HV-03 | `FrmHocVien`, `FrmHocVienEdit`, `FrmHocVienDetail` | PASSED (Unit + Integration) | `MANUAL_UI_UAT_NOT_VERIFIED` |
| **REQ-01: Quản lý lớp học** | UAT-LH-01, UAT-LH-02, UAT-LH-03, UAT-LH-04, UAT-LH-05 | `FrmDaoTao` (Tab Lớp học) | PASSED (Unit + Integration) | `MANUAL_UI_UAT_NOT_VERIFIED` |
| **REQ-02: Xếp học viên vào lớp** | UAT-XL-01, UAT-XL-02, UAT-XL-03 | `FrmDaoTao` (Tab Xếp lớp) | PASSED (Unit + Integration) | `MANUAL_UI_UAT_NOT_VERIFIED` |
| **REQ-03: Quản lý lịch học** | UAT-LIC-01, UAT-LIC-02, UAT-LIC-03 | `FrmDaoTao` (Tab Lịch học) | PASSED (Unit + Integration) | `MANUAL_UI_UAT_NOT_VERIFIED` |
| **REQ-04: Quản lý điểm số** | UAT-DIEM-01, UAT-DIEM-02, UAT-DIEM-03 | `FrmDiem` | PASSED (Unit + Integration) | `MANUAL_UI_UAT_NOT_VERIFIED` |
| **REQ-05: Học phí & Phiếu thu** | UAT-HP-01, UAT-HP-02, UAT-HP-03 | `FrmHocPhi`, `FrmThuHocPhi` | PASSED (Unit + Integration) | `MANUAL_UI_UAT_NOT_VERIFIED` |
| **REQ-06: Báo cáo thống kê** | UAT-BC-01, UAT-BC-02 | `FrmBaoCao` | PASSED (Unit + Integration) | `MANUAL_UI_UAT_NOT_VERIFIED` |
| **REQ-07: Phân quyền & RBAC** | UAT-SEC-01, UAT-SEC-02, UAT-SEC-03 | `FrmMain`, Toàn hệ thống | PASSED (Unit Tests) | `MANUAL_UI_UAT_NOT_VERIFIED` |

---

## 3. Danh Sách Chi Tiết Ca Kiểm Thử Chấp Nhận (UAT Test Cases)

### Module 1: Quản Lý Lớp Học (LopHoc)
- **UAT-LH-01: Thêm mới lớp học hợp lệ**
  - *Các bước*: Đăng nhập `daotao`, mở Đào tạo -> Lớp học -> Thêm lớp. Nhập mã lớp `LH_UAT_01`, Tên `Lớp UAT 01`, Sĩ số 30, chọn ngày bắt đầu hợp lệ. Nhấn Lưu.
  - *Kỳ vọng*: Lớp học hiển thị trong danh sách với trạng thái *Đang hoạt động*.
- **UAT-LH-02: Kiểm tra trùng mã lớp học**
  - *Các bước*: Thêm mới lớp với mã `LH_UAT_01` vừa tạo.
  - *Kỳ vọng*: Hệ thống báo lỗi trùng mã lớp và ngăn chặn thao tác ghi.
- **UAT-LH-03: Kiểm tra giới hạn sĩ số (1 - 200)**
  - *Các bước*: Thêm lớp học với sĩ số 0 hoặc 201.
  - *Kỳ vọng*: Hệ thống hiển thị cảnh báo validation lỗi sĩ số phải nằm trong khoảng từ 1 đến 200.
- **UAT-LH-04: Chặn ngừng hoạt động lớp học khi còn học viên đang học**
  - *Các bước*: Chọn một lớp học đang có học viên theo học (ví dụ `LH_NET_01`), thử chuyển trạng thái lớp sang *Ngừng hoạt động*.
  - *Kỳ vọng*: Hệ thống chặn thao tác và hiển thị thông báo lỗi yêu cầu chuyển hoặc hủy đăng ký học viên trước khi ngừng hoạt động lớp.
- **UAT-LH-05: Chặn ngừng hoạt động lớp học khi còn lịch học đang hoạt động**
  - *Các bước*: Chọn một lớp học chưa có học viên nhưng còn các buổi học đang hoạt động trong tương lai, thử chuyển trạng thái lớp sang *Ngừng hoạt động*.
  - *Kỳ vọng*: Hệ thống chặn thao tác và hiển thị thông báo lỗi yêu cầu hủy các buổi học trước khi ngừng hoạt động lớp.

### Module 2: Xếp Học Viên Vào Lớp (DangKyLopHoc)
- **UAT-XL-01: Thêm học viên vào lớp và chụp snapshot học phí**
  - *Các bước*: Mở Tab Xếp lớp, chọn lớp `LH_UAT_01` (khóa học có học phí 5,000,000 VNĐ). Nhấn Thêm học viên, chọn học viên `HV001`.
  - *Kỳ vọng*: Học viên được ghi nhận vào lớp; học phí phải thu hiển thị chính xác 5,000,000 VNĐ; công nợ ghi nhận 5,000,000 VNĐ.
- **UAT-XL-02: Kiểm tra trùng học viên trong cùng lớp**
  - *Các bước*: Tiếp tục thêm học viên `HV001` vào lớp `LH_UAT_01`.
  - *Kỳ vọng*: Hệ thống thông báo học viên đã được ghi danh vào lớp này.
- **UAT-XL-03: Kiểm tra vượt quá sĩ số tối đa**
  - *Các bước*: Tạo lớp có sĩ số tối đa là 1. Thêm học viên thứ 1, sau đó thử thêm tiếp học viên thứ 2.
  - *Kỳ vọng*: Hệ thống báo lỗi lớp đã đủ sĩ số tối đa.

### Module 3: Quản Lý Lịch Học & Trùng Lịch (LichHoc)
- **UAT-LIC-01: Thêm buổi học hợp lệ**
  - *Các bước*: Mở Tab Lịch học, chọn lớp, chọn môn học, phòng học `P101`, giảng viên `GV001`, thời gian từ 08:00 đến 11:30 ngày mai. Nhấn Lưu.
  - *Kỳ vọng*: Buổi học được tạo thành công và xuất hiện trên thời khóa biểu.
- **UAT-LIC-02: Phát hiện xung đột trùng phòng học**
  - *Các bước*: Tạo buổi học cho lớp khác tại phòng `P101` có khung giờ giao nhau (09:00 - 10:00 cùng ngày).
  - *Kỳ vọng*: Hệ thống báo lỗi trùng phòng học và từ chối lưu.
- **UAT-LIC-03: Phát hiện xung đột trùng giảng viên**
  - *Các bước*: Tạo buổi học cho lớp khác với giảng viên `GV001` trong cùng khung giờ.
  - *Kỳ vọng*: Hệ thống báo lỗi giảng viên đã có lịch dạy vào khung giờ này.

### Module 4: Kết Quả Học Tập (KetQuaHocTap)
- **UAT-DIEM-01: Nhập và tự động tính điểm tổng kết**
  - *Các bước*: Đăng nhập `giangvien` hoặc `admin`, mở form Kết quả học tập. Chọn lớp và môn học phụ trách. Nhập Điểm tổng kết = 8.5.
  - *Kỳ vọng*: Cột Đạt hiển thị *Đạt* (vì điểm $\ge 5.00$). Nhấn Lưu điểm thành công.
- **UAT-DIEM-02: Kiểm tra thang điểm hợp lệ (0.0 - 10.0)**
  - *Các bước*: Nhập điểm -1 hoặc 10.5 vào ô điểm.
  - *Kỳ vọng*: Hệ thống từ chối hoặc báo lỗi điểm số không hợp lệ.
- **UAT-DIEM-03: Xóa điểm học viên khi để trắng ô điểm (Blank grade deletion)**
  - *Các bước*: Chọn một ô điểm đã nhập điểm trước đó, xóa trắng (để ô trống) và nhấn Lưu điểm.
  - *Kỳ vọng*: Bản ghi điểm được xóa khỏi cơ sở dữ liệu, cột kết quả hiển thị "Chưa nhập" mà không báo lỗi.

### Module 5: Quản Lý Học Phí & Phiếu Thu (PhieuThu)
- **UAT-HP-01: Thu học phí một phần và theo dõi công nợ**
  - *Các bước*: Đăng nhập `thungan`, mở form Học phí. Chọn học viên đang nợ 5,000,000 VNĐ. Nhấn Lập phiếu thu, nhập 2,000,000 VNĐ, chọn Tiền mặt. Nhấn Xác nhận.
  - *Kỳ vọng*: Tạo phiếu thu thành công. Đã nộp cập nhật 2,000,000 VNĐ, Còn nợ giảm xuống 3,000,000 VNĐ.
- **UAT-HP-02: Ngăn chặn thu vượt quá số tiền còn nợ**
  - *Các bước*: Thử lập phiếu thu mới với số tiền 4,000,000 VNĐ cho học viên chỉ còn nợ 3,000,000 VNĐ.
  - *Kỳ vọng*: Hệ thống báo lỗi số tiền thu không được vượt quá số nợ còn lại.
- **UAT-HP-03: Hủy phiếu thu có ghi nhận lý do (Soft Delete)**
  - *Các bước*: Chọn phiếu thu 2,000,000 VNĐ vừa lập, nhấn Hủy phiếu thu. Nhập lý do "Học viên yêu cầu đổi hình thức thanh toán".
  - *Kỳ vọng*: Phiếu thu chuyển trạng thái *DaHuy*, công nợ học viên tự động tăng lại thành 5,000,000 VNĐ. Dữ liệu phiếu thu không bị xóa khỏi CSDL để phục vụ kiểm toán.

### Module 6: Báo Cáo Thống Kê (BaoCao)
- **UAT-BC-01: Báo cáo học viên theo lớp và kết quả học tập**
  - *Các bước*: Mở form Báo cáo -> Tab Học viên theo lớp, Tab Kết quả học tập.
  - *Kỳ vọng*: Hiển thị danh sách học viên theo từng lớp kèm tên khóa thật, ngày sinh thật, email thật; phân biệt rõ điểm 0.00 và điểm chưa nhập (null); thống kê tỷ lệ đạt từng môn học.
- **UAT-BC-02: Báo cáo công nợ học phí theo lớp**
  - *Các bước*: Mở form Báo cáo -> Tab Công nợ học phí.
  - *Kỳ vọng*: Hiển thị tổng hợp từng lớp: Sĩ số, Tổng học phí phải thu, Tổng tiền đã thu thực tế, Tổng nợ còn lại và Tỷ lệ thu hồi học phí (%).

### Module 7: Phân Quyền Bảo Mật (RBAC)
- **UAT-SEC-01: Phân quyền vai trò Giảng viên (`GIANGVIEN`)**
  - *Các bước*: Đăng nhập tài khoản `giangvien`.
  - *Kỳ vọng*: Chỉ thao tác được với Lịch giảng dạy và Kết quả học tập; không truy cập được Học phí, Quản lý đào tạo chung hay Báo cáo tổng hợp.
- **UAT-SEC-02: Phân quyền vai trò Thu ngân (`THUNGAN`)**
  - *Các bước*: Đăng nhập tài khoản `thungan`.
  - *Kỳ vọng*: Thao tác được trên phân hệ Học phí & Phiếu thu; không có quyền sửa đổi điểm thi, không có quyền Quản lý đào tạo hay Xem báo cáo.
- **UAT-SEC-03: Phân quyền vai trò Đào tạo (`DAOTAO`)**
  - *Các bước*: Đăng nhập tài khoản `daotao`.
  - *Kỳ vọng*: Quản lý danh mục và phân hệ Đào tạo (Lớp, Xếp lớp, Lịch học); không có quyền Nhập điểm, Thu học phí hay Xem báo cáo.

---

## 4. Biên Bản Ký Duyệt UAT (Sign-off Template)

| Đại diện | Họ và tên | Chức vụ | Chữ ký | Ngày ký |
| :--- | :--- | :--- | :--- | :--- |
| **Đội ngũ Phát triển** | | Lead Developer | | |
| **Đội ngũ Đảm bảo chất lượng (QA)** | | QA Lead | | |
| **Người dùng đại diện (Đào tạo)** | | Chuyên viên Giáo vụ | | |
| **Người dùng đại diện (Tài chính)** | | Phụ trách Kế toán | | |
| **Ban Quản lý Dự án** | | Project Manager | | |
