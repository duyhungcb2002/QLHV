using QLHV.Contracts.Dtos;
using QLHV.Contracts.Results;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmKhoaHocMonHocEdit : Form
{
    private readonly IKhoaHocMonHocService _service;
    private readonly IMonHocService _monHocService;
    private readonly KhoaHocDto _khoaHoc;
    private readonly KhoaHocMonHocDto? _editingRelation;
    private readonly bool _isEditMode;

    public FrmKhoaHocMonHocEdit(
        IKhoaHocMonHocService service,
        IMonHocService monHocService,
        KhoaHocDto khoaHoc,
        KhoaHocMonHocDto? editingRelation = null)
    {
        InitializeComponent();
        _service = service;
        _monHocService = monHocService;
        _khoaHoc = khoaHoc;
        _editingRelation = editingRelation;
        _isEditMode = editingRelation != null;

        Load += async (_, _) => await InitializeFormDataAsync();
    }

    private async Task InitializeFormDataAsync()
    {
        lblKhoaHocVal.Text = $"[{_khoaHoc.MaKhoaHoc}] {_khoaHoc.TenKhoaHoc}";

        var monHocResult = await _monHocService.GetAllAsync();
        var subjects = monHocResult.Data ?? Array.Empty<MonHocDto>();

        cboMonHoc.DisplayMember = "TenMonHoc";
        cboMonHoc.ValueMember = "Id";
        cboMonHoc.DataSource = subjects.ToList();

        if (_isEditMode && _editingRelation != null)
        {
            lblTitle.Text = "CẬP NHẬT MÔN HỌC TRONG KHÓA";
            Text = "Cập nhật môn trong khóa";
            cboMonHoc.SelectedValue = _editingRelation.MonHocId;
            cboMonHoc.Enabled = false; // Không đổi môn học khi sửa
            nudThuTu.Value = _editingRelation.ThuTu > 0 ? _editingRelation.ThuTu : 1;
            nudSoBuoi.Value = _editingRelation.SoBuoiDuKien > 0 ? _editingRelation.SoBuoiDuKien : 15;
        }
        else
        {
            lblTitle.Text = "GÁN MÔN HỌC VÀO KHÓA HỌC";
            Text = "Thêm môn vào khóa";
            if (subjects.Count > 0)
            {
                cboMonHoc.SelectedIndex = 0;
                var currentMon = cboMonHoc.SelectedItem as MonHocDto;
                if (currentMon != null && currentMon.SoBuoiMacDinh > 0)
                    nudSoBuoi.Value = currentMon.SoBuoiMacDinh;
            }

            cboMonHoc.SelectedIndexChanged += (_, _) =>
            {
                var cur = cboMonHoc.SelectedItem as MonHocDto;
                if (cur != null && cur.SoBuoiMacDinh > 0)
                {
                    nudSoBuoi.Value = cur.SoBuoiMacDinh;
                }
            };
        }
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        errorProvider.Clear();
        lblError.Text = string.Empty;

        if (cboMonHoc.SelectedValue == null)
        {
            errorProvider.SetError(cboMonHoc, "Vui lòng chọn môn học.");
            lblError.Text = "Vui lòng chọn một môn học.";
            return;
        }

        int monHocId = (int)cboMonHoc.SelectedValue;
        int thuTu = (int)nudThuTu.Value;
        int soBuoi = (int)nudSoBuoi.Value;

        btnSave.Enabled = false;

        try
        {
            var dto = new KhoaHocMonHocDto
            {
                KhoaHocId = _khoaHoc.Id,
                MonHocId = monHocId,
                ThuTu = thuTu,
                SoBuoiDuKien = soBuoi
            };

            OperationResult<KhoaHocMonHocDto> result;
            if (_isEditMode)
            {
                result = await _service.UpdateThuTuSoBuoiAsync(dto);
            }
            else
            {
                result = await _service.AddMonHocAsync(dto);
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
                    errorProvider.SetError(cboMonHoc, result.Message);
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
