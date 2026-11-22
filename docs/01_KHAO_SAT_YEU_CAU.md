# TÀI LIỆU KHẢO SÁT YÊU CẦU - HỆ THỐNG QUẢN LÝ HỌC VIÊN (QLHV)

## 1. Giới Thiệu & Mục Tiêu Dự Án
Hệ thống Quản lý học viên (QLHV) là ứng dụng Windows Forms trên nền tảng .NET 10, phục vụ công tác quản lý toàn diện tại các trung tâm đào tạo tin học, lập trình và ngoại ngữ. Hệ thống kết nối cơ sở dữ liệu Microsoft SQL Server thông qua ADO.NET thuần, đảm bảo hiệu năng cao, kiểm soát chặt chẽ luồng dữ liệu và phân quyền nghiệp vụ rõ ràng.

## 2. Các Vai Trò Trong Hệ Thống (Actors)
1. **Quản trị hệ thống (Admin):**
   - Toàn quyền quản trị tài khoản, danh mục, lớp học, lịch học, học phí, điểm số và xem toàn bộ báo cáo.
2. **Cán bộ Đào tạo (DaoTao):**
   - Quản lý danh mục khóa học, môn học, phòng học, giảng viên, học viên.
   - Quản lý mở lớp học, xếp học viên vào lớp, phân công lịch học và điều phối phòng học.
   - Không có quyền Nhập điểm, Thu học phí hoặc Xem báo cáo.
3. **Thu ngân / Kế toán (ThuNgan):**
   - Xem danh mục cơ bản, quản lý thu học phí, theo dõi công nợ học viên theo lớp, lập và hủy phiếu thu.
   - Không có quyền Quản lý đào tạo, Nhập điểm hoặc Xem báo cáo.
4. **Giảng viên (GiangVien):**
   - Xem lịch dạy được phân công, tra cứu danh sách học viên trong lớp, nhập và cập nhật bảng điểm môn học cho các lớp mình giảng dạy.
   - Không có quyền Quản lý đào tạo, Thu ngân hoặc Xem báo cáo tổng hợp.
5. **Ban Quản lý (QuanLy):**
   - Xem danh mục, theo dõi toàn bộ các báo cáo thống kê về đào tạo, tiến độ, kết quả học tập và tài chính công nợ.
   - Không có quyền chỉnh sửa danh mục, Quản lý đào tạo, Nhập điểm hoặc Thu học phí.

## 3. Khảo Sát Yêu Cầu Nghiệp Vụ (Functional Requirements)

### 3.1. Phân Hệ Danh Mục & Hồ Sơ Học Viên (Catalog & Student Profile)
- **Học viên:** Quản lý hồ sơ học viên chi tiết:
  - *Thông tin cơ bản:* Mã, Họ tên, Ngày sinh, Giới tính, Điện thoại, Email, Địa chỉ, Ngày đăng ký, Trạng thái.
  - *Giấy tờ tùy thân:* Số CCCD/CMND (9 hoặc 12 chữ số, duy nhất toàn hệ thống thông qua filtered unique index, tự động che mờ dạng `********1234` trên danh sách và chỉ người dùng có quyền `CatalogManage` mới được xem số đầy đủ), Ngày cấp, Nơi cấp (nguyên tắc: nhập đầy đủ cả 3 hoặc để trống cả 3; ngày cấp không được ở tương lai).
  - *Người giám hộ:* Họ tên, Điện thoại, Mối quan hệ (bắt buộc nhập đối với học viên dưới 18 tuổi tính đến ngày hiện tại).
  - *Nghề nghiệp & Tuyển sinh:* Nghề nghiệp, Đơn vị công tác, Nguồn tuyển sinh (theo danh mục chuẩn: Website, Facebook, Giới thiệu, Trực tiếp...), Nhân viên phụ trách tuyển sinh (phải là tài khoản đang hoạt động có vai trò ADMIN hoặc DAOTAO).
  - *Ghi chú:* Ghi chú đặc biệt (tối đa 500 ký tự).
  - *Lịch sử đào tạo:* Tra cứu toàn bộ các khóa/lớp học viên đã và đang tham gia, học phí và công nợ trực tiếp từ giao diện chi tiết.
- **Giảng viên:** Quản lý thông tin giảng viên và chuyên môn giảng dạy.
- **Môn học:** Danh mục các môn học với số buổi mặc định và mô tả.
- **Khóa học:** Danh mục các chương trình đào tạo kèm học phí niêm yết và danh sách môn học thuộc khóa học.
- **Phòng học:** Quản lý phòng học và sức chứa tối đa.

### 3.2. Phân Hệ Đào Tạo (Training Operations)
- **Lớp học (LopHoc):**
  - Mở lớp học theo một khóa học cụ thể.
  - Phải chỉ định Giảng viên chủ nhiệm (`GiangVienChuNhiemId`) đang hoạt động.
  - Thiết lập ngày bắt đầu, ngày kết thúc dự kiến, sĩ số tối đa (từ 1 đến 200 học viên).
  - Tự động thống kê sĩ số hiện tại theo số lượng học viên được xếp lớp.
- **Xếp học viên vào lớp (DangKyLopHoc):**
  - Chọn học viên xếp vào lớp học đang mở.
  - **Quy tắc quan trọng (Snapshot học phí):** Tại thời điểm xếp lớp, hệ thống tự động ghi nhận mức học phí phải thu từ `HocPhiNiemYet` của khóa học tương ứng. Mọi thay đổi học phí khóa học trong danh mục sau này không ảnh hưởng đến số tiền phải nộp của các lượt đăng ký trước đó.
  - Ngăn chặn xếp vượt quá sĩ số tối đa của lớp học.
  - Ngăn chặn xếp trùng một học viên vào cùng một lớp học nhiều lần.
  - Chỉ cho phép hủy lượt xếp lớp khi học viên chưa phát sinh bất kỳ khoản đóng học phí nào.
- **Lịch học (LichHoc):**
  - Lập lịch cho từng buổi học của lớp (Môn học, Giảng viên giảng dạy, Phòng học, Ngày học, Khung giờ bắt đầu - kết thúc).
  - **Quy tắc chống xung đột lịch:** Hệ thống tự động kiểm tra trùng giờ:
    - Phòng học không bị lớp khác sử dụng cùng ngày và khung giờ chồng lấn (`StartA < EndB && EndA > StartB`).
    - Giảng viên không bị trùng lịch dạy lớp khác trong cùng thời điểm.

### 3.3. Phân Hệ Điểm Số (Grades)
- **Kết quả học tập (KetQuaHocTap):**
  - Giảng viên hoặc Quản trị viên nhập điểm tổng kết cho từng học viên theo môn học trong lớp.
  - Điểm số hợp lệ nằm trong thang điểm `0.00` đến `10.00`.
  - Giảng viên chỉ được phép nhập điểm cho các môn học/lớp học mà mình được phân công trong lịch giảng dạy.
  - Đánh giá kết quả Đạt / Không đạt được tính tự động (`DiemTongKet >= 5.00` là Đạt), không lưu cứng để tránh bất nhất dữ liệu.

### 3.4. Phân Hệ Học Phí & Phiếu Thu (Tuition & Receipts)
- **Theo dõi công nợ:**
  - `ConNo = HocPhiPhaiThu - SUM(PhieuThu hiệu lực)`.
  - Phân loại trạng thái thanh toán: "Hoàn thành", "Đóng một phần", "Chưa đóng".
- **Thu học phí:**
  - Hỗ trợ thanh toán từng phần hoặc toàn bộ bằng tiền mặt hoặc chuyển khoản (hệ thống chỉ hỗ trợ 2 hình thức này, không hỗ trợ thanh toán qua thẻ).
  - Ngăn chặn thu vượt quá số tiền còn nợ của học viên.
- **Hủy phiếu thu:**
  - Không xóa vật lý dữ liệu giao dịch tài chính. Chuyển trạng thái sang `DaHuy` (0) và bắt buộc ghi nhận lý do hủy phiếu để phục vụ kiểm toán nội bộ.

### 3.5. Phân Hệ Báo Cáo & Thống Kê (Reporting)
1. **Báo cáo danh sách học viên theo lớp:** Danh sách học viên, ngày đăng ký, học phí, số tiền đã đóng và số tiền còn nợ.
2. **Báo cáo lịch học theo lớp:** Lịch biểu chi tiết từng buổi học, môn học, giảng viên và phòng học.
3. **Báo cáo phân công giảng dạy theo giảng viên:** Lịch trình giảng dạy của giảng viên theo thời gian.
4. **Báo cáo kết quả học tập & Thống kê tỷ lệ đạt:** Chi tiết điểm từng học viên và tỷ lệ đạt/trượt theo từng môn học.
5. **Báo cáo công nợ học phí theo lớp:** Tổng sĩ số, tổng học phí phải thu, tổng đã thu, tổng nợ và tỷ lệ thu hồi học phí.

## 4. Yêu Cầu Phi Chức Năng (Non-Functional Requirements)
- **Bảo mật:** Mật khẩu người dùng băm bằng thuật toán chuẩn PBKDF2-SHA256 với 100.000 vòng lặp và muối ngẫu nhiên (salt 16 bytes).
- **Toàn vẹn dữ liệu:** Không xóa vật lý (soft-delete / soft-cancel) đối với các nghiệp vụ tài chính và lịch sử học tập. Sử dụng Transaction và `SET XACT_ABORT ON` đảm bảo tính nguyên tử (ACID).
- **Tính khả chuyển:** Hỗ trợ song song hai chế độ: kết nối Microsoft SQL Server thật qua ADO.NET hoặc chế độ Fake In-Memory phục vụ kiểm thử và demo nhanh mà không cần cài đặt SQL Server.
