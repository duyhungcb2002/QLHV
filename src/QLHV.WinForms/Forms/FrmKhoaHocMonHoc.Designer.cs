namespace QLHV.WinForms.Forms;

partial class FrmKhoaHocMonHoc
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
        pnlTop = new Panel();
        lblCourseInfo = new Label();
        lblTitle = new Label();
        pnlBottom = new Panel();
        btnClose = new Button();
        btnRemove = new Button();
        btnEdit = new Button();
        btnAdd = new Button();
        dgvSubjects = new DataGridView();
        colThuTu = new DataGridViewTextBoxColumn();
        colMaMonHoc = new DataGridViewTextBoxColumn();
        colTenMonHoc = new DataGridViewTextBoxColumn();
        colSoBuoiDuKien = new DataGridViewTextBoxColumn();
        pnlTop.SuspendLayout();
        pnlBottom.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvSubjects).BeginInit();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.BackColor = Color.White;
        pnlTop.Controls.Add(lblCourseInfo);
        pnlTop.Controls.Add(lblTitle);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Size = new Size(680, 75);
        pnlTop.TabIndex = 0;
        // 
        // lblCourseInfo
        // 
        lblCourseInfo.AutoSize = true;
        lblCourseInfo.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        lblCourseInfo.ForeColor = Color.FromArgb(55, 65, 81);
        lblCourseInfo.Location = new Point(20, 42);
        lblCourseInfo.Name = "lblCourseInfo";
        lblCourseInfo.Size = new Size(139, 19);
        lblCourseInfo.TabIndex = 1;
        lblCourseInfo.Text = "Khóa học: [Thông tin]";
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.ForeColor = Color.FromArgb(30, 64, 175);
        lblTitle.Location = new Point(20, 15);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(335, 21);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "QUẢN LÝ MÔN HỌC THUỘC KHÓA ĐÀO TẠO";
        // 
        // pnlBottom
        // 
        pnlBottom.BackColor = Color.FromArgb(249, 250, 251);
        pnlBottom.Controls.Add(btnClose);
        pnlBottom.Controls.Add(btnRemove);
        pnlBottom.Controls.Add(btnEdit);
        pnlBottom.Controls.Add(btnAdd);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 390);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Size = new Size(680, 50);
        pnlBottom.TabIndex = 1;
        // 
        // btnClose
        // 
        btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClose.BackColor = Color.FromArgb(229, 231, 235);
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.Location = new Point(585, 10);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(80, 30);
        btnClose.TabIndex = 3;
        btnClose.Text = "Đóng";
        btnClose.UseVisualStyleBackColor = false;
        btnClose.Click += btnClose_Click;
        // 
        // btnRemove
        // 
        btnRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRemove.BackColor = Color.FromArgb(239, 68, 68);
        btnRemove.FlatAppearance.BorderSize = 0;
        btnRemove.FlatStyle = FlatStyle.Flat;
        btnRemove.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnRemove.ForeColor = Color.White;
        btnRemove.Location = new Point(485, 10);
        btnRemove.Name = "btnRemove";
        btnRemove.Size = new Size(90, 30);
        btnRemove.TabIndex = 2;
        btnRemove.Text = "Xóa môn";
        btnRemove.UseVisualStyleBackColor = false;
        btnRemove.Click += btnRemove_Click;
        // 
        // btnEdit
        // 
        btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnEdit.BackColor = Color.FromArgb(245, 158, 11);
        btnEdit.FlatAppearance.BorderSize = 0;
        btnEdit.FlatStyle = FlatStyle.Flat;
        btnEdit.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnEdit.ForeColor = Color.White;
        btnEdit.Location = new Point(385, 10);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(90, 30);
        btnEdit.TabIndex = 1;
        btnEdit.Text = "Sửa thứ tự";
        btnEdit.UseVisualStyleBackColor = false;
        btnEdit.Click += btnEdit_Click;
        // 
        // btnAdd
        // 
        btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAdd.BackColor = Color.FromArgb(16, 185, 129);
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        btnAdd.ForeColor = Color.White;
        btnAdd.Location = new Point(275, 10);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(100, 30);
        btnAdd.TabIndex = 0;
        btnAdd.Text = "Gán môn học";
        btnAdd.UseVisualStyleBackColor = false;
        btnAdd.Click += btnAdd_Click;
        // 
        // dgvSubjects
        // 
        dgvSubjects.AllowUserToAddRows = false;
        dgvSubjects.AllowUserToDeleteRows = false;
        dgvSubjects.BackgroundColor = Color.White;
        dgvSubjects.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvSubjects.Columns.AddRange(new DataGridViewColumn[] { colThuTu, colMaMonHoc, colTenMonHoc, colSoBuoiDuKien });
        dgvSubjects.Dock = DockStyle.Fill;
        dgvSubjects.Location = new Point(0, 75);
        dgvSubjects.MultiSelect = false;
        dgvSubjects.Name = "dgvSubjects";
        dgvSubjects.ReadOnly = true;
        dgvSubjects.RowHeadersVisible = false;
        dgvSubjects.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSubjects.Size = new Size(680, 315);
        dgvSubjects.TabIndex = 2;
        // 
        // colThuTu
        // 
        colThuTu.DataPropertyName = "ThuTu";
        colThuTu.HeaderText = "Thứ tự";
        colThuTu.Name = "colThuTu";
        colThuTu.ReadOnly = true;
        colThuTu.Width = 80;
        // 
        // colMaMonHoc
        // 
        colMaMonHoc.DataPropertyName = "MaMonHoc";
        colMaMonHoc.HeaderText = "Mã môn học";
        colMaMonHoc.Name = "colMaMonHoc";
        colMaMonHoc.ReadOnly = true;
        colMaMonHoc.Width = 110;
        // 
        // colTenMonHoc
        // 
        colTenMonHoc.DataPropertyName = "TenMonHoc";
        colTenMonHoc.HeaderText = "Tên môn học";
        colTenMonHoc.Name = "colTenMonHoc";
        colTenMonHoc.ReadOnly = true;
        colTenMonHoc.Width = 320;
        // 
        // colSoBuoiDuKien
        // 
        colSoBuoiDuKien.DataPropertyName = "SoBuoiDuKien";
        colSoBuoiDuKien.HeaderText = "Số buổi";
        colSoBuoiDuKien.Name = "colSoBuoiDuKien";
        colSoBuoiDuKien.ReadOnly = true;
        colSoBuoiDuKien.Width = 120;
        // 
        // FrmKhoaHocMonHoc
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(680, 440);
        Controls.Add(dgvSubjects);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTop);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FrmKhoaHocMonHoc";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Quản lý môn học trong khóa";
        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        pnlBottom.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvSubjects).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlTop;
    private Label lblTitle;
    private Label lblCourseInfo;
    private Panel pnlBottom;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnRemove;
    private Button btnClose;
    private DataGridView dgvSubjects;
    private DataGridViewTextBoxColumn colThuTu;
    private DataGridViewTextBoxColumn colMaMonHoc;
    private DataGridViewTextBoxColumn colTenMonHoc;
    private DataGridViewTextBoxColumn colSoBuoiDuKien;
}
