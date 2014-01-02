using System.Collections.Generic;
using DapperTesting.Core.Model;

namespace DapperTesting.Core.Data
{
    public interface IPostRepository
    {
        void Create(Post post);
        void AddDetails(int postId, PostDetails details);
        void Delete(int postId);
        void DeleteDetails(int detailsId);
        List<Post> GetPostsForUser(int userId);
        Post Get(int id);
        PostDetails GetDetails(int postId, int sequence);
        void Update(Post post);
        void UpdateDetails(PostDetails details);
    }
}
