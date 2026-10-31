using QLHV.Contracts.Enums;

namespace QLHV.Contracts.Dtos;

/// <summary>
/// Data Transfer Object cho Học viên
/// </summary>
public class HocVienDto
{
    public int Id { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string? DienThoai { get; set; }
    public string? Email { get; set; }
    public string? DiaChi { get; set; }
    public DateTime NgayDangKy { get; set; } = DateTime.Today;
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.DangHoatDong;
}

/// <summary>
/// Data Transfer Object cho Giảng viên
/// </summary>
public class GiangVienDto
{
    public int Id { get; set; }
    public string MaGiangVien { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string? ChuyenMon { get; set; }
    public string? DienThoai { get; set; }
    public string? Email { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.DangHoatDong;
}

/// <summary>
/// Data Transfer Object cho Môn học
/// </summary>
public class MonHocDto
{
    public int Id { get; set; }
    public string MaMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public int SoBuoiMacDinh { get; set; }
    public string? MoTa { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.DangHoatDong;
}

/// <summary>
/// Data Transfer Object cho Khóa học
/// </summary>
public class KhoaHocDto
{
    public int Id { get; set; }
    public string MaKhoaHoc { get; set; } = string.Empty;
    public string TenKhoaHoc { get; set; } = string.Empty;
    public decimal HocPhiNiemYet { get; set; }
    public string? MoTa { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.DangHoatDong;
}

/// <summary>
/// Data Transfer Object cho quan hệ Khóa học - Môn học
/// </summary>
public class KhoaHocMonHocDto
{
    public int KhoaHocId { get; set; }
    public int MonHocId { get; set; }
    public int ThuTu { get; set; }
    public int SoBuoiDuKien { get; set; }

    // Thông tin hiển thị tiện ích
    public string? MaMonHoc { get; set; }
    public string? TenMonHoc { get; set; }
    public string? MaKhoaHoc { get; set; }
    public string? TenKhoaHoc { get; set; }
}

/// <summary>
/// Data Transfer Object cho Phòng học
/// </summary>
public class PhongHocDto
{
    public int Id { get; set; }
    public string MaPhong { get; set; } = string.Empty;
    public string TenPhong { get; set; } = string.Empty;
    public int SucChua { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.DangHoatDong;
}

/// <summary>
/// Data Transfer Object cho Tài khoản người dùng
/// </summary>
public class TaiKhoanDto
{
    public int Id { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public VaiTro VaiTro { get; set; }
    public int? GiangVienId { get; set; }
    public TrangThaiTaiKhoan TrangThai { get; set; } = TrangThaiTaiKhoan.HoatDong;
    public DateTime? LanDangNhapCuoi { get; set; }
}
