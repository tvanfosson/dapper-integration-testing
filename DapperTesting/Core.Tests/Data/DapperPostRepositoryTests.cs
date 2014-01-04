using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperTesting.Core.Data;
using DapperTesting.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DapperTesting.Core.Tests.Data
{
    [TestClass]
    public class DapperPostRepositoryTests : TestBase
    {
        private DapperPostRepositoryTestContext _c;

        [TestMethod]
        public void When_a_new_post_is_created_the_values_are_stored_in_the_database()
        {
            var testUser = _c.CreateTestUser();

            var repository = _c.GetRepository();
            var post = _c.CreatePost(testUser);

            repository.Create(post);

            var createdPost = repository.Get(post.Id);

            Assert.AreEqual(post.Title, createdPost.Title);
            Assert.AreEqual(post.Slug, createdPost.Slug);
            Assert.AreEqual(post.OwnerId, createdPost.OwnerId);
            Assert.AreEqual(post.Deleted, createdPost.Deleted);
        }

        [TestMethod]
        public void When_a_new_post_is_created_as_deleted_the_deleted_value_is_stored_in_the_database()
        {
            var testUser = _c.CreateTestUser();

            var repository = _c.GetRepository();
            var post = _c.CreatePost(testUser);
            post.Deleted = true;

            repository.Create(post);

            var createdPost = repository.Get(post.Id);

            Assert.AreEqual(post.Deleted, createdPost.Deleted);
        }

        [TestMethod]
        public void When_a_new_post_is_created_the_posted_date_and_edited_dates_are_updated()
        {
            var testUser = _c.CreateTestUser();

            var repository = _c.GetRepository();
            var post = _c.CreatePost(testUser);

            var before = _c.RoundToSecond(DateTime.Now);
            repository.Create(post);
            var after = _c.RoundToSecond(DateTime.Now);

            var posted = _c.RoundToSecond(post.PostedDate);
            var edited = _c.RoundToSecond(post.EditedDate);

            Assert.IsTrue(before <= posted && posted <= after);
            Assert.IsTrue(before <= edited && edited <= after);
        }

        [TestMethod]
        public void When_a_new_post_detail_is_created_the_values_are_stored_in_the_database()
        {
            var testUser = _c.CreateTestUser();

            var repository = _c.GetRepository();
            var post = _c.CreatePost(testUser);

            repository.Create(post);

            const int sequenceNumber = 0;
            var detail = new PostDetail
            {
                PostId = post.Id,
                SequenceNumber = sequenceNumber,
                Text = "details"
            };

            repository.AddDetail(detail);

            var createdDetail = repository.GetDetail(post.Id, sequenceNumber);

            Assert.AreEqual(detail.Text, createdDetail.Text);
        }

        [TestInitialize]
        public void Init()
        {
            Start();

            _c = new DapperPostRepositoryTestContext();
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

        private class DapperPostRepositoryTestContext : TestContextBase
        {
            private const string UserConnectionStringName = "UserConnectionString";
            private const string PostConnectionStringName = "PostConnectionString";
            private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, string>> _slugs = new ConcurrentDictionary<int, ConcurrentDictionary<string, string>>();

            private IUserRepository GetUserRepository()
            {
                var connectionFactory = CreateConnectionFactory(UserConnectionStringName);

                return new DapperUserRepository(connectionFactory, UserConnectionStringName);
            }

            private string CreateSlug(int id, string title)
            {
                var builder = new StringBuilder(title.Length + 4);

                builder.Append(id);
                builder.Append('-');

                var previousChar = '\0';
                foreach (var c in title)
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        builder.Append(c);
                        previousChar = c;
                    }
                    else if (previousChar != '-' && (c == ' ' || c == '-'))
                    {
                        builder.Append('-');
                        previousChar = '-';
                    }
                }

                return builder.ToString();
            }

            private string GetCachedSlug(int id, string title)
            {
                var ownerDictionary = _slugs.GetOrAdd(id, i => new ConcurrentDictionary<string, string>());
                return ownerDictionary.GetOrAdd(title, t => CreateSlug(id, t));
            }

            public IPostRepository GetRepository()
            {
                var connectionFactory = CreateConnectionFactory(PostConnectionStringName);

                return new DapperPostRepository(connectionFactory, PostConnectionStringName);
            }

            public Post CreatePost(User owner, string title = "This is my post title")
            {
                return new Post
                {
                    OwnerId = owner.Id,
                    Title = title,
                    Slug = GetCachedSlug(owner.Id, title)
                };
            }

            public User CreateTestUser()
            {
                var repository = GetUserRepository();

                var user = CreateStandardUser(1000);

                repository.Create(user);

                return user;
            }
        }
    }
}
