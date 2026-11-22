/* ============================================================================
   QLHV - Hệ Thống Quản Lý Học Viên
   File: database/001_schema.sql
   Giai đoạn: SQL-01B1 (Schema cho chức năng Đăng nhập và 6 Danh mục hiện có)
   Lưu ý quan trọng:
     - Đây là script khởi tạo database mới, không phải migration.
     - Nếu database hoặc bảng đã tồn tại từ schema cũ, chạy lại script không tự đổi collation.
     - Không chứa lệnh xóa database.
     - Không chứa chuỗi kết nối.
     - Tên cột ánh xạ trực tiếp với DTO trong QLHV.Contracts.Dtos và Repository Interface.
     - Trạng thái: 1 = Đang hoạt động / Hoạt động, 0 = Ngừng hoạt động / Bị khóa.
     - Phân định Collation:
       1) Vietnamese_100_CI_AI: Các trường tìm kiếm nội dung tiếng Việt không dấu (Họ tên,
          Tên môn học, Tên khóa học, Tên phòng, Chuyên môn, Địa chỉ, Mô tả).
       2) Vietnamese_100_CI_AS: Các trường định danh nghiệp vụ (TenDangNhap, MaHocVien,
          MaGiangVien, MaMonHoc, MaKhoaHoc, MaPhong, Email, DienThoai) để không phân biệt
          hoa/thường nhưng phân biệt dấu chính xác ('hung' != 'hùng').
       3) Latin1_General_100_BIN2: Các trường token chính xác và hash mật khẩu (VaiTro,
          MatKhauHash) để CHECK constraint và so sánh token/hash tuyệt đối chính xác.
   Danh sách bảng:
     1. GiangVien
     2. TaiKhoan
     3. HocVien
     4. MonHoc
     5. KhoaHoc
     6. KhoaHocMonHoc
     7. PhongHoc
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- ============================================================================
-- 1. Bảng GiangVien (Danh mục Giảng viên)
-- ============================================================================
IF OBJECT_ID(N'dbo.GiangVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.GiangVien
    (
        Id          INT                                           IDENTITY(1,1) NOT NULL,
        MaGiangVien NVARCHAR(20)  COLLATE Vietnamese_100_CI_AS    NOT NULL,
        HoTen       NVARCHAR(100) COLLATE Vietnamese_100_CI_AI    NOT NULL,
        ChuyenMon   NVARCHAR(200) COLLATE Vietnamese_100_CI_AI    NULL,
        DienThoai   NVARCHAR(20)  COLLATE Vietnamese_100_CI_AS    NULL,
        Email       NVARCHAR(150) COLLATE Vietnamese_100_CI_AS    NULL,
        TrangThai   INT                                           NOT NULL CONSTRAINT DF_GiangVien_TrangThai DEFAULT (1),

        CONSTRAINT PK_GiangVien PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_GiangVien_MaGiangVien UNIQUE (MaGiangVien),
        CONSTRAINT CK_GiangVien_TrangThai CHECK (TrangThai IN (0, 1))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_GiangVien_HoTen' AND object_id = OBJECT_ID(N'dbo.GiangVien'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_GiangVien_HoTen ON dbo.GiangVien (HoTen);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_GiangVien_TrangThai' AND object_id = OBJECT_ID(N'dbo.GiangVien'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_GiangVien_TrangThai ON dbo.GiangVien (TrangThai);
END;
GO

-- ============================================================================
-- 2. Bảng TaiKhoan (Tài khoản đăng nhập hệ thống)
-- ============================================================================
IF OBJECT_ID(N'dbo.TaiKhoan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaiKhoan
    (
        Id              INT                                           IDENTITY(1,1) NOT NULL,
        TenDangNhap     NVARCHAR(50)  COLLATE Vietnamese_100_CI_AS    NOT NULL,
        MatKhauHash     NVARCHAR(256) COLLATE Latin1_General_100_BIN2 NOT NULL,
        VaiTro          NVARCHAR(30)  COLLATE Latin1_General_100_BIN2 NOT NULL,
        GiangVienId     INT                                           NULL,
        TrangThai       INT                                           NOT NULL CONSTRAINT DF_TaiKhoan_TrangThai DEFAULT (1),
        LanDangNhapCuoi DATETIME2(0)                                  NULL,

        CONSTRAINT PK_TaiKhoan PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_TaiKhoan_TenDangNhap UNIQUE (TenDangNhap),
        CONSTRAINT FK_TaiKhoan_GiangVien FOREIGN KEY (GiangVienId) REFERENCES dbo.GiangVien (Id),
        CONSTRAINT CK_TaiKhoan_VaiTro CHECK (VaiTro IN (N'ADMIN', N'DAOTAO', N'THUNGAN', N'GIANGVIEN', N'QUANLY')),
        CONSTRAINT CK_TaiKhoan_TrangThai CHECK (TrangThai IN (0, 1))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TaiKhoan_GiangVienId' AND object_id = OBJECT_ID(N'dbo.TaiKhoan'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_TaiKhoan_GiangVienId ON dbo.TaiKhoan (GiangVienId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TaiKhoan_TrangThai' AND object_id = OBJECT_ID(N'dbo.TaiKhoan'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_TaiKhoan_TrangThai ON dbo.TaiKhoan (TrangThai);
END;
GO

-- ============================================================================
-- 3. Bảng HocVien (Danh mục Học viên)
-- ============================================================================
IF OBJECT_ID(N'dbo.HocVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HocVien
    (
        Id          INT                                           IDENTITY(1,1) NOT NULL,
        MaHocVien   NVARCHAR(20)  COLLATE Vietnamese_100_CI_AS    NOT NULL,
        HoTen       NVARCHAR(100) COLLATE Vietnamese_100_CI_AI    NOT NULL,
        NgaySinh    DATE                                          NULL,
        GioiTinh    NVARCHAR(20)  COLLATE Vietnamese_100_CI_AI    NULL,
        DienThoai   NVARCHAR(20)  COLLATE Vietnamese_100_CI_AS    NULL,
        Email       NVARCHAR(150) COLLATE Vietnamese_100_CI_AS    NULL,
        DiaChi      NVARCHAR(250) COLLATE Vietnamese_100_CI_AI    NULL,
        NgayDangKy  DATE                                          NOT NULL CONSTRAINT DF_HocVien_NgayDangKy DEFAULT (CAST(GETDATE() AS DATE)),
        TrangThai   INT                                           NOT NULL CONSTRAINT DF_HocVien_TrangThai DEFAULT (1),

        CONSTRAINT PK_HocVien PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_HocVien_MaHocVien UNIQUE (MaHocVien),
        CONSTRAINT CK_HocVien_TrangThai CHECK (TrangThai IN (0, 1))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HocVien_HoTen' AND object_id = OBJECT_ID(N'dbo.HocVien'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_HocVien_HoTen ON dbo.HocVien (HoTen);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HocVien_DienThoai' AND object_id = OBJECT_ID(N'dbo.HocVien'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_HocVien_DienThoai ON dbo.HocVien (DienThoai);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HocVien_TrangThai' AND object_id = OBJECT_ID(N'dbo.HocVien'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_HocVien_TrangThai ON dbo.HocVien (TrangThai);
END;
GO

-- ============================================================================
-- 4. Bảng MonHoc (Danh mục Môn học)
-- ============================================================================
IF OBJECT_ID(N'dbo.MonHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MonHoc
    (
        Id            INT                                           IDENTITY(1,1) NOT NULL,
        MaMonHoc      NVARCHAR(20)  COLLATE Vietnamese_100_CI_AS    NOT NULL,
        TenMonHoc     NVARCHAR(150) COLLATE Vietnamese_100_CI_AI    NOT NULL,
        SoBuoiMacDinh INT                                           NOT NULL,
        MoTa          NVARCHAR(500) COLLATE Vietnamese_100_CI_AI    NULL,
        TrangThai     INT                                           NOT NULL CONSTRAINT DF_MonHoc_TrangThai DEFAULT (1),

        CONSTRAINT PK_MonHoc PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_MonHoc_MaMonHoc UNIQUE (MaMonHoc),
        CONSTRAINT CK_MonHoc_SoBuoiMacDinh CHECK (SoBuoiMacDinh > 0),
        CONSTRAINT CK_MonHoc_TrangThai CHECK (TrangThai IN (0, 1))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MonHoc_TenMonHoc' AND object_id = OBJECT_ID(N'dbo.MonHoc'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_MonHoc_TenMonHoc ON dbo.MonHoc (TenMonHoc);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_MonHoc_TrangThai' AND object_id = OBJECT_ID(N'dbo.MonHoc'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_MonHoc_TrangThai ON dbo.MonHoc (TrangThai);
END;
GO

-- ============================================================================
-- 5. Bảng KhoaHoc (Danh mục Khóa học)
-- ============================================================================
IF OBJECT_ID(N'dbo.KhoaHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.KhoaHoc
    (
        Id            INT                                           IDENTITY(1,1) NOT NULL,
        MaKhoaHoc     NVARCHAR(20)  COLLATE Vietnamese_100_CI_AS    NOT NULL,
        TenKhoaHoc    NVARCHAR(150) COLLATE Vietnamese_100_CI_AI    NOT NULL,
        HocPhiNiemYet DECIMAL(18, 2)                                NOT NULL CONSTRAINT DF_KhoaHoc_HocPhiNiemYet DEFAULT (0),
        MoTa          NVARCHAR(500) COLLATE Vietnamese_100_CI_AI    NULL,
        TrangThai     INT                                           NOT NULL CONSTRAINT DF_KhoaHoc_TrangThai DEFAULT (1),

        CONSTRAINT PK_KhoaHoc PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_KhoaHoc_MaKhoaHoc UNIQUE (MaKhoaHoc),
        CONSTRAINT CK_KhoaHoc_HocPhiNiemYet CHECK (HocPhiNiemYet >= 0),
        CONSTRAINT CK_KhoaHoc_TrangThai CHECK (TrangThai IN (0, 1))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_KhoaHoc_TenKhoaHoc' AND object_id = OBJECT_ID(N'dbo.KhoaHoc'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_KhoaHoc_TenKhoaHoc ON dbo.KhoaHoc (TenKhoaHoc);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_KhoaHoc_TrangThai' AND object_id = OBJECT_ID(N'dbo.KhoaHoc'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_KhoaHoc_TrangThai ON dbo.KhoaHoc (TrangThai);
END;
GO

-- ============================================================================
-- 6. Bảng KhoaHocMonHoc (Quan hệ Khóa học - Môn học)
-- ============================================================================
IF OBJECT_ID(N'dbo.KhoaHocMonHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.KhoaHocMonHoc
    (
        KhoaHocId    INT NOT NULL,
        MonHocId     INT NOT NULL,
        ThuTu        INT NOT NULL,
        SoBuoiDuKien INT NOT NULL,

        CONSTRAINT PK_KhoaHocMonHoc PRIMARY KEY CLUSTERED (KhoaHocId, MonHocId),
        CONSTRAINT FK_KhoaHocMonHoc_KhoaHoc FOREIGN KEY (KhoaHocId) REFERENCES dbo.KhoaHoc (Id),
        CONSTRAINT FK_KhoaHocMonHoc_MonHoc FOREIGN KEY (MonHocId) REFERENCES dbo.MonHoc (Id),
        CONSTRAINT CK_KhoaHocMonHoc_ThuTu CHECK (ThuTu > 0),
        CONSTRAINT CK_KhoaHocMonHoc_SoBuoiDuKien CHECK (SoBuoiDuKien > 0)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_KhoaHocMonHoc_KhoaHocId_ThuTu' AND object_id = OBJECT_ID(N'dbo.KhoaHocMonHoc'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_KhoaHocMonHoc_KhoaHocId_ThuTu ON dbo.KhoaHocMonHoc (KhoaHocId, ThuTu);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_KhoaHocMonHoc_MonHocId' AND object_id = OBJECT_ID(N'dbo.KhoaHocMonHoc'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_KhoaHocMonHoc_MonHocId ON dbo.KhoaHocMonHoc (MonHocId);
END;
GO

-- ============================================================================
-- 7. Bảng PhongHoc (Danh mục Phòng học)
-- ============================================================================
IF OBJECT_ID(N'dbo.PhongHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhongHoc
    (
        Id        INT                                           IDENTITY(1,1) NOT NULL,
        MaPhong   NVARCHAR(20)  COLLATE Vietnamese_100_CI_AS    NOT NULL,
        TenPhong  NVARCHAR(100) COLLATE Vietnamese_100_CI_AI    NOT NULL,
        SucChua   INT                                           NOT NULL,
        TrangThai INT                                           NOT NULL CONSTRAINT DF_PhongHoc_TrangThai DEFAULT (1),

        CONSTRAINT PK_PhongHoc PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_PhongHoc_MaPhong UNIQUE (MaPhong),
        CONSTRAINT CK_PhongHoc_SucChua CHECK (SucChua > 0),
        CONSTRAINT CK_PhongHoc_TrangThai CHECK (TrangThai IN (0, 1))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PhongHoc_TenPhong' AND object_id = OBJECT_ID(N'dbo.PhongHoc'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PhongHoc_TenPhong ON dbo.PhongHoc (TenPhong);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PhongHoc_TrangThai' AND object_id = OBJECT_ID(N'dbo.PhongHoc'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PhongHoc_TrangThai ON dbo.PhongHoc (TrangThai);
END;
GO
