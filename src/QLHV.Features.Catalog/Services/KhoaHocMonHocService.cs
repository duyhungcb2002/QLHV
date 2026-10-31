using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Catalog.Services;

public class KhoaHocMonHocService : IKhoaHocMonHocService
{
    private readonly IKhoaHocMonHocRepository _repository;
    private readonly IKhoaHocRepository _khoaHocRepository;
    private readonly IMonHocRepository _monHocRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public KhoaHocMonHocService(
        IKhoaHocMonHocRepository repository,
        IKhoaHocRepository khoaHocRepository,
        IMonHocRepository monHocRepository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _khoaHocRepository = khoaHocRepository;
        _monHocRepository = monHocRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<KhoaHocMonHocDto>>> GetByKhoaHocIdAsync(int khoaHocId)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<IReadOnlyList<KhoaHocMonHocDto>>(
            _currentUserService,
            Permission.CatalogView,
            AuthorizationGuard.CatalogViewForbiddenMessage,
            Array.Empty<KhoaHocMonHocDto>());
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var list = await _repository.GetByKhoaHocIdAsync(khoaHocId);
            return OperationResult<IReadOnlyList<KhoaHocMonHocDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi tải môn học theo khóa học ID={khoaHocId}.", ex);
            return OperationResult<IReadOnlyList<KhoaHocMonHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách môn học của khóa học.", Array.Empty<KhoaHocMonHocDto>());
        }
    }

    public async Task<OperationResult<KhoaHocMonHocDto>> AddMonHocAsync(KhoaHocMonHocDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<KhoaHocMonHocDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            if (dto.ThuTu <= 0)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.INVALID_RANGE, "Thứ tự môn học phải lớn hơn 0.");
            }

            if (dto.SoBuoiDuKien <= 0)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.INVALID_RANGE, "Số buổi dự kiến phải lớn hơn 0.");
            }

            var khoaHoc = await _khoaHocRepository.GetByIdAsync(dto.KhoaHocId);
            if (khoaHoc == null)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Khóa học có ID={dto.KhoaHocId} không tồn tại.");
            }

            var monHoc = await _monHocRepository.GetByIdAsync(dto.MonHocId);
            if (monHoc == null)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Môn học có ID={dto.MonHocId} không tồn tại.");
            }

            var existing = await _repository.GetAsync(dto.KhoaHocId, dto.MonHocId);
            if (existing != null)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.DUPLICATE_CODE, "Môn học này đã có trong khóa học, không thể thêm lần thứ hai.");
            }

            dto.MaKhoaHoc = khoaHoc.MaKhoaHoc;
            dto.TenKhoaHoc = khoaHoc.TenKhoaHoc;
            dto.MaMonHoc = monHoc.MaMonHoc;
            dto.TenMonHoc = monHoc.TenMonHoc;

            var created = await _repository.AddAsync(dto);
            _logger.LogInfo($"Đã gán môn '{monHoc.TenMonHoc}' vào khóa học '{khoaHoc.TenKhoaHoc}' (Thứ tự: {dto.ThuTu}, Số buổi: {dto.SoBuoiDuKien}).");
            return OperationResult<KhoaHocMonHocDto>.Success(created, "Thêm môn học vào khóa học thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi thêm môn học vào khóa học.", ex);
            return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi hệ thống khi thêm môn học vào khóa học.");
        }
    }

    public async Task<OperationResult<bool>> RemoveMonHocAsync(int khoaHocId, int monHocId)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<bool>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage,
            false);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            var existing = await _repository.GetAsync(khoaHocId, monHocId);
            if (existing == null)
            {
                return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy thông tin môn học này trong khóa học.");
            }

            var success = await _repository.DeleteAsync(khoaHocId, monHocId);
            if (success)
            {
                _logger.LogInfo($"Đã xóa môn học ID={monHocId} khỏi khóa học ID={khoaHocId}.");
                return OperationResult<bool>.Success(true, "Xóa môn học khỏi khóa học thành công.");
            }

            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể xóa môn học khỏi khóa học.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi xóa môn ID={monHocId} khỏi khóa ID={khoaHocId}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi khi xóa môn học khỏi khóa học.");
        }
    }

    public async Task<OperationResult<KhoaHocMonHocDto>> UpdateThuTuSoBuoiAsync(KhoaHocMonHocDto dto)
    {
        var authCheck = AuthorizationGuard.EnsurePermission<KhoaHocMonHocDto>(
            _currentUserService,
            Permission.CatalogManage,
            AuthorizationGuard.CatalogManageForbiddenMessage);
        if (!authCheck.IsSuccess) return authCheck;

        try
        {
            if (dto.ThuTu <= 0)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.INVALID_RANGE, "Thứ tự môn học phải lớn hơn 0.");
            }

            if (dto.SoBuoiDuKien <= 0)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.INVALID_RANGE, "Số buổi dự kiến phải lớn hơn 0.");
            }

            var existing = await _repository.GetAsync(dto.KhoaHocId, dto.MonHocId);
            if (existing == null)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy thông tin môn học trong khóa học.");
            }

            var success = await _repository.UpdateAsync(dto);
            if (!success)
            {
                return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Cập nhật thông tin thất bại.");
            }

            _logger.LogInfo($"Đã cập nhật thứ tự/số buổi môn ID={dto.MonHocId} trong khóa ID={dto.KhoaHocId}.");
            return OperationResult<KhoaHocMonHocDto>.Success(dto, "Cập nhật thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi cập nhật thứ tự môn học trong khóa học.", ex);
            return OperationResult<KhoaHocMonHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Đã xảy ra lỗi khi cập nhật.");
        }
    }
}
