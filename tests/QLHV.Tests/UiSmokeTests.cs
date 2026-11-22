using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Features.Catalog.Services;
using QLHV.Features.Training.Repositories.Fake;
using QLHV.Features.Training.Services;
using QLHV.WinForms.Configuration;
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

    private OperationsServiceBundle _operationsBundle = null!;

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

        var trainingStore = new FakeTrainingStore();
        var lopRepo = new FakeLopHocRepository(trainingStore);
        var dkRepo = new FakeDangKyLopHocRepository(trainingStore);
        var lichRepo = new FakeLichHocRepository(trainingStore);
        var kqRepo = new FakeKetQuaHocTapRepository(trainingStore);
        var ptRepo = new FakePhieuThuRepository(trainingStore);
        var bcRepo = new FakeBaoCaoRepository(trainingStore, _khoaHocMonHocRepo, _monHocRepo);

        _operationsBundle = new OperationsServiceBundle
        {
            LopHocService = new LopHocService(lopRepo, _khoaHocRepo, _giangVienRepo, dkRepo, lichRepo, kqRepo, _currentUserService, _logger),
            DangKyLopHocService = new DangKyLopHocService(dkRepo, lopRepo, _hocVienRepo, _khoaHocRepo, _currentUserService, _logger),
            LichHocService = new LichHocService(lichRepo, lopRepo, _monHocRepo, _giangVienRepo, _phongHocRepo, _khoaHocMonHocRepo, _currentUserService, _logger),
            KetQuaHocTapService = new KetQuaHocTapService(kqRepo, dkRepo, lopRepo, _monHocRepo, _khoaHocMonHocRepo, lichRepo, _currentUserService, _logger),
            HocPhiService = new HocPhiService(ptRepo, dkRepo, lopRepo, _currentUserService, _logger),
            BaoCaoService = new BaoCaoService(bcRepo, lopRepo, _currentUserService, _logger)
        };
    }

    [TestMethod]
    public void Login_Form_Controls_Initialized()
    {
        using var form = new FrmLogin(_authService);
        Assert.IsNotNull(form);
        Assert.AreEqual("Đăng nhập hệ thống QLHV", form.Text);
        Assert.IsNotNull(form.AcceptButton);
        Assert.IsNotNull(form.CancelButton);

        var hintControls = form.Controls.Find("lblDemoHint", true);
        Assert.HasCount(1, hintControls);
        StringAssert.Contains(hintControls[0].Text, "Tài khoản mẫu:");
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
            _phongHocService,
            _operationsBundle,
            "SQL Server");

        Assert.IsNotNull(form);
        Assert.IsFalse(string.IsNullOrEmpty(form.Text));

        var badgeControls = form.Controls.Find("lblEnvBadge", true);
        Assert.HasCount(1, badgeControls);
        Assert.AreEqual("Môi trường: SQL Server", badgeControls[0].Text);
    }

    [TestMethod]
    public void Main_Form_Shows_Fake_Data_Environment_Badge()
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
            _phongHocService,
            _operationsBundle,
            "Fake Data");

        Assert.IsNotNull(form);
        var badgeControls = form.Controls.Find("lblEnvBadge", true);
        Assert.HasCount(1, badgeControls);
        Assert.AreEqual("Môi trường: Fake Data", badgeControls[0].Text);
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

        // 7. FrmDaoTao
        using (var daoTaoForm = new FrmDaoTao(
            _operationsBundle.LopHocService,
            _operationsBundle.DangKyLopHocService,
            _operationsBundle.LichHocService,
            _khoaHocService,
            _hocVienService,
            _monHocService,
            _giangVienService,
            _phongHocService,
            _currentUserService))
        {
            Assert.IsNotNull(daoTaoForm);
            Assert.AreEqual("Quản lý Đào tạo", daoTaoForm.Text);

            // Kiểm tra FrmDaoTao 3 DataGridViews AutoGenerateColumns = false
            var daoTaoDgvLop = daoTaoForm.GetType().GetField("dgvLopHoc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(daoTaoForm) as DataGridView;
            var daoTaoDgvDangKy = daoTaoForm.GetType().GetField("dgvDangKy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(daoTaoForm) as DataGridView;
            var daoTaoDgvLich = daoTaoForm.GetType().GetField("dgvLichHoc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(daoTaoForm) as DataGridView;

            Assert.IsNotNull(daoTaoDgvLop);
            Assert.IsNotNull(daoTaoDgvDangKy);
            Assert.IsNotNull(daoTaoDgvLich);

            Assert.IsFalse(daoTaoDgvLop.AutoGenerateColumns, "dgvLopHoc phải có AutoGenerateColumns == false");
            Assert.IsFalse(daoTaoDgvDangKy.AutoGenerateColumns, "dgvDangKy phải có AutoGenerateColumns == false");
            Assert.IsFalse(daoTaoDgvLich.AutoGenerateColumns, "dgvLichHoc phải có AutoGenerateColumns == false");
        }

        // 8. FrmDiem
        using (var diemForm = new FrmDiem(
            _operationsBundle.KetQuaHocTapService,
            _operationsBundle.LopHocService,
            _monHocService,
            _currentUserService))
        {
            Assert.IsNotNull(diemForm);
            Assert.AreEqual("Quản lý Điểm số", diemForm.Text);
        }

        // 9. FrmHocPhi
        using (var hocPhiForm = new FrmHocPhi(
            _operationsBundle.HocPhiService,
            _operationsBundle.LopHocService,
            _operationsBundle.DangKyLopHocService,
            _currentUserService))
        {
            Assert.IsNotNull(hocPhiForm);
            Assert.AreEqual("Quản lý Học phí", hocPhiForm.Text);
        }

        // 10. FrmBaoCao
        using (var baoCaoForm = new FrmBaoCao(
            _operationsBundle.BaoCaoService,
            _operationsBundle.LopHocService,
            _monHocService,
            _giangVienService,
            _currentUserService))
        {
            Assert.IsNotNull(baoCaoForm);
            Assert.AreEqual("Báo cáo thống kê", baoCaoForm.Text);
        }

        // 11. FrmHocVienDetail & FrmHocVienEdit Redesign (STUDENT-PROFILE-AND-TRAINING-UI-05)
        var sampleHv = new HocVienDto
        {
            Id = 1,
            MaHocVien = "HV001",
            HoTen = "Nguyễn Văn An",
            SoDinhDanh = "001200001234",
            NguonTuyenSinh = "Facebook"
        };
        using (var detailForm = new FrmHocVienDetail(sampleHv, _currentUserService, _operationsBundle.DangKyLopHocService))
        {
            Assert.IsNotNull(detailForm);
            Assert.AreEqual("Hồ sơ chi tiết học viên", detailForm.Text);
        }

        using (var hvEditForm = new FrmHocVienEdit(_hocVienService, sampleHv))
        {
            Assert.IsNotNull(hvEditForm);
            Assert.AreEqual("Cập nhật thông tin học viên", hvEditForm.Text);
        }
    }

    [TestMethod]
    public void FrmHocVien_UI_ExactStrings_AndHeaders()
    {
        _currentUserService.SetCurrentUser(new TaiKhoanDto { Id = 1, TenDangNhap = "admin", VaiTro = VaiTro.ADMIN });
        using var form = new FrmHocVien(_hocVienService, _currentUserService);

        Assert.AreEqual("Quản lý Học viên", form.Text);

        var lblTitle = form.Controls.Find("lblTitle", true).FirstOrDefault() as Label;
        Assert.IsNotNull(lblTitle);
        Assert.AreEqual("DANH MỤC HỌC VIÊN", lblTitle.Text);

        var lblSearch = form.Controls.Find("lblSearch", true).FirstOrDefault() as Label;
        Assert.IsNotNull(lblSearch);
        Assert.AreEqual("Từ khóa tìm kiếm:", lblSearch.Text);

        var txtSearch = form.Controls.Find("txtSearch", true).FirstOrDefault() as TextBox;
        Assert.IsNotNull(txtSearch);
        Assert.AreEqual("Nhập mã, họ tên, điện thoại, CCCD/CMND, giám hộ...", txtSearch.PlaceholderText);

        var lblStatus = form.Controls.Find("lblStatusFilter", true).FirstOrDefault() as Label;
        Assert.IsNotNull(lblStatus);
        Assert.AreEqual("Trạng thái:", lblStatus.Text);

        var btnSearch = form.Controls.Find("btnSearch", true).FirstOrDefault() as Button;
        Assert.IsNotNull(btnSearch);
        Assert.AreEqual("Tìm kiếm", btnSearch.Text);

        var btnRefresh = form.Controls.Find("btnRefresh", true).FirstOrDefault() as Button;
        Assert.IsNotNull(btnRefresh);
        Assert.AreEqual("Làm mới", btnRefresh.Text);

        var btnDetail = form.Controls.Find("btnDetail", true).FirstOrDefault() as Button;
        Assert.IsNotNull(btnDetail);
        Assert.AreEqual("Xem chi tiết", btnDetail.Text);

        var btnAdd = form.Controls.Find("btnAdd", true).FirstOrDefault() as Button;
        Assert.IsNotNull(btnAdd);
        Assert.AreEqual("Thêm mới", btnAdd.Text);

        var btnEdit = form.Controls.Find("btnEdit", true).FirstOrDefault() as Button;
        Assert.IsNotNull(btnEdit);
        Assert.AreEqual("Cập nhật", btnEdit.Text);

        var btnDeact = form.Controls.Find("btnDeactivate", true).FirstOrDefault() as Button;
        Assert.IsNotNull(btnDeact);
        Assert.AreEqual("Ngừng hoạt động", btnDeact.Text);

        var dgv = form.Controls.Find("dgvHocVien", true).FirstOrDefault() as DataGridView;
        Assert.IsNotNull(dgv);
        Assert.AreEqual("Mã HV", dgv.Columns["colMaHocVien"]?.HeaderText);
        Assert.AreEqual("Họ và tên", dgv.Columns["colHoTen"]?.HeaderText);
        Assert.AreEqual("Ngày sinh", dgv.Columns["colNgaySinh"]?.HeaderText);
        Assert.AreEqual("Giới tính", dgv.Columns["colGioiTinh"]?.HeaderText);
        Assert.AreEqual("Điện thoại", dgv.Columns["colDienThoai"]?.HeaderText);
        Assert.AreEqual("CCCD/CMND", dgv.Columns["colSoDinhDanh"]?.HeaderText);
        Assert.AreEqual("Nguồn tuyển sinh", dgv.Columns["colNguonTuyenSinh"]?.HeaderText);
        Assert.AreEqual("Nhân viên phụ trách", dgv.Columns["colNhanVienPhuTrach"]?.HeaderText);
        Assert.AreEqual("Trạng thái", dgv.Columns["colTrangThai"]?.HeaderText);
    }

    [TestMethod]
    public void FrmHocVienEdit_UI_ExactStrings_TabsAndLabels()
    {
        using var form = new FrmHocVienEdit(_hocVienService);

        Assert.AreEqual("Thêm học viên mới", form.Text);

        var tabCtrl = form.Controls.Find("tabControl", true).FirstOrDefault() as TabControl;
        Assert.IsNotNull(tabCtrl);
        Assert.AreEqual(5, tabCtrl.TabPages.Count);
        Assert.AreEqual("1. Thông tin cơ bản", tabCtrl.TabPages[0].Text);
        Assert.AreEqual("2. Định danh", tabCtrl.TabPages[1].Text);
        Assert.AreEqual("3. Người giám hộ", tabCtrl.TabPages[2].Text);
        Assert.AreEqual("4. Tuyển sinh & Việc làm", tabCtrl.TabPages[3].Text);
        Assert.AreEqual("5. Ghi chú đặc biệt", tabCtrl.TabPages[4].Text);

        var btnSave = form.Controls.Find("btnSave", true).FirstOrDefault() as Button;
        Assert.IsNotNull(btnSave);
        Assert.AreEqual("Lưu lại", btnSave.Text);

        var btnCancel = form.Controls.Find("btnCancel", true).FirstOrDefault() as Button;
        Assert.IsNotNull(btnCancel);
        Assert.AreEqual("Hủy bỏ", btnCancel.Text);

        var txtDonVi = form.Controls.Find("txtDonViCongTac", true).FirstOrDefault() as TextBox;
        Assert.IsNotNull(txtDonVi);
        Assert.AreEqual(200, txtDonVi.MaxLength);
    }

    [TestMethod]
    public void FrmDaoTao_UI_GridStandardization_MinWidthsAndHeaders()
    {
        _currentUserService.SetCurrentUser(new TaiKhoanDto { Id = 1, TenDangNhap = "admin", VaiTro = VaiTro.ADMIN });
        using var form = new FrmDaoTao(
            _operationsBundle.LopHocService,
            _operationsBundle.DangKyLopHocService,
            _operationsBundle.LichHocService,
            _khoaHocService,
            _hocVienService,
            _monHocService,
            _giangVienService,
            _phongHocService,
            _currentUserService);

        var dgvLop = form.GetType().GetField("dgvLopHoc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(form) as DataGridView;
        var dgvDangKy = form.GetType().GetField("dgvDangKy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(form) as DataGridView;
        var dgvLich = form.GetType().GetField("dgvLichHoc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(form) as DataGridView;

        Assert.IsNotNull(dgvLop);
        Assert.IsNotNull(dgvDangKy);
        Assert.IsNotNull(dgvLich);

        Assert.IsGreaterThanOrEqualTo(180, dgvLop.Columns["colLop_KhoaHoc"]!.MinimumWidth);
        Assert.IsGreaterThanOrEqualTo(160, dgvLop.Columns["colLop_GiangVien"]!.MinimumWidth);
        Assert.IsGreaterThanOrEqualTo(200, dgvLop.Columns["colLop_GhiChu"]!.MinimumWidth);

        Assert.IsGreaterThanOrEqualTo(115, dgvDangKy.Columns["colDk_DienThoai"]!.MinimumWidth);
        Assert.IsGreaterThanOrEqualTo(180, dgvDangKy.Columns["colDk_GhiChu"]!.MinimumWidth);

        Assert.IsGreaterThanOrEqualTo(180, dgvLich.Columns["colLich_MonHoc"]!.MinimumWidth);
        Assert.IsGreaterThanOrEqualTo(140, dgvLich.Columns["colLich_PhongHoc"]!.MinimumWidth);
        Assert.IsGreaterThanOrEqualTo(240, dgvLich.Columns["colLich_NoiDung"]!.MinimumWidth);
    }

    [TestMethod]
    public void FrmHocVienDetail_UI_ColumnsAndFormatting()
    {
        var hv = new HocVienDto
        {
            Id = 1,
            MaHocVien = "HV001",
            HoTen = "Nguyễn Văn An",
            SoDinhDanh = "001200000001",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        using var form = new FrmHocVienDetail(hv, _currentUserService);
        Assert.IsNotNull(form);

        var dgv = form.Controls.Find("dgvLichSu", true).OfType<DataGridView>().FirstOrDefault();
        Assert.IsNotNull(dgv);

        // Verify all 8 column names are explicitly assigned
        Assert.IsNotNull(dgv.Columns["colMaLop"]);
        Assert.IsNotNull(dgv.Columns["colTenLop"]);
        Assert.IsNotNull(dgv.Columns["colTenKhoaHoc"]);
        Assert.IsNotNull(dgv.Columns["colNgayDangKy"]);
        Assert.IsNotNull(dgv.Columns["colTrangThai"]);
        Assert.IsNotNull(dgv.Columns["colHocPhiPhaiThu"]);
        Assert.IsNotNull(dgv.Columns["colDaThu"]);
        Assert.IsNotNull(dgv.Columns["colConNo"]);

        Assert.AreEqual("colMaLop", dgv.Columns[0].Name);
        Assert.AreEqual("colTenLop", dgv.Columns[1].Name);
        Assert.AreEqual("colTenKhoaHoc", dgv.Columns[2].Name);
        Assert.AreEqual("colNgayDangKy", dgv.Columns[3].Name);
        Assert.AreEqual("colTrangThai", dgv.Columns[4].Name);
        Assert.AreEqual("colHocPhiPhaiThu", dgv.Columns[5].Name);
        Assert.AreEqual("colDaThu", dgv.Columns[6].Name);
        Assert.AreEqual("colConNo", dgv.Columns[7].Name);

        // Test thực tế kết quả format qua hàm FormatTrainingHistoryCell
        // 1. Format Ngày: DateTime(2026, 3, 1) -> "01/03/2026"
        var dateArg = new DataGridViewCellFormattingEventArgs(3, 0, new DateTime(2026, 3, 1), typeof(string), new DataGridViewCellStyle());
        FrmHocVienDetail.FormatTrainingHistoryCell("colNgayDangKy", dateArg);
        Assert.IsTrue(dateArg.FormattingApplied);
        Assert.AreEqual("01/03/2026", dateArg.Value);

        // 2. Format Trạng thái: DangHoatDong -> "Đang học", NgungHoatDong -> "Đã thôi/hủy"
        var statusArgActive = new DataGridViewCellFormattingEventArgs(4, 0, TrangThaiHoatDong.DangHoatDong, typeof(string), new DataGridViewCellStyle());
        FrmHocVienDetail.FormatTrainingHistoryCell("colTrangThai", statusArgActive);
        Assert.IsTrue(statusArgActive.FormattingApplied);
        Assert.AreEqual("Đang học", statusArgActive.Value);

        var statusArgInactive = new DataGridViewCellFormattingEventArgs(4, 0, TrangThaiHoatDong.NgungHoatDong, typeof(string), new DataGridViewCellStyle());
        FrmHocVienDetail.FormatTrainingHistoryCell("colTrangThai", statusArgInactive);
        Assert.IsTrue(statusArgInactive.FormattingApplied);
        Assert.AreEqual("Đã thôi/hủy", statusArgInactive.Value);

        // 3. Format Học phí: 1500000m -> có phân cách hàng nghìn và hậu tố "đ"
        var feeArg = new DataGridViewCellFormattingEventArgs(5, 0, 1500000m, typeof(string), new DataGridViewCellStyle());
        FrmHocVienDetail.FormatTrainingHistoryCell("colHocPhiPhaiThu", feeArg);
        Assert.IsTrue(feeArg.FormattingApplied);
        Assert.AreEqual(1500000m.ToString("N0") + " đ", feeArg.Value);

        // Test thực tế kích hoạt qua event handler trên DataGridView của form (thêm 1 dòng để grid có row)
        dgv.Rows.Add();
        var onCellFormattingMethod = typeof(DataGridView).GetMethod("OnCellFormatting",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            null,
            new[] { typeof(DataGridViewCellFormattingEventArgs) },
            null);
        Assert.IsNotNull(onCellFormattingMethod, "DataGridView phải có phương thức OnCellFormatting");

        var dgvDateArg = new DataGridViewCellFormattingEventArgs(3, 0, new DateTime(2026, 3, 1), typeof(string), new DataGridViewCellStyle());
        onCellFormattingMethod.Invoke(dgv, new object[] { dgvDateArg });
        Assert.IsTrue(dgvDateArg.FormattingApplied, "Handler dgvLichSu.CellFormatting phải áp dụng định dạng cho colNgayDangKy");
        Assert.AreEqual("01/03/2026", dgvDateArg.Value);

        var dgvStatusArg = new DataGridViewCellFormattingEventArgs(4, 0, TrangThaiHoatDong.DangHoatDong, typeof(string), new DataGridViewCellStyle());
        onCellFormattingMethod.Invoke(dgv, new object[] { dgvStatusArg });
        Assert.IsTrue(dgvStatusArg.FormattingApplied, "Handler dgvLichSu.CellFormatting phải áp dụng định dạng cho colTrangThai");
        Assert.AreEqual("Đang học", dgvStatusArg.Value);

        var dgvFeeArg = new DataGridViewCellFormattingEventArgs(5, 0, 1500000m, typeof(string), new DataGridViewCellStyle());
        onCellFormattingMethod.Invoke(dgv, new object[] { dgvFeeArg });
        Assert.IsTrue(dgvFeeArg.FormattingApplied, "Handler dgvLichSu.CellFormatting phải áp dụng định dạng cho colHocPhiPhaiThu");
        Assert.AreEqual(1500000m.ToString("N0") + " đ", dgvFeeArg.Value);
    }
}
