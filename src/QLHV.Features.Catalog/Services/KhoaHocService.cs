using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Catalog.Services;

public class KhoaHocService : IKhoaHocService
{
    private readonly IKhoaHocRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public KhoaHocService(
        IKhoaHocRepository repository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<KhoaHocDto>>> GetAllAsync(KhoaHocFilter? filter = null)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<IReadOnlyList<KhoaHocDto>>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage,
            Array.Empty<KhoaHocDto>());
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<KhoaHocDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách khóa học.", ex);
            return OperationResult<IReadOnlyList<KhoaHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách khóa học.", Array.Empty<KhoaHocDto>());
        }
    }

    public async Task<OperationResult<KhoaHocDto>> GetByIdAsync(int id)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<KhoaHocDto>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return OperationResult<KhoaHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy khóa học có ID={id}.");
            }
            return OperationResult<KhoaHocDto>.Success(item);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy thông tin khóa học ID={id}.", ex);
            return OperationResult<KhoaHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy thông tin khóa học.");
        }
    }

    public async Task<OperationResult<KhoaHocDto>> CreateAsync(KhoaHocDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<KhoaHocDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var validation = await ValidateKhoaHocAsync(dto, isCreating: true);
            if (!validation.IsSuccess)
            {
                return OperationResult<KhoaHocDto>.Failed(validation.Error!);
            }

            dto.MaKhoaHoc = dto.MaKhoaHoc.Trim();
            dto.TenKhoaHoc = dto.TenKhoaHoc.Trim();
            dto.MoTa = string.IsNullOrWhiteSpace(dto.MoTa) ? null : dto.MoTa.Trim();

            var created = await _repository.AddAsync(dto);
            _logger.LogInfo($"Đã thêm mới khóa học: {created.MaKhoaHoc} - {created.TenKhoaHoc}");
            return OperationResult<KhoaHocDto>.Success(created, "Thêm khóa học thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tạo mới khóa học.", ex);
            return OperationResult<KhoaHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi thêm khóa học.");
        }
    }

    public async Task<OperationResult<KhoaHocDto>> UpdateAsync(KhoaHocDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<KhoaHocDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                return OperationResult<KhoaHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy khóa học có ID={dto.Id}.");
            }

            var validation = await ValidateKhoaHocAsync(dto, isCreating: false);
            if (!validation.IsSuccess)
            {
                return OperationResult<KhoaHocDto>.Failed(validation.Error!);
            }

            dto.MaKhoaHoc = dto.MaKhoaHoc.Trim();
            dto.TenKhoaHoc = dto.TenKhoaHoc.Trim();
            dto.MoTa = string.IsNullOrWhiteSpace(dto.MoTa) ? null : dto.MoTa.Trim();

            var success = await _repository.UpdateAsync(dto);
            if (!success)
            {
                return OperationResult<KhoaHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Cập nhật khóa học thất bại.");
            }

            _logger.LogInfo($"Đã cập nhật khóa học: {dto.MaKhoaHoc} - {dto.TenKhoaHoc}");
            return OperationResult<KhoaHocDto>.Success(dto, "Cập nhật thông tin khóa học thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi cập nhật khóa học ID={dto.Id}.", ex);
            return OperationResult<KhoaHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi cập nhật khóa học.");
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
                return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy khóa học có ID={id}.");
            }

            var success = await _repository.SetTrangThaiAsync(id, TrangThaiHoatDong.NgungHoatDong);
            if (success)
            {
                _logger.LogInfo($"Đã ngừng hoạt động khóa học: {existing.MaKhoaHoc} - {existing.TenKhoaHoc}");
                return OperationResult<bool>.Success(true, "Ngừng hoạt động khóa học thành công.");
            }

            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể ngừng hoạt động khóa học.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi ngừng hoạt động khóa học ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi khi ngừng hoạt động khóa học.");
        }
    }

    private async Task<OperationResult> ValidateKhoaHocAsync(KhoaHocDto dto, bool isCreating)
    {
        if (string.IsNullOrWhiteSpace(dto.MaKhoaHoc))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Mã khóa học là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(dto.TenKhoaHoc))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Tên khóa học là bắt buộc.");
        }

        if (dto.HocPhiNiemYet < 0)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, "Học phí niêm yết không được âm.");
        }

        var duplicate = await _repository.GetByMaKhoaHocAsync(dto.MaKhoaHoc);
        if (duplicate != null && (isCreating || duplicate.Id != dto.Id))
        {
            return OperationResult.Failed(ErrorCodes.DUPLICATE_CODE, $"Mã khóa học '{dto.MaKhoaHoc.Trim()}' đã tồn tại trong hệ thống.");
        }

        return OperationResult.Success();
    }
}
