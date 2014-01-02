using System.Data.Common;
using System.Data.SqlClient;

using DapperTesting.Core.Configuration;

namespace DapperTesting.Core.Data
{
    public class MsSqlConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public MsSqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbConnection Create(string connectionStringName)
        {
            return new SqlConnection(_configuration.GetConnectionString(connectionStringName));
        }
    }
}
