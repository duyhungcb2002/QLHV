namespace QLHV.Contracts.Dtos;

/// <summary>
/// Báo cáo danh sách học viên theo lớp học
/// </summary>
public class BaoCaoHocVienTheoLopDto
{
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string MaHocVien { get; set; } = string.Empty;
    public string HoTenHocVien { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public string? DienThoai { get; set; }
    public string? Email { get; set; }
    public DateTime NgayDangKy { get; set; }
    public decimal HocPhiPhaiThu { get; set; }
    public decimal DaThu { get; set; }
    public decimal ConNo => Math.Max(0m, HocPhiPhaiThu - DaThu);
}

/// <summary>
/// Báo cáo lịch học theo lớp
/// </summary>
public class BaoCaoLichHocTheoLopDto
{
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public string MaMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public string HoTenGiangVien { get; set; } = string.Empty;
    public string TenPhong { get; set; } = string.Empty;
    public DateTime NgayHoc { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public string? NoiDung { get; set; }
}

/// <summary>
/// Báo cáo phân công giảng dạy theo giảng viên
/// </summary>
public class BaoCaoGiangDayTheoGiangVienDto
{
    public string MaGiangVien { get; set; } = string.Empty;
    public string HoTenGiangVien { get; set; } = string.Empty;
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public DateTime NgayHoc { get; set; }
    public TimeSpan GioBatDau { get; set; }
    public TimeSpan GioKetThuc { get; set; }
    public string TenPhong { get; set; } = string.Empty;
}

/// <summary>
/// Báo cáo kết quả học tập và tỷ lệ đạt/trượt theo lớp hoặc theo môn
/// </summary>
public class BaoCaoKetQuaHocTapDto
{
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public string MaMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public string MaHocVien { get; set; } = string.Empty;
    public string HoTenHocVien { get; set; } = string.Empty;
    public decimal? DiemTongKet { get; set; }
    public bool CoDiem => DiemTongKet.HasValue;
    public bool Dat => DiemTongKet.HasValue && DiemTongKet.Value >= 5.00m;
    public string KetQua => !DiemTongKet.HasValue ? "Chưa nhập" : (Dat ? "Đạt" : "Không đạt");
    public string? GhiChu { get; set; }
}

/// <summary>
/// Thống kê tỷ lệ đạt môn học
/// </summary>
public class ThongKeTyLeMonHocDto
{
    public string MaMonHoc { get; set; } = string.Empty;
    public string TenMonHoc { get; set; } = string.Empty;
    public int TongSoHocVien { get; set; }
    public int SoLuongChuaNhap { get; set; }
    public int SoLuongDaCham { get; set; }
    public int SoLuongDat { get; set; }
    public int SoLuongKhongDat => Math.Max(0, SoLuongDaCham - SoLuongDat);
    public decimal TyLeDatTrenDaCham => SoLuongDaCham > 0 ? Math.Round((decimal)SoLuongDat * 100m / SoLuongDaCham, 2) : 0m;
    public decimal TyLeDatTrenTongSo => TongSoHocVien > 0 ? Math.Round((decimal)SoLuongDat * 100m / TongSoHocVien, 2) : 0m;
}

/// <summary>
/// Báo cáo doanh thu & công nợ học phí theo lớp học
/// </summary>
public class BaoCaoCongNoHocPhiDto
{
    public string MaLopHoc { get; set; } = string.Empty;
    public string TenLopHoc { get; set; } = string.Empty;
    public int TongHocVien { get; set; }
    public decimal TongHocPhiPhaiThu { get; set; }
    public decimal TongDaThu { get; set; }
    public decimal TongConNo => Math.Max(0m, TongHocPhiPhaiThu - TongDaThu);
    public decimal TyLeThu => TongHocPhiPhaiThu > 0 ? Math.Round(TongDaThu * 100m / TongHocPhiPhaiThu, 2) : 0m;
}
