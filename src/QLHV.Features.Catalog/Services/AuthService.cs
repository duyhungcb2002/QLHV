using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Catalog.Services;

public class AuthService : IAuthService
{
    private readonly ITaiKhoanRepository _taiKhoanRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public AuthService(
        ITaiKhoanRepository taiKhoanRepository,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _taiKhoanRepository = taiKhoanRepository;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<TaiKhoanDto>> LoginAsync(string tenDangNhap, string matKhau)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                return OperationResult<TaiKhoanDto>.Failed(ErrorCodes.REQUIRED, "Tên đăng nhập không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(matKhau))
            {
                return OperationResult<TaiKhoanDto>.Failed(ErrorCodes.REQUIRED, "Mật khẩu không được để trống.");
            }

            var username = tenDangNhap.Trim();
            var user = await _taiKhoanRepository.GetByTenDangNhapAsync(username);

            if (user == null)
            {
                _logger.LogWarning($"Đăng nhập thất bại: Tài khoản '{username}' không tồn tại.");
                return OperationResult<TaiKhoanDto>.Failed(ErrorCodes.INVALID_CREDENTIALS, "Tên đăng nhập hoặc mật khẩu không chính xác.");
            }

            if (user.TrangThai == TrangThaiTaiKhoan.BiKhoa)
            {
                _logger.LogWarning($"Đăng nhập thất bại: Tài khoản '{username}' đã bị khóa.");
                return OperationResult<TaiKhoanDto>.Failed(ErrorCodes.ACCOUNT_LOCKED, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
            }

            var storedHash = await _taiKhoanRepository.GetPasswordHashAsync(username);
            if (string.IsNullOrEmpty(storedHash) || !_passwordHasher.VerifyPassword(matKhau, storedHash))
            {
                _logger.LogWarning($"Đăng nhập thất bại: Sai mật khẩu cho tài khoản '{username}'.");
                return OperationResult<TaiKhoanDto>.Failed(ErrorCodes.INVALID_CREDENTIALS, "Tên đăng nhập hoặc mật khẩu không chính xác.");
            }

            var now = DateTime.Now;
            await _taiKhoanRepository.UpdateLanDangNhapCuoiAsync(user.Id, now);
            user.LanDangNhapCuoi = now;

            _currentUserService.SetCurrentUser(user);
            _logger.LogInfo($"Người dùng '{username}' đăng nhập thành công với vai trò '{user.VaiTro}'.");

            return OperationResult<TaiKhoanDto>.Success(user, "Đăng nhập thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi không mong muốn trong quá trình đăng nhập.", ex);
            return OperationResult<TaiKhoanDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi trong hệ thống. Vui lòng thử lại sau.");
        }
    }

    public Task<OperationResult> LogoutAsync()
    {
        try
        {
            var currentUser = _currentUserService.CurrentUser;
            if (currentUser != null)
            {
                _logger.LogInfo($"Người dùng '{currentUser.TenDangNhap}' đã đăng xuất.");
            }

            _currentUserService.Clear();
            return Task.FromResult(OperationResult.Success("Đăng xuất thành công."));
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi trong quá trình đăng xuất.", ex);
            return Task.FromResult(OperationResult.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi khi đăng xuất."));
        }
    }
}
