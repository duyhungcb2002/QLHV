using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Training.Services;

public class HocPhiService : IHocPhiService
{
    private readonly IPhieuThuRepository _repository;
    private readonly IDangKyLopHocRepository _dangKyRepository;
    private readonly ILopHocRepository _lopHocRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public HocPhiService(
        IPhieuThuRepository repository,
        IDangKyLopHocRepository dangKyRepository,
        ILopHocRepository lopHocRepository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _dangKyRepository = dangKyRepository;
        _lopHocRepository = lopHocRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<LopHocDto>>> GetLopHocOptionsAsync()
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<LopHocDto>>(
            _currentUserService, Permission.TuitionManage, "Bạn không có quyền quản lý học phí.", Array.Empty<LopHocDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var lops = await _lopHocRepository.GetAllAsync(new LopHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
            return OperationResult<IReadOnlyList<LopHocDto>>.Success(lops);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách lớp học cho màn hình học phí.", ex);
            return OperationResult<IReadOnlyList<LopHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách lớp học.", Array.Empty<LopHocDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<PhieuThuDto>>> GetAllAsync(PhieuThuFilter? filter = null)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<PhieuThuDto>>(
            _currentUserService, Permission.TuitionManage, "Bạn không có quyền quản lý học phí.", Array.Empty<PhieuThuDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<PhieuThuDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách phiếu thu.", ex);
            return OperationResult<IReadOnlyList<PhieuThuDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách phiếu thu.", Array.Empty<PhieuThuDto>());
        }
    }

    public async Task<OperationResult<PhieuThuDto>> GetByIdAsync(int id)
    {
        var auth = AuthorizationGuard.EnsurePermission<PhieuThuDto>(
            _currentUserService, Permission.TuitionManage, "Bạn không có quyền quản lý học phí.");
        if (!auth.IsSuccess) return auth;

        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return OperationResult<PhieuThuDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy phiếu thu ID={id}.");

            return OperationResult<PhieuThuDto>.Success(item);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy thông tin phiếu thu ID={id}.", ex);
            return OperationResult<PhieuThuDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy thông tin phiếu thu.");
        }
    }

    public async Task<OperationResult<PhieuThuDto>> ThuHocPhiAsync(int dangKyLopHocId, decimal soTien, HinhThucThanhToan hinhThuc, string? ghiChu = null)
    {
        var auth = AuthorizationGuard.EnsurePermission<PhieuThuDto>(
            _currentUserService, Permission.TuitionManage, "Bạn không có quyền lập phiếu thu học phí.");
        if (!auth.IsSuccess) return auth;

        if (soTien <= 0)
            return OperationResult<PhieuThuDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Số tiền thu phải lớn hơn 0 VNĐ.");

        if (!string.IsNullOrWhiteSpace(ghiChu) && ghiChu.Trim().Length > 250)
            return OperationResult<PhieuThuDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Ghi chú phiếu thu không được vượt quá 250 ký tự.");

        try
        {
            var dk = await _dangKyRepository.GetByIdAsync(dangKyLopHocId);
            if (dk == null)
                return OperationResult<PhieuThuDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy thông tin đăng ký lớp học.");

            if (dk.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<PhieuThuDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể thu học phí vì học viên đã hủy hoặc ngừng hoạt động trong lớp.");

            var user = _currentUserService.CurrentUser;
            if (user == null)
                return OperationResult<PhieuThuDto>.Failed(ErrorCodes.UNAUTHORIZED, "Người dùng chưa đăng nhập hệ thống.");

            int currentUserId = user.Id;

            // Sử dụng repository operation nguyên tử có transaction và khóa Serializable / UPDLOCK
            var result = await _repository.ThuHocPhiAtomicAsync(dangKyLopHocId, soTien, hinhThuc, currentUserId, ghiChu?.Trim());
            if (!result.Success)
            {
                return OperationResult<PhieuThuDto>.Failed(result.ErrorCode ?? ErrorCodes.BUSINESS_RULE_VIOLATION, result.ErrorMessage ?? "Không thể lập phiếu thu.");
            }

            return OperationResult<PhieuThuDto>.Success(result.Data!);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lập phiếu thu cho đăng ký ID={dangKyLopHocId}.", ex);
            return OperationResult<PhieuThuDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lập phiếu thu học phí.");
        }
    }

    public async Task<OperationResult<bool>> HuyPhieuThuAsync(int id, string lyDoHuy)
    {
        var auth = AuthorizationGuard.EnsurePermission<bool>(
            _currentUserService, Permission.TuitionManage, "Bạn không có quyền hủy phiếu thu học phí.", false);
        if (!auth.IsSuccess) return auth;

        if (string.IsNullOrWhiteSpace(lyDoHuy))
            return OperationResult<bool>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng nhập lý do hủy phiếu thu.", false);

        if (lyDoHuy.Trim().Length > 250)
            return OperationResult<bool>.Failed(ErrorCodes.VALIDATION_ERROR, "Lý do hủy phiếu thu không được vượt quá 250 ký tự.", false);

        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy phiếu thu ID={id}.", false);

            if (existing.TrangThai == TrangThaiPhieuThu.DaHuy)
                return OperationResult<bool>.Failed(ErrorCodes.VALIDATION_ERROR, "Phiếu thu này đã được hủy trước đó.", false);

            int? currentUserId = _currentUserService.CurrentUser?.Id;
            var ok = await _repository.CancelAsync(id, lyDoHuy.Trim(), currentUserId);
            if (!ok)
            {
                return OperationResult<bool>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể hủy phiếu thu hoặc phiếu thu đã bị hủy bởi tiến trình khác.", false);
            }

            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi hủy phiếu thu ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể hủy phiếu thu.", false);
        }
    }

    public async Task<OperationResult<decimal>> GetRemainingFeeAsync(int dangKyLopHocId)
    {
        var auth = AuthorizationGuard.EnsurePermission<decimal>(
            _currentUserService, Permission.TuitionManage, "Bạn không có quyền quản lý học phí.", 0m);
        if (!auth.IsSuccess) return auth;

        try
        {
            var dangKy = await _dangKyRepository.GetByIdAsync(dangKyLopHocId);
            if (dangKy == null)
                return OperationResult<decimal>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy hồ sơ đăng ký lớp.", 0m);

            var daThu = await _repository.GetTotalPaidByDangKyIdAsync(dangKyLopHocId);
            var conNo = Math.Max(0m, dangKy.HocPhiPhaiThu - daThu);
            return OperationResult<decimal>.Success(conNo);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi tính công nợ đăng ký ID={dangKyLopHocId}.", ex);
            return OperationResult<decimal>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tính số tiền còn nợ.", 0m);
        }
    }
}
