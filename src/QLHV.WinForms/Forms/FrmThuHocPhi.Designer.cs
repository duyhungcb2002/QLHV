namespace QLHV.WinForms.Forms;

partial class FrmThuHocPhi
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
        lblThongTinHv = new Label();
        lblSoTienNo = new Label();
        lblSoTienThu = new Label();
        nudSoTien = new NumericUpDown();
        lblHinhThuc = new Label();
        cboHinhThuc = new ComboBox();
        lblGhiChu = new Label();
        txtGhiChu = new TextBox();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)nudSoTien).BeginInit();
        SuspendLayout();
        //
        // lblThongTinHv
        //
        lblThongTinHv.AutoSize = true;
        lblThongTinHv.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblThongTinHv.ForeColor = Color.FromArgb(31, 41, 55);
        lblThongTinHv.Location = new Point(25, 20);
        lblThongTinHv.Name = "lblThongTinHv";
        lblThongTinHv.Size = new Size(160, 19);
        lblThongTinHv.TabIndex = 0;
        lblThongTinHv.Text = "Học viên: [Họ tên mã]";
        //
        // lblSoTienNo
        //
        lblSoTienNo.AutoSize = true;
        lblSoTienNo.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        lblSoTienNo.ForeColor = Color.FromArgb(220, 38, 38);
        lblSoTienNo.Location = new Point(25, 50);
        lblSoTienNo.Name = "lblSoTienNo";
        lblSoTienNo.Size = new Size(130, 17);
        lblSoTienNo.TabIndex = 1;
        lblSoTienNo.Text = "Số tiền còn nợ: 0 đ";
        //
        // lblSoTienThu
        //
        lblSoTienThu.AutoSize = true;
        lblSoTienThu.Location = new Point(25, 90);
        lblSoTienThu.Name = "lblSoTienThu";
        lblSoTienThu.Size = new Size(100, 17);
        lblSoTienThu.TabIndex = 2;
        lblSoTienThu.Text = "Số tiền thu (đ) *:";
        //
        // nudSoTien
        //
        nudSoTien.DecimalPlaces = 0;
        nudSoTien.Increment = new decimal(new int[] { 500000, 0, 0, 0 });
        nudSoTien.Location = new Point(135, 87);
        nudSoTien.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
        nudSoTien.Name = "nudSoTien";
        nudSoTien.Size = new Size(200, 25);
        nudSoTien.TabIndex = 3;
        nudSoTien.ThousandsSeparator = true;
        //
        // lblHinhThuc
        //
        lblHinhThuc.AutoSize = true;
        lblHinhThuc.Location = new Point(25, 130);
        lblHinhThuc.Name = "lblHinhThuc";
        lblHinhThuc.Size = new Size(74, 17);
        lblHinhThuc.TabIndex = 4;
        lblHinhThuc.Text = "Hình thức *:";
        //
        // cboHinhThuc
        //
        cboHinhThuc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboHinhThuc.FormattingEnabled = true;
        cboHinhThuc.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản" });
        cboHinhThuc.Location = new Point(135, 127);
        cboHinhThuc.Name = "cboHinhThuc";
        cboHinhThuc.Size = new Size(200, 25);
        cboHinhThuc.TabIndex = 5;
        //
        // lblGhiChu
        //
        lblGhiChu.AutoSize = true;
        lblGhiChu.Location = new Point(25, 170);
        lblGhiChu.Name = "lblGhiChu";
        lblGhiChu.Size = new Size(54, 17);
        lblGhiChu.TabIndex = 6;
        lblGhiChu.Text = "Ghi chú:";
        //
        // txtGhiChu
        //
        txtGhiChu.Location = new Point(135, 167);
        txtGhiChu.MaxLength = 250;
        txtGhiChu.Multiline = true;
        txtGhiChu.Name = "txtGhiChu";
        txtGhiChu.Size = new Size(250, 50);
        txtGhiChu.TabIndex = 7;
        //
        // btnSave
        //
        btnSave.BackColor = Color.FromArgb(16, 185, 129);
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(180, 235);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 32);
        btnSave.TabIndex = 8;
        btnSave.Text = "Lập phiếu";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += btnSave_Click;
        //
        // btnCancel
        //
        btnCancel.BackColor = Color.FromArgb(156, 163, 175);
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.ForeColor = Color.White;
        btnCancel.Location = new Point(290, 235);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(95, 32);
        btnCancel.TabIndex = 9;
        btnCancel.Text = "Hủy";
        btnCancel.UseVisualStyleBackColor = false;
        btnCancel.Click += btnCancel_Click;
        //
        // FrmThuHocPhi
        //
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(414, 285);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(txtGhiChu);
        Controls.Add(lblGhiChu);
        Controls.Add(cboHinhThuc);
        Controls.Add(lblHinhThuc);
        Controls.Add(nudSoTien);
        Controls.Add(lblSoTienThu);
        Controls.Add(lblSoTienNo);
        Controls.Add(lblThongTinHv);
        Font = new Font("Segoe UI", 9.75F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmThuHocPhi";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thu học phí học viên";
        ((System.ComponentModel.ISupportInitialize)nudSoTien).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblThongTinHv;
    private Label lblSoTienNo;
    private Label lblSoTienThu;
    private NumericUpDown nudSoTien;
    private Label lblHinhThuc;
    private ComboBox cboHinhThuc;
    private Label lblGhiChu;
    private TextBox txtGhiChu;
    private Button btnSave;
    private Button btnCancel;
}
