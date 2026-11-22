namespace QLHV.WinForms.Forms;

partial class FrmHocVien
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
        btnDetail = new Button();
        btnDeactivate = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        dgvHocVien = new DataGridView();
        colId = new DataGridViewTextBoxColumn();
        colMaHocVien = new DataGridViewTextBoxColumn();
        colHoTen = new DataGridViewTextBoxColumn();
        colNgaySinh = new DataGridViewTextBoxColumn();
        colGioiTinh = new DataGridViewTextBoxColumn();
        colDienThoai = new DataGridViewTextBoxColumn();
        colSoDinhDanh = new DataGridViewTextBoxColumn();
        colNguonTuyenSinh = new DataGridViewTextBoxColumn();
        colNhanVienPhuTrach = new DataGridViewTextBoxColumn();
        colTrangThai = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        pnlBottom.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvHocVien).BeginInit();
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
        pnlTop.Size = new Size(950, 85);
        pnlTop.TabIndex = 0;
        //
        // lblStatusFilter
        //
        lblStatusFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblStatusFilter.AutoSize = true;
        lblStatusFilter.Location = new Point(580, 52);
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
        cboFilterStatus.Location = new Point(648, 48);
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
        btnRefresh.Location = new Point(866, 47);
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
        btnSearch.Location = new Point(786, 47);
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
        txtSearch.PlaceholderText = "Nhập mã, họ tên, điện thoại, CCCD/CMND, giám hộ...";
        txtSearch.Size = new Size(425, 23);
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
        lblTitle.Size = new Size(207, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "DANH MỤC HỌC VIÊN";
        //
        // pnlBottom
        //
        pnlBottom.BackColor = Color.FromArgb(249, 250, 251);
        pnlBottom.Controls.Add(btnDetail);
        pnlBottom.Controls.Add(btnDeactivate);
        pnlBottom.Controls.Add(btnEdit);
        pnlBottom.Controls.Add(btnAdd);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 480);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Size = new Size(950, 50);
        pnlBottom.TabIndex = 1;
        //
        // btnDetail
        //
        btnDetail.BackColor = Color.FromArgb(79, 70, 229);
        btnDetail.FlatAppearance.BorderSize = 0;
        btnDetail.FlatStyle = FlatStyle.Flat;
        btnDetail.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnDetail.ForeColor = Color.White;
        btnDetail.Location = new Point(15, 10);
        btnDetail.Name = "btnDetail";
        btnDetail.Size = new Size(115, 30);
        btnDetail.TabIndex = 3;
        btnDetail.Text = "Xem chi tiết";
        btnDetail.UseVisualStyleBackColor = false;
        btnDetail.Click += btnDetail_Click;
        //
        // btnDeactivate
        //
        btnDeactivate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDeactivate.BackColor = Color.FromArgb(239, 68, 68);
        btnDeactivate.FlatAppearance.BorderSize = 0;
        btnDeactivate.FlatStyle = FlatStyle.Flat;
        btnDeactivate.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnDeactivate.ForeColor = Color.White;
        btnDeactivate.Location = new Point(808, 10);
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
        btnEdit.Location = new Point(692, 10);
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
        btnAdd.Location = new Point(576, 10);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(105, 30);
        btnAdd.TabIndex = 0;
        btnAdd.Text = "Thêm mới";
        btnAdd.UseVisualStyleBackColor = false;
        btnAdd.Click += btnAdd_Click;
        //
        // dgvHocVien
        //
        dgvHocVien.AllowUserToAddRows = false;
        dgvHocVien.AllowUserToDeleteRows = false;
        dgvHocVien.BackgroundColor = Color.White;
        dgvHocVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvHocVien.Columns.AddRange(new DataGridViewColumn[] {
            colId, colMaHocVien, colHoTen, colNgaySinh, colGioiTinh, colDienThoai,
            colSoDinhDanh, colNguonTuyenSinh, colNhanVienPhuTrach, colTrangThai
        });
        dgvHocVien.Dock = DockStyle.Fill;
        dgvHocVien.Location = new Point(0, 85);
        dgvHocVien.MultiSelect = false;
        dgvHocVien.Name = "dgvHocVien";
        dgvHocVien.ReadOnly = true;
        dgvHocVien.RowHeadersVisible = false;
        dgvHocVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHocVien.Size = new Size(950, 395);
        dgvHocVien.TabIndex = 2;
        //
        // colId
        //
        colId.DataPropertyName = "Id";
        colId.HeaderText = "ID";
        colId.Name = "colId";
        colId.ReadOnly = true;
        colId.Visible = false;
        //
        // colMaHocVien
        //
        colMaHocVien.DataPropertyName = "MaHocVien";
        colMaHocVien.HeaderText = "Mã HV";
        colMaHocVien.Name = "colMaHocVien";
        colMaHocVien.ReadOnly = true;
        colMaHocVien.Width = 85;
        //
        // colHoTen
        //
        colHoTen.DataPropertyName = "HoTen";
        colHoTen.HeaderText = "Họ và tên";
        colHoTen.Name = "colHoTen";
        colHoTen.ReadOnly = true;
        colHoTen.Width = 150;
        //
        // colNgaySinh
        //
        colNgaySinh.DataPropertyName = "NgaySinh";
        colNgaySinh.HeaderText = "Ngày sinh";
        colNgaySinh.Name = "colNgaySinh";
        colNgaySinh.ReadOnly = true;
        colNgaySinh.Width = 90;
        //
        // colGioiTinh
        //
        colGioiTinh.DataPropertyName = "GioiTinh";
        colGioiTinh.HeaderText = "Giới tính";
        colGioiTinh.Name = "colGioiTinh";
        colGioiTinh.ReadOnly = true;
        colGioiTinh.Width = 80;
        //
        // colDienThoai
        //
        colDienThoai.DataPropertyName = "DienThoai";
        colDienThoai.HeaderText = "Điện thoại";
        colDienThoai.Name = "colDienThoai";
        colDienThoai.ReadOnly = true;
        colDienThoai.Width = 100;
        //
        // colSoDinhDanh
        //
        colSoDinhDanh.DataPropertyName = "SoDinhDanhMasked";
        colSoDinhDanh.HeaderText = "CCCD/CMND";
        colSoDinhDanh.Name = "colSoDinhDanh";
        colSoDinhDanh.ReadOnly = true;
        colSoDinhDanh.Width = 115;
        //
        // colNguonTuyenSinh
        //
        colNguonTuyenSinh.DataPropertyName = "NguonTuyenSinh";
        colNguonTuyenSinh.HeaderText = "Nguồn tuyển sinh";
        colNguonTuyenSinh.Name = "colNguonTuyenSinh";
        colNguonTuyenSinh.ReadOnly = true;
        colNguonTuyenSinh.Width = 130;
        //
        // colNhanVienPhuTrach
        //
        colNhanVienPhuTrach.DataPropertyName = "TenNhanVienPhuTrach";
        colNhanVienPhuTrach.HeaderText = "Nhân viên phụ trách";
        colNhanVienPhuTrach.Name = "colNhanVienPhuTrach";
        colNhanVienPhuTrach.ReadOnly = true;
        colNhanVienPhuTrach.Width = 150;
        //
        // colTrangThai
        //
        colTrangThai.DataPropertyName = "TrangThai";
        colTrangThai.HeaderText = "Trạng thái";
        colTrangThai.Name = "colTrangThai";
        colTrangThai.ReadOnly = true;
        colTrangThai.Width = 110;
        //
        // FrmHocVien
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(950, 530);
        Controls.Add(dgvHocVien);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTop);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Name = "FrmHocVien";
        Text = "Quản lý Học viên";
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        pnlBottom.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvHocVien).EndInit();
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
    private Button btnDetail;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDeactivate;
    private DataGridView dgvHocVien;
    private DataGridViewTextBoxColumn colId;
    private DataGridViewTextBoxColumn colMaHocVien;
    private DataGridViewTextBoxColumn colHoTen;
    private DataGridViewTextBoxColumn colNgaySinh;
    private DataGridViewTextBoxColumn colGioiTinh;
    private DataGridViewTextBoxColumn colDienThoai;
    private DataGridViewTextBoxColumn colSoDinhDanh;
    private DataGridViewTextBoxColumn colNguonTuyenSinh;
    private DataGridViewTextBoxColumn colNhanVienPhuTrach;
    private DataGridViewTextBoxColumn colTrangThai;
}
