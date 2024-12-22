using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class SocialDbcontext : DbContext
    {
       public SocialDbcontext(DbContextOptions options) : base(options) { }

       public DbSet<Post> Posts { get; set; }
    }
}
