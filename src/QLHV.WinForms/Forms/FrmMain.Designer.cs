namespace QLHV.WinForms.Forms;

partial class FrmMain
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
        pnlSidebar = new Panel();
        btnLogout = new Button();
        btnMenuBaoCao = new Button();
        btnMenuHocPhi = new Button();
        btnMenuDiem = new Button();
        btnMenuDaoTao = new Button();
        btnMenuPhongHoc = new Button();
        btnMenuKhoaHoc = new Button();
        btnMenuMonHoc = new Button();
        btnMenuGiangVien = new Button();
        btnMenuHocVien = new Button();
        pnlUserCard = new Panel();
        lblRole = new Label();
        lblUsername = new Label();
        pnlBrand = new Panel();
        lblAppSub = new Label();
        lblAppTitle = new Label();
        pnlTopBar = new Panel();
        lblEnvBadge = new Label();
        lblActiveViewTitle = new Label();
        pnlContent = new Panel();
        pnlSidebar.SuspendLayout();
        pnlUserCard.SuspendLayout();
        pnlBrand.SuspendLayout();
        pnlTopBar.SuspendLayout();
        SuspendLayout();
        // 
        // pnlSidebar
        // 
        pnlSidebar.BackColor = Color.FromArgb(17, 24, 39);
        pnlSidebar.Controls.Add(btnLogout);
        pnlSidebar.Controls.Add(btnMenuBaoCao);
        pnlSidebar.Controls.Add(btnMenuHocPhi);
        pnlSidebar.Controls.Add(btnMenuDiem);
        pnlSidebar.Controls.Add(btnMenuDaoTao);
        pnlSidebar.Controls.Add(btnMenuPhongHoc);
        pnlSidebar.Controls.Add(btnMenuKhoaHoc);
        pnlSidebar.Controls.Add(btnMenuMonHoc);
        pnlSidebar.Controls.Add(btnMenuGiangVien);
        pnlSidebar.Controls.Add(btnMenuHocVien);
        pnlSidebar.Controls.Add(pnlUserCard);
        pnlSidebar.Controls.Add(pnlBrand);
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.Location = new Point(0, 0);
        pnlSidebar.Name = "pnlSidebar";
        pnlSidebar.Size = new Size(230, 680);
        pnlSidebar.TabIndex = 0;
        // 
        // btnLogout
        // 
        btnLogout.BackColor = Color.FromArgb(220, 38, 38);
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.Dock = DockStyle.Bottom;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        btnLogout.ForeColor = Color.White;
        btnLogout.Location = new Point(0, 638);
        btnLogout.Name = "btnLogout";
        btnLogout.Size = new Size(230, 42);
        btnLogout.TabIndex = 11;
        btnLogout.Text = "🚪 Đăng xuất";
        btnLogout.UseVisualStyleBackColor = false;
        btnLogout.Click += btnLogout_Click;
        // 
        // btnMenuBaoCao
        // 
        btnMenuBaoCao.Cursor = Cursors.Hand;
        btnMenuBaoCao.Dock = DockStyle.Top;
        btnMenuBaoCao.FlatAppearance.BorderSize = 0;
        btnMenuBaoCao.FlatStyle = FlatStyle.Flat;
        btnMenuBaoCao.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuBaoCao.ForeColor = Color.FromArgb(156, 163, 175);
        btnMenuBaoCao.Location = new Point(0, 470);
        btnMenuBaoCao.Name = "btnMenuBaoCao";
        btnMenuBaoCao.Padding = new Padding(20, 0, 0, 0);
        btnMenuBaoCao.Size = new Size(230, 38);
        btnMenuBaoCao.TabIndex = 10;
        btnMenuBaoCao.Text = "📊 Báo cáo [Chờ tích hợp]";
        btnMenuBaoCao.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuBaoCao.UseVisualStyleBackColor = true;
        btnMenuBaoCao.Click += btnMenuBaoCao_Click;
        // 
        // btnMenuHocPhi
        // 
        btnMenuHocPhi.Cursor = Cursors.Hand;
        btnMenuHocPhi.Dock = DockStyle.Top;
        btnMenuHocPhi.FlatAppearance.BorderSize = 0;
        btnMenuHocPhi.FlatStyle = FlatStyle.Flat;
        btnMenuHocPhi.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuHocPhi.ForeColor = Color.FromArgb(156, 163, 175);
        btnMenuHocPhi.Location = new Point(0, 432);
        btnMenuHocPhi.Name = "btnMenuHocPhi";
        btnMenuHocPhi.Padding = new Padding(20, 0, 0, 0);
        btnMenuHocPhi.Size = new Size(230, 38);
        btnMenuHocPhi.TabIndex = 9;
        btnMenuHocPhi.Text = "💰 Học phí [Chờ tích hợp]";
        btnMenuHocPhi.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuHocPhi.UseVisualStyleBackColor = true;
        btnMenuHocPhi.Click += btnMenuHocPhi_Click;
        // 
        // btnMenuDiem
        // 
        btnMenuDiem.Cursor = Cursors.Hand;
        btnMenuDiem.Dock = DockStyle.Top;
        btnMenuDiem.FlatAppearance.BorderSize = 0;
        btnMenuDiem.FlatStyle = FlatStyle.Flat;
        btnMenuDiem.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuDiem.ForeColor = Color.FromArgb(156, 163, 175);
        btnMenuDiem.Location = new Point(0, 394);
        btnMenuDiem.Name = "btnMenuDiem";
        btnMenuDiem.Padding = new Padding(20, 0, 0, 0);
        btnMenuDiem.Size = new Size(230, 38);
        btnMenuDiem.TabIndex = 8;
        btnMenuDiem.Text = "📝 Điểm số [Chờ tích hợp]";
        btnMenuDiem.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuDiem.UseVisualStyleBackColor = true;
        btnMenuDiem.Click += btnMenuDiem_Click;
        // 
        // btnMenuDaoTao
        // 
        btnMenuDaoTao.Cursor = Cursors.Hand;
        btnMenuDaoTao.Dock = DockStyle.Top;
        btnMenuDaoTao.FlatAppearance.BorderSize = 0;
        btnMenuDaoTao.FlatStyle = FlatStyle.Flat;
        btnMenuDaoTao.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuDaoTao.ForeColor = Color.FromArgb(156, 163, 175);
        btnMenuDaoTao.Location = new Point(0, 356);
        btnMenuDaoTao.Name = "btnMenuDaoTao";
        btnMenuDaoTao.Padding = new Padding(20, 0, 0, 0);
        btnMenuDaoTao.Size = new Size(230, 38);
        btnMenuDaoTao.TabIndex = 7;
        btnMenuDaoTao.Text = "🎓 Đào tạo [Chờ tích hợp]";
        btnMenuDaoTao.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuDaoTao.UseVisualStyleBackColor = true;
        btnMenuDaoTao.Click += btnMenuDaoTao_Click;
        // 
        // btnMenuPhongHoc
        // 
        btnMenuPhongHoc.Cursor = Cursors.Hand;
        btnMenuPhongHoc.Dock = DockStyle.Top;
        btnMenuPhongHoc.FlatAppearance.BorderSize = 0;
        btnMenuPhongHoc.FlatStyle = FlatStyle.Flat;
        btnMenuPhongHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuPhongHoc.ForeColor = Color.White;
        btnMenuPhongHoc.Location = new Point(0, 318);
        btnMenuPhongHoc.Name = "btnMenuPhongHoc";
        btnMenuPhongHoc.Padding = new Padding(20, 0, 0, 0);
        btnMenuPhongHoc.Size = new Size(230, 38);
        btnMenuPhongHoc.TabIndex = 6;
        btnMenuPhongHoc.Text = "🏢 Danh mục Phòng học";
        btnMenuPhongHoc.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuPhongHoc.UseVisualStyleBackColor = true;
        btnMenuPhongHoc.Click += btnMenuPhongHoc_Click;
        // 
        // btnMenuKhoaHoc
        // 
        btnMenuKhoaHoc.Cursor = Cursors.Hand;
        btnMenuKhoaHoc.Dock = DockStyle.Top;
        btnMenuKhoaHoc.FlatAppearance.BorderSize = 0;
        btnMenuKhoaHoc.FlatStyle = FlatStyle.Flat;
        btnMenuKhoaHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuKhoaHoc.ForeColor = Color.White;
        btnMenuKhoaHoc.Location = new Point(0, 280);
        btnMenuKhoaHoc.Name = "btnMenuKhoaHoc";
        btnMenuKhoaHoc.Padding = new Padding(20, 0, 0, 0);
        btnMenuKhoaHoc.Size = new Size(230, 38);
        btnMenuKhoaHoc.TabIndex = 5;
        btnMenuKhoaHoc.Text = "📚 Danh mục Khóa học";
        btnMenuKhoaHoc.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuKhoaHoc.UseVisualStyleBackColor = true;
        btnMenuKhoaHoc.Click += btnMenuKhoaHoc_Click;
        // 
        // btnMenuMonHoc
        // 
        btnMenuMonHoc.Cursor = Cursors.Hand;
        btnMenuMonHoc.Dock = DockStyle.Top;
        btnMenuMonHoc.FlatAppearance.BorderSize = 0;
        btnMenuMonHoc.FlatStyle = FlatStyle.Flat;
        btnMenuMonHoc.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuMonHoc.ForeColor = Color.White;
        btnMenuMonHoc.Location = new Point(0, 242);
        btnMenuMonHoc.Name = "btnMenuMonHoc";
        btnMenuMonHoc.Padding = new Padding(20, 0, 0, 0);
        btnMenuMonHoc.Size = new Size(230, 38);
        btnMenuMonHoc.TabIndex = 4;
        btnMenuMonHoc.Text = "📖 Danh mục Môn học";
        btnMenuMonHoc.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuMonHoc.UseVisualStyleBackColor = true;
        btnMenuMonHoc.Click += btnMenuMonHoc_Click;
        // 
        // btnMenuGiangVien
        // 
        btnMenuGiangVien.Cursor = Cursors.Hand;
        btnMenuGiangVien.Dock = DockStyle.Top;
        btnMenuGiangVien.FlatAppearance.BorderSize = 0;
        btnMenuGiangVien.FlatStyle = FlatStyle.Flat;
        btnMenuGiangVien.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuGiangVien.ForeColor = Color.White;
        btnMenuGiangVien.Location = new Point(0, 204);
        btnMenuGiangVien.Name = "btnMenuGiangVien";
        btnMenuGiangVien.Padding = new Padding(20, 0, 0, 0);
        btnMenuGiangVien.Size = new Size(230, 38);
        btnMenuGiangVien.TabIndex = 3;
        btnMenuGiangVien.Text = "👨‍🏫 Danh mục Giảng viên";
        btnMenuGiangVien.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuGiangVien.UseVisualStyleBackColor = true;
        btnMenuGiangVien.Click += btnMenuGiangVien_Click;
        // 
        // btnMenuHocVien
        // 
        btnMenuHocVien.Cursor = Cursors.Hand;
        btnMenuHocVien.Dock = DockStyle.Top;
        btnMenuHocVien.FlatAppearance.BorderSize = 0;
        btnMenuHocVien.FlatStyle = FlatStyle.Flat;
        btnMenuHocVien.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
        btnMenuHocVien.ForeColor = Color.White;
        btnMenuHocVien.Location = new Point(0, 166);
        btnMenuHocVien.Name = "btnMenuHocVien";
        btnMenuHocVien.Padding = new Padding(20, 0, 0, 0);
        btnMenuHocVien.Size = new Size(230, 38);
        btnMenuHocVien.TabIndex = 2;
        btnMenuHocVien.Text = "👤 Danh mục Học viên";
        btnMenuHocVien.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuHocVien.UseVisualStyleBackColor = true;
        btnMenuHocVien.Click += btnMenuHocVien_Click;
        // 
        // pnlUserCard
        // 
        pnlUserCard.BackColor = Color.FromArgb(31, 41, 55);
        pnlUserCard.Controls.Add(lblRole);
        pnlUserCard.Controls.Add(lblUsername);
        pnlUserCard.Dock = DockStyle.Top;
        pnlUserCard.Location = new Point(0, 70);
        pnlUserCard.Name = "pnlUserCard";
        pnlUserCard.Size = new Size(230, 96);
        pnlUserCard.TabIndex = 1;
        // 
        // lblRole
        // 
        lblRole.AutoSize = true;
        lblRole.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblRole.ForeColor = Color.FromArgb(52, 211, 153);
        lblRole.Location = new Point(15, 52);
        lblRole.Name = "lblRole";
        lblRole.Size = new Size(95, 15);
        lblRole.TabIndex = 1;
        lblRole.Text = "Vai trò: ADMIN";
        // 
        // lblUsername
        // 
        lblUsername.AutoSize = true;
        lblUsername.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblUsername.ForeColor = Color.White;
        lblUsername.Location = new Point(15, 22);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(137, 17);
        lblUsername.TabIndex = 0;
        lblUsername.Text = "👤 Tài khoản: admin";
        // 
        // pnlBrand
        // 
        pnlBrand.BackColor = Color.FromArgb(30, 64, 175);
        pnlBrand.Controls.Add(lblAppSub);
        pnlBrand.Controls.Add(lblAppTitle);
        pnlBrand.Dock = DockStyle.Top;
        pnlBrand.Location = new Point(0, 0);
        pnlBrand.Name = "pnlBrand";
        pnlBrand.Size = new Size(230, 70);
        pnlBrand.TabIndex = 0;
        // 
        // lblAppSub
        // 
        lblAppSub.AutoSize = true;
        lblAppSub.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
        lblAppSub.ForeColor = Color.FromArgb(219, 234, 254);
        lblAppSub.Location = new Point(15, 42);
        lblAppSub.Name = "lblAppSub";
        lblAppSub.Size = new Size(130, 13);
        lblAppSub.TabIndex = 1;
        lblAppSub.Text = "Quản lý đào tạo học viên";
        // 
        // lblAppTitle
        // 
        lblAppTitle.AutoSize = true;
        lblAppTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        lblAppTitle.ForeColor = Color.White;
        lblAppTitle.Location = new Point(15, 15);
        lblAppTitle.Name = "lblAppTitle";
        lblAppTitle.Size = new Size(135, 21);
        lblAppTitle.TabIndex = 0;
        lblAppTitle.Text = "HỆ THỐNG QLHV";
        // 
        // pnlTopBar
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(lblEnvBadge);
        pnlTopBar.Controls.Add(lblActiveViewTitle);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(230, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(950, 50);
        pnlTopBar.TabIndex = 1;
        // 
        // lblEnvBadge
        // 
        lblEnvBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblEnvBadge.AutoSize = true;
        lblEnvBadge.BackColor = Color.FromArgb(254, 243, 199);
        lblEnvBadge.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblEnvBadge.ForeColor = Color.FromArgb(180, 83, 9);
        lblEnvBadge.Location = new Point(740, 18);
        lblEnvBadge.Name = "lblEnvBadge";
        lblEnvBadge.Padding = new Padding(6, 2, 6, 2);
        lblEnvBadge.Size = new Size(195, 19);
        lblEnvBadge.TabIndex = 1;
        lblEnvBadge.Text = "Môi trường: Fake Data In-Memory";
        // 
        // lblActiveViewTitle
        // 
        lblActiveViewTitle.AutoSize = true;
        lblActiveViewTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lblActiveViewTitle.ForeColor = Color.FromArgb(30, 41, 59);
        lblActiveViewTitle.Location = new Point(20, 16);
        lblActiveViewTitle.Name = "lblActiveViewTitle";
        lblActiveViewTitle.Size = new Size(160, 20);
        lblActiveViewTitle.TabIndex = 0;
        lblActiveViewTitle.Text = "Trang chủ / Danh mục";
        // 
        // pnlContent
        // 
        pnlContent.BackColor = Color.FromArgb(243, 244, 246);
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(230, 50);
        pnlContent.Name = "pnlContent";
        pnlContent.Size = new Size(950, 630);
        pnlContent.TabIndex = 2;
        // 
        // FrmMain
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1180, 680);
        Controls.Add(pnlContent);
        Controls.Add(pnlTopBar);
        Controls.Add(pnlSidebar);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        MinimumSize = new Size(980, 600);
        Name = "FrmMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hệ thống Quản lý học viên - QLHV";
        pnlSidebar.ResumeLayout(false);
        pnlUserCard.ResumeLayout(false);
        pnlUserCard.PerformLayout();
        pnlBrand.ResumeLayout(false);
        pnlBrand.PerformLayout();
        pnlTopBar.ResumeLayout(false);
        pnlTopBar.PerformLayout();
        ResumeLayout(false);
    }

    private Panel pnlSidebar;
    private Panel pnlBrand;
    private Label lblAppTitle;
    private Label lblAppSub;
    private Panel pnlUserCard;
    private Label lblUsername;
    private Label lblRole;
    private Button btnMenuHocVien;
    private Button btnMenuGiangVien;
    private Button btnMenuMonHoc;
    private Button btnMenuKhoaHoc;
    private Button btnMenuPhongHoc;
    private Button btnMenuDaoTao;
    private Button btnMenuDiem;
    private Button btnMenuHocPhi;
    private Button btnMenuBaoCao;
    private Button btnLogout;
    private Panel pnlTopBar;
    private Label lblActiveViewTitle;
    private Label lblEnvBadge;
    private Panel pnlContent;
}
