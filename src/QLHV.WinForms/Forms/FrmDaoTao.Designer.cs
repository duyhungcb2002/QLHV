namespace QLHV.WinForms.Forms;

partial class FrmDaoTao
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
        tabControlDaoTao = new TabControl();
        tabLopHoc = new TabPage();
        dgvLopHoc = new DataGridView();
        pnlBottomLop = new Panel();
        btnNgungLop = new Button();
        btnSuaLop = new Button();
        btnThemLop = new Button();
        pnlTopLop = new Panel();
        btnTimLop = new Button();
        txtTimKiemLop = new TextBox();
        lblTimLop = new Label();
        cboFilterKhoaHoc = new ComboBox();
        lblKhoaHoc = new Label();
        tabXepLop = new TabPage();
        dgvDangKy = new DataGridView();
        pnlBottomXepLop = new Panel();
        btnHuyXepLop = new Button();
        btnXepLop = new Button();
        pnlTopXepLop = new Panel();
        btnTimDangKy = new Button();
        txtTimKiemHocVien = new TextBox();
        lblTimHV = new Label();
        cboXepLop_LopHoc = new ComboBox();
        lblChonLop = new Label();
        tabLichHoc = new TabPage();
        dgvLichHoc = new DataGridView();
        pnlBottomLich = new Panel();
        btnXoaLich = new Button();
        btnSuaLich = new Button();
        btnThemLich = new Button();
        pnlTopLich = new Panel();
        cboLichHoc_LopHoc = new ComboBox();
        lblChonLopLich = new Label();

        // Columns for dgvLopHoc
        colLop_Id = new DataGridViewTextBoxColumn();
        colLop_MaLop = new DataGridViewTextBoxColumn();
        colLop_TenLop = new DataGridViewTextBoxColumn();
        colLop_KhoaHoc = new DataGridViewTextBoxColumn();
        colLop_GiangVien = new DataGridViewTextBoxColumn();
        colLop_NgayBatDau = new DataGridViewTextBoxColumn();
        colLop_NgayKetThuc = new DataGridViewTextBoxColumn();
        colLop_SiSo = new DataGridViewTextBoxColumn();
        colLop_TrangThai = new DataGridViewTextBoxColumn();
        colLop_GhiChu = new DataGridViewTextBoxColumn();

        // Columns for dgvDangKy
        colDk_Id = new DataGridViewTextBoxColumn();
        colDk_MaHocVien = new DataGridViewTextBoxColumn();
        colDk_HoTen = new DataGridViewTextBoxColumn();
        colDk_DienThoai = new DataGridViewTextBoxColumn();
        colDk_NgayDangKy = new DataGridViewTextBoxColumn();
        colDk_HocPhiPhaiThu = new DataGridViewTextBoxColumn();
        colDk_DaThu = new DataGridViewTextBoxColumn();
        colDk_ConNo = new DataGridViewTextBoxColumn();
        colDk_TrangThai = new DataGridViewTextBoxColumn();
        colDk_GhiChu = new DataGridViewTextBoxColumn();

        // Columns for dgvLichHoc
        colLich_Id = new DataGridViewTextBoxColumn();
        colLich_MonHoc = new DataGridViewTextBoxColumn();
        colLich_GiangVien = new DataGridViewTextBoxColumn();
        colLich_PhongHoc = new DataGridViewTextBoxColumn();
        colLich_NgayHoc = new DataGridViewTextBoxColumn();
        colLich_ThoiGian = new DataGridViewTextBoxColumn();
        colLich_NoiDung = new DataGridViewTextBoxColumn();

        tabControlDaoTao.SuspendLayout();
        tabLopHoc.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvLopHoc).BeginInit();
        pnlBottomLop.SuspendLayout();
        pnlTopLop.SuspendLayout();
        tabXepLop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvDangKy).BeginInit();
        pnlBottomXepLop.SuspendLayout();
        pnlTopXepLop.SuspendLayout();
        tabLichHoc.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvLichHoc).BeginInit();
        pnlBottomLich.SuspendLayout();
        pnlTopLich.SuspendLayout();
        SuspendLayout();

        // tabControlDaoTao
        tabControlDaoTao.Controls.Add(tabLopHoc);
        tabControlDaoTao.Controls.Add(tabXepLop);
        tabControlDaoTao.Controls.Add(tabLichHoc);
        tabControlDaoTao.Dock = DockStyle.Fill;
        tabControlDaoTao.Font = new Font("Segoe UI", 10F);
        tabControlDaoTao.Location = new Point(0, 0);
        tabControlDaoTao.Name = "tabControlDaoTao";
        tabControlDaoTao.SelectedIndex = 0;
        tabControlDaoTao.Size = new Size(1020, 600);
        tabControlDaoTao.TabIndex = 0;

        // ================= TAB LỚP HỌC =================
        tabLopHoc.Controls.Add(dgvLopHoc);
        tabLopHoc.Controls.Add(pnlBottomLop);
        tabLopHoc.Controls.Add(pnlTopLop);
        tabLopHoc.Location = new Point(4, 26);
        tabLopHoc.Name = "tabLopHoc";
        tabLopHoc.Padding = new Padding(3);
        tabLopHoc.Size = new Size(1012, 570);
        tabLopHoc.TabIndex = 0;
        tabLopHoc.Text = "  Quản lý Lớp học  ";
        tabLopHoc.UseVisualStyleBackColor = true;

        // dgvLopHoc
        dgvLopHoc.AllowUserToAddRows = false;
        dgvLopHoc.AllowUserToDeleteRows = false;
        dgvLopHoc.AutoGenerateColumns = false;
        dgvLopHoc.BackgroundColor = Color.White;
        dgvLopHoc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvLopHoc.Columns.AddRange(new DataGridViewColumn[] {
            colLop_Id, colLop_MaLop, colLop_TenLop, colLop_KhoaHoc, colLop_GiangVien,
            colLop_NgayBatDau, colLop_NgayKetThuc, colLop_SiSo, colLop_TrangThai, colLop_GhiChu
        });
        dgvLopHoc.Dock = DockStyle.Fill;
        dgvLopHoc.Location = new Point(3, 63);
        dgvLopHoc.MultiSelect = false;
        dgvLopHoc.Name = "dgvLopHoc";
        dgvLopHoc.ReadOnly = true;
        dgvLopHoc.RowHeadersVisible = false;
        dgvLopHoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvLopHoc.Size = new Size(1006, 454);
        dgvLopHoc.TabIndex = 2;

        colLop_Id.DataPropertyName = "Id";
        colLop_Id.HeaderText = "ID";
        colLop_Id.Name = "colLop_Id";
        colLop_Id.ReadOnly = true;
        colLop_Id.Visible = false;

        colLop_MaLop.DataPropertyName = "MaLopHoc";
        colLop_MaLop.HeaderText = "Mã lớp";
        colLop_MaLop.Name = "colLop_MaLop";
        colLop_MaLop.ReadOnly = true;
        colLop_MaLop.Width = 90;

        colLop_TenLop.DataPropertyName = "TenLopHoc";
        colLop_TenLop.HeaderText = "Tên lớp học";
        colLop_TenLop.Name = "colLop_TenLop";
        colLop_TenLop.ReadOnly = true;
        colLop_TenLop.Width = 170;

        colLop_KhoaHoc.DataPropertyName = "TenKhoaHoc";
        colLop_KhoaHoc.HeaderText = "Khóa học";
        colLop_KhoaHoc.MinimumWidth = 180;
        colLop_KhoaHoc.Name = "colLop_KhoaHoc";
        colLop_KhoaHoc.ReadOnly = true;
        colLop_KhoaHoc.Width = 180;

        colLop_GiangVien.DataPropertyName = "HoTenGiangVienChuNhiem";
        colLop_GiangVien.HeaderText = "GV chủ nhiệm";
        colLop_GiangVien.MinimumWidth = 160;
        colLop_GiangVien.Name = "colLop_GiangVien";
        colLop_GiangVien.ReadOnly = true;
        colLop_GiangVien.Width = 160;

        colLop_NgayBatDau.DataPropertyName = "NgayBatDau";
        colLop_NgayBatDau.HeaderText = "Bắt đầu";
        colLop_NgayBatDau.Name = "colLop_NgayBatDau";
        colLop_NgayBatDau.ReadOnly = true;
        colLop_NgayBatDau.Width = 90;

        colLop_NgayKetThuc.DataPropertyName = "NgayKetThuc";
        colLop_NgayKetThuc.HeaderText = "Kết thúc";
        colLop_NgayKetThuc.Name = "colLop_NgayKetThuc";
        colLop_NgayKetThuc.ReadOnly = true;
        colLop_NgayKetThuc.Width = 90;

        colLop_SiSo.DataPropertyName = "SiSo";
        colLop_SiSo.HeaderText = "Sĩ số (HT/TĐ)";
        colLop_SiSo.Name = "colLop_SiSo";
        colLop_SiSo.ReadOnly = true;
        colLop_SiSo.Width = 100;

        colLop_TrangThai.DataPropertyName = "TrangThai";
        colLop_TrangThai.HeaderText = "Trạng thái";
        colLop_TrangThai.Name = "colLop_TrangThai";
        colLop_TrangThai.ReadOnly = true;
        colLop_TrangThai.Width = 110;

        colLop_GhiChu.DataPropertyName = "GhiChu";
        colLop_GhiChu.HeaderText = "Ghi chú";
        colLop_GhiChu.MinimumWidth = 200;
        colLop_GhiChu.Name = "colLop_GhiChu";
        colLop_GhiChu.ReadOnly = true;
        colLop_GhiChu.Width = 200;

        // pnlBottomLop
        pnlBottomLop.Controls.Add(btnNgungLop);
        pnlBottomLop.Controls.Add(btnSuaLop);
        pnlBottomLop.Controls.Add(btnThemLop);
        pnlBottomLop.Dock = DockStyle.Bottom;
        pnlBottomLop.Location = new Point(3, 517);
        pnlBottomLop.Name = "pnlBottomLop";
        pnlBottomLop.Size = new Size(1006, 50);
        pnlBottomLop.TabIndex = 1;

        btnNgungLop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnNgungLop.BackColor = Color.FromArgb(239, 68, 68);
        btnNgungLop.FlatAppearance.BorderSize = 0;
        btnNgungLop.FlatStyle = FlatStyle.Flat;
        btnNgungLop.ForeColor = Color.White;
        btnNgungLop.Location = new Point(866, 10);
        btnNgungLop.Name = "btnNgungLop";
        btnNgungLop.Size = new Size(130, 32);
        btnNgungLop.TabIndex = 2;
        btnNgungLop.Text = "Ngừng hoạt động";
        btnNgungLop.UseVisualStyleBackColor = false;
        btnNgungLop.Click += btnNgungLop_Click;

        btnSuaLop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSuaLop.BackColor = Color.FromArgb(245, 158, 11);
        btnSuaLop.FlatAppearance.BorderSize = 0;
        btnSuaLop.FlatStyle = FlatStyle.Flat;
        btnSuaLop.ForeColor = Color.White;
        btnSuaLop.Location = new Point(760, 10);
        btnSuaLop.Name = "btnSuaLop";
        btnSuaLop.Size = new Size(100, 32);
        btnSuaLop.TabIndex = 1;
        btnSuaLop.Text = "Cập nhật";
        btnSuaLop.UseVisualStyleBackColor = false;
        btnSuaLop.Click += btnSuaLop_Click;

        btnThemLop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnThemLop.BackColor = Color.FromArgb(16, 185, 129);
        btnThemLop.FlatAppearance.BorderSize = 0;
        btnThemLop.FlatStyle = FlatStyle.Flat;
        btnThemLop.ForeColor = Color.White;
        btnThemLop.Location = new Point(654, 10);
        btnThemLop.Name = "btnThemLop";
        btnThemLop.Size = new Size(100, 32);
        btnThemLop.TabIndex = 0;
        btnThemLop.Text = "Thêm lớp";
        btnThemLop.UseVisualStyleBackColor = false;
        btnThemLop.Click += btnThemLop_Click;

        // pnlTopLop
        pnlTopLop.Controls.Add(btnTimLop);
        pnlTopLop.Controls.Add(txtTimKiemLop);
        pnlTopLop.Controls.Add(lblTimLop);
        pnlTopLop.Controls.Add(cboFilterKhoaHoc);
        pnlTopLop.Controls.Add(lblKhoaHoc);
        pnlTopLop.Dock = DockStyle.Top;
        pnlTopLop.Location = new Point(3, 3);
        pnlTopLop.Name = "pnlTopLop";
        pnlTopLop.Size = new Size(1006, 60);
        pnlTopLop.TabIndex = 0;

        btnTimLop.BackColor = Color.FromArgb(37, 99, 235);
        btnTimLop.FlatAppearance.BorderSize = 0;
        btnTimLop.FlatStyle = FlatStyle.Flat;
        btnTimLop.ForeColor = Color.White;
        btnTimLop.Location = new Point(690, 15);
        btnTimLop.Name = "btnTimLop";
        btnTimLop.Size = new Size(85, 29);
        btnTimLop.TabIndex = 4;
        btnTimLop.Text = "Tìm kiếm";
        btnTimLop.UseVisualStyleBackColor = false;
        btnTimLop.Click += btnTimLop_Click;

        txtTimKiemLop.Location = new Point(480, 17);
        txtTimKiemLop.Name = "txtTimKiemLop";
        txtTimKiemLop.PlaceholderText = "Nhập mã hoặc tên lớp...";
        txtTimKiemLop.Size = new Size(200, 25);
        txtTimKiemLop.TabIndex = 3;

        lblTimLop.AutoSize = true;
        lblTimLop.Location = new Point(410, 20);
        lblTimLop.Name = "lblTimLop";
        lblTimLop.Size = new Size(66, 19);
        lblTimLop.TabIndex = 2;
        lblTimLop.Text = "Từ khóa:";

        cboFilterKhoaHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilterKhoaHoc.FormattingEnabled = true;
        cboFilterKhoaHoc.Location = new Point(90, 17);
        cboFilterKhoaHoc.Name = "cboFilterKhoaHoc";
        cboFilterKhoaHoc.Size = new Size(290, 25);
        cboFilterKhoaHoc.TabIndex = 1;

        lblKhoaHoc.AutoSize = true;
        lblKhoaHoc.Location = new Point(15, 20);
        lblKhoaHoc.Name = "lblKhoaHoc";
        lblKhoaHoc.Size = new Size(69, 19);
        lblKhoaHoc.TabIndex = 0;
        lblKhoaHoc.Text = "Khóa học:";

        // ================= TAB XẾP LỚP =================
        tabXepLop.Controls.Add(dgvDangKy);
        tabXepLop.Controls.Add(pnlBottomXepLop);
        tabXepLop.Controls.Add(pnlTopXepLop);
        tabXepLop.Location = new Point(4, 26);
        tabXepLop.Name = "tabXepLop";
        tabXepLop.Padding = new Padding(3);
        tabXepLop.Size = new Size(1012, 570);
        tabXepLop.TabIndex = 1;
        tabXepLop.Text = "  Xếp lớp học viên  ";
        tabXepLop.UseVisualStyleBackColor = true;

        // dgvDangKy
        dgvDangKy.AllowUserToAddRows = false;
        dgvDangKy.AllowUserToDeleteRows = false;
        dgvDangKy.AutoGenerateColumns = false;
        dgvDangKy.BackgroundColor = Color.White;
        dgvDangKy.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvDangKy.Columns.AddRange(new DataGridViewColumn[] {
            colDk_Id, colDk_MaHocVien, colDk_HoTen, colDk_DienThoai, colDk_NgayDangKy,
            colDk_HocPhiPhaiThu, colDk_DaThu, colDk_ConNo, colDk_TrangThai, colDk_GhiChu
        });
        dgvDangKy.Dock = DockStyle.Fill;
        dgvDangKy.Location = new Point(3, 63);
        dgvDangKy.MultiSelect = false;
        dgvDangKy.Name = "dgvDangKy";
        dgvDangKy.ReadOnly = true;
        dgvDangKy.RowHeadersVisible = false;
        dgvDangKy.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvDangKy.Size = new Size(1006, 454);
        dgvDangKy.TabIndex = 2;

        colDk_Id.DataPropertyName = "Id";
        colDk_Id.HeaderText = "ID";
        colDk_Id.Name = "colDk_Id";
        colDk_Id.ReadOnly = true;
        colDk_Id.Visible = false;

        colDk_MaHocVien.DataPropertyName = "MaHocVien";
        colDk_MaHocVien.HeaderText = "Mã HV";
        colDk_MaHocVien.Name = "colDk_MaHocVien";
        colDk_MaHocVien.ReadOnly = true;
        colDk_MaHocVien.Width = 90;

        colDk_HoTen.DataPropertyName = "HoTenHocVien";
        colDk_HoTen.HeaderText = "Họ và tên";
        colDk_HoTen.Name = "colDk_HoTen";
        colDk_HoTen.ReadOnly = true;
        colDk_HoTen.Width = 160;

        colDk_DienThoai.DataPropertyName = "DienThoaiHocVien";
        colDk_DienThoai.HeaderText = "Điện thoại";
        colDk_DienThoai.MinimumWidth = 115;
        colDk_DienThoai.Name = "colDk_DienThoai";
        colDk_DienThoai.ReadOnly = true;
        colDk_DienThoai.Width = 115;

        colDk_NgayDangKy.DataPropertyName = "NgayDangKy";
        colDk_NgayDangKy.HeaderText = "Ngày ĐK";
        colDk_NgayDangKy.Name = "colDk_NgayDangKy";
        colDk_NgayDangKy.ReadOnly = true;
        colDk_NgayDangKy.Width = 95;

        colDk_HocPhiPhaiThu.DataPropertyName = "HocPhiPhaiThu";
        colDk_HocPhiPhaiThu.HeaderText = "Học phí phải thu";
        colDk_HocPhiPhaiThu.Name = "colDk_HocPhiPhaiThu";
        colDk_HocPhiPhaiThu.ReadOnly = true;
        colDk_HocPhiPhaiThu.Width = 120;

        colDk_DaThu.DataPropertyName = "DaThu";
        colDk_DaThu.HeaderText = "Đã thu";
        colDk_DaThu.Name = "colDk_DaThu";
        colDk_DaThu.ReadOnly = true;
        colDk_DaThu.Width = 110;

        colDk_ConNo.DataPropertyName = "ConNo";
        colDk_ConNo.HeaderText = "Còn nợ";
        colDk_ConNo.Name = "colDk_ConNo";
        colDk_ConNo.ReadOnly = true;
        colDk_ConNo.Width = 110;

        colDk_TrangThai.DataPropertyName = "TrangThai";
        colDk_TrangThai.HeaderText = "Trạng thái";
        colDk_TrangThai.Name = "colDk_TrangThai";
        colDk_TrangThai.ReadOnly = true;
        colDk_TrangThai.Width = 100;

        colDk_GhiChu.DataPropertyName = "GhiChu";
        colDk_GhiChu.HeaderText = "Ghi chú";
        colDk_GhiChu.MinimumWidth = 180;
        colDk_GhiChu.Name = "colDk_GhiChu";
        colDk_GhiChu.ReadOnly = true;
        colDk_GhiChu.Width = 180;

        // pnlBottomXepLop
        pnlBottomXepLop.Controls.Add(btnHuyXepLop);
        pnlBottomXepLop.Controls.Add(btnXepLop);
        pnlBottomXepLop.Dock = DockStyle.Bottom;
        pnlBottomXepLop.Location = new Point(3, 517);
        pnlBottomXepLop.Name = "pnlBottomXepLop";
        pnlBottomXepLop.Size = new Size(1006, 50);
        pnlBottomXepLop.TabIndex = 1;

        btnHuyXepLop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnHuyXepLop.BackColor = Color.FromArgb(239, 68, 68);
        btnHuyXepLop.FlatAppearance.BorderSize = 0;
        btnHuyXepLop.FlatStyle = FlatStyle.Flat;
        btnHuyXepLop.ForeColor = Color.White;
        btnHuyXepLop.Location = new Point(886, 10);
        btnHuyXepLop.Name = "btnHuyXepLop";
        btnHuyXepLop.Size = new Size(110, 32);
        btnHuyXepLop.TabIndex = 1;
        btnHuyXepLop.Text = "Hủy xếp lớp";
        btnHuyXepLop.UseVisualStyleBackColor = false;
        btnHuyXepLop.Click += btnHuyXepLop_Click;

        btnXepLop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnXepLop.BackColor = Color.FromArgb(16, 185, 129);
        btnXepLop.FlatAppearance.BorderSize = 0;
        btnXepLop.FlatStyle = FlatStyle.Flat;
        btnXepLop.ForeColor = Color.White;
        btnXepLop.Location = new Point(746, 10);
        btnXepLop.Name = "btnXepLop";
        btnXepLop.Size = new Size(130, 32);
        btnXepLop.TabIndex = 0;
        btnXepLop.Text = "+ Xếp học viên";
        btnXepLop.UseVisualStyleBackColor = false;
        btnXepLop.Click += btnXepLop_Click;

        // pnlTopXepLop
        pnlTopXepLop.Controls.Add(btnTimDangKy);
        pnlTopXepLop.Controls.Add(txtTimKiemHocVien);
        pnlTopXepLop.Controls.Add(lblTimHV);
        pnlTopXepLop.Controls.Add(cboXepLop_LopHoc);
        pnlTopXepLop.Controls.Add(lblChonLop);
        pnlTopXepLop.Dock = DockStyle.Top;
        pnlTopXepLop.Location = new Point(3, 3);
        pnlTopXepLop.Name = "pnlTopXepLop";
        pnlTopXepLop.Size = new Size(1006, 60);
        pnlTopXepLop.TabIndex = 0;

        btnTimDangKy.BackColor = Color.FromArgb(37, 99, 235);
        btnTimDangKy.FlatAppearance.BorderSize = 0;
        btnTimDangKy.FlatStyle = FlatStyle.Flat;
        btnTimDangKy.ForeColor = Color.White;
        btnTimDangKy.Location = new Point(710, 15);
        btnTimDangKy.Name = "btnTimDangKy";
        btnTimDangKy.Size = new Size(85, 29);
        btnTimDangKy.TabIndex = 4;
        btnTimDangKy.Text = "Tìm kiếm";
        btnTimDangKy.UseVisualStyleBackColor = false;
        btnTimDangKy.Click += btnTimDangKy_Click;

        txtTimKiemHocVien.Location = new Point(490, 17);
        txtTimKiemHocVien.Name = "txtTimKiemHocVien";
        txtTimKiemHocVien.PlaceholderText = "Mã hoặc tên học viên...";
        txtTimKiemHocVien.Size = new Size(210, 25);
        txtTimKiemHocVien.TabIndex = 3;

        lblTimHV.AutoSize = true;
        lblTimHV.Location = new Point(410, 20);
        lblTimHV.Name = "lblTimHV";
        lblTimHV.Size = new Size(74, 19);
        lblTimHV.TabIndex = 2;
        lblTimHV.Text = "Tìm kiếm:";

        cboXepLop_LopHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboXepLop_LopHoc.FormattingEnabled = true;
        cboXepLop_LopHoc.Location = new Point(90, 17);
        cboXepLop_LopHoc.Name = "cboXepLop_LopHoc";
        cboXepLop_LopHoc.Size = new Size(290, 25);
        cboXepLop_LopHoc.TabIndex = 1;
        cboXepLop_LopHoc.SelectedIndexChanged += cboXepLop_LopHoc_SelectedIndexChanged;

        lblChonLop.AutoSize = true;
        lblChonLop.Location = new Point(15, 20);
        lblChonLop.Name = "lblChonLop";
        lblChonLop.Size = new Size(69, 19);
        lblChonLop.TabIndex = 0;
        lblChonLop.Text = "Chọn lớp:";

        // ================= TAB LỊCH HỌC =================
        tabLichHoc.Controls.Add(dgvLichHoc);
        tabLichHoc.Controls.Add(pnlBottomLich);
        tabLichHoc.Controls.Add(pnlTopLich);
        tabLichHoc.Location = new Point(4, 26);
        tabLichHoc.Name = "tabLichHoc";
        tabLichHoc.Padding = new Padding(3);
        tabLichHoc.Size = new Size(1012, 570);
        tabLichHoc.TabIndex = 2;
        tabLichHoc.Text = "  Thời khóa biểu / Lịch học  ";
        tabLichHoc.UseVisualStyleBackColor = true;

        // dgvLichHoc
        dgvLichHoc.AllowUserToAddRows = false;
        dgvLichHoc.AllowUserToDeleteRows = false;
        dgvLichHoc.AutoGenerateColumns = false;
        dgvLichHoc.BackgroundColor = Color.White;
        dgvLichHoc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvLichHoc.Columns.AddRange(new DataGridViewColumn[] {
            colLich_Id, colLich_MonHoc, colLich_GiangVien, colLich_PhongHoc,
            colLich_NgayHoc, colLich_ThoiGian, colLich_NoiDung
        });
        dgvLichHoc.Dock = DockStyle.Fill;
        dgvLichHoc.Location = new Point(3, 63);
        dgvLichHoc.MultiSelect = false;
        dgvLichHoc.Name = "dgvLichHoc";
        dgvLichHoc.ReadOnly = true;
        dgvLichHoc.RowHeadersVisible = false;
        dgvLichHoc.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvLichHoc.Size = new Size(1006, 454);
        dgvLichHoc.TabIndex = 2;

        colLich_Id.DataPropertyName = "Id";
        colLich_Id.HeaderText = "ID";
        colLich_Id.Name = "colLich_Id";
        colLich_Id.ReadOnly = true;
        colLich_Id.Visible = false;

        colLich_MonHoc.DataPropertyName = "TenMonHoc";
        colLich_MonHoc.HeaderText = "Môn học";
        colLich_MonHoc.MinimumWidth = 180;
        colLich_MonHoc.Name = "colLich_MonHoc";
        colLich_MonHoc.ReadOnly = true;
        colLich_MonHoc.Width = 180;

        colLich_GiangVien.DataPropertyName = "HoTenGiangVien";
        colLich_GiangVien.HeaderText = "Giảng viên";
        colLich_GiangVien.Name = "colLich_GiangVien";
        colLich_GiangVien.ReadOnly = true;
        colLich_GiangVien.Width = 150;

        colLich_PhongHoc.DataPropertyName = "TenPhong";
        colLich_PhongHoc.HeaderText = "Phòng học";
        colLich_PhongHoc.MinimumWidth = 140;
        colLich_PhongHoc.Name = "colLich_PhongHoc";
        colLich_PhongHoc.ReadOnly = true;
        colLich_PhongHoc.Width = 140;

        colLich_NgayHoc.DataPropertyName = "NgayHoc";
        colLich_NgayHoc.HeaderText = "Ngày học";
        colLich_NgayHoc.Name = "colLich_NgayHoc";
        colLich_NgayHoc.ReadOnly = true;
        colLich_NgayHoc.Width = 100;

        colLich_ThoiGian.DataPropertyName = "ThoiGian";
        colLich_ThoiGian.HeaderText = "Thời gian";
        colLich_ThoiGian.Name = "colLich_ThoiGian";
        colLich_ThoiGian.ReadOnly = true;
        colLich_ThoiGian.Width = 115;

        colLich_NoiDung.DataPropertyName = "NoiDung";
        colLich_NoiDung.HeaderText = "Nội dung bài giảng";
        colLich_NoiDung.MinimumWidth = 240;
        colLich_NoiDung.Name = "colLich_NoiDung";
        colLich_NoiDung.ReadOnly = true;
        colLich_NoiDung.Width = 240;

        // pnlBottomLich
        pnlBottomLich.Controls.Add(btnXoaLich);
        pnlBottomLich.Controls.Add(btnSuaLich);
        pnlBottomLich.Controls.Add(btnThemLich);
        pnlBottomLich.Dock = DockStyle.Bottom;
        pnlBottomLich.Location = new Point(3, 517);
        pnlBottomLich.Name = "pnlBottomLich";
        pnlBottomLich.Size = new Size(1006, 50);
        pnlBottomLich.TabIndex = 1;

        btnXoaLich.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnXoaLich.BackColor = Color.FromArgb(239, 68, 68);
        btnXoaLich.FlatAppearance.BorderSize = 0;
        btnXoaLich.FlatStyle = FlatStyle.Flat;
        btnXoaLich.ForeColor = Color.White;
        btnXoaLich.Location = new Point(886, 10);
        btnXoaLich.Name = "btnXoaLich";
        btnXoaLich.Size = new Size(110, 32);
        btnXoaLich.TabIndex = 2;
        btnXoaLich.Text = "Hủy buổi học";
        btnXoaLich.UseVisualStyleBackColor = false;
        btnXoaLich.Click += btnXoaLich_Click;

        btnSuaLich.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSuaLich.BackColor = Color.FromArgb(245, 158, 11);
        btnSuaLich.FlatAppearance.BorderSize = 0;
        btnSuaLich.FlatStyle = FlatStyle.Flat;
        btnSuaLich.ForeColor = Color.White;
        btnSuaLich.Location = new Point(780, 10);
        btnSuaLich.Name = "btnSuaLich";
        btnSuaLich.Size = new Size(100, 32);
        btnSuaLich.TabIndex = 1;
        btnSuaLich.Text = "Cập nhật";
        btnSuaLich.UseVisualStyleBackColor = false;
        btnSuaLich.Click += btnSuaLich_Click;

        btnThemLich.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnThemLich.BackColor = Color.FromArgb(16, 185, 129);
        btnThemLich.FlatAppearance.BorderSize = 0;
        btnThemLich.FlatStyle = FlatStyle.Flat;
        btnThemLich.ForeColor = Color.White;
        btnThemLich.Location = new Point(674, 10);
        btnThemLich.Name = "btnThemLich";
        btnThemLich.Size = new Size(100, 32);
        btnThemLich.TabIndex = 0;
        btnThemLich.Text = "+ Thêm lịch";
        btnThemLich.UseVisualStyleBackColor = false;
        btnThemLich.Click += btnThemLich_Click;

        // pnlTopLich
        pnlTopLich.Controls.Add(cboLichHoc_LopHoc);
        pnlTopLich.Controls.Add(lblChonLopLich);
        pnlTopLich.Dock = DockStyle.Top;
        pnlTopLich.Location = new Point(3, 3);
        pnlTopLich.Name = "pnlTopLich";
        pnlTopLich.Size = new Size(1006, 60);
        pnlTopLich.TabIndex = 0;

        cboLichHoc_LopHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLichHoc_LopHoc.FormattingEnabled = true;
        cboLichHoc_LopHoc.Location = new Point(90, 17);
        cboLichHoc_LopHoc.Name = "cboLichHoc_LopHoc";
        cboLichHoc_LopHoc.Size = new Size(290, 25);
        cboLichHoc_LopHoc.TabIndex = 1;
        cboLichHoc_LopHoc.SelectedIndexChanged += cboLichHoc_LopHoc_SelectedIndexChanged;

        lblChonLopLich.AutoSize = true;
        lblChonLopLich.Location = new Point(15, 20);
        lblChonLopLich.Name = "lblChonLopLich";
        lblChonLopLich.Size = new Size(69, 19);
        lblChonLopLich.TabIndex = 0;
        lblChonLopLich.Text = "Chọn lớp:";

        // FrmDaoTao
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1020, 600);
        Controls.Add(tabControlDaoTao);
        Font = new Font("Segoe UI", 9.75F);
        Name = "FrmDaoTao";
        Text = "Quản lý Đào tạo";

        tabControlDaoTao.ResumeLayout(false);
        tabLopHoc.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvLopHoc).EndInit();
        pnlBottomLop.ResumeLayout(false);
        pnlTopLop.ResumeLayout(false);
        pnlTopLop.PerformLayout();
        tabXepLop.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvDangKy).EndInit();
        pnlBottomXepLop.ResumeLayout(false);
        pnlTopXepLop.ResumeLayout(false);
        pnlTopXepLop.PerformLayout();
        tabLichHoc.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvLichHoc).EndInit();
        pnlBottomLich.ResumeLayout(false);
        pnlTopLich.ResumeLayout(false);
        pnlTopLich.PerformLayout();
        ResumeLayout(false);
    }

    private TabControl tabControlDaoTao;
    private TabPage tabLopHoc;
    private DataGridView dgvLopHoc;
    private Panel pnlBottomLop;
    private Button btnNgungLop;
    private Button btnSuaLop;
    private Button btnThemLop;
    private Panel pnlTopLop;
    private Button btnTimLop;
    private TextBox txtTimKiemLop;
    private Label lblTimLop;
    private ComboBox cboFilterKhoaHoc;
    private Label lblKhoaHoc;

    private TabPage tabXepLop;
    private DataGridView dgvDangKy;
    private Panel pnlBottomXepLop;
    private Button btnHuyXepLop;
    private Button btnXepLop;
    private Panel pnlTopXepLop;
    private Button btnTimDangKy;
    private TextBox txtTimKiemHocVien;
    private Label lblTimHV;
    private ComboBox cboXepLop_LopHoc;
    private Label lblChonLop;

    private TabPage tabLichHoc;
    private DataGridView dgvLichHoc;
    private Panel pnlBottomLich;
    private Button btnXoaLich;
    private Button btnSuaLich;
    private Button btnThemLich;
    private Panel pnlTopLich;
    private ComboBox cboLichHoc_LopHoc;
    private Label lblChonLopLich;

    // Columns for dgvLopHoc
    private DataGridViewTextBoxColumn colLop_Id;
    private DataGridViewTextBoxColumn colLop_MaLop;
    private DataGridViewTextBoxColumn colLop_TenLop;
    private DataGridViewTextBoxColumn colLop_KhoaHoc;
    private DataGridViewTextBoxColumn colLop_GiangVien;
    private DataGridViewTextBoxColumn colLop_NgayBatDau;
    private DataGridViewTextBoxColumn colLop_NgayKetThuc;
    private DataGridViewTextBoxColumn colLop_SiSo;
    private DataGridViewTextBoxColumn colLop_TrangThai;
    private DataGridViewTextBoxColumn colLop_GhiChu;

    // Columns for dgvDangKy
    private DataGridViewTextBoxColumn colDk_Id;
    private DataGridViewTextBoxColumn colDk_MaHocVien;
    private DataGridViewTextBoxColumn colDk_HoTen;
    private DataGridViewTextBoxColumn colDk_DienThoai;
    private DataGridViewTextBoxColumn colDk_NgayDangKy;
    private DataGridViewTextBoxColumn colDk_HocPhiPhaiThu;
    private DataGridViewTextBoxColumn colDk_DaThu;
    private DataGridViewTextBoxColumn colDk_ConNo;
    private DataGridViewTextBoxColumn colDk_TrangThai;
    private DataGridViewTextBoxColumn colDk_GhiChu;

    // Columns for dgvLichHoc
    private DataGridViewTextBoxColumn colLich_Id;
    private DataGridViewTextBoxColumn colLich_MonHoc;
    private DataGridViewTextBoxColumn colLich_GiangVien;
    private DataGridViewTextBoxColumn colLich_PhongHoc;
    private DataGridViewTextBoxColumn colLich_NgayHoc;
    private DataGridViewTextBoxColumn colLich_ThoiGian;
    private DataGridViewTextBoxColumn colLich_NoiDung;
}
