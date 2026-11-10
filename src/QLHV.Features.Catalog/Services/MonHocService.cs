using QLHV.Contracts.Constants;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Catalog.Services;

public class MonHocService : IMonHocService
{
    private readonly IMonHocRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public MonHocService(
        IMonHocRepository repository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<MonHocDto>>> GetAllAsync(MonHocFilter? filter = null)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<IReadOnlyList<MonHocDto>>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage,
            Array.Empty<MonHocDto>());
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<MonHocDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách môn học.", ex);
            return OperationResult<IReadOnlyList<MonHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách môn học.", Array.Empty<MonHocDto>());
        }
    }

    public async Task<OperationResult<MonHocDto>> GetByIdAsync(int id)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<MonHocDto>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return OperationResult<MonHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy môn học có ID={id}.");
            }
            return OperationResult<MonHocDto>.Success(item);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy thông tin môn học ID={id}.", ex);
            return OperationResult<MonHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy thông tin môn học.");
        }
    }

    public async Task<OperationResult<MonHocDto>> CreateAsync(MonHocDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<MonHocDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var validation = await ValidateMonHocAsync(dto, isCreating: true);
            if (!validation.IsSuccess)
            {
                return OperationResult<MonHocDto>.Failed(validation.Error!);
            }

            dto.MaMonHoc = dto.MaMonHoc.Trim();
            dto.TenMonHoc = dto.TenMonHoc.Trim();
            dto.MoTa = string.IsNullOrWhiteSpace(dto.MoTa) ? null : dto.MoTa.Trim();

            var created = await _repository.AddAsync(dto);
            _logger.LogInfo($"Đã thêm mới môn học: {created.MaMonHoc} - {created.TenMonHoc}");
            return OperationResult<MonHocDto>.Success(created, "Thêm môn học thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tạo mới môn học.", ex);
            return OperationResult<MonHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi thêm môn học.");
        }
    }

    public async Task<OperationResult<MonHocDto>> UpdateAsync(MonHocDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<MonHocDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                return OperationResult<MonHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy môn học có ID={dto.Id}.");
            }

            var validation = await ValidateMonHocAsync(dto, isCreating: false);
            if (!validation.IsSuccess)
            {
                return OperationResult<MonHocDto>.Failed(validation.Error!);
            }

            dto.MaMonHoc = dto.MaMonHoc.Trim();
            dto.TenMonHoc = dto.TenMonHoc.Trim();
            dto.MoTa = string.IsNullOrWhiteSpace(dto.MoTa) ? null : dto.MoTa.Trim();

            var success = await _repository.UpdateAsync(dto);
            if (!success)
            {
                return OperationResult<MonHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Cập nhật môn học thất bại.");
            }

            _logger.LogInfo($"Đã cập nhật môn học: {dto.MaMonHoc} - {dto.TenMonHoc}");
            return OperationResult<MonHocDto>.Success(dto, "Cập nhật thông tin môn học thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi cập nhật môn học ID={dto.Id}.", ex);
            return OperationResult<MonHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi cập nhật môn học.");
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
                return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy môn học có ID={id}.");
            }

            var success = await _repository.SetTrangThaiAsync(id, TrangThaiHoatDong.NgungHoatDong);
            if (success)
            {
                _logger.LogInfo($"Đã ngừng hoạt động môn học: {existing.MaMonHoc} - {existing.TenMonHoc}");
                return OperationResult<bool>.Success(true, "Ngừng hoạt động môn học thành công.");
            }

            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể ngừng hoạt động môn học.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi ngừng hoạt động môn học ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi khi ngừng hoạt động môn học.");
        }
    }

    private async Task<OperationResult> ValidateMonHocAsync(MonHocDto dto, bool isCreating)
    {
        if (string.IsNullOrWhiteSpace(dto.MaMonHoc))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Mã môn học là bắt buộc.");
        }

        if (dto.MaMonHoc.Trim().Length > CatalogFieldLimits.MaMonHoc)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Mã môn học không được vượt quá {CatalogFieldLimits.MaMonHoc} ký tự.");
        }

        if (string.IsNullOrWhiteSpace(dto.TenMonHoc))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Tên môn học là bắt buộc.");
        }

        if (dto.TenMonHoc.Trim().Length > CatalogFieldLimits.TenMonHoc)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Tên môn học không được vượt quá {CatalogFieldLimits.TenMonHoc} ký tự.");
        }

        if (!string.IsNullOrWhiteSpace(dto.MoTa) && dto.MoTa.Trim().Length > CatalogFieldLimits.MoTa)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Mô tả môn học không được vượt quá {CatalogFieldLimits.MoTa} ký tự.");
        }

        if (dto.SoBuoiMacDinh <= 0)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, "Số buổi mặc định phải lớn hơn 0.");
        }

        var duplicate = await _repository.GetByMaMonHocAsync(dto.MaMonHoc);
        if (duplicate != null && (isCreating || duplicate.Id != dto.Id))
        {
            return OperationResult.Failed(ErrorCodes.DUPLICATE_CODE, $"Mã môn học '{dto.MaMonHoc.Trim()}' đã tồn tại trong hệ thống.");
        }

        return OperationResult.Success();
    }
}
