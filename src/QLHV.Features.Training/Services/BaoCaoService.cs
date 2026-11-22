using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Training.Services;

public class BaoCaoService : IBaoCaoService
{
    private readonly IBaoCaoRepository _repository;
    private readonly ILopHocRepository _lopHocRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public BaoCaoService(
        IBaoCaoRepository repository,
        ILopHocRepository lopHocRepository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _lopHocRepository = lopHocRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<LopHocDto>>> GetLopHocFilterOptionsAsync()
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<LopHocDto>>(
            _currentUserService, Permission.ReportView, "Bạn không có quyền xem báo cáo thống kê.", Array.Empty<LopHocDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var lops = await _lopHocRepository.GetAllAsync();
            return OperationResult<IReadOnlyList<LopHocDto>>.Success(lops);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách lớp học cho bộ lọc báo cáo.", ex);
            return OperationResult<IReadOnlyList<LopHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách lớp học.", Array.Empty<LopHocDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<BaoCaoHocVienTheoLopDto>>> GetBaoCaoHocVienTheoLopAsync(int lopHocId)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<BaoCaoHocVienTheoLopDto>>(
            _currentUserService, Permission.ReportView, "Bạn không có quyền xem báo cáo thống kê.", Array.Empty<BaoCaoHocVienTheoLopDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetBaoCaoHocVienTheoLopAsync(lopHocId);
            return OperationResult<IReadOnlyList<BaoCaoHocVienTheoLopDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lập báo cáo học viên theo lớp ID={lopHocId}.", ex);
            return OperationResult<IReadOnlyList<BaoCaoHocVienTheoLopDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lập báo cáo học viên theo lớp.", Array.Empty<BaoCaoHocVienTheoLopDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<BaoCaoLichHocTheoLopDto>>> GetBaoCaoLichHocTheoLopAsync(int lopHocId)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<BaoCaoLichHocTheoLopDto>>(
            _currentUserService, Permission.ReportView, "Bạn không có quyền xem báo cáo thống kê.", Array.Empty<BaoCaoLichHocTheoLopDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetBaoCaoLichHocTheoLopAsync(lopHocId);
            return OperationResult<IReadOnlyList<BaoCaoLichHocTheoLopDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lập báo cáo lịch học theo lớp ID={lopHocId}.", ex);
            return OperationResult<IReadOnlyList<BaoCaoLichHocTheoLopDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lập báo cáo lịch học theo lớp.", Array.Empty<BaoCaoLichHocTheoLopDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>>> GetBaoCaoGiangDayTheoGiangVienAsync(int giangVienId, DateTime? tuNgay = null, DateTime? denNgay = null)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>>(
            _currentUserService, Permission.ReportView, "Bạn không có quyền xem báo cáo thống kê.", Array.Empty<BaoCaoGiangDayTheoGiangVienDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetBaoCaoGiangDayTheoGiangVienAsync(giangVienId, tuNgay, denNgay);
            return OperationResult<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lập báo cáo giảng dạy theo giảng viên ID={giangVienId}.", ex);
            return OperationResult<IReadOnlyList<BaoCaoGiangDayTheoGiangVienDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lập báo cáo giảng dạy theo giảng viên.", Array.Empty<BaoCaoGiangDayTheoGiangVienDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<BaoCaoKetQuaHocTapDto>>> GetBaoCaoKetQuaHocTapAsync(int? lopHocId, int? monHocId)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<BaoCaoKetQuaHocTapDto>>(
            _currentUserService, Permission.ReportView, "Bạn không có quyền xem báo cáo thống kê.", Array.Empty<BaoCaoKetQuaHocTapDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetBaoCaoKetQuaHocTapAsync(lopHocId, monHocId);
            return OperationResult<IReadOnlyList<BaoCaoKetQuaHocTapDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi lập báo cáo kết quả học tập.", ex);
            return OperationResult<IReadOnlyList<BaoCaoKetQuaHocTapDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lập báo cáo kết quả học tập.", Array.Empty<BaoCaoKetQuaHocTapDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<ThongKeTyLeMonHocDto>>> GetThongKeTyLeMonHocAsync(int? lopHocId)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<ThongKeTyLeMonHocDto>>(
            _currentUserService, Permission.ReportView, "Bạn không có quyền xem báo cáo thống kê.", Array.Empty<ThongKeTyLeMonHocDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetThongKeTyLeMonHocAsync(lopHocId);
            return OperationResult<IReadOnlyList<ThongKeTyLeMonHocDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi lập thống kê tỷ lệ đạt môn học.", ex);
            return OperationResult<IReadOnlyList<ThongKeTyLeMonHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lập thống kê tỷ lệ đạt môn học.", Array.Empty<ThongKeTyLeMonHocDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<BaoCaoCongNoHocPhiDto>>> GetBaoCaoCongNoHocPhiAsync(int? lopHocId = null)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<BaoCaoCongNoHocPhiDto>>(
            _currentUserService, Permission.ReportView, "Bạn không có quyền xem báo cáo thống kê.", Array.Empty<BaoCaoCongNoHocPhiDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetBaoCaoCongNoHocPhiAsync(lopHocId);
            return OperationResult<IReadOnlyList<BaoCaoCongNoHocPhiDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi lập báo cáo công nợ học phí.", ex);
            return OperationResult<IReadOnlyList<BaoCaoCongNoHocPhiDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lập báo cáo công nợ học phí.", Array.Empty<BaoCaoCongNoHocPhiDto>());
        }
    }
}
