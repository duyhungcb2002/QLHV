using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Core.Logging;

namespace QLHV.Tests;

[TestClass]
public class FileAppLoggerTests
{
    [TestMethod]
    public void FileAppLogger_Redacts_Password_In_Exception_Message()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "qlhv_log_test_" + Guid.NewGuid().ToString("N"));
        try
        {
            var logger = new FileAppLogger(tempDir);
            var secret = "SuperSecret123";
            var ex = new InvalidOperationException($"Lỗi kết nối cơ sở dữ liệu với Password={secret};Server=localhost;");

            logger.LogError("Thử nghiệm ghi log lỗi bảo mật", ex);

            var logFiles = Directory.GetFiles(tempDir, "*.txt");
            Assert.IsNotEmpty(logFiles);

            var logContent = File.ReadAllText(logFiles[0]);

            Assert.DoesNotContain(secret, logContent);
            Assert.Contains("[REDACTED]", logContent);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [TestMethod]
    public void FileAppLogger_Redacts_Sensitive_Keys_In_Inner_Exception_And_Message()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "qlhv_log_test_" + Guid.NewGuid().ToString("N"));
        try
        {
            var logger = new FileAppLogger(tempDir);
            var innerSecret = "InnerSecret456";
            var userSecret = "admin_user_999";
            var innerEx = new ArgumentException($"Inner fail with Pwd={innerSecret}");
            var outerEx = new InvalidOperationException($"Outer fail with User ID={userSecret}", innerEx);

            logger.LogError("Ghi log với Connection String: Server=srv;UID=myuser;Password=topsecret;", outerEx);

            var logFiles = Directory.GetFiles(tempDir, "*.txt");
            Assert.IsNotEmpty(logFiles);

            var logContent = File.ReadAllText(logFiles[0]);

            Assert.DoesNotContain(innerSecret, logContent);
            Assert.DoesNotContain(userSecret, logContent);
            Assert.DoesNotContain("topsecret", logContent);
            Assert.Contains("[REDACTED]", logContent);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [TestMethod]
    public void FileAppLogger_Sanitize_Preserves_Database_Name_While_Redacting_Credentials()
    {
        var input = "Server=localhost;Database=QLHV;User Id=sa;Password=MySecretPassword!;Connect Timeout=5";
        var sanitized = FileAppLogger.Sanitize(input);

        Assert.Contains("Database=QLHV", sanitized);
        Assert.DoesNotContain("MySecretPassword!", sanitized);
        Assert.DoesNotContain("sa", sanitized);
        Assert.Contains("[REDACTED]", sanitized);
    }

    [TestMethod]
    public void FileAppLogger_Sanitize_Redacts_Quoted_Password_With_Spaces()
    {
        var inputDouble = @"Server=localhost;Database=QLHV;Password=""Super Secret"";Connect Timeout=5";
        var sanitizedDouble = FileAppLogger.Sanitize(inputDouble);

        Assert.Contains("Database=QLHV", sanitizedDouble);
        Assert.DoesNotContain("Super Secret", sanitizedDouble);
        Assert.DoesNotContain("Super", sanitizedDouble);
        Assert.DoesNotContain("Secret", sanitizedDouble);
        Assert.Contains("Password=[REDACTED]", sanitizedDouble);

        var inputSingle = "Server=localhost;Database=QLHV;Password='Secret With Space';Connect Timeout=5";
        var sanitizedSingle = FileAppLogger.Sanitize(inputSingle);

        Assert.Contains("Database=QLHV", sanitizedSingle);
        Assert.DoesNotContain("Secret With Space", sanitizedSingle);
        Assert.DoesNotContain("Space", sanitizedSingle);
        Assert.Contains("Password=[REDACTED]", sanitizedSingle);
    }

    [TestMethod]
    public void FileAppLogger_Sanitize_Redacts_Quoted_Password_With_Semicolons()
    {
        var input = @"Server=localhost;Database=QLHV;Password=""Secret;With;Semicolon"";Connect Timeout=5";
        var sanitized = FileAppLogger.Sanitize(input);

        Assert.Contains("Database=QLHV", sanitized);
        Assert.DoesNotContain("Secret;With;Semicolon", sanitized);
        Assert.DoesNotContain("Semicolon", sanitized);
        Assert.Contains("Password=[REDACTED]", sanitized);
    }

    [TestMethod]
    public void FileAppLogger_Sanitize_Redacts_Quoted_User_Id_With_Spaces()
    {
        var inputDouble = @"Server=localhost;Database=QLHV;User ID=""sql admin"";Password=123";
        var sanitizedDouble = FileAppLogger.Sanitize(inputDouble);

        Assert.DoesNotContain("sql admin", sanitizedDouble);
        Assert.DoesNotContain("sql", sanitizedDouble);
        Assert.Contains("User ID=[REDACTED]", sanitizedDouble);

        var inputSingle = "Server=localhost;Database=QLHV;UID='user name';Password=123";
        var sanitizedSingle = FileAppLogger.Sanitize(inputSingle);

        Assert.DoesNotContain("user name", sanitizedSingle);
        Assert.Contains("UID=[REDACTED]", sanitizedSingle);
    }

    [TestMethod]
    public void FileAppLogger_Sanitize_Redacts_Quoted_Connection_String_With_Password_Having_Spaces()
    {
        var input = @"Lỗi khởi động Connection String=""Server=.;Database=QLHV;Password=Secret Value;"" tại Startup";
        var sanitized = FileAppLogger.Sanitize(input);

        Assert.DoesNotContain("Secret Value", sanitized);
        Assert.DoesNotContain("Secret", sanitized);
        Assert.Contains("Connection String=[REDACTED]", sanitized);
    }

    [TestMethod]
    public void FileAppLogger_Redacts_Complex_Quoted_Secrets_In_Exception_And_Inner_Exception()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "qlhv_log_complex_" + Guid.NewGuid().ToString("N"));
        try
        {
            var logger = new FileAppLogger(tempDir);

            var innerEx = new ArgumentException(@"Inner problem with Connection String=""Server=.;Database=QLHV;Password=Secret Value;"" and Pwd=""A B C"";");
            var outerEx = new InvalidOperationException(@"Outer connection failure User ID=""sql admin"";Password=""Super Secret"";", innerEx);

            logger.LogError("Ghi log kiểm thử toàn diện", outerEx);

            var logFiles = Directory.GetFiles(tempDir, "*.txt");
            Assert.IsNotEmpty(logFiles);

            var logContent = File.ReadAllText(logFiles[0]);

            // Xác nhận chuỗi secret hoàn chỉnh và từng phần đặc trưng không xuất hiện
            Assert.DoesNotContain("Super Secret", logContent);
            Assert.DoesNotContain("Super", logContent);
            Assert.DoesNotContain("sql admin", logContent);
            Assert.DoesNotContain("Secret Value", logContent);
            Assert.DoesNotContain("A B C", logContent);

            Assert.Contains("[REDACTED]", logContent);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
