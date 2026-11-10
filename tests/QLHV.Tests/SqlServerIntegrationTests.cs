using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Services;
using QLHV.Infrastructure.SqlServer;
using QLHV.Infrastructure.SqlServer.Repositories;

namespace QLHV.Tests;

[TestClass]
[TestCategory("SqlServer")]
[DoNotParallelize]
public class SqlServerIntegrationTests
{
    private const string TargetDatabaseName = "QLHV_Test";
    private static string _connectionString = string.Empty;
    private static SqlConnectionFactory _connectionFactory = null!;
    private static IPasswordHasher _passwordHasher = null!;
    private static IAppLogger _logger = null!;
    private static bool _initializedSuccessfully;

    [ClassInitialize]
    public static async Task ClassInit(TestContext context)
    {
        _initializedSuccessfully = false;
        _logger = new FileAppLogger();
        _passwordHasher = new Pbkdf2PasswordHasher();

        var envCs = Environment.GetEnvironmentVariable("QLHV_TEST_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(envCs))
        {
            _connectionString = envCs.Trim();
            var builder = new SqlConnectionStringBuilder(_connectionString);
            if (!string.Equals(builder.InitialCatalog, TargetDatabaseName, StringComparison.OrdinalIgnoreCase))
            {
                Assert.Fail($"An toàn dữ liệu: Chuỗi kết nối kiểm thử cung cấp qua QLHV_TEST_CONNECTION_STRING phải trỏ chính xác vào database '{TargetDatabaseName}', hiện tại là '{builder.InitialCatalog}'.");
            }

            _connectionFactory = new SqlConnectionFactory(_connectionString);
            try
            {
                await using var conn = (SqlConnection)_connectionFactory.CreateConnection();
                await conn.OpenAsync();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Không thể kết nối đến SQL Server Data Source='{builder.DataSource}', Database='{builder.InitialCatalog}': {ex.Message}");
            }
        }
        else
        {
            // Thử các chuỗi kết nối local mặc định đã kiểm chứng thành công
            string[] defaultDataSources = { @".\SQLEXPRESS", @"localhost\SQLEXPRESS", @"(local)\SQLEXPRESS" };
            Exception? lastEx = null;
            bool connected = false;

            foreach (var ds in defaultDataSources)
            {
                var candidateCs = $@"Server={ds};Database={TargetDatabaseName};Integrated Security=True;Encrypt=False;TrustServerCertificate=True;Connect Timeout=5";
                try
                {
                    var factory = new SqlConnectionFactory(candidateCs);
                    await using var conn = (SqlConnection)factory.CreateConnection();
                    await conn.OpenAsync();
                    _connectionString = candidateCs;
                    _connectionFactory = factory;
                    connected = true;
                    break;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                }
            }

            if (!connected)
            {
                Assert.Fail($"Không thể kết nối đến SQL Server với database '{TargetDatabaseName}' qua bất kỳ Data Source mặc định nào ({string.Join(", ", defaultDataSources)}): {lastEx?.Message}");
            }
        }

        _initializedSuccessfully = true;
        await CleanTestRecordsAsync();
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        if (_initializedSuccessfully)
        {
            await CleanTestRecordsAsync();
        }
    }

    [TestInitialize]
    public async Task TestInit()
    {
        if (_initializedSuccessfully)
        {
            await CleanTestRecordsAsync();
        }
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        if (_initializedSuccessfully)
        {
            await CleanTestRecordsAsync();
        }
    }

    private static async Task CleanTestRecordsAsync()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        if (!string.Equals(builder.InitialCatalog, TargetDatabaseName, StringComparison.OrdinalIgnoreCase))
        {
            Assert.Fail($"Bảo vệ an toàn dữ liệu: Chỉ được phép xóa dữ liệu trên database '{TargetDatabaseName}', hiện tại là '{builder.InitialCatalog}'.");
        }

        await using var conn = (SqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync();
        const string sql = """
            DELETE FROM dbo.KhoaHocMonHoc
            WHERE KhoaHocId IN (SELECT Id FROM dbo.KhoaHoc WHERE MaKhoaHoc LIKE 'TEST[_]%')
               OR MonHocId IN (SELECT Id FROM dbo.MonHoc WHERE MaMonHoc LIKE 'TEST[_]%');

            DELETE FROM dbo.HocVien WHERE MaHocVien LIKE 'TEST[_]%';
            DELETE FROM dbo.GiangVien WHERE MaGiangVien LIKE 'TEST[_]%';
            DELETE FROM dbo.MonHoc WHERE MaMonHoc LIKE 'TEST[_]%';
            DELETE FROM dbo.KhoaHoc WHERE MaKhoaHoc LIKE 'TEST[_]%';
            DELETE FROM dbo.PhongHoc WHERE MaPhong LIKE 'TEST[_]%';
            """;
        await using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    [TestMethod]
    public async Task SqlTaiKhoan_Login_Success_WithCorrectPassword()
    {
        var repo = new SqlTaiKhoanRepository(_connectionFactory);
        var authz = new AuthorizationService();
        var currentUserService = new CurrentUserService(authz);
        var authService = new AuthService(repo, _passwordHasher, currentUserService, _logger);

        var result = await authService.LoginAsync("admin", "Admin@123");

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("admin", result.Data.TenDangNhap);
        Assert.AreEqual(VaiTro.ADMIN, result.Data.VaiTro);
        Assert.IsNotNull(result.Data.LanDangNhapCuoi);
        Assert.IsTrue(currentUserService.IsAuthenticated);
        Assert.AreEqual("admin", currentUserService.CurrentUser?.TenDangNhap);
    }

    [TestMethod]
    public async Task SqlTaiKhoan_Login_Fails_WithWrongPassword()
    {
        var repo = new SqlTaiKhoanRepository(_connectionFactory);
        var authz = new AuthorizationService();
        var currentUserService = new CurrentUserService(authz);
        var authService = new AuthService(repo, _passwordHasher, currentUserService, _logger);

        var result = await authService.LoginAsync("admin", "SaiMatKhau@123");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_CREDENTIALS, result.Error?.Code);
        Assert.IsFalse(currentUserService.IsAuthenticated);
    }

    [TestMethod]
    public async Task SqlTaiKhoan_Login_Fails_WhenAccountLocked()
    {
        var repo = new SqlTaiKhoanRepository(_connectionFactory);
        var authz = new AuthorizationService();
        var currentUserService = new CurrentUserService(authz);
        var authService = new AuthService(repo, _passwordHasher, currentUserService, _logger);

        var result = await authService.LoginAsync("bikhoa", "BiKhoa@123");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.ACCOUNT_LOCKED, result.Error?.Code);
        Assert.IsFalse(currentUserService.IsAuthenticated);
    }

    [TestMethod]
    public async Task SqlHocVien_Crud_And_Filter_SqlServer()
    {
        var repo = new SqlHocVienRepository(_connectionFactory);

        // 1. Create
        var newHocVien = new HocVienDto
        {
            MaHocVien = "TEST_HV01",
            HoTen = "Nguyễn TEST An",
            NgaySinh = new DateTime(2002, 5, 10),
            GioiTinh = "Nam",
            DienThoai = "0987654321",
            Email = "test.an@example.com",
            DiaChi = "Hà Nội",
            NgayDangKy = DateTime.Today,
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        var created = await repo.AddAsync(newHocVien);
        Assert.IsGreaterThan(0, created.Id);

        // 2. Read by Id & Ma
        var byId = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(byId);
        Assert.AreEqual("TEST_HV01", byId.MaHocVien);
        Assert.AreEqual("Nguyễn TEST An", byId.HoTen);

        var byMa = await repo.GetByMaHocVienAsync("test_hv01"); // Case-insensitive check
        Assert.IsNotNull(byMa);
        Assert.AreEqual(created.Id, byMa.Id);

        // 3. Filter with unaccented Vietnamese search (Vietnamese_100_CI_AI)
        var searchUnaccented = await repo.GetAllAsync(new HocVienFilter { TuKhoa = "Nguyen TEST" });
        Assert.IsTrue(searchUnaccented.Any(h => h.MaHocVien == "TEST_HV01"));

        // 4. Update
        created.HoTen = "Nguyễn TEST An Cập Nhật";
        created.DiaChi = "Đà Nẵng";
        var updateSuccess = await repo.UpdateAsync(created);
        Assert.IsTrue(updateSuccess);

        var updated = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(updated);
        Assert.AreEqual("Nguyễn TEST An Cập Nhật", updated.HoTen);
        Assert.AreEqual("Đà Nẵng", updated.DiaChi);

        // 5. Soft Deactivate
        var deactivateSuccess = await repo.SetTrangThaiAsync(created.Id, TrangThaiHoatDong.NgungHoatDong);
        Assert.IsTrue(deactivateSuccess);

        var deactivated = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(deactivated);
        Assert.AreEqual(TrangThaiHoatDong.NgungHoatDong, deactivated.TrangThai);

        // 6. Persistence check with brand new repository instance
        var newRepoInstance = new SqlHocVienRepository(_connectionFactory);
        var persisted = await newRepoInstance.GetByIdAsync(created.Id);
        Assert.IsNotNull(persisted);
        Assert.AreEqual(TrangThaiHoatDong.NgungHoatDong, persisted.TrangThai);
    }

    [TestMethod]
    public async Task SqlGiangVien_Crud_And_Filter_SqlServer()
    {
        var repo = new SqlGiangVienRepository(_connectionFactory);

        var newGv = new GiangVienDto
        {
            MaGiangVien = "TEST_GV01",
            HoTen = "ThS. Hoàng TEST Dũng",
            ChuyenMon = "Lập trình C# WinForms",
            DienThoai = "0912345678",
            Email = "test.gv@example.com",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        var created = await repo.AddAsync(newGv);
        Assert.IsGreaterThan(0, created.Id);

        var byMa = await repo.GetByMaGiangVienAsync("test_gv01");
        Assert.IsNotNull(byMa);
        Assert.AreEqual("ThS. Hoàng TEST Dũng", byMa.HoTen);

        // Unaccented search
        var searchRes = await repo.GetAllAsync(new GiangVienFilter { TuKhoa = "Hoang TEST Dung" });
        Assert.IsTrue(searchRes.Any(g => g.MaGiangVien == "TEST_GV01"));

        // Update
        created.ChuyenMon = "Kiến trúc hệ thống .NET";
        var ok = await repo.UpdateAsync(created);
        Assert.IsTrue(ok);

        // Soft Deactivate
        var deact = await repo.SetTrangThaiAsync(created.Id, TrangThaiHoatDong.NgungHoatDong);
        Assert.IsTrue(deact);

        var check = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(check);
        Assert.AreEqual(TrangThaiHoatDong.NgungHoatDong, check.TrangThai);
    }

    [TestMethod]
    public async Task SqlMonHoc_Crud_And_Filter_SqlServer()
    {
        var repo = new SqlMonHocRepository(_connectionFactory);

        var newMh = new MonHocDto
        {
            MaMonHoc = "TEST_MH01",
            TenMonHoc = "Lập trình TEST Môn Học",
            SoBuoiMacDinh = 15,
            MoTa = "Mô tả môn học test",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        var created = await repo.AddAsync(newMh);
        Assert.IsGreaterThan(0, created.Id);

        // Search unaccented
        var searchRes = await repo.GetAllAsync(new MonHocFilter { TuKhoa = "Lap trinh TEST" });
        Assert.IsTrue(searchRes.Any(m => m.MaMonHoc == "TEST_MH01"));

        // Update
        created.SoBuoiMacDinh = 20;
        var ok = await repo.UpdateAsync(created);
        Assert.IsTrue(ok);

        var byId = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(byId);
        Assert.AreEqual(20, byId.SoBuoiMacDinh);

        // Deactivate
        await repo.SetTrangThaiAsync(created.Id, TrangThaiHoatDong.NgungHoatDong);
        var deact = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(deact);
        Assert.AreEqual(TrangThaiHoatDong.NgungHoatDong, deact.TrangThai);
    }

    [TestMethod]
    public async Task SqlKhoaHoc_Crud_And_Filter_SqlServer()
    {
        var repo = new SqlKhoaHocRepository(_connectionFactory);

        var newKh = new KhoaHocDto
        {
            MaKhoaHoc = "TEST_KH01",
            TenKhoaHoc = "Chuyên đề TEST Khóa Học",
            HocPhiNiemYet = 4500000.00m,
            MoTa = "Mô tả khóa học test",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        var created = await repo.AddAsync(newKh);
        Assert.IsGreaterThan(0, created.Id);

        // Search unaccented
        var searchRes = await repo.GetAllAsync(new KhoaHocFilter { TuKhoa = "Chuyen de TEST" });
        Assert.IsTrue(searchRes.Any(k => k.MaKhoaHoc == "TEST_KH01"));

        // Update
        created.HocPhiNiemYet = 5000000.00m;
        var ok = await repo.UpdateAsync(created);
        Assert.IsTrue(ok);

        var byId = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(byId);
        Assert.AreEqual(5000000.00m, byId.HocPhiNiemYet);

        // Deactivate
        await repo.SetTrangThaiAsync(created.Id, TrangThaiHoatDong.NgungHoatDong);
        var deact = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(deact);
        Assert.AreEqual(TrangThaiHoatDong.NgungHoatDong, deact.TrangThai);
    }

    [TestMethod]
    public async Task SqlPhongHoc_Crud_And_Filter_SqlServer()
    {
        var repo = new SqlPhongHocRepository(_connectionFactory);

        var newP = new PhongHocDto
        {
            MaPhong = "TEST_P01",
            TenPhong = "Phòng Máy TEST Thực Hành",
            SucChua = 40,
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        var created = await repo.AddAsync(newP);
        Assert.IsGreaterThan(0, created.Id);

        // Search unaccented
        var searchRes = await repo.GetAllAsync(new PhongHocFilter { TuKhoa = "Thuc Hanh" });
        Assert.IsTrue(searchRes.Any(p => p.MaPhong == "TEST_P01"));

        // Update
        created.SucChua = 50;
        var ok = await repo.UpdateAsync(created);
        Assert.IsTrue(ok);

        var byId = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(byId);
        Assert.AreEqual(50, byId.SucChua);

        // Deactivate
        await repo.SetTrangThaiAsync(created.Id, TrangThaiHoatDong.NgungHoatDong);
        var deact = await repo.GetByIdAsync(created.Id);
        Assert.IsNotNull(deact);
        Assert.AreEqual(TrangThaiHoatDong.NgungHoatDong, deact.TrangThai);
    }

    [TestMethod]
    public async Task SqlKhoaHocMonHoc_Relation_SqlServer()
    {
        var khRepo = new SqlKhoaHocRepository(_connectionFactory);
        var mhRepo = new SqlMonHocRepository(_connectionFactory);
        var khmRepo = new SqlKhoaHocMonHocRepository(_connectionFactory);

        var kh = await khRepo.AddAsync(new KhoaHocDto
        {
            MaKhoaHoc = "TEST_KH99",
            TenKhoaHoc = "Khóa Test Quan Hệ",
            HocPhiNiemYet = 2000000m
        });

        var mh = await mhRepo.AddAsync(new MonHocDto
        {
            MaMonHoc = "TEST_MH99",
            TenMonHoc = "Môn Test Quan Hệ",
            SoBuoiMacDinh = 10
        });

        // 1. Add relation
        var rel = new KhoaHocMonHocDto
        {
            KhoaHocId = kh.Id,
            MonHocId = mh.Id,
            ThuTu = 1,
            SoBuoiDuKien = 12
        };

        var added = await khmRepo.AddAsync(rel);
        Assert.AreEqual(kh.Id, added.KhoaHocId);
        Assert.AreEqual(mh.Id, added.MonHocId);
        Assert.AreEqual("TEST_KH99", added.MaKhoaHoc);
        Assert.AreEqual("Khóa Test Quan Hệ", added.TenKhoaHoc);
        Assert.AreEqual("TEST_MH99", added.MaMonHoc);
        Assert.AreEqual("Môn Test Quan Hệ", added.TenMonHoc);

        // 2. Get list by KhoaHocId
        var list = await khmRepo.GetByKhoaHocIdAsync(kh.Id);
        Assert.HasCount(1, list);
        Assert.AreEqual(mh.Id, list[0].MonHocId);

        // 3. Update relation
        rel.ThuTu = 2;
        rel.SoBuoiDuKien = 16;
        var updateOk = await khmRepo.UpdateAsync(rel);
        Assert.IsTrue(updateOk);

        var updated = await khmRepo.GetAsync(kh.Id, mh.Id);
        Assert.IsNotNull(updated);
        Assert.AreEqual(2, updated.ThuTu);
        Assert.AreEqual(16, updated.SoBuoiDuKien);

        // 4. Delete relation
        var delOk = await khmRepo.DeleteAsync(kh.Id, mh.Id);
        Assert.IsTrue(delOk);

        var deleted = await khmRepo.GetAsync(kh.Id, mh.Id);
        Assert.IsNull(deleted);
    }

    [TestMethod]
    public async Task Search_WithSpecialCharacters_PreservesLiteralBehavior()
    {
        var repo = new SqlMonHocRepository(_connectionFactory);

        // Create two items: one with special characters, one without
        var mhSpecial = await repo.AddAsync(new MonHocDto
        {
            MaMonHoc = "TEST_M_SPEC",
            TenMonHoc = "Môn Học 10% [Đặc Biệt]",
            SoBuoiMacDinh = 10
        });

        var mhNormal = await repo.AddAsync(new MonHocDto
        {
            MaMonHoc = "TEST_M_NORM",
            TenMonHoc = "Môn Học 100 Điểm",
            SoBuoiMacDinh = 10
        });

        // Search with "10%" - must NOT treat '%' as wildcard matching "100"
        var resPercent = await repo.GetAllAsync(new MonHocFilter { TuKhoa = "10%" });
        Assert.IsTrue(resPercent.Any(m => m.Id == mhSpecial.Id), "Nên tìm thấy môn học có chuỗi '10%'");
        Assert.IsFalse(resPercent.Any(m => m.Id == mhNormal.Id), "Không được khớp wildcard '100' khi tìm '10%'");

        // Search with "[" - must NOT treat '[' as wildcard character class
        var resBracket = await repo.GetAllAsync(new MonHocFilter { TuKhoa = "[" });
        Assert.IsTrue(resBracket.Any(m => m.Id == mhSpecial.Id));
        Assert.IsFalse(resBracket.Any(m => m.Id == mhNormal.Id));

        // Search with "_" - must NOT treat '_' as single-char wildcard
        var resUnderscore = await repo.GetAllAsync(new MonHocFilter { TuKhoa = "TEST_M_SPEC" });
        Assert.IsTrue(resUnderscore.Any(m => m.Id == mhSpecial.Id));
    }
}
