using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Training.Services;

public class KetQuaHocTapService : IKetQuaHocTapService
{
    private readonly IKetQuaHocTapRepository _repository;
    private readonly IDangKyLopHocRepository _dangKyRepository;
    private readonly ILopHocRepository _lopHocRepository;
    private readonly IMonHocRepository _monHocRepository;
    private readonly IKhoaHocMonHocRepository _khoaHocMonHocRepository;
    private readonly ILichHocRepository _lichHocRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public KetQuaHocTapService(
        IKetQuaHocTapRepository repository,
        IDangKyLopHocRepository dangKyRepository,
        ILopHocRepository lopHocRepository,
        IMonHocRepository monHocRepository,
        IKhoaHocMonHocRepository khoaHocMonHocRepository,
        ILichHocRepository lichHocRepository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _dangKyRepository = dangKyRepository;
        _lopHocRepository = lopHocRepository;
        _monHocRepository = monHocRepository;
        _khoaHocMonHocRepository = khoaHocMonHocRepository;
        _lichHocRepository = lichHocRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<LopHocDto>>> GetLopHocOptionsAsync()
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<LopHocDto>>(
            _currentUserService, Permission.GradeEntry, "Bạn không có quyền truy cập chức năng điểm số.", Array.Empty<LopHocDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var user = _currentUserService.CurrentUser;
            if (user == null) return OperationResult<IReadOnlyList<LopHocDto>>.Failed(ErrorCodes.UNAUTHORIZED, "Chưa đăng nhập.");

            var allLops = await _lopHocRepository.GetAllAsync(new LopHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });

            if (user.VaiTro == VaiTro.GIANGVIEN)
            {
                if (!user.GiangVienId.HasValue)
                {
                    return OperationResult<IReadOnlyList<LopHocDto>>.Success(Array.Empty<LopHocDto>());
                }

                // Giảng viên chỉ thấy lớp có lịch học active của chính mình
                var mySchedules = await _lichHocRepository.GetAllAsync(new LichHocFilter
                {
                    GiangVienId = user.GiangVienId.Value,
                    TrangThai = TrangThaiHoatDong.DangHoatDong
                });

                var myLopIds = mySchedules.Select(s => s.LopHocId).Distinct().ToHashSet();
                var filtered = allLops.Where(l => myLopIds.Contains(l.Id)).ToList();
                return OperationResult<IReadOnlyList<LopHocDto>>.Success(filtered);
            }

            return OperationResult<IReadOnlyList<LopHocDto>>.Success(allLops);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách lớp học cho màn hình điểm.", ex);
            return OperationResult<IReadOnlyList<LopHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách lớp học.", Array.Empty<LopHocDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<MonHocDto>>> GetMonHocOptionsAsync(int? lopHocId = null)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<MonHocDto>>(
            _currentUserService, Permission.GradeEntry, "Bạn không có quyền truy cập chức năng điểm số.", Array.Empty<MonHocDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var user = _currentUserService.CurrentUser;
            var allMons = await _monHocRepository.GetAllAsync(new MonHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });

            if (lopHocId.HasValue && lopHocId.Value > 0)
            {
                var lop = await _lopHocRepository.GetByIdAsync(lopHocId.Value);
                if (lop != null)
                {
                    var khoaMonList = await _khoaHocMonHocRepository.GetByKhoaHocIdAsync(lop.KhoaHocId);
                    var validMonIds = khoaMonList.Select(km => km.MonHocId).ToHashSet();
                    allMons = allMons.Where(m => validMonIds.Contains(m.Id)).ToList();
                }
            }

            if (user?.VaiTro == VaiTro.GIANGVIEN)
            {
                if (!user.GiangVienId.HasValue)
                {
                    return OperationResult<IReadOnlyList<MonHocDto>>.Success(Array.Empty<MonHocDto>());
                }

                var filter = new LichHocFilter
                {
                    GiangVienId = user.GiangVienId.Value,
                    TrangThai = TrangThaiHoatDong.DangHoatDong
                };
                if (lopHocId.HasValue && lopHocId.Value > 0) filter.LopHocId = lopHocId.Value;

                var mySchedules = await _lichHocRepository.GetAllAsync(filter);
                var myMonIds = mySchedules.Select(s => s.MonHocId).Distinct().ToHashSet();
                allMons = allMons.Where(m => myMonIds.Contains(m.Id)).ToList();
            }

            return OperationResult<IReadOnlyList<MonHocDto>>.Success(allMons);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách môn học cho màn hình điểm.", ex);
            return OperationResult<IReadOnlyList<MonHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách môn học.", Array.Empty<MonHocDto>());
        }
    }

    public async Task<OperationResult<IReadOnlyList<KetQuaHocTapDto>>> GetByLopAndMonAsync(int lopHocId, int monHocId)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<KetQuaHocTapDto>>(
            _currentUserService, Permission.GradeEntry, "Bạn không có quyền nhập hoặc xem điểm.", Array.Empty<KetQuaHocTapDto>());
        if (!auth.IsSuccess) return auth;

        // Xác minh giảng viên có được phân công lịch học đang hoạt động cho lớp-môn này không
        var gvCheck = await EnsureGiangVienAssignedAsync(lopHocId, monHocId);
        if (!gvCheck.IsSuccess)
        {
            return OperationResult<IReadOnlyList<KetQuaHocTapDto>>.Failed(
                gvCheck.Error?.Code ?? ErrorCodes.FORBIDDEN, gvCheck.Message, Array.Empty<KetQuaHocTapDto>());
        }

        try
        {
            var enrollments = await _dangKyRepository.GetAllAsync(new DangKyLopHocFilter
            {
                LopHocId = lopHocId,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });

            var existingGrades = await _repository.GetAllAsync(new KetQuaHocTapFilter
            {
                LopHocId = lopHocId,
                MonHocId = monHocId
            });

            var mon = await _monHocRepository.GetByIdAsync(monHocId);
            var lop = await _lopHocRepository.GetByIdAsync(lopHocId);

            var list = new List<KetQuaHocTapDto>();
            foreach (var en in enrollments)
            {
                var grade = existingGrades.FirstOrDefault(g => g.DangKyLopHocId == en.Id);
                if (grade != null)
                {
                    list.Add(grade);
                }
                else
                {
                    list.Add(new KetQuaHocTapDto
                    {
                        DangKyLopHocId = en.Id,
                        LopHocId = lopHocId,
                        MaLopHoc = lop?.MaLopHoc ?? en.MaLopHoc,
                        TenLopHoc = lop?.TenLopHoc ?? en.TenLopHoc,
                        HocVienId = en.HocVienId,
                        MaHocVien = en.MaHocVien,
                        HoTenHocVien = en.HoTenHocVien,
                        MonHocId = monHocId,
                        MaMonHoc = mon?.MaMonHoc ?? string.Empty,
                        TenMonHoc = mon?.TenMonHoc ?? string.Empty,
                        DiemTongKet = null // Phân biệt rõ chưa nhập điểm
                    });
                }
            }

            return OperationResult<IReadOnlyList<KetQuaHocTapDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi tải bảng điểm lớp {lopHocId}, môn {monHocId}.", ex);
            return OperationResult<IReadOnlyList<KetQuaHocTapDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách điểm học viên.", Array.Empty<KetQuaHocTapDto>());
        }
    }

    public async Task<OperationResult<KetQuaHocTapDto>> SaveGradeAsync(KetQuaHocTapDto dto)
    {
        var auth = AuthorizationGuard.EnsurePermission<KetQuaHocTapDto>(
            _currentUserService, Permission.GradeEntry, "Bạn không có quyền nhập hoặc sửa điểm.");
        if (!auth.IsSuccess) return auth;

        // Xác thực đăng ký từ Repository
        var dk = await _dangKyRepository.GetByIdAsync(dto.DangKyLopHocId);
        if (dk == null)
            return OperationResult<KetQuaHocTapDto>.Failed(ErrorCodes.NOT_FOUND, "Bản ghi đăng ký lớp học không tồn tại.");

        if (dk.TrangThai != TrangThaiHoatDong.DangHoatDong)
            return OperationResult<KetQuaHocTapDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Học viên này đã bị hủy hoặc ngừng hoạt động trong lớp.");

        // Không tin LopHocId, HocVienId do UI gửi; lấy từ DangKy thực tế
        int actualLopId = dk.LopHocId;
        dto.LopHocId = actualLopId;
        dto.HocVienId = dk.HocVienId;

        // Xác minh môn học thuộc khóa học của lớp
        var lop = await _lopHocRepository.GetByIdAsync(actualLopId);
        if (lop == null || lop.TrangThai != TrangThaiHoatDong.DangHoatDong)
            return OperationResult<KetQuaHocTapDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học không tồn tại hoặc đã ngừng hoạt động.");

        var khoaMonList = await _khoaHocMonHocRepository.GetByKhoaHocIdAsync(lop.KhoaHocId);
        if (!khoaMonList.Any(km => km.MonHocId == dto.MonHocId))
            return OperationResult<KetQuaHocTapDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của lớp học này.");

        // Kiểm tra độ dài ghi chú
        if (!string.IsNullOrEmpty(dto.GhiChu) && dto.GhiChu.Length > 250)
            return OperationResult<KetQuaHocTapDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Ghi chú không được vượt quá 250 ký tự.");

        // Kiểm tra quyền nếu người dùng hiện tại là GIANGVIEN
        var gvCheck = await EnsureGiangVienAssignedAsync(actualLopId, dto.MonHocId);
        if (!gvCheck.IsSuccess)
            return OperationResult<KetQuaHocTapDto>.Failed(gvCheck.Error?.Code ?? ErrorCodes.FORBIDDEN, gvCheck.Message);

        try
        {
            if (!dto.DiemTongKet.HasValue)
            {
                await _repository.DeleteByDangKyAndMonAsync(dto.DangKyLopHocId, dto.MonHocId);
                return OperationResult<KetQuaHocTapDto>.Success(dto);
            }

            if (dto.DiemTongKet.Value < 0.00m || dto.DiemTongKet.Value > 10.00m)
                return OperationResult<KetQuaHocTapDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Điểm tổng kết phải nằm trong khoảng từ 0.00 đến 10.00.");

            dto.NguoiCapNhatTaiKhoanId = _currentUserService.CurrentUser?.Id;
            dto.NguoiCapNhatTen = _currentUserService.CurrentUser?.TenDangNhap;
            dto.NgayCapNhat = DateTime.Now;

            var result = await _repository.UpsertAsync(dto);
            return OperationResult<KetQuaHocTapDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi lưu điểm học viên.", ex);
            return OperationResult<KetQuaHocTapDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lưu điểm học viên.");
        }
    }

    public async Task<OperationResult<bool>> DeleteGradeAsync(int dangKyLopHocId, int monHocId)
    {
        var auth = AuthorizationGuard.EnsurePermission<bool>(
            _currentUserService, Permission.GradeEntry, "Bạn không có quyền xóa điểm.", false);
        if (!auth.IsSuccess) return auth;

        var dk = await _dangKyRepository.GetByIdAsync(dangKyLopHocId);
        if (dk == null)
            return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, "Bản ghi đăng ký lớp học không tồn tại.", false);

        if (dk.TrangThai != TrangThaiHoatDong.DangHoatDong)
            return OperationResult<bool>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Học viên này không còn hoạt động trong lớp.", false);

        var lop = await _lopHocRepository.GetByIdAsync(dk.LopHocId);
        if (lop == null || lop.TrangThai != TrangThaiHoatDong.DangHoatDong)
            return OperationResult<bool>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học không tồn tại hoặc đã ngừng hoạt động.", false);

        var khoaMonList = await _khoaHocMonHocRepository.GetByKhoaHocIdAsync(lop.KhoaHocId);
        if (!khoaMonList.Any(km => km.MonHocId == monHocId))
            return OperationResult<bool>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của lớp học này.", false);

        var gvCheck = await EnsureGiangVienAssignedAsync(dk.LopHocId, monHocId);
        if (!gvCheck.IsSuccess)
            return OperationResult<bool>.Failed(gvCheck.Error?.Code ?? ErrorCodes.FORBIDDEN, gvCheck.Message, false);

        try
        {
            var deleted = await _repository.DeleteByDangKyAndMonAsync(dangKyLopHocId, monHocId);
            return OperationResult<bool>.Success(deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi xóa điểm của đăng ký {dangKyLopHocId}, môn {monHocId}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể xóa điểm.", false);
        }
    }

    public async Task<OperationResult<int>> SaveGradesBatchAsync(IEnumerable<KetQuaHocTapDto> dtos)
    {
        var auth = AuthorizationGuard.EnsurePermission<int>(
            _currentUserService, Permission.GradeEntry, "Bạn không có quyền nhập hoặc sửa điểm.", 0);
        if (!auth.IsSuccess) return auth;

        var dtoList = dtos.ToList();
        if (dtoList.Count == 0) return OperationResult<int>.Success(0);

        // 1. Kiểm tra tính đồng nhất của batch: Một batch phải cùng một môn học
        int firstMonId = dtoList[0].MonHocId;
        if (dtoList.Any(x => x.MonHocId != firstMonId))
        {
            return OperationResult<int>.Failed(ErrorCodes.VALIDATION_ERROR, "Một đợt lưu điểm chỉ được áp dụng cho một môn học duy nhất.", 0);
        }

        // 2. Xác thực từng phần tử từ repository
        int? targetLopId = null;
        var toUpsert = new List<KetQuaHocTapDto>();
        var toDeleteIds = new List<int>();

        foreach (var item in dtoList)
        {
            var dk = await _dangKyRepository.GetByIdAsync(item.DangKyLopHocId);
            if (dk == null)
            {
                return OperationResult<int>.Failed(ErrorCodes.NOT_FOUND, $"Đăng ký học #{item.DangKyLopHocId} không tồn tại.", 0);
            }

            if (dk.TrangThai != TrangThaiHoatDong.DangHoatDong)
            {
                return OperationResult<int>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION,
                    $"Học viên {dk.HoTenHocVien} ({dk.MaHocVien}) không còn hoạt động trong lớp.", 0);
            }

            if (!targetLopId.HasValue)
            {
                targetLopId = dk.LopHocId;
            }
            else if (targetLopId.Value != dk.LopHocId)
            {
                return OperationResult<int>.Failed(ErrorCodes.VALIDATION_ERROR, "Từ chối lưu batch vì chứa học viên từ các lớp học khác nhau.", 0);
            }

            item.LopHocId = dk.LopHocId;
            item.HocVienId = dk.HocVienId;

            if (!string.IsNullOrEmpty(item.GhiChu) && item.GhiChu.Length > 250)
            {
                return OperationResult<int>.Failed(ErrorCodes.VALIDATION_ERROR,
                    $"Ghi chú của học viên {dk.HoTenHocVien} vượt quá 250 ký tự.", 0);
            }

            if (item.DiemTongKet.HasValue)
            {
                if (item.DiemTongKet.Value < 0.00m || item.DiemTongKet.Value > 10.00m)
                {
                    return OperationResult<int>.Failed(ErrorCodes.VALIDATION_ERROR,
                        $"Điểm ({item.DiemTongKet.Value}) không hợp lệ. Điểm phải từ 0.00 đến 10.00.", 0);
                }
                item.NguoiCapNhatTaiKhoanId = _currentUserService.CurrentUser?.Id;
                item.NguoiCapNhatTen = _currentUserService.CurrentUser?.TenDangNhap;
                item.NgayCapNhat = DateTime.Now;
                toUpsert.Add(item);
            }
            else
            {
                toDeleteIds.Add(item.DangKyLopHocId);
            }
        }

        int finalLopId = targetLopId!.Value;

        // 3. Xác minh môn thuộc khóa học của lớp
        var lop = await _lopHocRepository.GetByIdAsync(finalLopId);
        if (lop == null || lop.TrangThai != TrangThaiHoatDong.DangHoatDong)
            return OperationResult<int>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học không tồn tại hoặc đã ngừng hoạt động.", 0);

        var khoaMonList = await _khoaHocMonHocRepository.GetByKhoaHocIdAsync(lop.KhoaHocId);
        if (!khoaMonList.Any(km => km.MonHocId == firstMonId))
            return OperationResult<int>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của lớp học này.", 0);

        // 4. Kiểm tra phân công giảng viên
        var gvCheck = await EnsureGiangVienAssignedAsync(finalLopId, firstMonId);
        if (!gvCheck.IsSuccess)
            return OperationResult<int>.Failed(gvCheck.Error?.Code ?? ErrorCodes.FORBIDDEN, gvCheck.Message, 0);

        try
        {
            var count = await _repository.SaveGradesBatchAtomicAsync(toUpsert, toDeleteIds, firstMonId);
            return OperationResult<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi lưu bảng điểm hàng loạt.", ex);
            return OperationResult<int>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lưu bảng điểm.", 0);
        }
    }

    private async Task<OperationResult> EnsureGiangVienAssignedAsync(int lopHocId, int monHocId)
    {
        var user = _currentUserService.CurrentUser;
        if (user == null) return OperationResult.Failed(ErrorCodes.FORBIDDEN, "Bạn cần đăng nhập để thao tác.");

        if (user.VaiTro == VaiTro.ADMIN || user.VaiTro == VaiTro.DAOTAO) return OperationResult.Success();

        if (user.VaiTro == VaiTro.GIANGVIEN)
        {
            if (!user.GiangVienId.HasValue)
                return OperationResult.Failed(ErrorCodes.FORBIDDEN, "Tài khoản giảng viên chưa được liên kết với hồ sơ giảng viên.");

            // Nguồn phân công hợp lệ: Lịch học còn hoạt động của đúng giảng viên, lớp và môn
            var schedules = await _lichHocRepository.GetAllAsync(new LichHocFilter
            {
                LopHocId = lopHocId,
                MonHocId = monHocId,
                GiangVienId = user.GiangVienId.Value,
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });

            if (schedules.Count == 0)
            {
                return OperationResult.Failed(ErrorCodes.FORBIDDEN, "Bạn không được phân công dạy môn học này cho lớp học đã chọn (không có lịch giảng dạy còn hoạt động).");
            }

            return OperationResult.Success();
        }

        return OperationResult.Failed(ErrorCodes.FORBIDDEN, "Bạn không có quyền nhập hoặc xem điểm.");
    }
}
