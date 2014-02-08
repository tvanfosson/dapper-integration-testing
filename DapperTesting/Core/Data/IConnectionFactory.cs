using System.Data.Common;

namespace DapperTesting.Core.Data
{
    public interface IConnectionFactory
    {
        DbConnection Create(string connectionStringName);
    }
}
