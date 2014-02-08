namespace DapperTesting.Core.Configuration
{
    public interface IConfiguration
    {
        string GetConnectionString(string connectionName);
    }
}
