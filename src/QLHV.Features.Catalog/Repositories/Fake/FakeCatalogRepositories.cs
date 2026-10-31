using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Repositories;
using QLHV.Core.Validation;

namespace QLHV.Features.Catalog.Repositories.Fake;

/// <summary>
/// Fake/In-memory repository cho Học viên
/// </summary>
public class FakeHocVienRepository : IHocVienRepository
{
    private readonly List<HocVienDto> _data = new();
    private int _autoId = 0;
    private readonly object _lock = new();

    public FakeHocVienRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        AddInternal(new HocVienDto
        {
            MaHocVien = "HV001",
            HoTen = "Nguyễn Văn An",
            NgaySinh = new DateTime(2003, 5, 12),
            GioiTinh = "Nam",
            DienThoai = "0981234567",
            Email = "an.nguyen@example.com",
            DiaChi = "Hà Nội",
            NgayDangKy = DateTime.Today.AddMonths(-3),
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new HocVienDto
        {
            MaHocVien = "HV002",
            HoTen = "Trần Thị Bình",
            NgaySinh = new DateTime(2004, 8, 20),
            GioiTinh = "Nữ",
            DienThoai = "0912345678",
            Email = "binh.tran@example.com",
            DiaChi = "Đà Nẵng",
            NgayDangKy = DateTime.Today.AddMonths(-2),
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new HocVienDto
        {
            MaHocVien = "HV003",
            HoTen = "Lê Hoàng Cường",
            NgaySinh = new DateTime(2002, 11, 5),
            GioiTinh = "Nam",
            DienThoai = "0903456789",
            Email = "cuong.le@example.com",
            DiaChi = "TP. Hồ Chí Minh",
            NgayDangKy = DateTime.Today.AddMonths(-1),
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });
    }

    private HocVienDto AddInternal(HocVienDto dto)
    {
        dto.Id = ++_autoId;
        _data.Add(Clone(dto));
        return dto;
    }

    public Task<IReadOnlyList<HocVienDto>> GetAllAsync(HocVienFilter? filter = null)
    {
        lock (_lock)
        {
            IEnumerable<HocVienDto> query = _data;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
                {
                    var keyword = filter.TuKhoa.Trim();
                    query = query.Where(h =>
                        ValidationHelper.ContainsSearch(h.MaHocVien, keyword) ||
                        ValidationHelper.ContainsSearch(h.HoTen, keyword) ||
                        ValidationHelper.ContainsSearch(h.DienThoai, keyword) ||
                        ValidationHelper.ContainsSearch(h.Email, keyword));
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(h => h.TrangThai == filter.TrangThai.Value);
                }
            }

            IReadOnlyList<HocVienDto> result = query.Select(Clone).ToList();
            return Task.FromResult(result);
        }
    }

    public Task<HocVienDto?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<HocVienDto?> GetByMaHocVienAsync(string maHocVien)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => string.Equals(x.MaHocVien, maHocVien?.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<HocVienDto> AddAsync(HocVienDto dto)
    {
        lock (_lock)
        {
            var copy = Clone(dto);
            copy.Id = ++_autoId;
            _data.Add(copy);
            return Task.FromResult(Clone(copy));
        }
    }

    public Task<bool> UpdateAsync(HocVienDto dto)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == dto.Id);
            if (existing == null) return Task.FromResult(false);

            existing.MaHocVien = dto.MaHocVien;
            existing.HoTen = dto.HoTen;
            existing.NgaySinh = dto.NgaySinh;
            existing.GioiTinh = dto.GioiTinh;
            existing.DienThoai = dto.DienThoai;
            existing.Email = dto.Email;
            existing.DiaChi = dto.DiaChi;
            existing.NgayDangKy = dto.NgayDangKy;
            existing.TrangThai = dto.TrangThai;

            return Task.FromResult(true);
        }
    }

    public Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.TrangThai = trangThai;
            return Task.FromResult(true);
        }
    }

    private static HocVienDto Clone(HocVienDto src)
    {
        return new HocVienDto
        {
            Id = src.Id,
            MaHocVien = src.MaHocVien,
            HoTen = src.HoTen,
            NgaySinh = src.NgaySinh,
            GioiTinh = src.GioiTinh,
            DienThoai = src.DienThoai,
            Email = src.Email,
            DiaChi = src.DiaChi,
            NgayDangKy = src.NgayDangKy,
            TrangThai = src.TrangThai
        };
    }
}

/// <summary>
/// Fake/In-memory repository cho Giảng viên
/// </summary>
public class FakeGiangVienRepository : IGiangVienRepository
{
    private readonly List<GiangVienDto> _data = new();
    private int _autoId = 0;
    private readonly object _lock = new();

    public FakeGiangVienRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        AddInternal(new GiangVienDto
        {
            MaGiangVien = "GV001",
            HoTen = "TS. Phạm Văn Dũng",
            ChuyenMon = "Lập trình C#, WinForms & .NET",
            DienThoai = "0971112233",
            Email = "dung.pham@example.com",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new GiangVienDto
        {
            MaGiangVien = "GV002",
            HoTen = "ThS. Đỗ Thị Hương",
            ChuyenMon = "Cơ sở dữ liệu & SQL Server",
            DienThoai = "0972223344",
            Email = "huong.do@example.com",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new GiangVienDto
        {
            MaGiangVien = "GV003",
            HoTen = "ThS. Ngô Quang Hải",
            ChuyenMon = "Lập trình Web & Kiến trúc phần mềm",
            DienThoai = "0973334455",
            Email = "hai.ngo@example.com",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });
    }

    private GiangVienDto AddInternal(GiangVienDto dto)
    {
        dto.Id = ++_autoId;
        _data.Add(Clone(dto));
        return dto;
    }

    public Task<IReadOnlyList<GiangVienDto>> GetAllAsync(GiangVienFilter? filter = null)
    {
        lock (_lock)
        {
            IEnumerable<GiangVienDto> query = _data;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
                {
                    var keyword = filter.TuKhoa.Trim();
                    query = query.Where(g =>
                        ValidationHelper.ContainsSearch(g.MaGiangVien, keyword) ||
                        ValidationHelper.ContainsSearch(g.HoTen, keyword) ||
                        ValidationHelper.ContainsSearch(g.ChuyenMon, keyword) ||
                        ValidationHelper.ContainsSearch(g.Email, keyword) ||
                        ValidationHelper.ContainsSearch(g.DienThoai, keyword));
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(g => g.TrangThai == filter.TrangThai.Value);
                }
            }

            IReadOnlyList<GiangVienDto> result = query.Select(Clone).ToList();
            return Task.FromResult(result);
        }
    }

    public Task<GiangVienDto?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<GiangVienDto?> GetByMaGiangVienAsync(string maGiangVien)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => string.Equals(x.MaGiangVien, maGiangVien?.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<GiangVienDto> AddAsync(GiangVienDto dto)
    {
        lock (_lock)
        {
            var copy = Clone(dto);
            copy.Id = ++_autoId;
            _data.Add(copy);
            return Task.FromResult(Clone(copy));
        }
    }

    public Task<bool> UpdateAsync(GiangVienDto dto)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == dto.Id);
            if (existing == null) return Task.FromResult(false);

            existing.MaGiangVien = dto.MaGiangVien;
            existing.HoTen = dto.HoTen;
            existing.ChuyenMon = dto.ChuyenMon;
            existing.DienThoai = dto.DienThoai;
            existing.Email = dto.Email;
            existing.TrangThai = dto.TrangThai;

            return Task.FromResult(true);
        }
    }

    public Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.TrangThai = trangThai;
            return Task.FromResult(true);
        }
    }

    private static GiangVienDto Clone(GiangVienDto src)
    {
        return new GiangVienDto
        {
            Id = src.Id,
            MaGiangVien = src.MaGiangVien,
            HoTen = src.HoTen,
            ChuyenMon = src.ChuyenMon,
            DienThoai = src.DienThoai,
            Email = src.Email,
            TrangThai = src.TrangThai
        };
    }
}

/// <summary>
/// Fake/In-memory repository cho Môn học
/// </summary>
public class FakeMonHocRepository : IMonHocRepository
{
    private readonly List<MonHocDto> _data = new();
    private int _autoId = 0;
    private readonly object _lock = new();

    public FakeMonHocRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        AddInternal(new MonHocDto
        {
            MaMonHoc = "MH01",
            TenMonHoc = "Lập trình C# cơ bản",
            SoBuoiMacDinh = 15,
            MoTa = "Nhập môn cú pháp C#, OOP và các cấu trúc dữ liệu cơ bản",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new MonHocDto
        {
            MaMonHoc = "MH02",
            TenMonHoc = "Lập trình Windows Forms",
            SoBuoiMacDinh = 20,
            MoTa = "Thiết kế giao diện Windows Forms, controls, validation, ADO.NET",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new MonHocDto
        {
            MaMonHoc = "MH03",
            TenMonHoc = "Quản trị & Lập trình SQL Server",
            SoBuoiMacDinh = 18,
            MoTa = "Thiết kế CSDL quan hệ, T-SQL, Stored Procedures, Triggers, Views",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });
    }

    private MonHocDto AddInternal(MonHocDto dto)
    {
        dto.Id = ++_autoId;
        _data.Add(Clone(dto));
        return dto;
    }

    public Task<IReadOnlyList<MonHocDto>> GetAllAsync(MonHocFilter? filter = null)
    {
        lock (_lock)
        {
            IEnumerable<MonHocDto> query = _data;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
                {
                    var keyword = filter.TuKhoa.Trim();
                    query = query.Where(m =>
                        ValidationHelper.ContainsSearch(m.MaMonHoc, keyword) ||
                        ValidationHelper.ContainsSearch(m.TenMonHoc, keyword) ||
                        ValidationHelper.ContainsSearch(m.MoTa, keyword));
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(m => m.TrangThai == filter.TrangThai.Value);
                }
            }

            IReadOnlyList<MonHocDto> result = query.Select(Clone).ToList();
            return Task.FromResult(result);
        }
    }

    public Task<MonHocDto?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<MonHocDto?> GetByMaMonHocAsync(string maMonHoc)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => string.Equals(x.MaMonHoc, maMonHoc?.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<MonHocDto> AddAsync(MonHocDto dto)
    {
        lock (_lock)
        {
            var copy = Clone(dto);
            copy.Id = ++_autoId;
            _data.Add(copy);
            return Task.FromResult(Clone(copy));
        }
    }

    public Task<bool> UpdateAsync(MonHocDto dto)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == dto.Id);
            if (existing == null) return Task.FromResult(false);

            existing.MaMonHoc = dto.MaMonHoc;
            existing.TenMonHoc = dto.TenMonHoc;
            existing.SoBuoiMacDinh = dto.SoBuoiMacDinh;
            existing.MoTa = dto.MoTa;
            existing.TrangThai = dto.TrangThai;

            return Task.FromResult(true);
        }
    }

    public Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.TrangThai = trangThai;
            return Task.FromResult(true);
        }
    }

    private static MonHocDto Clone(MonHocDto src)
    {
        return new MonHocDto
        {
            Id = src.Id,
            MaMonHoc = src.MaMonHoc,
            TenMonHoc = src.TenMonHoc,
            SoBuoiMacDinh = src.SoBuoiMacDinh,
            MoTa = src.MoTa,
            TrangThai = src.TrangThai
        };
    }
}

/// <summary>
/// Fake/In-memory repository cho Khóa học
/// </summary>
public class FakeKhoaHocRepository : IKhoaHocRepository
{
    private readonly List<KhoaHocDto> _data = new();
    private int _autoId = 0;
    private readonly object _lock = new();

    public FakeKhoaHocRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        AddInternal(new KhoaHocDto
        {
            MaKhoaHoc = "KH01",
            TenKhoaHoc = "Chuyên viên Lập trình .NET Toàn diện",
            HocPhiNiemYet = 8500000m,
            MoTa = "Khóa đào tạo chuyên sâu từ C# cơ bản, WinForms đến SQL Server",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new KhoaHocDto
        {
            MaKhoaHoc = "KH02",
            TenKhoaHoc = "Lập trình C# WinForms Cơ bản & Nâng cao",
            HocPhiNiemYet = 5500000m,
            MoTa = "Thành thạo xây dựng ứng dụng máy tính trên nền tảng Windows",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new KhoaHocDto
        {
            MaKhoaHoc = "KH03",
            TenKhoaHoc = "Quản trị CSDL & T-SQL Doanh nghiệp",
            HocPhiNiemYet = 4500000m,
            MoTa = "Thiết kế, tối ưu hóa và quản trị cơ sở dữ liệu SQL Server",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });
    }

    private KhoaHocDto AddInternal(KhoaHocDto dto)
    {
        dto.Id = ++_autoId;
        _data.Add(Clone(dto));
        return dto;
    }

    public Task<IReadOnlyList<KhoaHocDto>> GetAllAsync(KhoaHocFilter? filter = null)
    {
        lock (_lock)
        {
            IEnumerable<KhoaHocDto> query = _data;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
                {
                    var keyword = filter.TuKhoa.Trim();
                    query = query.Where(k =>
                        ValidationHelper.ContainsSearch(k.MaKhoaHoc, keyword) ||
                        ValidationHelper.ContainsSearch(k.TenKhoaHoc, keyword) ||
                        ValidationHelper.ContainsSearch(k.MoTa, keyword));
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(k => k.TrangThai == filter.TrangThai.Value);
                }
            }

            IReadOnlyList<KhoaHocDto> result = query.Select(Clone).ToList();
            return Task.FromResult(result);
        }
    }

    public Task<KhoaHocDto?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<KhoaHocDto?> GetByMaKhoaHocAsync(string maKhoaHoc)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => string.Equals(x.MaKhoaHoc, maKhoaHoc?.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<KhoaHocDto> AddAsync(KhoaHocDto dto)
    {
        lock (_lock)
        {
            var copy = Clone(dto);
            copy.Id = ++_autoId;
            _data.Add(copy);
            return Task.FromResult(Clone(copy));
        }
    }

    public Task<bool> UpdateAsync(KhoaHocDto dto)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == dto.Id);
            if (existing == null) return Task.FromResult(false);

            existing.MaKhoaHoc = dto.MaKhoaHoc;
            existing.TenKhoaHoc = dto.TenKhoaHoc;
            existing.HocPhiNiemYet = dto.HocPhiNiemYet;
            existing.MoTa = dto.MoTa;
            existing.TrangThai = dto.TrangThai;

            return Task.FromResult(true);
        }
    }

    public Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.TrangThai = trangThai;
            return Task.FromResult(true);
        }
    }

    private static KhoaHocDto Clone(KhoaHocDto src)
    {
        return new KhoaHocDto
        {
            Id = src.Id,
            MaKhoaHoc = src.MaKhoaHoc,
            TenKhoaHoc = src.TenKhoaHoc,
            HocPhiNiemYet = src.HocPhiNiemYet,
            MoTa = src.MoTa,
            TrangThai = src.TrangThai
        };
    }
}

/// <summary>
/// Fake/In-memory repository cho quan hệ Khóa học - Môn học
/// </summary>
public class FakeKhoaHocMonHocRepository : IKhoaHocMonHocRepository
{
    private readonly List<KhoaHocMonHocDto> _data = new();
    private readonly object _lock = new();

    public FakeKhoaHocMonHocRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        // Khóa học 1 (.NET Toàn diện) gồm môn 1, 2, 3
        _data.Add(new KhoaHocMonHocDto { KhoaHocId = 1, MonHocId = 1, ThuTu = 1, SoBuoiDuKien = 15, MaKhoaHoc = "KH01", TenKhoaHoc = "Chuyên viên Lập trình .NET Toàn diện", MaMonHoc = "MH01", TenMonHoc = "Lập trình C# cơ bản" });
        _data.Add(new KhoaHocMonHocDto { KhoaHocId = 1, MonHocId = 2, ThuTu = 2, SoBuoiDuKien = 20, MaKhoaHoc = "KH01", TenKhoaHoc = "Chuyên viên Lập trình .NET Toàn diện", MaMonHoc = "MH02", TenMonHoc = "Lập trình Windows Forms" });
        _data.Add(new KhoaHocMonHocDto { KhoaHocId = 1, MonHocId = 3, ThuTu = 3, SoBuoiDuKien = 18, MaKhoaHoc = "KH01", TenKhoaHoc = "Chuyên viên Lập trình .NET Toàn diện", MaMonHoc = "MH03", TenMonHoc = "Quản trị & Lập trình SQL Server" });

        // Khóa học 2 (WinForms) gồm môn 1, 2
        _data.Add(new KhoaHocMonHocDto { KhoaHocId = 2, MonHocId = 1, ThuTu = 1, SoBuoiDuKien = 15, MaKhoaHoc = "KH02", TenKhoaHoc = "Lập trình C# WinForms Cơ bản & Nâng cao", MaMonHoc = "MH01", TenMonHoc = "Lập trình C# cơ bản" });
        _data.Add(new KhoaHocMonHocDto { KhoaHocId = 2, MonHocId = 2, ThuTu = 2, SoBuoiDuKien = 20, MaKhoaHoc = "KH02", TenKhoaHoc = "Lập trình C# WinForms Cơ bản & Nâng cao", MaMonHoc = "MH02", TenMonHoc = "Lập trình Windows Forms" });
    }

    public Task<IReadOnlyList<KhoaHocMonHocDto>> GetByKhoaHocIdAsync(int khoaHocId)
    {
        lock (_lock)
        {
            IReadOnlyList<KhoaHocMonHocDto> result = _data
                .Where(x => x.KhoaHocId == khoaHocId)
                .OrderBy(x => x.ThuTu)
                .Select(Clone)
                .ToList();
            return Task.FromResult(result);
        }
    }

    public Task<KhoaHocMonHocDto?> GetAsync(int khoaHocId, int monHocId)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => x.KhoaHocId == khoaHocId && x.MonHocId == monHocId);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<KhoaHocMonHocDto> AddAsync(KhoaHocMonHocDto dto)
    {
        lock (_lock)
        {
            var copy = Clone(dto);
            _data.Add(copy);
            return Task.FromResult(Clone(copy));
        }
    }

    public Task<bool> UpdateAsync(KhoaHocMonHocDto dto)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.KhoaHocId == dto.KhoaHocId && x.MonHocId == dto.MonHocId);
            if (existing == null) return Task.FromResult(false);

            existing.ThuTu = dto.ThuTu;
            existing.SoBuoiDuKien = dto.SoBuoiDuKien;
            if (!string.IsNullOrEmpty(dto.MaMonHoc)) existing.MaMonHoc = dto.MaMonHoc;
            if (!string.IsNullOrEmpty(dto.TenMonHoc)) existing.TenMonHoc = dto.TenMonHoc;
            if (!string.IsNullOrEmpty(dto.MaKhoaHoc)) existing.MaKhoaHoc = dto.MaKhoaHoc;
            if (!string.IsNullOrEmpty(dto.TenKhoaHoc)) existing.TenKhoaHoc = dto.TenKhoaHoc;

            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteAsync(int khoaHocId, int monHocId)
    {
        lock (_lock)
        {
            var count = _data.RemoveAll(x => x.KhoaHocId == khoaHocId && x.MonHocId == monHocId);
            return Task.FromResult(count > 0);
        }
    }

    private static KhoaHocMonHocDto Clone(KhoaHocMonHocDto src)
    {
        return new KhoaHocMonHocDto
        {
            KhoaHocId = src.KhoaHocId,
            MonHocId = src.MonHocId,
            ThuTu = src.ThuTu,
            SoBuoiDuKien = src.SoBuoiDuKien,
            MaMonHoc = src.MaMonHoc,
            TenMonHoc = src.TenMonHoc,
            MaKhoaHoc = src.MaKhoaHoc,
            TenKhoaHoc = src.TenKhoaHoc
        };
    }
}

/// <summary>
/// Fake/In-memory repository cho Phòng học
/// </summary>
public class FakePhongHocRepository : IPhongHocRepository
{
    private readonly List<PhongHocDto> _data = new();
    private int _autoId = 0;
    private readonly object _lock = new();

    public FakePhongHocRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        AddInternal(new PhongHocDto
        {
            MaPhong = "P101",
            TenPhong = "Phòng Lab Máy tính 01",
            SucChua = 35,
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new PhongHocDto
        {
            MaPhong = "P102",
            TenPhong = "Phòng Lab Máy tính 02",
            SucChua = 40,
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        AddInternal(new PhongHocDto
        {
            MaPhong = "P201",
            TenPhong = "Phòng Lý thuyết Hội thảo",
            SucChua = 60,
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });
    }

    private PhongHocDto AddInternal(PhongHocDto dto)
    {
        dto.Id = ++_autoId;
        _data.Add(Clone(dto));
        return dto;
    }

    public Task<IReadOnlyList<PhongHocDto>> GetAllAsync(PhongHocFilter? filter = null)
    {
        lock (_lock)
        {
            IEnumerable<PhongHocDto> query = _data;

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
                {
                    var keyword = filter.TuKhoa.Trim();
                    query = query.Where(p =>
                        ValidationHelper.ContainsSearch(p.MaPhong, keyword) ||
                        ValidationHelper.ContainsSearch(p.TenPhong, keyword));
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(p => p.TrangThai == filter.TrangThai.Value);
                }
            }

            IReadOnlyList<PhongHocDto> result = query.Select(Clone).ToList();
            return Task.FromResult(result);
        }
    }

    public Task<PhongHocDto?> GetByIdAsync(int id)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<PhongHocDto?> GetByMaPhongAsync(string maPhong)
    {
        lock (_lock)
        {
            var item = _data.FirstOrDefault(x => string.Equals(x.MaPhong, maPhong?.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(item != null ? Clone(item) : null);
        }
    }

    public Task<PhongHocDto> AddAsync(PhongHocDto dto)
    {
        lock (_lock)
        {
            var copy = Clone(dto);
            copy.Id = ++_autoId;
            _data.Add(copy);
            return Task.FromResult(Clone(copy));
        }
    }

    public Task<bool> UpdateAsync(PhongHocDto dto)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == dto.Id);
            if (existing == null) return Task.FromResult(false);

            existing.MaPhong = dto.MaPhong;
            existing.TenPhong = dto.TenPhong;
            existing.SucChua = dto.SucChua;
            existing.TrangThai = dto.TrangThai;

            return Task.FromResult(true);
        }
    }

    public Task<bool> SetTrangThaiAsync(int id, TrangThaiHoatDong trangThai)
    {
        lock (_lock)
        {
            var existing = _data.FirstOrDefault(x => x.Id == id);
            if (existing == null) return Task.FromResult(false);

            existing.TrangThai = trangThai;
            return Task.FromResult(true);
        }
    }

    private static PhongHocDto Clone(PhongHocDto src)
    {
        return new PhongHocDto
        {
            Id = src.Id,
            MaPhong = src.MaPhong,
            TenPhong = src.TenPhong,
            SucChua = src.SucChua,
            TrangThai = src.TrangThai
        };
    }
}
