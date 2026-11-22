using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Training.Services;

public class LopHocService : ILopHocService
{
    private readonly ILopHocRepository _repository;
    private readonly IKhoaHocRepository _khoaHocRepository;
    private readonly IGiangVienRepository _giangVienRepository;
    private readonly IDangKyLopHocRepository _dangKyRepository;
    private readonly ILichHocRepository _lichHocRepository;
    private readonly IKetQuaHocTapRepository _ketQuaRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public LopHocService(
        ILopHocRepository repository,
        IKhoaHocRepository khoaHocRepository,
        IGiangVienRepository giangVienRepository,
        IDangKyLopHocRepository dangKyRepository,
        ILichHocRepository lichHocRepository,
        IKetQuaHocTapRepository ketQuaRepository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _khoaHocRepository = khoaHocRepository ?? throw new ArgumentNullException(nameof(khoaHocRepository));
        _giangVienRepository = giangVienRepository ?? throw new ArgumentNullException(nameof(giangVienRepository));
        _dangKyRepository = dangKyRepository ?? throw new ArgumentNullException(nameof(dangKyRepository));
        _lichHocRepository = lichHocRepository ?? throw new ArgumentNullException(nameof(lichHocRepository));
        _ketQuaRepository = ketQuaRepository ?? throw new ArgumentNullException(nameof(ketQuaRepository));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OperationResult<IReadOnlyList<LopHocDto>>> GetAllAsync(LopHocFilter? filter = null)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<LopHocDto>>(
            _currentUserService, Permission.TrainingView, "Bạn không có quyền xem thông tin đào tạo.", Array.Empty<LopHocDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<LopHocDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách lớp học.", ex);
            return OperationResult<IReadOnlyList<LopHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách lớp học.", Array.Empty<LopHocDto>());
        }
    }

    public async Task<OperationResult<LopHocDto>> GetByIdAsync(int id)
    {
        var auth = AuthorizationGuard.EnsurePermission<LopHocDto>(
            _currentUserService, Permission.TrainingView, "Bạn không có quyền xem thông tin đào tạo.");
        if (!auth.IsSuccess) return auth;

        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return OperationResult<LopHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy lớp học với ID={id}.");

            return OperationResult<LopHocDto>.Success(item);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy thông tin lớp học ID={id}.", ex);
            return OperationResult<LopHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy thông tin lớp học.");
        }
    }

    public async Task<OperationResult<LopHocDto>> CreateAsync(LopHocDto dto)
    {
        var auth = AuthorizationGuard.EnsurePermission<LopHocDto>(
            _currentUserService, Permission.TrainingManage, "Bạn không có quyền quản lý đào tạo.");
        if (!auth.IsSuccess) return auth;

        if (string.IsNullOrWhiteSpace(dto.MaLopHoc))
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Mã lớp học không được để trống.");

        if (dto.MaLopHoc.Trim().Length > 20)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Mã lớp học không được vượt quá 20 ký tự.");

        if (string.IsNullOrWhiteSpace(dto.TenLopHoc))
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Tên lớp học không được để trống.");

        if (dto.TenLopHoc.Trim().Length > 150)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Tên lớp học không được vượt quá 150 ký tự.");

        if (dto.KhoaHocId <= 0)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng chọn khóa học hợp lệ.");

        var khoaHoc = await _khoaHocRepository.GetByIdAsync(dto.KhoaHocId);
        if (khoaHoc == null)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.NOT_FOUND, "Khóa học được chọn không tồn tại.");

        // Bắt buộc chọn Giảng viên chủ nhiệm (STUDENT-PROFILE-AND-TRAINING-UI-05)
        if (!dto.GiangVienChuNhiemId.HasValue || dto.GiangVienChuNhiemId.Value <= 0)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.REQUIRED, "Vui lòng chọn giảng viên chủ nhiệm cho lớp học.");

        var gv = await _giangVienRepository.GetByIdAsync(dto.GiangVienChuNhiemId.Value);
        if (gv == null || gv.TrangThai != TrangThaiHoatDong.DangHoatDong)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên chủ nhiệm được chọn không tồn tại hoặc đã ngừng hoạt động.");

        if (dto.SiSoToiDa < 1 || dto.SiSoToiDa > 200)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Sĩ số tối đa phải nằm trong khoảng từ 1 đến 200.");

        if (dto.NgayKetThuc.HasValue && dto.NgayKetThuc.Value < dto.NgayBatDau)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Ngày kết thúc dự kiến không thể trước ngày bắt đầu.");

        try
        {
            var existing = await _repository.GetByMaLopAsync(dto.MaLopHoc.Trim());
            if (existing != null)
                return OperationResult<LopHocDto>.Failed(ErrorCodes.DUPLICATE_CODE, $"Mã lớp '{dto.MaLopHoc}' đã tồn tại.");

            dto.MaLopHoc = dto.MaLopHoc.Trim().ToUpperInvariant();
            dto.TenLopHoc = dto.TenLopHoc.Trim();
            dto.TenKhoaHoc = khoaHoc.TenKhoaHoc;
            dto.HoTenGiangVienChuNhiem = gv.HoTen;
            dto.TrangThai = TrangThaiHoatDong.DangHoatDong;

            var created = await _repository.AddAsync(dto);
            return OperationResult<LopHocDto>.Success(created);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi thêm mới lớp học.", ex);
            return OperationResult<LopHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể thêm lớp học mới.");
        }
    }

    public async Task<OperationResult<LopHocDto>> UpdateAsync(LopHocDto dto)
    {
        var auth = AuthorizationGuard.EnsurePermission<LopHocDto>(
            _currentUserService, Permission.TrainingManage, "Bạn không có quyền quản lý đào tạo.");
        if (!auth.IsSuccess) return auth;

        if (dto.Id <= 0)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "ID lớp học không hợp lệ.");

        if (string.IsNullOrWhiteSpace(dto.TenLopHoc))
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Tên lớp học không được để trống.");

        if (dto.TenLopHoc.Trim().Length > 150)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Tên lớp học không được vượt quá 150 ký tự.");

        if (dto.KhoaHocId <= 0)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng chọn khóa học hợp lệ.");

        var khoaHoc = await _khoaHocRepository.GetByIdAsync(dto.KhoaHocId);
        if (khoaHoc == null)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.NOT_FOUND, "Khóa học được chọn không tồn tại.");

        // Bắt buộc chọn Giảng viên chủ nhiệm (STUDENT-PROFILE-AND-TRAINING-UI-05)
        if (!dto.GiangVienChuNhiemId.HasValue || dto.GiangVienChuNhiemId.Value <= 0)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.REQUIRED, "Vui lòng chọn giảng viên chủ nhiệm cho lớp học.");

        var existing = await _repository.GetByIdAsync(dto.Id);
        if (existing == null)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy lớp học có ID={dto.Id}.");

        // Giảng viên chủ nhiệm: nếu thay đổi giảng viên thì giảng viên mới bắt buộc đang hoạt động; nếu giữ nguyên thì cho phép nếu đã gán từ trước
        var gv = await _giangVienRepository.GetByIdAsync(dto.GiangVienChuNhiemId.Value);
        if (gv == null)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.NOT_FOUND, "Giảng viên chủ nhiệm được chọn không tồn tại.");

        if (existing.GiangVienChuNhiemId != dto.GiangVienChuNhiemId && gv.TrangThai != TrangThaiHoatDong.DangHoatDong)
        {
            return OperationResult<LopHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên chủ nhiệm mới được chọn đã ngừng hoạt động.");
        }

        if (dto.SiSoToiDa < 1 || dto.SiSoToiDa > 200)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Sĩ số tối đa phải nằm trong khoảng từ 1 đến 200.");

        if (dto.NgayKetThuc.HasValue && dto.NgayKetThuc.Value < dto.NgayBatDau)
            return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Ngày kết thúc dự kiến không thể trước ngày bắt đầu.");

        try
        {
            // Không cho phép phục hồi hoặc thay đổi trạng thái thông qua hàm Update thông thường
            dto.TrangThai = existing.TrangThai;

            // Ràng buộc 1: Không cho giảm SiSoToiDa xuống thấp hơn số học viên đang hoạt động
            var activeStudentCount = await _dangKyRepository.CountByLopHocIdAsync(dto.Id);
            if (dto.SiSoToiDa < activeStudentCount)
            {
                return OperationResult<LopHocDto>.Failed(ErrorCodes.VALIDATION_ERROR,
                    $"Không thể giảm sĩ số tối đa xuống {dto.SiSoToiDa} vì lớp đang có {activeStudentCount} học viên đang học.");
            }

            // Ràng buộc 2: Không cho đổi khóa học của lớp khi đã có đăng ký hoặc lịch học hoặc điểm (kể cả đã hủy/inactive)
            if (existing.KhoaHocId != dto.KhoaHocId)
            {
                var allEnrollments = await _dangKyRepository.GetAllAsync(new DangKyLopHocFilter { LopHocId = dto.Id });
                var hasEnrollment = allEnrollments.Count > 0;
                var schedules = await _lichHocRepository.GetAllAsync(new LichHocFilter { LopHocId = dto.Id });
                var hasSchedule = schedules.Count > 0;
                var grades = await _ketQuaRepository.GetAllAsync(new KetQuaHocTapFilter { LopHocId = dto.Id });
                var hasGrades = grades.Count > 0;

                if (hasEnrollment || hasSchedule || hasGrades)
                {
                    return OperationResult<LopHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION,
                        "Không thể thay đổi khóa học của lớp khi lớp đã phát sinh danh sách học viên, lịch học hoặc điểm số.");
                }
            }

            // Ràng buộc 3: Kiểm tra lịch học đang hoạt động phải nằm trong khoảng thời gian mới của lớp
            var activeSchedulesForDates = await _lichHocRepository.GetAllAsync(new LichHocFilter
            {
                LopHocId = dto.Id,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });

            foreach (var lh in activeSchedulesForDates)
            {
                if (lh.NgayHoc.Date < dto.NgayBatDau.Date || (dto.NgayKetThuc.HasValue && lh.NgayHoc.Date > dto.NgayKetThuc.Value.Date))
                {
                    return OperationResult<LopHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION,
                        $"Khoảng thời gian mới của lớp xung đột với lịch học ngày {lh.NgayHoc:dd/MM/yyyy}.");
                }
            }

            dto.TenLopHoc = dto.TenLopHoc.Trim();
            dto.TenKhoaHoc = khoaHoc.TenKhoaHoc;
            dto.HoTenGiangVienChuNhiem = gv.HoTen;

            var (success, errCode, errMsg) = await _repository.UpdateAtomicAsync(dto);
            if (!success)
                return OperationResult<LopHocDto>.Failed(errCode ?? ErrorCodes.UNEXPECTED_ERROR, errMsg ?? "Cập nhật lớp học không thành công.");

            return OperationResult<LopHocDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi cập nhật lớp học ID={dto.Id}.", ex);
            return OperationResult<LopHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể cập nhật lớp học.");
        }
    }

    public async Task<OperationResult<bool>> DeactivateAsync(int id)
    {
        var auth = AuthorizationGuard.EnsurePermission<bool>(
            _currentUserService, Permission.TrainingManage, "Bạn không có quyền quản lý đào tạo.", false);
        if (!auth.IsSuccess) return auth;

        try
        {
            var (success, errCode, errMsg) = await _repository.DeactivateAtomicAsync(id);
            if (!success)
            {
                return OperationResult<bool>.Failed(errCode ?? ErrorCodes.BUSINESS_RULE_VIOLATION, errMsg ?? "Không thể ngừng hoạt động lớp học.", false);
            }

            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi ngừng hoạt động lớp học ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể cập nhật trạng thái lớp học.", false);
        }
    }
}
