using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmLichHocEdit : Form
{
    private readonly ILichHocService _lichHocService;
    private readonly IMonHocService _monHocService;
    private readonly IGiangVienService _giangVienService;
    private readonly IPhongHocService _phongHocService;
    private readonly int _lopHocId;
    private readonly LichHocDto? _existingItem;

    public FrmLichHocEdit(
        ILichHocService lichHocService,
        IMonHocService monHocService,
        IGiangVienService giangVienService,
        IPhongHocService phongHocService,
        int lopHocId,
        LichHocDto? existingItem)
    {
        _lichHocService = lichHocService;
        _monHocService = monHocService;
        _giangVienService = giangVienService;
        _phongHocService = phongHocService;
        _lopHocId = lopHocId;
        _existingItem = existingItem;

        InitializeComponent();

        Load += async (_, _) =>
        {
            await LoadCombosAsync();
            PopulateData();
        };
    }

    private async Task LoadCombosAsync()
    {
        var monRes = await _monHocService.GetAllAsync(new MonHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (monRes.IsSuccess && monRes.Data != null)
        {
            var listMon = monRes.Data.ToList();
            if (_existingItem != null && !listMon.Any(x => x.Id == _existingItem.MonHocId))
            {
                var oldMon = await _monHocService.GetByIdAsync(_existingItem.MonHocId);
                if (oldMon.IsSuccess && oldMon.Data != null) listMon.Insert(0, oldMon.Data);
            }
            cboMonHoc.DataSource = listMon;
            cboMonHoc.DisplayMember = "TenMonHoc";
            cboMonHoc.ValueMember = "Id";
            if (_existingItem != null) cboMonHoc.SelectedValue = _existingItem.MonHocId;
        }

        var gvRes = await _giangVienService.GetAllAsync(new GiangVienFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (gvRes.IsSuccess && gvRes.Data != null)
        {
            var listGv = gvRes.Data.ToList();
            if (_existingItem != null && !listGv.Any(x => x.Id == _existingItem.GiangVienId))
            {
                var oldGv = await _giangVienService.GetByIdAsync(_existingItem.GiangVienId);
                if (oldGv.IsSuccess && oldGv.Data != null) listGv.Insert(0, oldGv.Data);
            }
            cboGiangVien.DataSource = listGv;
            cboGiangVien.DisplayMember = "HoTen";
            cboGiangVien.ValueMember = "Id";
            if (_existingItem != null) cboGiangVien.SelectedValue = _existingItem.GiangVienId;
        }

        var phRes = await _phongHocService.GetAllAsync(new PhongHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (phRes.IsSuccess && phRes.Data != null)
        {
            var listPh = phRes.Data.ToList();
            if (_existingItem != null && !listPh.Any(x => x.Id == _existingItem.PhongHocId))
            {
                var oldPh = await _phongHocService.GetByIdAsync(_existingItem.PhongHocId);
                if (oldPh.IsSuccess && oldPh.Data != null) listPh.Insert(0, oldPh.Data);
            }
            cboPhongHoc.DataSource = listPh;
            cboPhongHoc.DisplayMember = "TenPhong";
            cboPhongHoc.ValueMember = "Id";
            if (_existingItem != null) cboPhongHoc.SelectedValue = _existingItem.PhongHocId;
        }
    }

    private void PopulateData()
    {
        if (_existingItem != null)
        {
            Text = "Cập nhật buổi học";
            cboMonHoc.SelectedValue = _existingItem.MonHocId;
            cboGiangVien.SelectedValue = _existingItem.GiangVienId;
            cboPhongHoc.SelectedValue = _existingItem.PhongHocId;
            dtpNgayHoc.Value = _existingItem.NgayHoc;
            dtpGioBatDau.Value = DateTime.Today.Add(_existingItem.GioBatDau);
            dtpGioKetThuc.Value = DateTime.Today.Add(_existingItem.GioKetThuc);
            txtNoiDung.Text = _existingItem.NoiDung;
        }
        else
        {
            Text = "Thêm buổi học mới";
            dtpNgayHoc.Value = DateTime.Today;
            dtpGioBatDau.Value = DateTime.Today.AddHours(8);
            dtpGioKetThuc.Value = DateTime.Today.AddHours(11);
        }
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        if (cboMonHoc.SelectedValue is not int monId || monId <= 0)
        {
            MessageBox.Show("Vui lòng chọn môn học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboGiangVien.SelectedValue is not int gvId || gvId <= 0)
        {
            MessageBox.Show("Vui lòng chọn giảng viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboPhongHoc.SelectedValue is not int phongId || phongId <= 0)
        {
            MessageBox.Show("Vui lòng chọn phòng học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var batDau = dtpGioBatDau.Value.TimeOfDay;
        var ketThuc = dtpGioKetThuc.Value.TimeOfDay;

        if (ketThuc <= batDau)
        {
            MessageBox.Show("Giờ kết thúc phải sau giờ bắt đầu.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var dto = new LichHocDto
        {
            Id = _existingItem?.Id ?? 0,
            LopHocId = _lopHocId,
            MonHocId = monId,
            GiangVienId = gvId,
            PhongHocId = phongId,
            NgayHoc = dtpNgayHoc.Value.Date,
            GioBatDau = new TimeSpan(batDau.Hours, batDau.Minutes, 0),
            GioKetThuc = new TimeSpan(ketThuc.Hours, ketThuc.Minutes, 0),
            NoiDung = txtNoiDung.Text.Trim(),
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        var res = _existingItem == null
            ? await _lichHocService.CreateAsync(dto)
            : await _lichHocService.UpdateAsync(dto);

        if (res.IsSuccess)
        {
            MessageBox.Show("Lưu buổi học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show(res.Message, "Lỗi lưu lịch", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
