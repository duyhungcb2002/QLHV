# PHÂN TÍCH & THIẾT KẾ KIẾN TRÚC - HỆ THỐNG QUẢN LÝ HỌC VIÊN (QLHV)

## 1. Kiến Trúc Tổng Thể (Layered Architecture)
Hệ thống được thiết kế theo mô hình phân tầng chặt chẽ (Clean / Layered Architecture) nhằm phân tách rõ trách nhiệm giữa các thành phần:

```
[QLHV.WinForms (Presentation / UI)]
         │
         ▼
[QLHV.Features.Catalog & QLHV.Features.Training (Application Services)]
         │
         ▼
[QLHV.Core (Domain Contracts, Abstractions, Security, Validation)]
         ▲
         │ (Implements Interfaces)
[QLHV.Infrastructure.SqlServer (Data Access / ADO.NET)]
```

### 1.1. Các Project Trong Solution
1. **`QLHV.Contracts`:**
   - Chứa các Data Transfer Objects (DTO), Enums, Query Filters, và kết quả chuẩn hóa `OperationResult<T>`.
   - Không chứa bất kỳ logic nghiệp vụ hoặc thư viện ngoài nào.
2. **`QLHV.Core`:**
   - Chứa Repository Interfaces (`IHocVienRepository`, `ILopHocRepository`,...), Service Interfaces (`ILopHocService`,...), Security (`IPasswordHasher`, `AuthorizationGuard`), Database abstraction (`IConnectionFactory`), Logging (`IAppLogger`), và Helper validation.
3. **`QLHV.Features.Catalog`:**
   - Xử lý nghiệp vụ danh mục cơ sở (Học viên, Giảng viên, Môn học, Khóa học, Phòng học, Xác thực đăng nhập).
   - Chứa các Fake Repositories danh mục in-memory.
4. **`QLHV.Features.Training`:**
   - Xử lý toàn bộ nghiệp vụ vận hành: Lớp học, Xếp lớp học viên, Lịch học, Kết quả học tập, Học phí và Báo cáo.
   - Chứa `FakeTrainingStore` (bộ nhớ chia sẻ luồng an toàn) và 6 Fake repositories.
5. **`QLHV.Infrastructure.SqlServer`:**
   - Triển khai toàn bộ các Repository interfaces bằng công nghệ ADO.NET thuần trên Microsoft SQL Server.
   - Quản lý kết nối, giao dịch, tham số hóa SQL (chống SQL Injection), và chuyển đổi dữ liệu an toàn.
6. **`QLHV.WinForms`:**
   - Tầng giao diện người dùng Windows Forms, đóng vai trò Composition Root (tập trung khởi tạo các dịch vụ tại `Program.cs` thông qua Constructor Injection, không dùng thư viện DI cồng kềnh).
7. **`QLHV.Tests`:**
   - Dự án kiểm thử tự động toàn diện: Unit tests, Security authorization tests, UI smoke tests và SQL Server integration tests.

## 2. Thiết Kế Luồng Xử Lý Nghiệp Vụ

### 2.1. Luồng Mở Lớp & Xếp Học Viên
```mermaid
sequenceDiagram
    autonumber
    actor CB as Cán bộ Đào tạo
    participant UI as FrmDaoTao
    participant Svc as DangKyLopHocService
    participant LopRepo as ILopHocRepository
    participant DkRepo as IDangKyLopHocRepository
    participant DB as SQL Server

    CB->>UI: Chọn lớp học & bấm "Xếp học viên"
    UI->>CB: Hiển thị FrmChonHocVien
    CB->>UI: Chọn học viên HV001
    UI->>Svc: EnrollAsync(LopHocId, HocVienId)
    Svc->>Svc: Kiểm tra quyền TrainingManage
    Svc->>LopRepo: Lấy thông tin lớp học & sĩ số tối đa
    Svc->>DkRepo: Đếm số lượng học viên hiện tại
    alt Sĩ số hiện tại >= Sĩ số tối đa
        Svc-->>UI: Lỗi: Đã đạt sĩ số tối đa của lớp
    else Kiểm tra trùng lặp
        Svc->>DkRepo: GetByLopAndHocVienAsync()
        alt Đã có tên trong lớp
            Svc-->>UI: Lỗi: Học viên đã đăng ký lớp này
        else Đủ điều kiện xếp lớp
            Svc->>Svc: Snapshot học phí từ KhoaHoc.HocPhiNiemYet
            Svc->>DkRepo: AddAsync(DangKyLopHocDto)
            DkRepo->>DB: INSERT INTO DangKyLopHoc
            DB-->>DkRepo: Thành công (Id mới)
            Svc-->>UI: OperationResult.Success(dto)
            UI-->>CB: Thông báo xếp lớp thành công
        end
    end
```

### 2.2. Luồng Lập Lịch & Chống Xung Đột
- Khi tạo/cập nhật buổi học:
  $$\text{Xung đột nếu: } (\text{Start}_{\text{mới}} < \text{End}_{\text{cũ}}) \land (\text{End}_{\text{mới}} > \text{Start}_{\text{cũ}})$$
- Kiểm tra áp dụng đồng thời cho cả **Phòng học** và **Giảng viên** trong cùng một ngày.

### 2.3. Luồng Thu Học Phí & Công Nợ
```mermaid
sequenceDiagram
    autonumber
    actor TN as Thu ngân
    participant UI as FrmHocPhi
    participant Svc as HocPhiService
    participant Repo as IPhieuThuRepository
    participant DB as SQL Server

    TN->>UI: Chọn học viên có nợ & bấm "Thu học phí"
    UI->>Svc: GetRemainingFeeAsync(DangKyLopHocId)
    Svc->>Repo: GetTotalPaidByDangKyIdAsync()
    Svc-->>UI: Trả về số tiền còn nợ (ConNo)
    UI->>TN: Mở FrmThuHocPhi hiển thị ConNo
    TN->>UI: Nhập số tiền thu & hình thức thanh toán
    UI->>Svc: ThuHocPhiAsync(DangKyLopHocId, SoTien, HinhThuc)
    alt SoTien > ConNo
        Svc-->>UI: Lỗi: Số tiền thu vượt quá số tiền nợ
    else Hợp lệ
        Svc->>Svc: Sinh mã phiếu thu duy nhất (PT-yyyyMMdd-XXXXXX)
        Svc->>Repo: AddAsync(PhieuThuDto)
        Repo->>DB: INSERT INTO PhieuThu
        DB-->>Repo: Thành công
        Svc-->>UI: OperationResult.Success(PhieuThuDto)
        UI-->>TN: Thông báo lập phiếu thu thành công
    end
```

## 3. Ma Trận Phân Quyền (RBAC Matrix)

| Vai trò / Quyền | CatalogView | CatalogManage | TrainingView | TrainingManage | TuitionManage | GradeEntry | ReportView | UserManage |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **ADMIN** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **DAOTAO** | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| **THUNGAN** | ✅ | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ |
| **GIANGVIEN** | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ |
| **QUANLY** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |

- Toàn bộ việc kiểm tra quyền hạn được thực hiện nghiêm ngặt ở tầng Application Service thông qua `AuthorizationGuard.EnsurePermission`, không phụ thuộc vào trạng thái ẩn/hiện của giao diện người dùng.
