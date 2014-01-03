using System.Collections.Generic;
using DapperTesting.Core.Model;

namespace DapperTesting.Core.Data
{
    public interface IPostRepository
    {
        void Create(Post post);
        void AddDetails(int postId, PostDetails details);
        bool Delete(int postId);
        bool DeleteDetails(int detailsId);
        List<Post> GetPostsForUser(int userId);
        Post Get(int id);
        PostDetails GetDetails(int postId, int sequence);
        bool Update(Post post);
        bool UpdateDetails(PostDetails details);
    }
}
