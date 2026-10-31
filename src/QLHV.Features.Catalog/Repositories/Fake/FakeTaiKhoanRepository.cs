using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Repositories;
using QLHV.Core.Security;

namespace QLHV.Features.Catalog.Repositories.Fake;

/// <summary>
/// Fake/In-memory repository quản lý tài khoản người dùng
/// Mật khẩu được băm (hash) bằng PBKDF2, không lưu mật khẩu thô trong mã nguồn
/// </summary>
public class FakeTaiKhoanRepository : ITaiKhoanRepository
{
    private readonly List<TaiKhoanDto> _taiKhoans = new();
    private readonly Dictionary<string, string> _passwordHashes = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public FakeTaiKhoanRepository(IPasswordHasher passwordHasher)
    {
        InitializeSeedData(passwordHasher);
    }

    private void InitializeSeedData(IPasswordHasher hasher)
    {
        // Danh sách tài khoản demo (mật khẩu được băm ngay lúc nạp seed)
        var seedAccounts = new[]
        {
            new { Id = 1, Username = "admin", Role = VaiTro.ADMIN, Pass = "Admin@123", GvId = (int?)null, Status = TrangThaiTaiKhoan.HoatDong },
            new { Id = 2, Username = "daotao", Role = VaiTro.DAOTAO, Pass = "DaoTao@123", GvId = (int?)null, Status = TrangThaiTaiKhoan.HoatDong },
            new { Id = 3, Username = "thungan", Role = VaiTro.THUNGAN, Pass = "ThuNgan@123", GvId = (int?)null, Status = TrangThaiTaiKhoan.HoatDong },
            new { Id = 4, Username = "giangvien", Role = VaiTro.GIANGVIEN, Pass = "GiangVien@123", GvId = (int?)1, Status = TrangThaiTaiKhoan.HoatDong },
            new { Id = 5, Username = "quanly", Role = VaiTro.QUANLY, Pass = "QuanLy@123", GvId = (int?)null, Status = TrangThaiTaiKhoan.HoatDong },
            new { Id = 6, Username = "bikhoa", Role = VaiTro.DAOTAO, Pass = "BiKhoa@123", GvId = (int?)null, Status = TrangThaiTaiKhoan.BiKhoa }
        };

        foreach (var item in seedAccounts)
        {
            _taiKhoans.Add(new TaiKhoanDto
            {
                Id = item.Id,
                TenDangNhap = item.Username,
                VaiTro = item.Role,
                GiangVienId = item.GvId,
                TrangThai = item.Status,
                LanDangNhapCuoi = null
            });

            _passwordHashes[item.Username] = hasher.HashPassword(item.Pass);
        }
    }

    public Task<TaiKhoanDto?> GetByTenDangNhapAsync(string tenDangNhap)
    {
        lock (_lock)
        {
            var user = _taiKhoans.FirstOrDefault(u => string.Equals(u.TenDangNhap, tenDangNhap?.Trim(), StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user != null ? Clone(user) : null);
        }
    }

    public Task<string?> GetPasswordHashAsync(string tenDangNhap)
    {
        lock (_lock)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return Task.FromResult<string?>(null);
            _passwordHashes.TryGetValue(tenDangNhap.Trim(), out var hash);
            return Task.FromResult(hash);
        }
    }

    public Task<bool> UpdateLanDangNhapCuoiAsync(int id, DateTime thoiGian)
    {
        lock (_lock)
        {
            var user = _taiKhoans.FirstOrDefault(u => u.Id == id);
            if (user == null) return Task.FromResult(false);

            user.LanDangNhapCuoi = thoiGian;
            return Task.FromResult(true);
        }
    }

    public Task<IReadOnlyList<TaiKhoanDto>> GetAllAsync()
    {
        lock (_lock)
        {
            IReadOnlyList<TaiKhoanDto> list = _taiKhoans.Select(Clone).ToList();
            return Task.FromResult(list);
        }
    }

    private static TaiKhoanDto Clone(TaiKhoanDto src)
    {
        return new TaiKhoanDto
        {
            Id = src.Id,
            TenDangNhap = src.TenDangNhap,
            VaiTro = src.VaiTro,
            GiangVienId = src.GiangVienId,
            TrangThai = src.TrangThai,
            LanDangNhapCuoi = src.LanDangNhapCuoi
        };
    }
}
