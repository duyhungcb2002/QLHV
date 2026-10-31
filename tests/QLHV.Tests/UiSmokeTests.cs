using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Features.Catalog.Services;
using QLHV.WinForms.Forms;

namespace QLHV.Tests;

[TestClass]
public class UiSmokeTests
{
    private IAppLogger _logger = null!;
    private IPasswordHasher _hasher = null!;
    private FakeTaiKhoanRepository _taiKhoanRepo = null!;
    private IAuthorizationService _authzService = null!;
    private ICurrentUserService _currentUserService = null!;
    private AuthService _authService = null!;

    private FakeHocVienRepository _hocVienRepo = null!;
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
        _hasher = new Pbkdf2PasswordHasher();
        _taiKhoanRepo = new FakeTaiKhoanRepository(_hasher);
        _authzService = new AuthorizationService();
        _currentUserService = new CurrentUserService(_authzService);
        _authService = new AuthService(_taiKhoanRepo, _hasher, _currentUserService, _logger);

        _hocVienRepo = new FakeHocVienRepository();
        _hocVienService = new HocVienService(_hocVienRepo, _currentUserService, _logger);

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

    [TestMethod]
    public void Login_Form_Controls_Initialized()
    {
        using var form = new FrmLogin(_authService);
        Assert.IsNotNull(form);
        Assert.AreEqual("Đăng nhập hệ thống QLHV", form.Text);
        Assert.IsNotNull(form.AcceptButton);
        Assert.IsNotNull(form.CancelButton);
    }

    [TestMethod]
    public void Main_Form_Initializes_WithAdmin_AllMenuVisible()
    {
        _currentUserService.SetCurrentUser(new TaiKhoanDto
        {
            Id = 1,
            TenDangNhap = "admin",
            VaiTro = VaiTro.ADMIN
        });

        using var form = new FrmMain(
            _currentUserService,
            _authService,
            _hocVienService,
            _giangVienService,
            _monHocService,
            _khoaHocService,
            _khoaHocMonHocService,
            _phongHocService);

        Assert.IsNotNull(form);
        Assert.IsFalse(string.IsNullOrEmpty(form.Text));
    }

    [TestMethod]
    public void Catalog_Forms_Initialize_WithoutErrors()
    {
        _currentUserService.SetCurrentUser(new TaiKhoanDto
        {
            Id = 1,
            TenDangNhap = "admin",
            VaiTro = VaiTro.ADMIN
        });

        // 1. HocVien
        using (var hvForm = new FrmHocVien(_hocVienService, _currentUserService))
        {
            Assert.IsNotNull(hvForm);
            Assert.AreEqual("Quản lý Học viên", hvForm.Text);
        }

        // 2. GiangVien
        using (var gvForm = new FrmGiangVien(_giangVienService, _currentUserService))
        {
            Assert.IsNotNull(gvForm);
            Assert.AreEqual("Quản lý Giảng viên", gvForm.Text);
        }

        // 3. MonHoc
        using (var mhForm = new FrmMonHoc(_monHocService, _currentUserService))
        {
            Assert.IsNotNull(mhForm);
            Assert.AreEqual("Quản lý Môn học", mhForm.Text);
        }

        // 4. KhoaHoc
        using (var khForm = new FrmKhoaHoc(_khoaHocService, _khoaHocMonHocService, _monHocService, _currentUserService))
        {
            Assert.IsNotNull(khForm);
            Assert.AreEqual("Quản lý Khóa học", khForm.Text);
        }

        // 5. PhongHoc
        using (var phForm = new FrmPhongHoc(_phongHocService, _currentUserService))
        {
            Assert.IsNotNull(phForm);
            Assert.AreEqual("Quản lý Phòng học", phForm.Text);
        }

        // 6. BaoCaoMau
        using (var bcForm = new FrmBaoCaoMau())
        {
            Assert.IsNotNull(bcForm);
            Assert.AreEqual("Mẫu Báo cáo Thống kê", bcForm.Text);
        }
    }
}
