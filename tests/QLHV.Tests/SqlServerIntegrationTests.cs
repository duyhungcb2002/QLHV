using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Database;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Services;
using QLHV.Infrastructure.SqlServer;
using QLHV.Infrastructure.SqlServer.Repositories;
using QLHV.WinForms.Configuration;

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
                var candidateCs = $@"Server={ds};Database={TargetDatabaseName};Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30";
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
            DELETE FROM dbo.TaiKhoan WHERE TenDangNhap LIKE 'TEST[_]%';
            """;
        await using var cmd = new SqlCommand(sql, conn);
        cmd.CommandTimeout = 60;
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

        /// <summary>
    /// Bộ theo dõi và dọn dẹp chính xác theo từng bản ghi test tạo ra (không dùng wildcard LIKE 'TEST_%')
    /// Xóa theo thứ tự ngược của quan hệ khóa ngoại (Reverse FK Order)
    /// </summary>
    private sealed class SqlTestTracker
    {
        public List<int> PhieuThuIds { get; } = new();
        public List<int> KetQuaIds { get; } = new();
        public List<int> LichHocIds { get; } = new();
        public List<int> DangKyIds { get; } = new();
        public List<int> LopHocIds { get; } = new();
        public List<(int KhoaHocId, int MonHocId)> KhoaHocMonHocs { get; } = new();
        public List<int> KhoaHocIds { get; } = new();
        public List<int> MonHocIds { get; } = new();
        public List<int> PhongHocIds { get; } = new();
        public List<int> GiangVienIds { get; } = new();
        public List<int> HocVienIds { get; } = new();
        public List<int> TaiKhoanIds { get; } = new();

        public async Task CleanupAsync(IConnectionFactory connectionFactory)
        {
            var errors = new List<Exception>();
            try
            {
                await using var conn = (SqlConnection)connectionFactory.CreateConnection();
                await conn.OpenAsync();

                // Thứ tự xóa an toàn: PhieuThu -> KetQuaHocTap -> LichHoc -> DangKyLopHoc -> LopHoc -> KhoaHocMonHoc -> KhoaHoc -> MonHoc -> PhongHoc -> GiangVien -> HocVien -> TaiKhoan
                try { await DeleteByIdsAsync(conn, "dbo.PhieuThu", PhieuThuIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.KetQuaHocTap", KetQuaIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.LichHoc", LichHocIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.DangKyLopHoc", DangKyIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.LopHoc", LopHocIds); } catch (Exception ex) { errors.Add(ex); }

                foreach (var pair in KhoaHocMonHocs)
                {
                    try
                    {
                        await using var cmd = new SqlCommand("DELETE FROM dbo.KhoaHocMonHoc WHERE KhoaHocId = @KhId AND MonHocId = @MhId;", conn);
                        cmd.Parameters.Add(new SqlParameter("@KhId", SqlDbType.Int) { Value = pair.KhoaHocId });
                        cmd.Parameters.Add(new SqlParameter("@MhId", SqlDbType.Int) { Value = pair.MonHocId });
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex);
                    }
                }

                try { await DeleteByIdsAsync(conn, "dbo.KhoaHoc", KhoaHocIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.MonHoc", MonHocIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.PhongHoc", PhongHocIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.GiangVien", GiangVienIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.HocVien", HocVienIds); } catch (Exception ex) { errors.Add(ex); }
                try { await DeleteByIdsAsync(conn, "dbo.TaiKhoan", TaiKhoanIds); } catch (Exception ex) { errors.Add(ex); }
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }

            if (errors.Count > 0)
            {
                throw new AggregateException("Lỗi xảy ra trong quá trình dọn dẹp dữ liệu kiểm thử SQL.", errors);
            }
        }

        private static async Task DeleteByIdsAsync(SqlConnection conn, string tableName, List<int> ids)
        {
            if (ids.Count == 0) return;
            var distinctIds = ids.Distinct().ToList();
            var paramNames = distinctIds.Select((_, i) => $"@id{i}").ToList();
            string sql = $"DELETE FROM {tableName} WHERE Id IN ({string.Join(", ", paramNames)});";
            await using var cmd = new SqlCommand(sql, conn);
            for (int i = 0; i < distinctIds.Count; i++)
            {
                cmd.Parameters.Add(new SqlParameter($"@id{i}", SqlDbType.Int) { Value = distinctIds[i] });
            }
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private static async Task<int> CreateTestAccountAsync(IConnectionFactory connectionFactory, SqlTestTracker tracker, string tag)
    {
        var username = $"TEST_ACC_{tag}_{Guid.NewGuid().ToString("N")[..6]}";
        await using var conn = (SqlConnection)connectionFactory.CreateConnection();
        await conn.OpenAsync();

        const string sql = """
            INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, TrangThai)
            OUTPUT INSERTED.Id
            VALUES (@TenDangNhap, @MatKhauHash, @VaiTro, 1);
            """;
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@TenDangNhap", SqlDbType.NVarChar, 50) { Value = username });
        cmd.Parameters.Add(new SqlParameter("@MatKhauHash", SqlDbType.NVarChar, 256) { Value = _passwordHasher.HashPassword("TestPass@123") });
        cmd.Parameters.Add(new SqlParameter("@VaiTro", SqlDbType.NVarChar, 30) { Value = "THUNGAN" });

        var newId = (int)(await cmd.ExecuteScalarAsync())!;
        tracker.TaiKhoanIds.Add(newId);
        return newId;
    }

    [TestMethod]
    public async Task SqlHocVien_ProfileExpansion_RoundTrip_AndFilteredUniqueIndex()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var tkRepo = new SqlTaiKhoanRepository(_connectionFactory);

            var accounts = await tkRepo.GetAllAsync();
            var staff = accounts.FirstOrDefault(t => t.TrangThai == TrangThaiTaiKhoan.HoatDong) ?? throw new InvalidOperationException("Cần ít nhất một tài khoản hoạt động trong CSDL kiểm thử.");

            string maHocVien = "TEST_HV_P_" + DateTime.UtcNow.Ticks.ToString()[^6..];
            string uniqueSdd = "001" + DateTime.UtcNow.Ticks.ToString()[^9..];
            var hvDto = new HocVienDto
            {
                MaHocVien = maHocVien,
                HoTen = "Học viên Kiểm thử Mở rộng Hồ sơ",
                NgaySinh = new DateTime(1998, 5, 20),
                GioiTinh = "Nam",
                DienThoai = "0933445566",
                Email = "test_profile@example.com",
                DiaChi = "Tầng 5, Tòa nhà Công nghệ, Cầu Giấy, Hà Nội",
                NgayDangKy = DateTime.Today,
                TrangThai = TrangThaiHoatDong.DangHoatDong,
                SoDinhDanh = uniqueSdd,
                NgayCapGiayTo = new DateTime(2020, 6, 15),
                NoiCapGiayTo = "Cục Cảnh sát QLHC về TTXH",
                HoTenNguoiGiamHo = "Phạm Thị Mẹ",
                DienThoaiNguoiGiamHo = "0944556677",
                QuanHeNguoiGiamHo = "Mẹ",
                NhanVienPhuTrachTaiKhoanId = staff.Id,
                NguonTuyenSinh = "Website",
                NgheNghiep = "Kỹ sư Cầu đường",
                DonViCongTac = "Tổng công ty Xây dựng",
                GhiChuDacBiet = "Yêu cầu phòng học có máy chiếu và điều hòa"
            };

            var added = await hvRepo.AddAsync(hvDto);
            tracker.HocVienIds.Add(added.Id);

            // Read by Id
            var fetched = await hvRepo.GetByIdAsync(added.Id);
            Assert.IsNotNull(fetched);
            Assert.AreEqual(uniqueSdd, fetched.SoDinhDanh);
            Assert.AreEqual("Cục Cảnh sát QLHC về TTXH", fetched.NoiCapGiayTo);
            Assert.AreEqual("Phạm Thị Mẹ", fetched.HoTenNguoiGiamHo);
            Assert.AreEqual("0944556677", fetched.DienThoaiNguoiGiamHo);
            Assert.AreEqual("Mẹ", fetched.QuanHeNguoiGiamHo);
            Assert.AreEqual(staff.Id, fetched.NhanVienPhuTrachTaiKhoanId);
            Assert.AreEqual($"{staff.TenDangNhap} ({staff.VaiTro})", fetched.TenNhanVienPhuTrach);
            Assert.AreEqual("Website", fetched.NguonTuyenSinh);
            Assert.AreEqual("Kỹ sư Cầu đường", fetched.NgheNghiep);
            Assert.AreEqual("Tổng công ty Xây dựng", fetched.DonViCongTac);
            Assert.AreEqual("Yêu cầu phòng học có máy chiếu và điều hòa", fetched.GhiChuDacBiet);

            // Read by SoDinhDanh
            var bySdd = await hvRepo.GetBySoDinhDanhAsync(uniqueSdd);
            Assert.IsNotNull(bySdd);
            Assert.AreEqual(added.Id, bySdd.Id);

            // Update profile
            fetched.HoTen = "Học viên Kiểm thử Đã Cập Nhật";
            fetched.NgheNghiep = "Trưởng nhóm Giải pháp";
            fetched.GhiChuDacBiet = "Đã hoàn thành thủ tục nhập học";
            var updated = await hvRepo.UpdateAsync(fetched);
            Assert.IsTrue(updated);

            var fetchedUpdated = await hvRepo.GetByIdAsync(added.Id);
            Assert.IsNotNull(fetchedUpdated);
            Assert.AreEqual("Trưởng nhóm Giải pháp", fetchedUpdated.NgheNghiep);
            Assert.AreEqual("Đã hoàn thành thủ tục nhập học", fetchedUpdated.GhiChuDacBiet);

            // Filtered Unique Index trên SoDinhDanh
            var duplicateDto = new HocVienDto
            {
                MaHocVien = "TEST_HV_DUP_" + DateTime.UtcNow.Ticks.ToString()[^6..],
                HoTen = "Người Trùng Định Danh",
                SoDinhDanh = uniqueSdd,
                NgayDangKy = DateTime.Today,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            };

            bool threwDbException = false;
            try
            {
                await hvRepo.AddAsync(duplicateDto);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                threwDbException = true;
            }

            Assert.IsTrue(threwDbException, "SQL Server filtered unique index UX_HocVien_SoDinhDanh phải chặn trùng số định danh.");
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task SqlLopHoc_GiangVienChuNhiem_PersistenceAndJoin()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var gvRepo = new SqlGiangVienRepository(_connectionFactory);

            var kh = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_GVCN_{tag}",
                TenKhoaHoc = $"Khóa học có GV Chủ nhiệm {tag}",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(kh.Id);

            var gv1 = await gvRepo.AddAsync(new GiangVienDto
            {
                MaGiangVien = $"GV1_CN_{tag}",
                HoTen = $"Giảng viên Chủ nhiệm 1 {tag}",
                Email = $"gv1.cn.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.GiangVienIds.Add(gv1.Id);

            var gv2 = await gvRepo.AddAsync(new GiangVienDto
            {
                MaGiangVien = $"GV2_CN_{tag}",
                HoTen = $"Giảng viên Chủ nhiệm 2 {tag}",
                Email = $"gv2.cn.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.GiangVienIds.Add(gv2.Id);

            var lopDto = new LopHocDto
            {
                MaLopHoc = $"LH_GVCN_{tag}",
                TenLopHoc = $"Lớp Thí nghiệm GVCN {tag}",
                KhoaHocId = kh.Id,
                GiangVienChuNhiemId = gv1.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 25,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            };

            var added = await lopRepo.AddAsync(lopDto);
            tracker.LopHocIds.Add(added.Id);

            var fetched = await lopRepo.GetByIdAsync(added.Id);
            Assert.IsNotNull(fetched);
            Assert.AreEqual(gv1.Id, fetched.GiangVienChuNhiemId);
            Assert.AreEqual(gv1.HoTen, fetched.HoTenGiangVienChuNhiem, "LEFT JOIN dbo.GiangVien phải trả về họ tên giảng viên chủ nhiệm.");

            // Update GVCN sang giảng viên 2
            fetched.GiangVienChuNhiemId = gv2.Id;
            var updated = await lopRepo.UpdateAsync(fetched);
            Assert.IsTrue(updated);

            var fetched2 = await lopRepo.GetByIdAsync(added.Id);
            Assert.IsNotNull(fetched2);
            Assert.AreEqual(gv2.Id, fetched2.GiangVienChuNhiemId);
            Assert.AreEqual(gv2.HoTen, fetched2.HoTenGiangVienChuNhiem);
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task SqlData_UnicodeExactStrings_NoMojibake()
    {
        await using var conn = (SqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync();

        // 1. Kiểm tra chính xác các bản ghi seed mẫu theo mã nghiệp vụ
        const string sqlExactLop1 = "SELECT TenLopHoc, GhiChu FROM dbo.LopHoc WHERE MaLopHoc = 'LH_NET_01';";
        await using (var cmd = new SqlCommand(sqlExactLop1, conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            Assert.IsTrue(await reader.ReadAsync(), "Bản ghi seed LH_NET_01 phải tồn tại.");
            Assert.AreEqual("Lớp Chuyên viên .NET K01", reader.GetString(0));
            Assert.AreEqual("Lớp học ban ngày thứ 2-4-6", reader.GetString(1));
        }

        const string sqlExactLop2 = "SELECT TenLopHoc, GhiChu FROM dbo.LopHoc WHERE MaLopHoc = 'LH_WF_02';";
        await using (var cmd = new SqlCommand(sqlExactLop2, conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            Assert.IsTrue(await reader.ReadAsync(), "Bản ghi seed LH_WF_02 phải tồn tại.");
            Assert.AreEqual("Lớp WinForms Nâng Cao K02", reader.GetString(0));
            Assert.AreEqual("Lớp học buổi tối thứ 3-5-7", reader.GetString(1));
        }

        const string sqlExactMon = "SELECT TenMonHoc FROM dbo.MonHoc WHERE MaMonHoc = 'MH01';";
        await using (var cmd = new SqlCommand(sqlExactMon, conn))
        {
            var tenMon = (string?)await cmd.ExecuteScalarAsync();
            Assert.AreEqual("Lập trình C# cơ bản", tenMon);
        }

        const string sqlExactHv = "SELECT HoTen FROM dbo.HocVien WHERE MaHocVien = 'HV001';";
        await using (var cmd = new SqlCommand(sqlExactHv, conn))
        {
            var hoTen = (string?)await cmd.ExecuteScalarAsync();
            Assert.AreEqual("Nguyễn Văn An", hoTen);
        }

        const string sqlExactPt1 = "SELECT GhiChu FROM dbo.PhieuThu WHERE MaPhieuThu = 'PT-20260215-001';";
        await using (var cmd = new SqlCommand(sqlExactPt1, conn))
        {
            var ghiChuPt = (string?)await cmd.ExecuteScalarAsync();
            Assert.AreEqual("Nộp đợt 1 chuyển khoản qua Vietcombank", ghiChuPt);
        }

        const string sqlExactPt2 = "SELECT GhiChu FROM dbo.PhieuThu WHERE MaPhieuThu = 'PT-20260301-002';";
        await using (var cmd = new SqlCommand(sqlExactPt2, conn))
        {
            var ghiChuPt = (string?)await cmd.ExecuteScalarAsync();
            Assert.AreEqual("Nộp đợt 2 đủ học phí bằng tiền mặt", ghiChuPt);
        }
    }

    [TestMethod]
    public async Task Migration007_BOM_And_SafeRepair()
    {
        // 1. Kiểm tra UTF-8 BOM trên file database/007_unicode_data_repair.sql (EF BB BF)
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", "007_unicode_data_repair.sql");
        var fullPath = Path.GetFullPath(scriptPath);
        Assert.IsTrue(File.Exists(fullPath), $"File migration 007 phải tồn tại tại '{fullPath}'.");

        var fileBytes = await File.ReadAllBytesAsync(fullPath);
        Assert.IsGreaterThanOrEqualTo(3, fileBytes.Length, "File migration 007 phải có ít nhất 3 bytes.");
        Assert.AreEqual(0xEF, fileBytes[0], "Byte 1 của file 007 phải là UTF-8 BOM 0xEF.");
        Assert.AreEqual(0xBB, fileBytes[1], "Byte 2 của file 007 phải là UTF-8 BOM 0xBB.");
        Assert.AreEqual(0xBF, fileBytes[2], "Byte 3 của file 007 phải là UTF-8 BOM 0xBF.");

        // 2. Kiểm tra an toàn trực tiếp trên bản ghi seed LH_NET_01.GhiChu
        await using var conn = (SqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync();

        // Lưu lại dữ liệu ban đầu của LH_NET_01.GhiChu để luôn khôi phục trong finally
        string? originalGhiChu = null;
        await using (var getCmd = new SqlCommand("SELECT GhiChu FROM dbo.LopHoc WHERE MaLopHoc = 'LH_NET_01';", conn))
        {
            originalGhiChu = (string?)await getCmd.ExecuteScalarAsync();
        }

        // Đọc toàn bộ nội dung script migration 007
        var scriptText = await File.ReadAllTextAsync(fullPath, System.Text.Encoding.UTF8);
        var sqlStatements = scriptText
            .Split(new[] { "\r\nGO", "\nGO", "\rGO" }, StringSplitOptions.RemoveEmptyEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        async Task RunMigration007StatementsAsync()
        {
            foreach (var stmt in sqlStatements)
            {
                var trimmed = stmt.Trim();
                if (trimmed.StartsWith("USE ", StringComparison.OrdinalIgnoreCase)) continue;
                await using var cmd = new SqlCommand(trimmed, conn);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        try
        {
            // Bước A: Gán chuỗi tiếng Việt hợp lệ N'Âm thanh cần hỗ trợ'
            await using (var setValidCmd = new SqlCommand("UPDATE dbo.LopHoc SET GhiChu = N'Âm thanh cần hỗ trợ' WHERE MaLopHoc = 'LH_NET_01';", conn))
            {
                await setValidCmd.ExecuteNonQueryAsync();
            }

            // Chạy migration 007
            await RunMigration007StatementsAsync();

            // Xác nhận chuỗi hợp lệ N'Âm thanh cần hỗ trợ' TUYỆT ĐỐI không bị sửa đổi
            await using (var checkValidCmd = new SqlCommand("SELECT GhiChu FROM dbo.LopHoc WHERE MaLopHoc = 'LH_NET_01';", conn))
            {
                var currentGhiChu = (string?)await checkValidCmd.ExecuteScalarAsync();
                Assert.AreEqual("Âm thanh cần hỗ trợ", currentGhiChu, "Chuỗi tiếng Việt hợp lệ chứa ký tự Â không được bị migration 007 ghi đè.");
            }

            // Bước B: Gán chuỗi mojibake đã biết
            const string knownMojibake = "L?p h?c ban ngày th? 2-4-6";
            await using (var setMojiCmd = new SqlCommand("UPDATE dbo.LopHoc SET GhiChu = @Moji WHERE MaLopHoc = 'LH_NET_01';", conn))
            {
                setMojiCmd.Parameters.Add(new SqlParameter("@Moji", SqlDbType.NVarChar, 500) { Value = knownMojibake });
                await setMojiCmd.ExecuteNonQueryAsync();
            }

            // Chạy migration 007
            await RunMigration007StatementsAsync();

            // Xác nhận chuỗi mojibake đã biết được sửa đúng thành tiếng Việt chuẩn
            await using (var checkRepairedCmd = new SqlCommand("SELECT GhiChu FROM dbo.LopHoc WHERE MaLopHoc = 'LH_NET_01';", conn))
            {
                var repairedGhiChu = (string?)await checkRepairedCmd.ExecuteScalarAsync();
                Assert.AreEqual("Lớp học ban ngày thứ 2-4-6", repairedGhiChu, "Chuỗi mojibake đã biết phải được sửa đúng thành tiếng Việt chuẩn.");
            }

            // Bước C: Chạy lần hai để kiểm tra tính lũy thừa (Idempotent)
            await RunMigration007StatementsAsync();

            await using (var checkIdempotentCmd = new SqlCommand("SELECT GhiChu FROM dbo.LopHoc WHERE MaLopHoc = 'LH_NET_01';", conn))
            {
                var secondRunGhiChu = (string?)await checkIdempotentCmd.ExecuteScalarAsync();
                Assert.AreEqual("Lớp học ban ngày thứ 2-4-6", secondRunGhiChu, "Chạy migration lần 2 phải giữ nguyên dữ liệu tiếng Việt chuẩn.");
            }
        }
        finally
        {
            // Luôn khôi phục dữ liệu gốc trong finally, bảo toàn chính xác giá trị kể cả khi NULL
            await using var restoreCmd = new SqlCommand("UPDATE dbo.LopHoc SET GhiChu = @Orig WHERE MaLopHoc = 'LH_NET_01';", conn);
            restoreCmd.Parameters.Add(new SqlParameter("@Orig", SqlDbType.NVarChar, 500)
            {
                Value = originalGhiChu is null ? DBNull.Value : originalGhiChu
            });
            await restoreCmd.ExecuteNonQueryAsync();
        }
    }

    [TestMethod]
    public async Task Operations_CrudAndWorkflow_SqlServerIntegration()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var mhRepo = new SqlMonHocRepository(_connectionFactory);
            var khmRepo = new SqlKhoaHocMonHocRepository(_connectionFactory);
            var phongRepo = new SqlPhongHocRepository(_connectionFactory);
            var gvRepo = new SqlGiangVienRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var tkRepo = new SqlTaiKhoanRepository(_connectionFactory);

            var staffId = await CreateTestAccountAsync(_connectionFactory, tracker, tag);

            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);
            var lichRepo = new SqlLichHocRepository(_connectionFactory);
            var kqRepo = new SqlKetQuaHocTapRepository(_connectionFactory);
            var ptRepo = new SqlPhieuThuRepository(_connectionFactory);
            var bcRepo = new SqlBaoCaoRepository(_connectionFactory);

            // Tạo các danh mục fixture độc lập (không hardcode ID = 1)
            var course = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_OPS_{tag}",
                TenKhoaHoc = $"Khóa Ops Test {tag}",
                HocPhiNiemYet = 5000000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(course.Id);

            var subject = await mhRepo.AddAsync(new MonHocDto
            {
                MaMonHoc = $"MH_OPS_{tag}",
                TenMonHoc = $"Môn Ops Test {tag}",
                SoBuoiMacDinh = 10,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.MonHocIds.Add(subject.Id);

            await khmRepo.AddAsync(new KhoaHocMonHocDto
            {
                KhoaHocId = course.Id,
                MonHocId = subject.Id,
                ThuTu = 1,
                SoBuoiDuKien = 10
            });
            tracker.KhoaHocMonHocs.Add((course.Id, subject.Id));

            var room = await phongRepo.AddAsync(new PhongHocDto
            {
                MaPhong = $"P_OPS_{tag}",
                TenPhong = $"Phòng Ops Test {tag}",
                SucChua = 30,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.PhongHocIds.Add(room.Id);

            var teacher = await gvRepo.AddAsync(new GiangVienDto
            {
                MaGiangVien = $"GV_OPS_{tag}",
                HoTen = $"Giảng Viên Ops {tag}",
                Email = $"gv.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.GiangVienIds.Add(teacher.Id);

            var student = await hvRepo.AddAsync(new HocVienDto
            {
                MaHocVien = $"HV_OPS_{tag}",
                HoTen = $"Học Viên Ops {tag}",
                Email = $"hv.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.HocVienIds.Add(student.Id);

            // 1. Tạo Lớp học test
            var lop = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L_OPS_{tag}",
                TenLopHoc = $"Lớp Ops Test {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 20,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop.Id);
            Assert.IsGreaterThan(0, lop.Id);

            // 2. Xếp học viên vào lớp (DangKyLopHoc)
            var dk = await dkRepo.AddAsync(new DangKyLopHocDto
            {
                LopHocId = lop.Id,
                HocVienId = student.Id,
                NgayDangKy = DateTime.Today,
                HocPhiPhaiThu = 5000000m,
                GhiChu = $"Ghi chú {tag}",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.DangKyIds.Add(dk.Id);
            Assert.IsGreaterThan(0, dk.Id);

            // 3. Tạo Lịch học test
            var lich = await lichRepo.AddAsync(new LichHocDto
            {
                LopHocId = lop.Id,
                MonHocId = subject.Id,
                GiangVienId = teacher.Id,
                PhongHocId = room.Id,
                NgayHoc = DateTime.Today.AddDays(1),
                GioBatDau = new TimeSpan(8, 0, 0),
                GioKetThuc = new TimeSpan(11, 0, 0),
                NoiDung = $"Nội dung {tag}",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LichHocIds.Add(lich.Id);
            Assert.IsGreaterThan(0, lich.Id);

            // Kiểm tra xung đột phòng
            var hasConflict = await lichRepo.HasConflictPhongAsync(room.Id, DateTime.Today.AddDays(1), new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0));
            Assert.IsTrue(hasConflict, "Phải phát hiện xung đột trùng giờ phòng học");

            // 4. Nhập điểm
            var kq = await kqRepo.UpsertAsync(new KetQuaHocTapDto
            {
                DangKyLopHocId = dk.Id,
                MonHocId = subject.Id,
                DiemTongKet = 9.25m,
                NguoiCapNhatTaiKhoanId = staffId,
                GhiChu = $"Điểm {tag}"
            });
            tracker.KetQuaIds.Add(kq.Id);
            Assert.IsGreaterThan(0, kq.Id);
            Assert.AreEqual(9.25m, kq.DiemTongKet);

            // 5. Lập phiếu thu học phí
            var pt = await ptRepo.AddAsync(new PhieuThuDto
            {
                MaPhieuThu = $"PT_{tag}",
                DangKyLopHocId = dk.Id,
                SoTien = 2500000m,
                HinhThuc = HinhThucThanhToan.CHUYEN_KHOAN,
                NguoiThuTaiKhoanId = staffId,
                GhiChu = $"Thu phí {tag}",
                TrangThai = TrangThaiPhieuThu.HieuLuc
            });
            tracker.PhieuThuIds.Add(pt.Id);
            Assert.IsGreaterThan(0, pt.Id);

            // Kiểm tra tổng tiền đã thu
            var totalPaid = await ptRepo.GetTotalPaidByDangKyIdAsync(dk.Id);
            Assert.AreEqual(2500000m, totalPaid);

            // 6. Kiểm tra Báo cáo
            var bcHocVien = await bcRepo.GetBaoCaoHocVienTheoLopAsync(lop.Id);
            Assert.HasCount(1, bcHocVien);
            Assert.AreEqual(2500000m, bcHocVien[0].DaThu);
            Assert.AreEqual(2500000m, bcHocVien[0].ConNo);

            // 7. Hủy phiếu thu
            var cancelOk = await ptRepo.CancelAsync(pt.Id, "Hủy phiếu kiểm thử", staffId);
            Assert.IsTrue(cancelOk);
            var totalAfterCancel = await ptRepo.GetTotalPaidByDangKyIdAsync(dk.Id);
            Assert.AreEqual(0m, totalAfterCancel);
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task Concurrency_Enrollment_Atomic_ShouldNotExceedCapacity()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);

            var course = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_CAP_{tag}",
                TenKhoaHoc = $"Khóa Capacity {tag}",
                HocPhiNiemYet = 3000000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(course.Id);

            var lop = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L_CAP_{tag}",
                TenLopHoc = $"Lớp Concurrency {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 1,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop.Id);

            var hv1 = await hvRepo.AddAsync(new HocVienDto
            {
                MaHocVien = $"HV1_{tag}",
                HoTen = $"Học Viên 1 {tag}",
                Email = $"test1.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.HocVienIds.Add(hv1.Id);

            var hv2 = await hvRepo.AddAsync(new HocVienDto
            {
                MaHocVien = $"HV2_{tag}",
                HoTen = $"Học Viên 2 {tag}",
                Email = $"test2.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.HocVienIds.Add(hv2.Id);

            var task1 = Task.Run<(bool Success, string? ErrorCode, string? ErrorMessage, DangKyLopHocDto? Data)>(async () =>
            {
                var res = await dkRepo.EnrollAtomicAsync(lop.Id, hv1.Id, 3000000m, $"Enroll {tag}");
                return (res.Success, res.ErrorCode, res.ErrorMessage, res.Data);
            });

            var task2 = Task.Run<(bool Success, string? ErrorCode, string? ErrorMessage, DangKyLopHocDto? Data)>(async () =>
            {
                var res = await dkRepo.EnrollAtomicAsync(lop.Id, hv2.Id, 3000000m, $"Enroll {tag}");
                return (res.Success, res.ErrorCode, res.ErrorMessage, res.Data);
            });

            var results = await Task.WhenAll(task1, task2);
            int successCount = results.Count(r => r.Success);

            foreach (var r in results)
            {
                if (r.Success && r.Data != null)
                {
                    tracker.DangKyIds.Add(r.Data.Id);
                }
            }

            Assert.AreEqual(1, successCount, "Chỉ 1 học viên được xếp vào lớp khi sỉ số tối đa là 1.");

            var failed = results.Single(r => !r.Success);
            Assert.IsNull(failed.Data, "Giao dịch thất bại không được trả về dữ liệu entity.");
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, failed.ErrorCode, "Giao dịch vượt quá sĩ số phải trả về mã lỗi BUSINESS_RULE_VIOLATION.");
            Assert.AreNotEqual(ErrorCodes.UNEXPECTED_ERROR, failed.ErrorCode, "Giao dịch thất bại không được trả về mã UNEXPECTED_ERROR.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(failed.ErrorMessage), "Thông báo lỗi không được rỗng.");

            var activeCount = await dkRepo.CountByLopHocIdAsync(lop.Id);
            Assert.AreEqual(1, activeCount, "Sỉ số thực tế trong database không được vượt quá SiSoToiDa = 1.");
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task Concurrency_Payment_Atomic_ShouldNotExceedRemainingBalance()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);
            var ptRepo = new SqlPhieuThuRepository(_connectionFactory);
            var tkRepo = new SqlTaiKhoanRepository(_connectionFactory);
            var staffId = await CreateTestAccountAsync(_connectionFactory, tracker, tag);

            var course = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_PAY_{tag}",
                TenKhoaHoc = $"Khóa Pay {tag}",
                HocPhiNiemYet = 2000000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(course.Id);

            var lop = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L_PAY_{tag}",
                TenLopHoc = $"Lớp Thu Phí {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 10,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop.Id);

            var hv = await hvRepo.AddAsync(new HocVienDto
            {
                MaHocVien = $"HV_PAY_{tag}",
                HoTen = $"Học Viên Fee {tag}",
                Email = $"testfee.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.HocVienIds.Add(hv.Id);

            var dk = await dkRepo.AddAsync(new DangKyLopHocDto
            {
                LopHocId = lop.Id,
                HocVienId = hv.Id,
                NgayDangKy = DateTime.Today,
                HocPhiPhaiThu = 2000000m,
                GhiChu = $"Fee test {tag}",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.DangKyIds.Add(dk.Id);

            var task1 = Task.Run<(bool Success, string? ErrorCode, string? ErrorMessage, PhieuThuDto? Data)>(async () =>
            {
                var res = await ptRepo.ThuHocPhiAtomicAsync(dk.Id, 1500000m, HinhThucThanhToan.TIEN_MAT, staffId, $"Fee {tag}");
                return (res.Success, res.ErrorCode, res.ErrorMessage, res.Data);
            });

            var task2 = Task.Run<(bool Success, string? ErrorCode, string? ErrorMessage, PhieuThuDto? Data)>(async () =>
            {
                var res = await ptRepo.ThuHocPhiAtomicAsync(dk.Id, 1500000m, HinhThucThanhToan.CHUYEN_KHOAN, staffId, $"Fee {tag}");
                return (res.Success, res.ErrorCode, res.ErrorMessage, res.Data);
            });

            var results = await Task.WhenAll(task1, task2);
            int successCount = results.Count(r => r.Success);

            foreach (var r in results)
            {
                if (r.Success && r.Data != null)
                {
                    tracker.PhieuThuIds.Add(r.Data.Id);
                }
            }

            Assert.AreEqual(1, successCount, "Chỉ 1 giao dịch thu tiền được chấp nhận khi tổng tiền vượt quá số còn nợ.");

            var failed = results.Single(r => !r.Success);
            Assert.IsNull(failed.Data, "Giao dịch thất bại không được trả về dữ liệu entity.");
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, failed.ErrorCode, "Giao dịch vượt quá số tiền còn nợ phải trả về mã lỗi BUSINESS_RULE_VIOLATION.");
            Assert.AreNotEqual(ErrorCodes.UNEXPECTED_ERROR, failed.ErrorCode, "Giao dịch thất bại không được trả về mã UNEXPECTED_ERROR.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(failed.ErrorMessage), "Thông báo lỗi không được rỗng.");

            var totalPaid = await ptRepo.GetTotalPaidByDangKyIdAsync(dk.Id);
            Assert.AreEqual(1500000m, totalPaid, "Tổng tiền thực thu trong DB phải là 1.500.000đ và không bị vượt quá số phải thu.");
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task Concurrency_Schedule_Atomic_ShouldDetectConflict()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var mhRepo = new SqlMonHocRepository(_connectionFactory);
            var khmRepo = new SqlKhoaHocMonHocRepository(_connectionFactory);
            var gvRepo = new SqlGiangVienRepository(_connectionFactory);
            var phongRepo = new SqlPhongHocRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var lichRepo = new SqlLichHocRepository(_connectionFactory);

            // Fixture đầy đủ theo yêu cầu: 1 khóa học, 2 môn học, 2 giảng viên, 1 phòng học, 2 lớp học, liên kết khóa-môn
            var course = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_SCH_{tag}",
                TenKhoaHoc = $"Khóa Schedule {tag}",
                HocPhiNiemYet = 4000000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(course.Id);

            var mh1 = await mhRepo.AddAsync(new MonHocDto
            {
                MaMonHoc = $"MH1_{tag}",
                TenMonHoc = $"Môn Lịch 1 {tag}",
                SoBuoiMacDinh = 10,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.MonHocIds.Add(mh1.Id);

            var mh2 = await mhRepo.AddAsync(new MonHocDto
            {
                MaMonHoc = $"MH2_{tag}",
                TenMonHoc = $"Môn Lịch 2 {tag}",
                SoBuoiMacDinh = 10,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.MonHocIds.Add(mh2.Id);

            await khmRepo.AddAsync(new KhoaHocMonHocDto { KhoaHocId = course.Id, MonHocId = mh1.Id, ThuTu = 1, SoBuoiDuKien = 10 });
            tracker.KhoaHocMonHocs.Add((course.Id, mh1.Id));

            await khmRepo.AddAsync(new KhoaHocMonHocDto { KhoaHocId = course.Id, MonHocId = mh2.Id, ThuTu = 2, SoBuoiDuKien = 10 });
            tracker.KhoaHocMonHocs.Add((course.Id, mh2.Id));

            var gv1 = await gvRepo.AddAsync(new GiangVienDto
            {
                MaGiangVien = $"GV1_{tag}",
                HoTen = $"Giảng Viên 1 {tag}",
                Email = $"gv1.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.GiangVienIds.Add(gv1.Id);

            var gv2 = await gvRepo.AddAsync(new GiangVienDto
            {
                MaGiangVien = $"GV2_{tag}",
                HoTen = $"Giảng Viên 2 {tag}",
                Email = $"gv2.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.GiangVienIds.Add(gv2.Id);

            var room = await phongRepo.AddAsync(new PhongHocDto
            {
                MaPhong = $"P_SCH_{tag}",
                TenPhong = $"Phòng Học Trùng {tag}",
                SucChua = 40,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.PhongHocIds.Add(room.Id);

            var lop1 = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L1_{tag}",
                TenLopHoc = $"Lớp Lịch 1 {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 20,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop1.Id);

            var lop2 = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L2_{tag}",
                TenLopHoc = $"Lớp Lịch 2 {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 20,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop2.Id);

            var testDate = DateTime.Today.AddDays(7);

            // 2 giao dịch thêm lịch cùng phòng, giao nhau thời gian (08:00 - 11:00 và 09:00 - 12:00)
            var task1 = Task.Run<(bool Success, string? ErrorCode, LichHocDto? Data)>(async () =>
            {
                var res = await lichRepo.CreateAtomicAsync(new LichHocDto
                {
                    LopHocId = lop1.Id,
                    MonHocId = mh1.Id,
                    GiangVienId = gv1.Id,
                    PhongHocId = room.Id,
                    NgayHoc = testDate,
                    GioBatDau = new TimeSpan(8, 0, 0),
                    GioKetThuc = new TimeSpan(11, 0, 0),
                    NoiDung = $"Lịch 1 {tag}",
                    TrangThai = TrangThaiHoatDong.DangHoatDong
                });
                return (res.Success, res.ErrorCode, res.Data);
            });

            var task2 = Task.Run<(bool Success, string? ErrorCode, LichHocDto? Data)>(async () =>
            {
                var res = await lichRepo.CreateAtomicAsync(new LichHocDto
                {
                    LopHocId = lop2.Id,
                    MonHocId = mh2.Id,
                    GiangVienId = gv2.Id,
                    PhongHocId = room.Id, // Cùng phòng học
                    NgayHoc = testDate,
                    GioBatDau = new TimeSpan(9, 0, 0), // Trùng khoảng 09:00 - 11:00
                    GioKetThuc = new TimeSpan(12, 0, 0),
                    NoiDung = $"Lịch 2 {tag}",
                    TrangThai = TrangThaiHoatDong.DangHoatDong
                });
                return (res.Success, res.ErrorCode, res.Data);
            });

            var results = await Task.WhenAll(task1, task2);
            int successCount = results.Count(r => r.Success);

            foreach (var r in results)
            {
                if (r.Success && r.Data != null)
                {
                    tracker.LichHocIds.Add(r.Data.Id);
                }
            }

            Assert.AreEqual(1, successCount, "Chỉ 1 lịch học được tạo thành công khi có xung đột phòng học.");
            var failedResult = results.First(r => !r.Success);
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, failedResult.ErrorCode, "Giao dịch xung đột phải trả về mã lỗi TIME_CONFLICT.");
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task CancelEnrollment_FailsIfActiveReceiptExists()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);
            var ptRepo = new SqlPhieuThuRepository(_connectionFactory);
            var tkRepo = new SqlTaiKhoanRepository(_connectionFactory);

            var staffId = await CreateTestAccountAsync(_connectionFactory, tracker, tag);

            var course = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_CAN_{tag}",
                TenKhoaHoc = $"Khóa Cancel {tag}",
                HocPhiNiemYet = 2000000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(course.Id);

            var lop = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L_CAN_{tag}",
                TenLopHoc = $"Lớp Hủy ĐK {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 20,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop.Id);

            var hv = await hvRepo.AddAsync(new HocVienDto
            {
                MaHocVien = $"HV_CAN_{tag}",
                HoTen = $"Học Viên Hủy {tag}",
                Email = $"testhuy.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.HocVienIds.Add(hv.Id);

            var dk = await dkRepo.AddAsync(new DangKyLopHocDto
            {
                LopHocId = lop.Id,
                HocVienId = hv.Id,
                NgayDangKy = DateTime.Today,
                HocPhiPhaiThu = 2000000m,
                GhiChu = $"Cancel test {tag}",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.DangKyIds.Add(dk.Id);

            var pt = await ptRepo.AddAsync(new PhieuThuDto
            {
                MaPhieuThu = $"PT_CAN_{tag}",
                DangKyLopHocId = dk.Id,
                SoTien = 1000000m,
                HinhThuc = HinhThucThanhToan.TIEN_MAT,
                NguoiThuTaiKhoanId = staffId,
                GhiChu = $"Receipt {tag}",
                TrangThai = TrangThaiPhieuThu.HieuLuc
            });
            tracker.PhieuThuIds.Add(pt.Id);

            // 1. Huỷ đăng ký lớp học -> Phải thất bại vì đã có phiếu thu còn hiệu lực
            var cancelFail = await dkRepo.CancelEnrollmentAtomicAsync(dk.Id);
            Assert.IsFalse(cancelFail.Success, "Không được phép huỷ đăng ký lớp học khi còn phiếu thu hiệu lực.");
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, cancelFail.ErrorCode);

            // 2. Huỷ phiếu thu trước
            var cancelReceiptOk = await ptRepo.CancelAsync(pt.Id, "Huỷ phiếu để huỷ đăng ký", staffId);
            Assert.IsTrue(cancelReceiptOk);

            // 3. Huỷ đăng ký lớp học lại -> Phải thành công
            var cancelSuccess = await dkRepo.CancelEnrollmentAtomicAsync(dk.Id);
            Assert.IsTrue(cancelSuccess.Success, "Huỷ đăng ký thành công sau khi tất cả phiếu thu đã bị huỷ.");
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task Concurrency_ReceiptCancellation_Atomic_OnlyOneSucceeds()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);
            var ptRepo = new SqlPhieuThuRepository(_connectionFactory);
            var tkRepo = new SqlTaiKhoanRepository(_connectionFactory);

            var staffId = await CreateTestAccountAsync(_connectionFactory, tracker, tag);

            var course = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_CNC_{tag}",
                TenKhoaHoc = $"Khóa Concurrent Cancel {tag}",
                HocPhiNiemYet = 1000000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(course.Id);

            var lop = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L_CNC_{tag}",
                TenLopHoc = $"Lớp Cancel {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 10,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop.Id);

            var hv = await hvRepo.AddAsync(new HocVienDto
            {
                MaHocVien = $"HV_CNC_{tag}",
                HoTen = $"Học Viên Cancel {tag}",
                Email = $"testcnc.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.HocVienIds.Add(hv.Id);

            var dk = await dkRepo.AddAsync(new DangKyLopHocDto
            {
                LopHocId = lop.Id,
                HocVienId = hv.Id,
                NgayDangKy = DateTime.Today,
                HocPhiPhaiThu = 1000000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.DangKyIds.Add(dk.Id);

            var pt = await ptRepo.AddAsync(new PhieuThuDto
            {
                MaPhieuThu = $"PT_CNC_{tag}",
                DangKyLopHocId = dk.Id,
                SoTien = 500000m,
                HinhThuc = HinhThucThanhToan.TIEN_MAT,
                NguoiThuTaiKhoanId = staffId,
                TrangThai = TrangThaiPhieuThu.HieuLuc
            });
            tracker.PhieuThuIds.Add(pt.Id);

            // Hai task huỷ cùng lúc phiếu thu
            var task1 = Task.Run(() => ptRepo.CancelAsync(pt.Id, "Hủy luồng 1", staffId));
            var task2 = Task.Run(() => ptRepo.CancelAsync(pt.Id, "Hủy luồng 2", staffId));

            var results = await Task.WhenAll(task1, task2);
            int successCount = results.Count(r => r);

            // Đúng 1 luồng thành công, 1 luồng trả về false vì WHERE Id = @Id AND TrangThai = 1
            Assert.AreEqual(1, successCount, "Chỉ đúng 1 luồng hủy thành công phiếu thu.");

            var inDb = await ptRepo.GetByIdAsync(pt.Id);
            Assert.IsNotNull(inDb);
            Assert.AreEqual(TrangThaiPhieuThu.DaHuy, inDb.TrangThai);
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task SqlKetQuaHocTap_BlankVsZeroGrade_AndBatchAtomic()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var mhRepo = new SqlMonHocRepository(_connectionFactory);
            var khmRepo = new SqlKhoaHocMonHocRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);
            var kqRepo = new SqlKetQuaHocTapRepository(_connectionFactory);
            var tkRepo = new SqlTaiKhoanRepository(_connectionFactory);

            var staffId = await CreateTestAccountAsync(_connectionFactory, tracker, tag);

            var course = await khRepo.AddAsync(new KhoaHocDto { MaKhoaHoc = $"KH_GR_{tag}", TenKhoaHoc = $"Khóa Grade {tag}", HocPhiNiemYet = 1000000m });
            tracker.KhoaHocIds.Add(course.Id);

            var mh = await mhRepo.AddAsync(new MonHocDto { MaMonHoc = $"MH_GR_{tag}", TenMonHoc = $"Môn Grade {tag}", SoBuoiMacDinh = 10 });
            tracker.MonHocIds.Add(mh.Id);

            await khmRepo.AddAsync(new KhoaHocMonHocDto { KhoaHocId = course.Id, MonHocId = mh.Id, ThuTu = 1, SoBuoiDuKien = 10 });
            tracker.KhoaHocMonHocs.Add((course.Id, mh.Id));

            var lop = await lopRepo.AddAsync(new LopHocDto { MaLopHoc = $"L_GR_{tag}", TenLopHoc = $"Lớp Grade {tag}", KhoaHocId = course.Id, NgayBatDau = DateTime.Today, SiSoToiDa = 10 });
            tracker.LopHocIds.Add(lop.Id);

            var hv1 = await hvRepo.AddAsync(new HocVienDto { MaHocVien = $"HV1_GR_{tag}", HoTen = $"Học Viên 1 {tag}" });
            tracker.HocVienIds.Add(hv1.Id);

            var hv2 = await hvRepo.AddAsync(new HocVienDto { MaHocVien = $"HV2_GR_{tag}", HoTen = $"Học Viên 2 {tag}" });
            tracker.HocVienIds.Add(hv2.Id);

            var dk1 = await dkRepo.AddAsync(new DangKyLopHocDto { LopHocId = lop.Id, HocVienId = hv1.Id, NgayDangKy = DateTime.Today, HocPhiPhaiThu = 1000000m });
            tracker.DangKyIds.Add(dk1.Id);

            var dk2 = await dkRepo.AddAsync(new DangKyLopHocDto { LopHocId = lop.Id, HocVienId = hv2.Id, NgayDangKy = DateTime.Today, HocPhiPhaiThu = 1000000m });
            tracker.DangKyIds.Add(dk2.Id);

            // 1. Nhập điểm 0.00 cho học viên 1 -> Hợp lệ và lưu được
            var grade0 = await kqRepo.UpsertAsync(new KetQuaHocTapDto { DangKyLopHocId = dk1.Id, MonHocId = mh.Id, DiemTongKet = 0.00m, NguoiCapNhatTaiKhoanId = staffId });
            tracker.KetQuaIds.Add(grade0.Id);
            Assert.IsGreaterThan(0, grade0.Id);
            Assert.AreEqual(0.00m, grade0.DiemTongKet);

            var loadedGrade0 = await kqRepo.GetByDangKyAndMonAsync(dk1.Id, mh.Id);
            Assert.IsNotNull(loadedGrade0);
            Assert.AreEqual(0.00m, loadedGrade0.DiemTongKet);

            // 2. Xóa điểm học viên 1 (blank semantics)
            var deleted = await kqRepo.DeleteByDangKyAndMonAsync(dk1.Id, mh.Id);
            Assert.IsTrue(deleted);
            var loadedAfterDel = await kqRepo.GetByDangKyAndMonAsync(dk1.Id, mh.Id);
            Assert.IsNull(loadedAfterDel);

            // 3. Batch atomic: nhập điểm cho cả 2 học viên
            var toUpsert = new List<KetQuaHocTapDto>
            {
                new KetQuaHocTapDto { DangKyLopHocId = dk1.Id, MonHocId = mh.Id, DiemTongKet = 8.50m, NguoiCapNhatTaiKhoanId = staffId },
                new KetQuaHocTapDto { DangKyLopHocId = dk2.Id, MonHocId = mh.Id, DiemTongKet = 9.00m, NguoiCapNhatTaiKhoanId = staffId }
            };

            var batchCount = await kqRepo.SaveGradesBatchAtomicAsync(toUpsert, Enumerable.Empty<int>(), mh.Id);
            Assert.AreEqual(2, batchCount);

            var g1 = await kqRepo.GetByDangKyAndMonAsync(dk1.Id, mh.Id);
            var g2 = await kqRepo.GetByDangKyAndMonAsync(dk2.Id, mh.Id);
            Assert.IsNotNull(g1);
            Assert.IsNotNull(g2);
            tracker.KetQuaIds.Add(g1.Id);
            tracker.KetQuaIds.Add(g2.Id);
            Assert.AreEqual(8.50m, g1.DiemTongKet);
            Assert.AreEqual(9.00m, g2.DiemTongKet);

            // 4. Batch atomic kết hợp xóa điểm hv1 và cập nhật hv2
            var toUpsert2 = new List<KetQuaHocTapDto>
            {
                new KetQuaHocTapDto { DangKyLopHocId = dk2.Id, MonHocId = mh.Id, DiemTongKet = 9.50m, NguoiCapNhatTaiKhoanId = staffId }
            };
            var toDelete2 = new List<int> { dk1.Id };

            await kqRepo.SaveGradesBatchAtomicAsync(toUpsert2, toDelete2, mh.Id);

            var g1After = await kqRepo.GetByDangKyAndMonAsync(dk1.Id, mh.Id);
            var g2After = await kqRepo.GetByDangKyAndMonAsync(dk2.Id, mh.Id);
            Assert.IsNull(g1After, "Học viên 1 phải bị xóa điểm.");
            Assert.IsNotNull(g2After);
            Assert.AreEqual(9.50m, g2After.DiemTongKet);
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task SqlLopHoc_UpdateAtomicAsync_AllInvariantsAndEdgeCases()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var mhRepo = new SqlMonHocRepository(_connectionFactory);
            var khmRepo = new SqlKhoaHocMonHocRepository(_connectionFactory);
            var phongRepo = new SqlPhongHocRepository(_connectionFactory);
            var gvRepo = new SqlGiangVienRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var lichRepo = new SqlLichHocRepository(_connectionFactory);

            // Fixture: Khóa 1 & 2
            var kh1 = await khRepo.AddAsync(new KhoaHocDto { MaKhoaHoc = $"K1_{tag}", TenKhoaHoc = $"Khóa 1 {tag}", HocPhiNiemYet = 1000000m });
            tracker.KhoaHocIds.Add(kh1.Id);
            var kh2 = await khRepo.AddAsync(new KhoaHocDto { MaKhoaHoc = $"K2_{tag}", TenKhoaHoc = $"Khóa 2 {tag}", HocPhiNiemYet = 1500000m });
            tracker.KhoaHocIds.Add(kh2.Id);

            var mh = await mhRepo.AddAsync(new MonHocDto { MaMonHoc = $"M1_{tag}", TenMonHoc = $"Môn 1 {tag}", SoBuoiMacDinh = 10 });
            tracker.MonHocIds.Add(mh.Id);

            await khmRepo.AddAsync(new KhoaHocMonHocDto { KhoaHocId = kh1.Id, MonHocId = mh.Id, ThuTu = 1, SoBuoiDuKien = 10 });
            tracker.KhoaHocMonHocs.Add((kh1.Id, mh.Id));

            var gv = await gvRepo.AddAsync(new GiangVienDto { MaGiangVien = $"GV_{tag}", HoTen = $"GV {tag}", Email = $"gv.{tag}@test.com" });
            tracker.GiangVienIds.Add(gv.Id);

            // Phòng học sức chứa 30
            var room = await phongRepo.AddAsync(new PhongHocDto { MaPhong = $"P_{tag}", TenPhong = $"Phòng {tag}", SucChua = 30 });
            tracker.PhongHocIds.Add(room.Id);

            // Lớp học ban đầu: từ ngày T+1 đến T+30, sĩ số tối đa 25
            var lop = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L_UPD_{tag}",
                TenLopHoc = $"Lớp Update {tag}",
                KhoaHocId = kh1.Id,
                NgayBatDau = DateTime.Today.AddDays(1),
                NgayKetThuc = DateTime.Today.AddDays(30),
                SiSoToiDa = 25
            });
            tracker.LopHocIds.Add(lop.Id);

            // Thêm 1 lịch học active vào ngày T+10
            var schRes = await lichRepo.CreateAtomicAsync(new LichHocDto
            {
                LopHocId = lop.Id,
                MonHocId = mh.Id,
                GiangVienId = gv.Id,
                PhongHocId = room.Id,
                NgayHoc = DateTime.Today.AddDays(10),
                GioBatDau = new TimeSpan(8, 0, 0),
                GioKetThuc = new TimeSpan(11, 0, 0),
                NoiDung = "Lịch ban đầu"
            });
            Assert.IsTrue(schRes.Success);
            tracker.LichHocIds.Add(schRes.Data!.Id);

            // 1. Sửa tên lớp thông thường thành công (khi đã có lịch học hợp lệ)
            lop.TenLopHoc = $"Lớp Update Đã Đổi Tên {tag}";
            var updNormalRes = await lopRepo.UpdateAtomicAsync(lop);
            Assert.IsTrue(updNormalRes.Success, "Sửa tên lớp khi có lịch học hợp lệ phải thành công.");

            // 2. Thu hẹp ngày làm lịch hiện tại (T+10) nằm ngoài khoảng -> Bị từ chối
            lop.NgayBatDau = DateTime.Today.AddDays(15); // Lịch ở ngày 10 < 15
            var updNarrowRes = await lopRepo.UpdateAtomicAsync(lop);
            Assert.IsFalse(updNarrowRes.Success, "Thu hẹp ngày khiến lịch học nằm ngoài khoảng phải bị từ chối.");
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, updNarrowRes.ErrorCode);

            // Trả lại ngày hợp lệ
            lop.NgayBatDau = DateTime.Today.AddDays(1);

            // 3. Tăng sĩ số vượt quá sức chứa phòng (Phòng sức chứa 30, tăng sĩ số lên 35) -> Bị từ chối
            lop.SiSoToiDa = 35;
            var updCapRes = await lopRepo.UpdateAtomicAsync(lop);
            Assert.IsFalse(updCapRes.Success, "Sĩ số lớp vượt quá sức chứa phòng của lịch active phải bị từ chối.");
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, updCapRes.ErrorCode);

            // Trả lại sĩ số hợp lệ
            lop.SiSoToiDa = 25;

            // 4. Đổi khóa học (sang Khóa 2) khi đã có lịch sử lịch học -> Bị từ chối
            lop.KhoaHocId = kh2.Id;
            var updCourseRes = await lopRepo.UpdateAtomicAsync(lop);
            Assert.IsFalse(updCourseRes.Success, "Đổi khóa học khi đã có lịch học/đăng ký trong lịch sử phải bị từ chối.");
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, updCourseRes.ErrorCode);
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task SqlLopHoc_DeactivateAtomic_AllCasesAndConcurrency()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var mhRepo = new SqlMonHocRepository(_connectionFactory);
            var khmRepo = new SqlKhoaHocMonHocRepository(_connectionFactory);
            var phongRepo = new SqlPhongHocRepository(_connectionFactory);
            var gvRepo = new SqlGiangVienRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);
            var lichRepo = new SqlLichHocRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);

            var course = await khRepo.AddAsync(new KhoaHocDto { MaKhoaHoc = $"KH_DEA_{tag}", TenKhoaHoc = $"Khóa Deact {tag}", HocPhiNiemYet = 1000000m });
            tracker.KhoaHocIds.Add(course.Id);

            var mh = await mhRepo.AddAsync(new MonHocDto { MaMonHoc = $"MH_DEA_{tag}", TenMonHoc = $"Môn Deact {tag}", SoBuoiMacDinh = 10 });
            tracker.MonHocIds.Add(mh.Id);

            await khmRepo.AddAsync(new KhoaHocMonHocDto { KhoaHocId = course.Id, MonHocId = mh.Id, ThuTu = 1, SoBuoiDuKien = 10 });
            tracker.KhoaHocMonHocs.Add((course.Id, mh.Id));

            var gv = await gvRepo.AddAsync(new GiangVienDto { MaGiangVien = $"GV_DEA_{tag}", HoTen = $"GV Deact {tag}", Email = $"gv.dea.{tag}@test.com" });
            tracker.GiangVienIds.Add(gv.Id);

            var room = await phongRepo.AddAsync(new PhongHocDto { MaPhong = $"P_DEA_{tag}", TenPhong = $"Phòng Deact {tag}", SucChua = 30 });
            tracker.PhongHocIds.Add(room.Id);

            // 1. Lớp hoàn toàn trống (không học viên, không lịch) -> Ngừng thành công
            var lopEmpty = await lopRepo.AddAsync(new LopHocDto { MaLopHoc = $"L_MT_{tag}", TenLopHoc = $"Lớp Trống {tag}", KhoaHocId = course.Id, NgayBatDau = DateTime.Today, SiSoToiDa = 20 });
            tracker.LopHocIds.Add(lopEmpty.Id);

            var deactEmptyRes = await lopRepo.DeactivateAtomicAsync(lopEmpty.Id);
            Assert.IsTrue(deactEmptyRes.Success, "Lớp không có học viên/lịch phải ngừng hoạt động thành công.");

            // 2. Lớp chỉ có học viên (không có lịch) -> Bị chặn
            var lopStuOnly = await lopRepo.AddAsync(new LopHocDto { MaLopHoc = $"L_STU_{tag}", TenLopHoc = $"Lớp Học Viên {tag}", KhoaHocId = course.Id, NgayBatDau = DateTime.Today, SiSoToiDa = 20 });
            tracker.LopHocIds.Add(lopStuOnly.Id);

            var hv = await hvRepo.AddAsync(new HocVienDto { MaHocVien = $"HV_D_{tag}", HoTen = $"HV Deact {tag}", Email = $"hv.dea.{tag}@test.com" });
            tracker.HocVienIds.Add(hv.Id);

            var dkRes = await dkRepo.EnrollAtomicAsync(lopStuOnly.Id, hv.Id, 1000000m, "Enroll test");
            Assert.IsTrue(dkRes.Success);
            tracker.DangKyIds.Add(dkRes.Data!.Id);

            var deactStuRes = await lopRepo.DeactivateAtomicAsync(lopStuOnly.Id);
            Assert.IsFalse(deactStuRes.Success, "Lớp còn học viên active phải bị từ chối ngừng hoạt động.");
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, deactStuRes.ErrorCode);

            // 3. Lớp KHÔNG có học viên nhưng CÓ lịch học active -> Bị chặn (chứng minh guard lịch chạy độc lập)
            var lopSchOnly = await lopRepo.AddAsync(new LopHocDto { MaLopHoc = $"L_SCH_{tag}", TenLopHoc = $"Lớp Lịch {tag}", KhoaHocId = course.Id, NgayBatDau = DateTime.Today, SiSoToiDa = 20 });
            tracker.LopHocIds.Add(lopSchOnly.Id);

            var schRes = await lichRepo.CreateAtomicAsync(new LichHocDto
            {
                LopHocId = lopSchOnly.Id,
                MonHocId = mh.Id,
                GiangVienId = gv.Id,
                PhongHocId = room.Id,
                NgayHoc = DateTime.Today.AddDays(2),
                GioBatDau = new TimeSpan(8, 0, 0),
                GioKetThuc = new TimeSpan(11, 0, 0),
                NoiDung = "Lịch độc lập"
            });
            Assert.IsTrue(schRes.Success);
            tracker.LichHocIds.Add(schRes.Data!.Id);

            var deactSchRes = await lopRepo.DeactivateAtomicAsync(lopSchOnly.Id);
            Assert.IsFalse(deactSchRes.Success, "Lớp không có học viên nhưng còn lịch active phải bị từ chối ngừng hoạt động.");
            Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, deactSchRes.ErrorCode);

            // 4. Concurrency: 1 luồng Deactivate lớp và 1 luồng Enroll học viên đồng thời
            // Không được tạo ra lớp inactive mà có học viên active
            var lopRace = await lopRepo.AddAsync(new LopHocDto { MaLopHoc = $"L_RC_{tag}", TenLopHoc = $"Lớp Race {tag}", KhoaHocId = course.Id, NgayBatDau = DateTime.Today, SiSoToiDa = 20 });
            tracker.LopHocIds.Add(lopRace.Id);

            var hvRace = await hvRepo.AddAsync(new HocVienDto { MaHocVien = $"HV_RC_{tag}", HoTen = $"HV Race {tag}", Email = $"hv.rc.{tag}@test.com" });
            tracker.HocVienIds.Add(hvRace.Id);

            var taskDeact = Task.Run(() => lopRepo.DeactivateAtomicAsync(lopRace.Id));
            var taskEnroll = Task.Run(() => dkRepo.EnrollAtomicAsync(lopRace.Id, hvRace.Id, 1000000m, "Enroll Race"));

            await Task.WhenAll(taskDeact, taskEnroll);
            var deactOut = await taskDeact;
            var enrollOut = await taskEnroll;

            if (enrollOut.Success && enrollOut.Data != null)
            {
                tracker.DangKyIds.Add(enrollOut.Data.Id);
            }

            // Kiểm tra trạng thái DB: không bao giờ tồn tại (Lớp inactive AND có học viên active)
            var lopDb = await lopRepo.GetByIdAsync(lopRace.Id);
            Assert.IsNotNull(lopDb);
            var countActiveStudents = await dkRepo.CountByLopHocIdAsync(lopRace.Id);

            if (lopDb.TrangThai == TrangThaiHoatDong.NgungHoatDong)
            {
                Assert.AreEqual(0, countActiveStudents, "Nếu lớp đã ngừng hoạt động thì tuyệt đối không được có học viên active.");
                Assert.IsFalse(enrollOut.Success, "Ghi danh phải thất bại nếu lớp ngừng hoạt động trước.");
            }
            else
            {
                Assert.AreEqual(1, countActiveStudents, "Nếu ghi danh thành công thì lớp phải còn hoạt động.");
                Assert.IsFalse(deactOut.Success, "Ngừng hoạt động lớp phải bị chặn khi đã có học viên ghi danh.");
            }
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task SqlDangKyLopHoc_CourseMapping_AndStudentHistory()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);

            var course = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_MAP_{tag}",
                TenKhoaHoc = $"Khóa Học Mapping {tag}",
                HocPhiNiemYet = 4500000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(course.Id);

            var lop = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L_MAP_{tag}",
                TenLopHoc = $"Lớp Học Mapping {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 20,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop.Id);

            var hv = await hvRepo.AddAsync(new HocVienDto
            {
                MaHocVien = $"HV_MAP_{tag}",
                HoTen = $"Học Viên Mapping {tag}",
                Email = $"hv.map.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.HocVienIds.Add(hv.Id);

            var enrollRes = await dkRepo.EnrollAtomicAsync(lop.Id, hv.Id, 4500000m, $"Test Mapping {tag}");
            Assert.IsTrue(enrollRes.Success);
            Assert.IsNotNull(enrollRes.Data);
            tracker.DangKyIds.Add(enrollRes.Data.Id);

            // Kiểm tra 1: GetByIdAsync trả về đúng KhoaHocId và TenKhoaHoc qua INNER JOIN dbo.KhoaHoc
            var byId = await dkRepo.GetByIdAsync(enrollRes.Data.Id);
            Assert.IsNotNull(byId);
            Assert.AreEqual(course.Id, byId.KhoaHocId, "DangKyLopHocDto.KhoaHocId phải được join và map chính xác.");
            Assert.AreEqual(course.TenKhoaHoc, byId.TenKhoaHoc, "DangKyLopHocDto.TenKhoaHoc phải được join và map chính xác.");

            // Kiểm tra 2: GetByLopAndHocVienAsync trả về đúng KhoaHocId và TenKhoaHoc
            var byLopHv = await dkRepo.GetByLopAndHocVienAsync(lop.Id, hv.Id);
            Assert.IsNotNull(byLopHv);
            Assert.AreEqual(course.Id, byLopHv.KhoaHocId);
            Assert.AreEqual(course.TenKhoaHoc, byLopHv.TenKhoaHoc);

            // Kiểm tra 3: GetAllAsync lọc theo HocVienId trả về lịch sử học tập chứa đầy đủ thông tin khóa học
            var history = await dkRepo.GetAllAsync(new DangKyLopHocFilter { HocVienId = hv.Id });
            var matched = history.FirstOrDefault(h => h.Id == enrollRes.Data.Id);
            Assert.IsNotNull(matched);
            Assert.AreEqual(course.Id, matched.KhoaHocId);
            Assert.AreEqual(course.TenKhoaHoc, matched.TenKhoaHoc);
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task SqlDangKyLopHoc_EnrollAtomic_Returns_CourseContract()
    {
        var tracker = new SqlTestTracker();
        try
        {
            var tag = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var khRepo = new SqlKhoaHocRepository(_connectionFactory);
            var lopRepo = new SqlLopHocRepository(_connectionFactory);
            var hvRepo = new SqlHocVienRepository(_connectionFactory);
            var dkRepo = new SqlDangKyLopHocRepository(_connectionFactory);
            var ptRepo = new SqlPhieuThuRepository(_connectionFactory);

            var staffId = await CreateTestAccountAsync(_connectionFactory, tracker, tag);

            var course = await khRepo.AddAsync(new KhoaHocDto
            {
                MaKhoaHoc = $"KH_EA_{tag}",
                TenKhoaHoc = $"Khóa Enroll Contract {tag}",
                HocPhiNiemYet = 3500000m,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.KhoaHocIds.Add(course.Id);

            var lop = await lopRepo.AddAsync(new LopHocDto
            {
                MaLopHoc = $"L_EA_{tag}",
                TenLopHoc = $"Lớp Enroll Contract {tag}",
                KhoaHocId = course.Id,
                NgayBatDau = DateTime.Today,
                SiSoToiDa = 20,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.LopHocIds.Add(lop.Id);

            var hv = await hvRepo.AddAsync(new HocVienDto
            {
                MaHocVien = $"HV_EA_{tag}",
                HoTen = $"Học Viên Contract {tag}",
                Email = $"hv.ea.{tag}@example.com",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });
            tracker.HocVienIds.Add(hv.Id);

            // Test EnrollAtomicAsync immediately returns KhoaHocId and TenKhoaHoc
            var enrollRes = await dkRepo.EnrollAtomicAsync(lop.Id, hv.Id, 3500000m, "Enroll Contract Test");
            Assert.IsTrue(enrollRes.Success);
            Assert.IsNotNull(enrollRes.Data);
            tracker.DangKyIds.Add(enrollRes.Data.Id);

            Assert.AreEqual(course.Id, enrollRes.Data.KhoaHocId, "EnrollAtomicAsync phải trả về KhoaHocId ngay lập tức.");
            Assert.AreEqual(course.TenKhoaHoc, enrollRes.Data.TenKhoaHoc, "EnrollAtomicAsync phải trả về TenKhoaHoc ngay lập tức.");

            // Test ThuHocPhiAtomicAsync immediately returns KhoaHocId and TenKhoaHoc
            var payRes = await ptRepo.ThuHocPhiAtomicAsync(enrollRes.Data.Id, 1000000m, HinhThucThanhToan.TIEN_MAT, staffId, "Thu phí Contract Test");
            Assert.IsTrue(payRes.Success);
            Assert.IsNotNull(payRes.Data);
            tracker.PhieuThuIds.Add(payRes.Data.Id);

            Assert.AreEqual(course.Id, payRes.Data.KhoaHocId, "ThuHocPhiAtomicAsync phải trả về KhoaHocId ngay lập tức.");
            Assert.AreEqual(course.TenKhoaHoc, payRes.Data.TenKhoaHoc, "ThuHocPhiAtomicAsync phải trả về TenKhoaHoc ngay lập tức.");
        }
        finally
        {
            await tracker.CleanupAsync(_connectionFactory);
        }
    }

    [TestMethod]
    public async Task StartupConnectionTester_Migration006_Passes_On_Migrated_Database()
    {
        await using var conn = (SqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync();

        var result = StartupConnectionTester.CheckMigration006(conn, "sanitized");
        Assert.IsTrue(result.IsSuccess, $"CheckMigration006 phải thành công trên CSDL đã chạy migration: {result.ErrorMessage}");
    }
}
