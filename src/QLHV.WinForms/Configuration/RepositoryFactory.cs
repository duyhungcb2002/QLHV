using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Infrastructure.SqlServer;
using QLHV.Infrastructure.SqlServer.Repositories;

namespace QLHV.WinForms.Configuration;

/// <summary>
/// Khởi tạo tập repositories tương ứng theo chế độ cấu hình (Fake hoặc SqlServer)
/// Không sử dụng framework DI phức tạp để giữ composition root đơn giản cho sinh viên
/// </summary>
public static class RepositoryFactory
{
    public static RepositoryBundle CreateRepositories(AppConfig config, IPasswordHasher passwordHasher)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(passwordHasher);

        if (config.DataProvider == DataProviderMode.Fake)
        {
            return new RepositoryBundle
            {
                TaiKhoanRepository = new FakeTaiKhoanRepository(passwordHasher),
                HocVienRepository = new FakeHocVienRepository(),
                GiangVienRepository = new FakeGiangVienRepository(),
                MonHocRepository = new FakeMonHocRepository(),
                KhoaHocRepository = new FakeKhoaHocRepository(),
                KhoaHocMonHocRepository = new FakeKhoaHocMonHocRepository(),
                PhongHocRepository = new FakePhongHocRepository()
            };
        }

        if (config.DataProvider == DataProviderMode.SqlServer)
        {
            if (string.IsNullOrWhiteSpace(config.ConnectionString))
            {
                throw new InvalidOperationException("Chuỗi kết nối SQL Server không được để trống khi chạy chế độ SqlServer.");
            }

            var connectionFactory = new SqlConnectionFactory(config.ConnectionString);
            return new RepositoryBundle
            {
                TaiKhoanRepository = new SqlTaiKhoanRepository(connectionFactory),
                HocVienRepository = new SqlHocVienRepository(connectionFactory),
                GiangVienRepository = new SqlGiangVienRepository(connectionFactory),
                MonHocRepository = new SqlMonHocRepository(connectionFactory),
                KhoaHocRepository = new SqlKhoaHocRepository(connectionFactory),
                KhoaHocMonHocRepository = new SqlKhoaHocMonHocRepository(connectionFactory),
                PhongHocRepository = new SqlPhongHocRepository(connectionFactory)
            };
        }

        throw new InvalidOperationException($"Chế độ DataProvider '{config.DataProvider}' không được hỗ trợ.");
    }
}

