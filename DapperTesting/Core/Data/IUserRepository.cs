using System.Collections.Generic;
using DapperTesting.Core.Model;

namespace DapperTesting.Core.Data
{
    public interface IUserRepository
    {
        void Create(User user);
        bool Delete(int id);
        User Get(int id);
        User Get(string email);
        List<User> GetAll(); 
        bool Update(User user);
    }
}
