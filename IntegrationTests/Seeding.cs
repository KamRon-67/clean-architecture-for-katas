using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IntegrationTests
{
    public class Seeding
    {
        public static void InitializeTestDB(SocialDbcontext db)
        {

            db.Posts.AddRange(GetPosts());
            db.SaveChanges();
        }

        private static List<Post> GetPosts()
        {
            return new List<Post>()
            {
              new Post { Id = 1, Comments = "value1", Content = "value2", DateCreated = DateTime.Now },
              new Post { Id = 2, Comments = "value3", Content = "value4", DateCreated = DateTime.Now },
              new Post { Id = 3, Comments = "value5", Content = "value6", DateCreated = DateTime.Now },
            };
        }
    }
}
