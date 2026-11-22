using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Repositories;

namespace QLHV.Features.Training.Repositories.Fake;

/// <summary>
/// Fake repository cho Xếp học viên vào lớp (DangKyLopHoc) với hỗ trợ atomic concurrency
/// </summary>
public class FakeDangKyLopHocRepository : IDangKyLopHocRepository
{
    private readonly FakeTrainingStore _store;

    public FakeDangKyLopHocRepository(FakeTrainingStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<DangKyLopHocDto>> GetAllAsync(DangKyLopHocFilter? filter = null)
    {
        lock (_store.SyncLock)
        {
            var query = _store.DangKyLopHocs.AsEnumerable();

            if (filter != null)
            {
                if (filter.LopHocId.HasValue)
                {
                    query = query.Where(x => x.LopHocId == filter.LopHocId.Value);
                }

                if (filter.HocVienId.HasValue)
                {
                    query = query.Where(x => x.HocVienId == filter.HocVienId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
                {
                    var kw = filter.TuKhoa.Trim();
                    query = query.Where(x => x.MaHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                             x.HoTenHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                             x.MaLopHoc.Contains(kw, StringComparison.OrdinalIgnoreCase));
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(x => x.TrangThai == filter.TrangThai.Value);
                }
            }

            var list = query.Select(x =>
            {
                var clone = Clone(x);
                clone.DaThu = _store.PhieuThus
                    .Where(p => p.DangKyLopHocId == x.Id && p.TrangThai == TrangThaiPhieuThu.HieuLuc)
                    .Sum(p => p.SoTien);
                return clone;
            }).OrderByDescending(x => x.Id).ToList();

            return Task.FromResult<IReadOnlyList<DangKyLopHocDto>>(list);
        }
    }

    public Task<DangKyLopHocDto?> GetByIdAsync(int id)
    {
        lock (_store.SyncLock)
        {
            var item = _store.DangKyLopHocs.FirstOrDefault(x => x.Id == id);
            if (item == null) return Task.FromResult<DangKyLopHocDto?>(null);

            var clone = Clone(item);
            clone.DaThu = _store.PhieuThus
                .Where(p => p.DangKyLopHocId == item.Id && p.TrangThai == TrangThaiPhieuThu.HieuLuc)
                .Sum(p => p.SoTien);
            return Task.FromResult<DangKyLopHocDto?>(clone);
        }
    }

    public Task<DangKyLopHocDto?> GetByLopAndHocVienAsync(int lopHocId, int hocVienId)
    {
        lock (_store.SyncLock)
        {
            var item = _store.DangKyLopHocs.FirstOrDefault(x => x.LopHocId == lopHocId && x.HocVienId == hocVienId);
            if (item == null) return Task.FromResult<DangKyLopHocDto?>(null);

            var clone = Clone(item);
            clone.DaThu = _store.PhieuThus
                .Where(p => p.DangKyLopHocId == item.Id && p.TrangThai == TrangThaiPhieuThu.HieuLuc)
                .Sum(p => p.SoTien);
            return Task.FromResult<DangKyLopHocDto?>(clone);
        }
    }

    public Task<DangKyLopHocDto> AddAsync(DangKyLopHocDto dto)
    {
        lock (_store.SyncLock)
        {
            dto.Id = _store.NextDangKyId();
            dto.NgayTao = DateTime.Now;
            var clone = Clone(dto);
            _store.DangKyLopHocs.Add(clone);
            return Task.FromResult(Clone(clone));
        }
    }

    public Task<bool> UpdateAsync(DangKyLopHocDto dto)
    {
        lock (_store.SyncLock)
        {
            var item = _store.DangKyLopHocs.FirstOrDefault(x => x.Id == dto.Id);
            if (item == null) return Task.FromResult(false);

            item.GhiChu = dto.GhiChu;
            item.TrangThai = dto.TrangThai;
            return Task.FromResult(true);
        }
    }

    public Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        lock (_store.SyncLock)
        {
            var item = _store.DangKyLopHocs.FirstOrDefault(x => x.Id == id);
            if (item == null) return Task.FromResult(false);

            item.TrangThai = trangThai;
            return Task.FromResult(true);
        }
    }

    public Task<int> CountByLopHocIdAsync(int lopHocId)
    {
        lock (_store.SyncLock)
        {
            var count = _store.DangKyLopHocs.Count(x => x.LopHocId == lopHocId && x.TrangThai == TrangThaiHoatDong.DangHoatDong);
            return Task.FromResult(count);
        }
    }

    public Task<IReadOnlyList<CongNoHocPhiDto>> GetCongNoByLopHocAsync(int lopHocId)
    {
        lock (_store.SyncLock)
        {
            var enrollments = _store.DangKyLopHocs
                .Where(x => x.LopHocId == lopHocId && x.TrangThai == TrangThaiHoatDong.DangHoatDong)
                .ToList();

            var result = enrollments.Select(e =>
            {
                var daThu = _store.PhieuThus
                    .Where(p => p.DangKyLopHocId == e.Id && p.TrangThai == TrangThaiPhieuThu.HieuLuc)
                    .Sum(p => p.SoTien);

                return new CongNoHocPhiDto
                {
                    DangKyLopHocId = e.Id,
                    LopHocId = e.LopHocId,
                    MaLopHoc = e.MaLopHoc,
                    TenLopHoc = e.TenLopHoc,
                    KhoaHocId = e.KhoaHocId,
                    TenKhoaHoc = e.TenKhoaHoc,
                    HocVienId = e.HocVienId,
                    MaHocVien = e.MaHocVien,
                    HoTenHocVien = e.HoTenHocVien,
                    DienThoai = e.DienThoaiHocVien,
                    HocPhiPhaiThu = e.HocPhiPhaiThu,
                    DaThu = daThu
                };
            }).ToList();

            return Task.FromResult<IReadOnlyList<CongNoHocPhiDto>>(result);
        }
    }

    public Task<(bool Success, string? ErrorCode, string? ErrorMessage, DangKyLopHocDto? Data)> EnrollAtomicAsync(
        int lopHocId, int hocVienId, decimal hocPhiPhaiThu, string? ghiChu,
        string? maHocVien = null, string? hoTenHocVien = null, string? dienThoaiHocVien = null)
    {
        lock (_store.SyncLock)
        {
            var lop = _store.LopHocs.FirstOrDefault(l => l.Id == lopHocId);
            if (lop == null) return Task.FromResult<(bool, string?, string?, DangKyLopHocDto?)>((false, ErrorCodes.NOT_FOUND, "Không tìm thấy lớp học.", null));
            if (lop.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return Task.FromResult<(bool, string?, string?, DangKyLopHocDto?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động.", null));

            var activeCount = _store.DangKyLopHocs.Count(x => x.LopHocId == lopHocId && x.TrangThai == TrangThaiHoatDong.DangHoatDong);
            var existing = _store.DangKyLopHocs.FirstOrDefault(x => x.LopHocId == lopHocId && x.HocVienId == hocVienId);

            if (existing != null && existing.TrangThai == TrangThaiHoatDong.DangHoatDong)
            {
                return Task.FromResult<(bool, string?, string?, DangKyLopHocDto?)>((false, ErrorCodes.ALREADY_EXISTS, "Học viên này đã có trong danh sách lớp học.", null));
            }

            if (activeCount >= lop.SiSoToiDa)
            {
                return Task.FromResult<(bool, string?, string?, DangKyLopHocDto?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Lớp học đã đạt sĩ số tối đa ({lop.SiSoToiDa} học viên). Không thể xếp thêm.", null));
            }

            if (existing != null)
            {
                existing.TrangThai = TrangThaiHoatDong.DangHoatDong;
                existing.HocPhiPhaiThu = hocPhiPhaiThu;
                existing.GhiChu = ghiChu;
                existing.NgayDangKy = DateTime.Today;
                existing.KhoaHocId = lop.KhoaHocId;
                existing.TenKhoaHoc = lop.TenKhoaHoc;
                if (!string.IsNullOrWhiteSpace(maHocVien)) existing.MaHocVien = maHocVien;
                if (!string.IsNullOrWhiteSpace(hoTenHocVien)) existing.HoTenHocVien = hoTenHocVien;
                if (!string.IsNullOrWhiteSpace(dienThoaiHocVien)) existing.DienThoaiHocVien = dienThoaiHocVien;
                return Task.FromResult<(bool, string?, string?, DangKyLopHocDto?)>((true, null, null, Clone(existing)));
            }

            var newDk = new DangKyLopHocDto
            {
                Id = _store.NextDangKyId(),
                LopHocId = lopHocId,
                MaLopHoc = lop.MaLopHoc,
                TenLopHoc = lop.TenLopHoc,
                KhoaHocId = lop.KhoaHocId,
                TenKhoaHoc = lop.TenKhoaHoc,
                HocVienId = hocVienId,
                MaHocVien = !string.IsNullOrWhiteSpace(maHocVien) ? maHocVien : $"HV{hocVienId:D3}",
                HoTenHocVien = !string.IsNullOrWhiteSpace(hoTenHocVien) ? hoTenHocVien : $"Học viên {hocVienId}",
                DienThoaiHocVien = dienThoaiHocVien,
                NgayDangKy = DateTime.Today,
                HocPhiPhaiThu = hocPhiPhaiThu,
                DaThu = 0m,
                GhiChu = ghiChu,
                TrangThai = TrangThaiHoatDong.DangHoatDong,
                NgayTao = DateTime.Now
            };
            _store.DangKyLopHocs.Add(newDk);
            return Task.FromResult<(bool, string?, string?, DangKyLopHocDto?)>((true, null, null, Clone(newDk)));
        }
    }

    public Task<(bool Success, string? ErrorCode, string? ErrorMessage)> CancelEnrollmentAtomicAsync(int dangKyLopHocId)
    {
        lock (_store.SyncLock)
        {
            var dk = _store.DangKyLopHocs.FirstOrDefault(x => x.Id == dangKyLopHocId);
            if (dk == null) return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.NOT_FOUND, "Không tìm thấy thông tin đăng ký lớp."));
            if (dk.TrangThai == TrangThaiHoatDong.NgungHoatDong)
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Đăng ký này đã bị hủy trước đó."));

            var totalPaid = _store.PhieuThus
                .Where(p => p.DangKyLopHocId == dangKyLopHocId && p.TrangThai == TrangThaiPhieuThu.HieuLuc)
                .Sum(p => p.SoTien);

            if (totalPaid > 0m)
            {
                return Task.FromResult<(bool, string?, string?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Không thể hủy đăng ký vì học viên đã đóng {totalPaid:N0} đ học phí. Vui lòng hủy các phiếu thu hợp lệ trước."));
            }

            dk.TrangThai = TrangThaiHoatDong.NgungHoatDong;
            return Task.FromResult<(bool, string?, string?)>((true, null, null));
        }
    }

    private static DangKyLopHocDto Clone(DangKyLopHocDto src) => new()
    {
        Id = src.Id,
        LopHocId = src.LopHocId,
        MaLopHoc = src.MaLopHoc,
        TenLopHoc = src.TenLopHoc,
        KhoaHocId = src.KhoaHocId,
        TenKhoaHoc = src.TenKhoaHoc,
        HocVienId = src.HocVienId,
        MaHocVien = src.MaHocVien,
        HoTenHocVien = src.HoTenHocVien,
        DienThoaiHocVien = src.DienThoaiHocVien,
        NgayDangKy = src.NgayDangKy,
        HocPhiPhaiThu = src.HocPhiPhaiThu,
        DaThu = src.DaThu,
        GhiChu = src.GhiChu,
        TrangThai = src.TrangThai,
        NgayTao = src.NgayTao
    };
}
