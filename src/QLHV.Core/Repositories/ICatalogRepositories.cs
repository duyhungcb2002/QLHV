using QLHV.Contracts.Dtos;

namespace QLHV.Core.Repositories;

public interface IHocVienRepository
{
    Task<IReadOnlyList<HocVienDto>> GetAllAsync(HocVienFilter? filter = null);
    Task<HocVienDto?> GetByIdAsync(int id);
    Task<HocVienDto?> GetByMaHocVienAsync(string maHocVien);
    Task<HocVienDto?> GetBySoDinhDanhAsync(string soDinhDanh);
    Task<HocVienDto> AddAsync(HocVienDto dto);
    Task<bool> UpdateAsync(HocVienDto dto);
    Task<bool> SetTrangThaiAsync(int id, Contracts.Enums.TrangThaiHoatDong trangThai);
}

public interface IGiangVienRepository
{
    Task<IReadOnlyList<GiangVienDto>> GetAllAsync(GiangVienFilter? filter = null);
    Task<GiangVienDto?> GetByIdAsync(int id);
    Task<GiangVienDto?> GetByMaGiangVienAsync(string maGiangVien);
    Task<GiangVienDto> AddAsync(GiangVienDto dto);
    Task<bool> UpdateAsync(GiangVienDto dto);
    Task<bool> SetTrangThaiAsync(int id, Contracts.Enums.TrangThaiHoatDong trangThai);
}

public interface IMonHocRepository
{
    Task<IReadOnlyList<MonHocDto>> GetAllAsync(MonHocFilter? filter = null);
    Task<MonHocDto?> GetByIdAsync(int id);
    Task<MonHocDto?> GetByMaMonHocAsync(string maMonHoc);
    Task<MonHocDto> AddAsync(MonHocDto dto);
    Task<bool> UpdateAsync(MonHocDto dto);
    Task<bool> SetTrangThaiAsync(int id, Contracts.Enums.TrangThaiHoatDong trangThai);
}

public interface IKhoaHocRepository
{
    Task<IReadOnlyList<KhoaHocDto>> GetAllAsync(KhoaHocFilter? filter = null);
    Task<KhoaHocDto?> GetByIdAsync(int id);
    Task<KhoaHocDto?> GetByMaKhoaHocAsync(string maKhoaHoc);
    Task<KhoaHocDto> AddAsync(KhoaHocDto dto);
    Task<bool> UpdateAsync(KhoaHocDto dto);
    Task<bool> SetTrangThaiAsync(int id, Contracts.Enums.TrangThaiHoatDong trangThai);
}

public interface IKhoaHocMonHocRepository
{
    Task<IReadOnlyList<KhoaHocMonHocDto>> GetByKhoaHocIdAsync(int khoaHocId);
    Task<KhoaHocMonHocDto?> GetAsync(int khoaHocId, int monHocId);
    Task<KhoaHocMonHocDto> AddAsync(KhoaHocMonHocDto dto);
    Task<bool> UpdateAsync(KhoaHocMonHocDto dto);
    Task<bool> DeleteAsync(int khoaHocId, int monHocId);
}

public interface IPhongHocRepository
{
    Task<IReadOnlyList<PhongHocDto>> GetAllAsync(PhongHocFilter? filter = null);
    Task<PhongHocDto?> GetByIdAsync(int id);
    Task<PhongHocDto?> GetByMaPhongAsync(string maPhong);
    Task<PhongHocDto> AddAsync(PhongHocDto dto);
    Task<bool> UpdateAsync(PhongHocDto dto);
    Task<bool> SetTrangThaiAsync(int id, Contracts.Enums.TrangThaiHoatDong trangThai);
}
