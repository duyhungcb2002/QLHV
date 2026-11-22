using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Features.Catalog.Services;

namespace QLHV.Tests;

/// <summary>
/// Kiểm thử phân quyền trực tiếp tại tầng Service cho toàn bộ 6 Service Catalog:
/// - Học viên (HocVienService)
/// - Giảng viên (GiangVienService)
/// - Môn học (MonHocService)
/// - Khóa học (KhoaHocService)
/// - Khóa học - Môn học (KhoaHocMonHocService)
/// - Phòng học (PhongHocService)
/// </summary>
[TestClass]
public class ServiceAuthorizationTests
{
    private IAppLogger _logger = null!;
    private IAuthorizationService _authzService = null!;
    private ICurrentUserService _currentUserService = null!;

    private FakeHocVienRepository _hocVienRepo = null!;
    private FakeTaiKhoanRepository _taiKhoanRepo = null!;
    private HocVienService _hocVienService = null!;

    private FakeGiangVienRepository _giangVienRepo = null!;
    private GiangVienService _giangVienService = null!;

    private FakeMonHocRepository _monHocRepo = null!;
    private MonHocService _monHocService = null!;

    private FakeKhoaHocRepository _khoaHocRepo = null!;
    private KhoaHocService _khoaHocService = null!;

    private FakeKhoaHocMonHocRepository _khoaHocMonHocRepo = null!;
    private KhoaHocMonHocService _khoaHocMonHocService = null!;

    private FakePhongHocRepository _phongHocRepo = null!;
    private PhongHocService _phongHocService = null!;

    [TestInitialize]
    public void Setup()
    {
        _logger = new FileAppLogger();
        _authzService = new AuthorizationService();
        _currentUserService = new CurrentUserService(_authzService);

        _hocVienRepo = new FakeHocVienRepository();
        _taiKhoanRepo = new FakeTaiKhoanRepository(new Pbkdf2PasswordHasher());
        _hocVienService = new HocVienService(_hocVienRepo, _currentUserService, _logger, _taiKhoanRepo);

        _giangVienRepo = new FakeGiangVienRepository();
        _giangVienService = new GiangVienService(_giangVienRepo, _currentUserService, _logger);

        _monHocRepo = new FakeMonHocRepository();
        _monHocService = new MonHocService(_monHocRepo, _currentUserService, _logger);

        _khoaHocRepo = new FakeKhoaHocRepository();
        _khoaHocService = new KhoaHocService(_khoaHocRepo, _currentUserService, _logger);

        _khoaHocMonHocRepo = new FakeKhoaHocMonHocRepository();
        _khoaHocMonHocService = new KhoaHocMonHocService(_khoaHocMonHocRepo, _khoaHocRepo, _monHocRepo, _currentUserService, _logger);

        _phongHocRepo = new FakePhongHocRepository();
        _phongHocService = new PhongHocService(_phongHocRepo, _currentUserService, _logger);
    }

    private void SetUser(VaiTro role)
    {
        _currentUserService.SetCurrentUser(new TaiKhoanDto
        {
            Id = (int)role,
            TenDangNhap = role.ToString().ToLower(),
            VaiTro = role
        });
    }

    private void ClearUser()
    {
        _currentUserService.Clear();
    }

    #region 1. Chưa đăng nhập (Unauthenticated)

    [TestMethod]
    public async Task Unauthenticated_CannotRead_All6Services_ReturnsForbidden()
    {
        ClearUser();

        // 1. HocVien
        var hvGetAll = await _hocVienService.GetAllAsync();
        AssertForbiddenUnauthenticated(hvGetAll);
        var hvGetById = await _hocVienService.GetByIdAsync(1);
        AssertForbiddenUnauthenticated(hvGetById);

        // 2. GiangVien
        var gvGetAll = await _giangVienService.GetAllAsync();
        AssertForbiddenUnauthenticated(gvGetAll);
        var gvGetById = await _giangVienService.GetByIdAsync(1);
        AssertForbiddenUnauthenticated(gvGetById);

        // 3. MonHoc
        var mhGetAll = await _monHocService.GetAllAsync();
        AssertForbiddenUnauthenticated(mhGetAll);
        var mhGetById = await _monHocService.GetByIdAsync(1);
        AssertForbiddenUnauthenticated(mhGetById);

        // 4. KhoaHoc
        var khGetAll = await _khoaHocService.GetAllAsync();
        AssertForbiddenUnauthenticated(khGetAll);
        var khGetById = await _khoaHocService.GetByIdAsync(1);
        AssertForbiddenUnauthenticated(khGetById);

        // 5. KhoaHocMonHoc
        var khmhGet = await _khoaHocMonHocService.GetByKhoaHocIdAsync(1);
        AssertForbiddenUnauthenticated(khmhGet);

        // 6. PhongHoc
        var phGetAll = await _phongHocService.GetAllAsync();
        AssertForbiddenUnauthenticated(phGetAll);
        var phGetById = await _phongHocService.GetByIdAsync(1);
        AssertForbiddenUnauthenticated(phGetById);
    }

    [TestMethod]
    public async Task Unauthenticated_CannotCreateOrModify_All6Services_ReturnsForbidden()
    {
        ClearUser();

        // 1. HocVien
        var hvRes = await _hocVienService.CreateAsync(new HocVienDto { MaHocVien = "HV_TEST", HoTen = "Test Unauth" });
        AssertForbiddenUnauthenticated(hvRes);

        // 2. GiangVien
        var gvRes = await _giangVienService.CreateAsync(new GiangVienDto { MaGiangVien = "GV_TEST", HoTen = "Test Unauth" });
        AssertForbiddenUnauthenticated(gvRes);

        // 3. MonHoc
        var mhRes = await _monHocService.CreateAsync(new MonHocDto { MaMonHoc = "MH_TEST", TenMonHoc = "Test Unauth", SoBuoiMacDinh = 10 });
        AssertForbiddenUnauthenticated(mhRes);

        // 4. KhoaHoc
        var khRes = await _khoaHocService.CreateAsync(new KhoaHocDto { MaKhoaHoc = "KH_TEST", TenKhoaHoc = "Test Unauth", HocPhiNiemYet = 1000m });
        AssertForbiddenUnauthenticated(khRes);

        // 5. KhoaHocMonHoc
        var khmhRes = await _khoaHocMonHocService.AddMonHocAsync(new KhoaHocMonHocDto { KhoaHocId = 1, MonHocId = 4, ThuTu = 4, SoBuoiDuKien = 10 });
        AssertForbiddenUnauthenticated(khmhRes);

        // 6. PhongHoc
        var phRes = await _phongHocService.CreateAsync(new PhongHocDto { MaPhong = "P_TEST", TenPhong = "Test Unauth", SucChua = 20 });
        AssertForbiddenUnauthenticated(phRes);
    }

    #endregion

    #region 2. ADMIN

    [TestMethod]
    public async Task Admin_CanRead_All6Services_Success()
    {
        SetUser(VaiTro.ADMIN);

        // Read all 6 services
        var hvList = await _hocVienService.GetAllAsync();
        Assert.IsTrue(hvList.IsSuccess);
        var hvItem = await _hocVienService.GetByIdAsync(1);
        Assert.IsTrue(hvItem.IsSuccess);

        var gvList = await _giangVienService.GetAllAsync();
        Assert.IsTrue(gvList.IsSuccess);
        var gvItem = await _giangVienService.GetByIdAsync(1);
        Assert.IsTrue(gvItem.IsSuccess);

        var mhList = await _monHocService.GetAllAsync();
        Assert.IsTrue(mhList.IsSuccess);
        var mhItem = await _monHocService.GetByIdAsync(1);
        Assert.IsTrue(mhItem.IsSuccess);

        var khList = await _khoaHocService.GetAllAsync();
        Assert.IsTrue(khList.IsSuccess);
        var khItem = await _khoaHocService.GetByIdAsync(1);
        Assert.IsTrue(khItem.IsSuccess);

        var khmhList = await _khoaHocMonHocService.GetByKhoaHocIdAsync(1);
        Assert.IsTrue(khmhList.IsSuccess);

        var phList = await _phongHocService.GetAllAsync();
        Assert.IsTrue(phList.IsSuccess);
        var phItem = await _phongHocService.GetByIdAsync(1);
        Assert.IsTrue(phItem.IsSuccess);
    }

    [TestMethod]
    public async Task Admin_CanCreateUpdateDeactivate_AllServices_Success()
    {
        SetUser(VaiTro.ADMIN);

        // 1. HocVien
        var hvCreate = await _hocVienService.CreateAsync(new HocVienDto { MaHocVien = "HV_ADM", HoTen = "Admin Created" });
        Assert.IsTrue(hvCreate.IsSuccess);
        Assert.IsNotNull(hvCreate.Data);
        var hvUpdate = await _hocVienService.UpdateAsync(new HocVienDto { Id = hvCreate.Data.Id, MaHocVien = "HV_ADM", HoTen = "Admin Updated" });
        Assert.IsTrue(hvUpdate.IsSuccess);
        var hvDeact = await _hocVienService.DeactivateAsync(hvCreate.Data.Id);
        Assert.IsTrue(hvDeact.IsSuccess);

        // 2. GiangVien
        var gvCreate = await _giangVienService.CreateAsync(new GiangVienDto { MaGiangVien = "GV_ADM", HoTen = "Admin Created" });
        Assert.IsTrue(gvCreate.IsSuccess);
        Assert.IsNotNull(gvCreate.Data);
        var gvUpdate = await _giangVienService.UpdateAsync(new GiangVienDto { Id = gvCreate.Data.Id, MaGiangVien = "GV_ADM", HoTen = "Admin Updated" });
        Assert.IsTrue(gvUpdate.IsSuccess);
        var gvDeact = await _giangVienService.DeactivateAsync(gvCreate.Data.Id);
        Assert.IsTrue(gvDeact.IsSuccess);

        // 3. MonHoc
        var mhCreate = await _monHocService.CreateAsync(new MonHocDto { MaMonHoc = "MH_ADM", TenMonHoc = "Admin Created", SoBuoiMacDinh = 10 });
        Assert.IsTrue(mhCreate.IsSuccess);
        Assert.IsNotNull(mhCreate.Data);
        var mhUpdate = await _monHocService.UpdateAsync(new MonHocDto { Id = mhCreate.Data.Id, MaMonHoc = "MH_ADM", TenMonHoc = "Admin Updated", SoBuoiMacDinh = 12 });
        Assert.IsTrue(mhUpdate.IsSuccess);
        var mhDeact = await _monHocService.DeactivateAsync(mhCreate.Data.Id);
        Assert.IsTrue(mhDeact.IsSuccess);

        // 4. KhoaHoc
        var khCreate = await _khoaHocService.CreateAsync(new KhoaHocDto { MaKhoaHoc = "KH_ADM", TenKhoaHoc = "Admin Created", HocPhiNiemYet = 1000m });
        Assert.IsTrue(khCreate.IsSuccess);
        Assert.IsNotNull(khCreate.Data);
        var khUpdate = await _khoaHocService.UpdateAsync(new KhoaHocDto { Id = khCreate.Data.Id, MaKhoaHoc = "KH_ADM", TenKhoaHoc = "Admin Updated", HocPhiNiemYet = 1200m });
        Assert.IsTrue(khUpdate.IsSuccess);
        var khDeact = await _khoaHocService.DeactivateAsync(khCreate.Data.Id);
        Assert.IsTrue(khDeact.IsSuccess);

        // 5. PhongHoc
        var phCreate = await _phongHocService.CreateAsync(new PhongHocDto { MaPhong = "P_ADM", TenPhong = "Admin Created", SucChua = 30 });
        Assert.IsTrue(phCreate.IsSuccess);
        Assert.IsNotNull(phCreate.Data);
        var phUpdate = await _phongHocService.UpdateAsync(new PhongHocDto { Id = phCreate.Data.Id, MaPhong = "P_ADM", TenPhong = "Admin Updated", SucChua = 35 });
        Assert.IsTrue(phUpdate.IsSuccess);
        var phDeact = await _phongHocService.DeactivateAsync(phCreate.Data.Id);
        Assert.IsTrue(phDeact.IsSuccess);
    }

    #endregion

    #region 3. DAOTAO

    [TestMethod]
    public async Task DaoTao_CanRead_All6Services_Success()
    {
        SetUser(VaiTro.DAOTAO);

        Assert.IsTrue((await _hocVienService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _giangVienService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _monHocService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _khoaHocService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _khoaHocMonHocService.GetByKhoaHocIdAsync(1)).IsSuccess);
        Assert.IsTrue((await _phongHocService.GetAllAsync()).IsSuccess);
    }

    [TestMethod]
    public async Task DaoTao_CanModify_CatalogServices_Success()
    {
        SetUser(VaiTro.DAOTAO);

        // Create, update, deactivate
        var hvRes = await _hocVienService.CreateAsync(new HocVienDto { MaHocVien = "HV_DT", HoTen = "Dao Tao HV" });
        Assert.IsTrue(hvRes.IsSuccess);
        Assert.IsNotNull(hvRes.Data);
        Assert.IsTrue((await _hocVienService.UpdateAsync(new HocVienDto { Id = hvRes.Data.Id, MaHocVien = "HV_DT", HoTen = "Dao Tao HV Edit" })).IsSuccess);
        Assert.IsTrue((await _hocVienService.DeactivateAsync(hvRes.Data.Id)).IsSuccess);

        var mhRes = await _monHocService.CreateAsync(new MonHocDto { MaMonHoc = "MH_DT", TenMonHoc = "Dao Tao MH", SoBuoiMacDinh = 15 });
        Assert.IsTrue(mhRes.IsSuccess);
        Assert.IsNotNull(mhRes.Data);
        Assert.IsTrue((await _monHocService.UpdateAsync(new MonHocDto { Id = mhRes.Data.Id, MaMonHoc = "MH_DT", TenMonHoc = "Dao Tao MH Edit", SoBuoiMacDinh = 16 })).IsSuccess);
        Assert.IsTrue((await _monHocService.DeactivateAsync(mhRes.Data.Id)).IsSuccess);
    }

    #endregion

    #region 4. THUNGAN

    [TestMethod]
    public async Task ThuNgan_CanRead_All6Services_Success()
    {
        SetUser(VaiTro.THUNGAN);

        Assert.IsTrue((await _hocVienService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _giangVienService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _monHocService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _khoaHocService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _khoaHocMonHocService.GetByKhoaHocIdAsync(1)).IsSuccess);
        Assert.IsTrue((await _phongHocService.GetAllAsync()).IsSuccess);
    }

    [TestMethod]
    public async Task ThuNgan_CannotCreate_AnyCatalogService_ReturnsForbidden_AndRepositoryUntouched()
    {
        SetUser(VaiTro.THUNGAN);

        // Record initial repo counts
        int hvCountBefore = (await _hocVienRepo.GetAllAsync()).Count;
        int gvCountBefore = (await _giangVienRepo.GetAllAsync()).Count;
        int mhCountBefore = (await _monHocRepo.GetAllAsync()).Count;
        int khCountBefore = (await _khoaHocRepo.GetAllAsync()).Count;
        int phCountBefore = (await _phongHocRepo.GetAllAsync()).Count;
        int khmhCountBefore = (await _khoaHocMonHocRepo.GetByKhoaHocIdAsync(1)).Count;

        // Attempt creates
        var hvRes = await _hocVienService.CreateAsync(new HocVienDto { MaHocVien = "HV_TN_X", HoTen = "Thu Ngan Try" });
        AssertForbiddenCatalogManage(hvRes);

        var gvRes = await _giangVienService.CreateAsync(new GiangVienDto { MaGiangVien = "GV_TN_X", HoTen = "Thu Ngan Try" });
        AssertForbiddenCatalogManage(gvRes);

        var mhRes = await _monHocService.CreateAsync(new MonHocDto { MaMonHoc = "MH_TN_X", TenMonHoc = "Thu Ngan Try", SoBuoiMacDinh = 10 });
        AssertForbiddenCatalogManage(mhRes);

        var khRes = await _khoaHocService.CreateAsync(new KhoaHocDto { MaKhoaHoc = "KH_TN_X", TenKhoaHoc = "Thu Ngan Try", HocPhiNiemYet = 1000m });
        AssertForbiddenCatalogManage(khRes);

        var phRes = await _phongHocService.CreateAsync(new PhongHocDto { MaPhong = "P_TN_X", TenPhong = "Thu Ngan Try", SucChua = 20 });
        AssertForbiddenCatalogManage(phRes);

        var khmhRes = await _khoaHocMonHocService.AddMonHocAsync(new KhoaHocMonHocDto { KhoaHocId = 1, MonHocId = 4, ThuTu = 4, SoBuoiDuKien = 10 });
        AssertForbiddenCatalogManage(khmhRes);

        // Verify repository counts are completely untouched
        Assert.HasCount(hvCountBefore, await _hocVienRepo.GetAllAsync());
        Assert.HasCount(gvCountBefore, await _giangVienRepo.GetAllAsync());
        Assert.HasCount(mhCountBefore, await _monHocRepo.GetAllAsync());
        Assert.HasCount(khCountBefore, await _khoaHocRepo.GetAllAsync());
        Assert.HasCount(phCountBefore, await _phongHocRepo.GetAllAsync());
        Assert.HasCount(khmhCountBefore, await _khoaHocMonHocRepo.GetByKhoaHocIdAsync(1));
    }

    [TestMethod]
    public async Task ThuNgan_CannotUpdateOrDeactivate_AllServices_ReturnsForbidden_AndRepositoryUntouched()
    {
        SetUser(VaiTro.THUNGAN);

        // 1. HocVien 1
        var originalHv = await _hocVienRepo.GetByIdAsync(1);
        Assert.IsNotNull(originalHv);
        var hvUpd = await _hocVienService.UpdateAsync(new HocVienDto { Id = 1, MaHocVien = originalHv.MaHocVien, HoTen = "Hacked Name" });
        AssertForbiddenCatalogManage(hvUpd);
        var hvDeact = await _hocVienService.DeactivateAsync(1);
        AssertForbiddenCatalogManage(hvDeact);
        var currentHv = await _hocVienRepo.GetByIdAsync(1);
        Assert.AreEqual(originalHv.HoTen, currentHv!.HoTen);
        Assert.AreEqual(originalHv.TrangThai, currentHv.TrangThai);

        // 2. GiangVien 1
        var originalGv = await _giangVienRepo.GetByIdAsync(1);
        Assert.IsNotNull(originalGv);
        var gvUpd = await _giangVienService.UpdateAsync(new GiangVienDto { Id = 1, MaGiangVien = originalGv.MaGiangVien, HoTen = "Hacked GV" });
        AssertForbiddenCatalogManage(gvUpd);
        var gvDeact = await _giangVienService.DeactivateAsync(1);
        AssertForbiddenCatalogManage(gvDeact);
        var currentGv = await _giangVienRepo.GetByIdAsync(1);
        Assert.AreEqual(originalGv.HoTen, currentGv!.HoTen);
        Assert.AreEqual(originalGv.TrangThai, currentGv.TrangThai);

        // 3. MonHoc 1
        var originalMh = await _monHocRepo.GetByIdAsync(1);
        Assert.IsNotNull(originalMh);
        var mhUpd = await _monHocService.UpdateAsync(new MonHocDto { Id = 1, MaMonHoc = originalMh.MaMonHoc, TenMonHoc = "Hacked MH", SoBuoiMacDinh = 99 });
        AssertForbiddenCatalogManage(mhUpd);
        var mhDeact = await _monHocService.DeactivateAsync(1);
        AssertForbiddenCatalogManage(mhDeact);
        var currentMh = await _monHocRepo.GetByIdAsync(1);
        Assert.AreEqual(originalMh.TenMonHoc, currentMh!.TenMonHoc);
        Assert.AreEqual(originalMh.TrangThai, currentMh.TrangThai);

        // 4. KhoaHoc 1
        var originalKh = await _khoaHocRepo.GetByIdAsync(1);
        Assert.IsNotNull(originalKh);
        var khUpd = await _khoaHocService.UpdateAsync(new KhoaHocDto { Id = 1, MaKhoaHoc = originalKh.MaKhoaHoc, TenKhoaHoc = "Hacked KH", HocPhiNiemYet = 0m });
        AssertForbiddenCatalogManage(khUpd);
        var khDeact = await _khoaHocService.DeactivateAsync(1);
        AssertForbiddenCatalogManage(khDeact);
        var currentKh = await _khoaHocRepo.GetByIdAsync(1);
        Assert.AreEqual(originalKh.TenKhoaHoc, currentKh!.TenKhoaHoc);
        Assert.AreEqual(originalKh.TrangThai, currentKh.TrangThai);

        // 5. PhongHoc 1
        var originalPh = await _phongHocRepo.GetByIdAsync(1);
        Assert.IsNotNull(originalPh);
        var phUpd = await _phongHocService.UpdateAsync(new PhongHocDto { Id = 1, MaPhong = originalPh.MaPhong, TenPhong = "Hacked PH", SucChua = 999 });
        AssertForbiddenCatalogManage(phUpd);
        var phDeact = await _phongHocService.DeactivateAsync(1);
        AssertForbiddenCatalogManage(phDeact);
        var currentPh = await _phongHocRepo.GetByIdAsync(1);
        Assert.AreEqual(originalPh.TenPhong, currentPh!.TenPhong);
        Assert.AreEqual(originalPh.TrangThai, currentPh.TrangThai);
    }

    #endregion

    #region 5. GIANGVIEN

    [TestMethod]
    public async Task GiangVien_CanRead_All6Services_Success()
    {
        SetUser(VaiTro.GIANGVIEN);

        Assert.IsTrue((await _hocVienService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _giangVienService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _monHocService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _khoaHocService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _khoaHocMonHocService.GetByKhoaHocIdAsync(1)).IsSuccess);
        Assert.IsTrue((await _phongHocService.GetAllAsync()).IsSuccess);
    }

    [TestMethod]
    public async Task GiangVien_CannotModify_Services_ReturnsForbidden()
    {
        SetUser(VaiTro.GIANGVIEN);

        var createRes = await _hocVienService.CreateAsync(new HocVienDto { MaHocVien = "HV_GV_FAIL", HoTen = "GiangVien Try" });
        AssertForbiddenCatalogManage(createRes);

        var updateRes = await _hocVienService.UpdateAsync(new HocVienDto { Id = 1, MaHocVien = "HV001", HoTen = "GiangVien Try" });
        AssertForbiddenCatalogManage(updateRes);

        var deactRes = await _hocVienService.DeactivateAsync(1);
        AssertForbiddenCatalogManage(deactRes);
    }

    #endregion

    #region 6. QUANLY

    [TestMethod]
    public async Task QuanLy_CanRead_All6Services_Success()
    {
        SetUser(VaiTro.QUANLY);

        Assert.IsTrue((await _hocVienService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _giangVienService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _monHocService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _khoaHocService.GetAllAsync()).IsSuccess);
        Assert.IsTrue((await _khoaHocMonHocService.GetByKhoaHocIdAsync(1)).IsSuccess);
        Assert.IsTrue((await _phongHocService.GetAllAsync()).IsSuccess);
    }

    [TestMethod]
    public async Task QuanLy_CannotModify_Services_ReturnsForbidden()
    {
        SetUser(VaiTro.QUANLY);

        var createRes = await _khoaHocService.CreateAsync(new KhoaHocDto { MaKhoaHoc = "KH_QL_FAIL", TenKhoaHoc = "QuanLy Try", HocPhiNiemYet = 500m });
        AssertForbiddenCatalogManage(createRes);

        var updateRes = await _khoaHocService.UpdateAsync(new KhoaHocDto { Id = 1, MaKhoaHoc = "KH01", TenKhoaHoc = "QuanLy Try", HocPhiNiemYet = 500m });
        AssertForbiddenCatalogManage(updateRes);

        var deactRes = await _khoaHocService.DeactivateAsync(1);
        AssertForbiddenCatalogManage(deactRes);
    }

    #endregion

    #region 7. Khóa học - Môn học (KhoaHocMonHoc)

    [TestMethod]
    public async Task KhoaHocMonHoc_CatalogViewOnly_CannotAddMonHoc_ReturnsForbidden()
    {
        SetUser(VaiTro.THUNGAN);

        var res = await _khoaHocMonHocService.AddMonHocAsync(new KhoaHocMonHocDto
        {
            KhoaHocId = 1,
            MonHocId = 4,
            ThuTu = 4,
            SoBuoiDuKien = 10
        });

        AssertForbiddenCatalogManage(res);
    }

    [TestMethod]
    public async Task KhoaHocMonHoc_CatalogViewOnly_CannotRemoveMonHoc_ReturnsForbidden()
    {
        SetUser(VaiTro.THUNGAN);

        var res = await _khoaHocMonHocService.RemoveMonHocAsync(1, 1);

        AssertForbiddenCatalogManage(res);
    }

    [TestMethod]
    public async Task KhoaHocMonHoc_CatalogViewOnly_CannotUpdateThuTuSoBuoi_ReturnsForbidden()
    {
        SetUser(VaiTro.THUNGAN);

        var res = await _khoaHocMonHocService.UpdateThuTuSoBuoiAsync(new KhoaHocMonHocDto
        {
            KhoaHocId = 1,
            MonHocId = 1,
            ThuTu = 5,
            SoBuoiDuKien = 20
        });

        AssertForbiddenCatalogManage(res);
    }

    [TestMethod]
    public async Task KhoaHocMonHoc_AdminAndDaoTao_CanAddRemoveUpdate_Success()
    {
        // 1. ADMIN - Khóa học 2 (WinForms) hiện chỉ có môn 1 và 2, thêm môn 3
        SetUser(VaiTro.ADMIN);
        var addResAdm = await _khoaHocMonHocService.AddMonHocAsync(new KhoaHocMonHocDto
        {
            KhoaHocId = 2,
            MonHocId = 3,
            ThuTu = 3,
            SoBuoiDuKien = 18
        });
        Assert.IsTrue(addResAdm.IsSuccess);

        var updateResAdm = await _khoaHocMonHocService.UpdateThuTuSoBuoiAsync(new KhoaHocMonHocDto
        {
            KhoaHocId = 2,
            MonHocId = 3,
            ThuTu = 3,
            SoBuoiDuKien = 20
        });
        Assert.IsTrue(updateResAdm.IsSuccess);

        var removeResAdm = await _khoaHocMonHocService.RemoveMonHocAsync(2, 3);
        Assert.IsTrue(removeResAdm.IsSuccess);

        // 2. DAOTAO - Tạo môn mới MH04 rồi thêm vào Khóa học 1
        SetUser(VaiTro.DAOTAO);
        var newMhRes = await _monHocService.CreateAsync(new MonHocDto
        {
            MaMonHoc = "MH04",
            TenMonHoc = "Kiểm thử phần mềm",
            SoBuoiMacDinh = 10
        });
        Assert.IsTrue(newMhRes.IsSuccess);
        Assert.IsNotNull(newMhRes.Data);

        var addResDt = await _khoaHocMonHocService.AddMonHocAsync(new KhoaHocMonHocDto
        {
            KhoaHocId = 1,
            MonHocId = newMhRes.Data.Id,
            ThuTu = 4,
            SoBuoiDuKien = 12
        });
        Assert.IsTrue(addResDt.IsSuccess);

        var updateResDt = await _khoaHocMonHocService.UpdateThuTuSoBuoiAsync(new KhoaHocMonHocDto
        {
            KhoaHocId = 1,
            MonHocId = newMhRes.Data.Id,
            ThuTu = 4,
            SoBuoiDuKien = 14
        });
        Assert.IsTrue(updateResDt.IsSuccess);

        var removeResDt = await _khoaHocMonHocService.RemoveMonHocAsync(1, newMhRes.Data.Id);
        Assert.IsTrue(removeResDt.IsSuccess);
    }

    #endregion

    #region CCCD Protection Tests

    [TestMethod]
    public async Task HocVienService_CatalogManage_Receives_Full_CCCD()
    {
        SetUser(VaiTro.ADMIN);
        var res = await _hocVienService.GetByIdAsync(1);
        Assert.IsTrue(res.IsSuccess);
        Assert.IsNotNull(res.Data);
        Assert.AreEqual("001203001234", res.Data.SoDinhDanh);
        Assert.AreEqual("********1234", res.Data.SoDinhDanhMasked);

        var allRes = await _hocVienService.GetAllAsync();
        Assert.IsTrue(allRes.IsSuccess);
        var hv1 = allRes.Data!.FirstOrDefault(x => x.Id == 1);
        Assert.IsNotNull(hv1);
        Assert.AreEqual("001203001234", hv1.SoDinhDanh);
    }

    [TestMethod]
    public async Task HocVienService_CatalogViewOnly_Receives_Masked_CCCD()
    {
        SetUser(VaiTro.THUNGAN);
        var res = await _hocVienService.GetByIdAsync(1);
        Assert.IsTrue(res.IsSuccess);
        Assert.IsNotNull(res.Data);
        Assert.AreEqual("********1234", res.Data.SoDinhDanh);
        Assert.AreEqual("********1234", res.Data.SoDinhDanhMasked);
        Assert.DoesNotContain("00120300", res.Data.SoDinhDanh!, "Raw CCCD must not be exposed in memory to view-only roles.");

        var allRes = await _hocVienService.GetAllAsync();
        Assert.IsTrue(allRes.IsSuccess);
        var hv1 = allRes.Data!.FirstOrDefault(x => x.Id == 1);
        Assert.IsNotNull(hv1);
        Assert.AreEqual("********1234", hv1.SoDinhDanh);
        Assert.DoesNotContain("00120300", hv1.SoDinhDanh!);
    }

    #endregion

    #region Helper Assertions

    private static void AssertForbiddenUnauthenticated<T>(OperationResult<T> result)
    {
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.FORBIDDEN, result.Error?.Code);
        Assert.AreEqual(AuthorizationGuard.NotAuthenticatedMessage, result.Error?.Message);
    }

    private static void AssertForbiddenCatalogManage<T>(OperationResult<T> result)
    {
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.FORBIDDEN, result.Error?.Code);
        Assert.AreEqual(AuthorizationGuard.CatalogManageForbiddenMessage, result.Error?.Message);
    }

    #endregion
}
