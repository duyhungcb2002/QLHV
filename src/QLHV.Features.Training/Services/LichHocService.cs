using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;

namespace QLHV.Features.Training.Services;

public class LichHocService : ILichHocService
{
    private readonly ILichHocRepository _repository;
    private readonly ILopHocRepository _lopHocRepository;
    private readonly IMonHocRepository _monHocRepository;
    private readonly IGiangVienRepository _giangVienRepository;
    private readonly IPhongHocRepository _phongHocRepository;
    private readonly IKhoaHocMonHocRepository _khoaHocMonHocRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public LichHocService(
        ILichHocRepository repository,
        ILopHocRepository lopHocRepository,
        IMonHocRepository monHocRepository,
        IGiangVienRepository giangVienRepository,
        IPhongHocRepository phongHocRepository,
        IKhoaHocMonHocRepository khoaHocMonHocRepository,
        ICurrentUserService currentUserService,
        IAppLogger logger)
    {
        _repository = repository;
        _lopHocRepository = lopHocRepository;
        _monHocRepository = monHocRepository;
        _giangVienRepository = giangVienRepository;
        _phongHocRepository = phongHocRepository;
        _khoaHocMonHocRepository = khoaHocMonHocRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<OperationResult<IReadOnlyList<LichHocDto>>> GetAllAsync(LichHocFilter? filter = null)
    {
        var auth = AuthorizationGuard.EnsurePermission<IReadOnlyList<LichHocDto>>(
            _currentUserService, Permission.TrainingView, "Bạn không có quyền xem thông tin đào tạo.", Array.Empty<LichHocDto>());
        if (!auth.IsSuccess) return auth;

        try
        {
            var list = await _repository.GetAllAsync(filter);
            return OperationResult<IReadOnlyList<LichHocDto>>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi tải danh sách lịch học.", ex);
            return OperationResult<IReadOnlyList<LichHocDto>>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tải danh sách lịch học.", Array.Empty<LichHocDto>());
        }
    }

    public async Task<OperationResult<LichHocDto>> CreateAsync(LichHocDto dto)
    {
        var auth = AuthorizationGuard.EnsurePermission<LichHocDto>(
            _currentUserService, Permission.TrainingManage, "Bạn không có quyền quản lý đào tạo.");
        if (!auth.IsSuccess) return auth;

        var validationResult = ValidateLichHocBasic(dto);
        if (!validationResult.IsSuccess) return validationResult;

        try
        {
            var lop = await _lopHocRepository.GetByIdAsync(dto.LopHocId);
            if (lop == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy lớp học.");
            if (lop.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động.");

            var mon = await _monHocRepository.GetByIdAsync(dto.MonHocId);
            if (mon == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy môn học.");
            if (mon.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học đã ngừng hoạt động.");

            var gv = await _giangVienRepository.GetByIdAsync(dto.GiangVienId);
            if (gv == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy giảng viên.");
            if (gv.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên đã ngừng hoạt động.");

            var phong = await _phongHocRepository.GetByIdAsync(dto.PhongHocId);
            if (phong == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy phòng học.");
            if (phong.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học đã ngừng hoạt động.");

            // 1. Môn phải thuộc khóa học của lớp
            var km = await _khoaHocMonHocRepository.GetAsync(lop.KhoaHocId, dto.MonHocId);
            if (km == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của khóa học này.");

            // 2. Ngày học nằm trong thời gian diễn ra lớp
            if (dto.NgayHoc.Date < lop.NgayBatDau.Date)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, $"Ngày học ({dto.NgayHoc:dd/MM/yyyy}) không được trước ngày bắt đầu của lớp ({lop.NgayBatDau:dd/MM/yyyy}).");

            if (lop.NgayKetThuc.HasValue && dto.NgayHoc.Date > lop.NgayKetThuc.Value.Date)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, $"Ngày học ({dto.NgayHoc:dd/MM/yyyy}) không được sau ngày kết thúc của lớp ({lop.NgayKetThuc.Value:dd/MM/yyyy}).");

            // 3. Sức chứa phòng không nhỏ hơn sĩ số tối đa của lớp
            if (phong.SucChua < lop.SiSoToiDa)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, $"Sức chứa phòng học ({phong.SucChua} chỗ) nhỏ hơn sĩ số tối đa của lớp ({lop.SiSoToiDa} học viên).");

            // 4. Kiểm tra xung đột
            if (await _repository.HasConflictPhongAsync(dto.PhongHocId, dto.NgayHoc, dto.GioBatDau, dto.GioKetThuc))
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học đã có lịch xếp trong khung giờ này.");

            if (await _repository.HasConflictGiangVienAsync(dto.GiangVienId, dto.NgayHoc, dto.GioBatDau, dto.GioKetThuc))
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên đã có lịch dạy trong khung giờ này.");

            if (await _repository.HasConflictLopHocAsync(dto.LopHocId, dto.NgayHoc, dto.GioBatDau, dto.GioKetThuc))
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã có buổi học khác trong khung giờ này.");

            dto.MaLopHoc = lop.MaLopHoc;
            dto.TenLopHoc = lop.TenLopHoc;
            dto.MaMonHoc = mon.MaMonHoc;
            dto.TenMonHoc = mon.TenMonHoc;
            dto.MaGiangVien = gv.MaGiangVien;
            dto.HoTenGiangVien = gv.HoTen;
            dto.MaPhong = phong.MaPhong;
            dto.TenPhong = phong.TenPhong;
            dto.TrangThai = TrangThaiHoatDong.DangHoatDong;

            // Thực thi qua transaction nguyên tử chống race condition trùng lịch
            var result = await _repository.CreateAtomicAsync(dto);
            if (!result.Success)
            {
                return OperationResult<LichHocDto>.Failed(result.ErrorCode ?? ErrorCodes.BUSINESS_RULE_VIOLATION, result.ErrorMessage ?? "Không thể tạo lịch học mới.");
            }

            return OperationResult<LichHocDto>.Success(result.Data!);
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi thêm mới lịch học.", ex);
            return OperationResult<LichHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể tạo lịch học mới.");
        }
    }

    public async Task<OperationResult<LichHocDto>> UpdateAsync(LichHocDto dto)
    {
        var auth = AuthorizationGuard.EnsurePermission<LichHocDto>(
            _currentUserService, Permission.TrainingManage, "Bạn không có quyền quản lý đào tạo.");
        if (!auth.IsSuccess) return auth;

        if (dto.Id <= 0)
            return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "ID lịch học không hợp lệ.");

        var validationResult = ValidateLichHocBasic(dto);
        if (!validationResult.IsSuccess) return validationResult;

        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy lịch học ID={dto.Id}.");

            if (existing.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Không thể chỉnh sửa buổi học đã ngừng hoạt động/đã bị hủy.");

            // Khóa chặt LopHocId theo bản ghi hiện có, ngăn chặn injection chuyển đổi lớp học ngoài ý muốn
            dto.LopHocId = existing.LopHocId;

            var lop = await _lopHocRepository.GetByIdAsync(dto.LopHocId);
            if (lop == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy lớp học.");
            if (lop.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã ngừng hoạt động.");

            var mon = await _monHocRepository.GetByIdAsync(dto.MonHocId);
            if (mon == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy môn học.");
            if (mon.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học đã ngừng hoạt động.");

            var gv = await _giangVienRepository.GetByIdAsync(dto.GiangVienId);
            if (gv == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy giảng viên.");
            if (gv.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên đã ngừng hoạt động.");

            var phong = await _phongHocRepository.GetByIdAsync(dto.PhongHocId);
            if (phong == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.NOT_FOUND, "Không tìm thấy phòng học.");
            if (phong.TrangThai != TrangThaiHoatDong.DangHoatDong)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học đã ngừng hoạt động.");

            // Môn phải thuộc khóa của lớp
            var km = await _khoaHocMonHocRepository.GetAsync(lop.KhoaHocId, dto.MonHocId);
            if (km == null)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Môn học không thuộc chương trình đào tạo của khóa học này.");

            // Ngày học trong thời gian lớp
            if (dto.NgayHoc.Date < lop.NgayBatDau.Date)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, $"Ngày học ({dto.NgayHoc:dd/MM/yyyy}) không được trước ngày bắt đầu của lớp ({lop.NgayBatDau:dd/MM/yyyy}).");

            if (lop.NgayKetThuc.HasValue && dto.NgayHoc.Date > lop.NgayKetThuc.Value.Date)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, $"Ngày học ({dto.NgayHoc:dd/MM/yyyy}) không được sau ngày kết thúc của lớp ({lop.NgayKetThuc.Value:dd/MM/yyyy}).");

            // Sức chứa phòng
            if (phong.SucChua < lop.SiSoToiDa)
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, $"Sức chứa phòng học ({phong.SucChua} chỗ) nhỏ hơn sĩ số tối đa của lớp ({lop.SiSoToiDa} học viên).");

            // Xung đột
            if (await _repository.HasConflictPhongAsync(dto.PhongHocId, dto.NgayHoc, dto.GioBatDau, dto.GioKetThuc, dto.Id))
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Phòng học đã có lịch xếp trong khung giờ này.");

            if (await _repository.HasConflictGiangVienAsync(dto.GiangVienId, dto.NgayHoc, dto.GioBatDau, dto.GioKetThuc, dto.Id))
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Giảng viên đã có lịch dạy trong khung giờ này.");

            if (await _repository.HasConflictLopHocAsync(dto.LopHocId, dto.NgayHoc, dto.GioBatDau, dto.GioKetThuc, dto.Id))
                return OperationResult<LichHocDto>.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION, "Lớp học đã có buổi học khác trong khung giờ này.");

            dto.MaLopHoc = lop.MaLopHoc;
            dto.TenLopHoc = lop.TenLopHoc;
            dto.MaMonHoc = mon.MaMonHoc;
            dto.TenMonHoc = mon.TenMonHoc;
            dto.MaGiangVien = gv.MaGiangVien;
            dto.HoTenGiangVien = gv.HoTen;
            dto.MaPhong = phong.MaPhong;
            dto.TenPhong = phong.TenPhong;
            dto.TrangThai = TrangThaiHoatDong.DangHoatDong;

            var result = await _repository.UpdateAtomicAsync(dto);
            if (!result.Success)
            {
                return OperationResult<LichHocDto>.Failed(result.ErrorCode ?? ErrorCodes.BUSINESS_RULE_VIOLATION, result.ErrorMessage ?? "Không thể cập nhật lịch học.");
            }

            return OperationResult<LichHocDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi cập nhật lịch học ID={dto.Id}.", ex);
            return OperationResult<LichHocDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể cập nhật lịch học.");
        }
    }

    public async Task<OperationResult<bool>> DeleteAsync(int id)
    {
        var auth = AuthorizationGuard.EnsurePermission<bool>(
            _currentUserService, Permission.TrainingManage, "Bạn không có quyền quản lý đào tạo.", false);
        if (!auth.IsSuccess) return auth;

        try
        {
            // Soft delete: Ngừng hoạt động lịch học thay vì hard-delete
            var ok = await _repository.DeleteAsync(id);
            if (!ok)
                return OperationResult<bool>.Failed(ErrorCodes.NOT_FOUND, $"Không tìm thấy lịch học ID={id}.", false);

            return OperationResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi hủy/ngừng hoạt động lịch học ID={id}.", ex);
            return OperationResult<bool>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể hủy buổi học.", false);
        }
    }

    private static OperationResult<LichHocDto> ValidateLichHocBasic(LichHocDto dto)
    {
        if (dto.LopHocId <= 0)
            return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng chọn lớp học hợp lệ.");

        if (dto.MonHocId <= 0)
            return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng chọn môn học hợp lệ.");

        if (dto.GiangVienId <= 0)
            return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng chọn giảng viên hợp lệ.");

        if (dto.PhongHocId <= 0)
            return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Vui lòng chọn phòng học hợp lệ.");

        if (dto.GioKetThuc <= dto.GioBatDau)
            return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Giờ kết thúc phải lớn hơn giờ bắt đầu.");

        if (!string.IsNullOrWhiteSpace(dto.NoiDung) && dto.NoiDung.Trim().Length > 250)
            return OperationResult<LichHocDto>.Failed(ErrorCodes.VALIDATION_ERROR, "Nội dung buổi học không được vượt quá 250 ký tự.");

        return OperationResult<LichHocDto>.Success(dto);
    }
}
