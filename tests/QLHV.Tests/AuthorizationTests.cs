using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Enums;
using QLHV.Core.Security;

namespace QLHV.Tests;

[TestClass]
public class AuthorizationTests
{
    private IAuthorizationService _authzService = null!;

    [TestInitialize]
    public void Setup()
    {
        _authzService = new AuthorizationService();
    }

    [TestMethod]
    public void Admin_HasAllPermissions()
    {
        var allPermissions = Enum.GetValues<Permission>();
        foreach (var p in allPermissions)
        {
            Assert.IsTrue(_authzService.HasPermission(VaiTro.ADMIN, p), $"ADMIN must have permission {p}");
        }
    }

    [TestMethod]
    public void DaoTao_HasCatalogAndTrainingPermissions()
    {
        Assert.IsTrue(_authzService.HasPermission(VaiTro.DAOTAO, Permission.CatalogView));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.DAOTAO, Permission.CatalogManage));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.DAOTAO, Permission.TrainingView));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.DAOTAO, Permission.TrainingManage));

        // DaoTao does NOT have TuitionManage or UserManage
        Assert.IsFalse(_authzService.HasPermission(VaiTro.DAOTAO, Permission.TuitionManage));
        Assert.IsFalse(_authzService.HasPermission(VaiTro.DAOTAO, Permission.UserManage));
    }

    [TestMethod]
    public void ThuNgan_CannotManageCatalog_HasTuitionManage()
    {
        Assert.IsTrue(_authzService.HasPermission(VaiTro.THUNGAN, Permission.CatalogView));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.THUNGAN, Permission.TuitionManage));

        // ThuNgan cannot manage catalog or training
        Assert.IsFalse(_authzService.HasPermission(VaiTro.THUNGAN, Permission.CatalogManage));
        Assert.IsFalse(_authzService.HasPermission(VaiTro.THUNGAN, Permission.TrainingManage));
    }

    [TestMethod]
    public void GiangVien_CannotManageUsersOrCatalog_HasGradeEntry()
    {
        Assert.IsTrue(_authzService.HasPermission(VaiTro.GIANGVIEN, Permission.CatalogView));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.GIANGVIEN, Permission.GradeEntry));

        Assert.IsFalse(_authzService.HasPermission(VaiTro.GIANGVIEN, Permission.CatalogManage));
        Assert.IsFalse(_authzService.HasPermission(VaiTro.GIANGVIEN, Permission.UserManage));
    }

    [TestMethod]
    public void QuanLy_CanViewReportsAndCatalog_CannotManageCatalog()
    {
        Assert.IsTrue(_authzService.HasPermission(VaiTro.QUANLY, Permission.CatalogView));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.QUANLY, Permission.ReportView));

        Assert.IsFalse(_authzService.HasPermission(VaiTro.QUANLY, Permission.CatalogManage));
        Assert.IsFalse(_authzService.HasPermission(VaiTro.QUANLY, Permission.TuitionManage));
    }

    [TestMethod]
    public void GetPermissions_ReturnsDefensiveCopy_ExternalMutationDoesNotAffectMatrix()
    {
        // 1. Lấy quyền của vai trò DAOTAO và ADMIN bằng GetPermissions()
        var daoTaoPermissions = _authzService.GetPermissions(VaiTro.DAOTAO);
        var adminPermissions = _authzService.GetPermissions(VaiTro.ADMIN);

        Assert.AreNotSame(daoTaoPermissions, _authzService.GetPermissions(VaiTro.DAOTAO));
        Assert.AreNotSame(adminPermissions, _authzService.GetPermissions(VaiTro.ADMIN));

        // 2. Thử sửa collection nhận được nếu kiểu collection cho phép
        if (daoTaoPermissions is ICollection<Permission> mutableDaoTao && !mutableDaoTao.IsReadOnly)
        {
            mutableDaoTao.Add(Permission.UserManage);
            mutableDaoTao.Add(Permission.TuitionManage);
            mutableDaoTao.Remove(Permission.CatalogView);
        }

        if (adminPermissions is ICollection<Permission> mutableAdmin && !mutableAdmin.IsReadOnly)
        {
            mutableAdmin.Clear();
        }

        // 3 & 4. Gọi lại HasPermission() và GetPermissions() - Ma trận quyền nội bộ vẫn không đổi
        var freshDaoTaoPermissions = _authzService.GetPermissions(VaiTro.DAOTAO);
        Assert.HasCount(4, freshDaoTaoPermissions);
        Assert.IsTrue(_authzService.HasPermission(VaiTro.DAOTAO, Permission.CatalogView));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.DAOTAO, Permission.CatalogManage));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.DAOTAO, Permission.TrainingView));
        Assert.IsTrue(_authzService.HasPermission(VaiTro.DAOTAO, Permission.TrainingManage));

        // 5. ADMIN vẫn có toàn bộ quyền ban đầu
        var allPermissions = Enum.GetValues<Permission>();
        var freshAdminPermissions = _authzService.GetPermissions(VaiTro.ADMIN);
        Assert.HasCount(allPermissions.Length, freshAdminPermissions);
        foreach (var p in allPermissions)
        {
            Assert.IsTrue(_authzService.HasPermission(VaiTro.ADMIN, p), $"ADMIN must retain permission {p}");
        }

        // 6. Các vai trò khác không nhận thêm quyền trái phép
        Assert.IsFalse(_authzService.HasPermission(VaiTro.DAOTAO, Permission.UserManage));
        Assert.IsFalse(_authzService.HasPermission(VaiTro.DAOTAO, Permission.TuitionManage));
        Assert.IsFalse(_authzService.HasPermission(VaiTro.THUNGAN, Permission.UserManage));
        Assert.IsFalse(_authzService.HasPermission(VaiTro.GIANGVIEN, Permission.UserManage));
        Assert.IsFalse(_authzService.HasPermission(VaiTro.QUANLY, Permission.UserManage));
    }
}
