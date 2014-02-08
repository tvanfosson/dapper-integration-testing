using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperTesting.Core.Tests
{
    /// <summary>
    /// Based on https://gist.github.com/jbogard/5805783 for nHibernate, adapted to use SqlConnection/SqlCommand.
    /// Queries the database and builds a set of delete commands that takes into account the relations
    /// between tables to clean all the data out after a test run.  The delete string is built once per
    /// database with the connection string used to distinguish between databases. You must use
    /// the same connection string if you are accessing the same database regardless of which
    /// tables your test will be using as the delete command will encompass all tables within
    /// the database.
    /// </summary>
    internal class DatabaseDeleter
    {
        private class Relationship
        {
            public string PrimaryKeyTable { get; set; }
            public string ForeignKeyTable { get; set; }
        }

        private class NotInitializedException : Exception
        {
            public NotInitializedException(string database)
                : base(string.Format("<{0}> has not been initialized", database))
            {
            }
        }

        private static readonly object _lockObj = new object();
        private static readonly string[] _ignoredTables = { "sysdiagrams" /* add migration tables if necessary */ };
        private readonly string _database;
        private IEnumerable<string> _tablesToDelete;
        private readonly static Dictionary<string, string> _deleteSql = new Dictionary<string, string>();
        private readonly static HashSet<string> _initialized = new HashSet<string>();

        public DatabaseDeleter(string database)
        {
            _database = database;
            BuildDeleteTables();
        }

        public void DeleteAllData()
        {
            string deleteSql;

            if (!_deleteSql.TryGetValue(_database, out deleteSql))
            {
                throw new NotInitializedException(_database);
            }

            using (var connection = new SqlConnection(_database))
            {
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteSql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<string> GetTables()
        {
            return _tablesToDelete;
        }

        private void BuildDeleteTables()
        {
            if (_initialized.Contains(_database))
            {
                return;
            }

            lock (_lockObj)
            {
                if (!_initialized.Add(_database))
                {
                    return;
                }

                using (var connection = new SqlConnection(_database))
                {
                    connection.Open();

                    var allTables = GetAllTables(connection);

                    var allRelationships = GetRelationships(connection);

                    _tablesToDelete = BuildTableList(allTables, allRelationships);

                    _deleteSql[_database] = BuildTableSql(_tablesToDelete);
                }
            }
        }

        private static string BuildTableSql(IEnumerable<string> tablesToDelete)
        {
            return string.Join(" ", tablesToDelete.Select(t => string.Format("delete from [{0}];", t)));
        }

        private static IEnumerable<string> BuildTableList(ICollection<string> allTables, ICollection<Relationship> allRelationships)
        {
            var tablesToDelete = new List<string>();

            while (allTables.Any())
            {
                var leafTables = allTables.Except(allRelationships
                                          .Select(rel => rel.PrimaryKeyTable))
                                          .ToArray();

                tablesToDelete.AddRange(leafTables);

                foreach (var leafTable in leafTables)
                {
                    allTables.Remove(leafTable);

                    var relToRemove = allRelationships.Where(rel => rel.ForeignKeyTable == leafTable)
                                                      .ToArray();

                    foreach (var rel in relToRemove)
                    {
                        allRelationships.Remove(rel);
                    }
                }
            }

            return tablesToDelete;
        }

        private static IList<Relationship> GetRelationships(IDbConnection connection)
        {
            var relationships = new List<Relationship>();
            using (var query = connection.CreateCommand())
            {
                query.CommandText = @"select
                                            so_pk.name as PrimaryKeyTable
                                        ,   so_fk.name as ForeignKeyTable
                                        from
	                                        sysforeignkeys sfk
	                                        inner join sysobjects so_pk on sfk.rkeyid = so_pk.id
	                                        inner join sysobjects so_fk on sfk.fkeyid = so_fk.id
                                        order by
                                                 so_pk.name
                                                ,so_fk.name";

                using (var reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var primaryKeyTable = reader.GetString(0);
                        var foreignKeyTable = reader.GetString(1);
                        relationships.Add(new Relationship
                        {
                            PrimaryKeyTable = primaryKeyTable,
                            ForeignKeyTable = foreignKeyTable
                        });
                    }
                }
                return relationships;
            }
        }

        private static IList<string> GetAllTables(IDbConnection connection)
        {
            using (var query = connection.CreateCommand())
            {
                query.CommandText = "select t.name from sys.tables t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'dbo'";
                var tables = new List<string>();
                using (var reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }

                return tables.Except(_ignoredTables).ToList();
            }
        }
    }
}
