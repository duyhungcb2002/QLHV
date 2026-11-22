namespace QLHV.WinForms.Forms;

partial class FrmHocVienEdit
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
        components = new System.ComponentModel.Container();
        lblTitle = new Label();
        tabControl = new TabControl();
        tabCoBan = new TabPage();
        tabDinhDanh = new TabPage();
        tabGiamHo = new TabPage();
        tabTuyenSinh = new TabPage();
        tabGhiChu = new TabPage();

        // Cơ bản
        lblMaHocVien = new Label();
        txtMaHocVien = new TextBox();
        lblHoTen = new Label();
        txtHoTen = new TextBox();
        lblNgaySinh = new Label();
        dtpNgaySinh = new DateTimePicker();
        lblGioiTinh = new Label();
        cboGioiTinh = new ComboBox();
        lblDienThoai = new Label();
        txtDienThoai = new TextBox();
        lblEmail = new Label();
        txtEmail = new TextBox();
        lblDiaChi = new Label();
        txtDiaChi = new TextBox();
        lblTrangThai = new Label();
        cboTrangThai = new ComboBox();

        // Định danh
        lblSoDinhDanh = new Label();
        txtSoDinhDanh = new TextBox();
        lblNgayCapGiayTo = new Label();
        dtpNgayCapGiayTo = new DateTimePicker();
        lblNoiCapGiayTo = new Label();
        txtNoiCapGiayTo = new TextBox();

        // Giám hộ
        lblHoTenNguoiGiamHo = new Label();
        txtHoTenNguoiGiamHo = new TextBox();
        lblDienThoaiNguoiGiamHo = new Label();
        txtDienThoaiNguoiGiamHo = new TextBox();
        lblQuanHeNguoiGiamHo = new Label();
        txtQuanHeNguoiGiamHo = new TextBox();

        // Tuyển sinh & Việc làm
        lblNhanVienPhuTrach = new Label();
        cboNhanVienPhuTrach = new ComboBox();
        lblNguonTuyenSinh = new Label();
        cboNguonTuyenSinh = new ComboBox();
        lblNgheNghiep = new Label();
        txtNgheNghiep = new TextBox();
        lblDonViCongTac = new Label();
        txtDonViCongTac = new TextBox();

        // Ghi chú
        lblGhiChuDacBiet = new Label();
        txtGhiChuDacBiet = new TextBox();
        lblCharCounter = new Label();

        // Buttons & Error
        btnSave = new Button();
        btnCancel = new Button();
        errorProvider = new ErrorProvider(components);
        lblError = new Label();

        ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
        tabControl.SuspendLayout();
        tabCoBan.SuspendLayout();
        tabDinhDanh.SuspendLayout();
        tabGiamHo.SuspendLayout();
        tabTuyenSinh.SuspendLayout();
        tabGhiChu.SuspendLayout();
        SuspendLayout();

        // lblTitle
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 64, 175);
        lblTitle.Location = new Point(25, 15);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(204, 25);
        lblTitle.Text = "THÔNG TIN HỌC VIÊN";

        // tabControl
        tabControl.Controls.Add(tabCoBan);
        tabControl.Controls.Add(tabDinhDanh);
        tabControl.Controls.Add(tabGiamHo);
        tabControl.Controls.Add(tabTuyenSinh);
        tabControl.Controls.Add(tabGhiChu);
        tabControl.Font = new Font("Segoe UI", 9.5F);
        tabControl.Location = new Point(25, 50);
        tabControl.Name = "tabControl";
        tabControl.SelectedIndex = 0;
        tabControl.Size = new Size(580, 310);

        // ================= TAB CƠ BẢN =================
        tabCoBan.Text = "1. Thông tin cơ bản";
        tabCoBan.UseVisualStyleBackColor = true;

        lblMaHocVien.AutoSize = true;
        lblMaHocVien.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblMaHocVien.Location = new Point(20, 20);
        lblMaHocVien.Text = "Mã học viên (*):";

        txtMaHocVien.Location = new Point(20, 42);
        txtMaHocVien.MaxLength = 20;
        txtMaHocVien.Size = new Size(230, 24);

        lblHoTen.AutoSize = true;
        lblHoTen.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblHoTen.Location = new Point(280, 20);
        lblHoTen.Text = "Họ và tên (*):";

        txtHoTen.Location = new Point(280, 42);
        txtHoTen.MaxLength = 100;
        txtHoTen.Size = new Size(260, 24);

        lblNgaySinh.AutoSize = true;
        lblNgaySinh.Location = new Point(20, 80);
        lblNgaySinh.Text = "Ngày sinh:";

        dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
        dtpNgaySinh.Format = DateTimePickerFormat.Custom;
        dtpNgaySinh.Location = new Point(20, 102);
        dtpNgaySinh.ShowCheckBox = true;
        dtpNgaySinh.Size = new Size(230, 24);

        lblGioiTinh.AutoSize = true;
        lblGioiTinh.Location = new Point(280, 80);
        lblGioiTinh.Text = "Giới tính:";

        cboGioiTinh.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
        cboGioiTinh.Location = new Point(280, 102);
        cboGioiTinh.Size = new Size(260, 24);

        lblDienThoai.AutoSize = true;
        lblDienThoai.Location = new Point(20, 140);
        lblDienThoai.Text = "Điện thoại:";

        txtDienThoai.Location = new Point(20, 162);
        txtDienThoai.MaxLength = 20;
        txtDienThoai.Size = new Size(230, 24);

        lblEmail.AutoSize = true;
        lblEmail.Location = new Point(280, 140);
        lblEmail.Text = "Email:";

        txtEmail.Location = new Point(280, 162);
        txtEmail.MaxLength = 150;
        txtEmail.Size = new Size(260, 24);

        lblDiaChi.AutoSize = true;
        lblDiaChi.Location = new Point(20, 200);
        lblDiaChi.Text = "Địa chỉ:";

        txtDiaChi.Location = new Point(20, 222);
        txtDiaChi.MaxLength = 250;
        txtDiaChi.Size = new Size(230, 24);

        lblTrangThai.AutoSize = true;
        lblTrangThai.Location = new Point(280, 200);
        lblTrangThai.Text = "Trạng thái:";

        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Location = new Point(280, 222);
        cboTrangThai.Size = new Size(260, 24);

        tabCoBan.Controls.AddRange(new Control[] {
            lblMaHocVien, txtMaHocVien, lblHoTen, txtHoTen,
            lblNgaySinh, dtpNgaySinh, lblGioiTinh, cboGioiTinh,
            lblDienThoai, txtDienThoai, lblEmail, txtEmail,
            lblDiaChi, txtDiaChi, lblTrangThai, cboTrangThai
        });

        // ================= TAB ĐỊNH DANH =================
        tabDinhDanh.Text = "2. Định danh";
        tabDinhDanh.UseVisualStyleBackColor = true;

        lblSoDinhDanh.AutoSize = true;
        lblSoDinhDanh.Location = new Point(20, 25);
        lblSoDinhDanh.Text = "Số CCCD / CMND (9 hoặc 12 số):";

        txtSoDinhDanh.Location = new Point(20, 50);
        txtSoDinhDanh.MaxLength = 20;
        txtSoDinhDanh.Size = new Size(520, 24);

        lblNgayCapGiayTo.AutoSize = true;
        lblNgayCapGiayTo.Location = new Point(20, 95);
        lblNgayCapGiayTo.Text = "Ngày cấp:";

        dtpNgayCapGiayTo.CustomFormat = "dd/MM/yyyy";
        dtpNgayCapGiayTo.Format = DateTimePickerFormat.Custom;
        dtpNgayCapGiayTo.Location = new Point(20, 120);
        dtpNgayCapGiayTo.ShowCheckBox = true;
        dtpNgayCapGiayTo.Size = new Size(520, 24);

        lblNoiCapGiayTo.AutoSize = true;
        lblNoiCapGiayTo.Location = new Point(20, 165);
        lblNoiCapGiayTo.Text = "Nơi cấp (ví dụ: Cục Cảnh sát QLHC về TTXH):";

        txtNoiCapGiayTo.Location = new Point(20, 190);
        txtNoiCapGiayTo.MaxLength = 150;
        txtNoiCapGiayTo.Size = new Size(520, 24);

        tabDinhDanh.Controls.AddRange(new Control[] {
            lblSoDinhDanh, txtSoDinhDanh, lblNgayCapGiayTo, dtpNgayCapGiayTo, lblNoiCapGiayTo, txtNoiCapGiayTo
        });

        // ================= TAB GIÁM HỘ =================
        tabGiamHo.Text = "3. Người giám hộ";
        tabGiamHo.UseVisualStyleBackColor = true;

        lblHoTenNguoiGiamHo.AutoSize = true;
        lblHoTenNguoiGiamHo.Location = new Point(20, 25);
        lblHoTenNguoiGiamHo.Text = "Họ và tên người giám hộ (bắt buộc nếu học viên < 18 tuổi):";

        txtHoTenNguoiGiamHo.Location = new Point(20, 50);
        txtHoTenNguoiGiamHo.MaxLength = 100;
        txtHoTenNguoiGiamHo.Size = new Size(520, 24);

        lblDienThoaiNguoiGiamHo.AutoSize = true;
        lblDienThoaiNguoiGiamHo.Location = new Point(20, 95);
        lblDienThoaiNguoiGiamHo.Text = "Điện thoại người giám hộ:";

        txtDienThoaiNguoiGiamHo.Location = new Point(20, 120);
        txtDienThoaiNguoiGiamHo.MaxLength = 20;
        txtDienThoaiNguoiGiamHo.Size = new Size(520, 24);

        lblQuanHeNguoiGiamHo.AutoSize = true;
        lblQuanHeNguoiGiamHo.Location = new Point(20, 165);
        lblQuanHeNguoiGiamHo.Text = "Quan hệ với học viên (ví dụ: Bố, Mẹ, Anh/Chị, Người đỡ đầu):";

        txtQuanHeNguoiGiamHo.Location = new Point(20, 190);
        txtQuanHeNguoiGiamHo.MaxLength = 50;
        txtQuanHeNguoiGiamHo.Size = new Size(520, 24);

        tabGiamHo.Controls.AddRange(new Control[] {
            lblHoTenNguoiGiamHo, txtHoTenNguoiGiamHo, lblDienThoaiNguoiGiamHo, txtDienThoaiNguoiGiamHo, lblQuanHeNguoiGiamHo, txtQuanHeNguoiGiamHo
        });

        // ================= TAB TUYỂN SINH & VIỆC LÀM =================
        tabTuyenSinh.Text = "4. Tuyển sinh & Việc làm";
        tabTuyenSinh.UseVisualStyleBackColor = true;

        lblNhanVienPhuTrach.AutoSize = true;
        lblNhanVienPhuTrach.Location = new Point(20, 20);
        lblNhanVienPhuTrach.Text = "Nhân viên phụ trách:";

        cboNhanVienPhuTrach.DropDownStyle = ComboBoxStyle.DropDownList;
        cboNhanVienPhuTrach.Location = new Point(20, 42);
        cboNhanVienPhuTrach.Size = new Size(520, 24);

        lblNguonTuyenSinh.AutoSize = true;
        lblNguonTuyenSinh.Location = new Point(20, 80);
        lblNguonTuyenSinh.Text = "Nguồn tuyển sinh:";

        cboNguonTuyenSinh.DropDownStyle = ComboBoxStyle.DropDownList;
        cboNguonTuyenSinh.Location = new Point(20, 102);
        cboNguonTuyenSinh.Size = new Size(520, 24);

        lblNgheNghiep.AutoSize = true;
        lblNgheNghiep.Location = new Point(20, 140);
        lblNgheNghiep.Text = "Nghề nghiệp:";

        txtNgheNghiep.Location = new Point(20, 162);
        txtNgheNghiep.MaxLength = 100;
        txtNgheNghiep.Size = new Size(520, 24);

        lblDonViCongTac.AutoSize = true;
        lblDonViCongTac.Location = new Point(20, 200);
        lblDonViCongTac.Text = "Đơn vị công tác / Trường học:";

        txtDonViCongTac.Location = new Point(20, 222);
        txtDonViCongTac.MaxLength = 200;
        txtDonViCongTac.Name = "txtDonViCongTac";
        txtDonViCongTac.Size = new Size(520, 24);

        tabTuyenSinh.Controls.AddRange(new Control[] {
            lblNhanVienPhuTrach, cboNhanVienPhuTrach, lblNguonTuyenSinh, cboNguonTuyenSinh,
            lblNgheNghiep, txtNgheNghiep, lblDonViCongTac, txtDonViCongTac
        });

        // ================= TAB GHI CHÚ =================
        tabGhiChu.Text = "5. Ghi chú đặc biệt";
        tabGhiChu.UseVisualStyleBackColor = true;

        lblGhiChuDacBiet.AutoSize = true;
        lblGhiChuDacBiet.Location = new Point(20, 15);
        lblGhiChuDacBiet.Text = "Ghi chú đặc biệt / Yêu cầu riêng của học viên (tối đa 1000 ký tự):";

        txtGhiChuDacBiet.Location = new Point(20, 40);
        txtGhiChuDacBiet.MaxLength = 1000;
        txtGhiChuDacBiet.Multiline = true;
        txtGhiChuDacBiet.ScrollBars = ScrollBars.Vertical;
        txtGhiChuDacBiet.Size = new Size(520, 190);

        lblCharCounter.AutoSize = true;
        lblCharCounter.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblCharCounter.ForeColor = Color.Gray;
        lblCharCounter.Location = new Point(20, 240);
        lblCharCounter.Text = "0 / 1000 ký tự";

        tabGhiChu.Controls.AddRange(new Control[] {
            lblGhiChuDacBiet, txtGhiChuDacBiet, lblCharCounter
        });

        // ================= BOTTOM BUTTONS & ERROR =================
        lblError.AutoSize = true;
        lblError.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblError.ForeColor = Color.FromArgb(220, 38, 38);
        lblError.Location = new Point(25, 375);
        lblError.Size = new Size(400, 15);
        lblError.Text = "";

        btnSave.BackColor = Color.FromArgb(37, 99, 235);
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(395, 400);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 35);
        btnSave.Text = "Lưu lại";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += btnSave_Click;

        btnCancel.BackColor = Color.FromArgb(243, 244, 246);
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.Font = new Font("Segoe UI", 9.5F);
        btnCancel.ForeColor = Color.FromArgb(55, 65, 81);
        btnCancel.Location = new Point(505, 400);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 35);
        btnCancel.Text = "Hủy bỏ";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Click += btnCancel_Click;

        // FrmHocVienEdit
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnCancel;
        ClientSize = new Size(635, 455);
        Controls.Add(lblTitle);
        Controls.Add(tabControl);
        Controls.Add(lblError);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thông tin học viên";

        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        tabControl.ResumeLayout(false);
        tabCoBan.ResumeLayout(false);
        tabCoBan.PerformLayout();
        tabDinhDanh.ResumeLayout(false);
        tabDinhDanh.PerformLayout();
        tabGiamHo.ResumeLayout(false);
        tabGiamHo.PerformLayout();
        tabTuyenSinh.ResumeLayout(false);
        tabTuyenSinh.PerformLayout();
        tabGhiChu.ResumeLayout(false);
        tabGhiChu.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private TabControl tabControl;
    private TabPage tabCoBan;
    private TabPage tabDinhDanh;
    private TabPage tabGiamHo;
    private TabPage tabTuyenSinh;
    private TabPage tabGhiChu;

    private Label lblMaHocVien;
    private TextBox txtMaHocVien;
    private Label lblHoTen;
    private TextBox txtHoTen;
    private Label lblNgaySinh;
    private DateTimePicker dtpNgaySinh;
    private Label lblGioiTinh;
    private ComboBox cboGioiTinh;
    private Label lblDienThoai;
    private TextBox txtDienThoai;
    private Label lblEmail;
    private TextBox txtEmail;
    private Label lblDiaChi;
    private TextBox txtDiaChi;
    private Label lblTrangThai;
    private ComboBox cboTrangThai;

    private Label lblSoDinhDanh;
    private TextBox txtSoDinhDanh;
    private Label lblNgayCapGiayTo;
    private DateTimePicker dtpNgayCapGiayTo;
    private Label lblNoiCapGiayTo;
    private TextBox txtNoiCapGiayTo;

    private Label lblHoTenNguoiGiamHo;
    private TextBox txtHoTenNguoiGiamHo;
    private Label lblDienThoaiNguoiGiamHo;
    private TextBox txtDienThoaiNguoiGiamHo;
    private Label lblQuanHeNguoiGiamHo;
    private TextBox txtQuanHeNguoiGiamHo;

    private Label lblNhanVienPhuTrach;
    private ComboBox cboNhanVienPhuTrach;
    private Label lblNguonTuyenSinh;
    private ComboBox cboNguonTuyenSinh;
    private Label lblNgheNghiep;
    private TextBox txtNgheNghiep;
    private Label lblDonViCongTac;
    private TextBox txtDonViCongTac;

    private Label lblGhiChuDacBiet;
    private TextBox txtGhiChuDacBiet;
    private Label lblCharCounter;

    private Button btnSave;
    private Button btnCancel;
    private ErrorProvider errorProvider;
    private Label lblError;
}
