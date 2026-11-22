namespace QLHV.WinForms.Forms;

partial class FrmLopHocEdit
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
        lblMaLop = new Label();
        txtMaLop = new TextBox();
        lblTenLop = new Label();
        txtTenLop = new TextBox();
        lblKhoaHoc = new Label();
        cboKhoaHoc = new ComboBox();
        lblGiangVien = new Label();
        cboGiangVienChuNhiem = new ComboBox();
        lblNgayBatDau = new Label();
        dtpNgayBatDau = new DateTimePicker();
        chkCoNgayKetThuc = new CheckBox();
        dtpNgayKetThuc = new DateTimePicker();
        lblSiSo = new Label();
        nudSiSoToiDa = new NumericUpDown();
        lblGhiChu = new Label();
        txtGhiChu = new TextBox();
        chkDangHoatDong = new CheckBox();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudSiSoToiDa).BeginInit();
        SuspendLayout();
        //
        // lblMaLop
        //
        lblMaLop.AutoSize = true;
        lblMaLop.Location = new Point(25, 20);
        lblMaLop.Name = "lblMaLop";
        lblMaLop.Size = new Size(82, 17);
        lblMaLop.TabIndex = 0;
        lblMaLop.Text = "Mã lớp học *:";
        //
        // txtMaLop
        //
        txtMaLop.Location = new Point(145, 17);
        txtMaLop.MaxLength = 20;
        txtMaLop.Name = "txtMaLop";
        txtMaLop.Size = new Size(295, 25);
        txtMaLop.TabIndex = 1;
        //
        // lblTenLop
        //
        lblTenLop.AutoSize = true;
        lblTenLop.Location = new Point(25, 58);
        lblTenLop.Name = "lblTenLop";
        lblTenLop.Size = new Size(83, 17);
        lblTenLop.TabIndex = 2;
        lblTenLop.Text = "Tên lớp học *:";
        //
        // txtTenLop
        //
        txtTenLop.Location = new Point(145, 55);
        txtTenLop.MaxLength = 150;
        txtTenLop.Name = "txtTenLop";
        txtTenLop.Size = new Size(295, 25);
        txtTenLop.TabIndex = 3;
        //
        // lblKhoaHoc
        //
        lblKhoaHoc.AutoSize = true;
        lblKhoaHoc.Location = new Point(25, 96);
        lblKhoaHoc.Name = "lblKhoaHoc";
        lblKhoaHoc.Size = new Size(74, 17);
        lblKhoaHoc.TabIndex = 4;
        lblKhoaHoc.Text = "Khóa học *:";
        //
        // cboKhoaHoc
        //
        cboKhoaHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboKhoaHoc.FormattingEnabled = true;
        cboKhoaHoc.Location = new Point(145, 93);
        cboKhoaHoc.Name = "cboKhoaHoc";
        cboKhoaHoc.Size = new Size(295, 25);
        cboKhoaHoc.TabIndex = 5;
        //
        // lblGiangVien
        //
        lblGiangVien.AutoSize = true;
        lblGiangVien.Location = new Point(25, 134);
        lblGiangVien.Name = "lblGiangVien";
        lblGiangVien.Size = new Size(115, 17);
        lblGiangVien.TabIndex = 6;
        lblGiangVien.Text = "GV chủ nhiệm *:";
        //
        // cboGiangVienChuNhiem
        //
        cboGiangVienChuNhiem.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGiangVienChuNhiem.FormattingEnabled = true;
        cboGiangVienChuNhiem.Location = new Point(145, 131);
        cboGiangVienChuNhiem.Name = "cboGiangVienChuNhiem";
        cboGiangVienChuNhiem.Size = new Size(295, 25);
        cboGiangVienChuNhiem.TabIndex = 7;
        //
        // lblNgayBatDau
        //
        lblNgayBatDau.AutoSize = true;
        lblNgayBatDau.Location = new Point(25, 172);
        lblNgayBatDau.Name = "lblNgayBatDau";
        lblNgayBatDau.Size = new Size(98, 17);
        lblNgayBatDau.TabIndex = 8;
        lblNgayBatDau.Text = "Ngày bắt đầu *:";
        //
        // dtpNgayBatDau
        //
        dtpNgayBatDau.Format = DateTimePickerFormat.Short;
        dtpNgayBatDau.Location = new Point(145, 169);
        dtpNgayBatDau.Name = "dtpNgayBatDau";
        dtpNgayBatDau.Size = new Size(130, 25);
        dtpNgayBatDau.TabIndex = 9;
        //
        // chkCoNgayKetThuc
        //
        chkCoNgayKetThuc.AutoSize = true;
        chkCoNgayKetThuc.Location = new Point(25, 210);
        chkCoNgayKetThuc.Name = "chkCoNgayKetThuc";
        chkCoNgayKetThuc.Size = new Size(103, 21);
        chkCoNgayKetThuc.TabIndex = 10;
        chkCoNgayKetThuc.Text = "Ngày kết thúc:";
        chkCoNgayKetThuc.UseVisualStyleBackColor = true;
        chkCoNgayKetThuc.CheckedChanged += chkCoNgayKetThuc_CheckedChanged;
        //
        // dtpNgayKetThuc
        //
        dtpNgayKetThuc.Format = DateTimePickerFormat.Short;
        dtpNgayKetThuc.Location = new Point(145, 208);
        dtpNgayKetThuc.Name = "dtpNgayKetThuc";
        dtpNgayKetThuc.Size = new Size(130, 25);
        dtpNgayKetThuc.TabIndex = 11;
        //
        // lblSiSo
        //
        lblSiSo.AutoSize = true;
        lblSiSo.Location = new Point(25, 248);
        lblSiSo.Name = "lblSiSo";
        lblSiSo.Size = new Size(88, 17);
        lblSiSo.TabIndex = 12;
        lblSiSo.Text = "Sĩ số tối đa *:";
        //
        // nudSiSoToiDa
        //
        nudSiSoToiDa.Location = new Point(145, 246);
        nudSiSoToiDa.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
        nudSiSoToiDa.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudSiSoToiDa.Name = "nudSiSoToiDa";
        nudSiSoToiDa.Size = new Size(100, 25);
        nudSiSoToiDa.TabIndex = 13;
        nudSiSoToiDa.Value = new decimal(new int[] { 30, 0, 0, 0 });
        //
        // lblGhiChu
        //
        lblGhiChu.AutoSize = true;
        lblGhiChu.Location = new Point(25, 286);
        lblGhiChu.Name = "lblGhiChu";
        lblGhiChu.Size = new Size(54, 17);
        lblGhiChu.TabIndex = 14;
        lblGhiChu.Text = "Ghi chú:";
        //
        // txtGhiChu
        //
        txtGhiChu.Location = new Point(145, 283);
        txtGhiChu.MaxLength = 500;
        txtGhiChu.Multiline = true;
        txtGhiChu.Name = "txtGhiChu";
        txtGhiChu.Size = new Size(295, 55);
        txtGhiChu.TabIndex = 15;
        //
        // chkDangHoatDong
        //
        chkDangHoatDong.AutoSize = true;
        chkDangHoatDong.Checked = true;
        chkDangHoatDong.CheckState = CheckState.Checked;
        chkDangHoatDong.Location = new Point(145, 348);
        chkDangHoatDong.Name = "chkDangHoatDong";
        chkDangHoatDong.Size = new Size(122, 21);
        chkDangHoatDong.TabIndex = 16;
        chkDangHoatDong.Text = "Đang hoạt động";
        chkDangHoatDong.UseVisualStyleBackColor = true;
        //
        // btnSave
        //
        btnSave.BackColor = Color.FromArgb(16, 185, 129);
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(244, 385);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(95, 32);
        btnSave.TabIndex = 17;
        btnSave.Text = "Lưu";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += btnSave_Click;
        //
        // btnCancel
        //
        btnCancel.BackColor = Color.FromArgb(156, 163, 175);
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.ForeColor = Color.White;
        btnCancel.Location = new Point(345, 385);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(95, 32);
        btnCancel.TabIndex = 18;
        btnCancel.Text = "Hủy";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Click += btnCancel_Click;
        //
        // FrmLopHocEdit
        //
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 435);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(chkDangHoatDong);
        Controls.Add(txtGhiChu);
        Controls.Add(lblGhiChu);
        Controls.Add(nudSiSoToiDa);
        Controls.Add(lblSiSo);
        Controls.Add(dtpNgayKetThuc);
        Controls.Add(chkCoNgayKetThuc);
        Controls.Add(dtpNgayBatDau);
        Controls.Add(lblNgayBatDau);
        Controls.Add(cboGiangVienChuNhiem);
        Controls.Add(lblGiangVien);
        Controls.Add(cboKhoaHoc);
        Controls.Add(lblKhoaHoc);
        Controls.Add(txtTenLop);
        Controls.Add(lblTenLop);
        Controls.Add(txtMaLop);
        Controls.Add(lblMaLop);
        Font = new Font("Segoe UI", 9.75F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmLopHocEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thông tin Lớp học";
        ((System.ComponentModel.ISupportInitialize)nudSiSoToiDa).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblMaLop;
    private TextBox txtMaLop;
    private Label lblTenLop;
    private TextBox txtTenLop;
    private Label lblKhoaHoc;
    private ComboBox cboKhoaHoc;
    private Label lblGiangVien;
    private ComboBox cboGiangVienChuNhiem;
    private Label lblNgayBatDau;
    private DateTimePicker dtpNgayBatDau;
    private CheckBox chkCoNgayKetThuc;
    private DateTimePicker dtpNgayKetThuc;
    private Label lblSiSo;
    private NumericUpDown nudSiSoToiDa;
    private Label lblGhiChu;
    private TextBox txtGhiChu;
    private CheckBox chkDangHoatDong;
    private Button btnSave;
    private Button btnCancel;
}
