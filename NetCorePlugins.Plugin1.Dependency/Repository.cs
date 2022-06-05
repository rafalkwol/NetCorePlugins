using Dapper;
using Microsoft.Data.SqlClient;

namespace NetCorePlugins.Plugin1.Dependency
{
    public class Repository
    {
        public int GetValue(string connectionString)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                return sqlConnection.QuerySingle<int>(@"SELECT 5");
            }
        }
    }
}