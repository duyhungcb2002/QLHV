namespace QLHV.WinForms.Forms;

partial class FrmLichHocEdit
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
        lblMonHoc = new Label();
        cboMonHoc = new ComboBox();
        lblGiangVien = new Label();
        cboGiangVien = new ComboBox();
        lblPhongHoc = new Label();
        cboPhongHoc = new ComboBox();
        lblNgayHoc = new Label();
        dtpNgayHoc = new DateTimePicker();
        lblThoiGian = new Label();
        dtpGioBatDau = new DateTimePicker();
        lblDen = new Label();
        dtpGioKetThuc = new DateTimePicker();
        lblNoiDung = new Label();
        txtNoiDung = new TextBox();
        btnSave = new Button();
        btnCancel = new Button();
        SuspendLayout();
        //
        // lblMonHoc
        //
        lblMonHoc.AutoSize = true;
        lblMonHoc.Location = new Point(25, 20);
        lblMonHoc.Name = "lblMonHoc";
        lblMonHoc.Size = new Size(74, 17);
        lblMonHoc.TabIndex = 0;
        lblMonHoc.Text = "Môn học *:";
        //
        // cboMonHoc
        //
        cboMonHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMonHoc.FormattingEnabled = true;
        cboMonHoc.Location = new Point(125, 17);
        cboMonHoc.Name = "cboMonHoc";
        cboMonHoc.Size = new Size(290, 25);
        cboMonHoc.TabIndex = 1;
        //
        // lblGiangVien
        //
        lblGiangVien.AutoSize = true;
        lblGiangVien.Location = new Point(25, 60);
        lblGiangVien.Name = "lblGiangVien";
        lblGiangVien.Size = new Size(84, 17);
        lblGiangVien.TabIndex = 2;
        lblGiangVien.Text = "Giảng viên *:";
        //
        // cboGiangVien
        //
        cboGiangVien.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGiangVien.FormattingEnabled = true;
        cboGiangVien.Location = new Point(125, 57);
        cboGiangVien.Name = "cboGiangVien";
        cboGiangVien.Size = new Size(290, 25);
        cboGiangVien.TabIndex = 3;
        //
        // lblPhongHoc
        //
        lblPhongHoc.AutoSize = true;
        lblPhongHoc.Location = new Point(25, 100);
        lblPhongHoc.Name = "lblPhongHoc";
        lblPhongHoc.Size = new Size(84, 17);
        lblPhongHoc.TabIndex = 4;
        lblPhongHoc.Text = "Phòng học *:";
        //
        // cboPhongHoc
        //
        cboPhongHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPhongHoc.FormattingEnabled = true;
        cboPhongHoc.Location = new Point(125, 97);
        cboPhongHoc.Name = "cboPhongHoc";
        cboPhongHoc.Size = new Size(290, 25);
        cboPhongHoc.TabIndex = 5;
        //
        // lblNgayHoc
        //
        lblNgayHoc.AutoSize = true;
        lblNgayHoc.Location = new Point(25, 140);
        lblNgayHoc.Name = "lblNgayHoc";
        lblNgayHoc.Size = new Size(77, 17);
        lblNgayHoc.TabIndex = 6;
        lblNgayHoc.Text = "Ngày học *:";
        //
        // dtpNgayHoc
        //
        dtpNgayHoc.Format = DateTimePickerFormat.Short;
        dtpNgayHoc.Location = new Point(125, 137);
        dtpNgayHoc.Name = "dtpNgayHoc";
        dtpNgayHoc.Size = new Size(130, 25);
        dtpNgayHoc.TabIndex = 7;
        //
        // lblThoiGian
        //
        lblThoiGian.AutoSize = true;
        lblThoiGian.Location = new Point(25, 180);
        lblThoiGian.Name = "lblThoiGian";
        lblThoiGian.Size = new Size(76, 17);
        lblThoiGian.TabIndex = 8;
        lblThoiGian.Text = "Khung giờ *:";
        //
        // dtpGioBatDau
        //
        dtpGioBatDau.CustomFormat = "HH:mm";
        dtpGioBatDau.Format = DateTimePickerFormat.Custom;
        dtpGioBatDau.Location = new Point(125, 177);
        dtpGioBatDau.Name = "dtpGioBatDau";
        dtpGioBatDau.ShowUpDown = true;
        dtpGioBatDau.Size = new Size(80, 25);
        dtpGioBatDau.TabIndex = 9;
        //
        // lblDen
        //
        lblDen.AutoSize = true;
        lblDen.Location = new Point(215, 180);
        lblDen.Name = "lblDen";
        lblDen.Size = new Size(29, 17);
        lblDen.TabIndex = 10;
        lblDen.Text = "đến";
        //
        // dtpGioKetThuc
        //
        dtpGioKetThuc.CustomFormat = "HH:mm";
        dtpGioKetThuc.Format = DateTimePickerFormat.Custom;
        dtpGioKetThuc.Location = new Point(250, 177);
        dtpGioKetThuc.Name = "dtpGioKetThuc";
        dtpGioKetThuc.ShowUpDown = true;
        dtpGioKetThuc.Size = new Size(80, 25);
        dtpGioKetThuc.TabIndex = 11;
        //
        // lblNoiDung
        //
        lblNoiDung.AutoSize = true;
        lblNoiDung.Location = new Point(25, 220);
        lblNoiDung.Name = "lblNoiDung";
        lblNoiDung.Size = new Size(65, 17);
        lblNoiDung.TabIndex = 12;
        lblNoiDung.Text = "Nội dung:";
        //
        // txtNoiDung
        //
        txtNoiDung.Location = new Point(125, 217);
        txtNoiDung.MaxLength = 500;
        txtNoiDung.Multiline = true;
        txtNoiDung.Name = "txtNoiDung";
        txtNoiDung.Size = new Size(290, 60);
        txtNoiDung.TabIndex = 13;
        //
        // btnSave
        //
        btnSave.BackColor = Color.FromArgb(16, 185, 129);
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(224, 300);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(95, 32);
        btnSave.TabIndex = 14;
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
        btnCancel.Location = new Point(325, 300);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(90, 32);
        btnCancel.TabIndex = 15;
        btnCancel.Text = "Hủy";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Click += btnCancel_Click;
        //
        // FrmLichHocEdit
        //
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(444, 350);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(txtNoiDung);
        Controls.Add(lblNoiDung);
        Controls.Add(dtpGioKetThuc);
        Controls.Add(lblDen);
        Controls.Add(dtpGioBatDau);
        Controls.Add(lblThoiGian);
        Controls.Add(dtpNgayHoc);
        Controls.Add(lblNgayHoc);
        Controls.Add(cboPhongHoc);
        Controls.Add(lblPhongHoc);
        Controls.Add(cboGiangVien);
        Controls.Add(lblGiangVien);
        Controls.Add(cboMonHoc);
        Controls.Add(lblMonHoc);
        Font = new Font("Segoe UI", 9.75F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmLichHocEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thông tin buổi học";
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblMonHoc;
    private ComboBox cboMonHoc;
    private Label lblGiangVien;
    private ComboBox cboGiangVien;
    private Label lblPhongHoc;
    private ComboBox cboPhongHoc;
    private Label lblNgayHoc;
    private DateTimePicker dtpNgayHoc;
    private Label lblThoiGian;
    private DateTimePicker dtpGioBatDau;
    private Label lblDen;
    private DateTimePicker dtpGioKetThuc;
    private Label lblNoiDung;
    private TextBox txtNoiDung;
    private Button btnSave;
    private Button btnCancel;
}
