using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Security;
using QLHV.Core.Services;
using QLHV.WinForms.Configuration;

namespace QLHV.WinForms.Forms;

public partial class FrmHocPhi : Form
{
    private readonly IHocPhiService _hocPhiService;
    private readonly IDangKyLopHocService _dangKyService;
    private readonly ICurrentUserService _currentUserService;

    public FrmHocPhi(
        IHocPhiService hocPhiService,
        ILopHocService _,
        IDangKyLopHocService dangKyService,
        ICurrentUserService currentUserService)
    {
        _hocPhiService = hocPhiService;
        _dangKyService = dangKyService;
        _currentUserService = currentUserService;

        InitializeComponent();
        dgvCongNo.ApplyStandardGridStyle();
        dgvPhieuThu.ApplyStandardGridStyle();

        Load += async (_, _) =>
        {
            await LoadLopCombosAsync();
            await LoadCongNoDataAsync();
            await LoadLichSuThuDataAsync();
        };

        cboLopHoc.SelectedIndexChanged += async (_, _) => await LoadCongNoDataAsync();
    }

    private async Task LoadLopCombosAsync()
    {
        var res = await _hocPhiService.GetLopHocOptionsAsync();
        if (res.IsSuccess && res.Data != null)
        {
            var list = res.Data.ToList();
            cboLopHoc.DataSource = list;
            cboLopHoc.DisplayMember = "TenLopHoc";
            cboLopHoc.ValueMember = "Id";

            btnThuHocPhi.Enabled = list.Count > 0;
        }
        else
        {
            cboLopHoc.DataSource = null;
            btnThuHocPhi.Enabled = false;
            MessageBox.Show(res.Message ?? "Không thể tải danh sách lớp học.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task LoadCongNoDataAsync()
    {
        if (cboLopHoc.SelectedValue is not int lopId || lopId <= 0)
        {
            dgvCongNo.DataSource = null;
            return;
        }

        var res = await _dangKyService.GetCongNoByLopHocAsync(lopId);
        if (res.IsSuccess && res.Data != null)
        {
            dgvCongNo.DataSource = res.Data.Select(x => new
            {
                x.DangKyLopHocId,
                x.MaHocVien,
                x.HoTenHocVien,
                x.DienThoai,
                HocPhiPhaiThu = x.HocPhiPhaiThu.ToString("N0") + " đ",
                DaThu = x.DaThu.ToString("N0") + " đ",
                ConNo = x.ConNo.ToString("N0") + " đ",
                x.TrangThaiCongNo
            }).ToList();
        }
        else
        {
            dgvCongNo.DataSource = null;
            MessageBox.Show(res.Message, "Lỗi tải công nợ lớp", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadLichSuThuDataAsync()
    {
        var res = await _hocPhiService.GetAllAsync(new PhieuThuFilter { TuKhoa = txtTimKiemPhieu.Text.Trim() });
        if (res.IsSuccess && res.Data != null)
        {
            dgvPhieuThu.DataSource = res.Data.Select(x => new
            {
                x.Id,
                x.MaPhieuThu,
                x.TenLopHoc,
                x.MaHocVien,
                x.HoTenHocVien,
                NgayThu = x.NgayThu.ToString("dd/MM/yyyy HH:mm"),
                SoTien = x.SoTien.ToString("N0") + " đ",
                HinhThuc = x.HinhThuc == HinhThucThanhToan.TIEN_MAT ? "Tiền mặt" : "Chuyển khoản",
                x.NguoiThuTen,
                TrangThai = x.TrangThai == TrangThaiPhieuThu.HieuLuc ? "Hiệu lực" : "Đã hủy",
                x.LyDoHuy
            }).ToList();
        }
        else
        {
            dgvPhieuThu.DataSource = null;
            MessageBox.Show(res.Message ?? "Không thể tải lịch sử phiếu thu.", "Lỗi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnTimPhieu_Click(object? sender, EventArgs e)
    {
        await LoadLichSuThuDataAsync();
    }

    private async void btnThuHocPhi_Click(object? sender, EventArgs e)
    {
        if (dgvCongNo.CurrentRow == null)
        {
            MessageBox.Show("Vui lòng chọn học viên cần thu học phí.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int dangKyId = Convert.ToInt32(dgvCongNo.CurrentRow.Cells["DangKyLopHocId"].Value);
        string maHv = dgvCongNo.CurrentRow.Cells["MaHocVien"].Value?.ToString() ?? string.Empty;
        string tenHv = dgvCongNo.CurrentRow.Cells["HoTenHocVien"].Value?.ToString() ?? string.Empty;
        string tenLop = cboLopHoc.Text;

        var remRes = await _hocPhiService.GetRemainingFeeAsync(dangKyId);
        if (!remRes.IsSuccess)
        {
            MessageBox.Show(remRes.Message, "Lỗi kiểm tra công nợ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        decimal conNo = remRes.Data;
        if (conNo <= 0)
        {
            MessageBox.Show("Học viên này đã hoàn thành toàn bộ học phí.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var frm = new FrmThuHocPhi(_hocPhiService, dangKyId, maHv, tenHv, conNo);
        if (frm.ShowDialog(this) == DialogResult.OK)
        {
            await LoadCongNoDataAsync();
            await LoadLichSuThuDataAsync();
        }
    }

    private async void btnHuyPhieu_Click(object? sender, EventArgs e)
    {
        if (dgvPhieuThu.CurrentRow == null)
        {
            MessageBox.Show("Vui lòng chọn phiếu thu cần hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int id = Convert.ToInt32(dgvPhieuThu.CurrentRow.Cells["Id"].Value);
        string maPhieu = dgvPhieuThu.CurrentRow.Cells["MaPhieuThu"].Value?.ToString() ?? string.Empty;
        string trangThai = dgvPhieuThu.CurrentRow.Cells["TrangThai"].Value?.ToString() ?? string.Empty;

        if (trangThai == "Đã hủy")
        {
            MessageBox.Show("Phiếu thu này đã bị hủy trước đó.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var lyDo = Microsoft.VisualBasic.Interaction.InputBox(
            $"Nhập lý do hủy phiếu thu '{maPhieu}':",
            "Xác nhận hủy phiếu thu",
            "Nhập sai thông tin nộp tiền");

        if (string.IsNullOrWhiteSpace(lyDo))
        {
            return; // Người dùng nhấn cancel hoặc không nhập
        }

        var res = await _hocPhiService.HuyPhieuThuAsync(id, lyDo);
        if (res.IsSuccess)
        {
            MessageBox.Show($"Đã hủy thành công phiếu thu '{maPhieu}'!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadCongNoDataAsync();
            await LoadLichSuThuDataAsync();
        }
        else
        {
            MessageBox.Show(res.Message, "Lỗi hủy phiếu thu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
