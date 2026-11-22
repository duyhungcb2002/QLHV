namespace QLHV.WinForms.Forms;

partial class FrmHocPhi
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
        splitContainer = new SplitContainer();
        grpCongNo = new GroupBox();
        dgvCongNo = new DataGridView();
        pnlTopCongNo = new Panel();
        btnThuHocPhi = new Button();
        cboLopHoc = new ComboBox();
        lblLopHoc = new Label();
        grpPhieuThu = new GroupBox();
        dgvPhieuThu = new DataGridView();
        pnlTopPhieuThu = new Panel();
        btnHuyPhieu = new Button();
        btnTimPhieu = new Button();
        txtTimKiemPhieu = new TextBox();
        lblTimPhieu = new Label();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        grpCongNo.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCongNo).BeginInit();
        pnlTopCongNo.SuspendLayout();
        grpPhieuThu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPhieuThu).BeginInit();
        pnlTopPhieuThu.SuspendLayout();
        SuspendLayout();
        //
        // splitContainer
        //
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Location = new Point(0, 0);
        splitContainer.Name = "splitContainer";
        splitContainer.Orientation = Orientation.Horizontal;
        //
        // splitContainer.Panel1
        //
        splitContainer.Panel1.Controls.Add(grpCongNo);
        //
        // splitContainer.Panel2
        //
        splitContainer.Panel2.Controls.Add(grpPhieuThu);
        splitContainer.Size = new Size(950, 600);
        splitContainer.SplitterDistance = 290;
        splitContainer.TabIndex = 0;
        //
        // grpCongNo
        //
        grpCongNo.Controls.Add(dgvCongNo);
        grpCongNo.Controls.Add(pnlTopCongNo);
        grpCongNo.Dock = DockStyle.Fill;
        grpCongNo.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        grpCongNo.Location = new Point(0, 0);
        grpCongNo.Name = "grpCongNo";
        grpCongNo.Size = new Size(950, 290);
        grpCongNo.TabIndex = 0;
        grpCongNo.TabStop = false;
        grpCongNo.Text = "  Theo dõi Công nợ học viên theo Lớp  ";
        //
        // dgvCongNo
        //
        dgvCongNo.AllowUserToAddRows = false;
        dgvCongNo.AllowUserToDeleteRows = false;
        dgvCongNo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCongNo.BackgroundColor = Color.White;
        dgvCongNo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvCongNo.Dock = DockStyle.Fill;
        dgvCongNo.Font = new Font("Segoe UI", 9.75F);
        dgvCongNo.Location = new Point(3, 71);
        dgvCongNo.MultiSelect = false;
        dgvCongNo.Name = "dgvCongNo";
        dgvCongNo.ReadOnly = true;
        dgvCongNo.RowHeadersVisible = false;
        dgvCongNo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCongNo.Size = new Size(944, 216);
        dgvCongNo.TabIndex = 1;
        //
        // pnlTopCongNo
        //
        pnlTopCongNo.BackColor = Color.FromArgb(249, 250, 251);
        pnlTopCongNo.Controls.Add(btnThuHocPhi);
        pnlTopCongNo.Controls.Add(cboLopHoc);
        pnlTopCongNo.Controls.Add(lblLopHoc);
        pnlTopCongNo.Dock = DockStyle.Top;
        pnlTopCongNo.Font = new Font("Segoe UI", 9.75F);
        pnlTopCongNo.Location = new Point(3, 21);
        pnlTopCongNo.Name = "pnlTopCongNo";
        pnlTopCongNo.Size = new Size(944, 50);
        pnlTopCongNo.TabIndex = 0;
        //
        // btnThuHocPhi
        //
        btnThuHocPhi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnThuHocPhi.BackColor = Color.FromArgb(16, 185, 129);
        btnThuHocPhi.FlatAppearance.BorderSize = 0;
        btnThuHocPhi.FlatStyle = FlatStyle.Flat;
        btnThuHocPhi.ForeColor = Color.White;
        btnThuHocPhi.Location = new Point(780, 10);
        btnThuHocPhi.Name = "btnThuHocPhi";
        btnThuHocPhi.Size = new Size(150, 30);
        btnThuHocPhi.TabIndex = 2;
        btnThuHocPhi.Text = "💳 Thu học phí";
        btnThuHocPhi.UseVisualStyleBackColor = false;
        btnThuHocPhi.Click += btnThuHocPhi_Click;
        //
        // cboLopHoc
        //
        cboLopHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLopHoc.FormattingEnabled = true;
        cboLopHoc.Location = new Point(80, 12);
        cboLopHoc.Name = "cboLopHoc";
        cboLopHoc.Size = new Size(250, 25);
        cboLopHoc.TabIndex = 1;
        //
        // lblLopHoc
        //
        lblLopHoc.AutoSize = true;
        lblLopHoc.Location = new Point(15, 15);
        lblLopHoc.Name = "lblLopHoc";
        lblLopHoc.Size = new Size(59, 17);
        lblLopHoc.TabIndex = 0;
        lblLopHoc.Text = "Lớp học:";
        //
        // grpPhieuThu
        //
        grpPhieuThu.Controls.Add(dgvPhieuThu);
        grpPhieuThu.Controls.Add(pnlTopPhieuThu);
        grpPhieuThu.Dock = DockStyle.Fill;
        grpPhieuThu.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        grpPhieuThu.Location = new Point(0, 0);
        grpPhieuThu.Name = "grpPhieuThu";
        grpPhieuThu.Size = new Size(950, 306);
        grpPhieuThu.TabIndex = 0;
        grpPhieuThu.TabStop = false;
        grpPhieuThu.Text = "  Lịch sử Phiếu thu học phí  ";
        //
        // dgvPhieuThu
        //
        dgvPhieuThu.AllowUserToAddRows = false;
        dgvPhieuThu.AllowUserToDeleteRows = false;
        dgvPhieuThu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPhieuThu.BackgroundColor = Color.White;
        dgvPhieuThu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvPhieuThu.Dock = DockStyle.Fill;
        dgvPhieuThu.Font = new Font("Segoe UI", 9.75F);
        dgvPhieuThu.Location = new Point(3, 71);
        dgvPhieuThu.MultiSelect = false;
        dgvPhieuThu.Name = "dgvPhieuThu";
        dgvPhieuThu.ReadOnly = true;
        dgvPhieuThu.RowHeadersVisible = false;
        dgvPhieuThu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvPhieuThu.Size = new Size(944, 232);
        dgvPhieuThu.TabIndex = 1;
        //
        // pnlTopPhieuThu
        //
        pnlTopPhieuThu.BackColor = Color.FromArgb(249, 250, 251);
        pnlTopPhieuThu.Controls.Add(btnHuyPhieu);
        pnlTopPhieuThu.Controls.Add(btnTimPhieu);
        pnlTopPhieuThu.Controls.Add(txtTimKiemPhieu);
        pnlTopPhieuThu.Controls.Add(lblTimPhieu);
        pnlTopPhieuThu.Dock = DockStyle.Top;
        pnlTopPhieuThu.Font = new Font("Segoe UI", 9.75F);
        pnlTopPhieuThu.Location = new Point(3, 21);
        pnlTopPhieuThu.Name = "pnlTopPhieuThu";
        pnlTopPhieuThu.Size = new Size(944, 50);
        pnlTopPhieuThu.TabIndex = 0;
        //
        // btnHuyPhieu
        //
        btnHuyPhieu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnHuyPhieu.BackColor = Color.FromArgb(239, 68, 68);
        btnHuyPhieu.FlatAppearance.BorderSize = 0;
        btnHuyPhieu.FlatStyle = FlatStyle.Flat;
        btnHuyPhieu.ForeColor = Color.White;
        btnHuyPhieu.Location = new Point(810, 10);
        btnHuyPhieu.Name = "btnHuyPhieu";
        btnHuyPhieu.Size = new Size(120, 30);
        btnHuyPhieu.TabIndex = 3;
        btnHuyPhieu.Text = "Hủy phiếu thu";
        btnHuyPhieu.UseVisualStyleBackColor = false;
        btnHuyPhieu.Click += btnHuyPhieu_Click;
        //
        // btnTimPhieu
        //
        btnTimPhieu.BackColor = Color.FromArgb(79, 70, 229);
        btnTimPhieu.FlatAppearance.BorderSize = 0;
        btnTimPhieu.FlatStyle = FlatStyle.Flat;
        btnTimPhieu.ForeColor = Color.White;
        btnTimPhieu.Location = new Point(310, 10);
        btnTimPhieu.Name = "btnTimPhieu";
        btnTimPhieu.Size = new Size(80, 29);
        btnTimPhieu.TabIndex = 2;
        btnTimPhieu.Text = "Tìm";
        btnTimPhieu.UseVisualStyleBackColor = false;
        btnTimPhieu.Click += btnTimPhieu_Click;
        //
        // txtTimKiemPhieu
        //
        txtTimKiemPhieu.Location = new Point(90, 12);
        txtTimKiemPhieu.Name = "txtTimKiemPhieu";
        txtTimKiemPhieu.PlaceholderText = "Số phiếu, tên, mã học viên...";
        txtTimKiemPhieu.Size = new Size(210, 25);
        txtTimKiemPhieu.TabIndex = 1;
        //
        // lblTimPhieu
        //
        lblTimPhieu.AutoSize = true;
        lblTimPhieu.Location = new Point(15, 15);
        lblTimPhieu.Name = "lblTimPhieu";
        lblTimPhieu.Size = new Size(61, 17);
        lblTimPhieu.TabIndex = 0;
        lblTimPhieu.Text = "Từ khóa:";
        //
        // FrmHocPhi
        //
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(950, 600);
        Controls.Add(splitContainer);
        Font = new Font("Segoe UI", 9.75F);
        FormBorderStyle = FormBorderStyle.None;
        Name = "FrmHocPhi";
        Text = "Quản lý Học phí";
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        grpCongNo.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvCongNo).EndInit();
        pnlTopCongNo.ResumeLayout(false);
        pnlTopCongNo.PerformLayout();
        grpPhieuThu.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvPhieuThu).EndInit();
        pnlTopPhieuThu.ResumeLayout(false);
        pnlTopPhieuThu.PerformLayout();
        ResumeLayout(false);
    }

    private SplitContainer splitContainer;
    private GroupBox grpCongNo;
    private DataGridView dgvCongNo;
    private Panel pnlTopCongNo;
    private Label lblLopHoc;
    private ComboBox cboLopHoc;
    private Button btnThuHocPhi;
    private GroupBox grpPhieuThu;
    private DataGridView dgvPhieuThu;
    private Panel pnlTopPhieuThu;
    private Label lblTimPhieu;
    private TextBox txtTimKiemPhieu;
    private Button btnTimPhieu;
    private Button btnHuyPhieu;
}
