using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;

namespace QLHV.Core.Security;

/// <summary>
/// Quản lý phân quyền tập trung theo ma trận quyền hạn
/// </summary>
public interface IAuthorizationService
{
    bool HasPermission(VaiTro role, Permission permission);
    IReadOnlyCollection<Permission> GetPermissions(VaiTro role);
}

/// <summary>
/// Triển khai ma trận quyền chuẩn cho các vai trò trong hệ thống QLHV
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private static readonly Dictionary<VaiTro, HashSet<Permission>> RolePermissionMap = new()
    {
        [VaiTro.ADMIN] = new HashSet<Permission>
        {
            Permission.CatalogView,
            Permission.CatalogManage,
            Permission.TrainingView,
            Permission.TrainingManage,
            Permission.TuitionManage,
            Permission.GradeEntry,
            Permission.ReportView,
            Permission.UserManage
        },

        [VaiTro.DAOTAO] = new HashSet<Permission>
        {
            Permission.CatalogView,
            Permission.CatalogManage,
            Permission.TrainingView,
            Permission.TrainingManage
        },

        [VaiTro.THUNGAN] = new HashSet<Permission>
        {
            Permission.CatalogView,
            Permission.TuitionManage
        },

        [VaiTro.GIANGVIEN] = new HashSet<Permission>
        {
            Permission.CatalogView,
            Permission.GradeEntry
        },

        [VaiTro.QUANLY] = new HashSet<Permission>
        {
            Permission.CatalogView,
            Permission.ReportView
        }
    };

    public bool HasPermission(VaiTro role, Permission permission)
    {
        if (RolePermissionMap.TryGetValue(role, out var permissions))
        {
            return permissions.Contains(permission);
        }
        return false;
    }

    public IReadOnlyCollection<Permission> GetPermissions(VaiTro role)
    {
        if (RolePermissionMap.TryGetValue(role, out var permissions))
        {
            return permissions;
        }
        return Array.Empty<Permission>();
    }
}

/// <summary>
/// Quản lý phiên làm việc của người dùng hiện tại
/// </summary>
public interface ICurrentUserService
{
    TaiKhoanDto? CurrentUser { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(Permission permission);
    void SetCurrentUser(TaiKhoanDto user);
    void Clear();
}

/// <summary>
/// Triển khai lưu trữ phiên người dùng trong bộ nhớ ứng dụng WinForms
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IAuthorizationService _authorizationService;
    private TaiKhoanDto? _currentUser;

    public CurrentUserService(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public TaiKhoanDto? CurrentUser => _currentUser;

    public bool IsAuthenticated => _currentUser != null;

    public bool HasPermission(Permission permission)
    {
        if (_currentUser == null) return false;
        return _authorizationService.HasPermission(_currentUser.VaiTro, permission);
    }

    public void SetCurrentUser(TaiKhoanDto user)
    {
        _currentUser = user ?? throw new ArgumentNullException(nameof(user));
    }

    public void Clear()
    {
        _currentUser = null;
    }
}
