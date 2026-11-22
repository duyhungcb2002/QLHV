using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Repositories;

namespace QLHV.Features.Training.Repositories.Fake;

/// <summary>
/// Fake repository cho Lịch học (LichHoc) với hỗ trợ kiểm tra xung đột nguyên tử
/// </summary>
public class FakeLichHocRepository : ILichHocRepository
{
    private readonly FakeTrainingStore _store;
    private readonly IMonHocRepository? _monHocRepository;
    private readonly IGiangVienRepository? _giangVienRepository;
    private readonly IPhongHocRepository? _phongHocRepository;
    private readonly IKhoaHocMonHocRepository? _khoaHocMonHocRepository;

    public FakeLichHocRepository(
        FakeTrainingStore store,
        IMonHocRepository? monHocRepository = null,
        IGiangVienRepository? giangVienRepository = null,
        IPhongHocRepository? phongHocRepository = null,
        IKhoaHocMonHocRepository? khoaHocMonHocRepository = null)
    {
        _store = store;
        _monHocRepository = monHocRepository;
        _giangVienRepository = giangVienRepository;
        _phongHocRepository = phongHocRepository;
        _khoaHocMonHocRepository = khoaHocMonHocRepository;
    }

    public Task<IReadOnlyList<LichHocDto>> GetAllAsync(LichHocFilter? filter = null)
    {
        lock (_store.SyncLock)
        {
            var query = _store.LichHocs.AsEnumerable();

            if (filter != null)
            {
                if (filter.LopHocId.HasValue)
                {
                    query = query.Where(x => x.LopHocId == filter.LopHocId.Value);
                }

                if (filter.MonHocId.HasValue)
                {
                    query = query.Where(x => x.MonHocId == filter.MonHocId.Value);
                }

                if (filter.GiangVienId.HasValue)
                {
                    query = query.Where(x => x.GiangVienId == filter.GiangVienId.Value);
                }

                if (filter.PhongHocId.HasValue)
                {
                    query = query.Where(x => x.PhongHocId == filter.PhongHocId.Value);
                }

                if (filter.TuNgay.HasValue)
                {
                    query = query.Where(x => x.NgayHoc.Date >= filter.TuNgay.Value.Date);
                }

                if (filter.DenNgay.HasValue)
                {
                    query = query.Where(x => x.NgayHoc.Date <= filter.DenNgay.Value.Date);
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(x => x.TrangThai == filter.TrangThai.Value);
                }
            }

            var list = query.Select(Clone).OrderBy(x => x.NgayHoc).ThenBy(x => x.GioBatDau).ToList();
            return Task.FromResult<IReadOnlyList<LichHocDto>>(list);
        }
    }

    public Task<LichHocDto?> GetByIdAsync(int id)
    {
        lock (_store.SyncLock)
        {
            var item = _store.LichHocs.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<LichHocDto> AddAsync(LichHocDto dto)
    {
        lock (_store.SyncLock)
        {
            dto.Id = _store.NextLichHocId();
            var clone = Clone(dto);
            _store.LichHocs.Add(clone);
            return Task.FromResult(Clone(clone));
        }
    }

    public Task<bool> UpdateAsync(LichHocDto dto)
    {
        lock (_store.SyncLock)
        {
            var item = _store.LichHocs.FirstOrDefault(x => x.Id == dto.Id);
            if (item == null) return Task.FromResult(false);

            item.MonHocId = dto.MonHocId;
            item.GiangVienId = dto.GiangVienId;
            item.PhongHocId = dto.PhongHocId;
            item.NgayHoc = dto.NgayHoc;
            item.GioBatDau = dto.GioBatDau;
            item.GioKetThuc = dto.GioKetThuc;
            item.NoiDung = dto.NoiDung;
            item.TrangThai = dto.TrangThai;

            return Task.FromResult(true);
        }
    }

    public Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        lock (_store.SyncLock)
        {
            var item = _store.LichHocs.FirstOrDefault(x => x.Id == id);
            if (item == null) return Task.FromResult(false);

            item.TrangThai = trangThai;
            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        // Chuyển sang Soft Delete (Ngừng hoạt động)
        return SetTrangThaiAsync(id, TrangThaiHoatDong.NgungHoatDong);
    }

    public Task<bool> HasConflictPhongAsync(int phongHocId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null)
    {
        lock (_store.SyncLock)
        {
            var hasConflict = _store.LichHocs.Any(x =>
                x.PhongHocId == phongHocId &&
                x.NgayHoc.Date == ngayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (!excludeId.HasValue || x.Id != excludeId.Value) &&
                (gioBatDau < x.GioKetThuc && gioKetThuc > x.GioBatDau));

            return Task.FromResult(hasConflict);
        }
    }

    public Task<bool> HasConflictGiangVienAsync(int giangVienId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null)
    {
        lock (_store.SyncLock)
        {
            var hasConflict = _store.LichHocs.Any(x =>
                x.GiangVienId == giangVienId &&
                x.NgayHoc.Date == ngayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (!excludeId.HasValue || x.Id != excludeId.Value) &&
                (gioBatDau < x.GioKetThuc && gioKetThuc > x.GioBatDau));

            return Task.FromResult(hasConflict);
        }
    }

    public Task<bool> HasConflictLopHocAsync(int lopHocId, DateTime ngayHoc, TimeSpan gioBatDau, TimeSpan gioKetThuc, int? excludeId = null)
    {
        lock (_store.SyncLock)
        {
            var hasConflict = _store.LichHocs.Any(x =>
                x.LopHocId == lopHocId &&
                x.NgayHoc.Date == ngayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (!excludeId.HasValue || x.Id != excludeId.Value) &&
                (gioBatDau < x.GioKetThuc && gioKetThuc > x.GioBatDau));

            return Task.FromResult(hasConflict);
        }
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage, LichHocDto? Data)> CreateAtomicAsync(LichHocDto dto)
    {
        // 1. Sao chép và tra cứu dữ liệu danh mục ngoài lock
        MonHocDto? mon = null;
        if (_monHocRepository != null)
        {
            mon = await _monHocRepository.GetByIdAsync(dto.MonHocId);
            if (mon == null || mon.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không tồn tại hoặc đã ngừng hoạt động.", null);
        }

        GiangVienDto? gv = null;
        if (_giangVienRepository != null)
        {
            gv = await _giangVienRepository.GetByIdAsync(dto.GiangVienId);
            if (gv == null || gv.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên không tồn tại hoặc đã ngừng hoạt động.", null);
        }

        PhongHocDto? phong = null;
        if (_phongHocRepository != null)
        {
            phong = await _phongHocRepository.GetByIdAsync(dto.PhongHocId);
            if (phong == null || phong.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học không tồn tại hoặc đã ngừng hoạt động.", null);
        }

        lock (_store.SyncLock)
        {
            var lop = _store.LopHocs.FirstOrDefault(l => l.Id == dto.LopHocId);
            if (lop == null || lop.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học không tồn tại hoặc đã ngừng hoạt động.", null);

            // Kiểm tra môn thuộc khóa học của lớp
            if (_khoaHocMonHocRepository != null)
            {
                var kmList = _khoaHocMonHocRepository.GetByKhoaHocIdAsync(lop.KhoaHocId).GetAwaiter().GetResult();
                if (!kmList.Any(km => km.MonHocId == dto.MonHocId))
                {
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của khóa học này.", null);
                }
            }

            if (dto.NgayHoc.Date < lop.NgayBatDau.Date)
                return (false, ErrorCodes.VALIDATION_ERROR, $"Ngày học không được trước ngày bắt đầu của lớp ({lop.NgayBatDau:dd/MM/yyyy}).", null);

            if (lop.NgayKetThuc.HasValue && dto.NgayHoc.Date > lop.NgayKetThuc.Value.Date)
                return (false, ErrorCodes.VALIDATION_ERROR, $"Ngày học không được sau ngày kết thúc của lớp ({lop.NgayKetThuc.Value:dd/MM/yyyy}).", null);

            if (phong != null && phong.SucChua < lop.SiSoToiDa)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, $"Sức chứa phòng học ({phong.SucChua} chỗ) nhỏ hơn sĩ số tối đa của lớp ({lop.SiSoToiDa} học viên).", null);

            // Kiểm tra xung đột phòng
            bool conflictPhong = _store.LichHocs.Any(x =>
                x.PhongHocId == dto.PhongHocId &&
                x.NgayHoc.Date == dto.NgayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (dto.GioBatDau < x.GioKetThuc && dto.GioKetThuc > x.GioBatDau));
            if (conflictPhong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học đã có lịch xếp trong khung giờ này.", null);

            // Kiểm tra xung đột giảng viên
            bool conflictGv = _store.LichHocs.Any(x =>
                x.GiangVienId == dto.GiangVienId &&
                x.NgayHoc.Date == dto.NgayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (dto.GioBatDau < x.GioKetThuc && dto.GioKetThuc > x.GioBatDau));
            if (conflictGv)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên đã có lịch dạy trong khung giờ này.", null);

            // Kiểm tra xung đột lớp học
            bool conflictLop = _store.LichHocs.Any(x =>
                x.LopHocId == dto.LopHocId &&
                x.NgayHoc.Date == dto.NgayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (dto.GioBatDau < x.GioKetThuc && dto.GioKetThuc > x.GioBatDau));
            if (conflictLop)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã có buổi học khác trong khung giờ này.", null);

            dto.Id = _store.NextLichHocId();
            if (mon != null)
            {
                dto.MaMonHoc = mon.MaMonHoc;
                dto.TenMonHoc = mon.TenMonHoc;
            }
            if (gv != null)
            {
                dto.MaGiangVien = gv.MaGiangVien;
                dto.HoTenGiangVien = gv.HoTen;
            }
            if (phong != null)
            {
                dto.MaPhong = phong.MaPhong;
                dto.TenPhong = phong.TenPhong;
            }
            dto.TrangThai = TrangThaiHoatDong.DangHoatDong;
            var clone = Clone(dto);
            _store.LichHocs.Add(clone);
            return (true, null, null, Clone(clone));
        }
    }

    public async Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateAtomicAsync(LichHocDto dto)
    {
        // 1. Tra cứu danh mục ngoài lock
        MonHocDto? mon = null;
        if (_monHocRepository != null)
        {
            mon = await _monHocRepository.GetByIdAsync(dto.MonHocId);
            if (mon == null || mon.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không tồn tại hoặc đã ngừng hoạt động.");
        }

        GiangVienDto? gv = null;
        if (_giangVienRepository != null)
        {
            gv = await _giangVienRepository.GetByIdAsync(dto.GiangVienId);
            if (gv == null || gv.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên không tồn tại hoặc đã ngừng hoạt động.");
        }

        PhongHocDto? phong = null;
        if (_phongHocRepository != null)
        {
            phong = await _phongHocRepository.GetByIdAsync(dto.PhongHocId);
            if (phong == null || phong.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học không tồn tại hoặc đã ngừng hoạt động.");
        }

        lock (_store.SyncLock)
        {
            var item = _store.LichHocs.FirstOrDefault(x => x.Id == dto.Id);
            if (item == null)
                return (false, ErrorCodes.NOT_FOUND, "Không tìm thấy buổi học cần sửa.");

            if (item.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể chỉnh sửa buổi học đã ngừng hoạt động/đã bị hủy.");

            var lop = _store.LopHocs.FirstOrDefault(l => l.Id == item.LopHocId);
            if (lop == null || lop.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động.");

            // Kiểm tra môn thuộc khóa học của lớp
            if (_khoaHocMonHocRepository != null)
            {
                var kmList = _khoaHocMonHocRepository.GetByKhoaHocIdAsync(lop.KhoaHocId).GetAwaiter().GetResult();
                if (!kmList.Any(km => km.MonHocId == dto.MonHocId))
                {
                    return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của khóa học này.");
                }
            }

            string maMonHoc = mon?.MaMonHoc ?? dto.MaMonHoc;
            string tenMonHoc = mon?.TenMonHoc ?? dto.TenMonHoc;
            string maGv = gv?.MaGiangVien ?? dto.MaGiangVien;
            string hoTenGv = gv?.HoTen ?? dto.HoTenGiangVien;
            string maPhong = phong?.MaPhong ?? dto.MaPhong;
            string tenPhong = phong?.TenPhong ?? dto.TenPhong;

            if (phong != null && phong.SucChua < lop.SiSoToiDa)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, $"Sức chứa phòng học ({phong.SucChua} chỗ) nhỏ hơn sĩ số tối đa của lớp ({lop.SiSoToiDa} học viên).");

            if (dto.NgayHoc.Date < lop.NgayBatDau.Date)
                return (false, ErrorCodes.VALIDATION_ERROR, $"Ngày học không được trước ngày bắt đầu của lớp ({lop.NgayBatDau:dd/MM/yyyy}).");

            if (lop.NgayKetThuc.HasValue && dto.NgayHoc.Date > lop.NgayKetThuc.Value.Date)
                return (false, ErrorCodes.VALIDATION_ERROR, $"Ngày học không được sau ngày kết thúc của lớp ({lop.NgayKetThuc.Value:dd/MM/yyyy}).");

            bool conflictPhong = _store.LichHocs.Any(x =>
                x.Id != dto.Id &&
                x.PhongHocId == dto.PhongHocId &&
                x.NgayHoc.Date == dto.NgayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (dto.GioBatDau < x.GioKetThuc && dto.GioKetThuc > x.GioBatDau));
            if (conflictPhong)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học đã có lịch xếp trong khung giờ này.");

            bool conflictGv = _store.LichHocs.Any(x =>
                x.Id != dto.Id &&
                x.GiangVienId == dto.GiangVienId &&
                x.NgayHoc.Date == dto.NgayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (dto.GioBatDau < x.GioKetThuc && dto.GioKetThuc > x.GioBatDau));
            if (conflictGv)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên đã có lịch dạy trong khung giờ này.");

            bool conflictLop = _store.LichHocs.Any(x =>
                x.Id != dto.Id &&
                x.LopHocId == item.LopHocId &&
                x.NgayHoc.Date == dto.NgayHoc.Date &&
                x.TrangThai == TrangThaiHoatDong.DangHoatDong &&
                (dto.GioBatDau < x.GioKetThuc && dto.GioKetThuc > x.GioBatDau));
            if (conflictLop)
                return (false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã có buổi học khác trong khung giờ này.");

            item.MonHocId = dto.MonHocId;
            item.MaMonHoc = maMonHoc;
            item.TenMonHoc = tenMonHoc;
            item.GiangVienId = dto.GiangVienId;
            item.MaGiangVien = maGv;
            item.HoTenGiangVien = hoTenGv;
            item.PhongHocId = dto.PhongHocId;
            item.MaPhong = maPhong;
            item.TenPhong = tenPhong;
            item.NgayHoc = dto.NgayHoc;
            item.GioBatDau = dto.GioBatDau;
            item.GioKetThuc = dto.GioKetThuc;
            item.NoiDung = dto.NoiDung;
            item.TrangThai = TrangThaiHoatDong.DangHoatDong;

            return (true, null, null);
        }
    }

    private static LichHocDto Clone(LichHocDto src) => new()
    {
        Id = src.Id,
        LopHocId = src.LopHocId,
        MaLopHoc = src.MaLopHoc,
        TenLopHoc = src.TenLopHoc,
        MonHocId = src.MonHocId,
        MaMonHoc = src.MaMonHoc,
        TenMonHoc = src.TenMonHoc,
        GiangVienId = src.GiangVienId,
        MaGiangVien = src.MaGiangVien,
        HoTenGiangVien = src.HoTenGiangVien,
        PhongHocId = src.PhongHocId,
        MaPhong = src.MaPhong,
        TenPhong = src.TenPhong,
        NgayHoc = src.NgayHoc,
        GioBatDau = src.GioBatDau,
        GioKetThuc = src.GioKetThuc,
        NoiDung = src.NoiDung,
        TrangThai = src.TrangThai
    };
}
