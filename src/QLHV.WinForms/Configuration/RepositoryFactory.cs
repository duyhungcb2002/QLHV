using QLHV.Core.Repositories;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Features.Training.Repositories.Fake;
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
            var trainingStore = new FakeTrainingStore();
            var hocVienRepo = new FakeHocVienRepository();
            var giangVienRepo = new FakeGiangVienRepository();
            var monHocRepo = new FakeMonHocRepository();
            var phongHocRepo = new FakePhongHocRepository();
            var khoaHocRepo = new FakeKhoaHocRepository();
            var khoaHocMonHocRepo = new FakeKhoaHocMonHocRepository();

            return new RepositoryBundle
            {
                TaiKhoanRepository = new FakeTaiKhoanRepository(passwordHasher),
                HocVienRepository = hocVienRepo,
                GiangVienRepository = giangVienRepo,
                MonHocRepository = monHocRepo,
                KhoaHocRepository = khoaHocRepo,
                KhoaHocMonHocRepository = khoaHocMonHocRepo,
                PhongHocRepository = phongHocRepo,

                LopHocRepository = new FakeLopHocRepository(trainingStore, phongHocRepo),
                DangKyLopHocRepository = new FakeDangKyLopHocRepository(trainingStore),
                LichHocRepository = new FakeLichHocRepository(trainingStore, monHocRepo, giangVienRepo, phongHocRepo, khoaHocMonHocRepo),
                KetQuaHocTapRepository = new FakeKetQuaHocTapRepository(trainingStore),
                PhieuThuRepository = new FakePhieuThuRepository(trainingStore),
                BaoCaoRepository = new FakeBaoCaoRepository(trainingStore, khoaHocMonHocRepo, monHocRepo, hocVienRepo, khoaHocRepo)
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
                PhongHocRepository = new SqlPhongHocRepository(connectionFactory),

                LopHocRepository = new SqlLopHocRepository(connectionFactory),
                DangKyLopHocRepository = new SqlDangKyLopHocRepository(connectionFactory),
                LichHocRepository = new SqlLichHocRepository(connectionFactory),
                KetQuaHocTapRepository = new SqlKetQuaHocTapRepository(connectionFactory),
                PhieuThuRepository = new SqlPhieuThuRepository(connectionFactory),
                BaoCaoRepository = new SqlBaoCaoRepository(connectionFactory)
            };
        }

        throw new InvalidOperationException($"Chế độ DataProvider '{config.DataProvider}' không được hỗ trợ.");
    }
}
