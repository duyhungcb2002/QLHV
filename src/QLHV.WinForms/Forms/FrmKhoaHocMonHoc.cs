using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmKhoaHocMonHoc : Form
{
    private readonly IKhoaHocMonHocService _service;
    private readonly IMonHocService _monHocService;
    private readonly ICurrentUserService _currentUserService;
    private readonly KhoaHocDto _khoaHoc;
    private readonly bool _canManage;

    public FrmKhoaHocMonHoc(
        IKhoaHocMonHocService service,
        IMonHocService monHocService,
        ICurrentUserService currentUserService,
        KhoaHocDto khoaHoc)
    {
        InitializeComponent();
        _service = service;
        _monHocService = monHocService;
        _currentUserService = currentUserService;
        _khoaHoc = khoaHoc;
        _canManage = _currentUserService.HasPermission(Permission.CatalogManage);

        lblCourseInfo.Text = $"Khóa học: [{_khoaHoc.MaKhoaHoc}] {_khoaHoc.TenKhoaHoc}";

        dgvSubjects.AutoGenerateColumns = false;
        dgvSubjects.SelectionChanged += (_, _) => UpdateButtonStates();

        ApplyPermissions();
        Load += async (_, _) => await LoadDataAsync();
    }

    private void ApplyPermissions()
    {
        btnAdd.Visible = _canManage;
        btnEdit.Visible = _canManage;
        btnRemove.Visible = _canManage;
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        bool hasSelection = dgvSubjects.CurrentRow != null;
        btnEdit.Enabled = _canManage && hasSelection;
        btnRemove.Enabled = _canManage && hasSelection;
    }

    private async Task LoadDataAsync()
    {
        Cursor = Cursors.WaitCursor;
        try
        {
            var result = await _service.GetByKhoaHocIdAsync(_khoaHoc.Id);
            if (result.IsSuccess)
            {
                dgvSubjects.DataSource = result.Data?.ToList() ?? new List<KhoaHocMonHocDto>();
            }
            else
            {
                MessageBox.Show(result.Message, "Lỗi tải môn học", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            Cursor = Cursors.Default;
            UpdateButtonStates();
        }
    }

    private async void btnAdd_Click(object? sender, EventArgs e)
    {
        if (!_canManage) return;

        using var editForm = new FrmKhoaHocMonHocEdit(_service, _monHocService, _khoaHoc);
        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async void btnEdit_Click(object? sender, EventArgs e)
    {
        if (!_canManage || dgvSubjects.CurrentRow == null) return;

        var selected = dgvSubjects.CurrentRow.DataBoundItem as KhoaHocMonHocDto;
        if (selected == null) return;

        using var editForm = new FrmKhoaHocMonHocEdit(_service, _monHocService, _khoaHoc, selected);
        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
            await LoadDataAsync();
        }
    }

    private async void btnRemove_Click(object? sender, EventArgs e)
    {
        if (!_canManage || dgvSubjects.CurrentRow == null) return;

        var selected = dgvSubjects.CurrentRow.DataBoundItem as KhoaHocMonHocDto;
        if (selected == null) return;

        var confirm = MessageBox.Show(
            $"Bạn có chắc chắn muốn xóa môn học [{selected.MaMonHoc}] {selected.TenMonHoc} khỏi khóa học này?",
            "Xác nhận xóa môn học",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            var result = await _service.RemoveMonHocAsync(selected.KhoaHocId, selected.MonHocId);
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

    private void btnClose_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
