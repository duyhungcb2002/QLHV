namespace QLHV.WinForms.Forms;

partial class FrmKhoaHocEdit
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
        lblMaKhoaHoc = new Label();
        txtMaKhoaHoc = new TextBox();
        lblTenKhoaHoc = new Label();
        txtTenKhoaHoc = new TextBox();
        lblHocPhi = new Label();
        nudHocPhi = new NumericUpDown();
        lblMoTa = new Label();
        txtMoTa = new TextBox();
        lblTrangThai = new Label();
        cboTrangThai = new ComboBox();
        btnSave = new Button();
        btnCancel = new Button();
        errorProvider = new ErrorProvider(components);
        lblError = new Label();
        ((System.ComponentModel.ISupportInitialize)nudHocPhi).BeginInit();
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
        lblTitle.Size = new Size(223, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "THÔNG TIN KHÓA HỌC";
        // 
        // lblMaKhoaHoc
        // 
        lblMaKhoaHoc.AutoSize = true;
        lblMaKhoaHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblMaKhoaHoc.Location = new Point(25, 65);
        lblMaKhoaHoc.Name = "lblMaKhoaHoc";
        lblMaKhoaHoc.Size = new Size(122, 17);
        lblMaKhoaHoc.TabIndex = 1;
        lblMaKhoaHoc.Text = "Mã khóa học (*):";
        // 
        // txtMaKhoaHoc
        // 
        txtMaKhoaHoc.Location = new Point(25, 88);
        txtMaKhoaHoc.Name = "txtMaKhoaHoc";
        txtMaKhoaHoc.Size = new Size(220, 23);
        txtMaKhoaHoc.TabIndex = 2;
        // 
        // lblTenKhoaHoc
        // 
        lblTenKhoaHoc.AutoSize = true;
        lblTenKhoaHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblTenKhoaHoc.Location = new Point(265, 65);
        lblTenKhoaHoc.Name = "lblTenKhoaHoc";
        lblTenKhoaHoc.Size = new Size(125, 17);
        lblTenKhoaHoc.TabIndex = 3;
        lblTenKhoaHoc.Text = "Tên khóa học (*):";
        // 
        // txtTenKhoaHoc
        // 
        txtTenKhoaHoc.Location = new Point(265, 88);
        txtTenKhoaHoc.Name = "txtTenKhoaHoc";
        txtTenKhoaHoc.Size = new Size(250, 23);
        txtTenKhoaHoc.TabIndex = 4;
        // 
        // lblHocPhi
        // 
        lblHocPhi.AutoSize = true;
        lblHocPhi.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblHocPhi.Location = new Point(25, 125);
        lblHocPhi.Name = "lblHocPhi";
        lblHocPhi.Size = new Size(149, 17);
        lblHocPhi.TabIndex = 5;
        lblHocPhi.Text = "Học phí niêm yết (VNĐ):";
        // 
        // nudHocPhi
        // 
        nudHocPhi.Increment = new decimal(new int[] { 100000, 0, 0, 0 });
        nudHocPhi.Location = new Point(25, 148);
        nudHocPhi.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
        nudHocPhi.Name = "nudHocPhi";
        nudHocPhi.Size = new Size(220, 23);
        nudHocPhi.TabIndex = 6;
        nudHocPhi.ThousandsSeparator = true;
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
        // FrmKhoaHocEdit
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
        Controls.Add(nudHocPhi);
        Controls.Add(lblHocPhi);
        Controls.Add(txtTenKhoaHoc);
        Controls.Add(lblTenKhoaHoc);
        Controls.Add(txtMaKhoaHoc);
        Controls.Add(lblMaKhoaHoc);
        Controls.Add(lblTitle);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmKhoaHocEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Thông tin khóa học";
        ((System.ComponentModel.ISupportInitialize)nudHocPhi).EndInit();
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblMaKhoaHoc;
    private TextBox txtMaKhoaHoc;
    private Label lblTenKhoaHoc;
    private TextBox txtTenKhoaHoc;
    private Label lblHocPhi;
    private NumericUpDown nudHocPhi;
    private Label lblMoTa;
    private TextBox txtMoTa;
    private Label lblTrangThai;
    private ComboBox cboTrangThai;
    private Button btnSave;
    private Button btnCancel;
    private ErrorProvider errorProvider;
    private Label lblError;
}
