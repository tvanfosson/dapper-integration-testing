using System;

namespace DapperTesting.Core.Model
{
    public class User
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public int PostCount { get; set; }
    }
}
