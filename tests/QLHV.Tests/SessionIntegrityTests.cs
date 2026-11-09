using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Security;

namespace QLHV.Tests;

[TestClass]
public class SessionIntegrityTests
{
    private IAuthorizationService _authzService = null!;
    private ICurrentUserService _currentUserService = null!;

    [TestInitialize]
    public void Setup()
    {
        _authzService = new AuthorizationService();
        _currentUserService = new CurrentUserService(_authzService);
    }

    [TestMethod]
    public void CurrentUserService_ProtectsSessionFromExternalDtoMutation_AndClearsCorrectly()
    {
        // 1. Tạo người dùng có vai trò DAOTAO (GiangVienId != null)
        var originalLoginTime = new DateTime(2026, 1, 1, 8, 0, 0);
        var originalDto = new TaiKhoanDto
        {
            Id = 2,
            TenDangNhap = "daotao",
            VaiTro = VaiTro.DAOTAO,
            GiangVienId = 15,
            TrangThai = TrangThaiTaiKhoan.HoatDong,
            LanDangNhapCuoi = originalLoginTime
        };

        // 2. Gọi SetCurrentUser()
        _currentUserService.SetCurrentUser(originalDto);
        Assert.IsTrue(_currentUserService.IsAuthenticated);

        // 3. Sửa DTO ban đầu thành ADMIN và đổi cả GiangVienId, LanDangNhapCuoi
        originalDto.Id = 1;
        originalDto.TenDangNhap = "admin";
        originalDto.VaiTro = VaiTro.ADMIN;
        originalDto.GiangVienId = 99;
        originalDto.LanDangNhapCuoi = DateTime.Now;

        // 4. Phiên nội bộ vẫn là DAOTAO và giữ nguyên GiangVienId, LanDangNhapCuoi
        var sessionAfterCallerMutate = _currentUserService.CurrentUser;
        Assert.IsNotNull(sessionAfterCallerMutate);
        Assert.AreNotSame(originalDto, sessionAfterCallerMutate);
        Assert.AreEqual(2, sessionAfterCallerMutate.Id);
        Assert.AreEqual("daotao", sessionAfterCallerMutate.TenDangNhap);
        Assert.AreEqual(VaiTro.DAOTAO, sessionAfterCallerMutate.VaiTro);
        Assert.AreEqual(15, sessionAfterCallerMutate.GiangVienId);
        Assert.AreEqual(originalLoginTime, sessionAfterCallerMutate.LanDangNhapCuoi);
        Assert.IsFalse(_currentUserService.HasPermission(Permission.UserManage));

        // 5. Lấy DTO từ CurrentUser
        var returnedDto = _currentUserService.CurrentUser;
        Assert.IsNotNull(returnedDto);
        Assert.AreEqual(15, returnedDto.GiangVienId);
        Assert.AreEqual(originalLoginTime, returnedDto.LanDangNhapCuoi);

        // 6. Sửa DTO nhận được thành ADMIN và sửa GiangVienId, LanDangNhapCuoi
        returnedDto.Id = 1;
        returnedDto.TenDangNhap = "admin";
        returnedDto.VaiTro = VaiTro.ADMIN;
        returnedDto.TrangThai = TrangThaiTaiKhoan.BiKhoa;
        returnedDto.GiangVienId = 88;
        returnedDto.LanDangNhapCuoi = DateTime.MinValue;

        // 7. Phiên nội bộ vẫn là DAOTAO, GiangVienId và LanDangNhapCuoi không bị đổi
        var verifySession = _currentUserService.CurrentUser;
        Assert.IsNotNull(verifySession);
        Assert.AreNotSame(returnedDto, verifySession);
        Assert.AreEqual(2, verifySession.Id);
        Assert.AreEqual("daotao", verifySession.TenDangNhap);
        Assert.AreEqual(VaiTro.DAOTAO, verifySession.VaiTro);
        Assert.AreEqual(TrangThaiTaiKhoan.HoatDong, verifySession.TrangThai);
        Assert.AreEqual(15, verifySession.GiangVienId);
        Assert.AreEqual(originalLoginTime, verifySession.LanDangNhapCuoi);

        // 8. Quyền của phiên không bị thay đổi (không chiếm được quyền ADMIN)
        Assert.IsTrue(_currentUserService.HasPermission(Permission.CatalogView));
        Assert.IsTrue(_currentUserService.HasPermission(Permission.CatalogManage));
        Assert.IsTrue(_currentUserService.HasPermission(Permission.TrainingView));
        Assert.IsTrue(_currentUserService.HasPermission(Permission.TrainingManage));
        Assert.IsFalse(_currentUserService.HasPermission(Permission.UserManage));
        Assert.IsFalse(_currentUserService.HasPermission(Permission.TuitionManage));
        Assert.IsFalse(_currentUserService.HasPermission(Permission.GradeEntry));
        Assert.IsFalse(_currentUserService.HasPermission(Permission.ReportView));

        // 9. Clear() vẫn xóa phiên đúng
        _currentUserService.Clear();
        Assert.IsFalse(_currentUserService.IsAuthenticated);
        Assert.IsNull(_currentUserService.CurrentUser);
        Assert.IsFalse(_currentUserService.HasPermission(Permission.CatalogView));
    }
}
