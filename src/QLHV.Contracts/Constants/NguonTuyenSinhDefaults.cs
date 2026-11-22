namespace QLHV.Contracts.Constants;

/// <summary>
/// Danh sách nguồn tuyển sinh chuẩn dùng chung trong toàn bộ hệ thống
/// </summary>
public static class NguonTuyenSinhDefaults
{
    public const string TrucTiep = "Trực tiếp";
    public const string Facebook = "Facebook";
    public const string Website = "Website";
    public const string Google = "Google";
    public const string GioiThieu = "Giới thiệu";
    public const string SuKienHoiThao = "Sự kiện/Hội thảo";
    public const string TikTok = "TikTok";
    public const string Khac = "Khác";

    public static readonly IReadOnlyList<string> All = new[]
    {
        TrucTiep,
        Facebook,
        Website,
        Google,
        GioiThieu,
        SuKienHoiThao,
        TikTok,
        Khac
    };
}
