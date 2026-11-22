using QLHV.Contracts.Enums;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmThuHocPhi : Form
{
    private readonly IHocPhiService _hocPhiService;
    private readonly int _dangKyLopHocId;
    private readonly decimal _conNo;

    public FrmThuHocPhi(IHocPhiService hocPhiService, int dangKyLopHocId, string maHv, string hoTen, decimal conNo)
    {
        _hocPhiService = hocPhiService;
        _dangKyLopHocId = dangKyLopHocId;
        _conNo = conNo;

        InitializeComponent();

        lblThongTinHv.Text = $"Học viên: {hoTen} ({maHv})";
        lblSoTienNo.Text = $"Số tiền còn nợ: {conNo:N0} VNĐ";
        nudSoTien.Maximum = conNo > 0 ? conNo : 100000000m;
        nudSoTien.Value = conNo;
        cboHinhThuc.SelectedIndex = 0;
    }

    private async void btnSave_Click(object? sender, EventArgs e)
    {
        decimal soTien = nudSoTien.Value;
        if (soTien <= 0)
        {
            MessageBox.Show("Số tiền thu phải lớn hơn 0 VNĐ.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var hinhThuc = cboHinhThuc.SelectedIndex == 1 ? HinhThucThanhToan.CHUYEN_KHOAN : HinhThucThanhToan.TIEN_MAT;
        string ghiChu = txtGhiChu.Text.Trim();

        var res = await _hocPhiService.ThuHocPhiAsync(_dangKyLopHocId, soTien, hinhThuc, ghiChu);
        if (res.IsSuccess)
        {
            MessageBox.Show($"Lập phiếu thu thành công! Mã phiếu: {res.Data!.MaPhieuThu}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show(res.Message, "Lỗi lập phiếu thu", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
