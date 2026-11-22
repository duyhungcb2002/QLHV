using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;

namespace QLHV.Core.Repositories;

public interface ILopHocRepository
{
    Task<IReadOnlyList<LopHocDto>> GetAllAsync(LopHocFilter? filter = null);
    Task<LopHocDto?> GetByIdAsync(int id);
    Task<LopHocDto?> GetByMaLopAsync(string maLop);
    Task<LopHocDto> AddAsync(LopHocDto dto);
    Task<bool> UpdateAsync(LopHocDto dto);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateAtomicAsync(LopHocDto dto);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage)> DeactivateAtomicAsync(int id);
    Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai);
}

public interface IDangKyLopHocRepository
{
    Task<IReadOnlyList<DangKyLopHocDto>> GetAllAsync(DangKyLopHocFilter? filter = null);
    Task<DangKyLopHocDto?> GetByIdAsync(int id);
    Task<DangKyLopHocDto?> GetByLopAndHocVienAsync(int lopHocId, int hocVienId);
    Task<DangKyLopHocDto> AddAsync(DangKyLopHocDto dto);
    Task<bool> UpdateAsync(DangKyLopHocDto dto);
    Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai);
    Task<int> CountByLopHocIdAsync(int lopHocId);
    Task<IReadOnlyList<CongNoHocPhiDto>> GetCongNoByLopHocAsync(int lopHocId);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage, DangKyLopHocDto? Data)> EnrollAtomicAsync(int lopHocId, int hocVienId, decimal hocPhiPhaiThu, string? ghiChu, string? maHocVien = null, string? hoTenHocVien = null, string? dienThoaiHocVien = null);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage)> CancelEnrollmentAtomicAsync(int dangKyLopHocId);
}

public interface ILichHocRepository
{
    Task<IReadOnlyList<LichHocDto>> GetAllAsync(LichHocFilter? filter = null);
    Task<LichHocDto?> GetByIdAsync(int id);
    Task<LichHocDto> AddAsync(LichHocDto dto);
    Task<bool> UpdateAsync(LichHocDto dto);
    Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai);
    Task<bool> DeleteAsync(int id);
    Task<bool> HasConflictPhongAsync(int phongHocId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null);
    Task<bool> HasConflictGiangVienAsync(int giangVienId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null);
    Task<bool> HasConflictLopHocAsync(int lopHocId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage, LichHocDto? Data)> CreateAtomicAsync(LichHocDto dto);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateAtomicAsync(LichHocDto dto);
}

public interface IKetQuaHocTapRepository
{
    Task<IReadOnlyList<KetQuaHocTapDto>> GetAllAsync(KetQuaHocTapFilter? filter = null);
    Task<KetQuaHocTapDto?> GetByIdAsync(int id);
    Task<KetQuaHocTapDto?> GetByDangKyAndMonAsync(int dangKyLopHocId, int monHocId);
    Task<KetQuaHocTapDto> UpsertAsync(KetQuaHocTapDto dto);
    Task<int> UpsertBatchAsync(IEnumerable<KetQuaHocTapDto> list);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByDangKyAndMonAsync(int dangKyLopHocId, int monHocId);
    Task<int> SaveGradesBatchAtomicAsync(IEnumerable<KetQuaHocTapDto> toUpsert, IEnumerable<int> dangKyIdsToDelete, int monHocId);
}

public interface IPhieuThuRepository
{
    Task<IReadOnlyList<PhieuThuDto>> GetAllAsync(PhieuThuFilter? filter = null);
    Task<PhieuThuDto?> GetByIdAsync(int id);
    Task<PhieuThuDto?> GetByMaPhieuThuAsync(string maPhieuThu);
    Task<PhieuThuDto> AddAsync(PhieuThuDto dto);
    Task<bool> CancelAsync(int id, string lyDoHuy, int? nguoiHuyTaiKhoanId = null);
    Task<decimal> GetTotalPaidByDangKyIdAsync(int dangKyLopHocId);
    Task<(bool Success, string? ErrorCode, string? ErrorMessage, PhieuThuDto? Data)> ThuHocPhiAtomicAsync(int dangKyLopHocId, decimal soTien, HinhThucThanhToan hinhThuc, int nguoiThuTaiKhoanId, string? ghiChu);
}

public interface IBaoCaoRepository
{
    Task<IReadOnlyList<BaoCaoHocVienTheoLopDto>> GetBaoCaoHocVienTheoLopAsync(int lopHocId);
    Task<IReadOnlyList<BaoCaoLichHocTheoLopDto>> GetBaoCaoLichHocTheoLopAsync(int lopHocId);
    Task<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>> GetBaoCaoGiangDayTheoGiangVienAsync(int giangVienId, DateTime? tuNgay = null, DateTime? denNgay = null);
    Task<IReadOnlyList<BaoCaoKetQuaHocTapDto>> GetBaoCaoKetQuaHocTapAsync(int? lopHocId, int? monHocId);
    Task<IReadOnlyList<ThongKeTyLeMonHocDto>> GetThongKeTyLeMonHocAsync(int? lopHocId);
    Task<IReadOnlyList<BaoCaoCongNoHocPhiDto>> GetBaoCaoCongNoHocPhiAsync(int? lopHocId = null);
}
