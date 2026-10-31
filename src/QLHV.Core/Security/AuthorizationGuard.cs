using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;

namespace QLHV.Core.Security;

/// <summary>
/// Tiện ích kiểm tra phân quyền tập trung tại tầng Service
/// Đảm bảo người dùng đã đăng nhập và có đúng Permission tương ứng trước khi thao tác dữ liệu
/// </summary>
public static class AuthorizationGuard
{
    public const string NotAuthenticatedMessage = "Bạn cần đăng nhập để thực hiện chức năng này.";
    public const string CatalogViewForbiddenMessage = "Bạn không có quyền xem danh mục này.";
    public const string CatalogManageForbiddenMessage = "Bạn không có quyền thay đổi dữ liệu danh mục.";

    /// <summary>
    /// Kiểm tra quyền hạn của người dùng hiện tại (không trả về dữ liệu)
    /// </summary>
    public static OperationResult EnsurePermission(
        ICurrentUserService currentUserService,
        Permission requiredPermission,
        string forbiddenMessage)
    {
        if (currentUserService == null || !currentUserService.IsAuthenticated)
        {
            return OperationResult.Failed(ErrorCodes.FORBIDDEN, NotAuthenticatedMessage);
        }

        if (!currentUserService.HasPermission(requiredPermission))
        {
            return OperationResult.Failed(ErrorCodes.FORBIDDEN, forbiddenMessage);
        }

        return OperationResult.Success();
    }

    /// <summary>
    /// Kiểm tra quyền hạn của người dùng hiện tại (trả về kiểu dữ liệu generic T)
    /// </summary>
    public static OperationResult<T> EnsurePermission<T>(
        ICurrentUserService currentUserService,
        Permission requiredPermission,
        string forbiddenMessage,
        T? defaultData = default)
    {
        if (currentUserService == null || !currentUserService.IsAuthenticated)
        {
            return OperationResult<T>.Failed(ErrorCodes.FORBIDDEN, NotAuthenticatedMessage, defaultData);
        }

        if (!currentUserService.HasPermission(requiredPermission))
        {
            return OperationResult<T>.Failed(ErrorCodes.FORBIDDEN, forbiddenMessage, defaultData);
        }

        return OperationResult<T>.Success(defaultData!, string.Empty);
    }
}
