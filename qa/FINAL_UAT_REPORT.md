# BÁO CÁO NGHIỆM THU TỔNG THỂ (FINAL UAT REPORT)

- **Dự án:** Hệ thống Quản lý Học viên (QLHV)
- **Phiên bản:** 2.1.2 (Hardened Mega MVP & Final Audit 05C - Đầy đủ 5 phân hệ Đào tạo, Lớp học, Lịch học, Điểm số, Học phí, Báo cáo & Mở rộng Hồ sơ học viên, Giảng viên chủ nhiệm, Bảo mật CCCD)
- **Thời điểm thực hiện:** 22/11/2026
- **Người thực hiện:** Phạm Duy Hưng (Lead Architecture / Core & Integration)
- **Trạng thái kiểm thử thủ công:** `MANUAL_UI_UAT_NOT_VERIFIED` (Môi trường console tự động hóa không có Interactive Desktop Session; toàn bộ logic nghiệp vụ được bảo vệ bởi 164 bài kiểm thử tự động đạt 100%).

---

## 1. Môi Trường Vận Hành & Kiểm Thử

- **Hệ điều hành:** Windows 10/11 (64-bit)
- **Nền tảng:** .NET 10 (`net10.0-windows` cho WinForms, `net10.0` cho các thư viện Core/Contracts/Infrastructure)
- **Cơ sở dữ liệu:** Microsoft SQL Server Express (`.\SQLEXPRESS`)
- **Công nghệ truy xuất:** ADO.NET thuần (`Microsoft.Data.SqlClient`), không dùng ORM
- **Cơ chế xác thực:** Windows Authentication (`Integrated Security=True;TrustServerCertificate=True`)
- **Database vận hành ứng dụng:** `QLHV` (đã chạy migration 001 đến 007)
- **Database kiểm thử tự động:** `QLHV_Test` (Cô lập hoàn toàn, đã chạy migration 001 đến 007)
- **Chuỗi kết nối kiểm thử:**
  ```text
  Server=.\SQLEXPRESS;Database=QLHV_Test;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30
  ```

---

## 2. Kết Quả Biên Dịch (Build Results)

Thực hiện biên dịch lại toàn bộ solution ở cả hai chế độ cấu hình:

| Cấu hình | Lệnh thực thi | Kết quả | Cảnh báo | Lỗi |
|:---|:---|:---:|:---:|:---:|
| **Debug** | `dotnet build QLHV.sln -c Debug --nologo` | **THÀNH CÔNG** | 0 compiler/analyzer warning (*) | 0 Error |
| **Release** | `dotnet build QLHV.sln -c Release --nologo` | **THÀNH CÔNG** | 0 compiler/analyzer warning (*) | 0 Error |

- Không phát sinh file tạm, file rác hoặc project thử nghiệm ngoài phạm vi solution.
- 0 compiler/analyzer warning, 0 error; cảnh báo NU1900 có thể xuất hiện khi môi trường không truy cập được nguồn kiểm tra lỗ hổng NuGet.
- Kiểm tra chuẩn định dạng mã nguồn: `git diff --check` đạt 100% (không có lỗi whitespace, trailing space hay conflict marker).

---

## 3. Kết Quả Kiểm Thử Tự Động (Automated Test Results)

Toàn bộ 164 bài kiểm thử MSTest được thực thi và vượt qua tuyệt đối (100% PASSED):

| Nhóm kiểm thử | Bộ lọc (Filter) | Số lượng | Kết quả | Thời gian |
|:---|:---|:---:|:---:|:---:|
| **Non-SQL Tests** | `TestCategory!=SqlServer` | 138 | **138/138 PASSED** | ~4 giây |
| **SQL Integration Tests** | `TestCategory=SqlServer` | 26 | **26/26 PASSED** | ~3 giây |
| **Toàn bộ Test Suite** | *(Tất cả)* | 164 | **164/164 PASSED** | ~6 giây |

### Phân rã chi tiết các nhóm kiểm thử:
1. **Xác thực & Bảo mật (Auth & Security - SEC-01):** Hashing mật khẩu PBKDF2, phân biệt sai mật khẩu và tài khoản bị khóa, ma trận quyền theo vai trò (`IAuthorizationService`), phân quyền 2 lớp tại tầng Service (`AuthorizationGuard`).
2. **Bảo mật CCCD tầng Service:** Mặt nạ định dạng chuẩn `********1234` (giữ 4 chữ số cuối), DTO trả về tầng giao diện đã được che đối với các tài khoản không có quyền `CatalogManage` (chỉ quyền `CatalogManage` mới được xem số đầy đủ).
3. **WinForms UI Smoke Tests:** Khởi tạo không lỗi đối với toàn bộ các Form giao diện (`FrmLogin`, `FrmMain`, `FrmHocVien`, `FrmGiangVien`, `FrmMonHoc`, `FrmKhoaHoc`, `FrmPhongHoc`, `FrmDaoTao`, `FrmDiem`, `FrmHocPhi`, `FrmBaoCao`, `FrmLopHocEdit`, `FrmChonHocVien`, `FrmLichHocEdit`, `FrmThuHocPhi`, `FrmHocVienEdit`, `FrmHocVienDetail`), kiểm tra tiêu đề tiếng Việt chuẩn, DataGridView columns tường minh và cell formatting.
4. **Cấu hình & Điểm khởi động (AppConfig & Composition Root):** Đọc cấu hình `appsettings.json`, ghi đè bằng `appsettings.Local.json` và biến môi trường, kiểm tra kết nối an toàn lúc khởi động (`StartupConnectionTester`), kiểm tra bắt buộc cấu trúc migration 006 (11 cột hồ sơ học viên, cột GVCN lớp, filtered unique index, 2 FKs), từ chối nhầm database `QLHV_Test`, không tự ý fallback sang Fake khi SQL Server lỗi.
5. **Bảo mật Logging (FileAppLogger Sanitization):** Làm sạch toàn diện các thông tin nhạy cảm (`Password`, `Pwd`, `User ID`, `UID`, `Connection String`) thành `[REDACTED]`, hỗ trợ giá trị nháy đơn, nháy kép, khoảng trắng và dấu chấm phẩy.
6. **Nghiệp vụ Đào tạo, Xếp lớp & Lịch học:** Giới hạn sĩ số (1 - 200), snapshot học phí tại thời điểm ghi danh, chống xếp lớp trùng học viên, chống ghi danh vượt sĩ số, kiểm tra giao thoa lịch học chống trùng phòng và trùng giảng viên.
7. **Nghiệp vụ Điểm số & Học phí:** Thang điểm 0.00 - 10.00, tự động xếp loại Đạt/Không đạt (ngưỡng 5.00), phân biệt điểm 0.00 và điểm null (chưa chấm), xóa điểm khi ô điểm để trắng, thu học phí ngăn chặn thu vượt công nợ, hủy phiếu thu an toàn có lưu vết lý do hủy (Soft-Cancel audit).
8. **SQL Server Integration Tests (26 bài chạy thật trên QLHV_Test):**
   - 13 SQL Repositories ADO.NET: Tài khoản, Học viên, Giảng viên, Môn học, Khóa học, Khóa-Môn, Phòng học, Lớp học, Đăng ký lớp, Lịch học, Kết quả học tập, Phiếu thu, Báo cáo.
   - Nguyên tử hóa cập nhật lớp học (`UpdateAtomicAsync`): Bảo toàn lịch active khi đổi tên, từ chối thu hẹp ngày làm lịch nằm ngoài khoảng, từ chối tăng sĩ số vượt sức chứa phòng, từ chối đổi khóa học khi đã có dữ liệu lịch sử.
   - Nguyên tử hóa ngừng hoạt động lớp học (`DeactivateAtomicAsync`): Ngừng thành công lớp trống; từ chối ngừng lớp khi còn học viên đang học; từ chối ngừng lớp khi còn lịch active; tranh chấp đồng thời (`Concurrency`) giữa ngừng lớp và xếp lớp được xử lý an toàn.
   - Đồng bộ hợp đồng khóa học: `EnrollAtomicAsync` và `ThuHocPhiAtomicAsync` trả về đầy đủ `KhoaHocId` và `TenKhoaHoc`.
   - Migration 007 UTF-8 BOM, an toàn với chuỗi tiếng Việt hợp lệ (dùng collation nhị phân chính xác `COLLATE Latin1_General_BIN`), chứng minh tính lũy thừa khi chạy lặp lại.
   - Xác minh toàn diện cấu trúc migration 006 trên database thật thông qua `StartupConnectionTester.CheckMigration006`.

---

## 4. Nghiệm Thu Logic & Phân Quyền Theo Từng Vai Trò

Dựa trên mã nguồn, cấu hình RBAC và bộ kiểm thử tự động, từng vai trò người dùng được xác minh nghiêm ngặt:

### 4.1 Vai trò Quản Trị Viên (`ADMIN` - `admin / Admin@123`)
- **Quyền hạn:** Toàn quyền trên mọi phân hệ (CatalogView, CatalogManage, TrainingView, TrainingManage, TuitionManage, GradeEntry, ReportView).
- **Trạng thái:** Toàn bộ menu và nút bấm hiển thị đầy đủ; thực thi đầy đủ các chức năng thêm, sửa, ngừng hoạt động và xem báo cáo.

### 4.2 Vai trò Chuyên Viên Đào Tạo (`DAOTAO` - `daotao / DaoTao@123`)
- **Quyền hạn:** CatalogView, CatalogManage, TrainingView, TrainingManage.
- **Ranh giới bảo vệ:**
  - Được phép: Mở lớp học, sửa lớp học, xếp học viên vào lớp, lập và sửa lịch học, quản lý danh mục.
  - Chặn chặt chẽ: Không có quyền nhập điểm (`GradeEntry`), không có quyền thu học phí (`TuitionManage`), không có quyền xem báo cáo thống kê (`ReportView`). Tầng Service chặn bằng mã lỗi `FORBIDDEN` nếu cố ý can thiệp.

### 4.3 Vai trò Giảng Viên (`GIANGVIEN` - `giangvien / GiangVien@123`)
- **Quyền hạn:** CatalogView, GradeEntry.
- **Ranh giới bảo vệ:**
  - Được phép: Xem danh mục (chỉ đọc), xem lịch giảng dạy của cá nhân, nhập/sửa bảng điểm cho lớp và môn học được phân công.
  - Chặn chặt chẽ: Không được truy cập phân hệ Quản lý đào tạo (`FrmDaoTao`), không được quản lý học phí (`FrmHocPhi`), không được mở báo cáo (`FrmBaoCao`).
  - Điểm 0.00 được lưu trữ chính xác là 0.00 (không đạt); để trắng ô điểm sẽ xóa bản ghi điểm khỏi DB (trở về trạng thái "Chưa nhập").

### 4.4 Vai trò Nhân Viên Thu Ngân (`THUNGAN` - `thungan / ThuNgan@123`)
- **Quyền hạn:** CatalogView, TuitionManage.
- **Ranh giới bảo vệ:**
  - Được phép: Xem danh mục (chỉ đọc), tra cứu công nợ học viên theo lớp, lập phiếu thu thanh toán (tiền mặt / chuyển khoản), hủy phiếu thu có ghi nhận lý do.
  - Chặn chặt chẽ: Chặn số tiền thu <= 0 hoặc vượt quá số nợ còn lại; không được thao tác quản lý đào tạo, không được nhập điểm và không được xem báo cáo đào tạo.

### 4.5 Vai trò Ban Quản Lý (`QUANLY` - `quanly / QuanLy@123`)
- **Quyền hạn:** CatalogView, ReportView.
- **Ranh giới bảo vệ:**
  - Được phép: Xem danh mục (chỉ đọc), xem toàn bộ 5 chuyên đề báo cáo thống kê:
    1. Báo cáo danh sách học viên theo lớp.
    2. Báo cáo lịch học theo lớp.
    3. Báo cáo phân công giảng dạy theo giảng viên.
    4. Báo cáo kết quả học tập & Thống kê tỷ lệ đạt từng môn học.
    5. Báo cáo công nợ học phí theo lớp.
  - Chặn chặt chẽ: Không có quyền sửa danh mục, không có quyền mở lớp/xếp lớp, không có quyền nhập điểm hay thu học phí.

---

## 5. Danh Sách Lỗi Đã Phát Hiện Và Phương Án Khắc Phục

Trong quá trình rà soát và hardening toàn diện, các lỗi kỹ thuật sau đã được phát hiện và xử lý dứt điểm:

1. **Lỗi cú pháp SQL trong `SqlLopHocRepository.UpdateAtomicAsync`:**
   - *Phát hiện:* Câu lệnh đếm điểm số sử dụng `WHERE LopHocId = @Id` trong khi bảng `KetQuaHocTap` không có cột `LopHocId`; tên cột phòng học dùng nhầm `p.TenPhongHoc`.
   - *Khắc phục:* Đổi thành `INNER JOIN` qua `DangKyLopHoc` (`WHERE dk.LopHocId = @Id`) và đổi thành `p.TenPhong`.
2. **Lỗi `SqlDataReader` chưa đóng khi rollback transaction:**
   - *Phát hiện:* Khi kiểm tra vi phạm sức chứa phòng học, `capReader` vẫn mở trên connection dẫn đến exception *"There is already an open DataReader associated with this Connection which must be closed first"*.
   - *Khắc phục:* Đọc dữ liệu ra biến cục bộ và đóng reader hoàn toàn trước khi gọi `transaction.RollbackAsync()`.
3. **Thiếu ràng buộc nguyên tử khi ngừng hoạt động lớp học:**
   - *Phát hiện:* Chưa có phương thức nguyên tử kiểm tra đồng thời cả học viên active và lịch học active trước khi cập nhật trạng thái lớp.
   - *Khắc phục:* Bổ sung `DeactivateAtomicAsync` vào `ILopHocRepository`, triển khai transaction mức `Serializable` với `UPDLOCK, HOLDLOCK` trên SQL Server và lock đồng bộ trên Fake repository.
4. **Xử lý ô điểm để trắng (Blank cell) trong batch grading:**
   - *Phát hiện:* Khi người dùng xóa trắng điểm trên DataGridView để hủy điểm đã nhập, batch update bỏ qua các dòng null thay vì xóa bản ghi điểm tương ứng.
   - *Khắc phục:* Tách danh sách `toUpsert` và `toDeleteIds` trong `KetQuaHocTapService.SaveGradesBatchAsync`, thực hiện xóa nguyên tử các bản ghi điểm có giá trị null.
5. **Thông báo lỗi thân thiện tại điểm khởi động:**
   - *Phát hiện:* `StartupConnectionTester` trước đây có thể làm lộ exception kỹ thuật thô ra màn hình `MessageBox`.
   - *Khắc phục:* Bổ sung thuộc tính `UserFriendlyMessage` với câu từ tiếng Việt rõ ràng, giữ chi tiết kỹ thuật đã sanitize trong file log.
6. **Sửa an toàn migration Unicode 007 (`database/007_unicode_data_repair.sql`):**
   - *Phát hiện:* Các điều kiện pattern rộng như `LIKE '%Ã%'`, `LIKE '%Â%'` có thể khớp nhầm các chuỗi tiếng Việt hợp lệ (ví dụ `"Âm thanh cần hỗ trợ"`).
   - *Khắc phục:* Viết lại toàn bộ câu lệnh UPDATE sử dụng điều kiện kết hợp mã nghiệp vụ chính xác và collation nhị phân `COLLATE Latin1_General_BIN IN (...)`, lưu file với UTF-8 BOM (`EF BB BF`).
7. **Kiểm tra bắt buộc cấu trúc migration 006 tại điểm khởi động:**
   - *Phát hiện:* `StartupConnectionTester` trước đây chỉ kiểm tra danh sách bảng cơ sở, chưa xác minh các cột hồ sơ mới, foreign key và filtered unique index của migration 006.
   - *Khắc phục:* Bổ sung hàm `CheckMigration006` kiểm tra 11 cột hồ sơ trong `dbo.HocVien`, cột `GiangVienChuNhiemId` trong `dbo.LopHoc`, index `UX_HocVien_SoDinhDanh`, khóa ngoại `FK_HocVien_NhanVienPhuTrach` và `FK_LopHoc_GiangVienChuNhiem`.
8. **Bảo vệ số định danh CCCD tại tầng Service:**
   - *Phát hiện:* Việc che giấu CCCD trước đây chủ yếu dựa vào thuộc tính tính toán `SoDinhDanhMasked` trên UI, DTO trả về cho tài khoản chỉ xem vẫn chứa CCCD thô.
   - *Khắc phục:* Tại `HocVienService`, phương thức `GetAllAsync` và `GetByIdAsync` tự động thay thế `SoDinhDanh` bằng mặt nạ chuẩn `********1234` (giữ 4 chữ số cuối) nếu người dùng không có quyền `CatalogManage`, DTO trả về tầng giao diện đã được che đối với tài khoản không có quyền CatalogManage.
9. **Đồng bộ hợp đồng dữ liệu Khóa học (Course Contract):**
   - *Phát hiện:* Một số câu truy vấn và phương thức atomic trong `SqlDangKyLopHocRepository`, `SqlPhieuThuRepository`, `SqlKetQuaHocTapRepository` và Fake Store chưa map `KhoaHocId` và `TenKhoaHoc`.
   - *Khắc phục:* Bổ sung `INNER JOIN dbo.KhoaHoc` và gán đầy đủ `KhoaHocId`, `TenKhoaHoc` vào tất cả DTO trả về từ `EnrollAtomicAsync`, `ThuHocPhiAtomicAsync`, `UpsertBatchAsync`, và seed Fake Store.
10. **Gán tên cột DataGridView trong `FrmHocVienDetail`:**
    - *Phát hiện:* Các cột trong `dgvLichSu` thiếu gán thuộc tính `.Name`, dẫn đến sự kiện `CellFormatting` kiểm tra theo tên cột không bao giờ khớp.
    - *Khắc phục:* Gán tên tường minh cho toàn bộ 8 cột (`colMaLop`, `colTenLop`, `colTenKhoaHoc`, `colNgayDangKy`, `colTrangThai`, `colHocPhiPhaiThu`, `colDaThu`, `colConNo`).

---

## 6. Minh Chứng Ảnh Giao Diện (Screenshots Status)

- **Trạng thái:** Không tạo ảnh chụp màn hình giả lập.
- **Lý do kỹ thuật:** Phiên làm việc hiện tại được thực thi trong môi trường dòng lệnh (non-interactive console session) trên máy chủ/môi trường tự động, không có quyền truy cập vào Windows Interactive Desktop (Session 1 GUI Desktop) để hiển thị form trực quan và gọi API chụp màn hình.
- **Giải pháp bảo đảm chất lượng:**
  - Toàn bộ giao diện được kiểm tra khởi tạo thông qua 16 ca kiểm thử `UiSmokeTests`.
  - Cung cấp danh mục các bước thực hiện chi tiết dưới đây để người dùng/kiểm thử viên tự thực hiện nghiệm thu trực quan trên màn hình máy tính thật.

---

## 7. Hướng Dẫn Các Bước Nghiệm Thu Thủ Công Dành Cho Người Dùng

Người dùng mở ứng dụng bằng lệnh:
```powershell
dotnet run --project src\QLHV.WinForms\QLHV.WinForms.csproj
# Hoặc chạy file: src\QLHV.WinForms\bin\Release\net10.0-windows\win-x64\publish\QLHV.WinForms.exe
```

1. **Bước 1: Đăng nhập `admin / Admin@123`**
   - Quan sát badge môi trường: `Môi trường: SQL Server`.
   - Mở lần lượt các danh mục: Học viên, Giảng viên, Môn học, Khóa học, Phòng học. Thêm thử 1 học viên mới và sửa thông tin.
2. **Bước 2: Đăng nhập `daotao / DaoTao@123`**
   - Mở **Đào tạo & Lớp học** (`FrmDaoTao`).
   - Tab 1: Tạo một lớp học mới. Thử sửa tên lớp. Thử ngừng hoạt động lớp khi có học viên (quan sát thông báo bị chặn).
   - Tab 2: Xếp học viên vào lớp. Quan sát mức học phí hiển thị đúng snapshot khóa học. Thử thêm học viên đã có trong lớp (quan sát thông báo chặn).
   - Tab 3: Thêm buổi học mới. Thử tạo 2 buổi học trùng phòng hoặc trùng giảng viên trong cùng khung giờ (quan sát thông báo xung đột).
   - Xác nhận: Nút Kết quả học tập, Học phí và Báo cáo bị khóa/ẩn theo quyền.
3. **Bước 3: Đăng nhập `giangvien / GiangVien@123`**
   - Mở **Kết quả học tập** (`FrmDiem`).
   - Chọn lớp và môn học được phân công.
   - Nhập điểm 0.00 cho một học viên -> Lưu điểm -> Quan sát hiển thị 0.00 và xếp loại "Không đạt".
   - Xóa trắng ô điểm của một học viên -> Lưu điểm -> Quan sát ô điểm trở về trạng thái "Chưa chấm" / chưa có điểm.
   - Xác nhận: Không thể mở phân hệ Đào tạo, Học phí, Báo cáo.
4. **Bước 4: Đăng nhập `thungan / ThuNgan@123`**
   - Mở **Học phí & Phiếu thu** (`FrmHocPhi`).
   - Chọn học viên còn nợ -> Nhấn Lập phiếu thu -> Nhập số tiền thu một phần bằng Tiền mặt -> Xác nhận.
   - Quan sát công nợ giảm tương ứng.
   - Chọn phiếu thu vừa lập trong Lịch sử phiếu thu -> Nhấn Hủy phiếu thu -> Nhập lý do hủy -> Xác nhận.
   - Quan sát phiếu thu chuyển trạng thái `DaHuy`, công nợ học viên tự động tăng trở lại.
   - Xác nhận: Không thể mở phân hệ Đào tạo, Điểm, Báo cáo.
5. **Bước 5: Đăng nhập `quanly / QuanLy@123`**
   - Mở **Báo cáo tổng hợp** (`FrmBaoCao`).
   - Kiểm tra đủ 5 tab báo cáo:
     - Tab 1: Học viên theo lớp.
     - Tab 2: Lịch học theo lớp.
     - Tab 3: Phân công giảng viên.
     - Tab 4: Kết quả học tập & Thống kê tỷ lệ đạt.
     - Tab 5: Công nợ học phí theo lớp.
   - Xác nhận: Số liệu hiển thị chính xác, không xuất hiện báo cáo ngoài phạm vi.

---

## 8. Đóng Gói Bản Phát Hành (Release Package)

Ứng dụng đã được đóng gói thành công ở chế độ Release độc lập cho nền tảng `win-x64`:
- **Đường dẫn thư mục publish:**
  ```text
  D:\HMNU\Laptrinhwindow\QLHV\src\QLHV.WinForms\bin\Release\net10.0-windows\win-x64\publish\
  ```
- **File thực thi chính:** `QLHV.WinForms.exe`
- **Tệp cấu hình:** `appsettings.json` (chế độ `SqlServer`, trỏ database `QLHV`, không chứa mật khẩu rõ).
- **Đặc điểm gói:** Không chứa database kiểm thử `QLHV_Test`, không chứa file tạm hay log cá nhân, sẵn sàng chạy ngay trên môi trường Windows có cài đặt .NET 10 Desktop Runtime và kết nối SQL Server Express.

---

## 9. Đánh Giá Rủi Ro Còn Lại (Residual Risks)

1. **Dịch vụ SQL Server trên máy đích:** Nếu máy tính của người dùng chưa bật dịch vụ SQL Server Express hoặc cấu hình sai tên instance, ứng dụng sẽ hiển thị thông báo lỗi thân thiện và dừng khởi động (không tự động fallback sang Fake). Khuyến nghị người dùng kiểm tra dịch vụ MSSQL Server trước khi chạy.
2. **Kích thước màn hình độ phân giải thấp:** Giao diện WinForms thiết kế tối ưu cho độ phân giải từ 1366x768 trở lên. Nếu chạy ở độ phân giải quá thấp có thể cần cuộn thanh cuộn của Form.

---

## 10. Kết Luận Nghiệm Thu

Hệ thống Quản lý Học viên (QLHV) đã vượt qua toàn bộ 164 bài kiểm thử tự động (138 non-SQL + 26 SQL Server Integration Tests), biên dịch sạch 0 cảnh báo/0 lỗi ở cả Debug và Release, đóng gói bản phát hành thành công và hoàn thiện toàn bộ hồ sơ tài liệu kỹ thuật.

**KẾT LUẬN CHÍNH THỨC:**
**`READY_FOR_MANUAL_ACCEPTANCE`**
*(Hệ thống sẵn sàng để hội đồng / người dùng cuối thực hiện phiên nghiệm thu chấp nhận giao diện trực tiếp trên máy tính).*
