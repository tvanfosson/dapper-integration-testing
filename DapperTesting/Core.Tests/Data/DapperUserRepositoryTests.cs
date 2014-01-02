using System;
using System.Collections.Generic;
using System.Linq;
using DapperTesting.Core.Data;
using DapperTesting.Core.Model;

using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperTesting.Core.Tests.Data
{
    [TestClass]
    public class DapperUserRepositoryTests : TestBase
    {
        private DapperUserRepositoryTestContext _c;

        [TestMethod]
        public void When_a_new_user_is_created_the_data_is_inserted()
        {
            var respository = _c.GetRepository();
            var user = new User
            {
                DisplayName = "UserName",
                Email = "username@example.com",
                Active = true
            };

            respository.Create(user);

            var retrievedUser = respository.Get(user.Id);

            Assert.AreEqual(user.DisplayName, retrievedUser.DisplayName);
            Assert.AreEqual(user.Email, retrievedUser.Email);
            Assert.AreEqual(user.Active, retrievedUser.Active);
        }

        [TestInitialize]
        public void Init()
        {
            Start();
            _c = new DapperUserRepositoryTestContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_c != null)
            {
                _c.Dispose();
            }
            End();
        }

        private class DapperUserRepositoryTestContext : TestContextBase
        {
            private const string _connectionString = "UserConnectionString";

            public IUserRepository GetRepository()
            {
                var connectionFactory = A.Fake<IConnectionFactory>();

                A.CallTo(() => connectionFactory.Create(_connectionString)).ReturnsLazily(f => GetConnection());

                return new DapperUserRepository(connectionFactory, _connectionString);
            }
        }
    }
}
