using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public void AddDetails(int postId, PostDetails details)
        {
            throw new NotImplementedException();
        }

        public void Delete(int postId)
        {
            throw new NotImplementedException();
        }

        public void DeleteDetails(int detailsId)
        {
            throw new NotImplementedException();
        }

        public List<Post> GetPostsForUser(int userId)
        {
            throw new NotImplementedException();
        }

        public Post Get(int id)
        {
            throw new NotImplementedException();
        }

        public PostDetails GetDetails(int postId, int sequence)
        {
            throw new NotImplementedException();
        }

        public void Update(Post post)
        {
            throw new NotImplementedException();
        }

        public void UpdateDetails(PostDetails details)
        {
            throw new NotImplementedException();
        }
    }
}
