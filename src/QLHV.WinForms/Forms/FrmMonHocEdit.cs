using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmMonHocEdit : Form
{
    private readonly IMonHocService _service;
    private readonly MonHocDto? _editingMonHoc;
    private readonly bool _isEditMode;

    public FrmMonHocEdit(IMonHocService service, MonHocDto? editingMonHoc = null)
    {
        InitializeComponent();
        _service = service;
        _editingMonHoc = editingMonHoc;
        _isEditMode = editingMonHoc != null;

        LoadFormData();
    }

    private void LoadFormData()
    {
        cboTrangThai.DataSource = Enum.GetValues(typeof(TrangThaiHoatDong));

        if (_isEditMode && _editingMonHoc != null)
        {
            lblTitle.Text = "CẬP NHẬT MÔN HỌC";
            Text = "Cập nhật thông tin môn học";
            txtMaMonHoc.Text = _editingMonHoc.MaMonHoc;
            txtMaMonHoc.ReadOnly = true;
            txtTenMonHoc.Text = _editingMonHoc.TenMonHoc;
            nudSoBuoi.Value = _editingMonHoc.SoBuoiMacDinh > 0 ? _editingMonHoc.SoBuoiMacDinh : 15;
            txtMoTa.Text = _editingMonHoc.MoTa ?? string.Empty;
            cboTrangThai.SelectedItem = _editingMonHoc.TrangThai;
        }
        else
        {
            lblTitle.Text = "THÊM MỚI MÔN HỌC";
            Text = "Thêm môn học mới";
            nudSoBuoi.Value = 15;
            cboTrangThai.SelectedItem = TrangThaiHoatDong.DangHoatDong;
        }
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        errorProvider.Clear();
        lblError.Text = string.Empty;

        var maMonHoc = txtMaMonHoc.Text.Trim();
        var tenMonHoc = txtTenMonHoc.Text.Trim();
        var soBuoi = (int)nudSoBuoi.Value;

        bool hasError = false;
        if (string.IsNullOrWhiteSpace(maMonHoc))
        {
            errorProvider.SetError(txtMaMonHoc, "Mã môn học là bắt buộc.");
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(tenMonHoc))
        {
            errorProvider.SetError(txtTenMonHoc, "Tên môn học là bắt buộc.");
            hasError = true;
        }

        if (soBuoi <= 0)
        {
            errorProvider.SetError(nudSoBuoi, "Số buổi mặc định phải lớn hơn 0.");
            hasError = true;
        }

        if (hasError)
        {
            lblError.Text = "Vui lòng nhập các thông tin hợp lệ.";
            return;
        }

        btnSave.Enabled = false;

        try
        {
            var dto = new MonHocDto
            {
                Id = _isEditMode && _editingMonHoc != null ? _editingMonHoc.Id : 0,
                MaMonHoc = maMonHoc,
                TenMonHoc = tenMonHoc,
                SoBuoiMacDinh = soBuoi,
                MoTa = string.IsNullOrWhiteSpace(txtMoTa.Text) ? null : txtMoTa.Text.Trim(),
                TrangThai = (TrangThaiHoatDong)(cboTrangThai.SelectedItem ?? TrangThaiHoatDong.DangHoatDong)
            };

            OperationResult<MonHocDto> result;
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
                    errorProvider.SetError(txtMaMonHoc, result.Message);
                }
                else if (result.Error?.Code == ErrorCodes.INVALID_RANGE)
                {
                    errorProvider.SetError(nudSoBuoi, result.Message);
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
