namespace QLHV.WinForms.Forms;

partial class FrmBaoCao
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
        tabControlBaoCao = new TabControl();
        tabHocVienLop = new TabPage();
        dgvTab1 = new DataGridView();
        pnlTop1 = new Panel();
        btnTab1_Xem = new Button();
        cboTab1_Lop = new ComboBox();
        lblTab1_Lop = new Label();
        tabLichHocLop = new TabPage();
        dgvTab2 = new DataGridView();
        pnlTop2 = new Panel();
        btnTab2_Xem = new Button();
        cboTab2_Lop = new ComboBox();
        lblTab2_Lop = new Label();
        tabPhanCongGV = new TabPage();
        dgvTab3 = new DataGridView();
        pnlTop3 = new Panel();
        btnTab3_Xem = new Button();
        cboTab3_GV = new ComboBox();
        lblTab3_GV = new Label();
        tabKetQuaHocTap = new TabPage();
        splitTab4 = new SplitContainer();
        grpTab4_ChiTiet = new GroupBox();
        dgvTab4_ChiTiet = new DataGridView();
        grpTab4_TyLe = new GroupBox();
        dgvTab4_TyLe = new DataGridView();
        pnlTop4 = new Panel();
        btnTab4_Xem = new Button();
        cboTab4_Mon = new ComboBox();
        lblTab4_Mon = new Label();
        cboTab4_Lop = new ComboBox();
        lblTab4_Lop = new Label();
        tabCongNoHocPhi = new TabPage();
        dgvTab5 = new DataGridView();
        pnlTop5 = new Panel();
        btnTab5_TaiLai = new Button();
        tabControlBaoCao.SuspendLayout();
        tabHocVienLop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTab1).BeginInit();
        pnlTop1.SuspendLayout();
        tabLichHocLop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTab2).BeginInit();
        pnlTop2.SuspendLayout();
        tabPhanCongGV.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTab3).BeginInit();
        pnlTop3.SuspendLayout();
        tabKetQuaHocTap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitTab4).BeginInit();
        splitTab4.Panel1.SuspendLayout();
        splitTab4.Panel2.SuspendLayout();
        splitTab4.SuspendLayout();
        grpTab4_ChiTiet.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTab4_ChiTiet).BeginInit();
        grpTab4_TyLe.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTab4_TyLe).BeginInit();
        pnlTop4.SuspendLayout();
        tabCongNoHocPhi.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTab5).BeginInit();
        pnlTop5.SuspendLayout();
        SuspendLayout();
        //
        // tabControlBaoCao
        //
        tabControlBaoCao.Controls.Add(tabHocVienLop);
        tabControlBaoCao.Controls.Add(tabLichHocLop);
        tabControlBaoCao.Controls.Add(tabPhanCongGV);
        tabControlBaoCao.Controls.Add(tabKetQuaHocTap);
        tabControlBaoCao.Controls.Add(tabCongNoHocPhi);
        tabControlBaoCao.Dock = DockStyle.Fill;
        tabControlBaoCao.Font = new Font("Segoe UI", 9.75F);
        tabControlBaoCao.Location = new Point(0, 0);
        tabControlBaoCao.Name = "tabControlBaoCao";
        tabControlBaoCao.SelectedIndex = 0;
        tabControlBaoCao.Size = new Size(950, 600);
        tabControlBaoCao.TabIndex = 0;
        //
        // tabHocVienLop
        //
        tabHocVienLop.Controls.Add(dgvTab1);
        tabHocVienLop.Controls.Add(pnlTop1);
        tabHocVienLop.Location = new Point(4, 26);
        tabHocVienLop.Name = "tabHocVienLop";
        tabHocVienLop.Padding = new Padding(3);
        tabHocVienLop.Size = new Size(942, 570);
        tabHocVienLop.TabIndex = 0;
        tabHocVienLop.Text = "  Học viên theo Lớp  ";
        tabHocVienLop.UseVisualStyleBackColor = true;
        //
        // dgvTab1
        //
        dgvTab1.AllowUserToAddRows = false;
        dgvTab1.AllowUserToDeleteRows = false;
        dgvTab1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTab1.BackgroundColor = Color.White;
        dgvTab1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTab1.Dock = DockStyle.Fill;
        dgvTab1.Location = new Point(3, 53);
        dgvTab1.MultiSelect = false;
        dgvTab1.Name = "dgvTab1";
        dgvTab1.ReadOnly = true;
        dgvTab1.RowHeadersVisible = false;
        dgvTab1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTab1.Size = new Size(936, 514);
        dgvTab1.TabIndex = 1;
        //
        // pnlTop1
        //
        pnlTop1.BackColor = Color.FromArgb(249, 250, 251);
        pnlTop1.Controls.Add(btnTab1_Xem);
        pnlTop1.Controls.Add(cboTab1_Lop);
        pnlTop1.Controls.Add(lblTab1_Lop);
        pnlTop1.Dock = DockStyle.Top;
        pnlTop1.Location = new Point(3, 3);
        pnlTop1.Name = "pnlTop1";
        pnlTop1.Size = new Size(936, 50);
        pnlTop1.TabIndex = 0;
        //
        // btnTab1_Xem
        //
        btnTab1_Xem.BackColor = Color.FromArgb(79, 70, 229);
        btnTab1_Xem.FlatAppearance.BorderSize = 0;
        btnTab1_Xem.FlatStyle = FlatStyle.Flat;
        btnTab1_Xem.ForeColor = Color.White;
        btnTab1_Xem.Location = new Point(350, 10);
        btnTab1_Xem.Name = "btnTab1_Xem";
        btnTab1_Xem.Size = new Size(90, 28);
        btnTab1_Xem.TabIndex = 2;
        btnTab1_Xem.Text = "Xem báo cáo";
        btnTab1_Xem.UseVisualStyleBackColor = false;
        btnTab1_Xem.Click += btnTab1_Xem_Click;
        //
        // cboTab1_Lop
        //
        cboTab1_Lop.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTab1_Lop.FormattingEnabled = true;
        cboTab1_Lop.Location = new Point(80, 12);
        cboTab1_Lop.Name = "cboTab1_Lop";
        cboTab1_Lop.Size = new Size(250, 25);
        cboTab1_Lop.TabIndex = 1;
        //
        // lblTab1_Lop
        //
        lblTab1_Lop.AutoSize = true;
        lblTab1_Lop.Location = new Point(15, 15);
        lblTab1_Lop.Name = "lblTab1_Lop";
        lblTab1_Lop.Size = new Size(59, 17);
        lblTab1_Lop.TabIndex = 0;
        lblTab1_Lop.Text = "Lớp học:";
        //
        // tabLichHocLop
        //
        tabLichHocLop.Controls.Add(dgvTab2);
        tabLichHocLop.Controls.Add(pnlTop2);
        tabLichHocLop.Location = new Point(4, 26);
        tabLichHocLop.Name = "tabLichHocLop";
        tabLichHocLop.Padding = new Padding(3);
        tabLichHocLop.Size = new Size(942, 570);
        tabLichHocLop.TabIndex = 1;
        tabLichHocLop.Text = "  Lịch học theo Lớp  ";
        tabLichHocLop.UseVisualStyleBackColor = true;
        //
        // dgvTab2
        //
        dgvTab2.AllowUserToAddRows = false;
        dgvTab2.AllowUserToDeleteRows = false;
        dgvTab2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTab2.BackgroundColor = Color.White;
        dgvTab2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTab2.Dock = DockStyle.Fill;
        dgvTab2.Location = new Point(3, 53);
        dgvTab2.MultiSelect = false;
        dgvTab2.Name = "dgvTab2";
        dgvTab2.ReadOnly = true;
        dgvTab2.RowHeadersVisible = false;
        dgvTab2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTab2.Size = new Size(936, 514);
        dgvTab2.TabIndex = 2;
        //
        // pnlTop2
        //
        pnlTop2.BackColor = Color.FromArgb(249, 250, 251);
        pnlTop2.Controls.Add(btnTab2_Xem);
        pnlTop2.Controls.Add(cboTab2_Lop);
        pnlTop2.Controls.Add(lblTab2_Lop);
        pnlTop2.Dock = DockStyle.Top;
        pnlTop2.Location = new Point(3, 3);
        pnlTop2.Name = "pnlTop2";
        pnlTop2.Size = new Size(936, 50);
        pnlTop2.TabIndex = 1;
        //
        // btnTab2_Xem
        //
        btnTab2_Xem.BackColor = Color.FromArgb(79, 70, 229);
        btnTab2_Xem.FlatAppearance.BorderSize = 0;
        btnTab2_Xem.FlatStyle = FlatStyle.Flat;
        btnTab2_Xem.ForeColor = Color.White;
        btnTab2_Xem.Location = new Point(350, 10);
        btnTab2_Xem.Name = "btnTab2_Xem";
        btnTab2_Xem.Size = new Size(90, 28);
        btnTab2_Xem.TabIndex = 2;
        btnTab2_Xem.Text = "Xem lịch";
        btnTab2_Xem.UseVisualStyleBackColor = false;
        btnTab2_Xem.Click += btnTab2_Xem_Click;
        //
        // cboTab2_Lop
        //
        cboTab2_Lop.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTab2_Lop.FormattingEnabled = true;
        cboTab2_Lop.Location = new Point(80, 12);
        cboTab2_Lop.Name = "cboTab2_Lop";
        cboTab2_Lop.Size = new Size(250, 25);
        cboTab2_Lop.TabIndex = 1;
        //
        // lblTab2_Lop
        //
        lblTab2_Lop.AutoSize = true;
        lblTab2_Lop.Location = new Point(15, 15);
        lblTab2_Lop.Name = "lblTab2_Lop";
        lblTab2_Lop.Size = new Size(59, 17);
        lblTab2_Lop.TabIndex = 0;
        lblTab2_Lop.Text = "Lớp học:";
        //
        // tabPhanCongGV
        //
        tabPhanCongGV.Controls.Add(dgvTab3);
        tabPhanCongGV.Controls.Add(pnlTop3);
        tabPhanCongGV.Location = new Point(4, 26);
        tabPhanCongGV.Name = "tabPhanCongGV";
        tabPhanCongGV.Padding = new Padding(3);
        tabPhanCongGV.Size = new Size(942, 570);
        tabPhanCongGV.TabIndex = 2;
        tabPhanCongGV.Text = "  Phân công Giảng viên  ";
        tabPhanCongGV.UseVisualStyleBackColor = true;
        //
        // dgvTab3
        //
        dgvTab3.AllowUserToAddRows = false;
        dgvTab3.AllowUserToDeleteRows = false;
        dgvTab3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTab3.BackgroundColor = Color.White;
        dgvTab3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTab3.Dock = DockStyle.Fill;
        dgvTab3.Location = new Point(3, 53);
        dgvTab3.MultiSelect = false;
        dgvTab3.Name = "dgvTab3";
        dgvTab3.ReadOnly = true;
        dgvTab3.RowHeadersVisible = false;
        dgvTab3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTab3.Size = new Size(936, 514);
        dgvTab3.TabIndex = 2;
        //
        // pnlTop3
        //
        pnlTop3.BackColor = Color.FromArgb(249, 250, 251);
        pnlTop3.Controls.Add(btnTab3_Xem);
        pnlTop3.Controls.Add(cboTab3_GV);
        pnlTop3.Controls.Add(lblTab3_GV);
        pnlTop3.Dock = DockStyle.Top;
        pnlTop3.Location = new Point(3, 3);
        pnlTop3.Name = "pnlTop3";
        pnlTop3.Size = new Size(936, 50);
        pnlTop3.TabIndex = 1;
        //
        // btnTab3_Xem
        //
        btnTab3_Xem.BackColor = Color.FromArgb(79, 70, 229);
        btnTab3_Xem.FlatAppearance.BorderSize = 0;
        btnTab3_Xem.FlatStyle = FlatStyle.Flat;
        btnTab3_Xem.ForeColor = Color.White;
        btnTab3_Xem.Location = new Point(350, 10);
        btnTab3_Xem.Name = "btnTab3_Xem";
        btnTab3_Xem.Size = new Size(90, 28);
        btnTab3_Xem.TabIndex = 2;
        btnTab3_Xem.Text = "Xem lịch";
        btnTab3_Xem.UseVisualStyleBackColor = false;
        btnTab3_Xem.Click += btnTab3_Xem_Click;
        //
        // cboTab3_GV
        //
        cboTab3_GV.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTab3_GV.FormattingEnabled = true;
        cboTab3_GV.Location = new Point(95, 12);
        cboTab3_GV.Name = "cboTab3_GV";
        cboTab3_GV.Size = new Size(240, 25);
        cboTab3_GV.TabIndex = 1;
        //
        // lblTab3_GV
        //
        lblTab3_GV.AutoSize = true;
        lblTab3_GV.Location = new Point(15, 15);
        lblTab3_GV.Name = "lblTab3_GV";
        lblTab3_GV.Size = new Size(74, 17);
        lblTab3_GV.TabIndex = 0;
        lblTab3_GV.Text = "Giảng viên:";
        //
        // tabKetQuaHocTap
        //
        tabKetQuaHocTap.Controls.Add(splitTab4);
        tabKetQuaHocTap.Controls.Add(pnlTop4);
        tabKetQuaHocTap.Location = new Point(4, 26);
        tabKetQuaHocTap.Name = "tabKetQuaHocTap";
        tabKetQuaHocTap.Padding = new Padding(3);
        tabKetQuaHocTap.Size = new Size(942, 570);
        tabKetQuaHocTap.TabIndex = 3;
        tabKetQuaHocTap.Text = "  Kết quả & Tỷ lệ đạt  ";
        tabKetQuaHocTap.UseVisualStyleBackColor = true;
        //
        // splitTab4
        //
        splitTab4.Dock = DockStyle.Fill;
        splitTab4.Location = new Point(3, 53);
        splitTab4.Name = "splitTab4";
        //
        // splitTab4.Panel1
        //
        splitTab4.Panel1.Controls.Add(grpTab4_ChiTiet);
        //
        // splitTab4.Panel2
        //
        splitTab4.Panel2.Controls.Add(grpTab4_TyLe);
        splitTab4.Size = new Size(936, 514);
        splitTab4.SplitterDistance = 580;
        splitTab4.TabIndex = 1;
        //
        // grpTab4_ChiTiet
        //
        grpTab4_ChiTiet.Controls.Add(dgvTab4_ChiTiet);
        grpTab4_ChiTiet.Dock = DockStyle.Fill;
        grpTab4_ChiTiet.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grpTab4_ChiTiet.Location = new Point(0, 0);
        grpTab4_ChiTiet.Name = "grpTab4_ChiTiet";
        grpTab4_ChiTiet.Size = new Size(580, 514);
        grpTab4_ChiTiet.TabIndex = 0;
        grpTab4_ChiTiet.TabStop = false;
        grpTab4_ChiTiet.Text = " Chi tiết điểm thi học viên ";
        //
        // dgvTab4_ChiTiet
        //
        dgvTab4_ChiTiet.AllowUserToAddRows = false;
        dgvTab4_ChiTiet.AllowUserToDeleteRows = false;
        dgvTab4_ChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTab4_ChiTiet.BackgroundColor = Color.White;
        dgvTab4_ChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTab4_ChiTiet.Dock = DockStyle.Fill;
        dgvTab4_ChiTiet.Font = new Font("Segoe UI", 9F);
        dgvTab4_ChiTiet.Location = new Point(3, 19);
        dgvTab4_ChiTiet.MultiSelect = false;
        dgvTab4_ChiTiet.Name = "dgvTab4_ChiTiet";
        dgvTab4_ChiTiet.ReadOnly = true;
        dgvTab4_ChiTiet.RowHeadersVisible = false;
        dgvTab4_ChiTiet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTab4_ChiTiet.Size = new Size(574, 492);
        dgvTab4_ChiTiet.TabIndex = 0;
        //
        // grpTab4_TyLe
        //
        grpTab4_TyLe.Controls.Add(dgvTab4_TyLe);
        grpTab4_TyLe.Dock = DockStyle.Fill;
        grpTab4_TyLe.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grpTab4_TyLe.Location = new Point(0, 0);
        grpTab4_TyLe.Name = "grpTab4_TyLe";
        grpTab4_TyLe.Size = new Size(352, 514);
        grpTab4_TyLe.TabIndex = 0;
        grpTab4_TyLe.TabStop = false;
        grpTab4_TyLe.Text = " Thống kê tỷ lệ Đạt / Trượt ";
        //
        // dgvTab4_TyLe
        //
        dgvTab4_TyLe.AllowUserToAddRows = false;
        dgvTab4_TyLe.AllowUserToDeleteRows = false;
        dgvTab4_TyLe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTab4_TyLe.BackgroundColor = Color.White;
        dgvTab4_TyLe.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTab4_TyLe.Dock = DockStyle.Fill;
        dgvTab4_TyLe.Font = new Font("Segoe UI", 9F);
        dgvTab4_TyLe.Location = new Point(3, 19);
        dgvTab4_TyLe.MultiSelect = false;
        dgvTab4_TyLe.Name = "dgvTab4_TyLe";
        dgvTab4_TyLe.ReadOnly = true;
        dgvTab4_TyLe.RowHeadersVisible = false;
        dgvTab4_TyLe.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTab4_TyLe.Size = new Size(346, 492);
        dgvTab4_TyLe.TabIndex = 0;
        //
        // pnlTop4
        //
        pnlTop4.BackColor = Color.FromArgb(249, 250, 251);
        pnlTop4.Controls.Add(btnTab4_Xem);
        pnlTop4.Controls.Add(cboTab4_Mon);
        pnlTop4.Controls.Add(lblTab4_Mon);
        pnlTop4.Controls.Add(cboTab4_Lop);
        pnlTop4.Controls.Add(lblTab4_Lop);
        pnlTop4.Dock = DockStyle.Top;
        pnlTop4.Location = new Point(3, 3);
        pnlTop4.Name = "pnlTop4";
        pnlTop4.Size = new Size(936, 50);
        pnlTop4.TabIndex = 0;
        //
        // btnTab4_Xem
        //
        btnTab4_Xem.BackColor = Color.FromArgb(79, 70, 229);
        btnTab4_Xem.FlatAppearance.BorderSize = 0;
        btnTab4_Xem.FlatStyle = FlatStyle.Flat;
        btnTab4_Xem.ForeColor = Color.White;
        btnTab4_Xem.Location = new Point(590, 10);
        btnTab4_Xem.Name = "btnTab4_Xem";
        btnTab4_Xem.Size = new Size(90, 28);
        btnTab4_Xem.TabIndex = 4;
        btnTab4_Xem.Text = "Xem";
        btnTab4_Xem.UseVisualStyleBackColor = false;
        btnTab4_Xem.Click += btnTab4_Xem_Click;
        //
        // cboTab4_Mon
        //
        cboTab4_Mon.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTab4_Mon.FormattingEnabled = true;
        cboTab4_Mon.Location = new Point(370, 12);
        cboTab4_Mon.Name = "cboTab4_Mon";
        cboTab4_Mon.Size = new Size(200, 25);
        cboTab4_Mon.TabIndex = 3;
        //
        // lblTab4_Mon
        //
        lblTab4_Mon.AutoSize = true;
        lblTab4_Mon.Location = new Point(300, 15);
        lblTab4_Mon.Name = "lblTab4_Mon";
        lblTab4_Mon.Size = new Size(64, 17);
        lblTab4_Mon.TabIndex = 2;
        lblTab4_Mon.Text = "Môn học:";
        //
        // cboTab4_Lop
        //
        cboTab4_Lop.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTab4_Lop.FormattingEnabled = true;
        cboTab4_Lop.Location = new Point(80, 12);
        cboTab4_Lop.Name = "cboTab4_Lop";
        cboTab4_Lop.Size = new Size(200, 25);
        cboTab4_Lop.TabIndex = 1;
        //
        // lblTab4_Lop
        //
        lblTab4_Lop.AutoSize = true;
        lblTab4_Lop.Location = new Point(15, 15);
        lblTab4_Lop.Name = "lblTab4_Lop";
        lblTab4_Lop.Size = new Size(59, 17);
        lblTab4_Lop.TabIndex = 0;
        lblTab4_Lop.Text = "Lớp học:";
        //
        // tabCongNoHocPhi
        //
        tabCongNoHocPhi.Controls.Add(dgvTab5);
        tabCongNoHocPhi.Controls.Add(pnlTop5);
        tabCongNoHocPhi.Location = new Point(4, 26);
        tabCongNoHocPhi.Name = "tabCongNoHocPhi";
        tabCongNoHocPhi.Padding = new Padding(3);
        tabCongNoHocPhi.Size = new Size(942, 570);
        tabCongNoHocPhi.TabIndex = 4;
        tabCongNoHocPhi.Text = "  Doanh thu & Công nợ  ";
        tabCongNoHocPhi.UseVisualStyleBackColor = true;
        //
        // dgvTab5
        //
        dgvTab5.AllowUserToAddRows = false;
        dgvTab5.AllowUserToDeleteRows = false;
        dgvTab5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTab5.BackgroundColor = Color.White;
        dgvTab5.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTab5.Dock = DockStyle.Fill;
        dgvTab5.Location = new Point(3, 53);
        dgvTab5.MultiSelect = false;
        dgvTab5.Name = "dgvTab5";
        dgvTab5.ReadOnly = true;
        dgvTab5.RowHeadersVisible = false;
        dgvTab5.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTab5.Size = new Size(936, 514);
        dgvTab5.TabIndex = 2;
        //
        // pnlTop5
        //
        pnlTop5.BackColor = Color.FromArgb(249, 250, 251);
        pnlTop5.Controls.Add(btnTab5_TaiLai);
        pnlTop5.Dock = DockStyle.Top;
        pnlTop5.Location = new Point(3, 3);
        pnlTop5.Name = "pnlTop5";
        pnlTop5.Size = new Size(936, 50);
        pnlTop5.TabIndex = 1;
        //
        // btnTab5_TaiLai
        //
        btnTab5_TaiLai.BackColor = Color.FromArgb(79, 70, 229);
        btnTab5_TaiLai.FlatAppearance.BorderSize = 0;
        btnTab5_TaiLai.FlatStyle = FlatStyle.Flat;
        btnTab5_TaiLai.ForeColor = Color.White;
        btnTab5_TaiLai.Location = new Point(15, 10);
        btnTab5_TaiLai.Name = "btnTab5_TaiLai";
        btnTab5_TaiLai.Size = new Size(130, 28);
        btnTab5_TaiLai.TabIndex = 0;
        btnTab5_TaiLai.Text = "🔄 Tải lại dữ liệu";
        btnTab5_TaiLai.UseVisualStyleBackColor = false;
        btnTab5_TaiLai.Click += btnTab5_TaiLai_Click;
        //
        // FrmBaoCao
        //
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(950, 600);
        Controls.Add(tabControlBaoCao);
        Font = new Font("Segoe UI", 9.75F);
        FormBorderStyle = FormBorderStyle.None;
        Name = "FrmBaoCao";
        Text = "Báo cáo thống kê";
        tabControlBaoCao.ResumeLayout(false);
        tabHocVienLop.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTab1).EndInit();
        pnlTop1.ResumeLayout(false);
        pnlTop1.PerformLayout();
        tabLichHocLop.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTab2).EndInit();
        pnlTop2.ResumeLayout(false);
        pnlTop2.PerformLayout();
        tabPhanCongGV.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTab3).EndInit();
        pnlTop3.ResumeLayout(false);
        pnlTop3.PerformLayout();
        tabKetQuaHocTap.ResumeLayout(false);
        splitTab4.Panel1.ResumeLayout(false);
        splitTab4.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitTab4).EndInit();
        splitTab4.ResumeLayout(false);
        grpTab4_ChiTiet.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTab4_ChiTiet).EndInit();
        grpTab4_TyLe.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTab4_TyLe).EndInit();
        pnlTop4.ResumeLayout(false);
        pnlTop4.PerformLayout();
        tabCongNoHocPhi.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTab5).EndInit();
        pnlTop5.ResumeLayout(false);
        ResumeLayout(false);
    }

    private TabControl tabControlBaoCao;
    private TabPage tabHocVienLop;
    private TabPage tabLichHocLop;
    private TabPage tabPhanCongGV;
    private TabPage tabKetQuaHocTap;
    private TabPage tabCongNoHocPhi;
    private Panel pnlTop1;
    private Label lblTab1_Lop;
    private ComboBox cboTab1_Lop;
    private Button btnTab1_Xem;
    private DataGridView dgvTab1;
    private Panel pnlTop2;
    private Label lblTab2_Lop;
    private ComboBox cboTab2_Lop;
    private Button btnTab2_Xem;
    private DataGridView dgvTab2;
    private Panel pnlTop3;
    private Label lblTab3_GV;
    private ComboBox cboTab3_GV;
    private Button btnTab3_Xem;
    private DataGridView dgvTab3;
    private Panel pnlTop4;
    private Label lblTab4_Lop;
    private ComboBox cboTab4_Lop;
    private Label lblTab4_Mon;
    private ComboBox cboTab4_Mon;
    private Button btnTab4_Xem;
    private SplitContainer splitTab4;
    private GroupBox grpTab4_ChiTiet;
    private DataGridView dgvTab4_ChiTiet;
    private GroupBox grpTab4_TyLe;
    private DataGridView dgvTab4_TyLe;
    private Panel pnlTop5;
    private Button btnTab5_TaiLai;
    private DataGridView dgvTab5;
}
