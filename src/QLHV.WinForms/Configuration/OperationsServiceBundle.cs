using QLHV.Core.Services;

namespace QLHV.WinForms.Configuration;

/// <summary>
/// Gói chứa toàn bộ các Service vận hành đào tạo, điểm số, học phí và báo cáo
/// </summary>
public sealed class OperationsServiceBundle
{
    public required ILopHocService LopHocService { get; init; }
    public required IDangKyLopHocService DangKyLopHocService { get; init; }
    public required ILichHocService LichHocService { get; init; }
    public required IKetQuaHocTapService KetQuaHocTapService { get; init; }
    public required IHocPhiService HocPhiService { get; init; }
    public required IBaoCaoService BaoCaoService { get; init; }
}
