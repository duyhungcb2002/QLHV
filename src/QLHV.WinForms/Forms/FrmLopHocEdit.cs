using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmLopHocEdit : Form
{
    private readonly ILopHocService _lopHocService;
    private readonly IKhoaHocService _khoaHocService;
    private readonly IGiangVienService _giangVienService;
    private readonly LopHocDto? _existingItem;

    public FrmLopHocEdit(
        ILopHocService lopHocService,
        IKhoaHocService khoaHocService,
        IGiangVienService giangVienService,
        LopHocDto? existingItem)
    {
        _lopHocService = lopHocService;
        _khoaHocService = khoaHocService;
        _giangVienService = giangVienService;
        _existingItem = existingItem;

        InitializeComponent();

        Load += async (_, _) =>
        {
            await LoadKhoaHocComboAsync();
            await LoadGiangVienComboAsync();
            PopulateData();
        };
    }

    private async Task LoadKhoaHocComboAsync()
    {
        var res = await _khoaHocService.GetAllAsync(new KhoaHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (res.IsSuccess && res.Data != null)
        {
            var list = res.Data.ToList();
            if (_existingItem != null && !list.Any(x => x.Id == _existingItem.KhoaHocId))
            {
                var oldKhoaHocRes = await _khoaHocService.GetByIdAsync(_existingItem.KhoaHocId);
                if (oldKhoaHocRes.IsSuccess && oldKhoaHocRes.Data != null)
                {
                    list.Insert(0, oldKhoaHocRes.Data);
                }
            }

            cboKhoaHoc.DataSource = list;
            cboKhoaHoc.DisplayMember = "TenKhoaHoc";
            cboKhoaHoc.ValueMember = "Id";

            if (_existingItem != null)
            {
                cboKhoaHoc.SelectedValue = _existingItem.KhoaHocId;
            }
        }
    }

    private async Task LoadGiangVienComboAsync()
    {
        var res = await _giangVienService.GetAllAsync(new GiangVienFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (res.IsSuccess && res.Data != null)
        {
            var list = res.Data.ToList();
            if (_existingItem != null && _existingItem.GiangVienChuNhiemId.HasValue &&
                !list.Any(x => x.Id == _existingItem.GiangVienChuNhiemId.Value))
            {
                var oldGvRes = await _giangVienService.GetByIdAsync(_existingItem.GiangVienChuNhiemId.Value);
                if (oldGvRes.IsSuccess && oldGvRes.Data != null)
                {
                    list.Insert(0, oldGvRes.Data);
                }
            }

            cboGiangVienChuNhiem.DataSource = list;
            cboGiangVienChuNhiem.DisplayMember = "HoTen";
            cboGiangVienChuNhiem.ValueMember = "Id";

            if (_existingItem != null && _existingItem.GiangVienChuNhiemId.HasValue)
            {
                cboGiangVienChuNhiem.SelectedValue = _existingItem.GiangVienChuNhiemId.Value;
            }
        }
    }

    private void PopulateData()
    {
        if (_existingItem != null)
        {
            Text = "Cập nhật Lớp học";
            txtMaLop.Text = _existingItem.MaLopHoc;
            txtMaLop.Enabled = false;
            txtTenLop.Text = _existingItem.TenLopHoc;
            cboKhoaHoc.SelectedValue = _existingItem.KhoaHocId;
            if (_existingItem.GiangVienChuNhiemId.HasValue)
            {
                cboGiangVienChuNhiem.SelectedValue = _existingItem.GiangVienChuNhiemId.Value;
            }
            dtpNgayBatDau.Value = _existingItem.NgayBatDau;
            if (_existingItem.NgayKetThuc.HasValue)
            {
                dtpNgayKetThuc.Value = _existingItem.NgayKetThuc.Value;
                chkCoNgayKetThuc.Checked = true;
            }
            else
            {
                chkCoNgayKetThuc.Checked = false;
            }
            nudSiSoToiDa.Value = _existingItem.SiSoToiDa;
            txtGhiChu.Text = _existingItem.GhiChu;
            chkDangHoatDong.Checked = _existingItem.TrangThai == TrangThaiHoatDong.DangHoatDong;
            chkDangHoatDong.Enabled = false;
        }
        else
        {
            Text = "Thêm mới Lớp học";
            txtMaLop.Enabled = true;
            dtpNgayBatDau.Value = DateTime.Today;
            nudSiSoToiDa.Value = 30;
            chkCoNgayKetThuc.Checked = false;
            chkDangHoatDong.Checked = true;
            chkDangHoatDong.Enabled = false;
        }
        ToggleNgayKetThuc();
    }

    private void chkCoNgayKetThuc_CheckedChanged(object? sender, EventArgs e) => ToggleNgayKetThuc();

    private void ToggleNgayKetThuc()
    {
        dtpNgayKetThuc.Enabled = chkCoNgayKetThuc.Checked;
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        if (cboKhoaHoc.SelectedValue is not int khId || khId <= 0)
        {
            MessageBox.Show("Vui lòng chọn khóa học.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboGiangVienChuNhiem.SelectedValue is not int gvId || gvId <= 0)
        {
            MessageBox.Show("Vui lòng chọn giảng viên chủ nhiệm.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var dto = new LopHocDto
        {
            Id = _existingItem?.Id ?? 0,
            MaLopHoc = txtMaLop.Text.Trim(),
            TenLopHoc = txtTenLop.Text.Trim(),
            KhoaHocId = khId,
            GiangVienChuNhiemId = gvId,
            NgayBatDau = dtpNgayBatDau.Value.Date,
            NgayKetThuc = chkCoNgayKetThuc.Checked ? dtpNgayKetThuc.Value.Date : null,
            SiSoToiDa = (int)nudSiSoToiDa.Value,
            GhiChu = txtGhiChu.Text.Trim(),
            TrangThai = chkDangHoatDong.Checked ? TrangThaiHoatDong.DangHoatDong : TrangThaiHoatDong.NgungHoatDong
        };

        var res = _existingItem == null
            ? await _lopHocService.CreateAsync(dto)
            : await _lopHocService.UpdateAsync(dto);

        if (res.IsSuccess)
        {
            MessageBox.Show("Lưu lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show(res.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
