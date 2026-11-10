using QLHV.Contracts.Constants;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Catalog.Services;

public class PhongHocService : IPhongHocService
{
    private readonly IPhongHocRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public PhongHocService(
        IPhongHocRepository repository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<PhongHocDto>>> GetAllAsync(PhongHocFilter? filter = null)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<IReadOnlyList<PhongHocDto>>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage,
            Array.Empty<PhongHocDto>());
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<PhongHocDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách phòng học.", ex);
            return OperationResult<IReadOnlyList<PhongHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách phòng học.", Array.Empty<PhongHocDto>());
        }
    }

    public async Task<OperationResult<PhongHocDto>> GetByIdAsync(int id)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<PhongHocDto>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return OperationResult<PhongHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy phòng học có ID={id}.");
            }
            return OperationResult<PhongHocDto>.Success(item);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy thông tin phòng học ID={id}.", ex);
            return OperationResult<PhongHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy thông tin phòng học.");
        }
    }

    public async Task<OperationResult<PhongHocDto>> CreateAsync(PhongHocDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<PhongHocDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var validation = await ValidatePhongHocAsync(dto, isCreating: true);
            if (!validation.IsSuccess)
            {
                return OperationResult<PhongHocDto>.Failed(validation.Error!);
            }

            dto.MaPhong = dto.MaPhong.Trim();
            dto.TenPhong = dto.TenPhong.Trim();

            var created = await _repository.AddAsync(dto);
            _logger.LogInfo($"Đã thêm mới phòng học: {created.MaPhong} - {created.TenPhong}");
            return OperationResult<PhongHocDto>.Success(created, "Thêm phòng học thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tạo mới phòng học.", ex);
            return OperationResult<PhongHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi thêm phòng học.");
        }
    }

    public async Task<OperationResult<PhongHocDto>> UpdateAsync(PhongHocDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<PhongHocDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                return OperationResult<PhongHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy phòng học có ID={dto.Id}.");
            }

            var validation = await ValidatePhongHocAsync(dto, isCreating: false);
            if (!validation.IsSuccess)
            {
                return OperationResult<PhongHocDto>.Failed(validation.Error!);
            }

            dto.MaPhong = dto.MaPhong.Trim();
            dto.TenPhong = dto.TenPhong.Trim();

            var success = await _repository.UpdateAsync(dto);
            if (!success)
            {
                return OperationResult<PhongHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Cập nhật phòng học thất bại.");
            }

            _logger.LogInfo($"Đã cập nhật phòng học: {dto.MaPhong} - {dto.TenPhong}");
            return OperationResult<PhongHocDto>.Success(dto, "Cập nhật thông tin phòng học thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi cập nhật phòng học ID={dto.Id}.", ex);
            return OperationResult<PhongHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi cập nhật phòng học.");
        }
    }

    public async Task<OperationResult<bool>> DeactivateAsync(int id)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<bool>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage,
            false);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy phòng học có ID={id}.");
            }

            var success = await _repository.SetTrangThaiAsync(id, TrangThaiHoatDong.NgungHoatDong);
            if (success)
            {
                _logger.LogInfo($"Đã ngừng hoạt động phòng học: {existing.MaPhong} - {existing.TenPhong}");
                return OperationResult<bool>.Success(true, "Ngừng hoạt động phòng học thành công.");
            }

            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể ngừng hoạt động phòng học.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi ngừng hoạt động phòng học ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi khi ngừng hoạt động phòng học.");
        }
    }

    private async Task<OperationResult> ValidatePhongHocAsync(PhongHocDto dto, bool isCreating)
    {
        if (string.IsNullOrWhiteSpace(dto.MaPhong))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Mã phòng học là bắt buộc.");
        }

        if (dto.MaPhong.Trim().Length > CatalogFieldLimits.MaPhong)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Mã phòng học không được vượt quá {CatalogFieldLimits.MaPhong} ký tự.");
        }

        if (string.IsNullOrWhiteSpace(dto.TenPhong))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Tên phòng học là bắt buộc.");
        }

        if (dto.TenPhong.Trim().Length > CatalogFieldLimits.TenPhong)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Tên phòng học không được vượt quá {CatalogFieldLimits.TenPhong} ký tự.");
        }

        if (dto.SucChua <= 0)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, "Sức chứa phòng học phải lớn hơn 0.");
        }

        var duplicate = await _repository.GetByMaPhongAsync(dto.MaPhong);
        if (duplicate != null && (isCreating || duplicate.Id != dto.Id))
        {
            return OperationResult.Failed(ErrorCodes.DUPLICATE_CODE, $"Mã phòng '{dto.MaPhong.Trim()}' đã tồn tại trong hệ thống.");
        }

        return OperationResult.Success();
    }
}
