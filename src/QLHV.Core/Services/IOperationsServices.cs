using QLHV.Contracts.Dtos;
using QLHV.Contracts.Results;

namespace QLHV.Core.Services;

public interface ILopHocService
{
    Task<OperationResult<IReadOnlyList<LopHocDto>>> GetAllAsync(LopHocFilter? filter = null);
    Task<OperationResult<LopHocDto>> GetByIdAsync(int id);
    Task<OperationResult<LopHocDto>> CreateAsync(LopHocDto dto);
    Task<OperationResult<LopHocDto>> UpdateAsync(LopHocDto dto);
    Task<OperationResult<bool>> DeactivateAsync(int id);
}

public interface IDangKyLopHocService
{
    Task<OperationResult<IReadOnlyList<DangKyLopHocDto>>> GetAllAsync(DangKyLopHocFilter? filter = null);
    Task<OperationResult<DangKyLopHocDto>> EnrollAsync(int lopHocId, int hocVienId, string? ghiChu = null);
    Task<OperationResult<bool>> CancelEnrollmentAsync(int id);
    Task<OperationResult<IReadOnlyList<CongNoHocPhiDto>>> GetCongNoByLopHocAsync(int lopHocId);
}

public interface ILichHocService
{
    Task<OperationResult<IReadOnlyList<LichHocDto>>> GetAllAsync(LichHocFilter? filter = null);
    Task<OperationResult<LichHocDto>> CreateAsync(LichHocDto dto);
    Task<OperationResult<LichHocDto>> UpdateAsync(LichHocDto dto);
    Task<OperationResult<bool>> DeleteAsync(int id);
}

public interface IKetQuaHocTapService
{
    Task<OperationResult<IReadOnlyList<LopHocDto>>> GetLopHocOptionsAsync();
    Task<OperationResult<IReadOnlyList<MonHocDto>>> GetMonHocOptionsAsync(int? lopHocId = null);
    Task<OperationResult<IReadOnlyList<KetQuaHocTapDto>>> GetByLopAndMonAsync(int lopHocId, int monHocId);
    Task<OperationResult<KetQuaHocTapDto>> SaveGradeAsync(KetQuaHocTapDto dto);
    Task<OperationResult<bool>> DeleteGradeAsync(int dangKyLopHocId, int monHocId);
    Task<OperationResult<int>> SaveGradesBatchAsync(IEnumerable<KetQuaHocTapDto> dtos);
}

public interface IHocPhiService
{
    Task<OperationResult<IReadOnlyList<LopHocDto>>> GetLopHocOptionsAsync();
    Task<OperationResult<IReadOnlyList<PhieuThuDto>>> GetAllAsync(PhieuThuFilter? filter = null);
    Task<OperationResult<PhieuThuDto>> GetByIdAsync(int id);
    Task<OperationResult<PhieuThuDto>> ThuHocPhiAsync(int dangKyLopHocId, decimal soTien, Contracts.Enums.HinhThucThanhToan hinhThuc, string? ghiChu = null);
    Task<OperationResult<bool>> HuyPhieuThuAsync(int id, string lyDoHuy);
    Task<OperationResult<decimal>> GetRemainingFeeAsync(int dangKyLopHocId);
}

public interface IBaoCaoService
{
    Task<OperationResult<IReadOnlyList<LopHocDto>>> GetLopHocFilterOptionsAsync();
    Task<OperationResult<IReadOnlyList<BaoCaoHocVienTheoLopDto>>> GetBaoCaoHocVienTheoLopAsync(int lopHocId);
    Task<OperationResult<IReadOnlyList<BaoCaoLichHocTheoLopDto>>> GetBaoCaoLichHocTheoLopAsync(int lopHocId);
    Task<OperationResult<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>>> GetBaoCaoGiangDayTheoGiangVienAsync(int giangVienId, DateTime? tuNgay = null, DateTime? denNgay = null);
    Task<OperationResult<IReadOnlyList<BaoCaoKetQuaHocTapDto>>> GetBaoCaoKetQuaHocTapAsync(int? lopHocId, int? monHocId);
    Task<OperationResult<IReadOnlyList<ThongKeTyLeMonHocDto>>> GetThongKeTyLeMonHocAsync(int? lopHocId);
    Task<OperationResult<IReadOnlyList<BaoCaoCongNoHocPhiDto>>> GetBaoCaoCongNoHocPhiAsync(int? lopHocId = null);
}
