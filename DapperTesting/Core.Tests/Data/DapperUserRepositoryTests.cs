using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public void When_a_non_existent_user_is_retrieved_by_id_the_value_returned_is_null()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser(0);

            repository.Create(user);

            var retrievedUser = repository.Get(1);

            Assert.IsNull(retrievedUser);
        }

        [TestMethod]
        public void When_a_new_user_is_retrieved_by_id_it_is_the_correct_user()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser(0);
            var otherUser = _c.CreateStandardUser(1);

            repository.Create(user);
            repository.Create(otherUser);

            var retrievedUser = repository.Get(user.Id);

            Assert.AreEqual(user.Id, retrievedUser.Id);
        }

        [TestMethod]
        public void When_a_non_existent_user_is_retrieved_by_email_the_value_returned_is_null()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser(0);

            repository.Create(user);

            var retrievedUser = repository.Get(_c.CreateEmail(1));

            Assert.IsNull(retrievedUser);
        }

        [TestMethod]
        public void When_a_new_user_is_retrieved_by_email_it_is_the_correct_user()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser(0);
            var otherUser = _c.CreateStandardUser(1);

            repository.Create(user);
            repository.Create(otherUser);

            var retrievedUser = repository.Get(user.Email);

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
        [ExpectedException(typeof(SqlException))]
        public void When_a_user_with_a_conflicting_email_address_is_created_an_exception_is_thrown()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();
            var otherUser = _c.CreateStandardUser(1);
            otherUser.Email = user.Email;

            repository.Create(user);

            repository.Create(otherUser);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void When_a_user_with_a_conflicting_displayname_is_created_an_exception_is_thrown()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();
            var otherUser = _c.CreateStandardUser(1);
            otherUser.DisplayName = user.DisplayName;

            repository.Create(user);

            repository.Create(otherUser);
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
        public void When_an_existing_user_is_deleted_the_return_value_is_true()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            repository.Create(user);
            var success = repository.Delete(user.Id);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void When_a_nonexistent_user_is_deleted_the_return_value_is_false()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            repository.Create(user);
            var success = repository.Delete(1);

            Assert.IsFalse(success);
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

        [TestMethod]
        public void When_an_existing_user_is_updated_the_return_value_is_true()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            repository.Create(user);

            var success = repository.Update(user);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void When_a_nonexistent_user_is_updated_the_return_value_is_false()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();
            var otherUser = _c.CreateStandardUser(1);

            repository.Create(user);

            var success = repository.Update(otherUser);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void When_the_display_name_is_updated_the_new_value_has_been_persisted()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();
            var newName = user.DisplayName + "new";

            repository.Create(user);

            user.DisplayName = newName;

            repository.Update(user);

            var retrievedUser = repository.Get(user.Id);

            Assert.AreEqual(newName, retrievedUser.DisplayName);
        }

        [TestMethod]
        public void When_the_email_is_updated_the_new_value_has_been_persisted()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();
            var newEmail = _c.CreateEmail(1);

            repository.Create(user);

            user.Email = newEmail;

            repository.Update(user);

            var retrievedUser = repository.Get(user.Id);

            Assert.AreEqual(newEmail, retrievedUser.Email);
        }

        [TestMethod]
        public void When_the_active_bit_is_updated_the_new_value_has_been_persisted()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            repository.Create(user);

            user.Active = false;

            repository.Update(user);

            var retrievedUser = repository.Get(user.Id);

            Assert.IsFalse(retrievedUser.Active);
        }

        [TestMethod]
        public void When_a_user_is_updated_with_an_existing_email_the_value_returned_is_false_and_the_user_is_not_updated()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();
            var otherUser = _c.CreateStandardUser(1);

            repository.Create(user);
            repository.Create(otherUser);

            var oldName = user.DisplayName;
            user.Email = otherUser.Email;
            user.DisplayName = user.DisplayName + "-new";

            var success = repository.Update(user);

            var retrievedUser = repository.Get(user.Id);

            Assert.IsFalse(success);
            Assert.AreEqual(oldName, retrievedUser.DisplayName);
        }

        [TestMethod]
        public void When_a_user_is_updated_with_an_existing_displayname_the_value_returned_is_false_and_the_user_is_not_updated()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();
            var otherUser = _c.CreateStandardUser(1);

            repository.Create(user);
            repository.Create(otherUser);

            var oldEmail = user.Email;
            user.Email = _c.CreateEmail(2);
            user.DisplayName = otherUser.DisplayName;

            var success = repository.Update(user);

            var retrievedUser = repository.Get(user.Id);

            Assert.IsFalse(success);
            Assert.AreEqual(oldEmail, retrievedUser.Email);
        }

        [TestMethod]
        public void When_a_user_with_a_new_created_date_is_updated_the_value_returned_is_true_and_the_user_is_not_updated()
        {
            var repository = _c.GetRepository();
            var user = _c.CreateStandardUser();

            repository.Create(user);

            var oldDate = _c.RoundToSecond(user.CreatedDate);
            user.CreatedDate = DateTime.Now.AddDays(-1);

            var success = repository.Update(user);

            var retrievedUser = repository.Get(user.Id);

            Assert.IsTrue(success);
            Assert.AreEqual(oldDate, _c.RoundToSecond(retrievedUser.CreatedDate));
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
            private const string ConnectionStringName = "UserConnectionString";

            public IUserRepository GetRepository()
            {
                var connectionFactory = CreateConnectionFactory(ConnectionStringName);

                return new DapperUserRepository(connectionFactory, ConnectionStringName);
            }
        }
    }
}
