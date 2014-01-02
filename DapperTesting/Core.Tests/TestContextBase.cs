using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperTesting.Core.Tests
{
    internal abstract class TestContextBase : IDisposable
    {
        private const string ScriptsDirectory = @"..\..\..\..\DatabaseScripts\";
        private readonly static string[] _createScripts = { "000-Create-DapperTesting.sql" };
        private static readonly string[] _deleteScripts = { "000-Drop-DapperTesting.sql" };
        private readonly static string _dbFile = Path.Combine(Environment.CurrentDirectory, "IntegrationDB.mdf");
        private static readonly string _connectionString = @"Data Source=(localdb)\V11.0; Initial Catalog=DapperTesting; Integrated Security=true;AttachDbFileName=" + _dbFile;
        private static readonly Func<string, bool> _startsBatch = s => s.StartsWith("GO", StringComparison.OrdinalIgnoreCase);
        private static readonly Func<string, bool> _isUse = s => s.StartsWith("USE", StringComparison.OrdinalIgnoreCase);
        private static readonly Func<string, bool> _isSet = s => s.StartsWith("SET", StringComparison.OrdinalIgnoreCase);
        private static readonly Func<string, bool> _isComment = s => s.StartsWith("/*") && s.EndsWith("*/");

        static TestContextBase()
        {
            var deleteScripts = _deleteScripts.Select(s => Path.Combine(ScriptsDirectory, s)).ToList();
            var createScripts = _createScripts.Select(s => Path.Combine(ScriptsDirectory, s)).ToList();

            Assert.IsTrue(deleteScripts.All(File.Exists));
            Assert.IsTrue(createScripts.All(File.Exists));

            ExecuteScripts(deleteScripts);
            ExecuteScripts(createScripts);
        }

        private static void ExecuteScripts(IEnumerable<string> scripts)
        {
            Func<string, bool> isValidCommand = c => !_isUse(c) && !_isSet(c) && !_isComment(c);
            using (var conn = GetConnection())
            {
                conn.Open();
                foreach (var script in scripts)
                {
                    var commands = File.ReadAllLines(script).Where(isValidCommand).ToList();
                    IEnumerable<IEnumerable<string>> batches = CreateBatches(commands).ToList();
                    foreach (var batch in batches)
                    {
                        using (var query = conn.CreateCommand())
                        {
                            query.CommandText = string.Join(Environment.NewLine, batch);
                            query.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private static IEnumerable<IEnumerable<string>> CreateBatches(IEnumerable<string> commands)
        {
            var batch = new List<string>();
            foreach (var command in commands)
            {
                if (_startsBatch(command))
                {
                    if (batch.Any())
                    {
                        yield return batch;
                        batch = new List<string>();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(command))
                {
                    batch.Add(command);
                }
            }

            if (batch.Any())
            {
                yield return batch;
            }
        }

        protected static DbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TestContextBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                var deleter = new DatabaseDeleter(_connectionString);
                deleter.DeleteAllData();
            }

            _disposed = true;
        }
    }
}
