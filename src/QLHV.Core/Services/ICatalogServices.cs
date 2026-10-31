using QLHV.Contracts.Dtos;
using QLHV.Contracts.Results;

namespace QLHV.Core.Services;

public interface IAuthService
{
    Task<OperationResult<TaiKhoanDto>> LoginAsync(string tenDangNhap, string matKhau);
    Task<OperationResult> LogoutAsync();
}

public interface IHocVienService
{
    Task<OperationResult<IReadOnlyList<HocVienDto>>> GetAllAsync(HocVienFilter? filter = null);
    Task<OperationResult<HocVienDto>> GetByIdAsync(int id);
    Task<OperationResult<HocVienDto>> CreateAsync(HocVienDto dto);
    Task<OperationResult<HocVienDto>> UpdateAsync(HocVienDto dto);
    Task<OperationResult<bool>> DeactivateAsync(int id);
}

public interface IGiangVienService
{
    Task<OperationResult<IReadOnlyList<GiangVienDto>>> GetAllAsync(GiangVienFilter? filter = null);
    Task<OperationResult<GiangVienDto>> GetByIdAsync(int id);
    Task<OperationResult<GiangVienDto>> CreateAsync(GiangVienDto dto);
    Task<OperationResult<GiangVienDto>> UpdateAsync(GiangVienDto dto);
    Task<OperationResult<bool>> DeactivateAsync(int id);
}

public interface IMonHocService
{
    Task<OperationResult<IReadOnlyList<MonHocDto>>> GetAllAsync(MonHocFilter? filter = null);
    Task<OperationResult<MonHocDto>> GetByIdAsync(int id);
    Task<OperationResult<MonHocDto>> CreateAsync(MonHocDto dto);
    Task<OperationResult<MonHocDto>> UpdateAsync(MonHocDto dto);
    Task<OperationResult<bool>> DeactivateAsync(int id);
}

public interface IKhoaHocService
{
    Task<OperationResult<IReadOnlyList<KhoaHocDto>>> GetAllAsync(KhoaHocFilter? filter = null);
    Task<OperationResult<KhoaHocDto>> GetByIdAsync(int id);
    Task<OperationResult<KhoaHocDto>> CreateAsync(KhoaHocDto dto);
    Task<OperationResult<KhoaHocDto>> UpdateAsync(KhoaHocDto dto);
    Task<OperationResult<bool>> DeactivateAsync(int id);
}

public interface IKhoaHocMonHocService
{
    Task<OperationResult<IReadOnlyList<KhoaHocMonHocDto>>> GetByKhoaHocIdAsync(int khoaHocId);
    Task<OperationResult<KhoaHocMonHocDto>> AddMonHocAsync(KhoaHocMonHocDto dto);
    Task<OperationResult<bool>> RemoveMonHocAsync(int khoaHocId, int monHocId);
    Task<OperationResult<KhoaHocMonHocDto>> UpdateThuTuSoBuoiAsync(KhoaHocMonHocDto dto);
}

public interface IPhongHocService
{
    Task<OperationResult<IReadOnlyList<PhongHocDto>>> GetAllAsync(PhongHocFilter? filter = null);
    Task<OperationResult<PhongHocDto>> GetByIdAsync(int id);
    Task<OperationResult<PhongHocDto>> CreateAsync(PhongHocDto dto);
    Task<OperationResult<PhongHocDto>> UpdateAsync(PhongHocDto dto);
    Task<OperationResult<bool>> DeactivateAsync(int id);
}
