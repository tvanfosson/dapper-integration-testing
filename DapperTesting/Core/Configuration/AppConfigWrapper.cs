using System;
using System.Configuration;

namespace DapperTesting.Core.Configuration
{
    public class AppConfigWrapper : IConfiguration
    {
        public string GetConnectionString(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new ArgumentNullException("connectionName");
            }

            try
            {
                return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}
