using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Repositories;

namespace QLHV.Features.Training.Repositories.Fake;

/// <summary>
/// Fake repository cho Báo cáo thống kê
/// </summary>
public class FakeBaoCaoRepository : IBaoCaoRepository
{
    private readonly FakeTrainingStore _store;
    private readonly IKhoaHocMonHocRepository? _khoaHocMonHocRepository;
    private readonly IMonHocRepository? _monHocRepository;
    private readonly IHocVienRepository? _hocVienRepository;
    private readonly IKhoaHocRepository? _khoaHocRepository;

    public FakeBaoCaoRepository(
        FakeTrainingStore store,
        IKhoaHocMonHocRepository? khoaHocMonHocRepository = null,
        IMonHocRepository? monHocRepository = null,
        IHocVienRepository? hocVienRepository = null,
        IKhoaHocRepository? khoaHocRepository = null)
    {
        _store = store;
        _khoaHocMonHocRepository = khoaHocMonHocRepository;
        _monHocRepository = monHocRepository;
        _hocVienRepository = hocVienRepository;
        _khoaHocRepository = khoaHocRepository;
    }

    public async Task<IReadOnlyList<BaoCaoHocVienTheoLopDto>> GetBaoCaoHocVienTheoLopAsync(int lopHocId)
    {
        List<DangKyLopHocDto> enrollments;
        List<PhieuThuDto> phieuThus;
        LopHocDto? lop;

        lock (_store.SyncLock)
        {
            enrollments = _store.DangKyLopHocs
                .Where(x => x.LopHocId == lopHocId && x.TrangThai == TrangThaiHoatDong.DangHoatDong)
                .ToList();

            phieuThus = _store.PhieuThus
                .Where(p => p.TrangThai == TrangThaiPhieuThu.HieuLuc)
                .ToList();

            lop = _store.LopHocs.FirstOrDefault(l => l.Id == lopHocId);
        }

        string tenKhoaHoc = string.Empty;
        if (lop != null && _khoaHocRepository != null && lop.KhoaHocId > 0)
        {
            var kh = await _khoaHocRepository.GetByIdAsync(lop.KhoaHocId);
            if (kh != null && !string.IsNullOrWhiteSpace(kh.TenKhoaHoc))
            {
                tenKhoaHoc = kh.TenKhoaHoc;
            }
        }
        else if (lop != null && !string.IsNullOrWhiteSpace(lop.TenKhoaHoc))
        {
            tenKhoaHoc = lop.TenKhoaHoc;
        }

        var list = new List<BaoCaoHocVienTheoLopDto>();
        foreach (var e in enrollments)
        {
            var daThu = phieuThus
                .Where(p => p.DangKyLopHocId == e.Id)
                .Sum(p => p.SoTien);

            DateTime? ngaySinh = null;
            string dienThoai = e.DienThoaiHocVien ?? string.Empty;
            string email = string.Empty;

            if (_hocVienRepository != null && e.HocVienId > 0)
            {
                var hv = await _hocVienRepository.GetByIdAsync(e.HocVienId);
                if (hv != null)
                {
                    ngaySinh = hv.NgaySinh;
                    if (!string.IsNullOrWhiteSpace(hv.DienThoai)) dienThoai = hv.DienThoai;
                    if (!string.IsNullOrWhiteSpace(hv.Email)) email = hv.Email;
                }
            }

            list.Add(new BaoCaoHocVienTheoLopDto
            {
                MaLopHoc = e.MaLopHoc,
                TenLopHoc = e.TenLopHoc,
                TenKhoaHoc = tenKhoaHoc,
                MaHocVien = e.MaHocVien,
                HoTenHocVien = e.HoTenHocVien,
                NgaySinh = ngaySinh,
                DienThoai = dienThoai,
                Email = email,
                NgayDangKy = e.NgayDangKy,
                HocPhiPhaiThu = e.HocPhiPhaiThu,
                DaThu = daThu
            });
        }

        return list.OrderBy(x => x.MaHocVien).ToList();
    }

    public Task<IReadOnlyList<BaoCaoLichHocTheoLopDto>> GetBaoCaoLichHocTheoLopAsync(int lopHocId)
    {
        lock (_store.SyncLock)
        {
            var schedules = _store.LichHocs
                .Where(x => x.LopHocId == lopHocId && x.TrangThai == TrangThaiHoatDong.DangHoatDong)
                .OrderBy(x => x.NgayHoc)
                .ThenBy(x => x.GioBatDau)
                .Select(s => new BaoCaoLichHocTheoLopDto
                {
                    MaLopHoc = s.MaLopHoc,
                    TenLopHoc = s.TenLopHoc,
                    MaMonHoc = s.MaMonHoc,
                    TenMonHoc = s.TenMonHoc,
                    HoTenGiangVien = s.HoTenGiangVien,
                    TenPhong = s.TenPhong,
                    NgayHoc = s.NgayHoc,
                    GioBatDau = s.GioBatDau,
                    GioKetThuc = s.GioKetThuc,
                    NoiDung = s.NoiDung
                }).ToList();

            return Task.FromResult<IReadOnlyList<BaoCaoLichHocTheoLopDto>>(schedules);
        }
    }

    public Task<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>> GetBaoCaoGiangDayTheoGiangVienAsync(int giangVienId, DateTime? tuNgay = null, DateTime? denNgay = null)
    {
        lock (_store.SyncLock)
        {
            var query = _store.LichHocs
                .Where(x => x.GiangVienId == giangVienId && x.TrangThai == TrangThaiHoatDong.DangHoatDong);

            if (tuNgay.HasValue)
                query = query.Where(x => x.NgayHoc.Date >= tuNgay.Value.Date);

            if (denNgay.HasValue)
                query = query.Where(x => x.NgayHoc.Date <= denNgay.Value.Date);

            var list = query
                .OrderBy(x => x.NgayHoc)
                .ThenBy(x => x.GioBatDau)
                .Select(s => new BaoCaoGiangDayTheoGiangVienDto
                {
                    MaGiangVien = s.MaGiangVien,
                    HoTenGiangVien = s.HoTenGiangVien,
                    MaLopHoc = s.MaLopHoc,
                    TenLopHoc = s.TenLopHoc,
                    TenMonHoc = s.TenMonHoc,
                    NgayHoc = s.NgayHoc,
                    GioBatDau = s.GioBatDau,
                    GioKetThuc = s.GioKetThuc,
                    TenPhong = s.TenPhong
                }).ToList();

            return Task.FromResult<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>>(list);
        }
    }

    public async Task<IReadOnlyList<BaoCaoKetQuaHocTapDto>> GetBaoCaoKetQuaHocTapAsync(int? lopHocId, int? monHocId)
    {
        List<DangKyLopHocDto> enrollments;
        List<KetQuaHocTapDto> allGrades;
        List<LopHocDto> allLops;

        lock (_store.SyncLock)
        {
            var enQuery = _store.DangKyLopHocs.Where(x => x.TrangThai == TrangThaiHoatDong.DangHoatDong);
            if (lopHocId.HasValue)
                enQuery = enQuery.Where(x => x.LopHocId == lopHocId.Value);

            enrollments = enQuery.ToList();
            allGrades = _store.KetQuaHocTaps.ToList();
            allLops = _store.LopHocs.ToList();
        }

        var list = new List<BaoCaoKetQuaHocTapDto>();

        foreach (var dk in enrollments)
        {
            var lop = allLops.FirstOrDefault(l => l.Id == dk.LopHocId);
            int khoaHocId = lop?.KhoaHocId ?? 0;

            var courseSubjects = new List<(int MonHocId, string MaMonHoc, string TenMonHoc)>();
            if (_khoaHocMonHocRepository != null && khoaHocId > 0)
            {
                var kmList = await _khoaHocMonHocRepository.GetByKhoaHocIdAsync(khoaHocId);
                foreach (var km in kmList)
                {
                    string ma = km.MaMonHoc ?? string.Empty;
                    string ten = km.TenMonHoc ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(ma) && _monHocRepository != null)
                    {
                        var m = await _monHocRepository.GetByIdAsync(km.MonHocId);
                        if (m != null)
                        {
                            ma = m.MaMonHoc ?? string.Empty;
                            ten = m.TenMonHoc ?? string.Empty;
                        }
                    }
                    courseSubjects.Add((km.MonHocId, ma, ten));
                }
            }

            if (courseSubjects.Count == 0)
            {
                var knownGrades = allGrades.Where(k => k.DangKyLopHocId == dk.Id).ToList();
                if (knownGrades.Count > 0)
                {
                    courseSubjects.AddRange(knownGrades.Select(g => (g.MonHocId, g.MaMonHoc, g.TenMonHoc)).Distinct());
                }
            }

            if (monHocId.HasValue)
            {
                courseSubjects = courseSubjects.Where(s => s.MonHocId == monHocId.Value).ToList();
            }

            foreach (var subj in courseSubjects)
            {
                var grade = allGrades.FirstOrDefault(g => g.DangKyLopHocId == dk.Id && g.MonHocId == subj.MonHocId);
                list.Add(new BaoCaoKetQuaHocTapDto
                {
                    MaLopHoc = dk.MaLopHoc,
                    TenLopHoc = dk.TenLopHoc,
                    MaMonHoc = subj.MaMonHoc,
                    TenMonHoc = subj.TenMonHoc,
                    MaHocVien = dk.MaHocVien,
                    HoTenHocVien = dk.HoTenHocVien,
                    DiemTongKet = grade?.DiemTongKet,
                    GhiChu = grade?.GhiChu
                });
            }
        }

        return list.OrderBy(x => x.MaLopHoc).ThenBy(x => x.MaMonHoc).ThenBy(x => x.MaHocVien).ToList();
    }

    public async Task<IReadOnlyList<ThongKeTyLeMonHocDto>> GetThongKeTyLeMonHocAsync(int? lopHocId)
    {
        var rawReport = await GetBaoCaoKetQuaHocTapAsync(lopHocId, null);
        var groups = rawReport.GroupBy(x => new { x.MaMonHoc, x.TenMonHoc }).ToList();

        var result = groups.Select(g =>
        {
            int tong = g.Count();
            int daCham = g.Count(x => x.DiemTongKet.HasValue);
            int chuaNhap = g.Count(x => !x.DiemTongKet.HasValue);
            int dat = g.Count(x => x.DiemTongKet.HasValue && x.DiemTongKet.Value >= 5.00m);

            return new ThongKeTyLeMonHocDto
            {
                MaMonHoc = g.Key.MaMonHoc,
                TenMonHoc = g.Key.TenMonHoc,
                TongSoHocVien = tong,
                SoLuongDaCham = daCham,
                SoLuongChuaNhap = chuaNhap,
                SoLuongDat = dat
            };
        }).OrderBy(x => x.MaMonHoc).ToList();

        return result;
    }

    public Task<IReadOnlyList<BaoCaoCongNoHocPhiDto>> GetBaoCaoCongNoHocPhiAsync(int? lopHocId = null)
    {
        lock (_store.SyncLock)
        {
            var lops = _store.LopHocs.AsEnumerable();
            if (lopHocId.HasValue)
                lops = lops.Where(x => x.Id == lopHocId.Value);

            var list = lops.Select(l =>
            {
                var enrollments = _store.DangKyLopHocs
                    .Where(dk => dk.LopHocId == l.Id && dk.TrangThai == TrangThaiHoatDong.DangHoatDong)
                    .ToList();

                var tongPhaiThu = enrollments.Sum(x => x.HocPhiPhaiThu);
                var activeDkIds = enrollments.Select(x => x.Id).ToHashSet();

                var tongDaThu = _store.PhieuThus
                    .Where(pt => activeDkIds.Contains(pt.DangKyLopHocId) && pt.TrangThai == TrangThaiPhieuThu.HieuLuc)
                    .Sum(pt => pt.SoTien);

                return new BaoCaoCongNoHocPhiDto
                {
                    MaLopHoc = l.MaLopHoc,
                    TenLopHoc = l.TenLopHoc,
                    TongHocVien = enrollments.Count,
                    TongHocPhiPhaiThu = tongPhaiThu,
                    TongDaThu = tongDaThu
                };
            }).OrderBy(x => x.MaLopHoc).ToList();

            return Task.FromResult<IReadOnlyList<BaoCaoCongNoHocPhiDto>>(list);
        }
    }
}
