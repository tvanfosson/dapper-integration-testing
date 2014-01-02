using System;
using System.Collections.Generic;
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

        public void Delete(int id)
        {
            const string sql = "DELETE FROM [Users] WHERE [Id] = @userId";
            Execute(c => c.Execute(sql, new { userId = id }));
        }

        public User Get(int id)
        {
            const string sql = "SELECT * FROM [Users] WHERE [Id] = @userId";
            var user = Fetch(c => c.Query<User>(sql, new { userId = id }).SingleOrDefault());
            return user;
        }

        public User Get(string email)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAll()
        {
            const string sql = "SELECT * FROM [Users]";
            var users = Fetch(c => c.Query<User>(sql));
            return users.ToList();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
