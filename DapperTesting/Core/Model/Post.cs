using System;

namespace DapperTesting.Core.Model
{
    public class Post
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime EditedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
