namespace QLHV.WinForms.Forms;

partial class FrmGiangVien
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
        lblStatusFilter = new Label();
        cboFilterStatus = new ComboBox();
        btnRefresh = new Button();
        btnSearch = new Button();
        txtSearch = new TextBox();
        lblSearch = new Label();
        lblTitle = new Label();
        pnlBottom = new Panel();
        btnDeactivate = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        dgvGiangVien = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colMaGiangVien = new DataGridViewTextBoxColumn();
        colHoTen = new DataGridViewTextBoxColumn();
        colChuyenMon = new DataGridViewTextBoxColumn();
        colDienThoai = new DataGridViewTextBoxColumn();
        colEmail = new DataGridViewTextBoxColumn();
        colTrangThai = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        pnlBottom.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvGiangVien).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.BackColor = Color.White;
        pnlTop.Controls.Add(lblStatusFilter);
        pnlTop.Controls.Add(cboFilterStatus);
        pnlTop.Controls.Add(btnRefresh);
        pnlTop.Controls.Add(btnSearch);
        pnlTop.Controls.Add(txtSearch);
        pnlTop.Controls.Add(lblSearch);
        pnlTop.Controls.Add(lblTitle);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Size = new Size(880, 85);
        pnlTop.TabIndex = 0;
        // 
        // lblStatusFilter
        // 
        lblStatusFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblStatusFilter.AutoSize = true;
        lblStatusFilter.Location = new Point(510, 52);
        lblStatusFilter.Name = "lblStatusFilter";
        lblStatusFilter.Size = new Size(62, 15);
        lblStatusFilter.TabIndex = 6;
        lblStatusFilter.Text = "Trạng thái:";
        // 
        // cboFilterStatus
        // 
        cboFilterStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilterStatus.FormattingEnabled = true;
        cboFilterStatus.Location = new Point(578, 48);
        cboFilterStatus.Name = "cboFilterStatus";
        cboFilterStatus.Size = new Size(130, 23);
        cboFilterStatus.TabIndex = 5;
        // 
        // btnRefresh
        // 
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.BackColor = Color.FromArgb(243, 244, 246);
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.Location = new Point(796, 47);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(72, 25);
        btnRefresh.TabIndex = 4;
        btnRefresh.Text = "Làm mới";
        btnRefresh.UseVisualStyleBackColor = false;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnSearch
        // 
        btnSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSearch.BackColor = Color.FromArgb(30, 64, 175);
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.ForeColor = Color.White;
        btnSearch.Location = new Point(716, 47);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(74, 25);
        btnSearch.TabIndex = 3;
        btnSearch.Text = "Tìm kiếm";
        btnSearch.UseVisualStyleBackColor = false;
        btnSearch.Click += btnSearch_Click;
        // 
        // txtSearch
        // 
        txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtSearch.Location = new Point(140, 48);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Nhập mã, tên giảng viên, chuyên môn để tìm...";
        txtSearch.Size = new Size(355, 23);
        txtSearch.TabIndex = 2;
        // 
        // lblSearch
        // 
        lblSearch.AutoSize = true;
        lblSearch.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        lblSearch.Location = new Point(15, 52);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(111, 15);
        lblSearch.TabIndex = 1;
        lblSearch.Text = "Từ khóa tìm kiếm:";
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.ForeColor = Color.FromArgb(30, 64, 175);
        lblTitle.Location = new Point(15, 12);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(230, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "DANH MỤC GIẢNG VIÊN";
        // 
        // pnlBottom
        // 
        pnlBottom.BackColor = Color.FromArgb(249, 250, 251);
        pnlBottom.Controls.Add(btnDeactivate);
        pnlBottom.Controls.Add(btnEdit);
        pnlBottom.Controls.Add(btnAdd);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 480);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Size = new Size(880, 50);
        pnlBottom.TabIndex = 1;
        // 
        // btnDeactivate
        // 
        btnDeactivate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDeactivate.BackColor = Color.FromArgb(239, 68, 68);
        btnDeactivate.FlatAppearance.BorderSize = 0;
        btnDeactivate.FlatStyle = FlatStyle.Flat;
        btnDeactivate.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnDeactivate.ForeColor = Color.White;
        btnDeactivate.Location = new Point(738, 10);
        btnDeactivate.Name = "btnDeactivate";
        btnDeactivate.Size = new Size(130, 30);
        btnDeactivate.TabIndex = 2;
        btnDeactivate.Text = "Ngừng hoạt động";
        btnDeactivate.UseVisualStyleBackColor = false;
        btnDeactivate.Click += btnDeactivate_Click;
        // 
        // btnEdit
        // 
        btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnEdit.BackColor = Color.FromArgb(245, 158, 11);
        btnEdit.FlatAppearance.BorderSize = 0;
        btnEdit.FlatStyle = FlatStyle.Flat;
        btnEdit.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnEdit.ForeColor = Color.White;
        btnEdit.Location = new Point(622, 10);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(105, 30);
        btnEdit.TabIndex = 1;
        btnEdit.Text = "Cập nhật";
        btnEdit.UseVisualStyleBackColor = false;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnAdd
        // 
        btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAdd.BackColor = Color.FromArgb(16, 185, 129);
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnAdd.ForeColor = Color.White;
        btnAdd.Location = new Point(506, 10);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(105, 30);
        btnAdd.TabIndex = 0;
        btnAdd.Text = "Thêm mới";
        btnAdd.UseVisualStyleBackColor = false;
        btnAdd.Click += btnAdd_Click;
        // 
        // dgvGiangVien
        // 
        dgvGiangVien.AllowUserToAddRows = false;
        dgvGiangVien.AllowUserToDeleteRows = false;
        dgvGiangVien.BackgroundColor = Color.White;
        dgvGiangVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvGiangVien.Columns.AddRange(new DataGridViewColumn[] { colId, colMaGiangVien, colHoTen, colChuyenMon, colDienThoai, colEmail, colTrangThai });
        dgvGiangVien.Dock = DockStyle.Fill;
        dgvGiangVien.Location = new Point(0, 85);
        dgvGiangVien.MultiSelect = false;
        dgvGiangVien.Name = "dgvGiangVien";
        dgvGiangVien.ReadOnly = true;
        dgvGiangVien.RowHeadersVisible = false;
        dgvGiangVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvGiangVien.Size = new Size(880, 395);
        dgvGiangVien.TabIndex = 2;
        // 
        // colId
        // 
        colId.DataPropertyName = "Id";
        colId.HeaderText = "ID";
        colId.Name = "colId";
        colId.ReadOnly = true;
        colId.Visible = false;
        // 
        // colMaGiangVien
        // 
        colMaGiangVien.DataPropertyName = "MaGiangVien";
        colMaGiangVien.HeaderText = "Mã GV";
        colMaGiangVien.Name = "colMaGiangVien";
        colMaGiangVien.ReadOnly = true;
        colMaGiangVien.Width = 90;
        // 
        // colHoTen
        // 
        colHoTen.DataPropertyName = "HoTen";
        colHoTen.HeaderText = "Họ và tên";
        colHoTen.Name = "colHoTen";
        colHoTen.ReadOnly = true;
        colHoTen.Width = 180;
        // 
        // colChuyenMon
        // 
        colChuyenMon.DataPropertyName = "ChuyenMon";
        colChuyenMon.HeaderText = "Chuyên môn giảng dạy";
        colChuyenMon.Name = "colChuyenMon";
        colChuyenMon.ReadOnly = true;
        colChuyenMon.Width = 220;
        // 
        // colDienThoai
        // 
        colDienThoai.DataPropertyName = "DienThoai";
        colDienThoai.HeaderText = "Điện thoại";
        colDienThoai.Name = "colDienThoai";
        colDienThoai.ReadOnly = true;
        colDienThoai.Width = 110;
        // 
        // colEmail
        // 
        colEmail.DataPropertyName = "Email";
        colEmail.HeaderText = "Email";
        colEmail.Name = "colEmail";
        colEmail.ReadOnly = true;
        colEmail.Width = 150;
        // 
        // colTrangThai
        // 
        colTrangThai.DataPropertyName = "TrangThai";
        colTrangThai.HeaderText = "Trạng thái";
        colTrangThai.Name = "colTrangThai";
        colTrangThai.ReadOnly = true;
        colTrangThai.Width = 110;
        // 
        // FrmGiangVien
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(880, 530);
        Controls.Add(dgvGiangVien);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTop);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Name = "FrmGiangVien";
        Text = "Quản lý Giảng viên";
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        pnlBottom.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvGiangVien).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlTop;
    private Label lblTitle;
    private Label lblSearch;
    private TextBox txtSearch;
    private Button btnSearch;
    private Button btnRefresh;
    private Label lblStatusFilter;
    private ComboBox cboFilterStatus;
    private Panel pnlBottom;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDeactivate;
    private DataGridView dgvGiangVien;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colMaGiangVien;
    private DataGridViewTextBoxColumn colHoTen;
    private DataGridViewTextBoxColumn colChuyenMon;
    private DataGridViewTextBoxColumn colDienThoai;
    private DataGridViewTextBoxColumn colEmail;
    private DataGridViewTextBoxColumn colTrangThai;
}
