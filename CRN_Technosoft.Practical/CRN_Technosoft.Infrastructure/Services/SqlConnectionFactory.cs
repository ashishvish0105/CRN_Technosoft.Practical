using Microsoft.Data.SqlClient;

namespace CRN_Technosoft.Infrastructure.Services;
public sealed class SqlConnectionFactory(string connectionString)
{
    public SqlConnection Create() => new(connectionString);
}

    