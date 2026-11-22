using QLHV.Core.Repositories;

namespace QLHV.WinForms.Configuration;

/// <summary>
/// Gói chứa toàn bộ 7 repositories danh mục của hệ thống
/// </summary>
public sealed class RepositoryBundle
{
    public required ITaiKhoanRepository TaiKhoanRepository { get; init; }
    public required IHocVienRepository HocVienRepository { get; init; }
    public required IGiangVienRepository GiangVienRepository { get; init; }
    public required IMonHocRepository MonHocRepository { get; init; }
    public required IKhoaHocRepository KhoaHocRepository { get; init; }
    public required IKhoaHocMonHocRepository KhoaHocMonHocRepository { get; init; }
    public required IPhongHocRepository PhongHocRepository { get; init; }

    // Operations repositories
    public required ILopHocRepository LopHocRepository { get; init; }
    public required IDangKyLopHocRepository DangKyLopHocRepository { get; init; }
    public required ILichHocRepository LichHocRepository { get; init; }
    public required IKetQuaHocTapRepository KetQuaHocTapRepository { get; init; }
    public required IPhieuThuRepository PhieuThuRepository { get; init; }
    public required IBaoCaoRepository BaoCaoRepository { get; init; }
}
