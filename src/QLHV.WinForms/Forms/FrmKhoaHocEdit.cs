using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmKhoaHocEdit : Form
{
    private readonly IKhoaHocService _service;
    private readonly KhoaHocDto? _editingKhoaHoc;
    private readonly bool _isEditMode;

    public FrmKhoaHocEdit(IKhoaHocService service, KhoaHocDto? editingKhoaHoc = null)
    {
        InitializeComponent();
        _service = service;
        _editingKhoaHoc = editingKhoaHoc;
        _isEditMode = editingKhoaHoc != null;

        LoadFormData();
    }

    private void LoadFormData()
    {
        cboTrangThai.DataSource = Enum.GetValues(typeof(TrangThaiHoatDong));

        if (_isEditMode && _editingKhoaHoc != null)
        {
            lblTitle.Text = "CẬP NHẬT KHÓA HỌC";
            Text = "Cập nhật thông tin khóa học";
            txtMaKhoaHoc.Text = _editingKhoaHoc.MaKhoaHoc;
            txtMaKhoaHoc.ReadOnly = true;
            txtTenKhoaHoc.Text = _editingKhoaHoc.TenKhoaHoc;
            nudHocPhi.Value = _editingKhoaHoc.HocPhiNiemYet >= 0 ? _editingKhoaHoc.HocPhiNiemYet : 0;
            txtMoTa.Text = _editingKhoaHoc.MoTa ?? string.Empty;
            cboTrangThai.SelectedItem = _editingKhoaHoc.TrangThai;
        }
        else
        {
            lblTitle.Text = "THÊM MỚI KHÓA HỌC";
            Text = "Thêm khóa học mới";
            nudHocPhi.Value = 5000000m;
            cboTrangThai.SelectedItem = TrangThaiHoatDong.DangHoatDong;
        }
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        errorProvider.Clear();
        lblError.Text = string.Empty;

        var maKhoaHoc = txtMaKhoaHoc.Text.Trim();
        var tenKhoaHoc = txtTenKhoaHoc.Text.Trim();
        var hocPhi = nudHocPhi.Value;

        bool hasError = false;
        if (string.IsNullOrWhiteSpace(maKhoaHoc))
        {
            errorProvider.SetError(txtMaKhoaHoc, "Mã khóa học là bắt buộc.");
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(tenKhoaHoc))
        {
            errorProvider.SetError(txtTenKhoaHoc, "Tên khóa học là bắt buộc.");
            hasError = true;
        }

        if (hocPhi < 0)
        {
            errorProvider.SetError(nudHocPhi, "Học phí niêm yết không được âm.");
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
            var dto = new KhoaHocDto
            {
                Id = _isEditMode && _editingKhoaHoc != null ? _editingKhoaHoc.Id : 0,
                MaKhoaHoc = maKhoaHoc,
                TenKhoaHoc = tenKhoaHoc,
                HocPhiNiemYet = hocPhi,
                MoTa = string.IsNullOrWhiteSpace(txtMoTa.Text) ? null : txtMoTa.Text.Trim(),
                TrangThai = (TrangThaiHoatDong)(cboTrangThai.SelectedItem ?? TrangThaiHoatDong.DangHoatDong)
            };

            OperationResult<KhoaHocDto> result;
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
                    errorProvider.SetError(txtMaKhoaHoc, result.Message);
                }
                else if (result.Error?.Code == ErrorCodes.INVALID_RANGE)
                {
                    errorProvider.SetError(nudHocPhi, result.Message);
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
