using QLHV.Contracts.Enums;

namespace QLHV.Contracts.Dtos;

/// <summary>
/// Bộ lọc Lớp học
/// </summary>
public class LopHocFilter
{
    public string? TuKhoa { get; set; }
    public int? KhoaHocId { get; set; }
    public TrangThaiHoatDong? TrangThai { get; set; }
}

/// <summary>
/// Bộ lọc Xếp lớp học viên
/// </summary>
public class DangKyLopHocFilter
{
    public int? LopHocId { get; set; }
    public int? HocVienId { get; set; }
    public string? TuKhoa { get; set; }
    public TrangThaiHoatDong? TrangThai { get; set; }
}

/// <summary>
/// Bộ lọc Lịch học
/// </summary>
public class LichHocFilter
{
    public int? LopHocId { get; set; }
    public int? MonHocId { get; set; }
    public int? GiangVienId { get; set; }
    public int? PhongHocId { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public TrangThaiHoatDong? TrangThai { get; set; }
}

/// <summary>
/// Bộ lọc Kết quả học tập
/// </summary>
public class KetQuaHocTapFilter
{
    public int? LopHocId { get; set; }
    public int? MonHocId { get; set; }
    public int? HocVienId { get; set; }
}

/// <summary>
/// Bộ lọc Phiếu thu học phí
/// </summary>
public class PhieuThuFilter
{
    public int? LopHocId { get; set; }
    public int? DangKyLopHocId { get; set; }
    public string? TuKhoa { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public TrangThaiPhieuThu? TrangThai { get; set; }
}

/// <summary>
/// Bộ lọc tổng quát cho các Báo cáo
/// </summary>
public class BaoCaoFilter
{
    public int? LopHocId { get; set; }
    public int? MonHocId { get; set; }
    public int? GiangVienId { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
}
