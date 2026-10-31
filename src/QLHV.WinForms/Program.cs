using QLHV.Core.Logging;
using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Core.Services;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Features.Catalog.Services;
using QLHV.WinForms.Forms;

namespace QLHV.WinForms;

/// <summary>
/// Điểm khởi động ứng dụng và Composition Root cho toàn bộ hệ thống WinForms
/// Khởi tạo các dependencies và tiêm (inject) qua constructor, không dùng Service Locator
/// </summary>
static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // 1. Khởi tạo Core Infrastructure (Logging, Security)
        IAppLogger logger = new FileAppLogger();
        IPasswordHasher passwordHasher = new Pbkdf2PasswordHasher();

        // 2. Khởi tạo Repositories (Hiện tại dùng In-Memory Fake; Sau này Chu Đức thay bằng ADO.NET SQL Server)
        ITaiKhoanRepository taiKhoanRepository = new FakeTaiKhoanRepository(passwordHasher);
        IHocVienRepository hocVienRepository = new FakeHocVienRepository();
        IGiangVienRepository giangVienRepository = new FakeGiangVienRepository();
        IMonHocRepository monHocRepository = new FakeMonHocRepository();
        IKhoaHocRepository khoaHocRepository = new FakeKhoaHocRepository();
        IKhoaHocMonHocRepository khoaHocMonHocRepository = new FakeKhoaHocMonHocRepository();
        IPhongHocRepository phongHocRepository = new FakePhongHocRepository();

        // 3. Khởi tạo Phân quyền và Quản lý phiên làm việc
        IAuthorizationService authorizationService = new AuthorizationService();
        ICurrentUserService currentUserService = new CurrentUserService(authorizationService);

        // 4. Khởi tạo Application Services
        IAuthService authService = new AuthService(taiKhoanRepository, passwordHasher, currentUserService, logger);
        IHocVienService hocVienService = new HocVienService(hocVienRepository, currentUserService, logger);
        IGiangVienService giangVienService = new GiangVienService(giangVienRepository, currentUserService, logger);
        IMonHocService monHocService = new MonHocService(monHocRepository, currentUserService, logger);
        IKhoaHocService khoaHocService = new KhoaHocService(khoaHocRepository, currentUserService, logger);
        IKhoaHocMonHocService khoaHocMonHocService = new KhoaHocMonHocService(khoaHocMonHocRepository, khoaHocRepository, monHocRepository, currentUserService, logger);
        IPhongHocService phongHocService = new PhongHocService(phongHocRepository, currentUserService, logger);

        logger.LogInfo("Khởi động ứng dụng Hệ thống Quản lý học viên (QLHV).");

        // 5. Vòng lặp điều hướng Đăng nhập <-> Màn hình chính
        while (true)
        {
            using var loginForm = new FrmLogin(authService);
            var loginResult = loginForm.ShowDialog();

            if (loginResult != DialogResult.OK)
            {
                // Người dùng bấm Thoát hoặc đóng form đăng nhập
                break;
            }

            using var mainForm = new FrmMain(
                currentUserService,
                authService,
                hocVienService,
                giangVienService,
                monHocService,
                khoaHocService,
                khoaHocMonHocService,
                phongHocService);

            Application.Run(mainForm);

            // Nếu người dùng không bấm Đăng xuất mà đóng cửa sổ FormMain thì thoát ứng dụng
            if (!mainForm.IsLoggedOut)
            {
                break;
            }
        }

        logger.LogInfo("Thoát ứng dụng Hệ thống Quản lý học viên (QLHV).");
    }
}