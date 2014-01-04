using System.Collections.Generic;
using DapperTesting.Core.Model;

namespace DapperTesting.Core.Data
{
    public interface IPostRepository
    {
        void Create(Post post);
        void AddDetail(PostDetail detail);
        bool Delete(int postId);
        bool DeleteDetail(int detailId);
        List<Post> GetPostsForUser(int userId);
        Post Get(int id);
        PostDetail GetDetail(int postId, int sequenceNumber);
        bool Update(Post post);
        bool UpdateDetail(PostDetail detail);
    }
}
