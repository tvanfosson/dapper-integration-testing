using System;
using System.Data;
using System.Data.Common;

namespace DapperTesting.Core.Data
{
    public abstract class DapperRepositoryBase
    {
        protected readonly IConnectionFactory _connectionFactory;
        protected readonly string _connectionStringName;

        protected DapperRepositoryBase(IConnectionFactory connectionFactory, string connectionStringName)
        {
            _connectionFactory = connectionFactory;
            _connectionStringName = connectionStringName;
        }

        protected DbConnection OpenConnection()
        {
            var connection = _connectionFactory.Create(_connectionStringName);
            connection.Open();
            return connection;
        }

        protected T Fetch<T>(Func<DbConnection, T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            using (var connection = OpenConnection())
            {
                return func(connection);
            }
        }

        protected int Execute(Func<DbConnection, int> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            using (var connection = OpenConnection())
            {
                return func(connection);
            }
        }

        protected int ExecuteTransaction(Func<DbConnection, IDbTransaction, int> func, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            using (var connection = OpenConnection())
            using (var transaction = connection.BeginTransaction(isolationLevel))
            {
                var value = func(connection, transaction);

                transaction.Commit();

                return value;
            }
        }
    }
}
