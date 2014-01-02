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

        [TestMethod]
        public void When_a_new_user_is_created_the_current_time_is_set_as_the_created_date()
        {
            var respository = _c.GetRepository();
            var user = new User
            {
                DisplayName = "UserName",
                Email = "username@example.com",
                Active = true
            };

            var before = _c.RoundToSecond(DateTime.Now);
            respository.Create(user);
            var after = _c.RoundToSecond(DateTime.Now);

            var retrievedUser = respository.Get(user.Id);

            var created = _c.RoundToSecond(retrievedUser.CreatedDate);

            // we only care about the nearest second
            Assert.IsTrue(before <= created && created <= after);
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

            public DateTime RoundToSecond(DateTime input)
            {
                return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, input.Second);
            }
        }
    }
}
