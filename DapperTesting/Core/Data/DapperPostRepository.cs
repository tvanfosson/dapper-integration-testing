using System;
using System.Collections.Generic;
using System.Linq;

using Dapper;

using DapperTesting.Core.Model;

namespace DapperTesting.Core.Data
{
    public class DapperPostRepository : DapperRepositoryBase, IPostRepository
    {
        public DapperPostRepository(IConnectionFactory connectionFactory, string connectionStringName) : base(connectionFactory, connectionStringName)
        {
        }

        public void Create(Post post)
        {
            var date = DateTime.Now;
            const string sql = "INSERT INTO [Posts] ([OwnerId], [Title], [Slug], [PostedDate], [EditedDate], [Deleted]) OUTPUT inserted.[Id] VALUES(@ownerId, @title, @slug, @postedDate, @editedDate, 0)";

            var id = Fetch(c => c.Query<int>(sql, new
            {
                ownerId = post.OwnerId,
                title = post.Title,
                slug = post.Slug,
                postedDate = date,
                editedDate = date
            })).Single();

            post.Id = id;
            post.PostedDate = date;
            post.EditedDate = date;
        }

        public void AddDetails(int postId, PostDetails details)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int postId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteDetails(int detailsId)
        {
            throw new NotImplementedException();
        }

        public List<Post> GetPostsForUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Post Get(int id)
        {
            const string sql = "SELECT * FROM [Posts] WHERE [Id] = @postId";

            var post = Fetch(c => c.Query<Post>(sql, new { postId = id })).SingleOrDefault();

            return post;
        }

        public PostDetails GetDetails(int postId, int sequence)
        {
            throw new NotImplementedException();
        }

        public bool Update(Post post)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDetails(PostDetails details)
        {
            throw new NotImplementedException();
        }
    }
}
