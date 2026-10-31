using QLHV.Contracts.Results;
using QLHV.Core.Services;

namespace QLHV.WinForms.Forms;

public partial class FrmLogin : Form
{
    private readonly IAuthService _authService;

    public FrmLogin(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        txtUsername.TextChanged += (_, _) => ClearErrors();
        txtPassword.TextChanged += (_, _) => ClearErrors();
    }

    private void ClearErrors()
    {
        errorProvider.SetError(txtUsername, string.Empty);
        errorProvider.SetError(txtPassword, string.Empty);
        lblError.Text = string.Empty;
    }

    private async void btnLogin_Click(object? sender, EventArgs e)
    {
        ClearErrors();

        var username = txtUsername.Text.Trim();
        var password = txtPassword.Text;

        bool hasError = false;
        if (string.IsNullOrWhiteSpace(username))
        {
            errorProvider.SetError(txtUsername, "Vui lòng nhập tên đăng nhập.");
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errorProvider.SetError(txtPassword, "Vui lòng nhập mật khẩu.");
            hasError = true;
        }

        if (hasError)
        {
            lblError.Text = "Vui lòng điền đầy đủ tên đăng nhập và mật khẩu.";
            return;
        }

        btnLogin.Enabled = false;
        btnLogin.Text = "Đang xử lý...";

        try
        {
            var result = await _authService.LoginAsync(username, password);

            if (result.IsSuccess)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                lblError.Text = result.Message;

                if (result.Error?.Code == ErrorCodes.ACCOUNT_LOCKED)
                {
                    errorProvider.SetError(txtUsername, result.Message);
                }
                else
                {
                    errorProvider.SetError(txtPassword, "Mật khẩu không chính xác.");
                }
            }
        }
        finally
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "Đăng nhập";
        }
    }

    private void btnExit_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
