using QLHV.Contracts.Dtos;
using QLHV.Core.Repositories;

namespace QLHV.Features.Training.Repositories.Fake;

/// <summary>
/// Fake repository cho Kết quả học tập (KetQuaHocTap)
/// </summary>
public class FakeKetQuaHocTapRepository : IKetQuaHocTapRepository
{
    private readonly FakeTrainingStore _store;

    public FakeKetQuaHocTapRepository(FakeTrainingStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<KetQuaHocTapDto>> GetAllAsync(KetQuaHocTapFilter? filter = null)
    {
        lock (_store.SyncLock)
        {
            var query = _store.KetQuaHocTaps.AsEnumerable();

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

                if (filter.HocVienId.HasValue)
                {
                    query = query.Where(x => x.HocVienId == filter.HocVienId.Value);
                }
            }

            var list = query.Select(Clone).OrderBy(x => x.MaHocVien).ToList();
            return Task.FromResult<IReadOnlyList<KetQuaHocTapDto>>(list);
        }
    }

    public Task<KetQuaHocTapDto?> GetByIdAsync(int id)
    {
        lock (_store.SyncLock)
        {
            var item = _store.KetQuaHocTaps.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<KetQuaHocTapDto?> GetByDangKyAndMonAsync(int dangKyLopHocId, int monHocId)
    {
        lock (_store.SyncLock)
        {
            var item = _store.KetQuaHocTaps.FirstOrDefault(x => x.DangKyLopHocId == dangKyLopHocId && x.MonHocId == monHocId);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<KetQuaHocTapDto> UpsertAsync(KetQuaHocTapDto dto)
    {
        lock (_store.SyncLock)
        {
            if (!dto.DiemTongKet.HasValue) return Task.FromResult(dto);

            var existing = _store.KetQuaHocTaps.FirstOrDefault(x => x.DangKyLopHocId == dto.DangKyLopHocId && x.MonHocId == dto.MonHocId);
            if (existing != null)
            {
                existing.DiemTongKet = dto.DiemTongKet;
                existing.NguoiCapNhatTaiKhoanId = dto.NguoiCapNhatTaiKhoanId;
                existing.NguoiCapNhatTen = dto.NguoiCapNhatTen;
                existing.NgayCapNhat = DateTime.Now;
                existing.GhiChu = dto.GhiChu;
                return Task.FromResult(Clone(existing));
            }

            dto.Id = _store.NextKetQuaId();
            dto.NgayCapNhat = DateTime.Now;
            if (dto.KhoaHocId == 0 || string.IsNullOrWhiteSpace(dto.TenKhoaHoc))
            {
                var dk = _store.DangKyLopHocs.FirstOrDefault(d => d.Id == dto.DangKyLopHocId);
                if (dk != null)
                {
                    dto.KhoaHocId = dk.KhoaHocId;
                    dto.TenKhoaHoc = dk.TenKhoaHoc;
                }
            }
            var clone = Clone(dto);
            _store.KetQuaHocTaps.Add(clone);
            return Task.FromResult(Clone(clone));
        }
    }

    public Task<int> UpsertBatchAsync(IEnumerable<KetQuaHocTapDto> list)
    {
        lock (_store.SyncLock)
        {
            int count = 0;
            foreach (var item in list)
            {
                if (!item.DiemTongKet.HasValue) continue;

                var existing = _store.KetQuaHocTaps.FirstOrDefault(x => x.DangKyLopHocId == item.DangKyLopHocId && x.MonHocId == item.MonHocId);
                if (existing != null)
                {
                    existing.DiemTongKet = item.DiemTongKet;
                    existing.NguoiCapNhatTaiKhoanId = item.NguoiCapNhatTaiKhoanId;
                    existing.NguoiCapNhatTen = item.NguoiCapNhatTen;
                    existing.NgayCapNhat = DateTime.Now;
                    existing.GhiChu = item.GhiChu;
                }
                else
                {
                    item.Id = _store.NextKetQuaId();
                    item.NgayCapNhat = DateTime.Now;
                    if (item.KhoaHocId == 0 || string.IsNullOrWhiteSpace(item.TenKhoaHoc))
                    {
                        var dk = _store.DangKyLopHocs.FirstOrDefault(d => d.Id == item.DangKyLopHocId);
                        if (dk != null)
                        {
                            item.KhoaHocId = dk.KhoaHocId;
                            item.TenKhoaHoc = dk.TenKhoaHoc;
                        }
                    }
                    _store.KetQuaHocTaps.Add(Clone(item));
                }
                count++;
            }
            return Task.FromResult(count);
        }
    }

    public Task<bool> DeleteAsync(int id)
    {
        lock (_store.SyncLock)
        {
            var item = _store.KetQuaHocTaps.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                _store.KetQuaHocTaps.Remove(item);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> DeleteByDangKyAndMonAsync(int dangKyLopHocId, int monHocId)
    {
        lock (_store.SyncLock)
        {
            var item = _store.KetQuaHocTaps.FirstOrDefault(x => x.DangKyLopHocId == dangKyLopHocId && x.MonHocId == monHocId);
            if (item != null)
            {
                _store.KetQuaHocTaps.Remove(item);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<int> SaveGradesBatchAtomicAsync(IEnumerable<KetQuaHocTapDto> toUpsert, IEnumerable<int> dangKyIdsToDelete, int monHocId)
    {
        lock (_store.SyncLock)
        {
            int count = 0;
            var delSet = new HashSet<int>(dangKyIdsToDelete);
            if (delSet.Count > 0)
            {
                var toRemove = _store.KetQuaHocTaps.Where(x => x.MonHocId == monHocId && delSet.Contains(x.DangKyLopHocId)).ToList();
                foreach (var item in toRemove)
                {
                    _store.KetQuaHocTaps.Remove(item);
                    count++;
                }
            }

            foreach (var item in toUpsert)
            {
                if (!item.DiemTongKet.HasValue) continue;

                var existing = _store.KetQuaHocTaps.FirstOrDefault(x => x.DangKyLopHocId == item.DangKyLopHocId && x.MonHocId == item.MonHocId);
                if (existing != null)
                {
                    existing.DiemTongKet = item.DiemTongKet;
                    existing.NguoiCapNhatTaiKhoanId = item.NguoiCapNhatTaiKhoanId;
                    existing.NguoiCapNhatTen = item.NguoiCapNhatTen;
                    existing.NgayCapNhat = DateTime.Now;
                    existing.GhiChu = item.GhiChu;
                }
                else
                {
                    item.Id = _store.NextKetQuaId();
                    item.NgayCapNhat = DateTime.Now;
                    if (item.KhoaHocId == 0 || string.IsNullOrWhiteSpace(item.TenKhoaHoc))
                    {
                        var dk = _store.DangKyLopHocs.FirstOrDefault(d => d.Id == item.DangKyLopHocId);
                        if (dk != null)
                        {
                            item.KhoaHocId = dk.KhoaHocId;
                            item.TenKhoaHoc = dk.TenKhoaHoc;
                        }
                    }
                    _store.KetQuaHocTaps.Add(Clone(item));
                }
                count++;
            }

            return Task.FromResult(count);
        }
    }

    private static KetQuaHocTapDto Clone(KetQuaHocTapDto src) => new()
    {
        Id = src.Id,
        DangKyLopHocId = src.DangKyLopHocId,
        LopHocId = src.LopHocId,
        MaLopHoc = src.MaLopHoc,
        TenLopHoc = src.TenLopHoc,
        KhoaHocId = src.KhoaHocId,
        TenKhoaHoc = src.TenKhoaHoc,
        HocVienId = src.HocVienId,
        MaHocVien = src.MaHocVien,
        HoTenHocVien = src.HoTenHocVien,
        MonHocId = src.MonHocId,
        MaMonHoc = src.MaMonHoc,
        TenMonHoc = src.TenMonHoc,
        DiemTongKet = src.DiemTongKet,
        NguoiCapNhatTaiKhoanId = src.NguoiCapNhatTaiKhoanId,
        NguoiCapNhatTen = src.NguoiCapNhatTen,
        NgayCapNhat = src.NgayCapNhat,
        GhiChu = src.GhiChu
    };
}
