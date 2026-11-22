using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Services;
using QLHV.WinForms.Configuration;

namespace QLHV.WinForms.Forms;

public partial class FrmChonHocVien : Form
{
    private readonly IHocVienService _hocVienService;
    private readonly HashSet<int> _excludeHocVienIds;
    public int SelectedHocVienId { get; private set; }

    public FrmChonHocVien(IHocVienService hocVienService, IEnumerable<int>? excludeHocVienIds = null)
    {
        _hocVienService = hocVienService;
        _excludeHocVienIds = excludeHocVienIds != null ? new HashSet<int>(excludeHocVienIds) : new HashSet<int>();
        InitializeComponent();
        dgvHocVien.ApplyStandardGridStyle();

        Load += async (_, _) => await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var filter = new HocVienFilter
        {
            TuKhoa = txtSearch.Text.Trim(),
            TrangThai = TrangThaiHoatDong.DangHoatDong
        };

        var res = await _hocVienService.GetAllAsync(filter);
        if (res.IsSuccess && res.Data != null)
        {
            var filtered = res.Data.Where(x => !_excludeHocVienIds.Contains(x.Id)).ToList();
            dgvHocVien.DataSource = filtered.Select(x => new
            {
                x.Id,
                x.MaHocVien,
                x.HoTen,
                x.GioiTinh,
                x.DienThoai,
                x.Email,
                x.DiaChi
            }).ToList();
        }
        else
        {
            dgvHocVien.DataSource = null;
        }
    }

    private async void btnSearch_Click(object? sender, EventArgs e) => await LoadDataAsync();

    private void btnSelect_Click(object? sender, EventArgs e)
    {
        if (dgvHocVien.CurrentRow == null)
        {
            MessageBox.Show("Vui lòng chọn một học viên từ danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SelectedHocVienId = Convert.ToInt32(dgvHocVien.CurrentRow.Cells["Id"].Value);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
