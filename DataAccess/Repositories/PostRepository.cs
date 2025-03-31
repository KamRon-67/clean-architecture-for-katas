using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly SocialDbcontext _socialDbcontext;

        public PostRepository(SocialDbcontext socialDbcontext)
        {
            _socialDbcontext = socialDbcontext;
        }

        public async Task<Post> CreatePost(Post toCreate)
        {
            toCreate.DateCreated = DateTime.UtcNow;
            toCreate.LastModified = DateTime.UtcNow;

            await _socialDbcontext.AddAsync(toCreate);  // ✅ Use async version
            await _socialDbcontext.SaveChangesAsync();  // ✅ Await SaveChangesAsync

            return toCreate;
        }

        public void DeletePost(int postId)
        {
            var post = _socialDbcontext.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null) return;

            _socialDbcontext.Remove(post);

            _socialDbcontext.SaveChangesAsync();
        }

        public async Task<Post?> GetPostById(int postId)
        {
            var post = await _socialDbcontext.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null)
            {
                // Log a message or throw an exception if necessary
                Console.WriteLine($"Post with ID {postId} not found.");
            }

            return post;
        }

        public async Task<ICollection<Post>> GetPosts()
        {
            return await _socialDbcontext.Posts.ToListAsync();
        }

        public async Task<Post> UpdatePost(string updatedContent, int postId)
        {
            var post = _socialDbcontext.Posts.FirstOrDefault(x => x.Id == postId);
            post.LastModified = DateTime.Now;
            post.Content = updatedContent;
            await _socialDbcontext.SaveChangesAsync();
            return post;
        }
    }
}
