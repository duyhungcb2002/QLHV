namespace QLHV.WinForms.Forms;

partial class FrmMonHocEdit
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
        lblMaMonHoc = new Label();
        txtMaMonHoc = new TextBox();
        lblTenMonHoc = new Label();
        txtTenMonHoc = new TextBox();
        lblSoBuoi = new Label();
        nudSoBuoi = new NumericUpDown();
        lblMoTa = new Label();
        txtMoTa = new TextBox();
        lblTrangThai = new Label();
        cboTrangThai = new ComboBox();
        btnSave = new Button();
        btnCancel = new Button();
        errorProvider = new ErrorProvider(components);
        lblError = new Label();
        ((System.ComponentModel.ISupportInitialize)nudSoBuoi).BeginInit();
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
        lblTitle.Size = new Size(207, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "THÔNG TIN MÔN HỌC";
        //
        // lblMaMonHoc
        //
        lblMaMonHoc.AutoSize = true;
        lblMaMonHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblMaMonHoc.Location = new Point(25, 65);
        lblMaMonHoc.Name = "lblMaMonHoc";
        lblMaMonHoc.Size = new Size(115, 17);
        lblMaMonHoc.TabIndex = 1;
        lblMaMonHoc.Text = "Mã môn học (*):";
        //
        // txtMaMonHoc
        //
        txtMaMonHoc.Location = new Point(25, 88);
        txtMaMonHoc.Name = "txtMaMonHoc";
        txtMaMonHoc.Size = new Size(220, 23);
        txtMaMonHoc.TabIndex = 2;
        //
        // lblTenMonHoc
        //
        lblTenMonHoc.AutoSize = true;
        lblTenMonHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblTenMonHoc.Location = new Point(265, 65);
        lblTenMonHoc.Name = "lblTenMonHoc";
        lblTenMonHoc.Size = new Size(118, 17);
        lblTenMonHoc.TabIndex = 3;
        lblTenMonHoc.Text = "Tên môn học (*):";
        //
        // txtTenMonHoc
        //
        txtTenMonHoc.Location = new Point(265, 88);
        txtTenMonHoc.Name = "txtTenMonHoc";
        txtTenMonHoc.Size = new Size(250, 23);
        txtTenMonHoc.TabIndex = 4;
        //
        // lblSoBuoi
        //
        lblSoBuoi.AutoSize = true;
        lblSoBuoi.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblSoBuoi.Location = new Point(25, 125);
        lblSoBuoi.Name = "lblSoBuoi";
        lblSoBuoi.Size = new Size(139, 17);
        lblSoBuoi.TabIndex = 5;
        lblSoBuoi.Text = "Số buổi mặc định (*):";
        //
        // nudSoBuoi
        //
        nudSoBuoi.Location = new Point(25, 148);
        nudSoBuoi.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
        nudSoBuoi.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudSoBuoi.Name = "nudSoBuoi";
        nudSoBuoi.Size = new Size(220, 23);
        nudSoBuoi.TabIndex = 6;
        nudSoBuoi.Value = new decimal(new int[] { 15, 0, 0, 0 });
        //
        // lblMoTa
        //
        lblMoTa.AutoSize = true;
        lblMoTa.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblMoTa.Location = new Point(25, 185);
        lblMoTa.Name = "lblMoTa";
        lblMoTa.Size = new Size(46, 17);
        lblMoTa.TabIndex = 7;
        lblMoTa.Text = "Mô tả:";
        //
        // txtMoTa
        //
        txtMoTa.Location = new Point(25, 208);
        txtMoTa.Multiline = true;
        txtMoTa.Name = "txtMoTa";
        txtMoTa.Size = new Size(490, 60);
        txtMoTa.TabIndex = 8;
        //
        // lblTrangThai
        //
        lblTrangThai.AutoSize = true;
        lblTrangThai.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblTrangThai.Location = new Point(265, 125);
        lblTrangThai.Name = "lblTrangThai";
        lblTrangThai.Size = new Size(69, 17);
        lblTrangThai.TabIndex = 9;
        lblTrangThai.Text = "Trạng thái:";
        //
        // cboTrangThai
        //
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.FormattingEnabled = true;
        cboTrangThai.Location = new Point(265, 148);
        cboTrangThai.Name = "cboTrangThai";
        cboTrangThai.Size = new Size(250, 23);
        cboTrangThai.TabIndex = 10;
        //
        // btnSave
        //
        btnSave.BackColor = Color.FromArgb(30, 64, 175);
        btnSave.Cursor = Cursors.Hand;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(295, 305);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(105, 35);
        btnSave.TabIndex = 11;
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
        btnCancel.Location = new Point(410, 305);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(105, 35);
        btnCancel.TabIndex = 12;
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
        lblError.Location = new Point(25, 280);
        lblError.Name = "lblError";
        lblError.Size = new Size(490, 20);
        lblError.TabIndex = 13;
        //
        // FrmMonHocEdit
        //
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnCancel;
        ClientSize = new Size(545, 355);
        Controls.Add(lblError);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(cboTrangThai);
        Controls.Add(lblTrangThai);
        Controls.Add(txtMoTa);
        Controls.Add(lblMoTa);
        Controls.Add(nudSoBuoi);
        Controls.Add(lblSoBuoi);
        Controls.Add(txtTenMonHoc);
        Controls.Add(lblTenMonHoc);
        Controls.Add(txtMaMonHoc);
        Controls.Add(lblMaMonHoc);
        Controls.Add(lblTitle);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmMonHocEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thông tin môn học";
        ((System.ComponentModel.ISupportInitialize)nudSoBuoi).EndInit();
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblMaMonHoc;
    private TextBox txtMaMonHoc;
    private Label lblTenMonHoc;
    private TextBox txtTenMonHoc;
    private Label lblSoBuoi;
    private NumericUpDown nudSoBuoi;
    private Label lblMoTa;
    private TextBox txtMoTa;
    private Label lblTrangThai;
    private ComboBox cboTrangThai;
    private Button btnSave;
    private Button btnCancel;
    private ErrorProvider errorProvider;
    private Label lblError;
}
