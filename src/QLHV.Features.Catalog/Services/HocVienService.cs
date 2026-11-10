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

public class HocVienService : IHocVienService
{
    private readonly IHocVienRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public HocVienService(
        IHocVienRepository repository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<HocVienDto>>> GetAllAsync(HocVienFilter? filter = null)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<IReadOnlyList<HocVienDto>>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage,
            Array.Empty<HocVienDto>());
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<HocVienDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách học viên.", ex);
            return OperationResult<IReadOnlyList<HocVienDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách học viên. Vui lòng thử lại sau.", Array.Empty<HocVienDto>());
        }
    }

    public async Task<OperationResult<HocVienDto>> GetByIdAsync(int id)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<HocVienDto>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return OperationResult<HocVienDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy học viên có ID={id}.");
            }
            return OperationResult<HocVienDto>.Success(item);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy học viên ID={id}.", ex);
            return OperationResult<HocVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy thông tin học viên.");
        }
    }

    public async Task<OperationResult<HocVienDto>> CreateAsync(HocVienDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<HocVienDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var validation = await ValidateHocVienAsync(dto, isCreating: true);
            if (!validation.IsSuccess)
            {
                return OperationResult<HocVienDto>.Failed(validation.Error!);
            }

            dto.MaHocVien = dto.MaHocVien.Trim();
            dto.HoTen = dto.HoTen.Trim();
            dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            dto.DienThoai = string.IsNullOrWhiteSpace(dto.DienThoai) ? null : dto.DienThoai.Trim();
            dto.DiaChi = string.IsNullOrWhiteSpace(dto.DiaChi) ? null : dto.DiaChi.Trim();
            dto.GioiTinh = string.IsNullOrWhiteSpace(dto.GioiTinh) ? null : dto.GioiTinh.Trim();

            var created = await _repository.AddAsync(dto);
            _logger.LogInfo($"Đã thêm mới học viên: {created.MaHocVien} - {created.HoTen}");
            return OperationResult<HocVienDto>.Success(created, "Thêm học viên thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tạo mới học viên.", ex);
            return OperationResult<HocVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi thêm học viên.");
        }
    }

    public async Task<OperationResult<HocVienDto>> UpdateAsync(HocVienDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<HocVienDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                return OperationResult<HocVienDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy học viên có ID={dto.Id}.");
            }

            var validation = await ValidateHocVienAsync(dto, isCreating: false);
            if (!validation.IsSuccess)
            {
                return OperationResult<HocVienDto>.Failed(validation.Error!);
            }

            dto.MaHocVien = dto.MaHocVien.Trim();
            dto.HoTen = dto.HoTen.Trim();
            dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            dto.DienThoai = string.IsNullOrWhiteSpace(dto.DienThoai) ? null : dto.DienThoai.Trim();
            dto.DiaChi = string.IsNullOrWhiteSpace(dto.DiaChi) ? null : dto.DiaChi.Trim();
            dto.GioiTinh = string.IsNullOrWhiteSpace(dto.GioiTinh) ? null : dto.GioiTinh.Trim();

            var success = await _repository.UpdateAsync(dto);
            if (!success)
            {
                return OperationResult<HocVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Cập nhật học viên thất bại.");
            }

            _logger.LogInfo($"Đã cập nhật học viên: {dto.MaHocVien} - {dto.HoTen}");
            return OperationResult<HocVienDto>.Success(dto, "Cập nhật thông tin học viên thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi cập nhật học viên ID={dto.Id}.", ex);
            return OperationResult<HocVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi cập nhật học viên.");
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
                return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy học viên có ID={id}.");
            }

            var success = await _repository.SetTrangThaiAsync(id, TrangThaiHoatDong.NgungHoatDong);
            if (success)
            {
                _logger.LogInfo($"Đã ngừng hoạt động học viên: {existing.MaHocVien} - {existing.HoTen}");
                return OperationResult<bool>.Success(true, "Ngừng hoạt động học viên thành công.");
            }

            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể ngừng hoạt động học viên.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi ngừng hoạt động học viên ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi khi ngừng hoạt động học viên.");
        }
    }

    private async Task<OperationResult> ValidateHocVienAsync(HocVienDto dto, bool isCreating)
    {
        if (string.IsNullOrWhiteSpace(dto.MaHocVien))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Mã học viên là bắt buộc.");
        }

        if (dto.MaHocVien.Trim().Length > CatalogFieldLimits.MaHocVien)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Mã học viên không được vượt quá {CatalogFieldLimits.MaHocVien} ký tự.");
        }

        if (string.IsNullOrWhiteSpace(dto.HoTen))
        {
            return OperationResult.Failed(ErrorCodes.REQUIRED, "Họ và tên học viên là bắt buộc.");
        }

        if (dto.HoTen.Trim().Length > CatalogFieldLimits.HoTen)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Họ và tên học viên không được vượt quá {CatalogFieldLimits.HoTen} ký tự.");
        }

        if (!string.IsNullOrWhiteSpace(dto.GioiTinh) && dto.GioiTinh.Trim().Length > CatalogFieldLimits.GioiTinh)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Giới tính không được vượt quá {CatalogFieldLimits.GioiTinh} ký tự.");
        }

        if (!string.IsNullOrWhiteSpace(dto.DienThoai) && dto.DienThoai.Trim().Length > CatalogFieldLimits.DienThoai)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Số điện thoại không được vượt quá {CatalogFieldLimits.DienThoai} ký tự.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email.Trim().Length > CatalogFieldLimits.Email)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Địa chỉ email không được vượt quá {CatalogFieldLimits.Email} ký tự.");
        }

        if (!string.IsNullOrWhiteSpace(dto.DiaChi) && dto.DiaChi.Trim().Length > CatalogFieldLimits.DiaChi)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Địa chỉ không được vượt quá {CatalogFieldLimits.DiaChi} ký tự.");
        }

        // Kiểm tra trùng mã học viên (không phân biệt hoa thường)
        var duplicate = await _repository.GetByMaHocVienAsync(dto.MaHocVien);
        if (duplicate != null && (isCreating || duplicate.Id != dto.Id))
        {
            return OperationResult.Failed(ErrorCodes.DUPLICATE_CODE, $"Mã học viên '{dto.MaHocVien.Trim()}' đã tồn tại trong hệ thống.");
        }

        // Ngày sinh không được lớn hơn ngày hiện tại
        if (dto.NgaySinh.HasValue && dto.NgaySinh.Value.Date > DateTime.Today)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, "Ngày sinh không được lớn hơn ngày hiện tại.");
        }

        // Email hợp lệ nếu có nhập
        if (!string.IsNullOrWhiteSpace(dto.Email) && !ValidationHelper.IsValidEmail(dto.Email))
        {
            return OperationResult.Failed(ErrorCodes.INVALID_FORMAT, "Địa chỉ email không đúng định dạng hợp lệ.");
        }

        // Số điện thoại hợp lệ nếu có nhập
        if (!string.IsNullOrWhiteSpace(dto.DienThoai) && !ValidationHelper.IsValidPhoneNumber(dto.DienThoai))
        {
            return OperationResult.Failed(ErrorCodes.INVALID_FORMAT, "Số điện thoại chỉ được chứa chữ số và các ký tự hợp lệ (+, -, khoảng trắng).");
        }

        return OperationResult.Success();
    }
}
