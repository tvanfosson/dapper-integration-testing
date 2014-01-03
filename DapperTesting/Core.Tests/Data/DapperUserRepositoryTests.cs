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
        public void When_a_new_user_is_retrieved_by_id_it_is_the_correct_user()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            repository.Create(user);

            var retrievedUser = repository.Get(user.Id);

            Assert.AreEqual(user.Id, retrievedUser.Id);
        }

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
        public void When_a_new_user_is_created_the_id_is_updated()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();
            var existingId = user.Id;

            repository.Create(user);

            Assert.AreNotEqual(existingId, user.Id);
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

        [TestMethod]
        public void When_all_users_are_retrieved_the_count_is_correct()
        {
            var repository = _c.GetRepository();
            const int userCount = 10;
            for (var i = 0; i < userCount; ++i)
            {
                repository.Create(_c.CreateStandardUser(i));
            }

            var users = repository.GetAll();

            Assert.AreEqual(userCount, users.Count);
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

            public User CreateStandardUser(int id = 0)
            {
                return new User
                {
                    DisplayName = "UserName" + id,
                    Email = string.Format("username{0}@example.com", id),
                    Active = true
                };
            }
        }
    }
}
