using Domain.Entities;
using Infrastructure;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class Utilities
    {
        public static void InitializeDbForTests(SocialDbcontext db)
        {
            db.Posts.AddRange(GetSeedingMessages());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(SocialDbcontext db)
        {
            db.Posts.RemoveRange(db.Posts);
            InitializeDbForTests(db);
        }

        public static List<Post> GetSeedingMessages()
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
