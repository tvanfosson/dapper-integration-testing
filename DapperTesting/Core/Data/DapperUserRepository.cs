using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperTesting.Core.Model;

namespace DapperTesting.Core.Data
{
    public class DapperUserRepository : DapperRepositoryBase, IUserRepository
    {
        public DapperUserRepository(IConnectionFactory connectionFactory, string connectionStringName)
            : base(connectionFactory, connectionStringName)
        {
        }

        public void Create(User user)
        {
            var date = DateTime.Now;

            const string sql = "INSERT INTO [Users] ([DisplayName], [Email], [CreatedDate], [Active]) OUTPUT inserted.[Id] VALUES (@displayName, @email, @createdDate, @active)";

            var id = Fetch(c => c.Query<int>(sql, new
            {
                displayName = user.DisplayName,
                email = user.Email,
                createdDate = date,
                active = user.Active
            }).Single());

            user.Id = id;
            user.CreatedDate = date;
        }

        public bool Delete(int id)
        {
            const string sql = "DELETE FROM [Users] WHERE [Id] = @userId";

            var deleted = Execute(c => c.Execute(sql, new { userId = id }));

            return deleted == 1;
        }

        public User Get(int id)
        {
            const string sql = "SELECT * FROM [Users] WHERE [Id] = @userId";

            var user = Fetch(c => c.Query<User>(sql, new { userId = id }).SingleOrDefault());

            return user;
        }

        public User Get2(int id)
        {
            const string sql = "SELECT * FROM [Users] WHERE [Id] = @userId";

            using (var connection = _connectionFactory.Create(_connectionStringName))
            {
                connection.Open();

                var user = connection.Query<User>(sql, new { userId = id }).SingleOrDefault();

                return user;
            }
        }

        public User Get(string email)
        {
            const string sql = "SELECT * FROM [Users] WHERE [Email] = @email";

            var user = Fetch(c => c.Query<User>(sql, new { email })).SingleOrDefault();

            return user;
        }

        public List<User> GetAll()
        {
            const string sql = "SELECT * FROM [Users]";

            var users = Fetch(c => c.Query<User>(sql));

            return users.ToList();
        }

        public bool Update(User user)
        {
            try
            {
                const string sql = "UPDATE [Users] SET [DisplayName] = @displayName, [Email] = @email, [Active] = @active WHERE [Id] = @userId";

                var updated = Execute(c => c.Execute(sql, new
                {
                    userId = user.Id,
                    displayName = user.DisplayName,
                    email = user.Email,
                    active = user.Active
                }));

                return updated == 1;
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}
