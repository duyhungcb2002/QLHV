using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Infrastructure.SqlServer;

namespace QLHV.Tests;

[TestClass]
public class SqlConnectionFactoryTests
{
    private const string SampleValidConnectionString = "Server=localhost;Database=QLHV;Trusted_Connection=True;TrustServerCertificate=True";

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("\t\r\n")]
    public void Constructor_WithInvalidString_ThrowsArgumentException(string? invalidConnectionString)
    {
        var ex = Assert.Throws<ArgumentException>(() => new SqlConnectionFactory(invalidConnectionString!));
        Assert.IsNotNull(ex);
        Assert.IsTrue(ex.Message.Contains("ConnectionString", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new SqlConnectionFactory((SqlServerOptions)null!));
        Assert.IsNotNull(ex);
        Assert.AreEqual("options", ex.ParamName);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_WithOptionsHavingInvalidConnectionString_ThrowsArgumentException(string? invalidConnStr)
    {
        var options = new SqlServerOptions { ConnectionString = invalidConnStr! };
        Assert.Throws<ArgumentException>(() => new SqlConnectionFactory(options));
    }

    [TestMethod]
    public void CreateConnection_ReturnsDbConnection_OfConcreteTypeSqlConnection()
    {
        var factory = new SqlConnectionFactory(SampleValidConnectionString);

        using DbConnection conn = factory.CreateConnection();

        Assert.IsNotNull(conn);
        Assert.IsInstanceOfType<SqlConnection>(conn);
    }

    [TestMethod]
    public void CreateConnection_StateIsClosed()
    {
        var factory = new SqlConnectionFactory(SampleValidConnectionString);

        using DbConnection conn = factory.CreateConnection();

        Assert.AreEqual(ConnectionState.Closed, conn.State);
    }

    [TestMethod]
    public void CreateConnection_ConnectionStringIsPreserved()
    {
        var factory = new SqlConnectionFactory(SampleValidConnectionString);

        using DbConnection conn = factory.CreateConnection();

        Assert.AreEqual(SampleValidConnectionString, conn.ConnectionString);
    }

    [TestMethod]
    public void CreateConnection_WithOptions_ConnectionStringIsPreserved()
    {
        var options = new SqlServerOptions { ConnectionString = SampleValidConnectionString };
        var factory = new SqlConnectionFactory(options);

        using DbConnection conn = factory.CreateConnection();

        Assert.AreEqual(SampleValidConnectionString, conn.ConnectionString);
    }

    [TestMethod]
    public void CreateConnection_DoesNotAttemptToOpen_WorksEvenWithUnreachableServer()
    {
        // Connection string pointing to non-existent server
        var dummyConnStr = "Server=192.0.2.1;Database=NonExistentDb;Connection Timeout=1;Trusted_Connection=True;";
        var factory = new SqlConnectionFactory(dummyConnStr);

        // CreateConnection must succeed synchronously without attempting to connect or timeout
        using DbConnection conn = factory.CreateConnection();

        Assert.IsNotNull(conn);
        Assert.AreEqual(ConnectionState.Closed, conn.State);
    }

    [TestMethod]
    public void Dispose_Connection_DoesNotThrow()
    {
        var factory = new SqlConnectionFactory(SampleValidConnectionString);
        var conn = factory.CreateConnection();

        // Disposing unopened connection should be safe and idempotent
        conn.Dispose();
        conn.Dispose();
    }
}
