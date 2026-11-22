namespace QLHV.WinForms.Forms;

partial class FrmBaoCaoMau
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlTop = new Panel();
        lblNote = new Label();
        btnExport = new Button();
        btnRefresh = new Button();
        dtpToDate = new DateTimePicker();
        lblToDate = new Label();
        dtpFromDate = new DateTimePicker();
        lblFromDate = new Label();
        lblTitle = new Label();
        dgvReport = new DataGridView();
        pnlTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvReport).BeginInit();
        SuspendLayout();
        //
        // pnlTop
        //
        pnlTop.BackColor = Color.White;
        pnlTop.Controls.Add(lblNote);
        pnlTop.Controls.Add(btnExport);
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(dtpToDate);
        pnlTop.Controls.Add(lblToDate);
        pnlTop.Controls.Add(dtpFromDate);
        pnlTop.Controls.Add(lblFromDate);
        pnlTop.Controls.Add(lblTitle);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Size = new Size(880, 115);
        pnlTop.TabIndex = 0;
        //
        // lblNote
        //
        lblNote.AutoSize = true;
        lblNote.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic, GraphicsUnit.Point);
        lblNote.ForeColor = Color.FromArgb(107, 114, 128);
        lblNote.Location = new Point(15, 85);
        lblNote.Name = "lblNote";
        lblNote.Size = new Size(495, 15);
        lblNote.TabIndex = 7;
        lblNote.Text = "* Mẫu giao diện báo cáo chuẩn (Chu Đức sẽ tích hợp câu truy vấn và dữ liệu báo cáo chính thức)";
        //
        // btnExport
        //
        btnExport.BackColor = Color.FromArgb(16, 185, 129);
        btnExport.FlatAppearance.BorderSize = 0;
        btnExport.FlatStyle = FlatStyle.Flat;
        btnExport.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnExport.ForeColor = Color.White;
        btnExport.Location = new Point(640, 48);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(100, 26);
        btnExport.TabIndex = 6;
        btnExport.Text = "Xuất Excel";
        btnExport.UseVisualStyleBackColor = false;
        btnExport.Click += btnExport_Click;
        //
        // btnRefresh
        //
        btnRefresh.BackColor = Color.FromArgb(30, 64, 175);
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnRefresh.ForeColor = Color.White;
        btnRefresh.Location = new Point(530, 48);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(95, 26);
        btnRefresh.TabIndex = 5;
        btnRefresh.Text = "Xem báo cáo";
        btnRefresh.UseVisualStyleBackColor = false;
        btnRefresh.Click += btnRefresh_Click;
        //
        // dtpToDate
        //
        dtpToDate.CustomFormat = "dd/MM/yyyy";
        dtpToDate.Format = DateTimePickerFormat.Custom;
        dtpToDate.Location = new Point(370, 50);
        dtpToDate.Name = "dtpToDate";
        dtpToDate.Size = new Size(130, 23);
        dtpToDate.TabIndex = 4;
        //
        // lblToDate
        //
        lblToDate.AutoSize = true;
        lblToDate.Location = new Point(300, 54);
        lblToDate.Name = "lblToDate";
        lblToDate.Size = new Size(60, 15);
        lblToDate.TabIndex = 3;
        lblToDate.Text = "Đến ngày:";
        //
        // dtpFromDate
        //
        dtpFromDate.CustomFormat = "dd/MM/yyyy";
        dtpFromDate.Format = DateTimePickerFormat.Custom;
        dtpFromDate.Location = new Point(145, 50);
        dtpFromDate.Name = "dtpFromDate";
        dtpFromDate.Size = new Size(130, 23);
        dtpFromDate.TabIndex = 2;
        //
        // lblFromDate
        //
        lblFromDate.AutoSize = true;
        lblFromDate.Location = new Point(15, 54);
        lblFromDate.Name = "lblFromDate";
        lblFromDate.Size = new Size(120, 15);
        lblFromDate.TabIndex = 1;
        lblFromDate.Text = "Khoảng thời gian: Từ:";
        //
        // lblTitle
        //
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.ForeColor = Color.FromArgb(30, 64, 175);
        lblTitle.Location = new Point(15, 12);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(342, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "MẪU BÁO CÁO THỐNG KÊ DÙNG LẠI";
        //
        // dgvReport
        //
        dgvReport.AllowUserToAddRows = false;
        dgvReport.AllowUserToDeleteRows = false;
        dgvReport.BackgroundColor = Color.White;
        dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvReport.Dock = DockStyle.Fill;
        dgvReport.Location = new Point(0, 115);
        dgvReport.MultiSelect = false;
        dgvReport.Name = "dgvReport";
        dgvReport.ReadOnly = true;
        dgvReport.RowHeadersVisible = false;
        dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvReport.Size = new Size(880, 415);
        dgvReport.TabIndex = 1;
        //
        // FrmBaoCaoMau
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(880, 530);
        Controls.Add(dgvReport);
        Controls.Add(pnlTop);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Name = "FrmBaoCaoMau";
        Text = "Mẫu Báo cáo Thống kê";
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvReport).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlTop;
    private Label lblTitle;
    private Label lblFromDate;
    private DateTimePicker dtpFromDate;
    private Label lblToDate;
    private DateTimePicker dtpToDate;
    private Button btnRefresh;
    private Button btnExport;
    private Label lblNote;
    private DataGridView dgvReport;
}
