using QLHV.Contracts.Dtos;

namespace QLHV.Core.Repositories;

public interface ITaiKhoanRepository
{
    Task<TaiKhoanDto?> GetByTenDangNhapAsync(string tenDangNhap);
    Task<string?> GetPasswordHashAsync(string tenDangNhap);
    Task<bool> UpdateLanDangNhapCuoiAsync(int id, DateTime thoiGian);
    Task<IReadOnlyList<TaiKhoanDto>> GetAllAsync();
}
