namespace QLHV.WinForms.Forms;

partial class FrmLogin
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
        pnlHeader = new Panel();
        lblSubtitle = new Label();
        lblTitle = new Label();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        btnLogin = new Button();
        btnExit = new Button();
        lblError = new Label();
        errorProvider = new ErrorProvider(components);
        lblDemoHint = new Label();
        pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
        SuspendLayout();
        //
        // pnlHeader
        //
        pnlHeader.BackColor = Color.FromArgb(30, 64, 175);
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(420, 80);
        pnlHeader.TabIndex = 0;
        //
        // lblSubtitle
        //
        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblSubtitle.ForeColor = Color.FromArgb(219, 234, 254);
        lblSubtitle.Location = new Point(130, 48);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(160, 15);
        lblSubtitle.TabIndex = 1;
        lblSubtitle.Text = "Hệ thống Quản lý học viên";
        //
        // lblTitle
        //
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(100, 18);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(220, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "ĐĂNG NHẬP HỆ THỐNG";
        //
        // lblUsername
        //
        lblUsername.AutoSize = true;
        lblUsername.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblUsername.Location = new Point(40, 105);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(104, 17);
        lblUsername.TabIndex = 1;
        lblUsername.Text = "Tên đăng nhập:";
        //
        // txtUsername
        //
        txtUsername.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        txtUsername.Location = new Point(40, 128);
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(340, 25);
        txtUsername.TabIndex = 2;
        //
        // lblPassword
        //
        lblPassword.AutoSize = true;
        lblPassword.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
        lblPassword.Location = new Point(40, 168);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(70, 17);
        lblPassword.TabIndex = 3;
        lblPassword.Text = "Mật khẩu:";
        //
        // txtPassword
        //
        txtPassword.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        txtPassword.Location = new Point(40, 191);
        txtPassword.Name = "txtPassword";
        txtPassword.Size = new Size(340, 25);
        txtPassword.TabIndex = 4;
        txtPassword.UseSystemPasswordChar = true;
        //
        // btnLogin
        //
        btnLogin.BackColor = Color.FromArgb(30, 64, 175);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(40, 245);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(160, 38);
        btnLogin.TabIndex = 5;
        btnLogin.Text = "Đăng nhập";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Click += btnLogin_Click;
        //
        // btnExit
        //
        btnExit.BackColor = Color.FromArgb(229, 231, 235);
        btnExit.Cursor = Cursors.Hand;
        btnExit.FlatAppearance.BorderSize = 0;
        btnExit.FlatStyle = FlatStyle.Flat;
        btnExit.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        btnExit.ForeColor = Color.FromArgb(55, 65, 81);
        btnExit.Location = new Point(220, 245);
        btnExit.Name = "btnExit";
        btnExit.Size = new Size(160, 38);
        btnExit.TabIndex = 6;
        btnExit.Text = "Thoát";
        btnExit.UseVisualStyleBackColor = false;
        btnExit.Click += btnExit_Click;
        //
        // lblError
        //
        lblError.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        lblError.ForeColor = Color.FromArgb(220, 38, 38);
        lblError.Location = new Point(40, 290);
        lblError.Name = "lblError";
        lblError.Size = new Size(340, 40);
        lblError.TabIndex = 7;
        lblError.TextAlign = ContentAlignment.MiddleCenter;
        //
        // errorProvider
        //
        errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        errorProvider.ContainerControl = this;
        //
        // lblDemoHint
        //
        lblDemoHint.AutoSize = true;
        lblDemoHint.Font = new Font("Segoe UI", 8F, FontStyle.Italic, GraphicsUnit.Point);
        lblDemoHint.ForeColor = Color.FromArgb(107, 114, 128);
        lblDemoHint.Location = new Point(40, 335);
        lblDemoHint.Name = "lblDemoHint";
        lblDemoHint.Size = new Size(236, 13);
        lblDemoHint.TabIndex = 8;
        lblDemoHint.Text = "Tài khoản mẫu: admin / Admin@123 (Xem README)";
        //
        // FrmLogin
        //
        AcceptButton = btnLogin;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnExit;
        ClientSize = new Size(420, 360);
        Controls.Add(lblDemoHint);
        Controls.Add(lblError);
        Controls.Add(btnExit);
        Controls.Add(btnLogin);
        Controls.Add(txtPassword);
        Controls.Add(lblPassword);
        Controls.Add(txtUsername);
        Controls.Add(lblUsername);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "FrmLogin";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Đăng nhập hệ thống QLHV";
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Panel pnlHeader;
    private Label lblTitle;
    private Label lblSubtitle;
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblPassword;
    private TextBox txtPassword;
    private Button btnLogin;
    private Button btnExit;
    private Label lblError;
    private ErrorProvider errorProvider;
    private Label lblDemoHint;
}
