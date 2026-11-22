namespace QLHV.WinForms.Forms;

partial class FrmDiem
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
        btnTaiBangDiem = new Button();
        cboMonHoc = new ComboBox();
        lblMonHoc = new Label();
        cboLopHoc = new ComboBox();
        lblLopHoc = new Label();
        pnlBottom = new Panel();
        btnLuuBangDiem = new Button();
        dgvDiem = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colDangKyId = new DataGridViewTextBoxColumn();
        colMaHocVien = new DataGridViewTextBoxColumn();
        colHoTen = new DataGridViewTextBoxColumn();
        colDiem = new DataGridViewTextBoxColumn();
        colKetQua = new DataGridViewTextBoxColumn();
        colGhiChu = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        pnlBottom.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvDiem).BeginInit();
        SuspendLayout();
        //
        // pnlTop
        //
        pnlTop.BackColor = Color.FromArgb(249, 250, 251);
        pnlTop.Controls.Add(btnTaiBangDiem);
        pnlTop.Controls.Add(cboMonHoc);
        pnlTop.Controls.Add(lblMonHoc);
        pnlTop.Controls.Add(cboLopHoc);
        pnlTop.Controls.Add(lblLopHoc);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Size = new Size(880, 60);
        pnlTop.TabIndex = 0;
        //
        // btnTaiBangDiem
        //
        btnTaiBangDiem.BackColor = Color.FromArgb(79, 70, 229);
        btnTaiBangDiem.FlatAppearance.BorderSize = 0;
        btnTaiBangDiem.FlatStyle = FlatStyle.Flat;
        btnTaiBangDiem.ForeColor = Color.White;
        btnTaiBangDiem.Location = new Point(620, 15);
        btnTaiBangDiem.Name = "btnTaiBangDiem";
        btnTaiBangDiem.Size = new Size(120, 29);
        btnTaiBangDiem.TabIndex = 4;
        btnTaiBangDiem.Text = "Tải bảng điểm";
        btnTaiBangDiem.UseVisualStyleBackColor = false;
        btnTaiBangDiem.Click += btnTaiBangDiem_Click;
        //
        // cboMonHoc
        //
        cboMonHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMonHoc.FormattingEnabled = true;
        cboMonHoc.Location = new Point(380, 17);
        cboMonHoc.Name = "cboMonHoc";
        cboMonHoc.Size = new Size(220, 25);
        cboMonHoc.TabIndex = 3;
        //
        // lblMonHoc
        //
        lblMonHoc.AutoSize = true;
        lblMonHoc.Location = new Point(310, 20);
        lblMonHoc.Name = "lblMonHoc";
        lblMonHoc.Size = new Size(64, 17);
        lblMonHoc.TabIndex = 2;
        lblMonHoc.Text = "Môn học:";
        //
        // cboLopHoc
        //
        cboLopHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLopHoc.FormattingEnabled = true;
        cboLopHoc.Location = new Point(80, 17);
        cboLopHoc.Name = "cboLopHoc";
        cboLopHoc.Size = new Size(210, 25);
        cboLopHoc.TabIndex = 1;
        //
        // lblLopHoc
        //
        lblLopHoc.AutoSize = true;
        lblLopHoc.Location = new Point(15, 20);
        lblLopHoc.Name = "lblLopHoc";
        lblLopHoc.Size = new Size(59, 17);
        lblLopHoc.TabIndex = 0;
        lblLopHoc.Text = "Lớp học:";
        //
        // pnlBottom
        //
        pnlBottom.BackColor = Color.FromArgb(249, 250, 251);
        pnlBottom.Controls.Add(btnLuuBangDiem);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 510);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Size = new Size(880, 50);
        pnlBottom.TabIndex = 1;
        //
        // btnLuuBangDiem
        //
        btnLuuBangDiem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnLuuBangDiem.BackColor = Color.FromArgb(16, 185, 129);
        btnLuuBangDiem.FlatAppearance.BorderSize = 0;
        btnLuuBangDiem.FlatStyle = FlatStyle.Flat;
        btnLuuBangDiem.ForeColor = Color.White;
        btnLuuBangDiem.Location = new Point(720, 10);
        btnLuuBangDiem.Name = "btnLuuBangDiem";
        btnLuuBangDiem.Size = new Size(150, 30);
        btnLuuBangDiem.TabIndex = 0;
        btnLuuBangDiem.Text = "💾 Lưu bảng điểm";
        btnLuuBangDiem.UseVisualStyleBackColor = false;
        btnLuuBangDiem.Click += btnLuuBangDiem_Click;
        //
        // dgvDiem
        //
        dgvDiem.AllowUserToAddRows = false;
        dgvDiem.AllowUserToDeleteRows = false;
        dgvDiem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvDiem.BackgroundColor = Color.White;
        dgvDiem.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvDiem.Columns.AddRange(new DataGridViewColumn[] { colId, colDangKyId, colMaHocVien, colHoTen, colDiem, colKetQua, colGhiChu });
        dgvDiem.Dock = DockStyle.Fill;
        dgvDiem.Location = new Point(0, 60);
        dgvDiem.MultiSelect = false;
        dgvDiem.Name = "dgvDiem";
        dgvDiem.RowHeadersVisible = false;
        dgvDiem.SelectionMode = DataGridViewSelectionMode.CellSelect;
        dgvDiem.Size = new Size(880, 450);
        dgvDiem.TabIndex = 2;
        dgvDiem.CellValueChanged += dgvDiem_CellValueChanged;
        //
        // colId
        //
        colId.HeaderText = "Id";
        colId.Name = "colId";
        colId.Visible = false;
        //
        // colDangKyId
        //
        colDangKyId.HeaderText = "DangKyId";
        colDangKyId.Name = "colDangKyId";
        colDangKyId.Visible = false;
        //
        // colMaHocVien
        //
        colMaHocVien.FillWeight = 25F;
        colMaHocVien.HeaderText = "Mã học viên";
        colMaHocVien.Name = "colMaHocVien";
        colMaHocVien.ReadOnly = true;
        //
        // colHoTen
        //
        colHoTen.FillWeight = 45F;
        colHoTen.HeaderText = "Họ và tên";
        colHoTen.Name = "colHoTen";
        colHoTen.ReadOnly = true;
        //
        // colDiem
        //
        colDiem.FillWeight = 20F;
        colDiem.HeaderText = "Điểm tổng kết (0-10)";
        colDiem.Name = "colDiem";
        //
        // colKetQua
        //
        colKetQua.FillWeight = 20F;
        colKetQua.HeaderText = "Kết quả";
        colKetQua.Name = "colKetQua";
        colKetQua.ReadOnly = true;
        //
        // colGhiChu
        //
        colGhiChu.FillWeight = 40F;
        colGhiChu.HeaderText = "Ghi chú";
        colGhiChu.Name = "colGhiChu";
        //
        // FrmDiem
        //
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(880, 560);
        Controls.Add(dgvDiem);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTop);
        Font = new Font("Segoe UI", 9.75F);
        FormBorderStyle = FormBorderStyle.None;
        Name = "FrmDiem";
        Text = "Quản lý Điểm số";
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        pnlBottom.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvDiem).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlTop;
    private Panel pnlBottom;
    private DataGridView dgvDiem;
    private Label lblLopHoc;
    private ComboBox cboLopHoc;
    private Label lblMonHoc;
    private ComboBox cboMonHoc;
    private Button btnTaiBangDiem;
    private Button btnLuuBangDiem;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colDangKyId;
    private DataGridViewTextBoxColumn colMaHocVien;
    private DataGridViewTextBoxColumn colHoTen;
    private DataGridViewTextBoxColumn colDiem;
    private DataGridViewTextBoxColumn colKetQua;
    private DataGridViewTextBoxColumn colGhiChu;
}
