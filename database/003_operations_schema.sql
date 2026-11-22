/* ============================================================================
   QLHV - Hệ Thống Quản Lý Học Viên
   File: database/003_operations_schema.sql
   Giai đoạn: MEGA MVP (Lớp học, Xếp học viên, Lịch học, Kết quả học tập, Phiếu thu)
   Lưu ý quan trọng:
     - Không drop database hoặc bảng hiện có.
     - Script có transaction, bảo đảm idempotent.
     - Phân định Collation:
       1) Vietnamese_100_CI_AI: Tên lớp, nội dung buổi học, ghi chú.
       2) Vietnamese_100_CI_AS: Mã lớp học, mã phiếu thu.
       3) Latin1_General_100_BIN2: Token hình thức thanh toán (TIEN_MAT, CHUYEN_KHOAN).
     - Danh sách bảng nghiệp vụ:
       1. LopHoc
       2. DangKyLopHoc
       3. LichHoc
       4. KetQuaHocTap
       5. PhieuThu
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

-- ============================================================================
-- 1. Bảng LopHoc (Lớp học đào tạo)
-- ============================================================================
IF OBJECT_ID(N'dbo.LopHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LopHoc
    (
        Id          INT                                           IDENTITY(1,1) NOT NULL,
        MaLopHoc    NVARCHAR(20)  COLLATE Vietnamese_100_CI_AS    NOT NULL,
        TenLopHoc   NVARCHAR(150) COLLATE Vietnamese_100_CI_AI    NOT NULL,
        KhoaHocId   INT                                           NOT NULL,
        NgayBatDau  DATE                                          NOT NULL,
        NgayKetThuc DATE                                          NULL,
        SiSoToiDa   INT                                           NOT NULL,
        GhiChu      NVARCHAR(500) COLLATE Vietnamese_100_CI_AI    NULL,
        TrangThai   INT                                           NOT NULL CONSTRAINT DF_LopHoc_TrangThai DEFAULT (1),
        NgayTao     DATETIME2(7)                                  NOT NULL CONSTRAINT DF_LopHoc_NgayTao DEFAULT (SYSDATETIME()),

        CONSTRAINT PK_LopHoc PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_LopHoc_MaLopHoc UNIQUE (MaLopHoc),
        CONSTRAINT FK_LopHoc_KhoaHoc FOREIGN KEY (KhoaHocId) REFERENCES dbo.KhoaHoc(Id),
        CONSTRAINT CK_LopHoc_SiSoToiDa CHECK (SiSoToiDa BETWEEN 1 AND 200),
        CONSTRAINT CK_LopHoc_NgayKetThuc CHECK (NgayKetThuc IS NULL OR NgayKetThuc >= NgayBatDau),
        CONSTRAINT CK_LopHoc_TrangThai CHECK (TrangThai IN (0, 1))
    );

    CREATE NONCLUSTERED INDEX IX_LopHoc_KhoaHocId ON dbo.LopHoc (KhoaHocId);
    CREATE NONCLUSTERED INDEX IX_LopHoc_TrangThai ON dbo.LopHoc (TrangThai);
    CREATE NONCLUSTERED INDEX IX_LopHoc_NgayBatDau ON dbo.LopHoc (NgayBatDau);
END;

-- ============================================================================
-- 2. Bảng DangKyLopHoc (Xếp học viên vào lớp học)
-- ============================================================================
IF OBJECT_ID(N'dbo.DangKyLopHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DangKyLopHoc
    (
        Id            INT                                           IDENTITY(1,1) NOT NULL,
        LopHocId      INT                                           NOT NULL,
        HocVienId     INT                                           NOT NULL,
        NgayDangKy    DATE                                          NOT NULL,
        HocPhiPhaiThu DECIMAL(18,2)                                 NOT NULL,
        GhiChu        NVARCHAR(250) COLLATE Vietnamese_100_CI_AI    NULL,
        TrangThai     INT                                           NOT NULL CONSTRAINT DF_DangKyLopHoc_TrangThai DEFAULT (1),
        NgayTao       DATETIME2(7)                                  NOT NULL CONSTRAINT DF_DangKyLopHoc_NgayTao DEFAULT (SYSDATETIME()),

        CONSTRAINT PK_DangKyLopHoc PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_DangKyLopHoc_Lop_HocVien UNIQUE (LopHocId, HocVienId),
        CONSTRAINT FK_DangKyLopHoc_LopHoc FOREIGN KEY (LopHocId) REFERENCES dbo.LopHoc(Id),
        CONSTRAINT FK_DangKyLopHoc_HocVien FOREIGN KEY (HocVienId) REFERENCES dbo.HocVien(Id),
        CONSTRAINT CK_DangKyLopHoc_HocPhiPhaiThu CHECK (HocPhiPhaiThu >= 0),
        CONSTRAINT CK_DangKyLopHoc_TrangThai CHECK (TrangThai IN (0, 1))
    );

    CREATE NONCLUSTERED INDEX IX_DangKyLopHoc_HocVienId ON dbo.DangKyLopHoc (HocVienId);
    CREATE NONCLUSTERED INDEX IX_DangKyLopHoc_LopHocId ON dbo.DangKyLopHoc (LopHocId);
    CREATE NONCLUSTERED INDEX IX_DangKyLopHoc_TrangThai ON dbo.DangKyLopHoc (TrangThai);
END;

-- ============================================================================
-- 3. Bảng LichHoc (Từng buổi học cụ thể của lớp)
-- ============================================================================
IF OBJECT_ID(N'dbo.LichHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LichHoc
    (
        Id          INT                                           IDENTITY(1,1) NOT NULL,
        LopHocId    INT                                           NOT NULL,
        MonHocId    INT                                           NOT NULL,
        GiangVienId INT                                           NOT NULL,
        PhongHocId  INT                                           NOT NULL,
        NgayHoc     DATE                                          NOT NULL,
        GioBatDau   TIME(0)                                       NOT NULL,
        GioKetThuc  TIME(0)                                       NOT NULL,
        NoiDung     NVARCHAR(500) COLLATE Vietnamese_100_CI_AI    NULL,
        TrangThai   INT                                           NOT NULL CONSTRAINT DF_LichHoc_TrangThai DEFAULT (1),

        CONSTRAINT PK_LichHoc PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_LichHoc_LopHoc FOREIGN KEY (LopHocId) REFERENCES dbo.LopHoc(Id),
        CONSTRAINT FK_LichHoc_MonHoc FOREIGN KEY (MonHocId) REFERENCES dbo.MonHoc(Id),
        CONSTRAINT FK_LichHoc_GiangVien FOREIGN KEY (GiangVienId) REFERENCES dbo.GiangVien(Id),
        CONSTRAINT FK_LichHoc_PhongHoc FOREIGN KEY (PhongHocId) REFERENCES dbo.PhongHoc(Id),
        CONSTRAINT CK_LichHoc_GioKetThuc CHECK (GioKetThuc > GioBatDau),
        CONSTRAINT CK_LichHoc_TrangThai CHECK (TrangThai IN (0, 1))
    );

    CREATE NONCLUSTERED INDEX IX_LichHoc_Ngay_Gio ON dbo.LichHoc (NgayHoc, GioBatDau, GioKetThuc);
    CREATE NONCLUSTERED INDEX IX_LichHoc_LopHocId ON dbo.LichHoc (LopHocId);
    CREATE NONCLUSTERED INDEX IX_LichHoc_GiangVienId ON dbo.LichHoc (GiangVienId);
    CREATE NONCLUSTERED INDEX IX_LichHoc_PhongHocId ON dbo.LichHoc (PhongHocId);
END;

-- ============================================================================
-- 4. Bảng KetQuaHocTap (Điểm tổng kết môn học của học viên trong lớp)
-- ============================================================================
IF OBJECT_ID(N'dbo.KetQuaHocTap', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.KetQuaHocTap
    (
        Id                     INT                                           IDENTITY(1,1) NOT NULL,
        DangKyLopHocId         INT                                           NOT NULL,
        MonHocId               INT                                           NOT NULL,
        DiemTongKet            DECIMAL(4,2)                                  NOT NULL,
        NguoiCapNhatTaiKhoanId INT                                           NULL,
        NgayCapNhat            DATETIME2(7)                                  NOT NULL CONSTRAINT DF_KetQuaHocTap_NgayCapNhat DEFAULT (SYSDATETIME()),
        GhiChu                 NVARCHAR(250) COLLATE Vietnamese_100_CI_AI    NULL,

        CONSTRAINT PK_KetQuaHocTap PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_KetQuaHocTap_DangKy_Mon UNIQUE (DangKyLopHocId, MonHocId),
        CONSTRAINT FK_KetQuaHocTap_DangKy FOREIGN KEY (DangKyLopHocId) REFERENCES dbo.DangKyLopHoc(Id),
        CONSTRAINT FK_KetQuaHocTap_MonHoc FOREIGN KEY (MonHocId) REFERENCES dbo.MonHoc(Id),
        CONSTRAINT FK_KetQuaHocTap_TaiKhoan FOREIGN KEY (NguoiCapNhatTaiKhoanId) REFERENCES dbo.TaiKhoan(Id),
        CONSTRAINT CK_KetQuaHocTap_Diem CHECK (DiemTongKet >= 0.00 AND DiemTongKet <= 10.00)
    );

    CREATE NONCLUSTERED INDEX IX_KetQuaHocTap_DangKyLopHocId ON dbo.KetQuaHocTap (DangKyLopHocId);
    CREATE NONCLUSTERED INDEX IX_KetQuaHocTap_MonHocId ON dbo.KetQuaHocTap (MonHocId);
END;

-- ============================================================================
-- 5. Bảng PhieuThu (Phiếu thu học phí)
-- ============================================================================
IF OBJECT_ID(N'dbo.PhieuThu', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhieuThu
    (
        Id                 INT                                           IDENTITY(1,1) NOT NULL,
        MaPhieuThu         NVARCHAR(30)  COLLATE Vietnamese_100_CI_AS    NOT NULL,
        DangKyLopHocId     INT                                           NOT NULL,
        NgayThu            DATETIME2(7)                                  NOT NULL CONSTRAINT DF_PhieuThu_NgayThu DEFAULT (SYSDATETIME()),
        SoTien             DECIMAL(18,2)                                 NOT NULL,
        HinhThuc           NVARCHAR(20)  COLLATE Latin1_General_100_BIN2 NOT NULL,
        NguoiThuTaiKhoanId INT                                           NOT NULL,
        GhiChu             NVARCHAR(250) COLLATE Vietnamese_100_CI_AI    NULL,
        TrangThai          INT                                           NOT NULL CONSTRAINT DF_PhieuThu_TrangThai DEFAULT (1),
        LyDoHuy            NVARCHAR(250) COLLATE Vietnamese_100_CI_AI    NULL,

        CONSTRAINT PK_PhieuThu PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_PhieuThu_MaPhieuThu UNIQUE (MaPhieuThu),
        CONSTRAINT FK_PhieuThu_DangKy FOREIGN KEY (DangKyLopHocId) REFERENCES dbo.DangKyLopHoc(Id),
        CONSTRAINT FK_PhieuThu_TaiKhoan FOREIGN KEY (NguoiThuTaiKhoanId) REFERENCES dbo.TaiKhoan(Id),
        CONSTRAINT CK_PhieuThu_SoTien CHECK (SoTien > 0),
        CONSTRAINT CK_PhieuThu_HinhThuc CHECK (HinhThuc IN (N'TIEN_MAT', N'CHUYEN_KHOAN')),
        CONSTRAINT CK_PhieuThu_TrangThai CHECK (TrangThai IN (0, 1))
    );

    CREATE NONCLUSTERED INDEX IX_PhieuThu_DangKyLopHocId ON dbo.PhieuThu (DangKyLopHocId);
    CREATE NONCLUSTERED INDEX IX_PhieuThu_NgayThu ON dbo.PhieuThu (NgayThu);
    CREATE NONCLUSTERED INDEX IX_PhieuThu_TrangThai ON dbo.PhieuThu (TrangThai);
END;

COMMIT TRANSACTION;
