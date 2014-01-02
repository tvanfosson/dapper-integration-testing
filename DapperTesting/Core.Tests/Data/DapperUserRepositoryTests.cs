using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        private static readonly object _userLock = new object();

        [TestMethod]
        public void When_a_new_user_is_created_the_data_is_inserted()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            repository.Create(user);

            var retrievedUser = repository.Get(user.Id);

            Assert.AreEqual(user.DisplayName, retrievedUser.DisplayName);
            Assert.AreEqual(user.Email, retrievedUser.Email);
            Assert.AreEqual(user.Active, retrievedUser.Active);
        }

        [TestMethod]
        public void When_a_new_user_is_created_the_current_time_is_set_as_the_created_date()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            var before = _c.RoundToSecond(DateTime.Now);
            repository.Create(user);
            var after = _c.RoundToSecond(DateTime.Now);

            var retrievedUser = repository.Get(user.Id);

            var created = _c.RoundToSecond(retrievedUser.CreatedDate);

            // we only care about the nearest second
            Assert.IsTrue(before <= created && created <= after);
        }

        [TestMethod]
        public void When_a_user_is_deleted_get_returns_null()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            repository.Create(user);
            repository.Delete(user.Id);

            var retrievedUser = repository.Get(user.Id);

            Assert.IsNull(retrievedUser);
        }

        [TestInitialize]
        public void Init()
        {
            Start();
            Monitor.Enter(_userLock);
            _c = new DapperUserRepositoryTestContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_c != null)
            {
                _c.Dispose();
            }
            Monitor.Exit(_userLock);
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

            public User CreateStandardUser()
            {
                return new User
                {
                    DisplayName = "UserName",
                    Email = "username@example.com",
                    Active = true
                };
            }
        }
    }
}
