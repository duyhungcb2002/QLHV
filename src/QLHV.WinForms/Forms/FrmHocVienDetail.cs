using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Core.Services;
using QLHV.WinForms.Configuration;

namespace QLHV.WinForms.Forms;

public partial class FrmHocVienDetail : Form
{
    private readonly HocVienDto _hocVien;
    private readonly IDangKyLopHocService? _dangKyService;
    private readonly ICurrentUserService _currentUserService;

    private bool _isShowingFullSdd;

    public FrmHocVienDetail(
        HocVienDto hocVien,
        ICurrentUserService currentUserService,
        IDangKyLopHocService? dangKyService = null)
    {
        InitializeComponent();
        _hocVien = hocVien ?? throw new ArgumentNullException(nameof(hocVien));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _dangKyService = dangKyService;

        ConfigureGrid();
        LoadData();
        SetupSecurityAndEvents();
        Load += async (_, _) => await LoadTrainingHistoryAsync();
    }

    private void SetupSecurityAndEvents()
    {
        bool canManage = _currentUserService.HasPermission(Permission.CatalogManage);
        btnToggleMask.Visible = canManage && !string.IsNullOrWhiteSpace(_hocVien.SoDinhDanh);
        btnToggleMask.Enabled = canManage;
        btnToggleMask.Click += (s, e) =>
        {
            if (!canManage) return;
            _isShowingFullSdd = !_isShowingFullSdd;
            UpdateSddDisplay();
        };
    }

    private void UpdateSddDisplay()
    {
        bool canManage = _currentUserService.HasPermission(Permission.CatalogManage);
        if (_isShowingFullSdd && canManage)
        {
            lblSoDinhDanh.Text = $"Số CCCD/CMND: {_hocVien.SoDinhDanh}";
            btnToggleMask.Text = "Ẩn số";
        }
        else
        {
            var masked = !string.IsNullOrWhiteSpace(_hocVien.SoDinhDanhMasked)
                ? _hocVien.SoDinhDanhMasked
                : "Chưa có";
            lblSoDinhDanh.Text = $"Số CCCD/CMND: {masked}";
            btnToggleMask.Text = "Hiện số";
        }
    }

    public static void FormatTrainingHistoryCell(string columnName, DataGridViewCellFormattingEventArgs e)
    {
        if (columnName == "colNgayDangKy" && e.Value is DateTime dt)
        {
            e.Value = dt.ToString("dd/MM/yyyy");
            e.FormattingApplied = true;
        }
        else if (columnName == "colTrangThai" && e.Value is TrangThaiHoatDong st)
        {
            e.Value = st == TrangThaiHoatDong.DangHoatDong ? "Đang học" : "Đã thôi/hủy";
            e.FormattingApplied = true;
        }
        else if ((columnName == "colHocPhiPhaiThu" ||
                  columnName == "colDaThu" ||
                  columnName == "colConNo") && e.Value is decimal amount)
        {
            e.Value = amount.ToString("N0") + " đ";
            e.FormattingApplied = true;
        }
    }

    private void ConfigureGrid()
    {
        dgvLichSu.ApplyStandardGridStyle();
        dgvLichSu.AutoGenerateColumns = false;
        dgvLichSu.CellFormatting += (s, e) =>
        {
            if (e.ColumnIndex >= 0 && e.ColumnIndex < dgvLichSu.Columns.Count)
            {
                FormatTrainingHistoryCell(dgvLichSu.Columns[e.ColumnIndex].Name, e);
            }
        };
    }

    private void LoadData()
    {
        // 1. Thông tin cơ bản
        lblMaHocVien.Text = $"Mã HV: {_hocVien.MaHocVien}";
        lblHoTen.Text = $"Họ tên: {_hocVien.HoTen}";
        lblNgaySinh.Text = $"Ngày sinh: {(_hocVien.NgaySinh.HasValue ? _hocVien.NgaySinh.Value.ToString("dd/MM/yyyy") : "Chưa cập nhật")}";
        lblGioiTinh.Text = $"Giới tính: {_hocVien.GioiTinh ?? "Chưa rõ"}";
        lblDienThoai.Text = $"Điện thoại: {_hocVien.DienThoai ?? "Chưa cập nhật"}";
        lblEmail.Text = $"Email: {_hocVien.Email ?? "Chưa cập nhật"}";
        lblDiaChi.Text = $"Địa chỉ: {_hocVien.DiaChi ?? "Chưa cập nhật"}";
        lblTrangThai.Text = $"Trạng thái: {(_hocVien.TrangThai == TrangThaiHoatDong.DangHoatDong ? "Đang hoạt động" : "Ngừng hoạt động")}";

        // 2. Giấy tờ định danh: Mặc định hiển thị CCCD đã che
        _isShowingFullSdd = false;
        UpdateSddDisplay();
        lblNgayCap.Text = $"Ngày cấp: {(_hocVien.NgayCapGiayTo.HasValue ? _hocVien.NgayCapGiayTo.Value.ToString("dd/MM/yyyy") : "Chưa cập nhật")}";
        lblNoiCap.Text = $"Nơi cấp: {_hocVien.NoiCapGiayTo ?? "Chưa cập nhật"}";

        // 3. Người giám hộ
        lblNguoiGiamHo.Text = $"Người giám hộ: {_hocVien.HoTenNguoiGiamHo ?? "Không có"}";
        lblSdtGiamHo.Text = $"Điện thoại giám hộ: {_hocVien.DienThoaiNguoiGiamHo ?? "Không có"}";
        lblQuanHe.Text = $"Quan hệ: {_hocVien.QuanHeNguoiGiamHo ?? "Không có"}";

        // 4. Tuyển sinh & Ghi chú
        lblNhanVien.Text = $"Nhân viên phụ trách: {_hocVien.TenNhanVienPhuTrach ?? "Chưa phân công"}";
        lblNguon.Text = $"Nguồn tuyển sinh: {_hocVien.NguonTuyenSinh ?? "Chưa xác định"}";
        lblNgheNghiep.Text = $"Nghề nghiệp: {_hocVien.NgheNghiep ?? "Chưa cập nhật"}";
        lblDonVi.Text = $"Đơn vị / Trường: {_hocVien.DonViCongTac ?? "Chưa cập nhật"}";
        lblGhiChu.Text = $"Ghi chú: {_hocVien.GhiChuDacBiet ?? "Không có"}";
    }

    private async Task LoadTrainingHistoryAsync()
    {
        if (_dangKyService == null) return;

        bool canViewTraining = _currentUserService.HasPermission(Permission.TrainingView) ||
                               _currentUserService.HasPermission(Permission.TrainingManage);

        if (!canViewTraining)
        {
            dgvLichSu.DataSource = null;
            grpLichSu.Text = "5. Lịch sử đào tạo & Học phí (Bạn không có quyền xem lịch sử đào tạo)";
            return;
        }

        try
        {
            var res = await _dangKyService.GetAllAsync(new DangKyLopHocFilter
            {
                HocVienId = _hocVien.Id
            });

            if (res.IsSuccess && res.Data != null)
            {
                dgvLichSu.DataSource = res.Data.ToList();
                grpLichSu.Text = $"5. Lịch sử đào tạo & Học phí ({res.Data.Count} lượt tham gia)";
            }
            else
            {
                dgvLichSu.DataSource = null;
                var msg = !string.IsNullOrWhiteSpace(res.Message) ? res.Message : "Không thể tải lịch sử đào tạo của học viên.";
                grpLichSu.Text = $"5. Lịch sử đào tạo & Học phí ({msg})";
                MessageBox.Show(msg, "Thông báo lịch sử đào tạo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            dgvLichSu.DataSource = null;
            grpLichSu.Text = "5. Lịch sử đào tạo & Học phí (Lỗi khi tải dữ liệu)";
            new FileAppLogger().LogError("Lỗi khi tải lịch sử đào tạo học viên", ex);
            MessageBox.Show("Có lỗi xảy ra khi tải lịch sử đào tạo. Vui lòng thử lại sau.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
