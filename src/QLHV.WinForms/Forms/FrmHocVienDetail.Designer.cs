namespace QLHV.WinForms.Forms;

partial class FrmHocVienDetail
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
        lblTitle = new Label();
        grpCoBan = new GroupBox();
        lblMaHocVien = new Label();
        lblHoTen = new Label();
        lblNgaySinh = new Label();
        lblGioiTinh = new Label();
        lblDienThoai = new Label();
        lblEmail = new Label();
        lblDiaChi = new Label();
        lblTrangThai = new Label();

        grpDinhDanh = new GroupBox();
        lblSoDinhDanh = new Label();
        btnToggleMask = new Button();
        lblNgayCap = new Label();
        lblNoiCap = new Label();

        grpGiamHo = new GroupBox();
        lblNguoiGiamHo = new Label();
        lblSdtGiamHo = new Label();
        lblQuanHe = new Label();

        grpTuyenSinh = new GroupBox();
        lblNhanVien = new Label();
        lblNguon = new Label();
        lblNgheNghiep = new Label();
        lblDonVi = new Label();
        lblGhiChu = new Label();

        grpLichSu = new GroupBox();
        dgvLichSu = new DataGridView();
        colMaLop = new DataGridViewTextBoxColumn();
        colTenLop = new DataGridViewTextBoxColumn();
        colTenKhoaHoc = new DataGridViewTextBoxColumn();
        colNgayDangKy = new DataGridViewTextBoxColumn();
        colTrangThai = new DataGridViewTextBoxColumn();
        colHocPhiPhaiThu = new DataGridViewTextBoxColumn();
        colDaThu = new DataGridViewTextBoxColumn();
        colConNo = new DataGridViewTextBoxColumn();

        btnClose = new Button();

        grpCoBan.SuspendLayout();
        grpDinhDanh.SuspendLayout();
        grpGiamHo.SuspendLayout();
        grpTuyenSinh.SuspendLayout();
        grpLichSu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvLichSu).BeginInit();
        SuspendLayout();

        // lblTitle
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 64, 175);
        lblTitle.Location = new Point(20, 15);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(270, 25);
        lblTitle.Text = "HỒ SƠ CHI TIẾT HỌC VIÊN";

        // grpCoBan
        grpCoBan.Controls.Add(lblMaHocVien);
        grpCoBan.Controls.Add(lblHoTen);
        grpCoBan.Controls.Add(lblNgaySinh);
        grpCoBan.Controls.Add(lblGioiTinh);
        grpCoBan.Controls.Add(lblDienThoai);
        grpCoBan.Controls.Add(lblEmail);
        grpCoBan.Controls.Add(lblDiaChi);
        grpCoBan.Controls.Add(lblTrangThai);
        grpCoBan.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpCoBan.Location = new Point(20, 50);
        grpCoBan.Name = "grpCoBan";
        grpCoBan.Size = new Size(420, 160);
        grpCoBan.Text = "1. Thông tin cơ bản";

        lblMaHocVien.Font = new Font("Segoe UI", 9F);
        lblMaHocVien.Location = new Point(15, 25);
        lblMaHocVien.Size = new Size(180, 20);
        lblMaHocVien.Text = "Mã HV: -";

        lblHoTen.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblHoTen.Location = new Point(200, 25);
        lblHoTen.Size = new Size(205, 20);
        lblHoTen.Text = "Họ tên: -";

        lblNgaySinh.Font = new Font("Segoe UI", 9F);
        lblNgaySinh.Location = new Point(15, 55);
        lblNgaySinh.Size = new Size(180, 20);
        lblNgaySinh.Text = "Ngày sinh: -";

        lblGioiTinh.Font = new Font("Segoe UI", 9F);
        lblGioiTinh.Location = new Point(200, 55);
        lblGioiTinh.Size = new Size(205, 20);
        lblGioiTinh.Text = "Giới tính: -";

        lblDienThoai.Font = new Font("Segoe UI", 9F);
        lblDienThoai.Location = new Point(15, 85);
        lblDienThoai.Size = new Size(180, 20);
        lblDienThoai.Text = "Điện thoại: -";

        lblEmail.Font = new Font("Segoe UI", 9F);
        lblEmail.Location = new Point(200, 85);
        lblEmail.Size = new Size(205, 20);
        lblEmail.Text = "Email: -";

        lblDiaChi.Font = new Font("Segoe UI", 9F);
        lblDiaChi.Location = new Point(15, 115);
        lblDiaChi.Size = new Size(390, 20);
        lblDiaChi.Text = "Địa chỉ: -";

        lblTrangThai.Font = new Font("Segoe UI", 9F);
        lblTrangThai.Location = new Point(15, 135);
        lblTrangThai.Size = new Size(390, 20);
        lblTrangThai.Text = "Trạng thái: -";

        // grpDinhDanh
        grpDinhDanh.Controls.Add(lblSoDinhDanh);
        grpDinhDanh.Controls.Add(btnToggleMask);
        grpDinhDanh.Controls.Add(lblNgayCap);
        grpDinhDanh.Controls.Add(lblNoiCap);
        grpDinhDanh.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpDinhDanh.Location = new Point(455, 50);
        grpDinhDanh.Name = "grpDinhDanh";
        grpDinhDanh.Size = new Size(420, 160);
        grpDinhDanh.Text = "2. Giấy tờ định danh";

        lblSoDinhDanh.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblSoDinhDanh.ForeColor = Color.FromArgb(17, 24, 39);
        lblSoDinhDanh.Location = new Point(15, 25);
        lblSoDinhDanh.Size = new Size(280, 25);
        lblSoDinhDanh.Text = "Số CCCD/CMND: -";

        btnToggleMask.Font = new Font("Segoe UI", 8F);
        btnToggleMask.Location = new Point(305, 23);
        btnToggleMask.Name = "btnToggleMask";
        btnToggleMask.Size = new Size(100, 26);
        btnToggleMask.Text = "Hiện số";
        btnToggleMask.UseVisualStyleBackColor = true;
        btnToggleMask.Visible = false;

        lblNgayCap.Font = new Font("Segoe UI", 9F);
        lblNgayCap.Location = new Point(15, 60);
        lblNgayCap.Size = new Size(390, 20);
        lblNgayCap.Text = "Ngày cấp: -";

        lblNoiCap.Font = new Font("Segoe UI", 9F);
        lblNoiCap.Location = new Point(15, 95);
        lblNoiCap.Size = new Size(390, 50);
        lblNoiCap.Text = "Nơi cấp: -";

        // grpGiamHo
        grpGiamHo.Controls.Add(lblNguoiGiamHo);
        grpGiamHo.Controls.Add(lblSdtGiamHo);
        grpGiamHo.Controls.Add(lblQuanHe);
        grpGiamHo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpGiamHo.Location = new Point(20, 220);
        grpGiamHo.Name = "grpGiamHo";
        grpGiamHo.Size = new Size(420, 120);
        grpGiamHo.Text = "3. Người giám hộ";

        lblNguoiGiamHo.Font = new Font("Segoe UI", 9F);
        lblNguoiGiamHo.Location = new Point(15, 25);
        lblNguoiGiamHo.Size = new Size(390, 20);
        lblNguoiGiamHo.Text = "Người giám hộ: -";

        lblSdtGiamHo.Font = new Font("Segoe UI", 9F);
        lblSdtGiamHo.Location = new Point(15, 55);
        lblSdtGiamHo.Size = new Size(390, 20);
        lblSdtGiamHo.Text = "Điện thoại giám hộ: -";

        lblQuanHe.Font = new Font("Segoe UI", 9F);
        lblQuanHe.Location = new Point(15, 85);
        lblQuanHe.Size = new Size(390, 20);
        lblQuanHe.Text = "Quan hệ: -";

        // grpTuyenSinh
        grpTuyenSinh.Controls.Add(lblNhanVien);
        grpTuyenSinh.Controls.Add(lblNguon);
        grpTuyenSinh.Controls.Add(lblNgheNghiep);
        grpTuyenSinh.Controls.Add(lblDonVi);
        grpTuyenSinh.Controls.Add(lblGhiChu);
        grpTuyenSinh.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpTuyenSinh.Location = new Point(455, 220);
        grpTuyenSinh.Name = "grpTuyenSinh";
        grpTuyenSinh.Size = new Size(420, 120);
        grpTuyenSinh.Text = "4. Tuyển sinh & Ghi chú";

        lblNhanVien.Font = new Font("Segoe UI", 9F);
        lblNhanVien.Location = new Point(15, 22);
        lblNhanVien.Size = new Size(390, 18);
        lblNhanVien.Text = "Nhân viên phụ trách: -";

        lblNguon.Font = new Font("Segoe UI", 9F);
        lblNguon.Location = new Point(15, 42);
        lblNguon.Size = new Size(390, 18);
        lblNguon.Text = "Nguồn tuyển sinh: -";

        lblNgheNghiep.Font = new Font("Segoe UI", 9F);
        lblNgheNghiep.Location = new Point(15, 62);
        lblNgheNghiep.Size = new Size(390, 18);
        lblNgheNghiep.Text = "Nghề nghiệp: -";

        lblDonVi.Font = new Font("Segoe UI", 9F);
        lblDonVi.Location = new Point(15, 82);
        lblDonVi.Size = new Size(390, 18);
        lblDonVi.Text = "Đơn vị / Trường: -";

        lblGhiChu.Font = new Font("Segoe UI", 9F);
        lblGhiChu.Location = new Point(15, 100);
        lblGhiChu.Size = new Size(390, 18);
        lblGhiChu.Text = "Ghi chú: -";

        // grpLichSu
        grpLichSu.Controls.Add(dgvLichSu);
        grpLichSu.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpLichSu.Location = new Point(20, 350);
        grpLichSu.Name = "grpLichSu";
        grpLichSu.Size = new Size(855, 210);
        grpLichSu.Text = "5. Quá trình đào tạo (Lớp học & Học phí)";

        dgvLichSu.AllowUserToAddRows = false;
        dgvLichSu.AllowUserToDeleteRows = false;
        dgvLichSu.BackgroundColor = Color.White;
        dgvLichSu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvLichSu.Columns.AddRange(new DataGridViewColumn[] {
            colMaLop, colTenLop, colTenKhoaHoc, colNgayDangKy, colTrangThai, colHocPhiPhaiThu, colDaThu, colConNo
        });
        dgvLichSu.Dock = DockStyle.Fill;
        dgvLichSu.Font = new Font("Segoe UI", 9F);
        dgvLichSu.Location = new Point(3, 20);
        dgvLichSu.MultiSelect = false;
        dgvLichSu.Name = "dgvLichSu";
        dgvLichSu.ReadOnly = true;
        dgvLichSu.RowHeadersVisible = false;
        dgvLichSu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvLichSu.Size = new Size(849, 187);

        colMaLop.DataPropertyName = "MaLopHoc";
        colMaLop.HeaderText = "Mã lớp";
        colMaLop.Name = "colMaLop";
        colMaLop.Width = 90;

        colTenLop.DataPropertyName = "TenLopHoc";
        colTenLop.HeaderText = "Tên lớp học";
        colTenLop.Name = "colTenLop";
        colTenLop.Width = 170;

        colTenKhoaHoc.DataPropertyName = "TenKhoaHoc";
        colTenKhoaHoc.HeaderText = "Khóa học";
        colTenKhoaHoc.Name = "colTenKhoaHoc";
        colTenKhoaHoc.Width = 160;

        colNgayDangKy.DataPropertyName = "NgayDangKy";
        colNgayDangKy.HeaderText = "Ngày ĐK";
        colNgayDangKy.Name = "colNgayDangKy";
        colNgayDangKy.Width = 85;

        colTrangThai.DataPropertyName = "TrangThai";
        colTrangThai.HeaderText = "Trạng thái";
        colTrangThai.Name = "colTrangThai";
        colTrangThai.Width = 100;

        colHocPhiPhaiThu.DataPropertyName = "HocPhiPhaiThu";
        colHocPhiPhaiThu.HeaderText = "Học phí";
        colHocPhiPhaiThu.Name = "colHocPhiPhaiThu";
        colHocPhiPhaiThu.Width = 90;

        colDaThu.DataPropertyName = "DaThu";
        colDaThu.HeaderText = "Đã thu";
        colDaThu.Name = "colDaThu";
        colDaThu.Width = 90;

        colConNo.DataPropertyName = "ConNo";
        colConNo.HeaderText = "Còn nợ";
        colConNo.Name = "colConNo";
        colConNo.Width = 90;

        // btnClose
        btnClose.BackColor = Color.FromArgb(243, 244, 246);
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnClose.Location = new Point(775, 570);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(100, 32);
        btnClose.Text = "Đóng";
        btnClose.UseVisualStyleBackColor = false;
        btnClose.Click += (s, e) => Close();

        // Form
        AcceptButton = btnClose;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnClose;
        ClientSize = new Size(895, 615);
        Controls.Add(lblTitle);
        Controls.Add(grpCoBan);
        Controls.Add(grpDinhDanh);
        Controls.Add(grpGiamHo);
        Controls.Add(grpTuyenSinh);
        Controls.Add(grpLichSu);
        Controls.Add(btnClose);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Hồ sơ chi tiết học viên";

        grpCoBan.ResumeLayout(false);
        grpDinhDanh.ResumeLayout(false);
        grpGiamHo.ResumeLayout(false);
        grpTuyenSinh.ResumeLayout(false);
        grpLichSu.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvLichSu).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private GroupBox grpCoBan;
    private Label lblMaHocVien;
    private Label lblHoTen;
    private Label lblNgaySinh;
    private Label lblGioiTinh;
    private Label lblDienThoai;
    private Label lblEmail;
    private Label lblDiaChi;
    private Label lblTrangThai;

    private GroupBox grpDinhDanh;
    private Label lblSoDinhDanh;
    private Button btnToggleMask;
    private Label lblNgayCap;
    private Label lblNoiCap;

    private GroupBox grpGiamHo;
    private Label lblNguoiGiamHo;
    private Label lblSdtGiamHo;
    private Label lblQuanHe;

    private GroupBox grpTuyenSinh;
    private Label lblNhanVien;
    private Label lblNguon;
    private Label lblNgheNghiep;
    private Label lblDonVi;
    private Label lblGhiChu;

    private GroupBox grpLichSu;
    private DataGridView dgvLichSu;
    private DataGridViewTextBoxColumn colMaLop;
    private DataGridViewTextBoxColumn colTenLop;
    private DataGridViewTextBoxColumn colTenKhoaHoc;
    private DataGridViewTextBoxColumn colNgayDangKy;
    private DataGridViewTextBoxColumn colTrangThai;
    private DataGridViewTextBoxColumn colHocPhiPhaiThu;
    private DataGridViewTextBoxColumn colDaThu;
    private DataGridViewTextBoxColumn colConNo;

    private Button btnClose;
}
