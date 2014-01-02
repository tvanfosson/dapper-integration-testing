using System;
using System.Collections.Generic;
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
            const string sql = "INSERT INTO [User] ([DisplayName], [Email], [CreatedDate], [Active]) VALUES (@displayName, @email, GETDATE(), @active)";
            Execute(c => c.Execute(sql, new
            {
                displayName = user.DisplayName,
                email = user.Email,
                active = user.Active
            }));
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM [User] WHERE [Id] = @userId";
            Execute(c => c.Execute(sql, new { userId = id }));
        }

        public User Get(int id)
        {
            throw new NotImplementedException();
        }

        public User Get(string email)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
