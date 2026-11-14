using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Infrastructure.SqlServer.Repositories;
using QLHV.WinForms.Configuration;

namespace QLHV.Tests;

[TestClass]
[DoNotParallelize]
public class AppConfigAndProviderTests
{
    private readonly IPasswordHasher _passwordHasher = new Pbkdf2PasswordHasher();

    [TestMethod]
    public void AppConfig_DisplayEnvironment_Returns_Correct_Label()
    {
        var sqlConfig = new AppConfig { DataProvider = DataProviderMode.SqlServer };
        var fakeConfig = new AppConfig { DataProvider = DataProviderMode.Fake };

        Assert.AreEqual("SQL Server", sqlConfig.DisplayEnvironment);
        Assert.AreEqual("Fake Data", fakeConfig.DisplayEnvironment);
    }

    [TestMethod]
    public void AppConfigLoader_SanitizeConnectionString_Hides_Credentials()
    {
        var rawCs = "Server=localhost\\SQLEXPRESS;Database=QLHV;User Id=sa;Password=SuperSecretPassword123!;Encrypt=False;";
        var sanitized = AppConfigLoader.SanitizeConnectionString(rawCs);

        Assert.Contains("Server=localhost\\SQLEXPRESS", sanitized);
        Assert.Contains("Database=QLHV", sanitized);
        Assert.DoesNotContain("SuperSecretPassword123!", sanitized);
        Assert.DoesNotContain("User Id=sa", sanitized);
    }

    [TestMethod]
    public void AppConfigLoader_SanitizeConnectionString_Handles_Invalid_Or_Null()
    {
        Assert.AreEqual("(không có chuỗi kết nối)", AppConfigLoader.SanitizeConnectionString(null));
        Assert.AreEqual("(không có chuỗi kết nối)", AppConfigLoader.SanitizeConnectionString(string.Empty));
        Assert.AreEqual("(chuỗi kết nối không đúng định dạng)", AppConfigLoader.SanitizeConnectionString("not a valid connection string;;;===;;"));
    }

    [TestMethod]
    public void AppConfigLoader_Loads_Fake_When_Configured()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var json = """
            {
              "DataProvider": "Fake"
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

            var config = AppConfigLoader.Load(tempDir, _ => null);

            Assert.AreEqual(DataProviderMode.Fake, config.DataProvider);
            Assert.AreEqual("Fake Data", config.DisplayEnvironment);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void AppConfigLoader_Throws_When_DataProvider_Missing()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var json = """
            {
              "ConnectionStrings": {
                "QLHV": "Server=.\\SQLEXPRESS;Database=QLHV;Integrated Security=True;"
              }
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

            var ex = Assert.Throws<InvalidOperationException>(() => AppConfigLoader.Load(tempDir, _ => null));
            StringAssert.Contains(ex.Message, "DataProvider");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void AppConfigLoader_Throws_When_DataProvider_Invalid()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var json = """
            {
              "DataProvider": "OracleDatabase"
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

            var ex = Assert.Throws<InvalidOperationException>(() => AppConfigLoader.Load(tempDir, _ => null));
            StringAssert.Contains(ex.Message, "OracleDatabase");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void AppConfigLoader_Throws_When_SqlServer_And_ConnectionString_Missing()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var json = """
            {
              "DataProvider": "SqlServer"
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

            var ex = Assert.Throws<InvalidOperationException>(() => AppConfigLoader.Load(tempDir, _ => null));
            StringAssert.Contains(ex.Message, "Chuỗi kết nối SQL Server");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void AppConfigLoader_Throws_When_SqlServer_And_ConnectionString_Malformed()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var json = """
            {
              "DataProvider": "SqlServer",
              "ConnectionStrings": {
                "QLHV": "InvalidKeyWords=Malformed;;===;;"
              }
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

            var ex = Assert.Throws<InvalidOperationException>(() => AppConfigLoader.Load(tempDir, _ => null));
            Assert.IsNotNull(ex);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void AppConfigLoader_Throws_When_SqlServer_Points_To_QLHV_Test()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var json = """
            {
              "DataProvider": "SqlServer",
              "ConnectionStrings": {
                "QLHV": "Server=.\\SQLEXPRESS;Database=QLHV_Test;Integrated Security=True;Encrypt=False;"
              }
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

            var ex = Assert.Throws<InvalidOperationException>(() => AppConfigLoader.Load(tempDir, _ => null));
            StringAssert.Contains(ex.Message, "QLHV_Test");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void AppConfigLoader_Throws_When_SqlServer_Points_To_Wrong_Database()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var json = """
            {
              "DataProvider": "SqlServer",
              "ConnectionStrings": {
                "QLHV": "Server=.\\SQLEXPRESS;Database=Master;Integrated Security=True;Encrypt=False;"
              }
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

            var ex = Assert.Throws<InvalidOperationException>(() => AppConfigLoader.Load(tempDir, _ => null));
            StringAssert.Contains(ex.Message, "Database cho ứng dụng chính phải là 'QLHV'");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void AppConfigLoader_Does_Not_Fallback_To_Fake_On_Invalid_SqlServer_Config()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var json = """
            {
              "DataProvider": "SqlServer",
              "ConnectionStrings": {
                "QLHV": ""
              }
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), json);

            // Bắt buộc phải ném ngoại lệ, tuyệt đối không âm thầm trả về DataProviderMode.Fake
            Assert.Throws<InvalidOperationException>(() => AppConfigLoader.Load(tempDir, _ => null));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void Fake_Provider_Does_Not_Open_Sql_Connection()
    {
        // Khi cấu hình Fake, truyền chuỗi kết nối SQL hỏng hoàn toàn
        var fakeConfig = new AppConfig
        {
            DataProvider = DataProviderMode.Fake,
            ConnectionString = "Server=DeadServerThatDoesNotExist999;Database=GhostDB;Connect Timeout=1;"
        };

        // Phải tạo thành công ngay lập tức mà không thử mở kết nối SQL
        var bundle = RepositoryFactory.CreateRepositories(fakeConfig, _passwordHasher);

        Assert.IsNotNull(bundle);
        Assert.IsInstanceOfType<FakeTaiKhoanRepository>(bundle.TaiKhoanRepository);
        Assert.IsInstanceOfType<FakeHocVienRepository>(bundle.HocVienRepository);
    }

    [TestMethod]
    public void AppConfigLoader_LocalSettings_Overrides_BaseSettings()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var baseJson = """
            {
              "DataProvider": "Fake",
              "ConnectionStrings": {
                "QLHV": "Server=base;Database=QLHV;"
              }
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), baseJson);

            var localJson = """
            {
              "DataProvider": "SqlServer",
              "ConnectionStrings": {
                "QLHV": "Server=.\\SQLEXPRESS;Database=QLHV;Integrated Security=True;Encrypt=False;"
              }
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.Local.json"), localJson);

            var config = AppConfigLoader.Load(tempDir, _ => null);

            Assert.AreEqual(DataProviderMode.SqlServer, config.DataProvider);
            Assert.AreEqual("Server=.\\SQLEXPRESS;Database=QLHV;Integrated Security=True;Encrypt=False;", config.ConnectionString);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void AppConfigLoader_EnvironmentVariables_Override_Files()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            var fileJson = """
            {
              "DataProvider": "SqlServer",
              "ConnectionStrings": {
                "QLHV": "Server=.\\SQLEXPRESS;Database=QLHV;Integrated Security=True;Encrypt=False;"
              }
            }
            """;
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), fileJson);

            var env = new Dictionary<string, string?>
            {
                [AppConfigLoader.EnvDataProviderKey] = "Fake",
                [AppConfigLoader.EnvConnectionStringKey] = "Server=overridden;Database=QLHV;"
            };

            var config = AppConfigLoader.Load(tempDir, key => env.GetValueOrDefault(key));

            Assert.AreEqual(DataProviderMode.Fake, config.DataProvider);
            Assert.AreEqual("Server=overridden;Database=QLHV;", config.ConnectionString);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [TestMethod]
    public void Environment_Variables_Are_Restored_Properly_In_Finally()
    {
        var origProvider = Environment.GetEnvironmentVariable(AppConfigLoader.EnvDataProviderKey);
        var origConn = Environment.GetEnvironmentVariable(AppConfigLoader.EnvConnectionStringKey);

        try
        {
            Environment.SetEnvironmentVariable(AppConfigLoader.EnvDataProviderKey, "Fake");
            Environment.SetEnvironmentVariable(AppConfigLoader.EnvConnectionStringKey, "Server=TestEnv;Database=QLHV;");

            var providerVal = Environment.GetEnvironmentVariable(AppConfigLoader.EnvDataProviderKey);
            Assert.AreEqual("Fake", providerVal);
        }
        finally
        {
            Environment.SetEnvironmentVariable(AppConfigLoader.EnvDataProviderKey, origProvider);
            Environment.SetEnvironmentVariable(AppConfigLoader.EnvConnectionStringKey, origConn);
        }

        Assert.AreEqual(origProvider, Environment.GetEnvironmentVariable(AppConfigLoader.EnvDataProviderKey));
        Assert.AreEqual(origConn, Environment.GetEnvironmentVariable(AppConfigLoader.EnvConnectionStringKey));
    }

    [TestMethod]
    public void RepositoryFactory_Creates_Fake_Repositories()
    {
        var config = new AppConfig { DataProvider = DataProviderMode.Fake };
        var bundle = RepositoryFactory.CreateRepositories(config, _passwordHasher);

        Assert.IsNotNull(bundle);
        Assert.IsInstanceOfType<FakeTaiKhoanRepository>(bundle.TaiKhoanRepository);
        Assert.IsInstanceOfType<FakeHocVienRepository>(bundle.HocVienRepository);
        Assert.IsInstanceOfType<FakeGiangVienRepository>(bundle.GiangVienRepository);
        Assert.IsInstanceOfType<FakeMonHocRepository>(bundle.MonHocRepository);
        Assert.IsInstanceOfType<FakeKhoaHocRepository>(bundle.KhoaHocRepository);
        Assert.IsInstanceOfType<FakeKhoaHocMonHocRepository>(bundle.KhoaHocMonHocRepository);
        Assert.IsInstanceOfType<FakePhongHocRepository>(bundle.PhongHocRepository);
    }

    [TestMethod]
    public void RepositoryFactory_Creates_SqlServer_Repositories()
    {
        var config = new AppConfig
        {
            DataProvider = DataProviderMode.SqlServer,
            ConnectionString = "Server=.\\SQLEXPRESS;Database=QLHV;Integrated Security=True;Encrypt=False;"
        };

        var bundle = RepositoryFactory.CreateRepositories(config, _passwordHasher);

        Assert.IsNotNull(bundle);
        Assert.IsInstanceOfType<SqlTaiKhoanRepository>(bundle.TaiKhoanRepository);
        Assert.IsInstanceOfType<SqlHocVienRepository>(bundle.HocVienRepository);
        Assert.IsInstanceOfType<SqlGiangVienRepository>(bundle.GiangVienRepository);
        Assert.IsInstanceOfType<SqlMonHocRepository>(bundle.MonHocRepository);
        Assert.IsInstanceOfType<SqlKhoaHocRepository>(bundle.KhoaHocRepository);
        Assert.IsInstanceOfType<SqlKhoaHocMonHocRepository>(bundle.KhoaHocMonHocRepository);
        Assert.IsInstanceOfType<SqlPhongHocRepository>(bundle.PhongHocRepository);
    }

    [TestMethod]
    public void StartupConnectionTester_Rejects_QLHV_Test()
    {
        var testCs = "Server=.\\SQLEXPRESS;Database=QLHV_Test;Integrated Security=True;Encrypt=False;";
        var result = StartupConnectionTester.TestConnection(testCs);

        Assert.IsFalse(result.IsSuccess);
        StringAssert.Contains(result.ErrorMessage, "QLHV_Test");
    }

    [TestMethod]
    public void StartupConnectionTester_Fails_On_Invalid_Server()
    {
        var invalidCs = "Server=NonExistentServer12345;Database=QLHV;Integrated Security=True;Encrypt=False;Connect Timeout=1;";
        var result = StartupConnectionTester.TestConnection(invalidCs);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }
}
