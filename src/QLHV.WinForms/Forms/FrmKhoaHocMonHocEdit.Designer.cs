namespace QLHV.WinForms.Forms;

partial class FrmKhoaHocMonHocEdit
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
        lblKhoaHoc = new Label();
        lblKhoaHocVal = new Label();
        lblMonHoc = new Label();
        cboMonHoc = new ComboBox();
        lblThuTu = new Label();
        nudThuTu = new NumericUpDown();
        lblSoBuoi = new Label();
        nudSoBuoi = new NumericUpDown();
        btnSave = new Button();
        btnCancel = new Button();
        errorProvider = new ErrorProvider(components);
        lblError = new Label();
        ((System.ComponentModel.ISupportInitialize)nudThuTu).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudSoBuoi).BeginInit();
        ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
        SuspendLayout();
        //
        // lblTitle
        //
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.ForeColor = Color.FromArgb(30, 64, 175);
        lblTitle.Location = new Point(25, 20);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(279, 21);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "GÁN MÔN HỌC VÀO KHÓA ĐÀO TẠO";
        //
        // lblKhoaHoc
        //
        lblKhoaHoc.AutoSize = true;
        lblKhoaHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblKhoaHoc.Location = new Point(25, 60);
        lblKhoaHoc.Name = "lblKhoaHoc";
        lblKhoaHoc.Size = new Size(69, 17);
        lblKhoaHoc.TabIndex = 1;
        lblKhoaHoc.Text = "Khóa học:";
        //
        // lblKhoaHocVal
        //
        lblKhoaHocVal.AutoSize = true;
        lblKhoaHocVal.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblKhoaHocVal.ForeColor = Color.FromArgb(30, 64, 175);
        lblKhoaHocVal.Location = new Point(105, 60);
        lblKhoaHocVal.Name = "lblKhoaHocVal";
        lblKhoaHocVal.Size = new Size(97, 17);
        lblKhoaHocVal.TabIndex = 2;
        lblKhoaHocVal.Text = "[Tên Khóa Học]";
        //
        // lblMonHoc
        //
        lblMonHoc.AutoSize = true;
        lblMonHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblMonHoc.Location = new Point(25, 95);
        lblMonHoc.Name = "lblMonHoc";
        lblMonHoc.Size = new Size(118, 17);
        lblMonHoc.TabIndex = 3;
        lblMonHoc.Text = "Chọn môn học (*):";
        //
        // cboMonHoc
        //
        cboMonHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMonHoc.FormattingEnabled = true;
        cboMonHoc.Location = new Point(25, 118);
        cboMonHoc.Name = "cboMonHoc";
        cboMonHoc.Size = new Size(410, 23);
        cboMonHoc.TabIndex = 4;
        //
        // lblThuTu
        //
        lblThuTu.AutoSize = true;
        lblThuTu.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblThuTu.Location = new Point(25, 160);
        lblThuTu.Name = "lblThuTu";
        lblThuTu.Size = new Size(88, 17);
        lblThuTu.TabIndex = 5;
        lblThuTu.Text = "Thứ tự học (*):";
        //
        // nudThuTu
        //
        nudThuTu.Location = new Point(25, 183);
        nudThuTu.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudThuTu.Name = "nudThuTu";
        nudThuTu.Size = new Size(180, 23);
        nudThuTu.TabIndex = 6;
        nudThuTu.Value = new decimal(new int[] { 1, 0, 0, 0 });
        //
        // lblSoBuoi
        //
        lblSoBuoi.AutoSize = true;
        lblSoBuoi.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        lblSoBuoi.Location = new Point(230, 160);
        lblSoBuoi.Name = "lblSoBuoi";
        lblSoBuoi.Size = new Size(135, 17);
        lblSoBuoi.TabIndex = 7;
        lblSoBuoi.Text = "Số buổi dự kiến (*):";
        //
        // nudSoBuoi
        //
        nudSoBuoi.Location = new Point(230, 183);
        nudSoBuoi.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
        nudSoBuoi.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        nudSoBuoi.Name = "nudSoBuoi";
        nudSoBuoi.Size = new Size(205, 23);
        nudSoBuoi.TabIndex = 8;
        nudSoBuoi.Value = new decimal(new int[] { 15, 0, 0, 0 });
        //
        // btnSave
        //
        btnSave.BackColor = Color.FromArgb(30, 64, 175);
        btnSave.Cursor = Cursors.Hand;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(215, 245);
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
        btnCancel.Location = new Point(330, 245);
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
        lblError.Location = new Point(25, 218);
        lblError.Name = "lblError";
        lblError.Size = new Size(410, 20);
        lblError.TabIndex = 11;
        //
        // FrmKhoaHocMonHocEdit
        //
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnCancel;
        ClientSize = new Size(465, 295);
        Controls.Add(lblError);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(nudSoBuoi);
        Controls.Add(lblSoBuoi);
        Controls.Add(nudThuTu);
        Controls.Add(lblThuTu);
        Controls.Add(cboMonHoc);
        Controls.Add(lblMonHoc);
        Controls.Add(lblKhoaHocVal);
        Controls.Add(lblKhoaHoc);
        Controls.Add(lblTitle);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmKhoaHocMonHocEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Gán môn học vào khóa";
        ((System.ComponentModel.ISupportInitialize)nudThuTu).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudSoBuoi).EndInit();
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblKhoaHoc;
    private Label lblKhoaHocVal;
    private Label lblMonHoc;
    private ComboBox cboMonHoc;
    private Label lblThuTu;
    private NumericUpDown nudThuTu;
    private Label lblSoBuoi;
    private NumericUpDown nudSoBuoi;
    private Button btnSave;
    private Button btnCancel;
    private ErrorProvider errorProvider;
    private Label lblError;
}
