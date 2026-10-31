using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Features.Catalog.Services;

namespace QLHV.Tests;

[TestClass]
public class AuthTests
{
    private IPasswordHasher _passwordHasher = null!;
    private FakeTaiKhoanRepository _taiKhoanRepo = null!;
    private IAuthorizationService _authzService = null!;
    private ICurrentUserService _currentUserService = null!;
    private IAppLogger _logger = null!;
    private AuthService _authService = null!;

    [TestInitialize]
    public void Setup()
    {
        _passwordHasher = new Pbkdf2PasswordHasher();
        _taiKhoanRepo = new FakeTaiKhoanRepository(_passwordHasher);
        _authzService = new AuthorizationService();
        _currentUserService = new CurrentUserService(_authzService);
        _logger = new FileAppLogger();
        _authService = new AuthService(_taiKhoanRepo, _passwordHasher, _currentUserService, _logger);
    }

    [TestMethod]
    public async Task Login_WithValidCredentials_ReturnsSuccessAndSetsCurrentUser()
    {
        var result = await _authService.LoginAsync("admin", "Admin@123");

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("admin", result.Data.TenDangNhap);
        Assert.AreEqual(VaiTro.ADMIN, result.Data.VaiTro);
        Assert.IsTrue(_currentUserService.IsAuthenticated);
        Assert.IsNotNull(_currentUserService.CurrentUser);
        Assert.AreEqual("admin", _currentUserService.CurrentUser.TenDangNhap);
    }

    [TestMethod]
    public async Task Login_WithWrongPassword_ReturnsInvalidCredentials()
    {
        var result = await _authService.LoginAsync("admin", "WrongPassword!@#");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_CREDENTIALS, result.Error?.Code);
        Assert.IsFalse(_currentUserService.IsAuthenticated);
    }

    [TestMethod]
    public async Task Login_WithNonExistentUser_ReturnsInvalidCredentials()
    {
        var result = await _authService.LoginAsync("nonexistentuser", "SomePass123");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_CREDENTIALS, result.Error?.Code);
        Assert.IsFalse(_currentUserService.IsAuthenticated);
    }

    [TestMethod]
    public async Task Login_WithLockedAccount_ReturnsAccountLocked()
    {
        // Account "bikhoa" has status BiKhoa in seed
        var result = await _authService.LoginAsync("bikhoa", "BiKhoa@123");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.ACCOUNT_LOCKED, result.Error?.Code);
        Assert.IsFalse(_currentUserService.IsAuthenticated);
    }

    [TestMethod]
    public async Task Logout_ClearsCurrentUserSession()
    {
        // Login first
        var loginResult = await _authService.LoginAsync("admin", "Admin@123");
        Assert.IsTrue(loginResult.IsSuccess);
        Assert.IsTrue(_currentUserService.IsAuthenticated);

        // Logout
        var logoutResult = await _authService.LogoutAsync();
        Assert.IsTrue(logoutResult.IsSuccess);
        Assert.IsFalse(_currentUserService.IsAuthenticated);
        Assert.IsNull(_currentUserService.CurrentUser);
    }
}
