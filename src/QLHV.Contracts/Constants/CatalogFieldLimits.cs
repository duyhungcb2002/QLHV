namespace QLHV.Contracts.Constants;

/// <summary>
/// Giới hạn độ dài các trường dữ liệu danh mục đồng bộ với schema cơ sở dữ liệu (001_schema.sql).
/// </summary>
public static class CatalogFieldLimits
{
    // Business Codes (Mã danh mục)
    public const int MaHocVien = 20;
    public const int MaGiangVien = 20;
    public const int MaMonHoc = 20;
    public const int MaKhoaHoc = 20;
    public const int MaPhong = 20;

    // Names (Họ tên, tên danh mục)
    public const int HoTen = 100;
    public const int TenMonHoc = 150;
    public const int TenKhoaHoc = 150;
    public const int TenPhong = 100;

    // Contact & Personal Info (Thông tin liên hệ & cá nhân)
    public const int Email = 150;
    public const int DienThoai = 20;
    public const int DiaChi = 250;
    public const int ChuyenMon = 200;
    public const int GioiTinh = 20;

    // Descriptions (Mô tả)
    public const int MoTa = 500;

    // Account (Tài khoản)
    public const int TenDangNhap = 50;
    public const int MatKhauHash = 256;
    public const int VaiTro = 30;

    // Student Extended Profile (Hồ sơ học viên mở rộng)
    public const int SoDinhDanh = 20;
    public const int NoiCapGiayTo = 150;
    public const int HoTenNguoiGiamHo = 100;
    public const int DienThoaiNguoiGiamHo = 20;
    public const int QuanHeNguoiGiamHo = 50;
    public const int NguonTuyenSinh = 50;
    public const int NgheNghiep = 100;
    public const int DonViCongTac = 200;
    public const int GhiChuDacBiet = 1000;
}
