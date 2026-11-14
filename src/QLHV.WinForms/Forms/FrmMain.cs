using QLHV.Contracts.Enums;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmMain : Form
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthService _authService;
    private readonly IHocVienService _hocVienService;
    private readonly IGiangVienService _giangVienService;
    private readonly IMonHocService _monHocService;
    private readonly IKhoaHocService _khoaHocService;
    private readonly IKhoaHocMonHocService _khoaHocMonHocService;
    private readonly IPhongHocService _phongHocService;

    private Form? _currentChildForm;
    private Button? _activeMenuButton;

    public bool IsLoggedOut { get; private set; }

    public FrmMain(
        ICurrentUserService currentUserService,
        IAuthService authService,
        IHocVienService hocVienService,
        IGiangVienService giangVienService,
        IMonHocService monHocService,
        IKhoaHocService khoaHocService,
        IKhoaHocMonHocService khoaHocMonHocService,
        IPhongHocService phongHocService,
        string environmentName)
    {
        InitializeComponent();
        _currentUserService = currentUserService;
        _authService = authService;
        _hocVienService = hocVienService;
        _giangVienService = giangVienService;
        _monHocService = monHocService;
        _khoaHocService = khoaHocService;
        _khoaHocMonHocService = khoaHocMonHocService;
        _phongHocService = phongHocService;

        SetupUserInfo();
        ConfigureMenuPermissions();
        UpdateEnvironmentBadge(environmentName);

        // Mở màn hình danh mục học viên mặc định nếu có quyền
        Load += (_, _) =>
        {
            if (_currentUserService.HasPermission(Permission.CatalogView))
            {
                btnMenuHocVien_Click(this, EventArgs.Empty);
            }
        };
    }

    private void SetupUserInfo()
    {
        var user = _currentUserService.CurrentUser;
        if (user != null)
        {
            lblUsername.Text = $"👤 {user.TenDangNhap}";
            lblRole.Text = $"Vai trò: {user.VaiTro}";
        }
    }

    private void UpdateEnvironmentBadge(string environmentName)
    {
        lblEnvBadge.Text = $"Môi trường: {environmentName}";
        if (environmentName.Contains("SQL", StringComparison.OrdinalIgnoreCase))
        {
            lblEnvBadge.BackColor = Color.FromArgb(220, 252, 231);
            lblEnvBadge.ForeColor = Color.FromArgb(22, 101, 52);
        }
        else
        {
            lblEnvBadge.BackColor = Color.FromArgb(254, 243, 199);
            lblEnvBadge.ForeColor = Color.FromArgb(180, 83, 9);
        }
    }

    private void ConfigureMenuPermissions()
    {
        bool canViewCatalog = _currentUserService.HasPermission(Permission.CatalogView);
        btnMenuHocVien.Visible = canViewCatalog;
        btnMenuGiangVien.Visible = canViewCatalog;
        btnMenuMonHoc.Visible = canViewCatalog;
        btnMenuKhoaHoc.Visible = canViewCatalog;
        btnMenuPhongHoc.Visible = canViewCatalog;

        btnMenuDaoTao.Visible = _currentUserService.HasPermission(Permission.TrainingView) || _currentUserService.HasPermission(Permission.TrainingManage);
        btnMenuDiem.Visible = _currentUserService.HasPermission(Permission.GradeEntry);
        btnMenuHocPhi.Visible = _currentUserService.HasPermission(Permission.TuitionManage);
        btnMenuBaoCao.Visible = _currentUserService.HasPermission(Permission.ReportView);
    }

    private void ShowChildForm(Form childForm, string title, Button menuButton)
    {
        // Tránh mở lặp nhiều bản của cùng một form
        if (_currentChildForm != null && _currentChildForm.GetType() == childForm.GetType())
        {
            childForm.Dispose();
            return;
        }

        if (_currentChildForm != null)
        {
            _currentChildForm.Close();
            _currentChildForm.Dispose();
        }

        _currentChildForm = childForm;
        childForm.TopLevel = false;
        childForm.FormBorderStyle = FormBorderStyle.None;
        childForm.Dock = DockStyle.Fill;

        pnlContent.Controls.Clear();
        pnlContent.Controls.Add(childForm);
        childForm.Show();

        lblActiveViewTitle.Text = title;
        HighlightMenuButton(menuButton);
    }

    private void HighlightMenuButton(Button button)
    {
        if (_activeMenuButton != null)
        {
            _activeMenuButton.BackColor = Color.FromArgb(17, 24, 39);
        }

        _activeMenuButton = button;
        _activeMenuButton.BackColor = Color.FromArgb(31, 41, 55);
    }

    private void btnMenuHocVien_Click(object? sender, EventArgs e)
    {
        var form = new FrmHocVien(_hocVienService, _currentUserService);
        ShowChildForm(form, "Danh mục / Quản lý Học viên", btnMenuHocVien);
    }

    private void btnMenuGiangVien_Click(object? sender, EventArgs e)
    {
        var form = new FrmGiangVien(_giangVienService, _currentUserService);
        ShowChildForm(form, "Danh mục / Quản lý Giảng viên", btnMenuGiangVien);
    }

    private void btnMenuMonHoc_Click(object? sender, EventArgs e)
    {
        var form = new FrmMonHoc(_monHocService, _currentUserService);
        ShowChildForm(form, "Danh mục / Quản lý Môn học", btnMenuMonHoc);
    }

    private void btnMenuKhoaHoc_Click(object? sender, EventArgs e)
    {
        var form = new FrmKhoaHoc(_khoaHocService, _khoaHocMonHocService, _monHocService, _currentUserService);
        ShowChildForm(form, "Danh mục / Quản lý Khóa học & Môn học", btnMenuKhoaHoc);
    }

    private void btnMenuPhongHoc_Click(object? sender, EventArgs e)
    {
        var form = new FrmPhongHoc(_phongHocService, _currentUserService);
        ShowChildForm(form, "Danh mục / Quản lý Phòng học", btnMenuPhongHoc);
    }

    private void btnMenuDaoTao_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Phân hệ Đào tạo (Lớp học, Lịch học, Xếp lớp) do thành viên BÁ HIỆP phụ trách.\nTrạng thái hiện tại: [PENDING INTEGRATION]",
            "Chức năng đang chờ tích hợp",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void btnMenuDiem_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Phân hệ Điểm số (Nhập điểm, Đánh giá kết quả) do thành viên BÁ HIỆP phụ trách.\nTrạng thái hiện tại: [PENDING INTEGRATION]",
            "Chức năng đang chờ tích hợp",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void btnMenuHocPhi_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Phân hệ Học phí (Thu học phí, Công nợ, Phiếu thu) do thành viên BÁ HIỆP phụ trách.\nTrạng thái hiện tại: [PENDING INTEGRATION]",
            "Chức năng đang chờ tích hợp",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void btnMenuBaoCao_Click(object? sender, EventArgs e)
    {
        var choice = MessageBox.Show(
            "Phân hệ Báo cáo & Thống kê do thành viên CHU ĐỨC phụ trách (đang chờ tích hợp CSDL).\n\nBạn có muốn mở mẫu giao diện báo cáo chuẩn (FrmBaoCaoMau) để xem thử không?",
            "Chức năng đang chờ tích hợp",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (choice == DialogResult.Yes)
        {
            var form = new FrmBaoCaoMau();
            ShowChildForm(form, "Báo cáo / Mẫu giao diện thống kê", btnMenuBaoCao);
        }
    }

    private async void btnLogout_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?",
            "Xác nhận đăng xuất",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            await _authService.LogoutAsync();
            IsLoggedOut = true;
            Close();
        }
    }
}
