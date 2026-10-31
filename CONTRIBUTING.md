# Quy Định Đóng Góp Và Làm Việc Nhóm - Dự Án QLHV

Tài liệu này quy định quy trình làm việc, phân chia trách nhiệm và quy ước phát triển mã nguồn cho nhóm bài tập lớn "Hệ thống Quản lý học viên" (QLHV).

---

## 1. Phân chia trách nhiệm thành viên

1. **Duy Hưng (Team Lead / Core Architecture)**:
   - Solution structure, Shared Contracts (`QLHV.Contracts`), Core Abstractions (`QLHV.Core`).
   - Xác thực & Phân quyền (`AuthService`, `AuthorizationService`).
   - Form chính (`FrmMain`) và điều hướng hệ thống.
   - Các danh mục độc lập (Catalog): Học viên, Giảng viên, Môn học, Khóa học, Quan hệ Khóa học - Môn học, Phòng học.
   - Logging, chuẩn hóa lỗi (`OperationResult`), điểm nối tích hợp.

2. **Bá Hiệp (Training Module)**:
   - Nghiệp vụ Đào tạo: Lớp học, Xếp học viên vào lớp / Đăng ký học, Phân công giảng viên, Lịch học, Điểm thi và Phiếu thu / Học phí.
   - Triển khai sau khi tích hợp contracts dùng chung.

3. **Chu Đức (Database & Reporting Module)**:
   - Thiết kế CSDL chính thức (schema SQL Server, migration script, seed SQL).
   - Triển khai ADO.NET repositories thay thế cho in-memory repositories.
   - Stored procedures, views và chức năng Thống kê / Báo cáo chính thức.

4. **Mai Chi (Documentation & Quality Assurance)**:
   - Tài liệu phân tích thiết kế, tài liệu hướng dẫn sử dụng trong `/docs`.
   - Bộ kịch bản kiểm thử nghiệm thu (UAT) trong `/qa`.

---

## 2. Quy ước nhánh Git (Branching Strategy)

- **`main`**: Nhánh chứa mã nguồn phát hành chính thức, luôn luôn ổn định (deployable/runnable).
- **`develop`**: Nhánh tích hợp chung của toàn nhóm. Mọi nhánh chức năng khi hoàn thành sẽ merge vào `develop`.
- **Nhánh chức năng (`feature/`)**: Đặt tên theo cú pháp:
  ```text
  feature/<ma-cong-viec>-<mo-ta-ngan>
  ```
  - Ví dụ: `feature/DH-03-auth-navigation`, `feature/BH-01-training-class`, `feature/CD-01-sql-schema`.

---

## 3. Quy tắc làm việc và Pull Request

1. **Không code trực tiếp lên `main` hoặc `develop`**: Luôn tạo nhánh chức năng từ `develop`.
2. **Phạm vi thay đổi (Atomicity)**: Mỗi Pull Request (PR) chỉ giải quyết một đầu việc / nhóm thay đổi cụ thể. Tránh PR quá lớn dồn nhiều tính năng.
3. **Ranh giới module**: Thành viên không tự ý sửa đổi file thuộc module của thành viên khác nếu chưa có sự thống nhất trong buổi họp nhóm.
4. **Thay đổi Contracts dùng chung**: Mọi thay đổi hoặc bổ sung trong `QLHV.Contracts` bắt buộc phải thông báo và thảo luận trước với cả nhóm.
5. **Kiểm thử trước khi merge**: Mọi PR phải:
   - Build thành công cả chế độ `Debug` và `Release`.
   - Chạy toàn bộ Unit Tests / Integration Tests không có lỗi (`dotnet test`).
   - Được ít nhất 1 thành viên review và phê duyệt trước khi merge vào `develop`.
