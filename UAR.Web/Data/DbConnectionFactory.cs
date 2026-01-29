using Microsoft.Data.SqlClient;

namespace UAR.Web.Data;

public class DbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("UarDatabase");
        return new SqlConnection(connectionString);
    }
}
