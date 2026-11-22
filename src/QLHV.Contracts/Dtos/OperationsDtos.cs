using QLHV.Contracts.Enums;

namespace QLHV.Contracts.Dtos;

/// <summary>
/// Data Transfer Object cho Lớp học
/// </summary>
public class LopHocDto
{
    public int Id { get; set; }
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public int KhoaHocId { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public DateTime NgayBatDau { get; set; } = DateTime.Today;
    public DateTime? NgayKetThuc { get; set; }
    public int SiSoToiDa { get; set; } = 30;
    public int SiSoHienTai { get; set; }
    public int? GiangVienChuNhiemId { get; set; }
    public string? HoTenGiangVienChuNhiem { get; set; }
    public string? GhiChu { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.DangHoatDong;
    public DateTime NgayTao { get; set; } = DateTime.Now;
}

/// <summary>
/// Data Transfer Object cho Đăng ký / Xếp học viên vào lớp học
/// </summary>
public class DangKyLopHocDto
{
    public int Id { get; set; }
    public int LopHocId { get; set; }
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public int KhoaHocId { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public int HocVienId { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string HoTenHocVien { get; set; } = string.Empty;
    public string? DienThoaiHocVien { get; set; }
    public DateTime NgayDangKy { get; set; } = DateTime.Today;
    public decimal HocPhiPhaiThu { get; set; }
    public decimal DaThu { get; set; }
    public decimal ConNo => Math.Max(0m, HocPhiPhaiThu - DaThu);
    public string? GhiChu { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.DangHoatDong;
    public DateTime NgayTao { get; set; } = DateTime.Now;
}

/// <summary>
/// Data Transfer Object cho Lịch học
/// </summary>
public class LichHocDto
{
    public int Id { get; set; }
    public int LopHocId { get; set; }
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public int MonHocId { get; set; }
    public string MaMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public int GiangVienId { get; set; }
    public string MaGiangVien { get; set; } = string.Empty;
    public string HoTenGiangVien { get; set; } = string.Empty;
    public int PhongHocId { get; set; }
    public string MaPhong { get; set; } = string.Empty;
    public string TenPhong { get; set; } = string.Empty;
    public DateTime NgayHoc { get; set; } = DateTime.Today;
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public string? NoiDung { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.DangHoatDong;
}

/// <summary>
/// Data Transfer Object cho Kết quả học tập
/// </summary>
public class KetQuaHocTapDto
{
    public int Id { get; set; }
    public int DangKyLopHocId { get; set; }
    public int LopHocId { get; set; }
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public int KhoaHocId { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public int HocVienId { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string HoTenHocVien { get; set; } = string.Empty;
    public int MonHocId { get; set; }
    public string MaMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public decimal? DiemTongKet { get; set; }
    public bool CoDiem => DiemTongKet.HasValue;
    public bool Dat => DiemTongKet.HasValue && DiemTongKet.Value >= 5.00m;
    public string KetQua => !DiemTongKet.HasValue ? "Chưa nhập" : (Dat ? "Đạt" : "Không đạt");
    public int? NguoiCapNhatTaiKhoanId { get; set; }
    public string? NguoiCapNhatTen { get; set; }
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;
    public string? GhiChu { get; set; }
}

/// <summary>
/// Data Transfer Object cho Phiếu thu học phí
/// </summary>
public class PhieuThuDto
{
    public int Id { get; set; }
    public string MaPhieuThu { get; set; } = string.Empty;
    public int DangKyLopHocId { get; set; }
    public int LopHocId { get; set; }
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public int KhoaHocId { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public int HocVienId { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string HoTenHocVien { get; set; } = string.Empty;
    public DateTime NgayThu { get; set; } = DateTime.Now;
    public decimal SoTien { get; set; }
    public HinhThucThanhToan HinhThuc { get; set; } = HinhThucThanhToan.TIEN_MAT;
    public int NguoiThuTaiKhoanId { get; set; }
    public string NguoiThuTen { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
    public TrangThaiPhieuThu TrangThai { get; set; } = TrangThaiPhieuThu.HieuLuc;
    public string? LyDoHuy { get; set; }
    public DateTime? NgayHuy { get; set; }
    public int? NguoiHuyTaiKhoanId { get; set; }
    public string? NguoiHuyTen { get; set; }
}

/// <summary>
/// DTO tổng hợp công nợ học phí theo lượt đăng ký
/// </summary>
public class CongNoHocPhiDto
{
    public int DangKyLopHocId { get; set; }
    public int LopHocId { get; set; }
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public int KhoaHocId { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public int HocVienId { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string HoTenHocVien { get; set; } = string.Empty;
    public string? DienThoai { get; set; }
    public decimal HocPhiPhaiThu { get; set; }
    public decimal DaThu { get; set; }
    public decimal ConNo => Math.Max(0m, HocPhiPhaiThu - DaThu);
    public string TrangThaiCongNo => ConNo == 0m ? "Hoàn thành" : (DaThu > 0m ? "Đóng một phần" : "Chưa đóng");
}
