namespace QLHV.WinForms.Forms;

partial class FrmChonHocVien
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
        btnSearch = new Button();
        txtSearch = new TextBox();
        lblSearch = new Label();
        pnlBottom = new Panel();
        btnSelect = new Button();
        btnCancel = new Button();
        dgvHocVien = new DataGridView();
        pnlTop.SuspendLayout();
        pnlBottom.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvHocVien).BeginInit();
        SuspendLayout();
        //
        // pnlTop
        //
        pnlTop.BackColor = Color.FromArgb(249, 250, 251);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(lblSearch);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Size = new Size(684, 50);
        pnlTop.TabIndex = 0;
        //
        // btnSearch
        //
        btnSearch.BackColor = Color.FromArgb(79, 70, 229);
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.ForeColor = Color.White;
        btnSearch.Location = new Point(310, 10);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(80, 28);
        btnSearch.TabIndex = 2;
        btnSearch.Text = "Tìm kiếm";
        btnSearch.UseVisualStyleBackColor = false;
        btnSearch.Click += btnSearch_Click;
        //
        // txtSearch
        //
        txtSearch.Location = new Point(90, 12);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Tên, mã, điện thoại...";
        txtSearch.Size = new Size(210, 25);
        txtSearch.TabIndex = 1;
        //
        // lblSearch
        //
        lblSearch.AutoSize = true;
        lblSearch.Location = new Point(20, 15);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(61, 17);
        lblSearch.TabIndex = 0;
        lblSearch.Text = "Từ khóa:";
        //
        // pnlBottom
        //
        pnlBottom.BackColor = Color.FromArgb(249, 250, 251);
        pnlBottom.Controls.Add(btnSelect);
        pnlBottom.Controls.Add(btnCancel);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 361);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Size = new Size(684, 50);
        pnlBottom.TabIndex = 1;
        //
        // btnSelect
        //
        btnSelect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSelect.BackColor = Color.FromArgb(16, 185, 129);
        btnSelect.FlatAppearance.BorderSize = 0;
        btnSelect.FlatStyle = FlatStyle.Flat;
        btnSelect.ForeColor = Color.White;
        btnSelect.Location = new Point(480, 10);
        btnSelect.Name = "btnSelect";
        btnSelect.Size = new Size(95, 30);
        btnSelect.TabIndex = 0;
        btnSelect.Text = "Chọn";
        btnSelect.UseVisualStyleBackColor = false;
        btnSelect.Click += btnSelect_Click;
        //
        // btnCancel
        //
        btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancel.BackColor = Color.FromArgb(156, 163, 175);
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.ForeColor = Color.White;
        btnCancel.Location = new Point(581, 10);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(95, 30);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Đóng";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Click += btnCancel_Click;
        //
        // dgvHocVien
        //
        dgvHocVien.AllowUserToAddRows = false;
        dgvHocVien.AllowUserToDeleteRows = false;
        dgvHocVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvHocVien.BackgroundColor = Color.White;
        dgvHocVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvHocVien.Dock = DockStyle.Fill;
        dgvHocVien.Location = new Point(0, 50);
        dgvHocVien.MultiSelect = false;
        dgvHocVien.Name = "dgvHocVien";
        dgvHocVien.ReadOnly = true;
        dgvHocVien.RowHeadersVisible = false;
        dgvHocVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHocVien.Size = new Size(684, 311);
        dgvHocVien.TabIndex = 2;
        //
        // FrmChonHocVien
        //
        AcceptButton = btnSelect;
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(684, 411);
        Controls.Add(dgvHocVien);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTop);
        Font = new Font("Segoe UI", 9.75F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmChonHocVien";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Chọn học viên xếp lớp";
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        pnlBottom.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvHocVien).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlTop;
    private Panel pnlBottom;
    private DataGridView dgvHocVien;
    private Label lblSearch;
    private TextBox txtSearch;
    private Button btnSearch;
    private Button btnSelect;
    private Button btnCancel;
}
