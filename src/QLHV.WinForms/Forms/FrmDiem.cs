using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Security;
using QLHV.Core.Services;
using QLHV.WinForms.Configuration;

namespace QLHV.WinForms.Forms;

public partial class FrmDiem : Form
{
    private readonly IKetQuaHocTapService _ketQuaService;
    private readonly ICurrentUserService _currentUserService;

    public FrmDiem(
        IKetQuaHocTapService ketQuaService,
        ILopHocService _,
        IMonHocService __,
        ICurrentUserService currentUserService)
    {
        _ketQuaService = ketQuaService;
        _currentUserService = currentUserService;

        InitializeComponent();
        dgvDiem.ApplyStandardGridStyle();

        dgvDiem.CellValidating += dgvDiem_CellValidating;

        Load += async (_, _) =>
        {
            await LoadCombosAsync();
        };

        cboLopHoc.SelectedIndexChanged += async (_, _) =>
        {
            await LoadMonHocForLopAsync();
        };
    }

    private async Task LoadCombosAsync()
    {
        btnTaiBangDiem.Enabled = false;
        btnLuuBangDiem.Enabled = false;

        var lopRes = await _ketQuaService.GetLopHocOptionsAsync();
        if (lopRes.IsSuccess && lopRes.Data != null)
        {
            var lops = lopRes.Data.ToList();
            cboLopHoc.DataSource = lops;
            cboLopHoc.DisplayMember = "TenLopHoc";
            cboLopHoc.ValueMember = "Id";

            if (lops.Count > 0)
            {
                btnTaiBangDiem.Enabled = true;
                btnLuuBangDiem.Enabled = true;
                await LoadMonHocForLopAsync();
            }
            else
            {
                cboMonHoc.DataSource = null;
                dgvDiem.Rows.Clear();
            }
        }
        else
        {
            cboLopHoc.DataSource = null;
            cboMonHoc.DataSource = null;
            dgvDiem.Rows.Clear();
            MessageBox.Show(lopRes.Message ?? "Không thể tải danh sách lớp học được phân công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task LoadMonHocForLopAsync()
    {
        if (cboLopHoc.SelectedValue is not int lopId || lopId <= 0)
        {
            cboMonHoc.DataSource = null;
            return;
        }

        var monRes = await _ketQuaService.GetMonHocOptionsAsync(lopId);
        if (monRes.IsSuccess && monRes.Data != null)
        {
            var mons = monRes.Data.ToList();
            cboMonHoc.DataSource = mons;
            cboMonHoc.DisplayMember = "TenMonHoc";
            cboMonHoc.ValueMember = "Id";

            btnTaiBangDiem.Enabled = mons.Count > 0;
            btnLuuBangDiem.Enabled = mons.Count > 0;
        }
        else
        {
            cboMonHoc.DataSource = null;
            btnTaiBangDiem.Enabled = false;
            btnLuuBangDiem.Enabled = false;
        }
    }

    private async void btnTaiBangDiem_Click(object? sender, EventArgs e)
    {
        await LoadBangDiemAsync();
    }

    private async Task LoadBangDiemAsync()
    {
        if (cboLopHoc.SelectedValue is not int lopId || lopId <= 0)
        {
            MessageBox.Show("Vui lòng chọn lớp học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboMonHoc.SelectedValue is not int monId || monId <= 0)
        {
            MessageBox.Show("Vui lòng chọn môn học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var res = await _ketQuaService.GetByLopAndMonAsync(lopId, monId);
        if (res.IsSuccess && res.Data != null)
        {
            dgvDiem.Rows.Clear();
            foreach (var item in res.Data)
            {
                object[] rowData = new object[]
                {
                    item.Id,
                    item.DangKyLopHocId,
                    item.MaHocVien,
                    item.HoTenHocVien,
                    item.DiemTongKet.HasValue ? item.DiemTongKet.Value.ToString("0.00") : string.Empty,
                    item.KetQua,
                    item.GhiChu ?? string.Empty
                };
                int rowIndex = dgvDiem.Rows.Add(rowData);

                // Định dạng hiển thị kết quả
                var row = dgvDiem.Rows[rowIndex];
                if (!item.DiemTongKet.HasValue)
                {
                    row.Cells["colKetQua"].Style.ForeColor = Color.Gray;
                    row.Cells["colKetQua"].Style.Font = new Font(dgvDiem.Font, FontStyle.Italic);
                }
                else if (item.Dat)
                {
                    row.Cells["colKetQua"].Style.ForeColor = Color.Green;
                    row.Cells["colKetQua"].Style.Font = new Font(dgvDiem.Font, FontStyle.Bold);
                }
                else
                {
                    row.Cells["colKetQua"].Style.ForeColor = Color.Red;
                    row.Cells["colKetQua"].Style.Font = new Font(dgvDiem.Font, FontStyle.Regular);
                }
            }
        }
        else
        {
            dgvDiem.Rows.Clear();
            MessageBox.Show(res.Message, "Lỗi tải bảng điểm", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void dgvDiem_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != dgvDiem.Columns["colDiem"]?.Index) return;

        string val = e.FormattedValue?.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(val))
        {
            // Để trống -> Hợp lệ (Chưa nhập điểm)
            return;
        }

        if (!decimal.TryParse(val, out var diem) || diem < 0.00m || diem > 10.00m)
        {
            e.Cancel = true;
            MessageBox.Show("Điểm tổng kết phải là số từ 0.00 đến 10.00 (hoặc để trống nếu chưa nhập).", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void dgvDiem_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != dgvDiem.Columns["colDiem"]?.Index) return;

        var row = dgvDiem.Rows[e.RowIndex];
        string val = row.Cells["colDiem"].Value?.ToString()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(val))
        {
            row.Cells["colKetQua"].Value = "Chưa nhập";
            row.Cells["colKetQua"].Style.ForeColor = Color.Gray;
            row.Cells["colKetQua"].Style.Font = new Font(dgvDiem.Font, FontStyle.Italic);
        }
        else if (decimal.TryParse(val, out var diem))
        {
            if (diem < 0m || diem > 10m)
            {
                row.Cells["colDiem"].Value = string.Empty;
                row.Cells["colKetQua"].Value = "Chưa nhập";
                return;
            }

            bool dat = diem >= 5.00m;
            row.Cells["colKetQua"].Value = dat ? "Đạt" : "Không đạt";
            row.Cells["colKetQua"].Style.ForeColor = dat ? Color.Green : Color.Red;
            row.Cells["colKetQua"].Style.Font = dat ? new Font(dgvDiem.Font, FontStyle.Bold) : new Font(dgvDiem.Font, FontStyle.Regular);
        }
    }

    private async void btnLuuBangDiem_Click(object? sender, EventArgs e)
    {
        if (cboLopHoc.SelectedValue is not int lopId || lopId <= 0 ||
            cboMonHoc.SelectedValue is not int monId || monId <= 0)
        {
            MessageBox.Show("Vui lòng chọn đầy đủ lớp và môn học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var list = new List<KetQuaHocTapDto>();
        foreach (DataGridViewRow row in dgvDiem.Rows)
        {
            if (row.IsNewRow) continue;

            int id = Convert.ToInt32(row.Cells["colId"].Value);
            int dkId = Convert.ToInt32(row.Cells["colDangKyId"].Value);
            string val = row.Cells["colDiem"].Value?.ToString()?.Trim() ?? string.Empty;
            string? ghiChu = row.Cells["colGhiChu"].Value?.ToString()?.Trim();

            if (!string.IsNullOrEmpty(ghiChu) && ghiChu.Length > 250)
            {
                MessageBox.Show($"Ghi chú của học viên ở dòng {row.Index + 1} vượt quá 250 ký tự.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgvDiem.CurrentCell = row.Cells["colGhiChu"];
                return;
            }

            decimal? diemTongKet = null;
            if (!string.IsNullOrEmpty(val))
            {
                if (!decimal.TryParse(val, out var diem) || diem < 0.00m || diem > 10.00m)
                {
                    MessageBox.Show($"Điểm của học viên ở dòng {row.Index + 1} không hợp lệ (phải từ 0 đến 10 hoặc để trống).", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvDiem.CurrentCell = row.Cells["colDiem"];
                    return;
                }
                diemTongKet = diem;
            }

            list.Add(new KetQuaHocTapDto
            {
                Id = id,
                DangKyLopHocId = dkId,
                LopHocId = lopId,
                MonHocId = monId,
                DiemTongKet = diemTongKet,
                GhiChu = ghiChu
            });
        }

        if (list.Count == 0)
        {
            MessageBox.Show("Lớp học chưa có học viên nào để lưu điểm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var res = await _ketQuaService.SaveGradesBatchAsync(list);
        if (res.IsSuccess)
        {
            MessageBox.Show($"Đã lưu bảng điểm thành công ({res.Data} bản ghi thay đổi/cập nhật)!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadBangDiemAsync();
        }
        else
        {
            MessageBox.Show(res.Message, "Lỗi lưu bảng điểm", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
