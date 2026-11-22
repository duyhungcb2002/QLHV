using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Training.Services;

public class DangKyLopHocService : IDangKyLopHocService
{
    private readonly IDangKyLopHocRepository _repository;
    private readonly ILopHocRepository _lopHocRepository;
    private readonly IHocVienRepository _hocVienRepository;
    private readonly IKhoaHocRepository _khoaHocRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public DangKyLopHocService(
        IDangKyLopHocRepository repository,
        ILopHocRepository lopHocRepository,
        IHocVienRepository hocVienRepository,
        IKhoaHocRepository khoaHocRepository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _lopHocRepository = lopHocRepository;
        _hocVienRepository = hocVienRepository;
        _khoaHocRepository = khoaHocRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<DangKyLopHocDto>>> GetAllAsync(DangKyLopHocFilter? filter = null)
    {
        // Cho phép cả TrainingView lẫn TuitionManage (phục vụ thu ngân xem công nợ / đăng ký)
        var user = _currentUserService.CurrentUser;
        bool hasViewPermission = _currentUserService.HasPermission(Permission.TrainingView) ||
                                 _currentUserService.HasPermission(Permission.TuitionManage);

        if (!hasViewPermission)
        {
            return OperationResult<IReadOnlyList<DangKyLopHocDto>>.Failed(
                ErrorCodes.FORBIDDEN, "Bạn không có quyền xem thông tin đăng ký lớp học.", Array.Empty<DangKyLopHocDto>());
        }

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<DangKyLopHocDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách học viên đăng ký lớp.", ex);
            return OperationResult<IReadOnlyList<DangKyLopHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách đăng ký lớp.", Array.Empty<DangKyLopHocDto>());
        }
    }

    public async Task<OperationResult<DangKyLopHocDto>> EnrollAsync(int lopHocId, int hocVienId, string? ghiChu = null)
    {
        var auth = AuthorizationGuard.EnsurePermission<DangKyLopHocDto>(
            _currentUserService, Permission.TrainingManage, "Bạn không có quyền quản lý đào tạo.");
        if (!auth.IsSuccess) return auth;

        if (lopHocId <= 0)
            return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng chọn lớp học hợp lệ.");

        if (hocVienId <= 0)
            return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng chọn học viên hợp lệ.");

        if (!string.IsNullOrWhiteSpace(ghiChu) && ghiChu.Trim().Length > 250)
            return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Ghi chú không được vượt quá 250 ký tự.");

        try
        {
            var hocVien = await _hocVienRepository.GetByIdAsync(hocVienId);
            if (hocVien == null)
                return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy học viên.");

            if (hocVien.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Học viên này đã ngừng hoạt động.");

            var lopHoc = await _lopHocRepository.GetByIdAsync(lopHocId);
            if (lopHoc == null)
                return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy lớp học được chọn.");

            if (lopHoc.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động.");

            // Lấy học phí niêm yết từ Khóa học làm snapshot
            var khoaHoc = await _khoaHocRepository.GetByIdAsync(lopHoc.KhoaHocId);
            if (khoaHoc == null)
                return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy khóa học của lớp.");

            if (khoaHoc.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Khóa học của lớp học này đã ngừng hoạt động.");

            decimal hocPhiSnapshot = khoaHoc.HocPhiNiemYet;

            // Dùng transaction nguyên tử có khóa hàng để chống race condition vượt sĩ số
            var result = await _repository.EnrollAtomicAsync(
                lopHocId, hocVienId, hocPhiSnapshot, ghiChu?.Trim(),
                hocVien.MaHocVien, hocVien.HoTen, hocVien.DienThoai);

            if (!result.Success)
            {
                return OperationResult<DangKyLopHocDto>.Failed(result.ErrorCode ?? ErrorCodes.BUSINESS_RULE_VIOLATION, result.ErrorMessage ?? "Không thể xếp học viên vào lớp.");
            }

            return OperationResult<DangKyLopHocDto>.Success(result.Data!);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi xếp học viên ID={hocVienId} vào lớp ID={lopHocId}.", ex);
            return OperationResult<DangKyLopHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể xếp học viên vào lớp.");
        }
    }

    public async Task<OperationResult<bool>> CancelEnrollmentAsync(int id)
    {
        var auth = AuthorizationGuard.EnsurePermission<bool>(
            _currentUserService, Permission.TrainingManage, "Bạn không có quyền quản lý đào tạo.", false);
        if (!auth.IsSuccess) return auth;

        try
        {
            // Dùng transaction nguyên tử kiểm tra không có phiếu thu và hủy đăng ký
            var result = await _repository.CancelEnrollmentAtomicAsync(id);
            if (!result.Success)
            {
                return OperationResult<bool>.Failed(result.ErrorCode ?? ErrorCodes.BUSINESS_RULE_VIOLATION, result.ErrorMessage ?? "Không thể hủy xếp lớp học viên.", false);
            }

            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi hủy đăng ký lớp ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể hủy xếp lớp học viên.", false);
        }
    }

    public async Task<OperationResult<IReadOnlyList<CongNoHocPhiDto>>> GetCongNoByLopHocAsync(int lopHocId)
    {
        // Cho phép cả TrainingView lẫn TuitionManage (Thu ngân truy vấn công nợ theo lớp)
        bool hasViewPermission = _currentUserService.HasPermission(Permission.TrainingView) ||
                                 _currentUserService.HasPermission(Permission.TuitionManage);

        if (!hasViewPermission)
        {
            return OperationResult<IReadOnlyList<CongNoHocPhiDto>>.Failed(
                ErrorCodes.FORBIDDEN, "Bạn không có quyền xem thông tin công nợ học phí.", Array.Empty<CongNoHocPhiDto>());
        }

        try
        {
            var list = await _repository.GetCongNoByLopHocAsync(lopHocId);
            return OperationResult<IReadOnlyList<CongNoHocPhiDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy dữ liệu công nợ lớp ID={lopHocId}.", ex);
            return OperationResult<IReadOnlyList<CongNoHocPhiDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy danh sách công nợ lớp học.", Array.Empty<CongNoHocPhiDto>());
        }
    }
}
