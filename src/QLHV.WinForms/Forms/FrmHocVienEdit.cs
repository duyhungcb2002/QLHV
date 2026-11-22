using QLHV.Contracts.Constants;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Repositories;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmHocVienEdit : Form
{
    private readonly IHocVienService _service;
    private readonly ITaiKhoanRepository? _taiKhoanRepository;
    private readonly HocVienDto? _editingHocVien;
    private readonly bool _isEditMode;

    private class AccountItem
    {
        public int? Id { get; set; }
        public string DisplayText { get; set; } = string.Empty;
        public override string ToString() => DisplayText;
    }

    public FrmHocVienEdit(
        IHocVienService service,
        HocVienDto? editingHocVien = null,
        ITaiKhoanRepository? taiKhoanRepository = null)
    {
        InitializeComponent();
        _service = service;
        _editingHocVien = editingHocVien;
        _taiKhoanRepository = taiKhoanRepository;
        _isEditMode = editingHocVien != null;

        txtGhiChuDacBiet.TextChanged += (s, e) =>
        {
            lblCharCounter.Text = $"{txtGhiChuDacBiet.Text.Length} / 1000 ký tự";
        };

        LoadFormData();
    }

    private async void LoadFormData()
    {
        cboTrangThai.DataSource = Enum.GetValues(typeof(TrangThaiHoatDong));

        // Load Nguồn tuyển sinh
        cboNguonTuyenSinh.Items.Clear();
        cboNguonTuyenSinh.Items.Add("-- Chọn nguồn tuyển sinh --");
        foreach (var src in NguonTuyenSinhDefaults.All)
        {
            cboNguonTuyenSinh.Items.Add(src);
        }
        cboNguonTuyenSinh.SelectedIndex = 0;

        // Load Nhân viên phụ trách (Admin hoặc DaoTao)
        cboNhanVienPhuTrach.Items.Clear();
        cboNhanVienPhuTrach.Items.Add(new AccountItem { Id = null, DisplayText = "-- Chưa phân công --" });

        if (_taiKhoanRepository != null)
        {
            try
            {
                var accounts = await _taiKhoanRepository.GetAllAsync();
                var eligibleAccounts = accounts
                    .Where(a => (a.TrangThai == TrangThaiTaiKhoan.HoatDong || (_editingHocVien != null && a.Id == _editingHocVien.NhanVienPhuTrachTaiKhoanId))
                                && (a.VaiTro == VaiTro.ADMIN || a.VaiTro == VaiTro.DAOTAO))
                    .OrderBy(a => a.TenDangNhap);

                foreach (var acc in eligibleAccounts)
                {
                    cboNhanVienPhuTrach.Items.Add(new AccountItem
                    {
                        Id = acc.Id,
                        DisplayText = $"{acc.TenDangNhap} ({acc.VaiTro})"
                    });
                }
            }
            catch
            {
                // Bỏ qua lỗi load tài khoản nếu offline/fake
            }
        }
        cboNhanVienPhuTrach.SelectedIndex = 0;

        if (_isEditMode && _editingHocVien != null)
        {
            lblTitle.Text = "CẬP NHẬT HỌC VIÊN";
            Text = "Cập nhật thông tin học viên";

            // Tab 1: Cơ bản
            txtMaHocVien.Text = _editingHocVien.MaHocVien;
            txtMaHocVien.ReadOnly = true;
            txtHoTen.Text = _editingHocVien.HoTen;

            if (_editingHocVien.NgaySinh.HasValue)
            {
                dtpNgaySinh.Checked = true;
                dtpNgaySinh.Value = _editingHocVien.NgaySinh.Value;
            }
            else
            {
                dtpNgaySinh.Checked = false;
            }

            cboGioiTinh.SelectedItem = _editingHocVien.GioiTinh ?? "Nam";
            txtDienThoai.Text = _editingHocVien.DienThoai ?? string.Empty;
            txtEmail.Text = _editingHocVien.Email ?? string.Empty;
            txtDiaChi.Text = _editingHocVien.DiaChi ?? string.Empty;
            cboTrangThai.SelectedItem = _editingHocVien.TrangThai;

            // Tab 2: Định danh
            txtSoDinhDanh.Text = _editingHocVien.SoDinhDanh ?? string.Empty;
            if (_editingHocVien.NgayCapGiayTo.HasValue)
            {
                dtpNgayCapGiayTo.Checked = true;
                dtpNgayCapGiayTo.Value = _editingHocVien.NgayCapGiayTo.Value;
            }
            else
            {
                dtpNgayCapGiayTo.Checked = false;
            }
            txtNoiCapGiayTo.Text = _editingHocVien.NoiCapGiayTo ?? string.Empty;

            // Tab 3: Giám hộ
            txtHoTenNguoiGiamHo.Text = _editingHocVien.HoTenNguoiGiamHo ?? string.Empty;
            txtDienThoaiNguoiGiamHo.Text = _editingHocVien.DienThoaiNguoiGiamHo ?? string.Empty;
            txtQuanHeNguoiGiamHo.Text = _editingHocVien.QuanHeNguoiGiamHo ?? string.Empty;

            // Tab 4: Tuyển sinh & Việc làm
            if (!string.IsNullOrWhiteSpace(_editingHocVien.NguonTuyenSinh))
            {
                var idx = cboNguonTuyenSinh.Items.IndexOf(_editingHocVien.NguonTuyenSinh);
                if (idx >= 0) cboNguonTuyenSinh.SelectedIndex = idx;
            }

            if (_editingHocVien.NhanVienPhuTrachTaiKhoanId.HasValue)
            {
                for (int i = 0; i < cboNhanVienPhuTrach.Items.Count; i++)
                {
                    if (cboNhanVienPhuTrach.Items[i] is AccountItem item && item.Id == _editingHocVien.NhanVienPhuTrachTaiKhoanId.Value)
                    {
                        cboNhanVienPhuTrach.SelectedIndex = i;
                        break;
                    }
                }
            }

            txtNgheNghiep.Text = _editingHocVien.NgheNghiep ?? string.Empty;
            txtDonViCongTac.Text = _editingHocVien.DonViCongTac ?? string.Empty;

            // Tab 5: Ghi chú
            txtGhiChuDacBiet.Text = _editingHocVien.GhiChuDacBiet ?? string.Empty;
        }
        else
        {
            lblTitle.Text = "THÊM MỚI HỌC VIÊN";
            Text = "Thêm học viên mới";
            cboGioiTinh.SelectedIndex = 0;
            dtpNgaySinh.Checked = false;
            dtpNgayCapGiayTo.Checked = false;
            cboTrangThai.SelectedItem = TrangThaiHoatDong.DangHoatDong;
        }

        lblCharCounter.Text = $"{txtGhiChuDacBiet.Text.Length} / 1000 ký tự";
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        errorProvider.Clear();
        lblError.Text = string.Empty;

        var maHocVien = txtMaHocVien.Text.Trim();
        var hoTen = txtHoTen.Text.Trim();

        bool hasError = false;
        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            errorProvider.SetError(txtMaHocVien, "Mã học viên là bắt buộc.");
            tabControl.SelectedTab = tabCoBan;
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(hoTen))
        {
            errorProvider.SetError(txtHoTen, "Họ và tên là bắt buộc.");
            if (!hasError) tabControl.SelectedTab = tabCoBan;
            hasError = true;
        }

        if (dtpNgaySinh.Checked && dtpNgaySinh.Value.Date > DateTime.Today)
        {
            errorProvider.SetError(dtpNgaySinh, "Ngày sinh không được lớn hơn ngày hiện tại.");
            if (!hasError) tabControl.SelectedTab = tabCoBan;
            hasError = true;
        }

        // Nhóm giấy tờ định danh (SoDinhDanh, NgayCapGiayTo, NoiCapGiayTo)
        var soDinhDanh = txtSoDinhDanh.Text.Trim();
        var noiCap = txtNoiCapGiayTo.Text.Trim();
        bool hasSdd = !string.IsNullOrWhiteSpace(soDinhDanh);
        bool hasNgayCap = dtpNgayCapGiayTo.Checked;
        bool hasNoiCap = !string.IsNullOrWhiteSpace(noiCap);

        if (hasSdd || hasNgayCap || hasNoiCap)
        {
            if (!hasSdd || !hasNgayCap || !hasNoiCap)
            {
                if (!hasSdd) errorProvider.SetError(txtSoDinhDanh, "Vui lòng nhập Số CCCD/CMND.");
                if (!hasNgayCap) errorProvider.SetError(dtpNgayCapGiayTo, "Vui lòng chọn Ngày cấp.");
                if (!hasNoiCap) errorProvider.SetError(txtNoiCapGiayTo, "Vui lòng nhập Nơi cấp.");
                if (!hasError) tabControl.SelectedTab = tabDinhDanh;
                hasError = true;
            }
            else if (dtpNgayCapGiayTo.Value.Date > DateTime.Today)
            {
                errorProvider.SetError(dtpNgayCapGiayTo, "Ngày cấp không được lớn hơn ngày hiện tại.");
                if (!hasError) tabControl.SelectedTab = tabDinhDanh;
                hasError = true;
            }
            else if (dtpNgaySinh.Checked && dtpNgayCapGiayTo.Value.Date < dtpNgaySinh.Value.Date)
            {
                errorProvider.SetError(dtpNgayCapGiayTo, "Ngày cấp không được trước ngày sinh.");
                if (!hasError) tabControl.SelectedTab = tabDinhDanh;
                hasError = true;
            }
        }

        // Kiểm tra người giám hộ nếu học viên < 18 tuổi
        if (dtpNgaySinh.Checked)
        {
            var birthDate = dtpNgaySinh.Value.Date;
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;

            if (age < 18)
            {
                if (string.IsNullOrWhiteSpace(txtHoTenNguoiGiamHo.Text))
                {
                    errorProvider.SetError(txtHoTenNguoiGiamHo, "Học viên dưới 18 tuổi bắt buộc nhập họ tên người giám hộ.");
                    if (!hasError) tabControl.SelectedTab = tabGiamHo;
                    hasError = true;
                }
                if (string.IsNullOrWhiteSpace(txtDienThoaiNguoiGiamHo.Text))
                {
                    errorProvider.SetError(txtDienThoaiNguoiGiamHo, "Học viên dưới 18 tuổi bắt buộc nhập điện thoại người giám hộ.");
                    if (!hasError) tabControl.SelectedTab = tabGiamHo;
                    hasError = true;
                }
                if (string.IsNullOrWhiteSpace(txtQuanHeNguoiGiamHo.Text))
                {
                    errorProvider.SetError(txtQuanHeNguoiGiamHo, "Học viên dưới 18 tuổi bắt buộc nhập quan hệ người giám hộ.");
                    if (!hasError) tabControl.SelectedTab = tabGiamHo;
                    hasError = true;
                }
            }
        }

        if (hasError)
        {
            lblError.Text = "Vui lòng kiểm tra và hoàn thiện các trường dữ liệu bị báo lỗi.";
            return;
        }

        btnSave.Enabled = false;

        try
        {
            int? selectedNhanVienId = null;
            if (cboNhanVienPhuTrach.SelectedItem is AccountItem item && item.Id.HasValue)
            {
                selectedNhanVienId = item.Id.Value;
            }

            string? selectedNguon = null;
            if (cboNguonTuyenSinh.SelectedIndex > 0 && cboNguonTuyenSinh.SelectedItem != null)
            {
                selectedNguon = cboNguonTuyenSinh.SelectedItem.ToString();
            }

            var dto = new HocVienDto
            {
                Id = _isEditMode && _editingHocVien != null ? _editingHocVien.Id : 0,
                MaHocVien = maHocVien,
                HoTen = hoTen,
                NgaySinh = dtpNgaySinh.Checked ? dtpNgaySinh.Value.Date : null,
                GioiTinh = cboGioiTinh.SelectedItem?.ToString(),
                DienThoai = string.IsNullOrWhiteSpace(txtDienThoai.Text) ? null : txtDienThoai.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                DiaChi = string.IsNullOrWhiteSpace(txtDiaChi.Text) ? null : txtDiaChi.Text.Trim(),
                NgayDangKy = _isEditMode && _editingHocVien != null ? _editingHocVien.NgayDangKy : DateTime.Today,
                TrangThai = (TrangThaiHoatDong)(cboTrangThai.SelectedItem ?? TrangThaiHoatDong.DangHoatDong),

                SoDinhDanh = string.IsNullOrWhiteSpace(txtSoDinhDanh.Text) ? null : txtSoDinhDanh.Text.Trim(),
                NgayCapGiayTo = dtpNgayCapGiayTo.Checked ? dtpNgayCapGiayTo.Value.Date : null,
                NoiCapGiayTo = string.IsNullOrWhiteSpace(txtNoiCapGiayTo.Text) ? null : txtNoiCapGiayTo.Text.Trim(),
                HoTenNguoiGiamHo = string.IsNullOrWhiteSpace(txtHoTenNguoiGiamHo.Text) ? null : txtHoTenNguoiGiamHo.Text.Trim(),
                DienThoaiNguoiGiamHo = string.IsNullOrWhiteSpace(txtDienThoaiNguoiGiamHo.Text) ? null : txtDienThoaiNguoiGiamHo.Text.Trim(),
                QuanHeNguoiGiamHo = string.IsNullOrWhiteSpace(txtQuanHeNguoiGiamHo.Text) ? null : txtQuanHeNguoiGiamHo.Text.Trim(),
                NhanVienPhuTrachTaiKhoanId = selectedNhanVienId,
                NguonTuyenSinh = selectedNguon,
                NgheNghiep = string.IsNullOrWhiteSpace(txtNgheNghiep.Text) ? null : txtNgheNghiep.Text.Trim(),
                DonViCongTac = string.IsNullOrWhiteSpace(txtDonViCongTac.Text) ? null : txtDonViCongTac.Text.Trim(),
                GhiChuDacBiet = string.IsNullOrWhiteSpace(txtGhiChuDacBiet.Text) ? null : txtGhiChuDacBiet.Text.Trim()
            };

            OperationResult<HocVienDto> result;
            if (_isEditMode)
            {
                result = await _service.UpdateAsync(dto);
            }
            else
            {
                result = await _service.CreateAsync(dto);
            }

            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                lblError.Text = result.Message;
                if (result.Error?.Code == ErrorCodes.DUPLICATE_CODE)
                {
                    if (result.Message.Contains("định danh", StringComparison.OrdinalIgnoreCase))
                    {
                        errorProvider.SetError(txtSoDinhDanh, result.Message);
                        tabControl.SelectedTab = tabDinhDanh;
                    }
                    else
                    {
                        errorProvider.SetError(txtMaHocVien, result.Message);
                        tabControl.SelectedTab = tabCoBan;
                    }
                }
                else if (result.Error?.Code == ErrorCodes.INVALID_FORMAT)
                {
                    if (result.Message.Contains("định danh", StringComparison.OrdinalIgnoreCase) || result.Message.Contains("CCCD", StringComparison.OrdinalIgnoreCase))
                    {
                        errorProvider.SetError(txtSoDinhDanh, result.Message);
                        tabControl.SelectedTab = tabDinhDanh;
                    }
                    else if (result.Message.Contains("giám hộ", StringComparison.OrdinalIgnoreCase))
                    {
                        errorProvider.SetError(txtDienThoaiNguoiGiamHo, result.Message);
                        tabControl.SelectedTab = tabGiamHo;
                    }
                    else if (result.Message.Contains("email", StringComparison.OrdinalIgnoreCase))
                    {
                        errorProvider.SetError(txtEmail, result.Message);
                        tabControl.SelectedTab = tabCoBan;
                    }
                    else
                    {
                        errorProvider.SetError(txtDienThoai, result.Message);
                        tabControl.SelectedTab = tabCoBan;
                    }
                }
            }
        }
        finally
        {
            btnSave.Enabled = true;
        }
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
