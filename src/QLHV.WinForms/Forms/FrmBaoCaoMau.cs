namespace QLHV.WinForms.Forms;

/// <summary>
/// Mẫu giao diện báo cáo dùng lại được cho module Reporting
/// Không chứa câu lệnh SQL, truy vấn hay nghiệp vụ báo cáo chính thức (thuộc trách nhiệm của Chu Đức)
/// </summary>
public partial class FrmBaoCaoMau : Form
{
    public FrmBaoCaoMau()
    {
        InitializeComponent();
        dtpFromDate.Value = DateTime.Today.AddMonths(-1);
        dtpToDate.Value = DateTime.Today;
    }

    private void btnRefresh_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Chức năng truy vấn báo cáo đang chờ Chu Đức hoàn thiện CSDL và module Reporting (DH-06).",
            "Đang chờ tích hợp",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void btnExport_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Chức năng xuất báo cáo đang chờ Chu Đức hoàn thiện (DH-06).",
            "Đang chờ tích hợp",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
