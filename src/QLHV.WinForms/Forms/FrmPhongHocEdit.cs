using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmPhongHocEdit : Form
{
    private readonly IPhongHocService _service;
    private readonly PhongHocDto? _editingPhongHoc;
    private readonly bool _isEditMode;

    public FrmPhongHocEdit(IPhongHocService service, PhongHocDto? editingPhongHoc = null)
    {
        InitializeComponent();
        _service = service;
        _editingPhongHoc = editingPhongHoc;
        _isEditMode = editingPhongHoc != null;

        LoadFormData();
    }

    private void LoadFormData()
    {
        cboTrangThai.DataSource = Enum.GetValues(typeof(TrangThaiHoatDong));

        if (_isEditMode && _editingPhongHoc != null)
        {
            lblTitle.Text = "CẬP NHẬT PHÒNG HỌC";
            Text = "Cập nhật thông tin phòng học";
            txtMaPhong.Text = _editingPhongHoc.MaPhong;
            txtMaPhong.ReadOnly = true;
            txtTenPhong.Text = _editingPhongHoc.TenPhong;
            nudSucChua.Value = _editingPhongHoc.SucChua > 0 ? _editingPhongHoc.SucChua : 30;
            cboTrangThai.SelectedItem = _editingPhongHoc.TrangThai;
        }
        else
        {
            lblTitle.Text = "THÊM MỚI PHÒNG HỌC";
            Text = "Thêm phòng học mới";
            nudSucChua.Value = 35;
            cboTrangThai.SelectedItem = TrangThaiHoatDong.DangHoatDong;
        }
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        errorProvider.Clear();
        lblError.Text = string.Empty;

        var maPhong = txtMaPhong.Text.Trim();
        var tenPhong = txtTenPhong.Text.Trim();
        var sucChua = (int)nudSucChua.Value;

        bool hasError = false;
        if (string.IsNullOrWhiteSpace(maPhong))
        {
            errorProvider.SetError(txtMaPhong, "Mã phòng học là bắt buộc.");
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(tenPhong))
        {
            errorProvider.SetError(txtTenPhong, "Tên phòng học là bắt buộc.");
            hasError = true;
        }

        if (sucChua <= 0)
        {
            errorProvider.SetError(nudSucChua, "Sức chứa phòng học phải lớn hơn 0.");
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
            var dto = new PhongHocDto
            {
                Id = _isEditMode && _editingPhongHoc != null ? _editingPhongHoc.Id : 0,
                MaPhong = maPhong,
                TenPhong = tenPhong,
                SucChua = sucChua,
                TrangThai = (TrangThaiHoatDong)(cboTrangThai.SelectedItem ?? TrangThaiHoatDong.DangHoatDong)
            };

            OperationResult<PhongHocDto> result;
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
                    errorProvider.SetError(txtMaPhong, result.Message);
                }
                else if (result.Error?.Code == ErrorCodes.INVALID_RANGE)
                {
                    errorProvider.SetError(nudSucChua, result.Message);
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
