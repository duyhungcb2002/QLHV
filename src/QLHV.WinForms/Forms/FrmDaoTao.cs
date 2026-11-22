using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Security;
using QLHV.Core.Services;
using QLHV.WinForms.Configuration;

namespace QLHV.WinForms.Forms;

public partial class FrmDaoTao : Form
{
    private readonly ILopHocService _lopHocService;
    private readonly IDangKyLopHocService _dangKyService;
    private readonly ILichHocService _lichHocService;
    private readonly IKhoaHocService _khoaHocService;
    private readonly IHocVienService _hocVienService;
    private readonly IMonHocService _monHocService;
    private readonly IGiangVienService _giangVienService;
    private readonly IPhongHocService _phongHocService;
    private readonly ICurrentUserService _currentUserService;

    public FrmDaoTao(
        ILopHocService lopHocService,
        IDangKyLopHocService dangKyService,
        ILichHocService lichHocService,
        IKhoaHocService khoaHocService,
        IHocVienService hocVienService,
        IMonHocService monHocService,
        IGiangVienService giangVienService,
        IPhongHocService phongHocService,
        ICurrentUserService currentUserService)
    {
        _lopHocService = lopHocService;
        _dangKyService = dangKyService;
        _lichHocService = lichHocService;
        _khoaHocService = khoaHocService;
        _hocVienService = hocVienService;
        _monHocService = monHocService;
        _giangVienService = giangVienService;
        _phongHocService = phongHocService;
        _currentUserService = currentUserService;

        InitializeComponent();
        ConfigureGridFormats();
        SetupPermissions();

        Load += async (_, _) =>
        {
            await LoadKhoaHocCombosAsync();
            await LoadLopHocDataAsync();
        };

        tabControlDaoTao.SelectedIndexChanged += async (_, _) =>
        {
            if (tabControlDaoTao.SelectedTab == tabLopHoc)
            {
                await LoadLopHocDataAsync();
            }
            else if (tabControlDaoTao.SelectedTab == tabXepLop)
            {
                await LoadLopCombosForXepLopAsync();
                await LoadDangKyDataAsync();
            }
            else if (tabControlDaoTao.SelectedTab == tabLichHoc)
            {
                await LoadLopCombosForLichHocAsync();
                await LoadLichHocDataAsync();
            }
        };
    }

    private void ConfigureGridFormats()
    {
        dgvLopHoc.ApplyStandardGridStyle();
        dgvDangKy.ApplyStandardGridStyle();
        dgvLichHoc.ApplyStandardGridStyle();

        // 1. Grid Lớp học formatters
        dgvLopHoc.CellFormatting += (s, e) =>
        {
            if (dgvLopHoc.Columns[e.ColumnIndex].Name == "colLop_NgayBatDau" && e.Value is DateTime dt1)
            {
                e.Value = dt1.ToString("dd/MM/yyyy");
                e.FormattingApplied = true;
            }
            else if (dgvLopHoc.Columns[e.ColumnIndex].Name == "colLop_NgayKetThuc")
            {
                if (e.Value is DateTime dt2)
                {
                    e.Value = dt2.ToString("dd/MM/yyyy");
                }
                else
                {
                    e.Value = "-";
                }
                e.FormattingApplied = true;
            }
            else if (dgvLopHoc.Columns[e.ColumnIndex].Name == "colLop_GiangVien" && (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString())))
            {
                e.Value = "Chưa phân công";
                e.FormattingApplied = true;
            }
        };

        // 2. Grid Xếp lớp formatters
        dgvDangKy.CellFormatting += (s, e) =>
        {
            if (dgvDangKy.Columns[e.ColumnIndex].Name == "colDk_NgayDangKy" && e.Value is DateTime dt)
            {
                e.Value = dt.ToString("dd/MM/yyyy");
                e.FormattingApplied = true;
            }
            else if ((dgvDangKy.Columns[e.ColumnIndex].Name == "colDk_HocPhiPhaiThu" ||
                      dgvDangKy.Columns[e.ColumnIndex].Name == "colDk_DaThu" ||
                      dgvDangKy.Columns[e.ColumnIndex].Name == "colDk_ConNo") && e.Value is decimal val)
            {
                e.Value = val.ToString("N0") + " đ";
                e.FormattingApplied = true;
            }
        };

        // 3. Grid Lịch học formatters
        dgvLichHoc.CellFormatting += (s, e) =>
        {
            if (dgvLichHoc.Columns[e.ColumnIndex].Name == "colLich_NgayHoc" && e.Value is DateTime dt)
            {
                e.Value = dt.ToString("dd/MM/yyyy");
                e.FormattingApplied = true;
            }
        };
    }

    private void SetupPermissions()
    {
        bool canManage = _currentUserService.HasPermission(Permission.TrainingManage);
        btnThemLop.Enabled = canManage;
        btnSuaLop.Enabled = canManage;
        btnNgungLop.Enabled = canManage;

        btnXepLop.Enabled = canManage;
        btnHuyXepLop.Enabled = canManage;

        btnThemLich.Enabled = canManage;
        btnSuaLich.Enabled = canManage;
        btnXoaLich.Enabled = canManage;
    }

    // ==========================================
    // TAB 1: LỚP HỌC
    // ==========================================
    private async Task LoadKhoaHocCombosAsync()
    {
        var res = await _khoaHocService.GetAllAsync(new KhoaHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (res.IsSuccess && res.Data != null)
        {
            var list = res.Data.ToList();
            list.Insert(0, new KhoaHocDto { Id = 0, TenKhoaHoc = "-- Tất cả khóa học --" });
            cboFilterKhoaHoc.DataSource = list;
            cboFilterKhoaHoc.DisplayMember = "TenKhoaHoc";
            cboFilterKhoaHoc.ValueMember = "Id";
        }
    }

    private async Task LoadLopHocDataAsync()
    {
        int? khId = (int?)cboFilterKhoaHoc.SelectedValue;
        if (khId <= 0) khId = null;

        var filter = new LopHocFilter
        {
            TuKhoa = txtTimKiemLop.Text.Trim(),
            KhoaHocId = khId
        };

        var res = await _lopHocService.GetAllAsync(filter);
        if (res.IsSuccess && res.Data != null)
        {
            dgvLopHoc.DataSource = res.Data.Select(x => new
            {
                x.Id,
                x.MaLopHoc,
                x.TenLopHoc,
                x.TenKhoaHoc,
                HoTenGiangVienChuNhiem = !string.IsNullOrWhiteSpace(x.HoTenGiangVienChuNhiem) ? x.HoTenGiangVienChuNhiem : "Chưa phân công",
                x.NgayBatDau,
                x.NgayKetThuc,
                SiSo = $"{x.SiSoHienTai}/{x.SiSoToiDa}",
                TrangThai = x.TrangThai == TrangThaiHoatDong.DangHoatDong ? "Đang hoạt động" : "Ngừng hoạt động",
                x.GhiChu
            }).ToList();
        }
    }

    private async void btnTimLop_Click(object? sender, EventArgs e) => await LoadLopHocDataAsync();

    private async void btnThemLop_Click(object? sender, EventArgs e)
    {
        using var editForm = new FrmLopHocEdit(_lopHocService, _khoaHocService, _giangVienService, null);
        if (editForm.ShowDialog() == DialogResult.OK)
        {
            await LoadLopHocDataAsync();
        }
    }

    private async void btnSuaLop_Click(object? sender, EventArgs e)
    {
        if (dgvLopHoc.CurrentRow == null) return;
        int id = Convert.ToInt32(dgvLopHoc.CurrentRow.Cells["colLop_Id"].Value);
        var lopRes = await _lopHocService.GetByIdAsync(id);
        if (lopRes.IsSuccess && lopRes.Data != null)
        {
            using var editForm = new FrmLopHocEdit(_lopHocService, _khoaHocService, _giangVienService, lopRes.Data);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                await LoadLopHocDataAsync();
            }
        }
    }

    private async void btnNgungLop_Click(object? sender, EventArgs e)
    {
        if (dgvLopHoc.CurrentRow == null) return;
        int id = Convert.ToInt32(dgvLopHoc.CurrentRow.Cells["colLop_Id"].Value);
        var confirm = MessageBox.Show("Bạn có chắc chắn muốn chuyển trạng thái ngừng hoạt động cho lớp học này?",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            var res = await _lopHocService.DeactivateAsync(id);
            if (res.IsSuccess)
            {
                MessageBox.Show("Cập nhật thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadLopHocDataAsync();
            }
            else
            {
                MessageBox.Show(res.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ==========================================
    // TAB 2: XẾP LỚP HỌC VIÊN
    // ==========================================
    private async Task LoadLopCombosForXepLopAsync()
    {
        var res = await _lopHocService.GetAllAsync(new LopHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (res.IsSuccess && res.Data != null)
        {
            var list = res.Data.ToList();
            cboXepLop_LopHoc.DataSource = list;
            cboXepLop_LopHoc.DisplayMember = "TenLopHoc";
            cboXepLop_LopHoc.ValueMember = "Id";
        }
    }

    private async Task LoadDangKyDataAsync()
    {
        if (cboXepLop_LopHoc.SelectedValue is not int lopId || lopId <= 0) return;

        var res = await _dangKyService.GetAllAsync(new DangKyLopHocFilter
        {
            LopHocId = lopId,
            TuKhoa = txtTimKiemHocVien.Text.Trim()
        });

        if (res.IsSuccess && res.Data != null)
        {
            dgvDangKy.DataSource = res.Data.Select(x => new
            {
                x.Id,
                x.MaHocVien,
                x.HoTenHocVien,
                x.DienThoaiHocVien,
                x.NgayDangKy,
                x.HocPhiPhaiThu,
                x.DaThu,
                x.ConNo,
                TrangThai = x.TrangThai == TrangThaiHoatDong.DangHoatDong ? "Đang học" : "Đã hủy",
                x.GhiChu
            }).ToList();
        }
    }

    private async void cboXepLop_LopHoc_SelectedIndexChanged(object? sender, EventArgs e) => await LoadDangKyDataAsync();
    private async void btnTimDangKy_Click(object? sender, EventArgs e) => await LoadDangKyDataAsync();

    private async void btnXepLop_Click(object? sender, EventArgs e)
    {
        if (cboXepLop_LopHoc.SelectedValue is not int lopId || lopId <= 0)
        {
            MessageBox.Show("Vui lòng chọn lớp học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var existingEnrollments = await _dangKyService.GetAllAsync(new DangKyLopHocFilter
        {
            LopHocId = lopId,
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });
        var excludeIds = existingEnrollments.IsSuccess && existingEnrollments.Data != null
            ? existingEnrollments.Data.Select(x => x.HocVienId).ToList()
            : new List<int>();

        using var chonHvForm = new FrmChonHocVien(_hocVienService, excludeIds);
        if (chonHvForm.ShowDialog() == DialogResult.OK && chonHvForm.SelectedHocVienId > 0)
        {
            var res = await _dangKyService.EnrollAsync(lopId, chonHvForm.SelectedHocVienId);
            if (res.IsSuccess)
            {
                MessageBox.Show("Xếp học viên vào lớp thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDangKyDataAsync();
            }
            else
            {
                MessageBox.Show(res.Message, "Lỗi xếp lớp", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void btnHuyXepLop_Click(object? sender, EventArgs e)
    {
        if (dgvDangKy.CurrentRow == null) return;
        int id = Convert.ToInt32(dgvDangKy.CurrentRow.Cells["colDk_Id"].Value);
        var confirm = MessageBox.Show("Bạn có chắc chắn muốn hủy lượt xếp lớp này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm == DialogResult.Yes)
        {
            var res = await _dangKyService.CancelEnrollmentAsync(id);
            if (res.IsSuccess)
            {
                MessageBox.Show("Đã hủy xếp lớp thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDangKyDataAsync();
            }
            else
            {
                MessageBox.Show(res.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ==========================================
    // TAB 3: LỊCH HỌC
    // ==========================================
    private async Task LoadLopCombosForLichHocAsync()
    {
        var res = await _lopHocService.GetAllAsync(new LopHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (res.IsSuccess && res.Data != null)
        {
            var list = res.Data.ToList();
            cboLichHoc_LopHoc.DataSource = list;
            cboLichHoc_LopHoc.DisplayMember = "TenLopHoc";
            cboLichHoc_LopHoc.ValueMember = "Id";
        }
    }

    private async Task LoadLichHocDataAsync()
    {
        if (cboLichHoc_LopHoc.SelectedValue is not int lopId || lopId <= 0) return;

        var res = await _lichHocService.GetAllAsync(new LichHocFilter
        {
            LopHocId = lopId,
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        if (res.IsSuccess && res.Data != null)
        {
            dgvLichHoc.DataSource = res.Data.Select(x => new
            {
                x.Id,
                x.TenMonHoc,
                x.HoTenGiangVien,
                x.TenPhong,
                x.NgayHoc,
                ThoiGian = $"{x.GioBatDau:hh\\:mm} - {x.GioKetThuc:hh\\:mm}",
                x.NoiDung
            }).ToList();
        }
        else
        {
            dgvLichHoc.DataSource = null;
            if (!res.IsSuccess)
            {
                MessageBox.Show(res.Message, "Lỗi tải lịch học", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void cboLichHoc_LopHoc_SelectedIndexChanged(object? sender, EventArgs e) => await LoadLichHocDataAsync();

    private async void btnThemLich_Click(object? sender, EventArgs e)
    {
        if (cboLichHoc_LopHoc.SelectedValue is not int lopId || lopId <= 0)
        {
            MessageBox.Show("Vui lòng chọn lớp học trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var form = new FrmLichHocEdit(_lichHocService, _monHocService, _giangVienService, _phongHocService, lopId, null);
        if (form.ShowDialog() == DialogResult.OK)
        {
            await LoadLichHocDataAsync();
        }
    }

    private async void btnSuaLich_Click(object? sender, EventArgs e)
    {
        if (dgvLichHoc.CurrentRow == null) return;
        int id = Convert.ToInt32(dgvLichHoc.CurrentRow.Cells["colLich_Id"].Value);
        if (cboLichHoc_LopHoc.SelectedValue is not int lopId) return;

        var curList = await _lichHocService.GetAllAsync(new LichHocFilter { LopHocId = lopId });
        var target = curList.Data?.FirstOrDefault(x => x.Id == id);
        if (target != null)
        {
            if (target.TrangThai != TrangThaiHoatDong.DangHoatDong)
            {
                MessageBox.Show("Buổi học này đã bị hủy, không thể chỉnh sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var form = new FrmLichHocEdit(_lichHocService, _monHocService, _giangVienService, _phongHocService, lopId, target);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadLichHocDataAsync();
            }
        }
    }

    private async void btnXoaLich_Click(object? sender, EventArgs e)
    {
        if (dgvLichHoc.CurrentRow == null) return;
        int id = Convert.ToInt32(dgvLichHoc.CurrentRow.Cells["colLich_Id"].Value);
        var confirm = MessageBox.Show("Bạn có chắc chắn muốn hủy buổi học này khỏi lịch?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm == DialogResult.Yes)
        {
            var res = await _lichHocService.DeleteAsync(id);
            if (res.IsSuccess)
            {
                MessageBox.Show("Đã hủy buổi học thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadLichHocDataAsync();
            }
            else
            {
                MessageBox.Show(res.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
