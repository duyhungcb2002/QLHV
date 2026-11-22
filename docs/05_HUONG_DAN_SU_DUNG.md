# HƯỚNG DẪN SỬ DỤNG HỆ THỐNG QUẢN LÝ HỌC VIÊN (QLHV)

Tài liệu này cung cấp hướng dẫn thao tác chi tiết từng phân hệ chức năng trên giao diện Windows Forms của hệ thống QLHV.

---

## 1. Màn Hình Đăng Nhập & Điều Hướng

### 1.1 Đăng nhập
1. Khi khởi động ứng dụng, form đăng nhập xuất hiện.
2. Nhập **Tên đăng nhập** và **Mật khẩu** theo tài khoản được phân quyền (xem mục 5 trong `04_HUONG_DAN_CAI_DAT.md`).
3. Nhấn nút **Đăng nhập** (hoặc phím Enter).
4. Hệ thống kiểm tra xác thực, tải thông tin người dùng và phân quyền động lên thanh điều hướng chính.

### 1.2 Màn hình chính (`FrmMain`)
- **Header**: Hiển thị tên phần mềm, thông tin người dùng hiện tại (`Họ tên (Mã vai trò)`), badge môi trường hoạt động (`Môi trường: SQL Server` hoặc `Môi trường: Fake Data`) và nút **Đăng xuất**.
- **Sidebar điều hướng bên trái**: Chứa các nút chức năng tương ứng với quyền của tài khoản:
  - Danh mục: **Học viên**, **Giảng viên**, **Khóa học**, **Môn học**, **Phòng học**.
  - Đào tạo: **Đào tạo & Lớp học** (`FrmDaoTao`).
  - Điểm số: **Kết quả học tập** (`FrmDiem`).
  - Tài chính: **Học phí & Phiếu thu** (`FrmHocPhi`).
  - Thống kê: **Báo cáo tổng hợp** (`FrmBaoCao`).

---

## 2. Phân Hệ Đào Tạo & Lớp Học (`FrmDaoTao`)

Form gồm 3 Tab chức năng chính: **Quản lý Lớp học**, **Xếp lớp học viên**, và **Lịch học**.

### 2.1 Tab 1: Quản lý Lớp học
- **Xem danh sách**: Chọn Khóa học từ combobox bộ lọc để tải danh sách lớp học tương ứng. Bảng `dgvLopHoc` hiển thị rõ ràng: Mã lớp, Tên lớp, Khóa học, Giảng viên chủ nhiệm, Sĩ số hiện tại, Sĩ số tối đa, Ngày bắt đầu, Ngày kết thúc và Trạng thái.
- **Thêm lớp học mới**:
  1. Nhấn nút **Thêm lớp**.
  2. Form `FrmLopHocEdit` mở ra.
  3. Chọn Khóa học, nhập Mã lớp học (duy nhất), Tên lớp học, chọn **Giảng viên chủ nhiệm** (bắt buộc từ danh sách giảng viên đang hoạt động), Sĩ số tối đa (từ 1 đến 200), Ngày bắt đầu, Ngày kết thúc dự kiến và Ghi chú.
  4. Nhấn **Lưu dữ liệu**.
- **Sửa lớp học**: Chọn một dòng trong bảng và nhấn **Sửa lớp**. Có thể thay đổi Giảng viên chủ nhiệm, tên lớp, sĩ số tối đa, ngày học...
- **Trạng thái lớp**: Có thể chuyển trạng thái lớp sang *Đang hoạt động* hoặc *Ngừng hoạt động*.

### 2.2 Tab 2: Xếp học viên vào lớp (Đăng ký lớp)
- **Xem học viên trong lớp**: Chọn Khóa học và Lớp học cụ thể. Bảng `dgvDangKy` hiển thị tường minh các cột: Mã học viên, Họ tên, Điện thoại, Ngày đăng ký, Học phí snapshot, Đã nộp, Còn nợ và Trạng thái.
- **Thêm học viên vào lớp**:
  1. Nhấn nút **Thêm học viên vào lớp**.
  2. Hộp thoại `FrmChonHocVien` mở ra, hiển thị danh sách các học viên đang hoạt động trong hệ thống.
  3. Tìm kiếm học viên theo Mã hoặc Tên, chọn học viên và nhấn **Chọn học viên**.
  4. Hệ thống tự động kiểm tra:
     - Học viên đã có trong lớp chưa?
     - Sĩ số lớp hiện tại đã đạt mức tối đa (Sĩ số tối đa) chưa?
     - Nếu thỏa mãn, lưu bản ghi đăng ký với mức học phí niêm yết hiện thời của khóa học.
- **Xóa/Hủy học viên khỏi lớp**: Chọn học viên và nhấn **Hủy đăng ký**.

### 2.3 Tab 3: Lịch học
- **Xem lịch học**: Chọn Khóa học và Lớp học. Bảng `dgvLichHoc` hiển thị đầy đủ: Ngày học, Giờ bắt đầu, Giờ kết thúc, Môn học, Giảng viên giảng dạy, Phòng học và Trạng thái.
- **Thêm / Sửa lịch học**: Mở dialog `FrmLichHocEdit`, hệ thống tự động kiểm tra trùng giờ phòng học và trùng lịch giảng viên.

### 2.3 Tab 3: Quản lý Lịch học
- **Xem thời khóa biểu lớp**: Chọn Lớp học từ dropdown để xem danh sách các buổi học: Thứ, Ngày học, Ca học (Giờ bắt đầu - Giờ kết thúc), Môn học, Phòng học, Giảng viên đứng lớp.
- **Thêm buổi học**:
  1. Nhấn nút **Thêm buổi học**.
  2. Hộp thoại `FrmLichHocEdit` mở ra.
  3. Chọn Ngày học, Giờ bắt đầu, Giờ kết thúc, Môn học, Phòng học và Giảng viên.
  4. Nhấn **Lưu lịch học**. Hệ thống sẽ kiểm tra xung đột thời gian (Collision Check):
     - Phòng học có bị trùng giờ với lớp khác không?
     - Giảng viên có bị trùng lịch giảng dạy lớp khác không?
- **Hủy buổi học**: Chọn buổi học cần hủy và nhấn nút **Hủy buổi học**.

---

## 3. Phân Hệ Kết Quả Học Tập (`FrmDiem`)

Dành cho Giáo vụ, Quản trị viên và Giảng viên bộ môn.

### Quy trình nhập và cập nhật điểm:
1. Mở màn hình **Kết quả học tập**.
2. Chọn **Khóa học**, chọn **Lớp học**, sau đó chọn **Môn học** cần ghi nhận điểm.
3. Bảng dữ liệu tự động tải danh sách toàn bộ học viên trong lớp kèm điểm số hiện có:
   - Điểm tổng kết môn học (thang điểm từ 0.00 đến 10.00; để trống ô điểm nếu chưa chấm hoặc muốn xóa điểm).
   - Đạt / Không đạt (Điểm tổng kết $\ge 5.00$ là Đạt, $< 5.00$ là Không đạt, để trống là Chưa chấm).
4. Người dùng có thể chỉnh sửa trực tiếp điểm tổng kết trên lưới DataGridView (Điểm hợp lệ từ 0.00 đến 10.00).
5. Nhấn nút **Lưu điểm** để lưu nguyên tử bảng điểm vào cơ sở dữ liệu.

---

## 4. Phân Hệ Học Phí & Phiếu Thu (`FrmHocPhi`)

Dành cho Kế toán, Thu ngân và Ban quản lý.

### 4.1 Tra cứu công nợ học phí
1. Chọn Khóa học và Lớp học (hoặc để Tất cả để tra cứu toàn diện).
2. Tìm kiếm theo Mã học viên hoặc Tên học viên.
3. Bảng công nợ hiển thị chi tiết:
   - Tổng học phí phải nộp (theo snapshot đăng ký).
   - Tổng tiền đã nộp (tổng các phiếu thu còn hiệu lực).
   - Số tiền còn nợ.
   - Tình trạng: *Đã hoàn thành*, *Còn nợ*, hoặc *Chưa nộp*.

### 4.2 Thu học phí & Lập phiếu thu
1. Trên bảng công nợ, chọn học viên cần đóng tiền và nhấn **Lập phiếu thu**.
2. Form `FrmThuHocPhi` mở ra, tự động hiển thị số tiền còn nợ tối đa có thể đóng.
3. Nhập số tiền thu (không được vượt quá số tiền còn nợ), chọn Hình thức thanh toán (Tiền mặt, Chuyển khoản) và nhập Ghi chú.
4. Nhấn **Xác nhận lập phiếu thu**.
5. Phiếu thu mới được tạo lập ngay lập tức, công nợ học viên tự động giảm tương ứng.

### 4.3 Xem lịch sử phiếu thu & Hủy phiếu thu
- Tại phần **Lịch sử phiếu thu**, xem danh sách toàn bộ phiếu thu kèm thông tin Người lập, Ngày thu, Số tiền, Hình thức và Trạng thái (*HieuLuc* hoặc *DaHuy*).
- Để hủy một phiếu thu do nhập sai:
  1. Chọn phiếu thu có trạng thái *HieuLuc*.
  2. Nhấn nút **Hủy phiếu thu**.
  3. Nhập lý do hủy bắt buộc vào hộp thoại xác nhận.
  4. Hệ thống thực hiện Soft-Cancel (đổi trạng thái sang *DaHuy*, lưu lý do hủy và thời gian hủy). Số tiền đã đóng của học viên tự động được khôi phục về trạng thái công nợ mà không làm mất vết kiểm toán tài chính.

---

## 5. Phân Hệ Báo Cáo Tổng Hợp (`FrmBaoCao`)

Cung cấp 5 nhóm báo cáo phục vụ công tác quản lý và ra quyết định:

1. **Tab 1: Báo cáo Học viên theo lớp (`tabHocVienLop`)**:
   - Chọn lớp học từ danh sách, hiển thị danh sách toàn bộ học viên trong lớp: Mã học viên, Họ tên, Ngày sinh, Email, Ngày đăng ký, Học phí phải thu, Đã thu và Còn nợ.
2. **Tab 2: Báo cáo Lịch học theo lớp (`tabLichHocLop`)**:
   - Chọn lớp học từ danh sách, hiển thị toàn bộ lịch học của lớp: Ngày học, Khung giờ học, Môn học, Giảng viên, Phòng học, Nội dung buổi học.
3. **Tab 3: Báo cáo Phân công giảng viên (`tabPhanCongGV`)**:
   - Chọn giảng viên từ danh sách, hiển thị toàn bộ lịch trình giảng dạy của giảng viên: Lớp học, Môn học, Phòng học, Ngày học, Khung giờ và Nội dung.
4. **Tab 4: Báo cáo Kết quả học tập & Thống kê tỷ lệ đạt (`tabKetQuaHocTap`)**:
   - Lọc theo Lớp học và Môn học (hoặc tất cả các môn thuộc khóa học của lớp).
   - Lưới chi tiết: Điểm từng học viên (Mã HV, Họ tên, Môn học, Điểm tổng kết, Kết quả Đạt/Không đạt, Xếp loại).
   - Lưới thống kê: Tỷ lệ theo từng môn học (Tổng số học viên, Đã chấm, Chưa nhập, Số lượng đạt, Tỷ lệ đạt %).
5. **Tab 5: Báo cáo Công nợ học phí theo lớp (`tabCongNoHocPhi`)**:
   - Tổng hợp công nợ của toàn bộ các lớp trong hệ thống: Mã lớp, Tên lớp, Khóa học, Sĩ số hiện tại, Tổng học phí phải thu, Tổng tiền đã thu thực tế, Tổng nợ còn lại và Tỷ lệ thu hồi học phí (%).
## 6. Quản Lý Hồ Sơ Học Viên & Tra Cứu Chi Tiết (`FrmHocVien`, `FrmHocVienEdit`, `FrmHocVienDetail`)

### 6.1 Danh sách học viên (`FrmHocVien`)
- Danh sách hiển thị các thông tin: Mã HV, Họ tên, Ngày sinh, Giới tính, Điện thoại, Email, Số CCCD/CMND đã che mờ dạng `********1234` nhằm bảo mật dữ liệu cá nhân (chỉ tài khoản có quyền `CatalogManage` mới được xem đầy đủ), Nguồn tuyển sinh, Nhân viên phụ trách và Trạng thái.
- **Tìm kiếm**: Tìm kiếm nhanh theo Mã, Họ tên, Điện thoại hoặc Số CCCD/CMND (hỗ trợ tiếng Việt có dấu và không dấu).
- **Xem chi tiết**: Chọn một học viên và nhấn **Xem chi tiết** để mở `FrmHocVienDetail`.

### 6.2 Chỉnh sửa hồ sơ 5 Tab (`FrmHocVienEdit`)
Giao diện form được thiết kế với 5 tab chuyên biệt:
1. **Tab 1: Thông tin cơ bản**: Mã học viên, Họ tên (bắt buộc), Ngày sinh, Giới tính, Điện thoại (bắt buộc), Email, Địa chỉ, Ngày đăng ký và Trạng thái hoạt động.
2. **Tab 2: Giấy tờ tùy thân**: Số CCCD/CMND (9 hoặc 12 số, không trùng lặp), Ngày cấp (không được lớn hơn ngày hiện tại), Nơi cấp. *Quy tắc:* Phải nhập đồng thời cả 3 trường hoặc để trống cả 3 trường.
3. **Tab 3: Người giám hộ**: Họ tên người giám hộ, Điện thoại người giám hộ, Quan hệ với học viên (Bố, Mẹ, Người bảo hộ...). *Quy tắc:* Nếu học viên dưới 18 tuổi tính đến thời điểm hiện tại, bắt buộc phải nhập đủ thông tin người giám hộ.
4. **Tab 4: Nghề nghiệp & Tuyển sinh**: Nghề nghiệp, Đơn vị công tác, Nguồn tuyển sinh (chọn từ combobox các nguồn chuẩn: Website, Facebook, Giới thiệu, Trực tiếp...), Nhân viên phụ trách (chọn từ danh sách tài khoản thuộc vai trò ADMIN hoặc DAOTAO đang hoạt động).
5. **Tab 5: Ghi chú**: Ghi chú đặc biệt (tối đa 500 ký tự) với bộ đếm ký tự trực quan thời gian thực `(Đã nhập: X/500 ký tự)`.

### 6.3 Màn hình chi tiết học viên & Lịch sử đào tạo (`FrmHocVienDetail`)
- **Thông tin đầy đủ**: Hiển thị rõ toàn bộ thông tin cá nhân, giấy tờ, người giám hộ, tuyển sinh và ghi chú.
- **Xem/Ẩn số CCCD/CMND**: Nút bấm tiện ích cho phép chuyển đổi giữa dạng che mờ (`********1234`) và dạng đầy đủ (`001098011234`). Chức năng này chỉ hiển thị và hoạt động đối với người dùng có quyền `CatalogManage`.
- **Lịch sử đào tạo (`dgvLichSuDaoTao`)**: Tự động tra cứu từ `DangKyLopHoc` toàn bộ các lớp mà học viên đã ghi danh kèm thông tin: Mã lớp, Tên lớp, Khóa học, Ngày ghi danh, Học phí snapshot, Đã đóng, Còn nợ và Trạng thái lớp. Hiển thị thông báo thân thiện nếu học viên chưa ghi danh lớp nào.
