namespace QLHV.WinForms.Forms;

partial class FrmGiangVienEdit
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
        lblMaGiangVien = new Label();
        txtMaGiangVien = new TextBox();
        lblHoTen = new Label();
        txtHoTen = new TextBox();
        lblChuyenMon = new Label();
        txtChuyenMon = new TextBox();
        lblDienThoai = new Label();
        txtDienThoai = new TextBox();
        lblEmail = new Label();
        txtEmail = new TextBox();
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
        lblTitle.Size = new Size(227, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "THÔNG TIN GIẢNG VIÊN";
        //
        // lblMaGiangVien
        //
        lblMaGiangVien.AutoSize = true;
        lblMaGiangVien.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblMaGiangVien.Location = new Point(25, 65);
        lblMaGiangVien.Name = "lblMaGiangVien";
        lblMaGiangVien.Size = new Size(125, 17);
        lblMaGiangVien.TabIndex = 1;
        lblMaGiangVien.Text = "Mã giảng viên (*):";
        //
        // txtMaGiangVien
        //
        txtMaGiangVien.Location = new Point(25, 88);
        txtMaGiangVien.Name = "txtMaGiangVien";
        txtMaGiangVien.Size = new Size(220, 23);
        txtMaGiangVien.TabIndex = 2;
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
        // lblChuyenMon
        //
        lblChuyenMon.AutoSize = true;
        lblChuyenMon.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblChuyenMon.Location = new Point(25, 125);
        lblChuyenMon.Name = "lblChuyenMon";
        lblChuyenMon.Size = new Size(83, 17);
        lblChuyenMon.TabIndex = 5;
        lblChuyenMon.Text = "Chuyên môn:";
        //
        // txtChuyenMon
        //
        txtChuyenMon.Location = new Point(25, 148);
        txtChuyenMon.Name = "txtChuyenMon";
        txtChuyenMon.Size = new Size(490, 23);
        txtChuyenMon.TabIndex = 6;
        //
        // lblDienThoai
        //
        lblDienThoai.AutoSize = true;
        lblDienThoai.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblDienThoai.Location = new Point(25, 185);
        lblDienThoai.Name = "lblDienThoai";
        lblDienThoai.Size = new Size(70, 17);
        lblDienThoai.TabIndex = 7;
        lblDienThoai.Text = "Điện thoại:";
        //
        // txtDienThoai
        //
        txtDienThoai.Location = new Point(25, 208);
        txtDienThoai.Name = "txtDienThoai";
        txtDienThoai.Size = new Size(220, 23);
        txtDienThoai.TabIndex = 8;
        //
        // lblEmail
        //
        lblEmail.AutoSize = true;
        lblEmail.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblEmail.Location = new Point(265, 185);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(42, 17);
        lblEmail.TabIndex = 9;
        lblEmail.Text = "Email:";
        //
        // txtEmail
        //
        txtEmail.Location = new Point(265, 208);
        txtEmail.Name = "txtEmail";
        txtEmail.Size = new Size(250, 23);
        txtEmail.TabIndex = 10;
        //
        // lblTrangThai
        //
        lblTrangThai.AutoSize = true;
        lblTrangThai.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblTrangThai.Location = new Point(25, 245);
        lblTrangThai.Name = "lblTrangThai";
        lblTrangThai.Size = new Size(69, 17);
        lblTrangThai.TabIndex = 11;
        lblTrangThai.Text = "Trạng thái:";
        //
        // cboTrangThai
        //
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.FormattingEnabled = true;
        cboTrangThai.Location = new Point(25, 268);
        cboTrangThai.Name = "cboTrangThai";
        cboTrangThai.Size = new Size(220, 23);
        cboTrangThai.TabIndex = 12;
        //
        // btnSave
        //
        btnSave.BackColor = Color.FromArgb(30, 64, 175);
        btnSave.Cursor = Cursors.Hand;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(295, 320);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(105, 35);
        btnSave.TabIndex = 13;
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
        btnCancel.Location = new Point(410, 320);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(105, 35);
        btnCancel.TabIndex = 14;
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
        lblError.Location = new Point(25, 298);
        lblError.Name = "lblError";
        lblError.Size = new Size(490, 20);
        lblError.TabIndex = 15;
        //
        // FrmGiangVienEdit
        //
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnCancel;
        ClientSize = new Size(545, 375);
        Controls.Add(lblError);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(cboTrangThai);
        Controls.Add(lblTrangThai);
        Controls.Add(txtEmail);
        Controls.Add(lblEmail);
        Controls.Add(txtDienThoai);
        Controls.Add(lblDienThoai);
        Controls.Add(txtChuyenMon);
        Controls.Add(lblChuyenMon);
        Controls.Add(txtHoTen);
        Controls.Add(lblHoTen);
        Controls.Add(txtMaGiangVien);
        Controls.Add(lblMaGiangVien);
        Controls.Add(lblTitle);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmGiangVienEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thông tin giảng viên";
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblMaGiangVien;
    private TextBox txtMaGiangVien;
    private Label lblHoTen;
    private TextBox txtHoTen;
    private Label lblChuyenMon;
    private TextBox txtChuyenMon;
    private Label lblDienThoai;
    private TextBox txtDienThoai;
    private Label lblEmail;
    private TextBox txtEmail;
    private Label lblTrangThai;
    private ComboBox cboTrangThai;
    private Button btnSave;
    private Button btnCancel;
    private ErrorProvider errorProvider;
    private Label lblError;
}
