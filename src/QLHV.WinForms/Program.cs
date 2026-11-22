using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Core.Services;
using QLHV.Features.Catalog.Services;
using QLHV.Features.Training.Services;
using QLHV.WinForms.Configuration;
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

        // 2. Nạp cấu hình ứng dụng (SqlServer hoặc Fake)
        AppConfig config;
        try
        {
            config = AppConfigLoader.Load();
        }
        catch (Exception ex)
        {
            logger.LogError("Không thể nạp cấu hình ứng dụng.", ex);
            var safeMessage = FileAppLogger.Sanitize(ex.Message);
            MessageBox.Show(
                $"Không thể khởi động ứng dụng do lỗi cấu hình:\n\n{safeMessage}\n\nVui lòng kiểm tra lại cấu hình trong appsettings.json hoặc biến môi trường.",
                "Lỗi cấu hình - QLHV",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        // 3. Nếu là SqlServer, kiểm tra kết nối tới cơ sở dữ liệu thật trước khi hiển thị form đăng nhập
        if (config.DataProvider == DataProviderMode.SqlServer)
        {
            var testResult = StartupConnectionTester.TestConnection(config.ConnectionString);
            if (!testResult.IsSuccess)
            {
                logger.LogError($"Kết nối SQL Server thất bại: {testResult.SanitizedInfo}. Chi tiết kỹ thuật: {testResult.ErrorMessage}");
                var userMsg = testResult.UserFriendlyMessage ?? "Không thể kết nối cơ sở dữ liệu.";
                MessageBox.Show(
                    $"Không thể kết nối đến cơ sở dữ liệu SQL Server:\n\nMục tiêu: {testResult.SanitizedInfo}\nThông báo: {userMsg}\n\nỨng dụng sẽ dừng khởi động. Vui lòng kiểm tra dịch vụ SQL Server và chuỗi kết nối trong appsettings.json.",
                    "Lỗi kết nối cơ sở dữ liệu - QLHV",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }

        // 4. Khởi tạo Repositories theo chế độ cấu hình
        RepositoryBundle repositories;
        try
        {
            repositories = RepositoryFactory.CreateRepositories(config, passwordHasher);
        }
        catch (Exception ex)
        {
            logger.LogError("Khởi tạo repositories thất bại.", ex);
            var safeMessage = FileAppLogger.Sanitize(ex.Message);
            MessageBox.Show(
                $"Khởi tạo kho dữ liệu thất bại:\n\n{safeMessage}",
                "Lỗi khởi động - QLHV",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        // 5. Khởi tạo Phân quyền và Quản lý phiên làm việc
        IAuthorizationService authorizationService = new AuthorizationService();
        ICurrentUserService currentUserService = new CurrentUserService(authorizationService);

        // 6. Khởi tạo Application Services
        IAuthService authService = new AuthService(repositories.TaiKhoanRepository, passwordHasher, currentUserService, logger);
        IHocVienService hocVienService = new HocVienService(repositories.HocVienRepository, currentUserService, logger, repositories.TaiKhoanRepository);
        IGiangVienService giangVienService = new GiangVienService(repositories.GiangVienRepository, currentUserService, logger);
        IMonHocService monHocService = new MonHocService(repositories.MonHocRepository, currentUserService, logger);
        IKhoaHocService khoaHocService = new KhoaHocService(repositories.KhoaHocRepository, currentUserService, logger);
        IKhoaHocMonHocService khoaHocMonHocService = new KhoaHocMonHocService(repositories.KhoaHocMonHocRepository, repositories.KhoaHocRepository, repositories.MonHocRepository, currentUserService, logger);
        IPhongHocService phongHocService = new PhongHocService(repositories.PhongHocRepository, currentUserService, logger);

        // Khởi tạo Operations Services
        var lopHocService = new LopHocService(repositories.LopHocRepository, repositories.KhoaHocRepository, repositories.GiangVienRepository, repositories.DangKyLopHocRepository, repositories.LichHocRepository, repositories.KetQuaHocTapRepository, currentUserService, logger);
        var dangKyService = new DangKyLopHocService(repositories.DangKyLopHocRepository, repositories.LopHocRepository, repositories.HocVienRepository, repositories.KhoaHocRepository, currentUserService, logger);
        var lichHocService = new LichHocService(repositories.LichHocRepository, repositories.LopHocRepository, repositories.MonHocRepository, repositories.GiangVienRepository, repositories.PhongHocRepository, repositories.KhoaHocMonHocRepository, currentUserService, logger);
        var ketQuaService = new KetQuaHocTapService(repositories.KetQuaHocTapRepository, repositories.DangKyLopHocRepository, repositories.LopHocRepository, repositories.MonHocRepository, repositories.KhoaHocMonHocRepository, repositories.LichHocRepository, currentUserService, logger);
        var hocPhiService = new HocPhiService(repositories.PhieuThuRepository, repositories.DangKyLopHocRepository, repositories.LopHocRepository, currentUserService, logger);
        var baoCaoService = new BaoCaoService(repositories.BaoCaoRepository, repositories.LopHocRepository, currentUserService, logger);

        var operationsBundle = new OperationsServiceBundle
        {
            LopHocService = lopHocService,
            DangKyLopHocService = dangKyService,
            LichHocService = lichHocService,
            KetQuaHocTapService = ketQuaService,
            HocPhiService = hocPhiService,
            BaoCaoService = baoCaoService
        };

        logger.LogInfo($"Khởi động ứng dụng Hệ thống Quản lý học viên (QLHV) - Môi trường: {config.DisplayEnvironment}.");

        // 7. Vòng lặp điều hướng Đăng nhập <-> Màn hình chính
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
                phongHocService,
                operationsBundle,
                config.DisplayEnvironment,
                repositories.TaiKhoanRepository);

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
