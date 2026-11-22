using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Repositories;

namespace QLHV.Features.Training.Repositories.Fake;

/// <summary>
/// Fake repository cho Lớp học
/// </summary>
public class FakeLopHocRepository : ILopHocRepository
{
    private readonly FakeTrainingStore _store;
    private readonly IPhongHocRepository? _phongHocRepository;

    public FakeLopHocRepository(FakeTrainingStore store, IPhongHocRepository? phongHocRepository = null)
    {
        _store = store;
        _phongHocRepository = phongHocRepository;
    }

    public Task<IReadOnlyList<LopHocDto>> GetAllAsync(LopHocFilter? filter = null)
    {
        lock (_store.SyncLock)
        {
            var query = _store.LopHocs.AsEnumerable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
                {
                    var kw = filter.TuKhoa.Trim();
                    query = query.Where(x => x.MaLopHoc.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                             x.TenLopHoc.Contains(kw, StringComparison.OrdinalIgnoreCase));
                }

                if (filter.KhoaHocId.HasValue)
                {
                    query = query.Where(x => x.KhoaHocId == filter.KhoaHocId.Value);
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(x => x.TrangThai == filter.TrangThai.Value);
                }
            }

            // Đồng bộ sĩ số hiện tại từ DangKyLopHoc
            var list = query.Select(x =>
            {
                var clone = Clone(x);
                clone.SiSoHienTai = _store.DangKyLopHocs.Count(dk => dk.LopHocId == x.Id && dk.TrangThai == TrangThaiHoatDong.DangHoatDong);
                return clone;
            }).OrderByDescending(x => x.Id).ToList();

            return Task.FromResult<IReadOnlyList<LopHocDto>>(list);
        }
    }

    public Task<LopHocDto?> GetByIdAsync(int id)
    {
        lock (_store.SyncLock)
        {
            var item = _store.LopHocs.FirstOrDefault(x => x.Id == id);
            if (item == null) return Task.FromResult<LopHocDto?>(null);

            var clone = Clone(item);
            clone.SiSoHienTai = _store.DangKyLopHocs.Count(dk => dk.LopHocId == item.Id && dk.TrangThai == TrangThaiHoatDong.DangHoatDong);
            return Task.FromResult<LopHocDto?>(clone);
        }
    }

    public Task<LopHocDto?> GetByMaLopAsync(string maLop)
    {
        lock (_store.SyncLock)
        {
            var item = _store.LopHocs.FirstOrDefault(x => x.MaLopHoc.Equals(maLop.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<LopHocDto> AddAsync(LopHocDto dto)
    {
        lock (_store.SyncLock)
        {
            dto.Id = _store.NextLopHocId();
            dto.NgayTao = DateTime.Now;
            var clone = Clone(dto);
            _store.LopHocs.Add(clone);
            return Task.FromResult(Clone(clone));
        }
    }

    public Task<bool> UpdateAsync(LopHocDto dto)
    {
        lock (_store.SyncLock)
        {
            var index = _store.LopHocs.FindIndex(x => x.Id == dto.Id);
            if (index < 0) return Task.FromResult(false);

            var existing = _store.LopHocs[index];
            existing.TenLopHoc = dto.TenLopHoc;
            existing.KhoaHocId = dto.KhoaHocId;
            existing.TenKhoaHoc = dto.TenKhoaHoc;
            existing.NgayBatDau = dto.NgayBatDau;
            existing.NgayKetThuc = dto.NgayKetThuc;
            existing.SiSoToiDa = dto.SiSoToiDa;
            existing.GhiChu = dto.GhiChu;
            existing.TrangThai = dto.TrangThai;
            existing.GiangVienChuNhiemId = dto.GiangVienChuNhiemId;
            existing.HoTenGiangVienChuNhiem = dto.HoTenGiangVienChuNhiem;

            return Task.FromResult(true);
        }
    }

    public Task<(bool Success, string? ErrorCode, string? ErrorMessage)> UpdateAtomicAsync(LopHocDto dto)
    {
        lock (_store.SyncLock)
        {
            var index = _store.LopHocs.FindIndex(x => x.Id == dto.Id);
            if (index < 0)
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.NOT_FOUND, $"Không tìm thấy lớp học ID={dto.Id}."));

            var existing = _store.LopHocs[index];

            // Nếu trạng thái đang là NgungHoatDong, từ chối chỉnh sửa/kích hoạt lại qua UpdateAtomic
            if (existing.TrangThai == TrangThaiHoatDong.NgungHoatDong)
            {
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động, không thể chỉnh sửa thông tin."));
            }

            // Đảm bảo không đổi trạng thái qua update thông thường
            dto.TrangThai = existing.TrangThai;

            int activeCount = _store.DangKyLopHocs.Count(x => x.LopHocId == dto.Id && x.TrangThai == TrangThaiHoatDong.DangHoatDong);
            if (dto.SiSoToiDa < activeCount)
            {
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.VALIDATION_ERROR,
                    $"Không thể giảm sĩ số tối đa xuống {dto.SiSoToiDa} vì lớp đang có {activeCount} học viên đang học."));
            }

            // Đổi khóa học: Không được phép nếu đã có lịch sử enrollment, schedule hoặc grade
            if (existing.KhoaHocId != dto.KhoaHocId)
            {
                bool hasHistory = _store.DangKyLopHocs.Any(x => x.LopHocId == dto.Id) ||
                                  _store.LichHocs.Any(x => x.LopHocId == dto.Id) ||
                                  _store.KetQuaHocTaps.Any(x => x.LopHocId == dto.Id);
                if (hasHistory)
                {
                    return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                        "Không thể thay đổi khóa học của lớp khi lớp đã phát sinh danh sách học viên, lịch học hoặc điểm số."));
                }
            }

            // Kiểm tra lịch học active phải nằm trong [NgayBatDau, NgayKetThuc]
            var activeSchedules = _store.LichHocs.Where(x => x.LopHocId == dto.Id && x.TrangThai == TrangThaiHoatDong.DangHoatDong).ToList();
            bool dateConflict = activeSchedules.Any(s => s.NgayHoc.Date < dto.NgayBatDau.Date || (dto.NgayKetThuc.HasValue && s.NgayHoc.Date > dto.NgayKetThuc.Value.Date));
            if (dateConflict)
            {
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    "Thời gian mới của lớp học xung đột với các lịch học đang hoạt động (có lịch học nằm ngoài khoảng thời gian này)."));
            }

            // Kiểm tra sức chứa phòng: dto.SiSoToiDa <= phong.SucChua
            if (_phongHocRepository != null)
            {
                foreach (var s in activeSchedules)
                {
                    var phong = _phongHocRepository.GetByIdAsync(s.PhongHocId).GetAwaiter().GetResult();
                    if (phong != null && dto.SiSoToiDa > phong.SucChua)
                    {
                        return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                            $"Sĩ số tối đa ({dto.SiSoToiDa}) vượt quá sức chứa ({phong.SucChua}) của phòng học '{phong.TenPhong}' đã xếp cho lớp."));
                    }
                }
            }

            existing.TenLopHoc = dto.TenLopHoc;
            existing.KhoaHocId = dto.KhoaHocId;
            existing.TenKhoaHoc = dto.TenKhoaHoc;
            existing.NgayBatDau = dto.NgayBatDau;
            existing.NgayKetThuc = dto.NgayKetThuc;
            existing.SiSoToiDa = dto.SiSoToiDa;
            existing.GhiChu = dto.GhiChu;
            existing.TrangThai = dto.TrangThai;
            existing.GiangVienChuNhiemId = dto.GiangVienChuNhiemId;
            existing.HoTenGiangVienChuNhiem = dto.HoTenGiangVienChuNhiem;

            return Task.FromResult<(bool, string?, string?)>((true, null, null));
        }
    }

    public Task<(bool Success, string? ErrorCode, string? ErrorMessage)> DeactivateAtomicAsync(int id)
    {
        lock (_store.SyncLock)
        {
            var item = _store.LopHocs.FirstOrDefault(x => x.Id == id);
            if (item == null)
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.NOT_FOUND, $"Không tìm thấy lớp học ID={id}."));

            if (item.TrangThai == TrangThaiHoatDong.NgungHoatDong)
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ở trạng thái ngừng hoạt động trước đó."));

            int activeStudents = _store.DangKyLopHocs.Count(x => x.LopHocId == id && x.TrangThai == TrangThaiHoatDong.DangHoatDong);
            if (activeStudents > 0)
            {
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Không thể ngừng hoạt động lớp học vì vẫn còn {activeStudents} học viên đang theo học. Vui lòng chuyển hoặc hủy đăng ký trước."));
            }

            int activeSchedules = _store.LichHocs.Count(x => x.LopHocId == id && x.TrangThai == TrangThaiHoatDong.DangHoatDong);
            if (activeSchedules > 0)
            {
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Không thể ngừng hoạt động lớp học vì vẫn còn {activeSchedules} buổi học đang hoạt động. Vui lòng hủy các buổi học trước."));
            }

            item.TrangThai = TrangThaiHoatDong.NgungHoatDong;
            return Task.FromResult<(bool, string?, string?)>((true, null, null));
        }
    }

    public Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        lock (_store.SyncLock)
        {
            var item = _store.LopHocs.FirstOrDefault(x => x.Id == id);
            if (item == null) return Task.FromResult(false);
            item.TrangThai = trangThai;
            return Task.FromResult(true);
        }
    }

    private static LopHocDto Clone(LopHocDto src) => new()
    {
        Id = src.Id,
        MaLopHoc = src.MaLopHoc,
        TenLopHoc = src.TenLopHoc,
        KhoaHocId = src.KhoaHocId,
        TenKhoaHoc = src.TenKhoaHoc,
        NgayBatDau = src.NgayBatDau,
        NgayKetThuc = src.NgayKetThuc,
        SiSoToiDa = src.SiSoToiDa,
        SiSoHienTai = src.SiSoHienTai,
        GhiChu = src.GhiChu,
        TrangThai = src.TrangThai,
        NgayTao = src.NgayTao,
        GiangVienChuNhiemId = src.GiangVienChuNhiemId,
        HoTenGiangVienChuNhiem = src.HoTenGiangVienChuNhiem
    };
}
