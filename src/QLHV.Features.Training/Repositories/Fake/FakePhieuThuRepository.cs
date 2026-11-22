using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Repositories;

namespace QLHV.Features.Training.Repositories.Fake;

/// <summary>
/// Fake repository cho Phiếu thu học phí (PhieuThu) với đồng bộ hóa atomic concurrency
/// </summary>
public class FakePhieuThuRepository : IPhieuThuRepository
{
    private readonly FakeTrainingStore _store;

    public FakePhieuThuRepository(FakeTrainingStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<PhieuThuDto>> GetAllAsync(PhieuThuFilter? filter = null)
    {
        lock (_store.SyncLock)
        {
            var query = _store.PhieuThus.AsEnumerable();

            if (filter != null)
            {
                if (filter.LopHocId.HasValue)
                {
                    query = query.Where(x => x.LopHocId == filter.LopHocId.Value);
                }

                if (filter.DangKyLopHocId.HasValue)
                {
                    query = query.Where(x => x.DangKyLopHocId == filter.DangKyLopHocId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
                {
                    var kw = filter.TuKhoa.Trim();
                    query = query.Where(x => x.MaPhieuThu.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                             x.MaHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                             x.HoTenHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase));
                }

                if (filter.TuNgay.HasValue)
                {
                    query = query.Where(x => x.NgayThu.Date >= filter.TuNgay.Value.Date);
                }

                if (filter.DenNgay.HasValue)
                {
                    query = query.Where(x => x.NgayThu.Date <= filter.DenNgay.Value.Date);
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(x => x.TrangThai == filter.TrangThai.Value);
                }
            }

            var list = query.Select(Clone).OrderByDescending(x => x.Id).ToList();
            return Task.FromResult<IReadOnlyList<PhieuThuDto>>(list);
        }
    }

    public Task<PhieuThuDto?> GetByIdAsync(int id)
    {
        lock (_store.SyncLock)
        {
            var item = _store.PhieuThus.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<PhieuThuDto?> GetByMaPhieuThuAsync(string maPhieuThu)
    {
        lock (_store.SyncLock)
        {
            var item = _store.PhieuThus.FirstOrDefault(x => string.Equals(x.MaPhieuThu, maPhieuThu.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<PhieuThuDto> AddAsync(PhieuThuDto dto)
    {
        lock (_store.SyncLock)
        {
            if (dto.KhoaHocId == 0 || string.IsNullOrWhiteSpace(dto.TenKhoaHoc))
            {
                var dk = _store.DangKyLopHocs.FirstOrDefault(x => x.Id == dto.DangKyLopHocId);
                if (dk != null)
                {
                    dto.KhoaHocId = dk.KhoaHocId;
                    dto.TenKhoaHoc = dk.TenKhoaHoc;
                }
            }

            dto.Id = _store.NextPhieuThuId();
            dto.NgayThu = DateTime.Now;
            var clone = Clone(dto);
            _store.PhieuThus.Add(clone);
            return Task.FromResult(Clone(clone));
        }
    }

    public Task<bool> CancelAsync(int id, string lyDoHuy, int? nguoiHuyTaiKhoanId = null)
    {
        lock (_store.SyncLock)
        {
            var item = _store.PhieuThus.FirstOrDefault(x => x.Id == id);
            if (item == null || item.TrangThai != TrangThaiPhieuThu.HieuLuc) return Task.FromResult(false);

            item.TrangThai = TrangThaiPhieuThu.DaHuy;
            item.LyDoHuy = lyDoHuy;
            item.NgayHuy = DateTime.Now;
            item.NguoiHuyTaiKhoanId = nguoiHuyTaiKhoanId;
            return Task.FromResult(true);
        }
    }

    public Task<decimal> GetTotalPaidByDangKyIdAsync(int dangKyLopHocId)
    {
        lock (_store.SyncLock)
        {
            var total = _store.PhieuThus
                .Where(x => x.DangKyLopHocId == dangKyLopHocId && x.TrangThai == TrangThaiPhieuThu.HieuLuc)
                .Sum(x => x.SoTien);

            return Task.FromResult(total);
        }
    }

    public Task<(bool Success, string? ErrorCode, string? ErrorMessage, PhieuThuDto? Data)> ThuHocPhiAtomicAsync(
        int dangKyLopHocId, decimal soTien, HinhThucThanhToan hinhThuc, int nguoiThuTaiKhoanId, string? ghiChu)
    {
        lock (_store.SyncLock)
        {
            var dk = _store.DangKyLopHocs.FirstOrDefault(x => x.Id == dangKyLopHocId);
            if (dk == null)
                return Task.FromResult<(bool, string?, string?, PhieuThuDto?)>((false, ErrorCodes.NOT_FOUND, "Không tìm thấy thông tin đăng ký lớp học.", null));

            if (dk.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return Task.FromResult<(bool, string?, string?, PhieuThuDto?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể thu học phí vì đăng ký này đã bị hủy hoặc ngừng hoạt động.", null));

            var daThu = _store.PhieuThus
                .Where(p => p.DangKyLopHocId == dangKyLopHocId && p.TrangThai == TrangThaiPhieuThu.HieuLuc)
                .Sum(p => p.SoTien);

            var conNo = Math.Max(0m, dk.HocPhiPhaiThu - daThu);
            if (conNo <= 0m)
                return Task.FromResult<(bool, string?, string?, PhieuThuDto?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION, "Học viên này đã nộp đủ học phí, không còn công nợ.", null));

            if (soTien > conNo)
                return Task.FromResult<(bool, string?, string?, PhieuThuDto?)>((false, ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Số tiền thu ({soTien:N0} đ) vượt quá số tiền còn nợ ({conNo:N0} đ).", null));

            var maPhieuThu = $"PT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
            var newPt = new PhieuThuDto
            {
                Id = _store.NextPhieuThuId(),
                MaPhieuThu = maPhieuThu,
                DangKyLopHocId = dangKyLopHocId,
                LopHocId = dk.LopHocId,
                MaLopHoc = dk.MaLopHoc,
                TenLopHoc = dk.TenLopHoc,
                KhoaHocId = dk.KhoaHocId,
                TenKhoaHoc = dk.TenKhoaHoc,
                HocVienId = dk.HocVienId,
                MaHocVien = dk.MaHocVien,
                HoTenHocVien = dk.HoTenHocVien,
                NgayThu = DateTime.Now,
                SoTien = soTien,
                HinhThuc = hinhThuc,
                NguoiThuTaiKhoanId = nguoiThuTaiKhoanId,
                NguoiThuTen = "admin",
                GhiChu = ghiChu,
                TrangThai = TrangThaiPhieuThu.HieuLuc
            };

            _store.PhieuThus.Add(newPt);
            return Task.FromResult<(bool, string?, string?, PhieuThuDto?)>((true, null, null, Clone(newPt)));
        }
    }

    private static PhieuThuDto Clone(PhieuThuDto src) => new()
    {
        Id = src.Id,
        MaPhieuThu = src.MaPhieuThu,
        DangKyLopHocId = src.DangKyLopHocId,
        LopHocId = src.LopHocId,
        MaLopHoc = src.MaLopHoc,
        TenLopHoc = src.TenLopHoc,
        KhoaHocId = src.KhoaHocId,
        TenKhoaHoc = src.TenKhoaHoc,
        HocVienId = src.HocVienId,
        MaHocVien = src.MaHocVien,
        HoTenHocVien = src.HoTenHocVien,
        NgayThu = src.NgayThu,
        SoTien = src.SoTien,
        HinhThuc = src.HinhThuc,
        NguoiThuTaiKhoanId = src.NguoiThuTaiKhoanId,
        NguoiThuTen = src.NguoiThuTen,
        GhiChu = src.GhiChu,
        TrangThai = src.TrangThai,
        LyDoHuy = src.LyDoHuy,
        NgayHuy = src.NgayHuy,
        NguoiHuyTaiKhoanId = src.NguoiHuyTaiKhoanId,
        NguoiHuyTen = src.NguoiHuyTen
    };
}
