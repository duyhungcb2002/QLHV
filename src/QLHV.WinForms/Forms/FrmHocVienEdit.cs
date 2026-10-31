using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmHocVienEdit : Form
{
    private readonly IHocVienService _service;
    private readonly HocVienDto? _editingHocVien;
    private readonly bool _isEditMode;

    public FrmHocVienEdit(IHocVienService service, HocVienDto? editingHocVien = null)
    {
        InitializeComponent();
        _service = service;
        _editingHocVien = editingHocVien;
        _isEditMode = editingHocVien != null;

        LoadFormData();
    }

    private void LoadFormData()
    {
        cboTrangThai.DataSource = Enum.GetValues(typeof(TrangThaiHoatDong));

        if (_isEditMode && _editingHocVien != null)
        {
            lblTitle.Text = "CẬP NHẬT HỌC VIÊN";
            Text = "Cập nhật thông tin học viên";
            txtMaHocVien.Text = _editingHocVien.MaHocVien;
            txtMaHocVien.ReadOnly = true; // Không cho sửa mã khi cập nhật
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
        }
        else
        {
            lblTitle.Text = "THÊM MỚI HỌC VIÊN";
            Text = "Thêm học viên mới";
            cboGioiTinh.SelectedIndex = 0;
            dtpNgaySinh.Checked = false;
            cboTrangThai.SelectedItem = TrangThaiHoatDong.DangHoatDong;
        }
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
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(hoTen))
        {
            errorProvider.SetError(txtHoTen, "Họ và tên là bắt buộc.");
            hasError = true;
        }

        if (dtpNgaySinh.Checked && dtpNgaySinh.Value.Date > DateTime.Today)
        {
            errorProvider.SetError(dtpNgaySinh, "Ngày sinh không được lớn hơn ngày hiện tại.");
            hasError = true;
        }

        if (hasError)
        {
            lblError.Text = "Vui lòng hoàn thiện các trường dữ liệu hợp lệ.";
            return;
        }

        btnSave.Enabled = false;

        try
        {
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
                TrangThai = (TrangThaiHoatDong)(cboTrangThai.SelectedItem ?? TrangThaiHoatDong.DangHoatDong)
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
                    errorProvider.SetError(txtMaHocVien, result.Message);
                }
                else if (result.Error?.Code == ErrorCodes.INVALID_FORMAT)
                {
                    if (result.Message.Contains("email", StringComparison.OrdinalIgnoreCase))
                        errorProvider.SetError(txtEmail, result.Message);
                    else
                        errorProvider.SetError(txtDienThoai, result.Message);
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
