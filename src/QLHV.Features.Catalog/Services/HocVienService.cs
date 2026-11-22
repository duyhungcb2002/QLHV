using System.Text.RegularExpressions;
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
    private readonly ITaiKhoanRepository _taiKhoanRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppLogger _logger;

    public HocVienService(
        IHocVienRepository repository,
        ICurrentUserService currentUserService,
        IAppLogger logger,
        ITaiKhoanRepository taiKhoanRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _taiKhoanRepository = taiKhoanRepository ?? throw new ArgumentNullException(nameof(taiKhoanRepository));
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
            var projectedList = list.Select(ProjectHocVienForCurrentUser).ToList();
            return OperationResult<IReadOnlyList<HocVienDto>>.Success(projectedList);
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
            return OperationResult<HocVienDto>.Success(ProjectHocVienForCurrentUser(item));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi khi lấy học viên ID={id}.", ex);
            return OperationResult<HocVienDto>.Failed(ErrorCodes.UNEXPECTED_ERROR, "Không thể lấy thông tin học viên.");
        }
    }

    private HocVienDto ProjectHocVienForCurrentUser(HocVienDto source)
    {
        var clone = CloneHocVien(source);
        bool canManage = _currentUserService.HasPermission(Permission.CatalogManage);
        if (!canManage)
        {
            clone.SoDinhDanh = HocVienDto.MaskSoDinhDanh(source.SoDinhDanh);
        }
        return clone;
    }

    private static HocVienDto CloneHocVien(HocVienDto src) => new()
    {
        Id = src.Id,
        MaHocVien = src.MaHocVien,
        HoTen = src.HoTen,
        NgaySinh = src.NgaySinh,
        GioiTinh = src.GioiTinh,
        DienThoai = src.DienThoai,
        Email = src.Email,
        DiaChi = src.DiaChi,
        NgayDangKy = src.NgayDangKy,
        TrangThai = src.TrangThai,
        SoDinhDanh = src.SoDinhDanh,
        NgayCapGiayTo = src.NgayCapGiayTo,
        NoiCapGiayTo = src.NoiCapGiayTo,
        HoTenNguoiGiamHo = src.HoTenNguoiGiamHo,
        DienThoaiNguoiGiamHo = src.DienThoaiNguoiGiamHo,
        QuanHeNguoiGiamHo = src.QuanHeNguoiGiamHo,
        NhanVienPhuTrachTaiKhoanId = src.NhanVienPhuTrachTaiKhoanId,
        TenNhanVienPhuTrach = src.TenNhanVienPhuTrach,
        NguonTuyenSinh = src.NguonTuyenSinh,
        NgheNghiep = src.NgheNghiep,
        DonViCongTac = src.DonViCongTac,
        GhiChuDacBiet = src.GhiChuDacBiet
    };

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

            SanitizeDto(dto);

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

            SanitizeDto(dto);

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

    private static void SanitizeDto(HocVienDto dto)
    {
        dto.MaHocVien = dto.MaHocVien.Trim();
        dto.HoTen = dto.HoTen.Trim();
        dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        dto.DienThoai = string.IsNullOrWhiteSpace(dto.DienThoai) ? null : dto.DienThoai.Trim();
        dto.DiaChi = string.IsNullOrWhiteSpace(dto.DiaChi) ? null : dto.DiaChi.Trim();
        dto.GioiTinh = string.IsNullOrWhiteSpace(dto.GioiTinh) ? null : dto.GioiTinh.Trim();

        dto.SoDinhDanh = string.IsNullOrWhiteSpace(dto.SoDinhDanh) ? null : dto.SoDinhDanh.Trim();
        dto.NoiCapGiayTo = string.IsNullOrWhiteSpace(dto.NoiCapGiayTo) ? null : dto.NoiCapGiayTo.Trim();
        dto.HoTenNguoiGiamHo = string.IsNullOrWhiteSpace(dto.HoTenNguoiGiamHo) ? null : dto.HoTenNguoiGiamHo.Trim();
        dto.DienThoaiNguoiGiamHo = string.IsNullOrWhiteSpace(dto.DienThoaiNguoiGiamHo) ? null : dto.DienThoaiNguoiGiamHo.Trim();
        dto.QuanHeNguoiGiamHo = string.IsNullOrWhiteSpace(dto.QuanHeNguoiGiamHo) ? null : dto.QuanHeNguoiGiamHo.Trim();
        dto.NguonTuyenSinh = string.IsNullOrWhiteSpace(dto.NguonTuyenSinh) ? null : dto.NguonTuyenSinh.Trim();
        dto.NgheNghiep = string.IsNullOrWhiteSpace(dto.NgheNghiep) ? null : dto.NgheNghiep.Trim();
        dto.DonViCongTac = string.IsNullOrWhiteSpace(dto.DonViCongTac) ? null : dto.DonViCongTac.Trim();
        dto.GhiChuDacBiet = string.IsNullOrWhiteSpace(dto.GhiChuDacBiet) ? null : dto.GhiChuDacBiet.Trim();
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

        // --- CÁC RÀNG BUỘC MỞ RỘNG (STUDENT-PROFILE-AND-TRAINING-UI-05) ---

        // 1. Giới hạn độ dài các trường mới
        if (!string.IsNullOrWhiteSpace(dto.SoDinhDanh) && dto.SoDinhDanh.Trim().Length > CatalogFieldLimits.SoDinhDanh)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Số định danh không được vượt quá {CatalogFieldLimits.SoDinhDanh} ký tự.");
        }
        if (!string.IsNullOrWhiteSpace(dto.NoiCapGiayTo) && dto.NoiCapGiayTo.Trim().Length > CatalogFieldLimits.NoiCapGiayTo)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Nơi cấp giấy tờ không được vượt quá {CatalogFieldLimits.NoiCapGiayTo} ký tự.");
        }
        if (!string.IsNullOrWhiteSpace(dto.HoTenNguoiGiamHo) && dto.HoTenNguoiGiamHo.Trim().Length > CatalogFieldLimits.HoTenNguoiGiamHo)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Họ tên người giám hộ không được vượt quá {CatalogFieldLimits.HoTenNguoiGiamHo} ký tự.");
        }
        if (!string.IsNullOrWhiteSpace(dto.DienThoaiNguoiGiamHo) && dto.DienThoaiNguoiGiamHo.Trim().Length > CatalogFieldLimits.DienThoaiNguoiGiamHo)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Điện thoại người giám hộ không được vượt quá {CatalogFieldLimits.DienThoaiNguoiGiamHo} ký tự.");
        }
        if (!string.IsNullOrWhiteSpace(dto.QuanHeNguoiGiamHo) && dto.QuanHeNguoiGiamHo.Trim().Length > CatalogFieldLimits.QuanHeNguoiGiamHo)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Quan hệ người giám hộ không được vượt quá {CatalogFieldLimits.QuanHeNguoiGiamHo} ký tự.");
        }
        if (!string.IsNullOrWhiteSpace(dto.NguonTuyenSinh) && dto.NguonTuyenSinh.Trim().Length > CatalogFieldLimits.NguonTuyenSinh)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Nguồn tuyển sinh không được vượt quá {CatalogFieldLimits.NguonTuyenSinh} ký tự.");
        }
        if (!string.IsNullOrWhiteSpace(dto.NgheNghiep) && dto.NgheNghiep.Trim().Length > CatalogFieldLimits.NgheNghiep)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Nghề nghiệp không được vượt quá {CatalogFieldLimits.NgheNghiep} ký tự.");
        }
        if (!string.IsNullOrWhiteSpace(dto.DonViCongTac) && dto.DonViCongTac.Trim().Length > CatalogFieldLimits.DonViCongTac)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Đơn vị công tác không được vượt quá {CatalogFieldLimits.DonViCongTac} ký tự.");
        }
        if (!string.IsNullOrWhiteSpace(dto.GhiChuDacBiet) && dto.GhiChuDacBiet.Trim().Length > CatalogFieldLimits.GhiChuDacBiet)
        {
            return OperationResult.Failed(ErrorCodes.INVALID_RANGE, $"Ghi chú đặc biệt không được vượt quá {CatalogFieldLimits.GhiChuDacBiet} ký tự.");
        }

        // 2. Nhóm giấy tờ định danh (SoDinhDanh, NgayCapGiayTo, NoiCapGiayTo):
        // Nếu nhập bất kỳ trường nào trong 3 trường thì cả 3 trường đều bắt buộc
        bool hasSoDinhDanh = !string.IsNullOrWhiteSpace(dto.SoDinhDanh);
        bool hasNgayCap = dto.NgayCapGiayTo.HasValue;
        bool hasNoiCap = !string.IsNullOrWhiteSpace(dto.NoiCapGiayTo);

        if (hasSoDinhDanh || hasNgayCap || hasNoiCap)
        {
            if (!hasSoDinhDanh || !hasNgayCap || !hasNoiCap)
            {
                return OperationResult.Failed(ErrorCodes.VALIDATION_ERROR,
                    "Thông tin định danh (Số CCCD/CMND, Ngày cấp, Nơi cấp) phải được nhập đầy đủ nếu đã nhập một trong ba thông tin này.");
            }

            var soDinhDanhClean = dto.SoDinhDanh!.Trim();
            // Số định danh phải là 9 hoặc 12 chữ số
            if (!Regex.IsMatch(soDinhDanhClean, @"^(\d{9}|\d{12})$"))
            {
                return OperationResult.Failed(ErrorCodes.INVALID_FORMAT,
                    "Số CCCD/CMND phải gồm đúng 9 hoặc 12 chữ số.");
            }

            // Kiểm tra trùng số định danh (nếu có nhập)
            var dupSoDinhDanh = await _repository.GetBySoDinhDanhAsync(soDinhDanhClean);
            if (dupSoDinhDanh != null && (isCreating || dupSoDinhDanh.Id != dto.Id))
            {
                return OperationResult.Failed(ErrorCodes.DUPLICATE_CODE,
                    $"Số định danh '{soDinhDanhClean}' đã tồn tại trên hồ sơ của học viên '{dupSoDinhDanh.HoTen}' ({dupSoDinhDanh.MaHocVien}).");
            }

            // Ngày cấp không được lớn hơn ngày hiện tại
            if (dto.NgayCapGiayTo!.Value.Date > DateTime.Today)
            {
                return OperationResult.Failed(ErrorCodes.INVALID_RANGE,
                    "Ngày cấp giấy tờ không được lớn hơn ngày hiện tại.");
            }

            // Ngày cấp không được trước ngày sinh của học viên (nếu có ngày sinh)
            if (dto.NgaySinh.HasValue && dto.NgayCapGiayTo.Value.Date < dto.NgaySinh.Value.Date)
            {
                return OperationResult.Failed(ErrorCodes.INVALID_RANGE,
                    "Ngày cấp giấy tờ không được trước ngày sinh của học viên.");
            }
        }

        // 3. Quy tắc người giám hộ: Nếu học viên dưới 18 tuổi thì bắt buộc nhập đầy đủ người giám hộ
        if (dto.NgaySinh.HasValue)
        {
            var birthDate = dto.NgaySinh.Value.Date;
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;

            if (age < 18)
            {
                if (string.IsNullOrWhiteSpace(dto.HoTenNguoiGiamHo) ||
                    string.IsNullOrWhiteSpace(dto.DienThoaiNguoiGiamHo) ||
                    string.IsNullOrWhiteSpace(dto.QuanHeNguoiGiamHo))
                {
                    return OperationResult.Failed(ErrorCodes.REQUIRED,
                        "Học viên dưới 18 tuổi bắt buộc phải có đầy đủ thông tin người giám hộ (Họ tên, Điện thoại, Quan hệ).");
                }
            }
        }

        // Kiểm tra định dạng điện thoại người giám hộ nếu có nhập
        if (!string.IsNullOrWhiteSpace(dto.DienThoaiNguoiGiamHo) && !ValidationHelper.IsValidPhoneNumber(dto.DienThoaiNguoiGiamHo))
        {
            return OperationResult.Failed(ErrorCodes.INVALID_FORMAT,
                "Số điện thoại người giám hộ chỉ được chứa chữ số và các ký tự hợp lệ (+, -, khoảng trắng).");
        }

        // 4. Kiểm tra nguồn tuyển sinh nằm trong danh sách chuẩn (nếu có chọn)
        if (!string.IsNullOrWhiteSpace(dto.NguonTuyenSinh))
        {
            var nguon = dto.NguonTuyenSinh.Trim();
            if (!NguonTuyenSinhDefaults.All.Any(x => string.Equals(x, nguon, StringComparison.OrdinalIgnoreCase)))
            {
                return OperationResult.Failed(ErrorCodes.INVALID_FORMAT,
                    $"Nguồn tuyển sinh '{nguon}' không nằm trong danh mục chuẩn quy định.");
            }
        }

        // 5. Kiểm tra nhân viên phụ trách: nếu có gán tài khoản thì tài khoản phải tồn tại, đang hoạt động, có vai trò ADMIN hoặc DAOTAO
        if (dto.NhanVienPhuTrachTaiKhoanId.HasValue)
        {
            var tk = await _taiKhoanRepository.GetByIdAsync(dto.NhanVienPhuTrachTaiKhoanId.Value);
            if (tk == null || tk.TrangThai != TrangThaiTaiKhoan.HoatDong)
            {
                return OperationResult.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION,
                    "Nhân viên phụ trách được chọn không tồn tại hoặc đã ngừng hoạt động.");
            }

            if (tk.VaiTro != VaiTro.ADMIN && tk.VaiTro != VaiTro.DAOTAO)
            {
                return OperationResult.Failed(ErrorCodes.BUSINESS_RULE_VIOLATION,
                    "Tài khoản phụ trách học viên phải có vai trò Quản trị viên (ADMIN) hoặc Nhân viên Đào tạo (DAOTAO).");
            }
        }

        return OperationResult.Success();
    }
}
