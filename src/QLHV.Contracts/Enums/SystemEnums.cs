namespace QLHV.Contracts.Enums;

/// <summary>
/// Vai trò người dùng trong hệ thống QLHV
/// </summary>
public enum VaiTro
{
    ADMIN,
    DAOTAO,
    THUNGAN,
    GIANGVIEN,
    QUANLY
}

/// <summary>
/// Trạng thái hoạt động của các đối tượng danh mục (Học viên, Giảng viên, Môn học, ...)
/// </summary>
public enum TrangThaiHoatDong
{
    DangHoatDong = 1,
    NgungHoatDong = 0
}

/// <summary>
/// Trạng thái tài khoản người dùng
/// </summary>
public enum TrangThaiTaiKhoan
{
    HoatDong = 1,
    BiKhoa = 0
}

/// <summary>
/// Danh sách các quyền hạn trong hệ thống
/// </summary>
public enum Permission
{
    CatalogView,
    CatalogManage,
    TrainingView,
    TrainingManage,
    TuitionManage,
    GradeEntry,
    ReportView,
    UserManage
}
