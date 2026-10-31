using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmGiangVienEdit : Form
{
    private readonly IGiangVienService _service;
    private readonly GiangVienDto? _editingGiangVien;
    private readonly bool _isEditMode;

    public FrmGiangVienEdit(IGiangVienService service, GiangVienDto? editingGiangVien = null)
    {
        InitializeComponent();
        _service = service;
        _editingGiangVien = editingGiangVien;
        _isEditMode = editingGiangVien != null;

        LoadFormData();
    }

    private void LoadFormData()
    {
        cboTrangThai.DataSource = Enum.GetValues(typeof(TrangThaiHoatDong));

        if (_isEditMode && _editingGiangVien != null)
        {
            lblTitle.Text = "CẬP NHẬT GIẢNG VIÊN";
            Text = "Cập nhật thông tin giảng viên";
            txtMaGiangVien.Text = _editingGiangVien.MaGiangVien;
            txtMaGiangVien.ReadOnly = true;
            txtHoTen.Text = _editingGiangVien.HoTen;
            txtChuyenMon.Text = _editingGiangVien.ChuyenMon ?? string.Empty;
            txtDienThoai.Text = _editingGiangVien.DienThoai ?? string.Empty;
            txtEmail.Text = _editingGiangVien.Email ?? string.Empty;
            cboTrangThai.SelectedItem = _editingGiangVien.TrangThai;
        }
        else
        {
            lblTitle.Text = "THÊM MỚI GIẢNG VIÊN";
            Text = "Thêm giảng viên mới";
            cboTrangThai.SelectedItem = TrangThaiHoatDong.DangHoatDong;
        }
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        errorProvider.Clear();
        lblError.Text = string.Empty;

        var maGiangVien = txtMaGiangVien.Text.Trim();
        var hoTen = txtHoTen.Text.Trim();

        bool hasError = false;
        if (string.IsNullOrWhiteSpace(maGiangVien))
        {
            errorProvider.SetError(txtMaGiangVien, "Mã giảng viên là bắt buộc.");
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(hoTen))
        {
            errorProvider.SetError(txtHoTen, "Họ và tên là bắt buộc.");
            hasError = true;
        }

        if (hasError)
        {
            lblError.Text = "Vui lòng nhập các thông tin bắt buộc.";
            return;
        }

        btnSave.Enabled = false;

        try
        {
            var dto = new GiangVienDto
            {
                Id = _isEditMode && _editingGiangVien != null ? _editingGiangVien.Id : 0,
                MaGiangVien = maGiangVien,
                HoTen = hoTen,
                ChuyenMon = string.IsNullOrWhiteSpace(txtChuyenMon.Text) ? null : txtChuyenMon.Text.Trim(),
                DienThoai = string.IsNullOrWhiteSpace(txtDienThoai.Text) ? null : txtDienThoai.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                TrangThai = (TrangThaiHoatDong)(cboTrangThai.SelectedItem ?? TrangThaiHoatDong.DangHoatDong)
            };

            OperationResult<GiangVienDto> result;
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
                    errorProvider.SetError(txtMaGiangVien, result.Message);
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
