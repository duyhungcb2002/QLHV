using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmGiangVien : Form
{
    private readonly IGiangVienService _service;
    private readonly ICurrentUserService _currentUserService;
    private readonly bool _canManage;
    private IReadOnlyList<GiangVienDto> _cachedList = Array.Empty<GiangVienDto>();

    public FrmGiangVien(IGiangVienService service, ICurrentUserService currentUserService)
    {
        InitializeComponent();
        _service = service;
        _currentUserService = currentUserService;
        _canManage = _currentUserService.HasPermission(Permission.CatalogManage);

        ConfigureGrid();
        ConfigureFilters();
        ApplyPermissions();

        dgvGiangVien.SelectionChanged += (_, _) => UpdateButtonStates();
        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnSearch_Click(s, e); };
        Load += async (_, _) => await LoadDataAsync();
    }

    private void ConfigureGrid()
    {
        dgvGiangVien.AutoGenerateColumns = false;
        dgvGiangVien.CellFormatting += (s, e) =>
        {
            if (dgvGiangVien.Columns[e.ColumnIndex].Name == "colTrangThai" && e.Value is TrangThaiHoatDong st)
            {
                e.Value = st == TrangThaiHoatDong.DangHoatDong ? "Đang hoạt động" : "Ngừng hoạt động";
                e.FormattingApplied = true;
            }
        };
    }

    private void ConfigureFilters()
    {
        cboFilterStatus.Items.Clear();
        cboFilterStatus.Items.Add("Tất cả");
        cboFilterStatus.Items.Add("Đang hoạt động");
        cboFilterStatus.Items.Add("Ngừng hoạt động");
        cboFilterStatus.SelectedIndex = 0;
    }

    private void ApplyPermissions()
    {
        btnAdd.Visible = _canManage;
        btnEdit.Visible = _canManage;
        btnDeactivate.Visible = _canManage;
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        bool hasSelection = dgvGiangVien.CurrentRow != null;
        btnEdit.Enabled = _canManage && hasSelection;
        btnDeactivate.Enabled = _canManage && hasSelection;
    }

    private async Task LoadDataAsync()
    {
        Cursor = Cursors.WaitCursor;
        try
        {
            var filter = BuildFilter();
            var result = await _service.GetAllAsync(filter);
            if (result.IsSuccess)
            {
                _cachedList = result.Data ?? Array.Empty<GiangVienDto>();
                dgvGiangVien.DataSource = _cachedList.ToList();
            }
            else
            {
                MessageBox.Show(result.Message, "Lỗi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            Cursor = Cursors.Default;
            UpdateButtonStates();
        }
    }

    private GiangVienFilter BuildFilter()
    {
        var filter = new GiangVienFilter
        {
            TuKhoa = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text.Trim()
        };

        if (cboFilterStatus.SelectedIndex == 1)
            filter.TrangThai = TrangThaiHoatDong.DangHoatDong;
        else if (cboFilterStatus.SelectedIndex == 2)
            filter.TrangThai = TrangThaiHoatDong.NgungHoatDong;

        return filter;
    }

    private async void btnSearch_Click(object? sender, EventArgs e)
    {
        await LoadDataAsync();
    }

    private async void btnRefresh_Click(object? sender, EventArgs e)
    {
        txtSearch.Clear();
        cboFilterStatus.SelectedIndex = 0;
        await LoadDataAsync();
    }

    private async void btnAdd_Click(object? sender, EventArgs e)
    {
        if (!_canManage) return;

        using var editForm = new FrmGiangVienEdit(_service);
        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async void btnEdit_Click(object? sender, EventArgs e)
    {
        if (!_canManage || dgvGiangVien.CurrentRow == null) return;

        var selected = dgvGiangVien.CurrentRow.DataBoundItem as GiangVienDto;
        if (selected == null) return;

        using var editForm = new FrmGiangVienEdit(_service, selected);
        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async void btnDeactivate_Click(object? sender, EventArgs e)
    {
        if (!_canManage || dgvGiangVien.CurrentRow == null) return;

        var selected = dgvGiangVien.CurrentRow.DataBoundItem as GiangVienDto;
        if (selected == null) return;

        if (selected.TrangThai == TrangThaiHoatDong.NgungHoatDong)
        {
            MessageBox.Show("Giảng viên này đã ở trạng thái ngừng hoạt động.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Bạn có chắc chắn muốn ngừng hoạt động giảng viên [{selected.MaGiangVien}] - {selected.HoTen} không?",
            "Xác nhận ngừng hoạt động",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            var result = await _service.DeactivateAsync(selected.Id);
            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show(result.Message, "Lỗi thao tác", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
