using QLHV.Contracts.Constants;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;
using QLHV.Core.Validation;

namespace QLHV.Features.Catalog.Services;

public class GiangVienService : IGiangVienService
{
    private readonly IGiangVienRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public GiangVienService(
        IGiangVienRepository repository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<GiangVienDto>>> GetAllAsync(GiangVienFilter? filter = null)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<IReadOnlyList<GiangVienDto>>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage,
            Array.Empty<GiangVienDto>());
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<GiangVienDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách giảng viên.", ex);
            return OperationResult<IReadOnlyList<GiangVienDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách giảng viên.", Array.Empty<GiangVienDto>());
        }
    }

    public async Task<OperationResult<GiangVienDto>> GetByIdAsync(int id)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<GiangVienDto>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return OperationResult<GiangVienDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy giảng viên có ID={id}.");
            }
            return OperationResult<GiangVienDto>.Success(item);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy thông tin giảng viên ID={id}.", ex);
            return OperationResult<GiangVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy thông tin giảng viên.");
        }
    }

    public async Task<OperationResult<GiangVienDto>> CreateAsync(GiangVienDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<GiangVienDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var validation = await ValidateGiangVienAsync(dto, isCreating: true);
            if (!validation.IsSuccess)
            {
                return OperationResult<GiangVienDto>.Failed(validation.Error!);
            }

            dto.MaGiangVien = dto.MaGiangVien.Trim();
            dto.HoTen = dto.HoTen.Trim();
            dto.ChuyenMon = string.IsNullOrWhiteSpace(dto.ChuyenMon) ? null : dto.ChuyenMon.Trim();
            dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            dto.DienThoai = string.IsNullOrWhiteSpace(dto.DienThoai) ? null : dto.DienThoai.Trim();

            var created = await _repository.AddAsync(dto);
            _logger.LogInfo($"Đã thêm mới giảng viên: {created.MaGiangVien} - {created.HoTen}");
            return OperationResult<GiangVienDto>.Success(created, "Thêm giảng viên thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tạo mới giảng viên.", ex);
            return OperationResult<GiangVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi thêm giảng viên.");
        }
    }

    public async Task<OperationResult<GiangVienDto>> UpdateAsync(GiangVienDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<GiangVienDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                return OperationResult<GiangVienDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy giảng viên có ID={dto.Id}.");
            }

            var validation = await ValidateGiangVienAsync(dto, isCreating: false);
            if (!validation.IsSuccess)
            {
                return OperationResult<GiangVienDto>.Failed(validation.Error!);
            }

            dto.MaGiangVien = dto.MaGiangVien.Trim();
            dto.HoTen = dto.HoTen.Trim();
            dto.ChuyenMon = string.IsNullOrWhiteSpace(dto.ChuyenMon) ? null : dto.ChuyenMon.Trim();
            dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            dto.DienThoai = string.IsNullOrWhiteSpace(dto.DienThoai) ? null : dto.DienThoai.Trim();

            var success = await _repository.UpdateAsync(dto);
            if (!success)
            {
                return OperationResult<GiangVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Cập nhật giảng viên thất bại.");
            }

            _logger.LogInfo($"Đã cập nhật giảng viên: {dto.MaGiangVien} - {dto.HoTen}");
            return OperationResult<GiangVienDto>.Success(dto, "Cập nhật thông tin giảng viên thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi cập nhật giảng viên ID={dto.Id}.", ex);
            return OperationResult<GiangVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi cập nhật giảng viên.");
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
                return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy giảng viên có ID={id}.");
            }

            var success = await _repository.SetTrangThaiAsync(id, TrangThaiHoatDong.NgungHoatDong);
            if (success)
            {
                _logger.LogInfo($"Đã ngừng hoạt động giảng viên: {existing.MaGiangVien} - {existing.HoTen}");
                return OperationResult<bool>.Success(true, "Ngừng hoạt động giảng viên thành công.");
            }

            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể ngừng hoạt động giảng viên.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi ngừng hoạt động giảng viên ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi khi ngừng hoạt động giảng viên.");
        }
    }

    private async Task<OperationResult> ValidateGiangVienAsync(GiangVienDto dto, bool isCreating)
    {
        if (string.IsNullOrWhiteSpace(dto.MaGiangVien))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Mã giảng viên là bắt buộc.");
        }

        if (dto.MaGiangVien.Trim().Length > CatalogFieldLimits.MaGiangVien)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Mã giảng viên không được vượt quá {CatalogFieldLimits.MaGiangVien} ký tự.");
        }

        if (string.IsNullOrWhiteSpace(dto.HoTen))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Họ và tên giảng viên là bắt buộc.");
        }

        if (dto.HoTen.Trim().Length > CatalogFieldLimits.HoTen)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Họ và tên giảng viên không được vượt quá {CatalogFieldLimits.HoTen} ký tự.");
        }

        if (!string.IsNullOrWhiteSpace(dto.ChuyenMon) && dto.ChuyenMon.Trim().Length > CatalogFieldLimits.ChuyenMon)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Chuyên môn không được vượt quá {CatalogFieldLimits.ChuyenMon} ký tự.");
        }

        if (!string.IsNullOrWhiteSpace(dto.DienThoai) && dto.DienThoai.Trim().Length > CatalogFieldLimits.DienThoai)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Số điện thoại không được vượt quá {CatalogFieldLimits.DienThoai} ký tự.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email.Trim().Length > CatalogFieldLimits.Email)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Địa chỉ email không được vượt quá {CatalogFieldLimits.Email} ký tự.");
        }

        // Kiểm tra trùng mã
        var duplicate = await _repository.GetByMaGiangVienAsync(dto.MaGiangVien);
        if (duplicate != null && (isCreating || duplicate.Id != dto.Id))
        {
            return OperationResult.Failed(ErrorCodes.DUPLICATE_CODE, $"Mã giảng viên '{dto.MaGiangVien.Trim()}' đã tồn tại trong hệ thống.");
        }

        // Email hợp lệ
        if (!string.IsNullOrWhiteSpace(dto.Email) && !ValidationHelper.IsValidEmail(dto.Email))
        {
            return OperationResult.Failed(ErrorCodes.INVALID_FORMAT, "Địa chỉ email không đúng định dạng hợp lệ.");
        }

        // Số điện thoại hợp lệ
        if (!string.IsNullOrWhiteSpace(dto.DienThoai) && !ValidationHelper.IsValidPhoneNumber(dto.DienThoai))
        {
            return OperationResult.Failed(ErrorCodes.INVALID_FORMAT, "Số điện thoại chỉ được chứa chữ số và các ký tự hợp lệ (+, -, khoảng trắng).");
        }

        return OperationResult.Success();
    }
}
