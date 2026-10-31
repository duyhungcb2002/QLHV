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
        btnSave = new Button();
        btnCancel = new Button();
        errorProvider = new ErrorProvider(components);
        lblError = new Label();
        ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.ForeColor = Color.FromArgb(30, 64, 175);
        lblTitle.Location = new Point(25, 20);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(204, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "THÔNG TIN HỌC VIÊN";
        // 
        // lblMaHocVien
        // 
        lblMaHocVien.AutoSize = true;
        lblMaHocVien.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblMaHocVien.Location = new Point(25, 65);
        lblMaHocVien.Name = "lblMaHocVien";
        lblMaHocVien.Size = new Size(111, 17);
        lblMaHocVien.TabIndex = 1;
        lblMaHocVien.Text = "Mã học viên (*):";
        // 
        // txtMaHocVien
        // 
        txtMaHocVien.Location = new Point(25, 88);
        txtMaHocVien.Name = "txtMaHocVien";
        txtMaHocVien.Size = new Size(220, 23);
        txtMaHocVien.TabIndex = 2;
        // 
        // lblHoTen
        // 
        lblHoTen.AutoSize = true;
        lblHoTen.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblHoTen.Location = new Point(265, 65);
        lblHoTen.Name = "lblHoTen";
        lblHoTen.Size = new Size(95, 17);
        lblHoTen.TabIndex = 3;
        lblHoTen.Text = "Họ và tên (*):";
        // 
        // txtHoTen
        // 
        txtHoTen.Location = new Point(265, 88);
        txtHoTen.Name = "txtHoTen";
        txtHoTen.Size = new Size(250, 23);
        txtHoTen.TabIndex = 4;
        // 
        // lblNgaySinh
        // 
        lblNgaySinh.AutoSize = true;
        lblNgaySinh.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblNgaySinh.Location = new Point(25, 125);
        lblNgaySinh.Name = "lblNgaySinh";
        lblNgaySinh.Size = new Size(69, 17);
        lblNgaySinh.TabIndex = 5;
        lblNgaySinh.Text = "Ngày sinh:";
        // 
        // dtpNgaySinh
        // 
        dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
        dtpNgaySinh.Format = DateTimePickerFormat.Custom;
        dtpNgaySinh.Location = new Point(25, 148);
        dtpNgaySinh.Name = "dtpNgaySinh";
        dtpNgaySinh.ShowCheckBox = true;
        dtpNgaySinh.Size = new Size(220, 23);
        dtpNgaySinh.TabIndex = 6;
        // 
        // lblGioiTinh
        // 
        lblGioiTinh.AutoSize = true;
        lblGioiTinh.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblGioiTinh.Location = new Point(265, 125);
        lblGioiTinh.Name = "lblGioiTinh";
        lblGioiTinh.Size = new Size(59, 17);
        lblGioiTinh.TabIndex = 7;
        lblGioiTinh.Text = "Giới tính:";
        // 
        // cboGioiTinh
        // 
        cboGioiTinh.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGioiTinh.FormattingEnabled = true;
        cboGioiTinh.Items.AddRange(new object[] { "Nam", "Nữ", "Khác" });
        cboGioiTinh.Location = new Point(265, 148);
        cboGioiTinh.Name = "cboGioiTinh";
        cboGioiTinh.Size = new Size(250, 23);
        cboGioiTinh.TabIndex = 8;
        // 
        // lblDienThoai
        // 
        lblDienThoai.AutoSize = true;
        lblDienThoai.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblDienThoai.Location = new Point(25, 185);
        lblDienThoai.Name = "lblDienThoai";
        lblDienThoai.Size = new Size(70, 17);
        lblDienThoai.TabIndex = 9;
        lblDienThoai.Text = "Điện thoại:";
        // 
        // txtDienThoai
        // 
        txtDienThoai.Location = new Point(25, 208);
        txtDienThoai.Name = "txtDienThoai";
        txtDienThoai.Size = new Size(220, 23);
        txtDienThoai.TabIndex = 10;
        // 
        // lblEmail
        // 
        lblEmail.AutoSize = true;
        lblEmail.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblEmail.Location = new Point(265, 185);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(42, 17);
        lblEmail.TabIndex = 11;
        lblEmail.Text = "Email:";
        // 
        // txtEmail
        // 
        txtEmail.Location = new Point(265, 208);
        txtEmail.Name = "txtEmail";
        txtEmail.Size = new Size(250, 23);
        txtEmail.TabIndex = 12;
        // 
        // lblDiaChi
        // 
        lblDiaChi.AutoSize = true;
        lblDiaChi.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblDiaChi.Location = new Point(25, 245);
        lblDiaChi.Name = "lblDiaChi";
        lblDiaChi.Size = new Size(50, 17);
        lblDiaChi.TabIndex = 13;
        lblDiaChi.Text = "Địa chỉ:";
        // 
        // txtDiaChi
        // 
        txtDiaChi.Location = new Point(25, 268);
        txtDiaChi.Name = "txtDiaChi";
        txtDiaChi.Size = new Size(490, 23);
        txtDiaChi.TabIndex = 14;
        // 
        // lblTrangThai
        // 
        lblTrangThai.AutoSize = true;
        lblTrangThai.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblTrangThai.Location = new Point(25, 305);
        lblTrangThai.Name = "lblTrangThai";
        lblTrangThai.Size = new Size(69, 17);
        lblTrangThai.TabIndex = 15;
        lblTrangThai.Text = "Trạng thái:";
        // 
        // cboTrangThai
        // 
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.FormattingEnabled = true;
        cboTrangThai.Location = new Point(25, 328);
        cboTrangThai.Name = "cboTrangThai";
        cboTrangThai.Size = new Size(220, 23);
        cboTrangThai.TabIndex = 16;
        // 
        // btnSave
        // 
        btnSave.BackColor = Color.FromArgb(30, 64, 175);
        btnSave.Cursor = Cursors.Hand;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(295, 385);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(105, 35);
        btnSave.TabIndex = 17;
        btnSave.Text = "Lưu lại";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.BackColor = Color.FromArgb(229, 231, 235);
        btnCancel.Cursor = Cursors.Hand;
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnCancel.ForeColor = Color.FromArgb(55, 65, 81);
        btnCancel.Location = new Point(410, 385);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(105, 35);
        btnCancel.TabIndex = 18;
        btnCancel.Text = "Hủy bỏ";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Click += btnCancel_Click;
        // 
        // errorProvider
        // 
        errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        errorProvider.ContainerControl = this;
        // 
        // lblError
        // 
        lblError.ForeColor = Color.FromArgb(220, 38, 38);
        lblError.Location = new Point(25, 360);
        lblError.Name = "lblError";
        lblError.Size = new Size(490, 20);
        lblError.TabIndex = 19;
        // 
        // FrmHocVienEdit
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnCancel;
        ClientSize = new Size(545, 435);
        Controls.Add(lblError);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(cboTrangThai);
        Controls.Add(lblTrangThai);
        Controls.Add(txtDiaChi);
        Controls.Add(lblDiaChi);
        Controls.Add(txtEmail);
        Controls.Add(lblEmail);
        Controls.Add(txtDienThoai);
        Controls.Add(lblDienThoai);
        Controls.Add(cboGioiTinh);
        Controls.Add(lblGioiTinh);
        Controls.Add(dtpNgaySinh);
        Controls.Add(lblNgaySinh);
        Controls.Add(txtHoTen);
        Controls.Add(lblHoTen);
        Controls.Add(txtMaHocVien);
        Controls.Add(lblMaHocVien);
        Controls.Add(lblTitle);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmHocVienEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thông tin học viên";
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
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
    private Button btnSave;
    private Button btnCancel;
    private ErrorProvider errorProvider;
    private Label lblError;
}
