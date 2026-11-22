using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Constants;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Features.Catalog.Services;

namespace QLHV.Tests;

[TestClass]
public class CatalogValidationAndCrudTests
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
        _currentUserService.SetCurrentUser(new TaiKhoanDto
        {
            Id = 1,
            TenDangNhap = "admin",
            VaiTro = VaiTro.ADMIN
        });

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

    #region HocVien Tests

    [TestMethod]
    public async Task HocVien_Validation_RequiredFields()
    {
        // Missing MaHocVien
        var res1 = await _hocVienService.CreateAsync(new HocVienDto { MaHocVien = "", HoTen = "Nguyễn A" });
        Assert.IsFalse(res1.IsSuccess);
        Assert.AreEqual(ErrorCodes.REQUIRED, res1.Error?.Code);

        // Missing HoTen
        var res2 = await _hocVienService.CreateAsync(new HocVienDto { MaHocVien = "HV999", HoTen = "   " });
        Assert.IsFalse(res2.IsSuccess);
        Assert.AreEqual(ErrorCodes.REQUIRED, res2.Error?.Code);
    }

    [TestMethod]
    public async Task HocVien_Validation_DuplicateCode_CaseInsensitive()
    {
        // "hv001" duplicates seed "HV001"
        var res = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "hv001",
            HoTen = "Trùng Mã"
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.DUPLICATE_CODE, res.Error?.Code);
    }

    [TestMethod]
    public async Task HocVien_Validation_FutureBirthDate_Fails()
    {
        var res = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV099",
            HoTen = "Trẻ Em Tương Lai",
            NgaySinh = DateTime.Today.AddDays(1)
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res.Error?.Code);
    }

    [TestMethod]
    public async Task HocVien_Validation_InvalidEmail_Fails()
    {
        var res = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV099",
            HoTen = "Email Sai",
            Email = "invalid-email-address@"
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_FORMAT, res.Error?.Code);
    }

    [TestMethod]
    public async Task HocVien_Validation_InvalidPhone_Fails()
    {
        var res = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV099",
            HoTen = "SĐT Sai",
            DienThoai = "abc123xyz"
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_FORMAT, res.Error?.Code);
    }

    [TestMethod]
    public async Task HocVien_Crud_Create_Update_Deactivate_Success()
    {
        // 1. Create
        var newDto = new HocVienDto
        {
            MaHocVien = "HV100",
            HoTen = "Lê Văn Tiến",
            Email = "tien.le@example.com",
            DienThoai = "0987654321",
            NgaySinh = new DateTime(2001, 1, 1),
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        var createRes = await _hocVienService.CreateAsync(newDto);
        Assert.IsTrue(createRes.IsSuccess);
        Assert.IsNotNull(createRes.Data);
        var createdId = createRes.Data.Id;
        Assert.IsGreaterThan(0, createdId);

        // 2. Update
        createRes.Data.HoTen = "Lê Văn Tiến (Updated)";
        var updateRes = await _hocVienService.UpdateAsync(createRes.Data);
        Assert.IsTrue(updateRes.IsSuccess);

        var getRes = await _hocVienService.GetByIdAsync(createdId);
        Assert.IsTrue(getRes.IsSuccess);
        Assert.AreEqual("Lê Văn Tiến (Updated)", getRes.Data?.HoTen);

        // 3. Deactivate
        var deactRes = await _hocVienService.DeactivateAsync(createdId);
        Assert.IsTrue(deactRes.IsSuccess);

        var checkRes = await _hocVienService.GetByIdAsync(createdId);
        Assert.AreEqual(TrangThaiHoatDong.NgungHoatDong, checkRes.Data?.TrangThai);
    }

    [TestMethod]
    public async Task HocVien_Search_CaseInsensitive()
    {
        var searchRes = await _hocVienService.GetAllAsync(new HocVienFilter { TuKhoa = "nguyen van an" });
        Assert.IsTrue(searchRes.IsSuccess);
        Assert.IsNotNull(searchRes.Data);
        Assert.HasCount(1, searchRes.Data);
        Assert.AreEqual("HV001", searchRes.Data[0].MaHocVien);
    }

    [TestMethod]
    public async Task HocVien_GetById_NotFound_ReturnsError()
    {
        var res = await _hocVienService.GetByIdAsync(99999);
        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.NOT_FOUND, res.Error?.Code);
    }

    #endregion

    #region GiangVien Tests

    [TestMethod]
    public async Task GiangVien_Validation_DuplicateCode_Fails()
    {
        var res = await _giangVienService.CreateAsync(new GiangVienDto
        {
            MaGiangVien = "gv001", // case-insensitive duplicate of GV001
            HoTen = "Giảng Viên Trùng"
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.DUPLICATE_CODE, res.Error?.Code);
    }

    #endregion

    #region MonHoc Tests

    [TestMethod]
    public async Task MonHoc_Validation_InvalidSessions_Fails()
    {
        var res = await _monHocService.CreateAsync(new MonHocDto
        {
            MaMonHoc = "MH99",
            TenMonHoc = "Môn Số Buổi 0",
            SoBuoiMacDinh = 0
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res.Error?.Code);
    }

    #endregion

    #region KhoaHoc Tests

    [TestMethod]
    public async Task KhoaHoc_Validation_NegativeTuition_Fails()
    {
        var res = await _khoaHocService.CreateAsync(new KhoaHocDto
        {
            MaKhoaHoc = "KH99",
            TenKhoaHoc = "Khóa Học Học Phí Âm",
            HocPhiNiemYet = -1000m
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res.Error?.Code);
    }

    #endregion

    #region KhoaHoc - MonHoc Tests

    [TestMethod]
    public async Task KhoaHocMonHoc_Validation_DuplicateSubject_Fails()
    {
        // Course 1 already has Subject 1 in seed
        var res = await _khoaHocMonHocService.AddMonHocAsync(new KhoaHocMonHocDto
        {
            KhoaHocId = 1,
            MonHocId = 1,
            ThuTu = 4,
            SoBuoiDuKien = 10
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.DUPLICATE_CODE, res.Error?.Code);
    }

    [TestMethod]
    public async Task KhoaHocMonHoc_Validation_NonExistentEntities_Fails()
    {
        var res = await _khoaHocMonHocService.AddMonHocAsync(new KhoaHocMonHocDto
        {
            KhoaHocId = 9999,
            MonHocId = 1,
            ThuTu = 1,
            SoBuoiDuKien = 10
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.NOT_FOUND, res.Error?.Code);
    }

    #endregion

    #region PhongHoc Tests

    [TestMethod]
    public async Task PhongHoc_Validation_NonPositiveCapacity_Fails()
    {
        var res = await _phongHocService.CreateAsync(new PhongHocDto
        {
            MaPhong = "P999",
            TenPhong = "Phòng Sức Chứa 0",
            SucChua = 0
        });

        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res.Error?.Code);
    }

    #endregion

    #region Catalog Field Limits Exceeded Tests

    [TestMethod]
    public async Task HocVien_Validation_ExceededFieldLimits_Fails()
    {
        // MaHocVien > 20 chars
        var res1 = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = new string('A', CatalogFieldLimits.MaHocVien + 1),
            HoTen = "Học viên Test"
        });
        Assert.IsFalse(res1.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res1.Error?.Code);

        // HoTen > 100 chars
        var res2 = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_TEST",
            HoTen = new string('B', CatalogFieldLimits.HoTen + 1)
        });
        Assert.IsFalse(res2.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res2.Error?.Code);

        // GioiTinh > 20 chars
        var res3 = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_TEST",
            HoTen = "Học viên Test",
            GioiTinh = new string('C', CatalogFieldLimits.GioiTinh + 1)
        });
        Assert.IsFalse(res3.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res3.Error?.Code);

        // DienThoai > 20 chars
        var res4 = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_TEST",
            HoTen = "Học viên Test",
            DienThoai = new string('1', CatalogFieldLimits.DienThoai + 1)
        });
        Assert.IsFalse(res4.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res4.Error?.Code);

        // Email > 150 chars
        var longEmail = new string('a', 140) + "@example.com"; // > 150 chars
        var res5 = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_TEST",
            HoTen = "Học viên Test",
            Email = longEmail
        });
        Assert.IsFalse(res5.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res5.Error?.Code);

        // DiaChi > 250 chars
        var res6 = await _hocVienService.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_TEST",
            HoTen = "Học viên Test",
            DiaChi = new string('D', CatalogFieldLimits.DiaChi + 1)
        });
        Assert.IsFalse(res6.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res6.Error?.Code);

        // Update with HoTen > 100 chars
        var res7 = await _hocVienService.UpdateAsync(new HocVienDto
        {
            Id = 1,
            MaHocVien = "HV001",
            HoTen = new string('U', CatalogFieldLimits.HoTen + 1)
        });
        Assert.IsFalse(res7.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res7.Error?.Code);
    }

    [TestMethod]
    public async Task GiangVien_Validation_ExceededFieldLimits_Fails()
    {
        // MaGiangVien > 20 chars
        var res1 = await _giangVienService.CreateAsync(new GiangVienDto
        {
            MaGiangVien = new string('G', CatalogFieldLimits.MaGiangVien + 1),
            HoTen = "Giảng viên Test"
        });
        Assert.IsFalse(res1.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res1.Error?.Code);

        // HoTen > 100 chars
        var res2 = await _giangVienService.CreateAsync(new GiangVienDto
        {
            MaGiangVien = "GV_TEST",
            HoTen = new string('H', CatalogFieldLimits.HoTen + 1)
        });
        Assert.IsFalse(res2.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res2.Error?.Code);

        // ChuyenMon > 200 chars
        var res3 = await _giangVienService.CreateAsync(new GiangVienDto
        {
            MaGiangVien = "GV_TEST",
            HoTen = "Giảng viên Test",
            ChuyenMon = new string('C', CatalogFieldLimits.ChuyenMon + 1)
        });
        Assert.IsFalse(res3.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res3.Error?.Code);

        // Update with MaGiangVien > 20 chars
        var res4 = await _giangVienService.UpdateAsync(new GiangVienDto
        {
            Id = 1,
            MaGiangVien = new string('G', CatalogFieldLimits.MaGiangVien + 1),
            HoTen = "Giảng viên Test"
        });
        Assert.IsFalse(res4.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res4.Error?.Code);
    }

    [TestMethod]
    public async Task MonHoc_Validation_ExceededFieldLimits_Fails()
    {
        // MaMonHoc > 20 chars
        var res1 = await _monHocService.CreateAsync(new MonHocDto
        {
            MaMonHoc = new string('M', CatalogFieldLimits.MaMonHoc + 1),
            TenMonHoc = "Môn học Test",
            SoBuoiMacDinh = 10
        });
        Assert.IsFalse(res1.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res1.Error?.Code);

        // TenMonHoc > 150 chars
        var res2 = await _monHocService.CreateAsync(new MonHocDto
        {
            MaMonHoc = "MH_TEST",
            TenMonHoc = new string('T', CatalogFieldLimits.TenMonHoc + 1),
            SoBuoiMacDinh = 10
        });
        Assert.IsFalse(res2.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res2.Error?.Code);

        // MoTa > 500 chars
        var res3 = await _monHocService.CreateAsync(new MonHocDto
        {
            MaMonHoc = "MH_TEST",
            TenMonHoc = "Môn học Test",
            MoTa = new string('D', CatalogFieldLimits.MoTa + 1),
            SoBuoiMacDinh = 10
        });
        Assert.IsFalse(res3.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res3.Error?.Code);
    }

    [TestMethod]
    public async Task KhoaHoc_Validation_ExceededFieldLimits_Fails()
    {
        // MaKhoaHoc > 20 chars
        var res1 = await _khoaHocService.CreateAsync(new KhoaHocDto
        {
            MaKhoaHoc = new string('K', CatalogFieldLimits.MaKhoaHoc + 1),
            TenKhoaHoc = "Khóa học Test",
            HocPhiNiemYet = 1000000
        });
        Assert.IsFalse(res1.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res1.Error?.Code);

        // TenKhoaHoc > 150 chars
        var res2 = await _khoaHocService.CreateAsync(new KhoaHocDto
        {
            MaKhoaHoc = "KH_TEST",
            TenKhoaHoc = new string('T', CatalogFieldLimits.TenKhoaHoc + 1),
            HocPhiNiemYet = 1000000
        });
        Assert.IsFalse(res2.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res2.Error?.Code);

        // MoTa > 500 chars
        var res3 = await _khoaHocService.CreateAsync(new KhoaHocDto
        {
            MaKhoaHoc = "KH_TEST",
            TenKhoaHoc = "Khóa học Test",
            MoTa = new string('D', CatalogFieldLimits.MoTa + 1),
            HocPhiNiemYet = 1000000
        });
        Assert.IsFalse(res3.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res3.Error?.Code);
    }

    [TestMethod]
    public async Task PhongHoc_Validation_ExceededFieldLimits_Fails()
    {
        // MaPhong > 20 chars
        var res1 = await _phongHocService.CreateAsync(new PhongHocDto
        {
            MaPhong = new string('P', CatalogFieldLimits.MaPhong + 1),
            TenPhong = "Phòng Test",
            SucChua = 30
        });
        Assert.IsFalse(res1.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res1.Error?.Code);

        // TenPhong > 100 chars
        var res2 = await _phongHocService.CreateAsync(new PhongHocDto
        {
            MaPhong = "PH_TEST",
            TenPhong = new string('T', CatalogFieldLimits.TenPhong + 1),
            SucChua = 30
        });
        Assert.IsFalse(res2.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res2.Error?.Code);

        // Update with TenPhong > 100 chars
        var res3 = await _phongHocService.UpdateAsync(new PhongHocDto
        {
            Id = 1,
            MaPhong = "P101",
            TenPhong = new string('T', CatalogFieldLimits.TenPhong + 1),
            SucChua = 30
        });
        Assert.IsFalse(res3.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, res3.Error?.Code);
    }

    #endregion
}
