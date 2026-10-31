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
}
