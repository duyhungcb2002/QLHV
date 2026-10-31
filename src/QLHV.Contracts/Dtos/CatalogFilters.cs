using QLHV.Contracts.Enums;

namespace QLHV.Contracts.Dtos;

/// <summary>
/// Bộ lọc danh sách Học viên
/// </summary>
public class HocVienFilter
{
    public string? TuKhoa { get; set; }
    public TrangThaiHoatDong? TrangThai { get; set; }
}

/// <summary>
/// Bộ lọc danh sách Giảng viên
/// </summary>
public class GiangVienFilter
{
    public string? TuKhoa { get; set; }
    public TrangThaiHoatDong? TrangThai { get; set; }
}

/// <summary>
/// Bộ lọc danh sách Môn học
/// </summary>
public class MonHocFilter
{
    public string? TuKhoa { get; set; }
    public TrangThaiHoatDong? TrangThai { get; set; }
}

/// <summary>
/// Bộ lọc danh sách Khóa học
/// </summary>
public class KhoaHocFilter
{
    public string? TuKhoa { get; set; }
    public TrangThaiHoatDong? TrangThai { get; set; }
}

/// <summary>
/// Bộ lọc danh sách Phòng học
/// </summary>
public class PhongHocFilter
{
    public string? TuKhoa { get; set; }
    public TrangThaiHoatDong? TrangThai { get; set; }
}
