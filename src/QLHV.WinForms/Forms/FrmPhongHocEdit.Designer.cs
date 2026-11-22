namespace QLHV.WinForms.Forms;

partial class FrmPhongHocEdit
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
        lblMaPhong = new Label();
        txtMaPhong = new TextBox();
        lblTenPhong = new Label();
        txtTenPhong = new TextBox();
        lblSucChua = new Label();
        nudSucChua = new NumericUpDown();
        lblTrangThai = new Label();
        cboTrangThai = new ComboBox();
        btnSave = new Button();
        btnCancel = new Button();
        errorProvider = new ErrorProvider(components);
        lblError = new Label();
        ((System.ComponentModel.ISupportInitialize)nudSucChua).BeginInit();
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
        lblTitle.Size = new Size(230, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "THÔNG TIN PHÒNG HỌC";
        //
        // lblMaPhong
        //
        lblMaPhong.AutoSize = true;
        lblMaPhong.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblMaPhong.Location = new Point(25, 65);
        lblMaPhong.Name = "lblMaPhong";
        lblMaPhong.Size = new Size(128, 17);
        lblMaPhong.TabIndex = 1;
        lblMaPhong.Text = "Mã phòng học (*):";
        //
        // txtMaPhong
        //
        txtMaPhong.Location = new Point(25, 88);
        txtMaPhong.Name = "txtMaPhong";
        txtMaPhong.Size = new Size(220, 23);
        txtMaPhong.TabIndex = 2;
        //
        // lblTenPhong
        //
        lblTenPhong.AutoSize = true;
        lblTenPhong.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblTenPhong.Location = new Point(265, 65);
        lblTenPhong.Name = "lblTenPhong";
        lblTenPhong.Size = new Size(131, 17);
        lblTenPhong.TabIndex = 3;
        lblTenPhong.Text = "Tên phòng học (*):";
        //
        // txtTenPhong
        //
        txtTenPhong.Location = new Point(265, 88);
        txtTenPhong.Name = "txtTenPhong";
        txtTenPhong.Size = new Size(250, 23);
        txtTenPhong.TabIndex = 4;
        //
        // lblSucChua
        //
        lblSucChua.AutoSize = true;
        lblSucChua.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblSucChua.Location = new Point(25, 125);
        lblSucChua.Name = "lblSucChua";
        lblSucChua.Size = new Size(140, 17);
        lblSucChua.TabIndex = 5;
        lblSucChua.Text = "Sức chứa (học viên) (*):";
        //
        // nudSucChua
        //
        nudSucChua.Location = new Point(25, 148);
        nudSucChua.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
        nudSucChua.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudSucChua.Name = "nudSucChua";
        nudSucChua.Size = new Size(220, 23);
        nudSucChua.TabIndex = 6;
        nudSucChua.Value = new decimal(new int[] { 35, 0, 0, 0 });
        //
        // lblTrangThai
        //
        lblTrangThai.AutoSize = true;
        lblTrangThai.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblTrangThai.Location = new Point(265, 125);
        lblTrangThai.Name = "lblTrangThai";
        lblTrangThai.Size = new Size(69, 17);
        lblTrangThai.TabIndex = 7;
        lblTrangThai.Text = "Trạng thái:";
        //
        // cboTrangThai
        //
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.FormattingEnabled = true;
        cboTrangThai.Location = new Point(265, 148);
        cboTrangThai.Name = "cboTrangThai";
        cboTrangThai.Size = new Size(250, 23);
        cboTrangThai.TabIndex = 8;
        //
        // btnSave
        //
        btnSave.BackColor = Color.FromArgb(30, 64, 175);
        btnSave.Cursor = Cursors.Hand;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(295, 215);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(105, 35);
        btnSave.TabIndex = 9;
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
        btnCancel.Location = new Point(410, 215);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(105, 35);
        btnCancel.TabIndex = 10;
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
        lblError.Location = new Point(25, 185);
        lblError.Name = "lblError";
        lblError.Size = new Size(490, 20);
        lblError.TabIndex = 11;
        //
        // FrmPhongHocEdit
        //
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnCancel;
        ClientSize = new Size(545, 265);
        Controls.Add(lblError);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(cboTrangThai);
        Controls.Add(lblTrangThai);
        Controls.Add(nudSucChua);
        Controls.Add(lblSucChua);
        Controls.Add(txtTenPhong);
        Controls.Add(lblTenPhong);
        Controls.Add(txtMaPhong);
        Controls.Add(lblMaPhong);
        Controls.Add(lblTitle);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmPhongHocEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thông tin phòng học";
        ((System.ComponentModel.ISupportInitialize)nudSucChua).EndInit();
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblMaPhong;
    private TextBox txtMaPhong;
    private Label lblTenPhong;
    private TextBox txtTenPhong;
    private Label lblSucChua;
    private NumericUpDown nudSucChua;
    private Label lblTrangThai;
    private ComboBox cboTrangThai;
    private Button btnSave;
    private Button btnCancel;
    private ErrorProvider errorProvider;
    private Label lblError;
}
